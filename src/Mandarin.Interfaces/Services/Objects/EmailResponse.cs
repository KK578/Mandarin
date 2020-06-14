namespace Mandarin.Services.Objects
{
    /// <summary>
    /// Represents the status response of sending a new email.
    /// </summary>
    public class EmailResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmailResponse"/> class.
        /// </summary>
        /// <param name="statusCode">The status code from sending the email.</param>
        public EmailResponse(int statusCode) => this.StatusCode = statusCode;

        /// <summary>
        /// Gets the status code from sending the email.
        /// </summary>
        public int StatusCode { get; }

        /// <summary>
        /// Gets a value indicating whether sending the email was successful.
        /// </summary>
        public bool IsSuccess => this.StatusCode >= 200 && this.StatusCode < 400;
    }
}
