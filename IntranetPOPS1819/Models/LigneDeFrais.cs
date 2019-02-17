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
		[Display(Name = "Nom")]
		public string Nom { get; set; }

		[Display(Name = "Mission")]
		public virtual Mission Mission { get; set; }

		[Required(ErrorMessage = "La somme de la dépense doit être saisi")]
		[Display(Name = "Montant")]
		public int Somme { get; set; }

		[Required(ErrorMessage = "La type de frais doit être saisi")]
		[Display(Name = "Type de Frais")]
		[UIHint("TypeLigneDeFrais")]
		public TypeLigneDeFrais Type { get; set; } = TypeLigneDeFrais.Autre;

		[Display(Name = "Note Complète")]
		public bool Complete { get; set; }

		//public DateTime Date { get; set; }
		[UIHint("StatutLigneDeFrais")]
		public StatutLigneDeFrais Statut { get; set; } = StatutLigneDeFrais.EnAttente;

		[UIHint("DateTemplate")]
		public DateTime? Date { get; set; }

		//public virtual NoteDeFrais Note { get; set; }
		//TODO rajouter qlq chose pour les Files
		public string ResumeFileUrl { set; get; }

		public string Filename { set; get; }
	}

    public enum StatutLigneDeFrais
    {
		Validée,
		Refusée,
		EnAttente
    }

	public enum TypeLigneDeFrais
	{
		Transports,
		Restauration,
		Hôtel,
		Autre
	}
}