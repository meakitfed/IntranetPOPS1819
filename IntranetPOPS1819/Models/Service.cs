using System.Collections.Generic;

namespace IntranetPOPS1819.Models
{
    public class Service
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public Collaborateur Chef { get; set; }
		public List<Mission> Missions { get; set; }
		public List<Collaborateur> Collaborateurs { get; set; }

        //Garder ? TODO
        public int NbAbsents { get; set; }

    }
}