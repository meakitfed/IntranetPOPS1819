using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace IntranetPOPS1819.Models
{
	public class Service
	{
		public Service()
		{
			Missions = new List<Mission>();
			Collaborateurs = new List<Collaborateur>();
		}

        public int Id { get; set; }
        public string Nom { get; set; }
		public virtual List<Mission> Missions { get; set; }
		public virtual List<Collaborateur> Collaborateurs { get; set; }

		/*public int ChefRefId { get; set; }
		[ForeignKey("ChefRefId")]
		public virtual Collaborateur Chef { get; set; }*/
		

		//Garder ? TODO
		public int NbAbsents { get; set; }

    }
}