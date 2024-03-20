using PixelShared.Models;

namespace StorageService.Services;

public record LogService
{
    public string GetLogEntry(HttpRequestInfo? httpRequestInfo)
    {
        var s = $"{DateTime.UtcNow.ToString("s", System.Globalization.CultureInfo.InvariantCulture)} | " +
                $"{httpRequestInfo.Referrer ?? "null"} | {httpRequestInfo.UserAgent ?? "null"} | {httpRequestInfo.IpAddress}" + Environment.NewLine;
        return s;
    }
}