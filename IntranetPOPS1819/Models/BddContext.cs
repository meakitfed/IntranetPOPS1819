using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace IntranetPOPS1819.Models
{
    public class BddContext : DbContext
    {
		public DbSet<Mission> Missions { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Collaborateurs> Collaborateurs { get; set; }

		/*protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			Database.SetInitializer<BddContext>(null);
			base.OnModelCreating(modelBuilder);
		}*/

	}
	
}