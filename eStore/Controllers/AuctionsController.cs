using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eStore.Models;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using Microsoft.Ajax.Utilities;
using PagedList;

namespace eStore.Controllers
{
    public class AuctionsController : Controller
    {
        private ApplicationDbContext _context;

        public AuctionsController()
        {
            _context = new ApplicationDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        // GET: Auctions
        public ActionResult Index(string currentFilter, string searchString, int? lowPrice, int? highPrice, string state, int? page)
        {
            

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var auctions = _context.Auctions.Where(a => a.Duration > 0);

            if (!String.IsNullOrEmpty(searchString))
            {
                auctions = auctions.Where(a => a.Name.Contains(searchString));
            }

            if (lowPrice != null)
            {
                auctions = auctions.Where(a => a.CurrentPrice >= lowPrice);
            }

            ViewBag.LowPrice = lowPrice;

            if (highPrice != null)
            {
                auctions = auctions.Where(a => a.CurrentPrice >= highPrice);
            }

            ViewBag.HighPrice = highPrice;

            if (String.IsNullOrEmpty(state))
            {
                ViewBag.State = "ALL";
            }
            else
            {
                ViewBag.State = state;
                auctions = auctions.Where(a => a.State == state);
            }

            //auctions.OrderBy(a => a.Name);

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            //return View(auctions.OrderBy(a => a.Name).ToPagedList(pageNumber, pageSize));
            return View(PagedListExtensions.ToPagedList(auctions.OrderBy(a => a.Name), pageNumber, pageSize));

        }

        [Authorize(Roles = RoleName.MaintenanceManager)]
        public ActionResult IndexReady()
        {
            var auctions = _context.Auctions.Where(a => a.State == AuctionState.Ready).ToList();

            return View(auctions);
        }

        [Authorize]
        public ActionResult New()
        {
            return View("Create");
        }

        [Authorize]
        [HttpPost]
        public ActionResult Create(CreateAuctionViewModel auctionViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View("Create", auctionViewModel);
            }

            Auction auction = new Auction()
            {
                Name = auctionViewModel.Name,
                Duration = auctionViewModel.Duration,
                StartingPrice = auctionViewModel.StartingPrice,
                CurrentPrice = auctionViewModel.StartingPrice,
                State = AuctionState.Ready,
                DateTimeCreated = DateTime.Now,
        };

            var userId = User.Identity.GetUserId();
            ApplicationUser user = _context.Users.SingleOrDefault(u => u.Id == userId);
            auction.User = user;
            auction.Image = new byte[auctionViewModel.ImageFile.ContentLength];
            auctionViewModel.ImageFile.InputStream.Read(auction.Image, 0, auction.Image.Length);
            auction.CurrentPrice = auction.StartingPrice;
            auction.State = AuctionState.Ready;

            _context.Auctions.Add(auction);
            _context.SaveChanges(); 

            return View("Details", auction);
        }

        public ActionResult Details(int id)
        {
            var auction = _context.Auctions.Include(a => a.User).SingleOrDefault(a => a.Id == id);

            if (auction == null)
                return HttpNotFound();

            return View(auction);
        }
    }
}