using IntranetPOPS1819.Models;
using IntranetPOPS1819.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI;

namespace IntranetPOPS1819.Controllers
{
	public class LoginController : Controller
	{
		private IDal dal;

		public LoginController() : this(new Dal())
		{

		}

		private LoginController(IDal dalIoc)
		{
			dal = dalIoc;
		}

		public ActionResult Index()
		{
		    bool connected = System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
			if(connected)
			{
				return Redirect("/UserPage/Profil");
			}
			CollaborateurViewModel viewModel = new CollaborateurViewModel { _Authentifie = HttpContext.User.Identity.IsAuthenticated };
			if (HttpContext.User.Identity.IsAuthenticated)
			{
				viewModel._Collaborateur = dal.ObtenirCollaborateur(HttpContext.User.Identity.Name);
			}
			return View(viewModel);
		}

		[HttpPost]
		public ActionResult Index(CollaborateurViewModel viewModel, string returnUrl)
		{
			if (ModelState.IsValid)
			{
				Collaborateur utilisateur = dal.Authentifier(viewModel._Collaborateur.Mail, viewModel._Collaborateur.MotDePasse);
				if (utilisateur != null)
				{
                    if(utilisateur.Licencie)
                    {
                        ModelState.AddModelError("Utilisateur.Prenom", "Vous avez été licencié, désolé");
                    }
                    else
                    {
					    FormsAuthentication.SetAuthCookie(utilisateur.Id.ToString(), false);
					    /*if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
						    return Redirect(returnUrl);*/
					    return Redirect("/UserPage/Profil");
                    }
				}
                else
                {
				    ModelState.AddModelError("Utilisateur.Prenom", "Prénom et/ou mot de passe incorrect(s)");
                }
			}
			return View(viewModel);
		}

		//[Authorize]
		public ActionResult CreerCompte()
		{
			return View();
		}

		[HttpPost]
		public ActionResult CreerCompte(Collaborateur utilisateur)
		{
			if (ModelState.IsValid)
			{
				int id = dal.AjoutCollaborateur(utilisateur.Nom, utilisateur.Prenom, utilisateur.Mail, utilisateur.MotDePasse).Id;
				FormsAuthentication.SetAuthCookie(id.ToString(), false);
				return Redirect("/");
			}
			return View(utilisateur);
		}

		public ActionResult Deconnexion()
		{
			FormsAuthentication.SignOut();
            return Redirect("/Login/Index");
        }
	}
}