using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IntranetPOPS1819.Models
{
    public class LigneDeFrais
    {
		public int Id { get; set; }
		[Required]
		[Display(Name = "Nom de la Dépense")]
		public string Nom { get; set; }
		
		[Display(Name = "Mission Associée")]
		public Mission Mission { get; set; }
		[Required]
		[Display(Name = "Somme")]
		public int Somme { get; set; }
		[Required]
		[Display(Name = "Note Complète")]
		public bool Complete { get; set; }
		public StatutLigneDeFrais Statut { get; set; }

        //TODO rajouter qlq chose pour les Files
        //Maybe un id + nom de Ligne de Frais qui irait chercher sur le serveur le doc ? 
    }

    public enum StatutLigneDeFrais
    {
		Validée,
		Refusée
    }
}