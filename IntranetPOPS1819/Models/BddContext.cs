
using System.Data.Entity;

namespace IntranetPOPS1819.Models
{
    public class BddContext : DbContext
    {
		public DbSet<Mission> Missions { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Collaborateur> Collaborateurs { get; set; }
		public DbSet<NoteDeFrais> NotesDeFrais { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			/*modelBuilder.Entity<Service>()
						.HasRequired(m => m.)
						.WithMany()
						.HasForeignKey(m => m.HomeTeamId)
						.WillCascadeOnDelete(false);

			modelBuilder.Entity<Collaborateur>()
						.HasRequired(m => m.GuestTeam)
						.WithMany()
						.HasForeignKey(m => m.GuestTeamId)
						.WillCascadeOnDelete(false);*/
		}

	}
	
}