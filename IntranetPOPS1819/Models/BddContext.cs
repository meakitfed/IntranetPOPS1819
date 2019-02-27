
using System.Data.Entity;

namespace IntranetPOPS1819.Models
{
    public class BddContext : DbContext
    {
		public DbSet<Mission> Missions { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Collaborateur> Collaborateurs { get; set; }
		public DbSet<NoteDeFrais> NotesDeFrais { get; set; }
		public DbSet<LigneDeFrais> LigneDeFrais { get; set; }
        public DbSet<Conge> Conges { get; set; }
		public DbSet<tmp> tmp { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
		}

	}
	
}