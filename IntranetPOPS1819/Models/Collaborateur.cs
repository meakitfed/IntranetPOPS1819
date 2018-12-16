using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IntranetPOPS1819.Models
{
    public class Collaborateur
    {
		public Collaborateur()
		{
			foreach(StatutCongé s in Enum.GetValues(typeof(StatutCongé)))
			{
				Congés[s] = new List<Congés>();
			}
		}

		//variables
		public int Id { get; set; }
		[Required]
		[Display(Name = "Mail")]
		public string Mail { get; set; }
		[Required]
		[Display(Name = "Mot de passe")]
		public string MotDePasse { get; set; }
		public string Nom { get; set; }
        public string Prenom { get; set; }
		public int CongésRestants { get; set; } = 0;
		//Garder ? TODO
		public bool Admin { get; set; } = false;
		public string Telephone { get; set; } = "Pas de numéro";

		public virtual List<Mission> Missions { get; set; } = new List<Mission>();
		public virtual Dictionary<StatutCongé, List<Congés>> Congés { get; set; } = new Dictionary<StatutCongé, List<Congés>>();
		public virtual List<NoteDeFrais> NotesDeFrais { get; set; } = new List<NoteDeFrais>();
		public virtual List<Message> Messages { get; set; } = new List<Message>();
		public virtual List<Message> Notifications { get; set; } = new List<Message>();
		public virtual Service Service { get; set; }
		
		public int GetNombreCongesPrisCetteAnnee()
		{
			int nb = 0;
			if(Congés[StatutCongé.Validé] != null)
			{
				foreach (Congés c in Congés[StatutCongé.Validé])
				{
					if (c.Date.Year == DateTime.Now.Year) nb += c.Durée;
				}
				return nb;
			}
			return 0;
		}

		public int GetNombreCongesEnAttente()
		{
			int nb = 0;
			if (Congés[StatutCongé.EnCours] != null)
			{
				foreach (Congés c in Congés[StatutCongé.EnCours])
				{
					nb += c.Durée;
				}
				return nb;
			}
			return 0;
			
		}

		public int GetNombreCongesValidesFuturs()
		{
			int nb = 0;
			if (Congés[StatutCongé.Validé] != null)
			{
				foreach (Congés c in Congés[StatutCongé.Validé])
				{
					if (c.Date.CompareTo(DateTime.Now) > 0) nb += c.Durée;
				}
				return nb;
			}
			return 0;
		}
	}
}