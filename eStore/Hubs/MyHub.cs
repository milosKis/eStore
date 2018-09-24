using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace eStore.Hubs
{
    public class MyHub : Hub
    {
        public void UpdatePriceAndBidder(double newPrice, string newBidder, int auctionId)
        {
           Clients.All.updatePriceAndBidder(newPrice, newBidder, auctionId);
        }
    }
}