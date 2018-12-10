using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace IntranetPOPS1819.Models
{
    public class Mission
    {
		private Service service;

		public int Id { get; set; }
        public string Nom { get; set; }
        public StatutMission Statut { get; set; }
        public Service Service
		{
			get
			{
				return service;
			}
			set
			{
				service = value;
				if(service != null && service.Missions.FirstOrDefault(m => m.Id == Id) == null)
				{
					service.Missions.Add(this);
				}
			}
		}
    }

    public enum StatutMission
    {
        EnCours, 
        Validée,
        Annulée
    }
}