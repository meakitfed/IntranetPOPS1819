using IntranetPOPS1819.Models;
using IntranetPOPS1819.ViewModel;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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

        public ActionResult Conges_Read([DataSourceRequest]DataSourceRequest request)
        {
            System.Diagnostics.Debug.WriteLine("Passage dans Conges_Read");

            if (HttpContext.User.Identity.IsAuthenticated)
            {
                Collaborateur c = dal.ObtenirCollaborateur(HttpContext.User.Identity.Name);
                IQueryable<Conge> conges = c.Conges.AsQueryable();
                DataSourceResult result = conges.ToDataSourceResult(request, conge => new {
                    conge.Id,
                    conge.Debut,
                    conge.Fin,
                    conge.Statut
                });

                return Json(result);
            }
            return null;

        }
        public ActionResult Conges_Validation(string nb, bool accepter)
        {
            System.Diagnostics.Debug.WriteLine("Passage dans la fonction de validation");
            System.Diagnostics.Debug.WriteLine(accepter);

            if (accepter)
                dal.ChangerStatut(Convert.ToInt32(nb), StatutConge.ValideChef);
            else
                dal.ChangerStatut(Convert.ToInt32(nb), StatutConge.Refuse);

            System.Diagnostics.Debug.WriteLine(dal.ObtenirConge(Convert.ToInt32(nb)).Statut);

            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Conges_Validation([DataSourceRequest]DataSourceRequest request, Conge conge, int contractId)
        {
            System.Diagnostics.Debug.WriteLine("Passage dans la fonction de validation");
            System.Diagnostics.Debug.WriteLine(contractId);

            dal.ChangerStatut(conge.Id, StatutConge.Valide);

            System.Diagnostics.Debug.WriteLine(dal.ObtenirConge(conge.Id).Statut);

            return Json(new[] { dal.ObtenirConge(conge.Id) }.ToDataSourceResult(request, ModelState));
        }
    }
}