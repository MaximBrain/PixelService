using PixelService.Services;
using PixelShared.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<HttpContextHandler>();
builder.Services.AddScoped<RabbitMqSender>();
builder.Services.Configure<RabbitMQConfiguration>(builder.Configuration.GetSection("RabbitMQ"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/track", async (HttpContext context, RabbitMqSender rabbitMqSender, HttpContextHandler httpContextHandler) =>
    {
        try
        {
            rabbitMqSender.SendMessageToQueue(httpContextHandler.GetHttpRequestInfo(context));
            
            context.Response.ContentType = "image/gif";
            const string base64Pixel = "R0lGODlhAQABAIAAAAAAAP///yH5BAEAAAAALAAAAAABAAEAAAIBRAA7";
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