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
			System.Diagnostics.Debug.WriteLine("Passage dans Index 1 NoteDeFraisControlleur");
			OngletNoteDeFraisViewModel vm = new OngletNoteDeFraisViewModel { _Authentifie = HttpContext.User.Identity.IsAuthenticated };
			if (HttpContext.User.Identity.IsAuthenticated)
			{
				vm._Collaborateur = dal.ObtenirCollaborateur(HttpContext.User.Identity.Name);
			}
			return View(vm);
		}

		[HttpPost]
		public ActionResult Index(OngletNoteDeFraisViewModel vm)
		{
			if (HttpContext.User.Identity.IsAuthenticated)
			{
				//vm._Collaborateur = dal.ObtenirCollaborateur(HttpContext.User.Identity.Name);
				System.Diagnostics.Debug.WriteLine("Passage dans Index 2 NoteDeFraisControlleur");
				System.Diagnostics.Debug.WriteLine("Info ! " + vm._Collaborateur + "   " + vm._Frais);
				if (ModelState.IsValid)
				{
					System.Diagnostics.Debug.WriteLine("Form pour créer une ligne de frais accepté");
				}
				return View(vm);
			}
			return View();

		}
	}
}