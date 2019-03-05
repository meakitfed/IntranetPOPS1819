using IntranetPOPS1819.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IntranetPOPS1819.ViewModel
{
    public class MissionsViewModel
    {
        [ScaffoldColumn(false)]
        public int Id { get; set; }
        public string Nom { get; set; }
        public StatutMission Statut { get; set; }
    }
}