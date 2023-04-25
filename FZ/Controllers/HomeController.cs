using FZ.Models;
using FZ.WriteLogs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FZ.Controllers
{
    public class HomeController : Controller
    {
        private static OneLogger _hosted;

        public ILogger<OneLogger> Logger { get; }

        public HomeController(OneLogger hosted)
        {
            _hosted = hosted;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Start()
        {
            _hosted.StartAsync(CancellationToken.None);
            //return RedirectToAction("Ind");
            return new EmptyResult();
        }

        public IActionResult Stop()
        {
            _hosted.StopAsync(CancellationToken.None);
            return new EmptyResult();
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
