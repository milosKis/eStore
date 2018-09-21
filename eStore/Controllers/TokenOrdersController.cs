using eStore.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eStore.fonts.Models;
using Microsoft.AspNet.Identity;

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
        [Authorize]
        public ActionResult Index()
        {
            if (User.IsInRole(RoleName.MaintenanceManager))
                return HttpNotFound();

            var userId = User.Identity.GetUserId();
            var orders = _context.TokenOrders.Where(o => o.User.Id == userId).ToList();

            return View(orders);
        }

        [Authorize]
        public ActionResult Create(TokenOrderViewModel model)
        {
            if (User.IsInRole(RoleName.MaintenanceManager))
                return HttpNotFound();

            string CurrentCurrency =
                _context.AppSettings.SingleOrDefault(s => s.Name == eStore.fonts.Models.Constants.CurrentCurrency).Value;

            double TokenValue =
                Convert.ToDouble(_context.AppSettings.SingleOrDefault(s => s.Name == eStore.fonts.Models.Constants.TokenValue).Value);

            int SilverCount =
                Int32.Parse(_context.AppSettings.SingleOrDefault(s => s.Name == eStore.fonts.Models.Constants.Silver).Value);

            int GoldCount =
                Int32.Parse(_context.AppSettings.SingleOrDefault(s => s.Name == eStore.fonts.Models.Constants.Gold).Value);

            int PlatinumCount =
                Int32.Parse(_context.AppSettings.SingleOrDefault(s => s.Name == eStore.fonts.Models.Constants.Platinum).Value);

            ViewBag.CurrentCurrency = CurrentCurrency;
            ViewBag.SilverCount = SilverCount;
            ViewBag.GoldCount = GoldCount;
            ViewBag.PlatinumCount = PlatinumCount;
            ViewBag.SilverPrice = SilverCount * TokenValue;
            ViewBag.GoldPrice = GoldCount * TokenValue;
            ViewBag.PlatinumPrice = PlatinumCount * TokenValue;

            if (!ModelState.IsValid)
            {
                return View("Create", model);
            }

            var userId = User.Identity.GetUserId();
            ApplicationUser user = _context.Users.SingleOrDefault(u => u.Id == userId);

            TokenOrder order = new TokenOrder
            {
                Id = Guid.NewGuid(),
                NumOfTokens = model.SilverCount * SilverCount + model.GoldCount * GoldCount + model.PlatinumCount * PlatinumCount,
                Price = (model.SilverCount * SilverCount + model.GoldCount * GoldCount + model.PlatinumCount * PlatinumCount) * TokenValue,
                Currency = CurrentCurrency,
                State = TokenOrderState.Submitted,
                DateTimeCreated = DateTime.Now,
                User = user
            };

            _context.TokenOrders.Add(order);
            _context.SaveChanges();

            return Redirect("http://stage.centili.com/payment/widget?apikey=162e68d0383d8eac6835fffac0759ec5&country=rs&reference=" + order.Id + "&returnurl=http://localhost:62040/TokenOrders/FinishedPaying");
            //return RedirectToAction("New");
        }

        [Authorize]
        public ActionResult New()
        {
            if (User.IsInRole(RoleName.MaintenanceManager))
                return HttpNotFound();

            string CurrentCurrency =
                _context.AppSettings.SingleOrDefault(s => s.Name == eStore.fonts.Models.Constants.CurrentCurrency).Value;

            double TokenValue =
                Convert.ToDouble(_context.AppSettings.SingleOrDefault(s => s.Name == eStore.fonts.Models.Constants.TokenValue).Value);

            int SilverCount =
                Int32.Parse(_context.AppSettings.SingleOrDefault(s => s.Name == eStore.fonts.Models.Constants.Silver).Value);

            int GoldCount =
                Int32.Parse(_context.AppSettings.SingleOrDefault(s => s.Name == eStore.fonts.Models.Constants.Gold).Value);

            int PlatinumCount =
                Int32.Parse(_context.AppSettings.SingleOrDefault(s => s.Name == eStore.fonts.Models.Constants.Platinum).Value);

            ViewBag.CurrentCurrency = CurrentCurrency;
            ViewBag.SilverCount = SilverCount;
            ViewBag.GoldCount = GoldCount;
            ViewBag.PlatinumCount = PlatinumCount;
            ViewBag.SilverPrice = SilverCount * TokenValue;
            ViewBag.GoldPrice = GoldCount * TokenValue;
            ViewBag.PlatinumPrice = PlatinumCount * TokenValue;

            TokenOrderViewModel model = new TokenOrderViewModel
            {
                GoldCount = 0,
                SilverCount = 0,
                PlatinumCount = 0
            };
            return View("Create", model);
        }


        [Authorize]
        public ActionResult FinishedPaying(Guid? reference, string status)
        {
            TokenOrder order = _context.TokenOrders.SingleOrDefault(o => o.Id == reference);
            var userId = User.Identity.GetUserId();

            if (order == null)
            {
                return RedirectToAction("Index");
            }

            if (status == "success")
            {
                ApplicationUser user = _context.Users.SingleOrDefault(u => u.Id == userId);
                if (user == null)
                    return View("Index");
                user.NumOfTokens += order.NumOfTokens;
                order.State = TokenOrderState.Completed;
                _context.SaveChanges();
            }
            else
            {
                order.State = TokenOrderState.Canceled;
                _context.SaveChanges();              
            }

            return RedirectToAction("Index");
        }

    }
}