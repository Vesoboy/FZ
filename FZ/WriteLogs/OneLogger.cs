using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Threading;
using System;
using Microsoft.Extensions.Hosting;
using FZ;
using System.Diagnostics;
using FZ.CheckFZ;

namespace FZ.WriteLogs
{
    public class OneLogger : BackgroundService
    {
        private readonly ILogger<OneLogger> _logger;
        private Timer _timer;
        private CheckSite _checkSite;
        public bool IsRunning { get; private set; }

        public OneLogger(ILogger<OneLogger> logger) 
        {
            _logger = logger;
            IsRunning = false;
            _checkSite = new CheckSite ("https://localhost:44315/", _logger);
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                IsRunning = true;
                
                // Check the site
                await _checkSite.CheckSiteAsync(stoppingToken);

                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
            IsRunning = false;
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            if (IsRunning)
            {
                _logger.LogInformation("Фоновая задача остановлена");
                _timer?.Dispose();
                IsRunning = false;
                await base.StopAsync(stoppingToken);
            }
        }
        public void SetIsRunning(bool value)
        {
            IsRunning = value;
        }
    }
}
