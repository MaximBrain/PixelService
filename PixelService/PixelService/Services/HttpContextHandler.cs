using PixelShared.Models;

namespace PixelService.Services;

public record HttpContextHandler()
{
    public HttpRequestInfo GetHttpRequestInfo(HttpContext context)
    {
        var referrer = context.Request.Headers["Referer"].ToString();
        var userAgent = context.Request.Headers["User-Agent"].ToString();
        var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? throw new InvalidOperationException("IP Address is missing");
        
        return new HttpRequestInfo(referrer, userAgent, ipAddress);
    }
}