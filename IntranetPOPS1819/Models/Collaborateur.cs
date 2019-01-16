﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IntranetPOPS1819.Models
{
    public class Collaborateur
    {
		public Collaborateur()
		{
			DateCreationCompte = DateTime.Now;
			foreach (StatutConge s in Enum.GetValues(typeof(StatutConge)))
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
		public virtual Dictionary<StatutConge, List<Conges>> Congés { get; set; } = new Dictionary<StatutConge, List<Conges>>();
		public virtual List<NoteDeFrais> NotesDeFrais { get; set; } = new List<NoteDeFrais>();
		public virtual List<Message> Messages { get; set; } = new List<Message>();
		public virtual List<Message> Notifications { get; set; } = new List<Message>();

		public DateTime DateCreationCompte { get; set; }

		/*[ForeignKey("Service")]
		public int ServiceRefId { get; set; }*/
		public virtual Service Service { get; set; }

		public void MiseAJourNotesDeFrais()
		{
			if(NotesDeFrais.Count == 0)
			{
				NotesDeFrais.Add(new NoteDeFrais { Date = new DateTime(DateCreationCompte.Year, DateCreationCompte.Month, 1), Statut = StatutNote.Brouillon });
			}
			DateTime d = NotesDeFrais[NotesDeFrais.Count - 1].Date;
			d = d.AddMonths(1);
			while ( d < DateTime.Now)
			{
				NotesDeFrais.Add(new NoteDeFrais { Date = d, Statut = StatutNote.Brouillon, Actif = false});
				d = d.AddMonths(1);
			}
			foreach(NoteDeFrais n in NotesDeFrais)
			{
				n.Actif = false;
			}
			NotesDeFrais[NotesDeFrais.Count - 1].Actif = true;
		}

		public int GetNombreCongesPrisCetteAnnee()
		{
			int nb = 0;
			if(Congés[StatutConge.Validé] != null)
			{
				foreach (Conges c in Congés[StatutConge.Validé])
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
			if (Congés[StatutConge.EnCours] != null)
			{
				foreach (Conges c in Congés[StatutConge.EnCours])
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
			if (Congés[StatutConge.Validé] != null)
			{
				foreach (Conges c in Congés[StatutConge.Validé])
				{
					if (c.Debut.CompareTo(DateTime.Now) > 0) nb += c.Fin.Subtract(c.Debut).Days;
				}
				return nb;
			}
			return 0;
		}
	}
}