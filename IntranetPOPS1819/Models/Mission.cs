using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace IntranetPOPS1819.Models
{
    public class Mission : IComparable
    {
		public int Id { get; set; }
        public string Nom { get; set; }
        public StatutMission Statut { get; set; }
        public Service Service { get; set; }

		public int CompareTo(object obj)
		{
			if (obj == null) return 1;

			Mission otherTemperature = obj as Mission;
			if (otherTemperature != null)
				return this.Nom.CompareTo(otherTemperature.Nom);
			else
				throw new ArgumentException("Object is not a Mission");
		}
	}

    public enum StatutMission
    {
        EnCours, 
        Validée,
        Annulée
    }
}