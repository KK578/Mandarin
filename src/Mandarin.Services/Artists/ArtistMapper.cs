using System.Collections.Generic;
using Mandarin.Models.Artists;
using Mandarin.Models.Commissions;

namespace Mandarin.Services.Artists
{
    /// <summary>
    /// Represents the converter between Artist related DTOs to Domain models.
    /// </summary>
    internal static class ArtistMapper
    {
        /// <summary>
        /// Converts the provided <see cref="ArtistDto"/> to an <see cref="ArtistDetailsModel"/>.
        /// </summary>
        /// <param name="dto">The artist DTO to convert.</param>
        /// <returns>A newly created <see cref="ArtistDetailsModel"/>.</returns>
        public static Stockist ConvertToModel(ArtistDto dto)
        {
            return new Stockist
            {
                StockistId = -1,
                StockistCode = dto.StockistCode,
                StockistName = dto.StockistName,
                Description = dto.Description,
                Details = new StockistDetail
                {
                    EmailAddress = dto.EmailAddress,
                    FacebookHandle = dto.FacebookHandle,
                    ImageUrl = dto.ImageUrl,
                    InstagramHandle = dto.InstagramHandle,
                    TumblrHandle = dto.TumblrHandle,
                    TwitterHandle = dto.TwitterHandle,
                    WebsiteUrl = dto.WebsiteUrl,
                },
                Commissions = new List<Models.Commissions.Commission>
                {
                    new Models.Commissions.Commission
                    {
                        StartDate = dto.StartDate.GetValueOrDefault(),
                        EndDate = dto.EndDate.GetValueOrDefault(),
                        RateGroup = new CommissionRateGroup
                        {
                            Rate = dto.Rate,
                        },
                    },
                },
                StatusCode = "ACTIVE",
            };
        }
    }
}
