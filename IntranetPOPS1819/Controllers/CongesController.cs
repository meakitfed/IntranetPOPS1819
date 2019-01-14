using IntranetPOPS1819.Models;
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
            return View();
        }

        [HttpPost]
        public ActionResult Index(Conges c)
        {
            string txt = "Service : \nCliquez pour consulter";
            Message notif = new Message { Titre = "Demande de validation congé", Date = DateTime.Now, Contenu = txt};

            IDal dal = new Dal();
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                //TODO
                dal.AjoutNotif(dal.ObtenirCollaborateur(HttpContext.User.Identity.Name).Service.Chef().Id, notif);
                System.Diagnostics.Debug.WriteLine("Notif ajoutée");
                System.Diagnostics.Debug.WriteLine(HttpContext.User.Identity.Name + " : " + dal.ObtenirCollaborateur(HttpContext.User.Identity.Name).Notifications.Count);
            }
            return View();
        }
    }
}