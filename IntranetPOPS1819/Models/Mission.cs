using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace IntranetPOPS1819.Models
{
    public class Mission : IComparable
    {
		public int Id { get; set; }
        public string Nom { get; set; }
		public StatutMission Statut { get; set; } = StatutMission.EnCours;
        public virtual Service Service { get; set; }

		[ScriptIgnore]
		public virtual List<Collaborateur> Collaborateurs { get; set; } = new List<Collaborateur>();

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
        Terminée,
        Annulée
    }
}