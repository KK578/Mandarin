using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mandarin.Models.Commissions;
using Mandarin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mandarin.Server.Controllers
{
    /// <summary>
    /// MVC Controller for RESTful interactions with <see cref="CommissionRateGroup"/>.
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class CommissionsController : ControllerBase
    {
        private readonly ICommissionService commissionService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommissionsController"/> class.
        /// </summary>
        /// <param name="commissionService">The service for interacting with commission details.</param>
        public CommissionsController(ICommissionService commissionService)
        {
            this.commissionService = commissionService;
        }

        /// <summary>
        /// Gets the ordered list of all currently available <see cref="CommissionRateGroup"/>.
        /// </summary>
        /// <returns>The ordered list of <see cref="CommissionRateGroup"/>.</returns>
        [HttpGet("rates")]
        [ProducesResponseType(typeof(IReadOnlyList<CommissionRateGroup>), 200)]
        public Task<IReadOnlyList<CommissionRateGroup>> GetCommissionRateGroups()
        {
            return this.commissionService.GetCommissionRateGroups();
        }
    }
}
