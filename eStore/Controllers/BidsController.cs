using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eStore.Models;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using System.Web.Optimization;
using DotNetNuke.Common.Utilities;
using eStore.Hubs;

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
        public JsonResult Create(int numOfTokens, int auctionId)
        {
            if (User.IsInRole(RoleName.MaintenanceManager))
            {
                return Json(-1);
            }


            if (numOfTokens < 0)
            {
                return Json(-1);
                //return error message
            }

            Auction auction = _context.Auctions.Include(a => a.LastBidder).Include(a => a.User).SingleOrDefault(a => a.Id == auctionId);
            if (auction == null || auction.State != AuctionState.Opened)
            {
                return Json(-1);
            }

            if ((numOfTokens == 0) && (auction.LastBidder != null))
            {
                return Json(-1);
            }

            var tokenValueString = eStore.fonts.Models.Constants.TokenValue;
            double tokenValue = Convert.ToDouble(_context.AppSettings.SingleOrDefault(s => s.Name == tokenValueString).Value);

            var userId = User.Identity.GetUserId();
            ApplicationUser user = _context.Users.SingleOrDefault(u => u.Id == userId);

            if (userId == auction.User.Id)
            {
                return Json(-1);
            }

            double tokensNeeded = auction.CurrentPrice / tokenValue;
            //if (auction.LastBidder != null)
            //{
              //  tokensNeeded += 1;
            //}

            //if (numOfTokens > 1)
                //tokensNeeded += numOfTokens - 1;

            tokensNeeded += numOfTokens;
            if (user.NumOfTokens < tokensNeeded)
            {
                if (auction.LastBidder.Id != userId)
                    return Json(-1);
                else if (user.NumOfTokens < numOfTokens)
                    return Json(-1);
            }

            Bid lastBid = _context.Bids.Include(b => b.Auction).Include(b => b.User).OrderByDescending(b => b.DateTimeCreated).FirstOrDefault(b => b.Auction.Id == auctionId);
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

            var hub = Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager.GetHubContext<MyHub>();
            hub.Clients.All.updatePriceAndBidder(auction.CurrentPrice, user.Email, auction.Id);
            hub.Clients.All.updateBidList(string.Format("{0:dd-MMM-yy hh:mm:ss tt}", bid.DateTimeCreated), user.Email, bid.NumOfTokens, auction.Id);
            if (lastBid != null)
            {
                if (lastBid.User.Id != userId)
                {
                    hub.Clients.All.updateNumOfTokens(lastBid.User.Id, lastBid.NumOfTokens);
                    hub.Clients.All.updateNumOfTokens(userId, tokensNeeded * (-1));
                }
                else
                    hub.Clients.All.updateNumOfTokens(lastBid.User.Id, lastBid.NumOfTokens - tokensNeeded);
            }
            else
            {
                hub.Clients.All.updateNumOfTokens(userId, tokensNeeded * (-1));
            }

            string[] jsonRes = { auction.CurrentPrice + " " + auction.Currency, user.Email };
            return Json(jsonRes);
            //return RedirectToAction("Details", "Auctions", new {id = auctionId });
        }
    }
}