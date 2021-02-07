using Amlakbashi.Application.Services;
using Amlakbashi.Application.Services.UserServices.Interfaces;
using Amlakbashi.Host.Models;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Amlakbashi.Host.Controllers
{
    public class HomeController : Controller
    {
        //private readonly ILogger<HomeController> _logger;
        private readonly ILog logger;

        public HomeController(ILog logger)
        {
            this.logger = logger;
        }

        public IActionResult Index()
        {
            logger.Info("log4net config test");
            return View("Index", "test");
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
