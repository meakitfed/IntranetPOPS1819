using IntranetPOPS1819.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntranetPOPS1819.ViewModel
{
    public class CollaborateursViewModel
    {
        //public List<Collaborateur> Collaborateurs { get; set; }

        public int Id { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Service { get; set; }
        public List<Mission> Missions { get; set; }
    }
}