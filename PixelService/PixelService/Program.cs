using PixelService.Services;
using PixelShared.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<RabbitMQConfiguration>(builder.Configuration.GetSection("RabbitMQ"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/track", async (HttpContext context, ContextHandler ContextHandler) =>
    {
        try
        {
            const string base64Pixel = "R0lGODlhAQABAIAAAAAAAP///yH5BAEAAAAALAAAAAABAAEAAAIBRAA7";
            
            // Collect information
            var referrer = context.Request.Headers["Referer"].ToString();
            var userAgent = context.Request.Headers["User-Agent"].ToString();
            var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? throw new InvalidOperationException("IP Address is missing");
            
            ContextHandler.SendMessageToQueue(new HttpRequestInfo(referrer, userAgent, ipAddress));
            
            context.Response.ContentType = "image/gif";
            await context.Response.WriteAsync(base64Pixel, context.RequestAborted);
        } 
        catch (Exception ex)
        {
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync($"An error occurred: {ex.Message}");
        }
    })
    .WithName("GetPixel")
    .WithOpenApi();

app.Run();