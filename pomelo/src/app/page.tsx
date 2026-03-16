import { auth0 } from "@/lib/auth0";
import { AuthGate } from "./components/AuthGate";

const Grid: React.FC<React.PropsWithChildren> = ({ children }) => {
  return (
    <div className="grid grid-cols-[repeat(auto-fill,minmax(300px,1fr))] gap-4">
      {children}
    </div>
  );
};

const LinkCard: React.FC<
  React.PropsWithChildren<{ href: string; title: string }>
> = ({ children, href, title }) => {
  return (
    <a href={href}>
      <div className="card bg-base-300">
        <div className="card-body">
          <h2 className="card-title text-xl">{title}</h2>
          {children}
        </div>
      </div>
    </a>
  );
};

export default async function Home() {
  const session = await auth0.getSession();

  if (!session) {
    return <AuthGate />;
  }

  return (
    <section className="h-full w-full p-4">
      <Grid>
        <div className="card bg-base-300 col-span-full">
          <div className="card-body">
            <h2 className="card-title text-xl">
              Welcome to the Admin Dashboard.
            </h2>
            <p>Hopefully all of this will start working soon...</p>
            <div className="card-actions">
              <button className="btn btn-primary">Primary</button>
              <button className="btn btn-secondary">Secondary</button>
              <button className="btn btn-accent">Accent</button>
            </div>
          </div>
        </div>
        <LinkCard href="/artists" title="Artists">
          <p>Add or manage artist details and commission rates.</p>
        </LinkCard>
        <LinkCard href="/inventory" title="Inventory">
          <p>Manage the product inventory and frame prices.</p>
        </LinkCard>
        <LinkCard href="/record-of-sales" title="Record of Sales">
          <p>View and Records of Sales and send emails.</p>
        </LinkCard>
      </Grid>
    </section>
  );
}
