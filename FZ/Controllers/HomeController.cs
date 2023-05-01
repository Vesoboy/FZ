using FZ.CheckFZ;
using FZ.DB;
using FZ.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace FZ.Controllers
{
    public class HomeController : Controller
    {
        private static SiteMonitor _hosted;
        private DataContext db;
        public ILogger<SiteMonitor> Logger { get; }

        public HomeController(SiteMonitor hosted, DataContext context, ILogger<SiteMonitor> logger)
        {
            _hosted = hosted;
            db = context;
            Logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            return View(await db.Site.ToListAsync());
        }

        public IActionResult Start()
        {
            _hosted.StartAsync(CancellationToken.None);
            return new EmptyResult();
        }

        public IActionResult Stop()
        {
            _hosted.StopAsync(CancellationToken.None);
            return new EmptyResult();
        }

        public IActionResult AddSiteModal(DataBaseSite site)
        {
            db.Site.Add(site);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult EditModal (DataBaseSite site)
        {
            var editSite = db.Site.FirstOrDefault(r => r.Id == site.Id);
            if (editSite != null)
            {
                editSite.Url = site.Url;
                editSite.Message = site.Message;
            }
            db.Site.Update(editSite);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult DeleteModal (DataBaseSite site)
        {
            var delSite = db.Site.FirstOrDefault(p => p.Id == site.Id);
            if (delSite != null)
            {
                db.Site.Remove(delSite);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public IActionResult ActiveTrue(DataBaseSite site)
        {
            var siteTrue = db.Site.FirstOrDefault(p => p.Id == site.Id);
            if (siteTrue != null)
            {
                siteTrue.Active = true;
                siteTrue.RetryCount = 0;
                Logger.LogInformation ($"Сайт {site.Url} был запущен вручную, связь в телеграм {site.Message}");
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public IActionResult ActiveFalse(DataBaseSite site)
        {
            var siteFalse = db.Site.FirstOrDefault(p => p.Id == site.Id);
            if (siteFalse != null)
            {
                siteFalse.Active = false;
                Logger.LogInformation($"Сайт {site.Url} был остановлен вручную, связь в телеграм {site.Message}");
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
