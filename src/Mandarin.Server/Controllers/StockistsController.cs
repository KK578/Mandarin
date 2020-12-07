using System.Collections.Generic;
using System.Net;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Mandarin.Models.Artists;
using Mandarin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mandarin.Server.Controllers
{
    /// <summary>
    /// MVC Controller for RESTful interactions with <see cref="Stockist"/>.
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class StockistsController : ControllerBase
    {
        private readonly IArtistService artistService;

        /// <summary>
        /// Initializes a new instance of the <see cref="StockistsController"/> class.
        /// </summary>
        /// <param name="artistService">The service for interacting with stockists.</param>
        public StockistsController(IArtistService artistService)
        {
            this.artistService = artistService;
        }

        /// <summary>
        /// Gets the stockist's details for the stockist with the specified code.
        /// </summary>
        /// <param name="id">The artist's unique code.</param>
        /// <returns>A <see cref="Task{TResult}"/> containing the specified stockist, otherwise null if not found.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Stockist), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<ActionResult<Stockist>> GetStockistByCode(string id)
        {
            var artist = await this.artistService.GetArtistByCodeAsync(id);
            return artist != null ? this.Ok(artist) : this.NoContent();
        }

        /// <summary>
        /// Gets the full set of stockists who are active for commission purposes.
        /// </summary>
        /// <returns>A <see cref="Task"/> containing the list of stockists to consider for commission.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<Stockist>), 200)]
        public async Task<ActionResult<IReadOnlyList<Stockist>>> GetStockists()
        {
            return this.Ok(await this.artistService.GetArtistsForCommissionAsync());
        }
    }
}
