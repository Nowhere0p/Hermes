namespace Hermes.Common;
public class HermesException(int statusCode, string message, string? details = null) : Exception(message)
{
    public const int BadRequest = 4000;
    public const int Unauthorized = 4001;
    public const int Forbidden = 4003;
    public const int NotFound = 4004;
    public const int InternalServerError = 5000;
    public int StatusCode { get; } = statusCode;

    public string? Details { get; } = details;
}