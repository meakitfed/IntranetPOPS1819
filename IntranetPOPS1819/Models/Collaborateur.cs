using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IntranetPOPS1819.Models
{
    public class Collaborateur
    { 
        public int Id { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
		[Required]
		[Display(Name = "Mail")]
		public string Mail { get; set; }
		[Required]
		[Display(Name = "Mot de passe")]
		public string MotDePasse { get; set; }
        public Service Service { get; set; }
        public List<Mission> Missions { get; set; }

        public int JoursRestants { get; set; }

        //Garder ? TODO
        public bool Admin { get; set; }

		public List<Congés> Congés { get; set; }
    }
}