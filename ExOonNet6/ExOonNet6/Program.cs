using ExOonNet6;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddOptions<ExOSettingsOptions>().Bind(builder.Configuration.GetSection("ExoSettings"));
builder.Services.AddTransient<IExchangeOnlineService, ExchangeOnlineService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/getexomailbox", (IExchangeOnlineService exchangeOnlineService, ILogger<Program> logger) =>
{
    var (err, result) = exchangeOnlineService.GetExoMailbox();
    return new ExoResult(err, result);
})
.WithName("GetExoMailbox");

app.Run();

internal record ExoResult(string Error, string Result);