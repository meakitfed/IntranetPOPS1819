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
                //dal.ObtenirCollaborateur(HttpContext.User.Identity.Name).Service.Chef.Notifications.Add(notif);
            }
            return View();
        }
    }
}