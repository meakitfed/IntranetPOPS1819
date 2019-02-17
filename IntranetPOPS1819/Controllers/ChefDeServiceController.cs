using IntranetPOPS1819.Models;
using IntranetPOPS1819.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IntranetPOPS1819.Controllers
{
    public class ChefDeServiceController : Controller
    {
		private IDal dal;

		public ChefDeServiceController() : this(new Dal())
		{

		}

		private ChefDeServiceController(IDal dalIoc)
		{
			dal = dalIoc;
		}

		// GET: ChefDeService
		public ActionResult Index(ChefDeServiceViewModel vm)
        {
			if (HttpContext.User.Identity.IsAuthenticated)
			{
				vm._Collaborateur = dal.ObtenirCollaborateur(HttpContext.User.Identity.Name);
				return View(vm);
			}
			return View();
        }

		public ActionResult InformationLigneDeFraisSelection(int idCollab = default(int), int idLigne = default(int))
		{
			
			Collaborateur c = dal.ObtenirCollaborateur(HttpContext.User.Identity.Name);
			if (idCollab != default(int) && idLigne != default(int))
			{
				LigneDeFrais ligne = dal.ObtenirCollaborateur(idCollab).GetLigneDeFraisAValider().FirstOrDefault(l => idLigne == l.Id);
				System.Diagnostics.Debug.WriteLine("Passage dans Get InformationLigneDeFraisSelection, ligne : " + ligne);
				return PartialView(ligne);
			}
			return PartialView(null);
		}

		public bool ValiderLigneDeFrais(int idCollab = default(int), int idLigne = default(int))
		{
			System.Diagnostics.Debug.WriteLine("Validation ligne de frais !");
			if (idCollab != default(int) && idLigne != default(int))
			{
				LigneDeFrais ligne = dal.ObtenirCollaborateur(idCollab).GetLigneDeFraisAValider().FirstOrDefault(l => idLigne == l.Id);
				dal.ChangerStatutLigneDeFrais(idLigne, StatutLigneDeFrais.Validée);
				System.Diagnostics.Debug.WriteLine("Passage dans Get InformationLigneDeFraisSelection, ligne : " + ligne);
			}
			return true;
		}

        public void ValiderConges(int idConge)
        {
            dal.ChangerStatut(idConge, StatutConge.Valide);
        }
	}
}