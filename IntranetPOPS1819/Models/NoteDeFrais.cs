using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IntranetPOPS1819.Models
{
    public class NoteDeFrais
    {
        public int Id { get; set; }
        public StatutNote Statut { get; set; }
		public DateTime Date { get; set; }
		public bool Actif { get; set; } = false;
        public List<LigneDeFrais> LignesDeFrais { get; set; }
    }

    public enum StatutNote
    {
        Brouillon,
        Enregistré
    }
}