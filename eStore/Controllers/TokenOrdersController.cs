using eStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eStore.fonts.Models;

namespace eStore.Controllers
{
    public class TokenOrdersController : Controller
    {
        private ApplicationDbContext _context;

        public TokenOrdersController()
        {
            _context = new ApplicationDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        // GET: TokenOrders
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult Create()
        {
            if (User.IsInRole(RoleName.MaintenanceManager))
                return HttpNotFound();

            string CurrentCurrency =
                _context.AppSettings.SingleOrDefault(s => s.Name == Constants.CurrentCurrency).Value;

            double TokenValue =
                Int32.Parse(_context.AppSettings.SingleOrDefault(s => s.Name == Constants.TokenValue).Value);

            int SilverCount =
                Int32.Parse(_context.AppSettings.SingleOrDefault(s => s.Name == Constants.Silver).Value);

            int GoldCount =
                Int32.Parse(_context.AppSettings.SingleOrDefault(s => s.Name == Constants.Gold).Value);

            int PlatinumCount =
                Int32.Parse(_context.AppSettings.SingleOrDefault(s => s.Name == Constants.Platinum).Value);

            ViewBag.CurrentCurrency = CurrentCurrency;
            ViewBag.SilverCount = SilverCount;
            ViewBag.GoldCount = GoldCount;
            ViewBag.PlatinumCount = PlatinumCount;
            ViewBag.SilverPrice = SilverCount * TokenValue;
            ViewBag.GoldPrice = GoldCount * TokenValue;
            ViewBag.PlatinumPrice = PlatinumCount * TokenValue;
            return View();
        }
    }
}