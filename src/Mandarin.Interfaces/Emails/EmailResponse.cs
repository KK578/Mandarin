namespace Mandarin.Emails
{
    /// <summary>
    /// Represents the status response of sending a new email.
    /// </summary>
    public record EmailResponse
    {
        /// <summary>
        /// Gets a value indicating whether the email was sent successfully.
        /// </summary>
        public bool IsSuccess { get; init; }

        /// <summary>
        /// Gets a user displayable message on the status of the email.
        /// </summary>
        public string Message { get; init; }
    }
}
