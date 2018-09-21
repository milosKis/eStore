using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eStore.Models;
using Microsoft.AspNet.Identity;
using System.Data.Entity;

namespace eStore.Controllers
{
    public class BidsController : Controller
    {

        private ApplicationDbContext _context;

        public BidsController()
        {
            _context = new ApplicationDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        // GET: Bid
        public ActionResult Index()
        {
            return View();
        }


        [Authorize]
        public ActionResult Create(int numOfTokens, int auctionId)
        {
            if (User.IsInRole(RoleName.MaintenanceManager))
            {
                return HttpNotFound();
            }

            if (numOfTokens < 1)
            {
                return HttpNotFound();
                //return error message
            }

            Auction auction = _context.Auctions.Include(a => a.LastBidder).SingleOrDefault(a => a.Id == auctionId);
            if (auction == null || auction.State != AuctionState.Opened)
            {
                return HttpNotFound();
            }

            var tokenValueString = eStore.fonts.Models.Constants.TokenValue;
            double tokenValue = Convert.ToDouble(_context.AppSettings.SingleOrDefault(s => s.Name == tokenValueString).Value);

            var userId = User.Identity.GetUserId();
            ApplicationUser user = _context.Users.SingleOrDefault(u => u.Id == userId);

            double tokensNeeded = auction.CurrentPrice / tokenValue;
            if (auction.LastBidder != null)
            {
                tokensNeeded += 1;
            }

            if (numOfTokens > 1)
                tokensNeeded += numOfTokens - 1;

            if (user.NumOfTokens < tokensNeeded)
            {
                return HttpNotFound();
            }

            Bid lastBid = _context.Bids.LastOrDefault(b => b.Auction.Id == auctionId);
            if (lastBid != null)
            {
                auction.LastBidder.NumOfTokens += lastBid.NumOfTokens;
                _context.SaveChanges();
            }

            auction.LastBidder = user;
            auction.CurrentPrice = tokensNeeded * tokenValue;
            user.NumOfTokens -= tokensNeeded;
            Bid bid = new Bid
            {
                User = user,
                NumOfTokens = tokensNeeded,
                DateTimeCreated = DateTime.Now,
                Auction = auction
            };

            _context.Bids.Add(bid);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}