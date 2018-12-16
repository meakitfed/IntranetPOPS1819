using IntranetPOPS1819.Models;
using IntranetPOPS1819.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace IntranetPOPS1819.Controllers
{
    public class NoteDeFraisController : Controller
    {
		private IDal dal;

		public NoteDeFraisController() : this(new Dal())
		{

		}

		private NoteDeFraisController(IDal dalIoc)
		{
			dal = dalIoc;
		}

		[Authorize]
		public ActionResult Index()
		{
			OngletNoteDeFraisViewModel viewModel = new OngletNoteDeFraisViewModel { Authentifie = HttpContext.User.Identity.IsAuthenticated };
			if (HttpContext.User.Identity.IsAuthenticated)
			{
				System.Diagnostics.Debug.WriteLine("aaaaaaaaaaaaaaaaaaaaaah" + dal.ObtenirTousLesServices().Count);
				viewModel.Collaborateur = dal.ObtenirCollaborateur(HttpContext.User.Identity.Name);
			}
			return View(viewModel);
		}

		[HttpPost]
		public ActionResult Index(OngletNoteDeFraisViewModel viewModel, string returnUrl)
		{
			if (ModelState.IsValid)
			{
				Collaborateur utilisateur = dal.Authentifier(viewModel.Collaborateur.Mail, viewModel.Collaborateur.MotDePasse);
				if (utilisateur != null)
				{
					FormsAuthentication.SetAuthCookie(utilisateur.Id.ToString(), false);
					/*if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
						return Redirect(returnUrl);*/
					return Redirect("/UserPage/Profil");
				}
				ModelState.AddModelError("Utilisateur.Prenom", "Prénom et/ou mot de passe incorrect(s)");
			}
			return View(viewModel);
		}
	}
}