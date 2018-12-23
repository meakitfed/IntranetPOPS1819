using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace IntranetPOPS1819.Models
{
    public class Mission
    {
		public int Id { get; set; }
        public string Nom { get; set; }
        public StatutMission Statut { get; set; }
        public Service Service { get; set; }
    }

    public enum StatutMission
    {
        EnCours, 
        Validée,
        Annulée
    }
}