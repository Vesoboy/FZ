using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FZ.Controllers;
using FZ.FZSite;
using FZ.WriteLogs;
using Microsoft.Extensions.Logging;
using static NLog.LayoutRenderers.Wrappers.ReplaceLayoutRendererWrapper;

namespace FZ.CheckFZ
{
    public class CheckSite
    {
        private readonly string _siteUrl;
        private readonly ILogger<OneLogger> _logger;
        private int _restartAttempts;

        public CheckSite(string siteUrl, ILogger<OneLogger> logger)
        {
            _siteUrl = siteUrl;
            _logger = logger;
            _restartAttempts = 0;
        }

        public async Task CheckSiteAsync(CancellationToken cancelToken)
        {
            using (var httpClient = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await httpClient.GetAsync(_siteUrl);

                    if (!response.IsSuccessStatusCode)
                    {
                        // Произошла ошибка на сайте
                        _logger.LogError("Произошла ошибка на сайте.");

                        // Попытка перезапустить сайт
                        if (_restartAttempts < 1)
                        {
                            _restartAttempts++;
                            _logger.LogInformation($"Перезапуск сайта (попытка {_restartAttempts})...");

                            //Запустите скрипт для перезапуска сайта (замените своим скриптом)
                            var scriptPath = "/path/to/script.sh";
                            var process2 = Process.Start(new ProcessStartInfo
                            {
                                FileName = "sh",
                                Arguments = scriptPath,
                                RedirectStandardOutput = true,
                                UseShellExecute = false,
                                CreateNoWindow = true
                            });

                            await process2.WaitForExitAsync();

                            // Подождите, пока сайт снова запустится, прежде чем продолжить
                            await Task.Delay(TimeSpan.FromSeconds(30));

                            _logger.LogInformation("Сайт успешно перезапущен.");
                        }
                        else
                        {
                            // Достигнуто максимальное количество попыток перезапуска —
                            // уведомите администратора о необходимости перезапустить сайт вручную.
                            _logger.LogWarning("Достигнуто максимальное количество попыток перезапуска. Пожалуйста, перезапустите сайт вручную.");

                            // SОтправьте электронное письмо администратору
                            // (замените свой собственный код отправки электронной почты)
                            var message = "Сайт не удалось перезапустить после 5 попыток. Пожалуйста, вручную перезапустите сайт.";
                            _ = EmailService.SendEmailAsync("vessolair@gmail.com", "Сайт мониторинг", message);

                        }
                    }
                    else _logger.LogInformation("Сайт работает корректно.");
                }
                catch (Exception ex)
                {
                    // Исключение при попытке доступа к сайту
                    _logger.LogError($"Исключение произошло при попытке доступа к сайту. Сообщение об исключении: {ex.Message}");
                }
            }
        }
    }
}
