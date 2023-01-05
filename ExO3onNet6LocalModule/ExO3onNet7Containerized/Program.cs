using ExO3PsLib;
using System.Management.Automation;
using System.Security.Cryptography.X509Certificates;

namespace ExO3onNet7Containerized
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddScoped<IExchangeOnlinePowerShellFactory, ExchangeOnlinePowerShellFactory>();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();


            app.MapPost("/getexomailboxwithpfx", async (ExOConnectionInformation connInfo,
                IExchangeOnlinePowerShellFactory exoPsFactory, IWebHostEnvironment env, ILogger<Program> logger) =>
            {
                byte[] data = Convert.FromBase64String(connInfo.PfxBase64);
                var cert = new X509Certificate2(data, connInfo.PfxPassword);

                var sw = System.Diagnostics.Stopwatch.StartNew();
                long elapsedConnect = 0, elapsedCmds = 0, elapsedTotal = 0;

                var modulePath = System.IO.Path.Combine(env.ContentRootPath, "_exo_module_unpacked/ExchangeOnlineManagement.psd1");
                using var ps = exoPsFactory.ConnectViaPool(connInfo.AppId, connInfo.Organization, cert, modulePath);
                elapsedConnect = sw.ElapsedMilliseconds;

                ps.Commands.AddCommand("Get-EXOMailBox").AddParameter("ResultSize", "unlimited");
                ICollection<PSObject> results = await ps.InvokeAsync().ConfigureAwait(false);

                elapsedCmds = sw.ElapsedMilliseconds;
                var err = ps.StreamsErrorToString();
                var result = ExchangeOnlineService.ResultsToSimpleString(results);

                ps.Commands.Clear();

                // Added because: "Win32Exception: Access is denied." in Windows container on InvokeAsync
                try
                {
                    ps.Commands.AddCommand("Disconnect-ExchangeOnline").AddParameter("Confirm", false);
                    var disconnectResult = await ps.InvokeAsync().ConfigureAwait(false);
                    var disconnectErr = ps.StreamsErrorToString();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to run disconnect command");
                }


                sw.Stop();
                elapsedTotal = sw.ElapsedMilliseconds;

                return new ExOResult(err, result, elapsedConnect, elapsedCmds, elapsedTotal);
            })
            .WithName("GetExoMailboxWithPfx");

            app.Run();
        }
    }

    public record ExOConnectionInformation(string Organization, string AppId, string PfxBase64, string PfxPassword);
}