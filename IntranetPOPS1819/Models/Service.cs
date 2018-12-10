using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntranetPOPS1819.Models
{
    public class Service
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public Collaborateurs Chef { get; set; }
		public List<Mission> Missions { get; set; }
		public List<Collaborateurs> Collaborateurs { get; set; }

        //Garder ? TODO
        public int NbAbsents { get; set; }
    }
}