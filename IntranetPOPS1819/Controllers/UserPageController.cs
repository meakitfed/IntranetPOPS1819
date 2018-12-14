using IntranetPOPS1819.Models;
using IntranetPOPS1819.ViewModel;
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
            CollaborateurViewModel viewModel = new CollaborateurViewModel { Authentifie = HttpContext.User.Identity.IsAuthenticated };
            if(HttpContext.User.Identity.IsAuthenticated)
            {
                viewModel.Collaborateur = d.ObtenirCollaborateur(HttpContext.User.Identity.Name);
            }
            return View(viewModel);
        }
    }
}