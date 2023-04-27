using FZ.DB;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using System;
using Microsoft.EntityFrameworkCore;
using FZ;
using System.Diagnostics;
using FZ.CheckFZ;
using FZ.Bot;

namespace FZ.CheckFZ
{
    public class SiteMonitor : BackgroundService
    {
        private readonly ILogger<SiteMonitor> _logger;
        private readonly DataContext _context;
        private readonly IHttpClientFactory _httpClientFactory;

        public SiteMonitor(ILogger<SiteMonitor> logger, DataContext context, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _context = context;
            _httpClientFactory = httpClientFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var httpClient = new HttpClient())
                {
                    try
                    {
                        var sites = await _context.Site.ToListAsync(stoppingToken);

                        foreach (var site in sites)
                        {
                            var client = _httpClientFactory.CreateClient();
                            client.Timeout = TimeSpan.FromSeconds(20);
                            HttpResponseMessage response = await httpClient.GetAsync(site.url, stoppingToken);


                            if (!response.IsSuccessStatusCode)
                            {
                                _logger.LogError($"Сайт {site.url} не доступен. StatusCode={response.StatusCode}");

                                var folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, site.Id.ToString());
                                Directory.CreateDirectory(folderPath);

                                var filePath = Path.Combine(folderPath, $"{DateTime.UtcNow.ToString("yyyy-MM-dd_HH-mm-ss")}.txt");
                                await File.WriteAllTextAsync(filePath, $"Сайт {site.url} не доступен. StatusCode={response.StatusCode}");

                                if (site.RetryCount < 4)
                                {
                                    site.RetryCount++;

                                    _logger.LogInformation($"Попытка перезагрузить сайт {site.url}. RetryCount={site.RetryCount}");

                                    //Скрипт который настраивает пользователь
                                    // Run site reload script here

                                    _context.Update(site);
                                    await _context.SaveChangesAsync(stoppingToken);
                                }

                                else 
                                {
                                    _logger.LogWarning($"Сайт {site.url} не перезагрузился после {site.RetryCount} попыток. Уведомить в телеграмме {site.message}");
                                    var message = $"Сайт {site.url} не удалось перезапустить после {site.RetryCount} попыток. Пожалуйста, вручную перезапустите сайт.";
                                    var sendTelegram = new SendMessage();
                                    await sendTelegram.SendMessageAsync(message, site.message);

                                    //_context.Remove(site);
                                }

                            }

                            else
                            {
                                _logger.LogInformation($"Сайт {site.url} работает коректно.");

                                if (site.RetryCount > 0)
                                {
                                    site.RetryCount = 0;

                                    _logger.LogInformation($"Сайт {site.url} перезапущен. RetryCount={site.RetryCount}");

                                    _context.Update(site);
                                    await _context.SaveChangesAsync(stoppingToken);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Произошла ошибка при мониторинге сайтов.");
                    }
                }
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Фоновая задача остановлена");
            await base.StopAsync(stoppingToken);
        }

    }
}