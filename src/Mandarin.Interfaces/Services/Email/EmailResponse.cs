namespace Mandarin.Services.Email
{
    public class EmailResponse
    {
        public EmailResponse(int statusCode) => this.StatusCode = statusCode;

        public int StatusCode { get; }

        public bool IsSuccess => this.StatusCode >= 200 && this.StatusCode < 400;
    }
}
