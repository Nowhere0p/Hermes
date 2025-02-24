namespace Hermes.Common
{
    public class HermesException : Exception
    {
        public const int BadRequest = 400;
        public const int Unauthorized = 401;
        public const int Forbidden = 403;
        public const int NotFound = 404;
        public const int InternalServerError = 500;
        public int StatusCode { get; }

        public HermesException(int statusCode, string message, string? details = null)
            : base(message)
        {
            StatusCode = statusCode;
            Details = details;
        }

        public string? Details { get; }
    }
}