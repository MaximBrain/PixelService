using PixelShared.Models;

namespace StorageService.Services;

public record LogService
{
    public static string GetLogEntry(HttpRequestInfo? requestInfo)
    {
        if (requestInfo == null)
        {
            throw new InvalidOperationException("Invalid message body");
        }
        
        return $"{DateTime.UtcNow.ToString("s", System.Globalization.CultureInfo.InvariantCulture)} | " +
                $"{(string.IsNullOrEmpty(requestInfo.Referrer) ? "null" : requestInfo.Referrer)} | " +
                $"{(string.IsNullOrEmpty(requestInfo.UserAgent) ? "null" : requestInfo.UserAgent)} | " +
                $"{requestInfo.IpAddress}" + Environment.NewLine;
    }
}