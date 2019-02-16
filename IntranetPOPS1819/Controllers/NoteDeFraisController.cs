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
using System.Web.Security;

namespace IntranetPOPS1819.Controllers
{
    public class NoteDeFraisController : Controller
    {
		private IDal dal;
		private BddContext db = new BddContext();

		public NoteDeFraisController() : this(new Dal())
		{

		}

		private NoteDeFraisController(IDal dalIoc)
		{
			dal = dalIoc;
		}

		public ActionResult LigneDeFrais_Read([DataSourceRequest]DataSourceRequest request)
		{
			System.Diagnostics.Debug.WriteLine("Passage dans LigneDeFrais_Read");
			if (HttpContext.User.Identity.IsAuthenticated)
			{
				dal.MiseAJourNotesDeFrais(HttpContext.User.Identity.Name);
				Collaborateur c = dal.ObtenirCollaborateur(HttpContext.User.Identity.Name);
				IQueryable<LigneDeFrais> lignedefrais = c.NotesDeFrais[0].LignesDeFrais.AsQueryable();
				DataSourceResult result = lignedefrais.ToDataSourceResult(request, ligneDeFrais => new {
					Id = ligneDeFrais.Id,
					Nom = ligneDeFrais.Nom,
					Somme = ligneDeFrais.Somme,
					Complete = ligneDeFrais.Complete,
					Statut = ligneDeFrais.Statut
				});

				return Json(result);
			}
			return null;
			
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult LigneDeFrais_Create([DataSourceRequest]DataSourceRequest request, LigneDeFrais ligneDeFrais)
		{
			if (ModelState.IsValid)
			{
				var entity = new LigneDeFrais
				{
					Nom = ligneDeFrais.Nom,
					Somme = ligneDeFrais.Somme,
					Complete = ligneDeFrais.Complete,
					Statut = ligneDeFrais.Statut
				};

				db.LigneDeFrais.Add(entity);
				db.SaveChanges();
				ligneDeFrais.Id = entity.Id;
			}

			return Json(new[] { ligneDeFrais }.ToDataSourceResult(request, ModelState));
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult LigneDeFrais_Update([DataSourceRequest]DataSourceRequest request, LigneDeFrais ligneDeFrais)
		{
			if (ModelState.IsValid)
			{
				var entity = new LigneDeFrais
				{
					Id = ligneDeFrais.Id,
					Nom = ligneDeFrais.Nom,
					Somme = ligneDeFrais.Somme,
					Complete = ligneDeFrais.Complete,
					Statut = ligneDeFrais.Statut
				};

				db.LigneDeFrais.Attach(entity);
				db.Entry(entity).State = EntityState.Modified;
				db.SaveChanges();
			}

			return Json(new[] { ligneDeFrais }.ToDataSourceResult(request, ModelState));
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult LigneDeFrais_Destroy([DataSourceRequest]DataSourceRequest request, LigneDeFrais ligneDeFrais)
		{
			if (ModelState.IsValid)
			{
				var entity = new LigneDeFrais
				{
					Id = ligneDeFrais.Id,
					Nom = ligneDeFrais.Nom,
					Somme = ligneDeFrais.Somme,
					Complete = ligneDeFrais.Complete,
					Statut = ligneDeFrais.Statut
				};

				db.LigneDeFrais.Attach(entity);
				db.LigneDeFrais.Remove(entity);
				db.SaveChanges();
			}

			return Json(new[] { ligneDeFrais }.ToDataSourceResult(request, ModelState));
		}

		protected override void Dispose(bool disposing)
		{
			db.Dispose();
			base.Dispose(disposing);
		}

		[Authorize]
		public ActionResult Index()
		{
			System.Diagnostics.Debug.WriteLine("Passage dans Index GET NoteDeFraisControlleur");
			OngletNoteDeFraisViewModel vm = new OngletNoteDeFraisViewModel { _Authentifie = HttpContext.User.Identity.IsAuthenticated };
			if (HttpContext.User.Identity.IsAuthenticated)
			{
				dal.MiseAJourNotesDeFrais(HttpContext.User.Identity.Name);
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
				//TODO valider le form?
				System.Diagnostics.Debug.WriteLine("Form pour créer une ligne de frais accepté");
				vm._Frais.Mission = dal.GetMission(vm._IdMission);
				foreach (NoteDeFrais n in vm._Collaborateur.NotesDeFrais)
				{
					if(n.Id == vm._IdNoteDeFrais)
					{
						dal.AjoutLigneDeFrais(vm._Collaborateur.Id, vm._IdNoteDeFrais, vm._Frais);
						//System.Diagnostics.Debug.WriteLine(vm._Collaborateur.NotesDeFrais.);
						if (vm._Frais.Complete)
						{
							dal.EnvoiLigneDeFraisChefService(vm._Collaborateur.Service.Id, vm._Collaborateur.Id, vm._Frais.Id);
							string txt = "Cliquez pour consulter";
							Message notif = new Message { Titre = "Demande de validation ligne de frais", Date = DateTime.Now, Contenu = txt };
							dal.AjoutNotif(dal.ObtenirCollaborateur(HttpContext.User.Identity.Name).Service.Chef().Id, notif);
							
						}
						return View(vm);
					}
				}
				return View(vm);
			}
			return View();

		}

		public ActionResult InformationLigneDeFrais()
		{
			System.Diagnostics.Debug.WriteLine("Passage dans InformationLigneDeFrais Get NoteDeFraisControlleur");
			return PartialView();
		}
	}
}