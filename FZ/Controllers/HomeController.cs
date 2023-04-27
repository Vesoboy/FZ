using FZ.CheckFZ;
using FZ.DB;
using FZ.Models;
using FZ.WriteLogs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

        public HomeController(SiteMonitor hosted, DataContext context)
        {
            _hosted = hosted;
            db = context;
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

        public IActionResult AddSite()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddSite(DataBaseSite site)
        {
            db.Site.Add(site);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public IActionResult Edit(Guid id)
        {
            var site = db.Site.FirstOrDefault(p => p.Id == id);
            if (site != null)
                return View(site);

            return NotFound();
        }

        [HttpPost]
        public IActionResult Edit (DataBaseSite site)
        {
            db.Update(site);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Delete(Guid id)
        {
            var site = db.Site.FirstOrDefault(p => p.Id == id);
            if (site != null)
            {
                db.Site.Remove(site);
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
