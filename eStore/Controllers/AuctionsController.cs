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
using eStore.Hubs;

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
            
            CheckAuctions();  //checks if there are opened auctions that expired
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
                ViewBag.State = "All";
            }
            else
            {
                ViewBag.State = state;
                if (state != "All")
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
            if (Request.IsAuthenticated && !User.IsInRole(RoleName.MaintenanceManager))
            {
                var userId = User.Identity.GetUserId();
                ViewBag.NumOfTokens = _context.Users.SingleOrDefault(u => u.Id == userId).NumOfTokens;
                var tokenValue = eStore.fonts.Models.Constants.TokenValue;
                ViewBag.TokenValue = _context.AppSettings.SingleOrDefault(s => s.Name == tokenValue).Value;
            }
            //return View(auctions.OrderBy(a => a.Name).ToPagedList(pageNumber, pageSize));
            return View(PagedListExtensions.ToPagedList(newList.OrderByDescending(a => a.DateTimeCreated), pageNumber, pageSize));

        }


        [Authorize]
        public ActionResult IndexMy(string currentFilter, string searchString, int? lowPrice, int? highPrice, string state, int? page)
        {
            if (User.IsInRole(RoleName.MaintenanceManager))
            {
                return HttpNotFound();
            }

            var userId = User.Identity.GetUserId();

            CheckAuctions();  //checks if there are opened auctions that expired
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var auctions = _context.Auctions.
                Include(a => a.User).
                Include(a => a.LastBidder).
                Where(a => a.User.Id == userId);

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
                ViewBag.State = "All";
            }
            else
            {
                ViewBag.State = state;
                if (state != "All")
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
            if (Request.IsAuthenticated && !User.IsInRole(RoleName.MaintenanceManager))
            {
                //var userId = User.Identity.GetUserId();
                ViewBag.NumOfTokens = _context.Users.SingleOrDefault(u => u.Id == userId).NumOfTokens;
                var tokenValue = eStore.fonts.Models.Constants.TokenValue;
                ViewBag.TokenValue = _context.AppSettings.SingleOrDefault(s => s.Name == tokenValue).Value;
            }
            //return View(auctions.OrderBy(a => a.Name).ToPagedList(pageNumber, pageSize));
            return View(PagedListExtensions.ToPagedList(newList.OrderByDescending(a => a.DateTimeCreated), pageNumber, pageSize));

        }


        private void CheckAuctions()
        {
            List<Auction> auctions = _context.Auctions.Include(a => a.LastBidder).Where(a => a.State == AuctionState.Opened).ToList();
            foreach (var auction in auctions)
            {
                if ((DateTime.Now - auction.DateTimeOpened.Value).TotalSeconds > auction.Duration)
                {
                    auction.DateTimeClosed = DateTime.Now;
                    auction.State = AuctionState.Completed;
                    if (auction.LastBidder != null)
                    {
                        string title = "Auction win!";
                        string message = "Congratulations! You have won the " + auction.Name + "(id = " + auction.Id + ")!";
                        string email = auction.LastBidder.Email;
                        //this is place for code that actually sends email
               //        Email.Send(email, title, message);
                    }

                    _context.SaveChanges();
                   
                    

                }
            }
        }


        /*public void CompleteAuction(int id)
        {
            Auction auction = _context.Auctions.Include(a => a.LastBidder).SingleOrDefault(a => a.Id == id);
            if (auction != null)
            {
                auction.DateTimeClosed = DateTime.Now;
                auction.State = AuctionState.Completed;
                if (auction.LastBidder != null)
                {
                    string title = "Auction win!";
                    string message = "Congratulations! You have won the " + auction.Name + "(id = " + auction.Id + ")!";
                    string email = auction.LastBidder.Email;
                    Email.Send(auction.LastBidder.Email, title, message);
                }

                _context.SaveChanges();
            }
        }*/


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



        [Authorize]
        public ActionResult IndexWon(string currentFilter, string searchString, int? lowPrice, int? highPrice, int? page)
        {
            if (User.IsInRole(RoleName.MaintenanceManager))
            {
                return HttpNotFound();
            }

            CheckAuctions();

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var userId = User.Identity.GetUserId();
            var auctions = _context.Auctions.
                Include(a => a.User).
                Include(a => a.LastBidder).
                Where(a => a.LastBidder.Id == userId).
                Where(a => a.State == AuctionState.Completed);

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

            return View(PagedListExtensions.ToPagedList(newList.OrderByDescending(a => a.DateTimeCreated), pageNumber, pageSize));
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
            var hub = Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager.GetHubContext<MyHub>();
            long time = (long)(((DateTime)auction.DateTimeOpened).Ticks - DateTime.Now.Ticks) / TimeSpan.TicksPerMillisecond + auction.Duration * 1000;
            hub.Clients.All.updateStateAndTimer(auction.Id, time, auction.User.Id);

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
            auction.Currency = CurrentCurrency();

            _context.Auctions.Add(auction);

            _context.SaveChanges();
           
            

            return RedirectToAction("Details", new { id = auction.Id});
        }

        public ActionResult Details(int id)
        {
            CheckAuctions();
            AuctionDetailsModel model = new AuctionDetailsModel();
            var auction = _context.Auctions.Include(a => a.User).Include(a => a.LastBidder).SingleOrDefault(a => a.Id == id);
            if (Request.IsAuthenticated && !User.IsInRole(RoleName.MaintenanceManager))
            {
                var userId = User.Identity.GetUserId();
                ViewBag.NumOfTokens = _context.Users.SingleOrDefault(u => u.Id == userId).NumOfTokens;
                var tokenValue = eStore.fonts.Models.Constants.TokenValue;
                ViewBag.TokenValue = _context.AppSettings.SingleOrDefault(s => s.Name == tokenValue).Value;
            }

            if (auction == null)
                return HttpNotFound();
            model.Auction = auction;
            model.Bids = _context.Bids.
                Include(b => b.Auction).
                Include(b => b.User).
                Where(b => b.Auction.Id == auction.Id).
                OrderBy(b => b.DateTimeCreated).
                ToList();
            return View(model);
        }

        public string CurrentCurrency()
        {
            return _context.AppSettings.SingleOrDefault(s => s.Name == eStore.fonts.Models.Constants.CurrentCurrency).Value;
        }
    }
}