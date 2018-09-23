using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eStore.Models
{
    public class AuctionDetailsModel
    {
        public Auction Auction { get; set; }

        public List<Bid> Bids { get; set; }
    }
}