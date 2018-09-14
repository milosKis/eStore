using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eStore.Models
{
    public static class AuctionState
    {
        public const string Ready = "READY";
        public const string Opened = "OPENED";
        public const string Completed = "COMPLETED";
    }
}