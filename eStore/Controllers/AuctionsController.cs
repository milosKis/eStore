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
        public ActionResult Index()
        {
            return View();
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