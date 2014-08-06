using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace lookback.Models
{
    public class AppContext : DbContext
    {
        public AppContext()
            : base("DefaultConnection")
        { }

        public DbSet<AccountModel> Accounts { get; set; }
    }
}