using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace IntranetPOPS1819.Models
{
    public class LigneDeFrais
    {
		public int Id { get; set; }

		[Required(ErrorMessage = "Le nom de la dépense doit être saisi")]
		[Display(Name = "Nom de la Dépense")]
		public string Nom { get; set; }

		[Display(Name = "Mission Associée")]
		public virtual Mission Mission { get; set; }

		[Required(ErrorMessage = "La somme de la dépense doit être saisi")]
		[Display(Name = "Somme")]
		public int Somme { get; set; }

		[Display(Name = "Note Complète")]
		public bool Complete { get; set; }

		//public DateTime Date { get; set; }
		[UIHint("StatutLigneDeFrais")]
		public StatutLigneDeFrais Statut { get; set; } = StatutLigneDeFrais.EnAttente;
	
		public DateTime Date;

		//public virtual NoteDeFrais Note { get; set; }
        //TODO rajouter qlq chose pour les Files
        //Maybe un id + nom de Ligne de Frais qui irait chercher sur le serveur le doc ? 
    }

    public enum StatutLigneDeFrais
    {
		Validée,
		Refusée,
		EnAttente
    }
}