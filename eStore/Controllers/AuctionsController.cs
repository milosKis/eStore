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
using eStore.Models;

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

            var auctions = _context.Auctions.Include(a => a.User).Where(a => a.Duration > 0);

            if (lowPrice != null)
            {
                auctions = auctions.Where(a => a.CurrentPrice >= lowPrice);
            }

            ViewBag.LowPrice = lowPrice;

            if (highPrice != null)
            {
                auctions = auctions.Where(a => a.CurrentPrice <= highPrice);
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

            List<Auction> newList;
            if (!String.IsNullOrEmpty(searchString))
            {
                newList = new List<Auction>();
                var words = searchString.Split(' ');

                foreach (var auction in auctions)
                {
                    foreach (var word in words)
                    {
                        if (auction.Name.ToLower().Contains(word.ToLower()))
                        {
                            newList.Add(auction);
                            break;
                        }
                    }
                }
            }
            else
            {
                newList = auctions.ToList();
            }

            int pageSize = 10;
            var size = _context.AppSettings.SingleOrDefault(s => s.Name == "NumOfItemsPerPage");
            if (size != null)
            {
                pageSize = Int32.Parse(size.Value);
            }
            
            int pageNumber = (page ?? 1);
            //return View(auctions.OrderBy(a => a.Name).ToPagedList(pageNumber, pageSize));
            return View(PagedListExtensions.ToPagedList(newList.OrderBy(a => a.DateTimeCreated), pageNumber, pageSize));

        }

        [Authorize(Roles = RoleName.MaintenanceManager)]
        public ActionResult IndexReady(string currentFilter, string searchString, int? lowPrice, int? highPrice, int? page)
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

            var auctions = _context.Auctions.Include(a => a.User).Where(a => a.State == AuctionState.Ready);

            if (lowPrice != null)
            {
                auctions = auctions.Where(a => a.CurrentPrice >= lowPrice);
            }

            ViewBag.LowPrice = lowPrice;

            if (highPrice != null)
            {
                auctions = auctions.Where(a => a.CurrentPrice <= highPrice);
            }

            ViewBag.HighPrice = highPrice;

            List<Auction> newList;
            if (!String.IsNullOrEmpty(searchString))
            {
                newList = new List<Auction>();
                var words = searchString.Split(' ');

                foreach (var auction in auctions)
                {
                    foreach (var word in words)
                    {
                        if (auction.Name.ToLower().Contains(word.ToLower()))
                        {
                            newList.Add(auction);
                            break;
                        }
                    }
                }
            }
            else
            {
                newList = auctions.ToList();
            }

            int pageSize = 10;
            var size = _context.AppSettings.SingleOrDefault(s => s.Name == "NumOfItemsPerPage");
            if (size != null)
            {
                pageSize = Int32.Parse(size.Value);
            }
            int pageNumber = (page ?? 1);

            return View(PagedListExtensions.ToPagedList(newList.OrderBy(a => a.DateTimeCreated), pageNumber, pageSize));
        }

        [Authorize(Roles = RoleName.MaintenanceManager)]
        public ActionResult Update(int id, string state)
        {
            var auction = _context.Auctions.Include(a => a.User).SingleOrDefault(a => a.Id == id);
            if (auction == null)
                return HttpNotFound();
            auction.State = state;
            auction.DateTimeOpened = DateTime.Now;
             _context.SaveChanges();
            
            return RedirectToAction("IndexReady", "Auctions");
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