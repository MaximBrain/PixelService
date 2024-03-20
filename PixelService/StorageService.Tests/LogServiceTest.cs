using PixelShared.Models;
using StorageService.Services;

namespace StorageService.Tests;

using NUnit.Framework;
using System;

[TestFixture]
public class LogServiceTests
{
    [Test]
    public void GetLogEntry_NullRequestInfo_ThrowsInvalidOperationException()
    {
        // Arrange 
        HttpRequestInfo? requestInfo = null;

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => LogService.GetLogEntry(requestInfo));
    }

    [Test]
    public void GetLogEntry_ValidRequestInfo_ReturnsCorrectFormat()
    {
        // Arrange 
        var requestInfo = new HttpRequestInfo(
        "testReferrer", "testUserAgent", "127.0.0.1");

        // Act
        string result = LogService.GetLogEntry(requestInfo);

        // Assert
        Assert.IsNotNull(result);
        StringAssert.Contains("testReferrer", result);
        StringAssert.Contains("testUserAgent", result);
        StringAssert.Contains("127.0.0.1", result);
    }

    [Test]
    public void GetLogEntry_EmptyFieldsInRequestInfo_ReturnsNullsInOutput()
    {
        // Arrange 
        var requestInfo = new HttpRequestInfo(string.Empty,string.Empty,"127.0.0.1");

        // Act
        string result = LogService.GetLogEntry(requestInfo);

        // Assert
        Assert.IsNotNull(result);
        StringAssert.Contains("null", result);
        StringAssert.Contains("127.0.0.1", result);
    }
}