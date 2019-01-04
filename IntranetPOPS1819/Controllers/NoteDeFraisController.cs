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
			System.Diagnostics.Debug.WriteLine("Passage dans Index GET NoteDeFraisControlleur");
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
				vm._Collaborateur = dal.ObtenirCollaborateur(HttpContext.User.Identity.Name);
				System.Diagnostics.Debug.WriteLine("Passage dans Index HttpPost NoteDeFraisControlleur");
				if (true)
				{
					System.Diagnostics.Debug.WriteLine("Form pour créer une ligne de frais accepté");
					dal.AjoutLigneDeFrais(vm._Collaborateur.Id, 1, vm._Frais);
				}
				return View(vm);
			}
			return View();

		}

		public ActionResult InformationLigneDeFrais(int idNote = default(int), int idLigne = default(int))
		{
			System.Diagnostics.Debug.WriteLine("Passage dans InformationLigneDeFrais Get NoteDeFraisControlleur");
			Collaborateur c = dal.ObtenirCollaborateur(HttpContext.User.Identity.Name);
			if(idNote != default(int) && idLigne != default(int))
			{
				LigneDeFrais ligne = c.NotesDeFrais.FirstOrDefault(n => n.Id == idNote).LignesDeFrais.FirstOrDefault(l => l.Id == idLigne);
				return PartialView(ligne);
			}
			return PartialView(null);
		}
	}
}