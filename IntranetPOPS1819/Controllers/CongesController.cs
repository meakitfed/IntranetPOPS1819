using IntranetPOPS1819.Models;
using IntranetPOPS1819.ViewModel;
using System;
using System.Web.Mvc;
using System.Web.Security;

namespace IntranetPOPS1819.Controllers
{
    public class CongesController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            IDal dal = new Dal();
            CongesViewModel vm = new CongesViewModel { _Authentifie = HttpContext.User.Identity.IsAuthenticated };
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                vm._Collaborateur = dal.ObtenirCollaborateur(HttpContext.User.Identity.Name);
            }
            
            return View(vm);
        }

        [HttpPost]
        public ActionResult Index(CongesViewModel vm)
        {
            IDal dal = new Dal();
            vm._Collaborateur = dal.ObtenirCollaborateur(HttpContext.User.Identity.Name);
            System.Diagnostics.Debug.WriteLine(vm._Collaborateur.Nom);
            string txt = "Service : " + vm._Collaborateur.Service.Nom + "\n";
            Message notif = new Message { Titre = "Demande de validation congé", Date = DateTime.Now, Contenu = txt};

            
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                dal.AjoutNotif(dal.ObtenirCollaborateur(HttpContext.User.Identity.Name).Service.Chef().Id, notif);
                dal.AjoutConge(dal.ObtenirCollaborateur(HttpContext.User.Identity.Name).Id, vm._Conge);
            }
            return View(vm);
        }
    }
}