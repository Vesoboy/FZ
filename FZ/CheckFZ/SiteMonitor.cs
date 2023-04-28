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
using System.Security.Policy;

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
            var delSite = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DelSite");
            Directory.CreateDirectory(delSite);
            var fileSiteDel = Path.Combine(delSite, $"Удаленные сайты {DateTime.UtcNow.ToString("yyyy-MM-dd_HH-mm-ss")}.txt");

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
                            HttpResponseMessage response = await httpClient.GetAsync(site.Url, stoppingToken);


                            if (!response.IsSuccessStatusCode)
                            {
                                _logger.LogError($"Сайт {site.Url} не доступен. StatusCode={response.StatusCode}");

                                var folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, site.Id.ToString());
                                Directory.CreateDirectory(folderPath);

                                var filePath = Path.Combine(folderPath, $"{DateTime.UtcNow.ToString("yyyy-MM-dd_HH-mm-ss")}.txt");
                                await File.WriteAllTextAsync(filePath, $"Сайт {site.Url} не доступен. StatusCode={response.StatusCode}");

                                if (site.RetryCount < 4)
                                {
                                    site.RetryCount++;

                                    _logger.LogInformation($"Попытка перезагрузить сайт {site.Url}. RetryCount={site.RetryCount}");

                                    //Скрипт который настраивает пользователь
                                    // Run site reload script here

                                    _context.Update(site);
                                    await _context.SaveChangesAsync(stoppingToken);
                                }

                                else 
                                {
                                    _logger.LogWarning($"Сайт {site.Url} не перезагрузился после {site.RetryCount} попыток. Уведомить в телеграмме {site.Message}");
                                    var message = $"Сайт {site.Url} не удалось перезапустить после {site.RetryCount} попыток. Пожалуйста, вручную перезапустите сайт.";
                                    var sendTelegram = new SendMessage();
                                    await sendTelegram.SendMessageAsync(message, site.Message);

                                    await File.AppendAllTextAsync(fileSiteDel, $"Сайт {site.Url} удален. Контакт для связи {site.Message}. StatusCode={response.StatusCode}\n");
                                    
                                    _context.Remove(site);
                                    _context.SaveChanges();
                                    //await _context.SaveChangesAsync(stoppingToken);
                                }

                            }

                            else
                            {
                                _logger.LogInformation($"Сайт {site.Url} работает коректно.");

                                if (site.RetryCount > 0)
                                {
                                    site.RetryCount = 0;

                                    _logger.LogInformation($"Сайт {site.Url} перезапущен. RetryCount={site.RetryCount}");

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