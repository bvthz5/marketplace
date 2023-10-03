using System.ComponentModel.DataAnnotations;

namespace MarketPlaceAdmin.Bussiness.Dto.Forms
{
    /// <summary>
    /// Represents a form that corresponds to request approval.
    /// </summary>
    public class RequestForm
    {
        /// <summary>
        /// Value indicating whether the request is eligible for approval.
        /// </summary>
        public bool Approved { get; set; }

        /// <summary>
        /// The reason for approving or rejecting a request.
        /// </summary>
        /// <remarks>
        /// This property is only relevant if <see cref="Approved"/> is false.
        /// </remarks>
        [StringLength(255)]
        public string? Reason { get; set; }
    }
}
