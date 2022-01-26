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

app.MapGet("/getexomailbox", async (IExchangeOnlineService exchangeOnlineService, ILogger<Program> logger) =>
{
    return await exchangeOnlineService.GetExoMailbox();
})
.WithName("GetExoMailbox");

// For eg Docker scenarios where you don't want to populate the ExoSettings.PfxPath property
app.MapGet("/getpfxinfo", (IExchangeOnlineService exchangeOnlineService) =>
{
    return exchangeOnlineService.GetPfxInfo();
})
.WithName("GetPfxInfo");

app.MapPost("/getexomailboxwithpfx", async (ExOPfx pfxInfo, IExchangeOnlineService exchangeOnlineService, ILogger<Program> logger) =>
{
    return await exchangeOnlineService.GetExoMailboxWithPfx(pfxInfo);
})
.WithName("GetExoMailboxWithPfx");

app.Run();