using System.ComponentModel.DataAnnotations;

namespace MarketPlaceAdmin.Bussiness.Dto.Forms
{
    /// <summary>
    /// Represents a form with a date range, specified by a "From" and "To" date.
    /// </summary>
    public class FromToDateForm
    {
        /// <summary>
        /// Gets or sets the start date of the range.
        /// </summary>
        /// <remarks>
        /// This property is required and must be in a string format.
        /// </remarks>
        [Required]
        public string From { get; set; } = null!;

        /// <summary>
        /// Gets or sets the end date of the range.
        /// </summary>
        /// <remarks>
        /// This property is required and must be in a string format.
        /// </remarks>
        [Required]
        public string To { get; set; } = null!;
    }
}
