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
		private Dal dal;

		public UserPageController() : this(new Dal())
		{

		}

		private UserPageController(Dal dalIoc)
		{
			dal = dalIoc;
		}

		public int SubmitDemandeInfo(string DemandeInformation, string DemandeTitre)
		{
			System.Diagnostics.Debug.WriteLine(DemandeTitre + DemandeInformation);
			if (HttpContext.User.Identity.IsAuthenticated)
			{
				Collaborateur c = dal.ObtenirCollaborateur(HttpContext.User.Identity.Name);
				dal.EnvoiDemandeInformation(new Message { Contenu = DemandeInformation, Emetteur = c.Prenom + " " + c.Nom + " - " + c.Service.Nom, Titre = DemandeTitre });
				dal.AjoutNotificationService(TypeService.RessourcesHumaines, new Message { Titre = "Deamande Information", Emetteur = c.Prenom + " " + c.Nom + " - " + c.Service.Nom, Redirection = "/RH/Index", Contenu = "Nouvelle demande d'information" });
				return 1;
			}
			return 0;
		}

		[Authorize]
		public ActionResult Profil()
        {
            CollaborateurViewModel viewModel = new CollaborateurViewModel { _Authentifie = HttpContext.User.Identity.IsAuthenticated };
            if(HttpContext.User.Identity.IsAuthenticated)
            {
                viewModel._Collaborateur = dal.ObtenirCollaborateur(HttpContext.User.Identity.Name);
            }
            return View(viewModel);
        }

        public int RetirerNotif(int idNotif)
        {
            IDal dal = new Dal();
            System.Diagnostics.Debug.WriteLine("ceci est un message des extra-terrestres");
            return dal.SupprimerNotif(idNotif);
        }
    }
}