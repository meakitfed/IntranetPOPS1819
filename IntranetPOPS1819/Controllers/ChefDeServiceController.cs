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
		private Dal dal;

		public ChefDeServiceController() : this(new Dal())
		{

		}

		private ChefDeServiceController(Dal dalIoc)
		{
			dal = dalIoc;
		}

		public ActionResult ValiderLigne(int Id)
		{
			System.Diagnostics.Debug.WriteLine("Valider la ligne" + Id);
			dal.ChangerStatutLigneDeFrais(Id, StatutLigneDeFrais.ValidéeChef);
			return Json(null, JsonRequestBehavior.AllowGet);
		}

		public ActionResult RefuserLigne(int Id)
		{
			System.Diagnostics.Debug.WriteLine("Refuser La ligne" + Id);
			dal.ChangerStatutLigneDeFrais(Id, StatutLigneDeFrais.Refusée);
			return Json(null, JsonRequestBehavior.AllowGet);
		}
		public ActionResult LigneDeFrais_Read([DataSourceRequest]DataSourceRequest request/*, int IdNote*/)
		{
			List<LigneDeFrais> l = new List<LigneDeFrais>();
			foreach(NoteDeFrais n in dal.ObtenirCollaborateur(HttpContext.User.Identity.Name).Service.NotesDeFrais)
			{
				l.AddRange(n.LignesDeFrais);
			}
			IQueryable<LigneDeFrais> lignedefrais = l.AsQueryable();
			/*IQueryable<LigneDeFrais> lignedefrais = dal.ObtenirCollaborateur(HttpContext.User.Identity.Name).Service.NotesDeFrais.FirstOrDefault(n => n.Id == IdNote).LignesDeFrais.AsQueryable();           /*foreach(LigneDeFrais l in lignedefrais)*/
			/*System.Diagnostics.Debug.WriteLine(l.Mission.Nom);*/
			DataSourceResult result = lignedefrais.ToDataSourceResult(request, ligneDeFrais => new {
				Id = ligneDeFrais.Id,
				Nom = ligneDeFrais.Nom,
				Somme = ligneDeFrais.Somme,
				Type = ligneDeFrais.Type,
				Complete = ligneDeFrais.Complete,
				Statut = ligneDeFrais.Statut,
				Date = ligneDeFrais.Date,
				ResumeFileUrl = ligneDeFrais.ResumeFileUrl,
				Filename = ligneDeFrais.Filename,
				Mission = ligneDeFrais.Mission
			});

			return Json(result);
		}

		protected override void Dispose(bool disposing)
		{
			dal.bdd.Dispose();
			base.Dispose(disposing);
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

		public ActionResult InformationLigneDeFraisSelection(/*int idNote = default(int)*/)
		{
			return PartialView(/*idNote*/);
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
                dal.MiseAJourNotesDeFrais(HttpContext.User.Identity.Name);
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

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Conges_Validation([DataSourceRequest]DataSourceRequest request, Conge conge)
        {
            System.Diagnostics.Debug.WriteLine("Passage dans la fonction de validation");
            //System.Diagnostics.Debug.WriteLine(nb);

            dal.ChangerStatut(conge.Id, StatutConge.Valide);

            System.Diagnostics.Debug.WriteLine(dal.ObtenirConge(conge.Id).Statut);

            return Json(new[] { dal.ObtenirConge(conge.Id) }.ToDataSourceResult(request, ModelState));
        }
    }
}