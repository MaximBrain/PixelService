using System.Net;
using Microsoft.AspNetCore.Http;
using PixelService.Services;

namespace PixelService.Tests;

[TestFixture]
public class HttpContextHandlerTests
{
    private HttpContextHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _handler = new HttpContextHandler();
    }

    [Test]
    public void GetHttpRequestInfo_AllInfoAvailable_ReturnsCorrectHttpRequestInfo()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Headers["Referer"] = "http://example.com";
        context.Request.Headers["User-Agent"] = "TestAgent";
        context.Connection.RemoteIpAddress = IPAddress.Parse("127.0.0.1");

        // Act
        var result = _handler.GetHttpRequestInfo(context);

        // Assert
        Assert.That(result.Referrer, Is.EqualTo("http://example.com"));
        Assert.That(result.UserAgent, Is.EqualTo("TestAgent"));
        Assert.That(result.IpAddress, Is.EqualTo("127.0.0.1"));
    }

    [Test]
    public void GetHttpRequestInfo_NoRefererInHeader_ReturnsCorrectHttpRequestInfo()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Headers["User-Agent"] = "TestAgent";
        context.Connection.RemoteIpAddress = IPAddress.Parse("127.0.0.1");

        // Act
        var result = _handler.GetHttpRequestInfo(context);

        // Assert
        Assert.That(result.Referrer, Is.EqualTo(string.Empty));
        Assert.That(result.UserAgent, Is.EqualTo("TestAgent"));
        Assert.That(result.IpAddress, Is.EqualTo("127.0.0.1"));
    }

    [Test]
    public void GetHttpRequestInfo_NoUserAgentInHeader_ReturnsCorrectHttpRequestInfo()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Headers["Referer"] = "http://example.com";
        context.Connection.RemoteIpAddress = IPAddress.Parse("127.0.0.1");

        // Act
        var result = _handler.GetHttpRequestInfo(context);

        // Assert
        Assert.That(result.Referrer, Is.EqualTo("http://example.com"));
        Assert.That(result.UserAgent, Is.EqualTo(string.Empty));
        Assert.That(result.IpAddress, Is.EqualTo("127.0.0.1"));
    }

    [Test]
    public void GetHttpRequestInfo_NoIpAddress_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Headers["Referer"] = "http://example.com";
        context.Request.Headers["User-Agent"] = "TestAgent";

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _handler.GetHttpRequestInfo(context));
    }
}