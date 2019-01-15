using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IntranetPOPS1819.Models
{
    public class Collaborateur
    {
		public Collaborateur()
		{
			LastUpdate = DateTime.Now;
			foreach (StatutCongé s in Enum.GetValues(typeof(StatutCongé)))
			{
				Congés[s] = new List<Conges>();
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
        public bool Chef { get; set; } = false;
		public string Telephone { get; set; } = "Pas de numéro";

		public virtual List<Mission> Missions { get; set; } = new List<Mission>();
		public virtual Dictionary<StatutCongé, List<Conges>> Congés { get; set; } = new Dictionary<StatutCongé, List<Conges>>();
		public virtual List<NoteDeFrais> NotesDeFrais { get; set; } = new List<NoteDeFrais>();
		public virtual List<Message> Messages { get; set; } = new List<Message>();
		public virtual List<Message> Notifications { get; set; } = new List<Message>();

		public DateTime LastUpdate { get; set; }
		public int LastUpdateNoteDeFrais { get; set; }

		/*[ForeignKey("Service")]
		public int ServiceRefId { get; set; }*/
		public virtual Service Service { get; set; }

		public void MiseAJourNotesDeFrais()
		{
			/*if (DateTime.Today != LastUpdate.Date)
			{
				System.Diagnostics.Debug.WriteLine("Passage MiseAJourNotesDeFrais, avec mise à jour");
				if (NotesDeFrais.Count == 0)
				{
					NotesDeFrais.Add(new List<NoteDeFrais>()
					{
						new NoteDeFrais { Date = new DateTime(LastUpdate.Year, LastUpdate.Month, 1), Statut = StatutNote.Brouillon }
					});
				}
				DateTime d = LastUpdate;
				d = d.AddMonths(1);
				while (d < DateTime.Now)
				{
					if (!NotesDeFrais.ContainsKey(d.Year))
					{
						NotesDeFrais[d.Year] = new List<NoteDeFrais>
						{
							new NoteDeFrais { Date = d, Statut = StatutNote.Brouillon, Actif = false }
						};
					}
					else
					{
						NotesDeFrais[d.Year].Add(new NoteDeFrais { Date = d, Statut = StatutNote.Brouillon, Actif = false });
					}
					d = d.AddMonths(1);
				}

				foreach (KeyValuePair<int, List<NoteDeFrais>> dic in NotesDeFrais)
				{
					foreach (NoteDeFrais n in dic.Value)
					{
						n.Actif = false;
					}
				}
				NotesDeFrais[DateTime.Now.Year][NotesDeFrais[DateTime.Now.Year].Count - 1].Actif = true;
				LastUpdate = DateTime.Now;
			}
			else
			{
				System.Diagnostics.Debug.WriteLine("Passage MiseAJourNotesDeFrais, sans mise à jour");
			}*/
			
		}

		public int GetNombreCongesPrisCetteAnnee()
		{
			int nb = 0;
			if(Congés[StatutCongé.Validé] != null)
			{
				foreach (Conges c in Congés[StatutCongé.Validé])
				{
                    if (c.Debut.Year == DateTime.Now.Year) nb += c.Fin.Subtract(c.Debut).Days;
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
				foreach (Conges c in Congés[StatutCongé.EnCours])
				{
					nb += c.Fin.Subtract(c.Debut).Days;
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
				foreach (Conges c in Congés[StatutCongé.Validé])
				{
					if (c.Debut.CompareTo(DateTime.Now) > 0) nb += c.Fin.Subtract(c.Debut).Days;
				}
				return nb;
			}
			return 0;
		}
	}
}