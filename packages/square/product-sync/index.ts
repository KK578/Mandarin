require("dotenv").config();
import { SquareClient, SquareEnvironment } from "square";
import { Pool } from "pg";

let pool: Pool;
let squareClient: SquareClient;

function initClients() {
  if (!pool) {
    pool = new Pool({
      connectionString: process.env.DATABASE_URL,
    });
  }
  if (!squareClient) {
    squareClient = new SquareClient({
      environment: process.env.SQUARE_ENVIRONMENT === "Production" ? SquareEnvironment.Production : SquareEnvironment.Sandbox,
      token: process.env.SQUARE_API_KEY,
    });
  }
}

function getProductName(catalogItemName: string): string {
  const squareBracketMatch = /^\[.*\] (?<name>.*)$/.exec(catalogItemName);
  if (squareBracketMatch && squareBracketMatch.groups) {
    return squareBracketMatch.groups.name;
  }

  const hyphenMatch = /^.* - (?<name>.*)$/.exec(catalogItemName);
  if (hyphenMatch && hyphenMatch.groups) {
    return hyphenMatch.groups.name;
  }

  return catalogItemName;
}

export class SquareProductReader {
  private client: SquareClient;

  constructor(client: SquareClient) {
    this.client = client;
  }

  async *getProducts(): AsyncGenerator<any, void, unknown> {
    let cursor: string | undefined;

    do {
      const response = await this.client.catalog.search({
        objectTypes: ["ITEM", "ITEM_VARIATION"],
        includeDeletedObjects: true,
        cursor: cursor,
      });

      const catalog = response.objects || [];

      const items = catalog.filter((obj: any) => obj.type === "ITEM");
      const variations = catalog.filter((obj: any) => obj.type === "ITEM_VARIATION");

      for (const item of items as any[]) {
        const itemVariations = variations.filter((v: any) => v.itemVariationData?.itemId === item.id);

        for (const variation of itemVariations as any[]) {
          if (!variation.itemVariationData?.sku) {
            continue;
          }

          const price = variation.itemVariationData.priceMoney;
          const unitPrice = price && price.amount ? Number(price.amount) / 100 : null;

          const productName = getProductName(item.itemData?.name || "");

          yield {
            productId: variation.id,
            productCode: variation.itemVariationData.sku,
            productName: `${productName} (${variation.itemVariationData.name})`,
            description: item.itemData?.description,
            unitPrice: unitPrice,
            lastUpdated: variation.updatedAt,
          };
        }
      }

      cursor = response.cursor;
    } while (cursor);
  }
}

export async function main(args: any) {
  initClients();
  let updateCount = 0;
  console.log("Starting Square product synchronisation.");

  try {
    const reader = new SquareProductReader(squareClient);

    for await (const product of reader.getProducts()) {
      const res = await pool.query("SELECT * FROM inventory.product WHERE product_id = $1", [product.productId]);
      const existingProduct = res.rows[0];

      let shouldUpdate = false;
      if (!existingProduct) {
        console.log(`Inserting new product: ${JSON.stringify(product)}`);
        shouldUpdate = true;
      } else {
        const areEquivalent = existingProduct.product_id === product.productId &&
                              existingProduct.product_code === product.productCode &&
                              existingProduct.product_name === product.productName &&
                              existingProduct.description === product.description &&
                              Number(existingProduct.unit_price) === product.unitPrice &&
                              new Date(existingProduct.last_updated).toISOString() === new Date(product.lastUpdated).toISOString();

        if (!areEquivalent) {
           console.log(`Updating ${product.productId} to new version: ${JSON.stringify(product)}`);
           shouldUpdate = true;
        }
      }

      if (shouldUpdate) {
        await pool.query(
          "CALL inventory.sp_product_upsert($1, $2, $3, $4, $5, $6)",
          [product.productId, product.productCode, product.productName, product.description, product.unitPrice, product.lastUpdated]
        );
        updateCount++;
      }
    }

    console.log(`Finished product synchronisation - Update Count: ${updateCount}.`);
    return {
      body: { message: `Successfully synchronized ${updateCount} products.` }
    };
  } catch (error) {
    console.error("Error synchronizing products:", error);
    return {
      statusCode: 500,
      body: { error: "Failed to synchronize products." }
    };
  }
}
