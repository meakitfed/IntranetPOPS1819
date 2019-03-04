using IntranetPOPS1819.Models;
using IntranetPOPS1819.ViewModel;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IntranetPOPS1819.Controllers
{
    public class ComptaController : Controller
    {
		private Dal dal;

		// GET: Compta
		public ActionResult Index()
        {
			ComptaViewModel vm = new ComptaViewModel
			{
				c = dal.ObtenirCollaborateur(HttpContext.User.Identity.Name),
				Services = dal.ObtenirTousLesServices().ToList()
			};
				
			return View(vm);
        }
		

		public ComptaController() : this(new Dal())
		{

		}

		private ComptaController(Dal dalIoc)
		{
			dal = dalIoc;
		}

		public ActionResult ValiderLigne(int Id)
		{
			//Collaborateur c = dal.ObtenirCollaborateur(HttpContext.User.Identity.Name);
			System.Diagnostics.Debug.WriteLine("Valider la ligne" + Id);
			dal.ChangerStatutLigneDeFrais(Id, StatutLigneDeFrais.Validée);
			LigneDeFrais ligne = dal.bdd.LigneDeFrais.FirstOrDefault(l => l.Id == Id);
			Collaborateur col = dal.bdd.Collaborateurs.FirstOrDefault(c => c.Id == ligne.IdCollab);
			foreach (NoteDeFrais n in col.NotesDeFrais)
			{
				if (n.LignesDeFrais.Contains(ligne))
				{
					if (n.LignesDeFrais.Contains(ligne))
					{
						if (n.EstValidée())
						{
							dal.EnvoiNoteDeFrais(col.Service.Id, col.Id, n.Id);
							return Json(null, JsonRequestBehavior.AllowGet);
						}
						else
						{
							return null;
						}
					}
				}
			}
			//dal.ValiderConge(c.Id, new Message { Titre = "Validation de Note", Date = DateTime.Now, Contenu = "Une Ligne de frais a été validée par le chef de service"})
			return Json(null, JsonRequestBehavior.AllowGet);
		}

		public ActionResult RefuserLigne(int Id)
		{
			System.Diagnostics.Debug.WriteLine("Refuser La ligne" + Id);
			dal.ChangerStatutLigneDeFrais(Id, StatutLigneDeFrais.Refusée);
			return Json(null, JsonRequestBehavior.AllowGet);
		}
		public ActionResult LigneDeFrais_Read([DataSourceRequest]DataSourceRequest request, int IdCollab)
		{
			List<LigneDeFrais> l = new List<LigneDeFrais>();
			foreach (NoteDeFrais n in dal.ObtenirCollaborateur(HttpContext.User.Identity.Name).Service.NotesDeFrais)
			{
				if(n.LignesDeFrais[0].IdCollab == IdCollab)
				{
					l.AddRange(n.LignesDeFrais.Where(s => (s.Statut != StatutLigneDeFrais.Validée)));
				}
			}
			IQueryable<LigneDeFrais> lignedefrais = l.AsQueryable();
			
			DataSourceResult result = lignedefrais.ToDataSourceResult(request, ligneDeFrais => new {
				Id = ligneDeFrais.Id,
				Nom = ligneDeFrais.Nom,
				Somme = ligneDeFrais.Somme,
				Type = ligneDeFrais.Type,
				Statut = ligneDeFrais.Statut,
				Date = ligneDeFrais.Date,
				ResumeFileUrl = ligneDeFrais.ResumeFileUrl,
				Filename = ligneDeFrais.Filename,
				Mission = ligneDeFrais.Mission,
				IdCollab = ligneDeFrais.IdCollab,
                IdNote = ligneDeFrais.IdNote,
            });

			return Json(result);
		}

		protected override void Dispose(bool disposing)
		{
			dal.bdd.Dispose();
			base.Dispose(disposing);
		}

		public ActionResult InformationLigneDeFrais(int IdCollab)
		{
			return PartialView(IdCollab);
		}
	}
}