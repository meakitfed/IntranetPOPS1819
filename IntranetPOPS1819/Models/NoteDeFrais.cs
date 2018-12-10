using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntranetPOPS1819.Models
{
    public class NoteDeFrais
    {
        public int Id { get; set; }
        public StatutNote Statut { get; set; }
        public List<LigneDeFrais> LignesDeFrais { get; set; }
    }

    public enum StatutNote
    {
        Brouillon,
        Enregistré
    }
}