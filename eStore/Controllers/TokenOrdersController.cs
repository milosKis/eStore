using eStore.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eStore.fonts.Models;
using Microsoft.AspNet.Identity;
using eStore.Hubs;

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
            var user = _context.Users.Where(u => u.Id == userId).SingleOrDefault();
            if (user != null)
            {
                double numOfTokens = user.NumOfTokens;
                numOfTokens = Math.Round(numOfTokens, 2);
                ViewBag.NumOfTokens = numOfTokens;
            }

            return View(orders);
        }

        [Authorize]
        public ActionResult IndexCancel()
        {
            if (User.IsInRole(RoleName.MaintenanceManager))
                return HttpNotFound();
            ViewBag.Message = "Order is canceled!";

            var userId = User.Identity.GetUserId();
            var orders = _context.TokenOrders.Where(o => o.User.Id == userId).ToList();
            var user = _context.Users.Where(u => u.Id == userId).SingleOrDefault();
            if (user != null)
            {
                double numOfTokens = user.NumOfTokens;
                numOfTokens = Math.Round(numOfTokens, 2);
                ViewBag.NumOfTokens = numOfTokens;
            }

            return View("Index", orders);
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

            return Redirect("http://stage.centili.com/payment/widget?apikey=162e68d0383d8eac6835fffac0759ec5&country=rs&reference=" + order.Id + "&returnurl=http://km150066d.azurewebsites.net/TokenOrders/FinishedPaying");
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

            var userId = User.Identity.GetUserId();
            var user = _context.Users.Where(u => u.Id == userId).SingleOrDefault();
            if (user != null)
            {
                double numOfTokens = user.NumOfTokens;
                numOfTokens = Math.Round(numOfTokens, 2);
                ViewBag.NumOfTokens = numOfTokens;
            }
                

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

            ApplicationUser user = _context.Users.SingleOrDefault(u => u.Id == userId);
            if (user == null)
                return RedirectToAction("Index");

            if (status == "success")
            {
                user.NumOfTokens += order.NumOfTokens;
                order.State = TokenOrderState.Completed;
                var hub = Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager.GetHubContext<MyHub>();
                hub.Clients.All.updateNumOfTokens(userId, order.NumOfTokens);
                _context.SaveChanges();
                string title = "Token order!";
                string message = "You have just ordered " + order.NumOfTokens + " tokens!"; 
                string email = user.Email;
                //this is place for code that actually sends email
                Email.Send(email, title, message);
            }
            else
            {
                order.State = TokenOrderState.Canceled;
                ViewBag.Message = "Order is canceled!";
                _context.SaveChanges();
                return RedirectToAction("IndexCancel");
            }

            return RedirectToAction("Index");
        }

    }
}