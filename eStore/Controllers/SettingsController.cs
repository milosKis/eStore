using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DotNetNuke.Web.InternalServices;
using eStore.fonts.Models;
using eStore.Models;
using Microsoft.Owin.Security.Infrastructure;

namespace eStore.Controllers
{
    public class SettingsController : Controller
    {
        private ApplicationDbContext _context;

        public SettingsController()
        {
            _context = new ApplicationDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        // GET: Settings
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = RoleName.MaintenanceManager)]
        public ActionResult Update()
        {
            UpdateSettingsViewModel model = new UpdateSettingsViewModel();
            model.Message = "";
            model.Duration =
                Int32.Parse((_context.AppSettings.SingleOrDefault(s => s.Name == Constants.Duration)).Value);
            model.NumOfItemsPerPage =
                Int32.Parse((_context.AppSettings.SingleOrDefault(s => s.Name == Constants.NumOfItemsPerPage)).Value);
            model.TokenValue =
                Convert.ToDouble((_context.AppSettings.SingleOrDefault(s => s.Name == Constants.TokenValue)).Value);
            List<SelectListItem> list = new List<SelectListItem>();
            string CurrentCurrency = _context.AppSettings.SingleOrDefault(s => s.Name == Constants.CurrentCurrency).Value;
            model.CurrentCurrency = CurrentCurrency;
            model.Currencies = CurrentCurrency;
            model.Silver = Int32.Parse((_context.AppSettings.SingleOrDefault(s => s.Name == Constants.Silver)).Value);
            model.Gold = Int32.Parse((_context.AppSettings.SingleOrDefault(s => s.Name == Constants.Gold)).Value);
            model.Platinum = Int32.Parse((_context.AppSettings.SingleOrDefault(s => s.Name == Constants.Platinum)).Value);

            var currencies = _context.AppSettings.ToList();
            foreach (var currency in currencies)
            {
                if ((currency.Name == Constants.Currency) && (currency.Value != CurrentCurrency))
                {
                    model.Currencies += " " + currency.Value;
                }
            }
            

            return View("Update", model);
        }


        [Authorize(Roles = RoleName.MaintenanceManager)]
        public ActionResult UpdateAll(UpdateSettingsViewModel model)
        {
            string CurrentCurrency = _context.AppSettings.SingleOrDefault(s => s.Name == Constants.CurrentCurrency).Value;
            model.Currencies = CurrentCurrency;
            var currencies = _context.AppSettings.ToList();
            foreach (var currency in currencies)
            {
                if ((currency.Name == Constants.Currency) && (currency.Value != CurrentCurrency))
                {
                    model.Currencies += " " + currency.Value;
                }
            }

            if (!ModelState.IsValid)
            {
                return View("Update", model);
            }

            AppSetting appSetting = _context.AppSettings.SingleOrDefault(s => s.Name == Constants.CurrentCurrency);
            appSetting.Value = model.CurrentCurrency;
            _context.SaveChanges();

            appSetting = _context.AppSettings.SingleOrDefault(s => s.Name == Constants.Duration);
            appSetting.Value = model.Duration + "";
            _context.SaveChanges();

            appSetting = _context.AppSettings.SingleOrDefault(s => s.Name == Constants.TokenValue);
            appSetting.Value = model.TokenValue + "";
            _context.SaveChanges();

            appSetting = _context.AppSettings.SingleOrDefault(s => s.Name == Constants.NumOfItemsPerPage);
            appSetting.Value = model.NumOfItemsPerPage + "";
            _context.SaveChanges();

            appSetting = _context.AppSettings.SingleOrDefault(s => s.Name == Constants.Silver);
            appSetting.Value = model.Silver + "";
            _context.SaveChanges();

            appSetting = _context.AppSettings.SingleOrDefault(s => s.Name == Constants.Gold);
            appSetting.Value = model.Gold + "";
            _context.SaveChanges();

            appSetting = _context.AppSettings.SingleOrDefault(s => s.Name == Constants.Platinum);
            appSetting.Value = model.Platinum + "";
            _context.SaveChanges();

            model.Message = "Saved!";

            return View("Update", model);

        }
    }
}