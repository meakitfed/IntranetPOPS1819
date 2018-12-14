using IntranetPOPS1819.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IntranetPOPS1819.Controllers
{
    public class UserPageController : Controller
    { 
        // GET: UserPage
        public ActionResult Profil()
        {
            Dal d = new Dal();
            d.AjoutCollaborateur("Nathan", "Bonnard", "nathan.bonnard@u-psud.fr", "nathan");
            Collaborateur c = d.ObtenirCollaborateur("1");
            return View(c);
        }
    }
}