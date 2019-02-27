using IntranetPOPS1819.Models;
using IntranetPOPS1819.ViewModel;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace IntranetPOPS1819.Controllers
{
    public class NoteDeFraisController : Controller
    {
		private Dal dal;

		public NoteDeFraisController() : this(new Dal())
		{

		}

		private NoteDeFraisController(Dal dalIoc)
		{
			dal = dalIoc;
		}

		public bool EnvoyerNote(int IdNote)
		{
			if (HttpContext.User.Identity.IsAuthenticated)
			{
				System.Diagnostics.Debug.WriteLine("Passage dans EnvoyerNote" + IdNote);
				Collaborateur c = dal.ObtenirCollaborateur(HttpContext.User.Identity.Name);
				dal.EnvoiNoteDeFraisChefService(c.Service.Id, c.Id, IdNote);
				System.Diagnostics.Debug.WriteLine(c.GetNotesDeFraisAValider().Count);
				return true;
			}
			return false;
		}

		public ActionResult LigneDeFrais_Read([DataSourceRequest]DataSourceRequest request, int IdNote)
		{
			System.Diagnostics.Debug.WriteLine("Passage dans LigneDeFrais_Read");
			if (HttpContext.User.Identity.IsAuthenticated)
			{
				dal.MiseAJourNotesDeFrais(HttpContext.User.Identity.Name);
				Collaborateur c = dal.ObtenirCollaborateur(HttpContext.User.Identity.Name);
				IQueryable<LigneDeFrais> lignedefrais = c.NotesDeFrais.FirstOrDefault(n => n.Id == IdNote).LignesDeFrais.AsQueryable();
				DataSourceResult result = lignedefrais.ToDataSourceResult(request, ligneDeFrais => new {
					Id = ligneDeFrais.Id,
					Nom = ligneDeFrais.Nom,
					Somme = ligneDeFrais.Somme,
					Complete = ligneDeFrais.Complete,
					Statut = ligneDeFrais.Statut,
					ResumeFileUrl = ligneDeFrais.ResumeFileUrl,
					Filename = ligneDeFrais.Filename,
					Date = ligneDeFrais.Date,
					Type = ligneDeFrais.Type,
					Mission = new Mission {
						Id = ligneDeFrais.Mission.Id,
						Nom = ligneDeFrais.Mission.Nom,
						Statut = ligneDeFrais.Mission.Statut,
					},
				});
				System.Diagnostics.Debug.WriteLine("Passage dans LigneDeFrais_Read envoie des données");
				return Json(result);
			}
			return null;
			
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult LigneDeFrais_Create([DataSourceRequest]DataSourceRequest request, LigneDeFrais ligneDeFrais, int IdNote)
		{
			System.Diagnostics.Debug.WriteLine("Passage dans LigneDeFrais_Create");
			if (HttpContext.User.Identity.IsAuthenticated)
			{
				var modelStateErrors = ModelState
					.Where(x => x.Value.Errors.Count > 0)
					.Select(x => new { x.Key, x.Value.Errors })
					.ToArray();

				if (ModelState.IsValid)
				{
					dal.MiseAJourNotesDeFrais(HttpContext.User.Identity.Name);
					Collaborateur c = dal.ObtenirCollaborateur(HttpContext.User.Identity.Name);
					
					var entity = new LigneDeFrais
					{
						Nom = ligneDeFrais.Nom,
						Somme = ligneDeFrais.Somme,
						Complete = ligneDeFrais.Complete,
						Statut = ligneDeFrais.Statut,
						ResumeFileUrl = ligneDeFrais.ResumeFileUrl,
						Filename = ligneDeFrais.Filename,
						Date = ligneDeFrais.Date,
						Type = ligneDeFrais.Type,
						Mission = ligneDeFrais.Mission,
					};
					dal.AjoutLigneDeFrais(c.Id, IdNote, entity);
					ligneDeFrais.Id = entity.Id;
				}
				else
				{
					var errors = ModelState.Select(x => x.Value.Errors)
										   .Where(y => y.Count > 0)
										   .ToList();
					System.Diagnostics.Debug.WriteLine("Errors : ModelState isn't valid : ");
					foreach (var v in errors)
					{
						System.Diagnostics.Debug.WriteLine(v);
					}
				}
				System.Diagnostics.Debug.WriteLine("Passage dans LigneDeFrais_Create envoie des données");
				return Json(new[] { ligneDeFrais }.ToDataSourceResult(request, ModelState));
			}
			return null;
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult LigneDeFrais_Update([DataSourceRequest]DataSourceRequest request, LigneDeFrais ligneDeFrais)
		{
			System.Diagnostics.Debug.WriteLine("Passage dans LigneDeFrais_update");
			if (ModelState.IsValid)
			{
				var entity = new LigneDeFrais
				{
					Id = ligneDeFrais.Id,
					Nom = ligneDeFrais.Nom,
					Somme = ligneDeFrais.Somme,
					Complete = ligneDeFrais.Complete,
					Statut = ligneDeFrais.Statut,
					ResumeFileUrl = ligneDeFrais.ResumeFileUrl,
					Filename = ligneDeFrais.Filename,
					Date = ligneDeFrais.Date,
					Type = ligneDeFrais.Type,
				};
				dal.bdd.LigneDeFrais.Attach(entity);
				dal.bdd.Entry(entity).State = EntityState.Modified;
				dal.bdd.SaveChanges();
				dal.ChangerMissionLigneDeFrais(ligneDeFrais.Id, ligneDeFrais.Mission.Id);
			}
			else
			{
				var errors = ModelState.Select(x => x.Value.Errors)
									   .Where(y => y.Count > 0)
									   .ToList();
				System.Diagnostics.Debug.WriteLine("Errors : ModelState isn't valid : ");
				foreach(var v in errors)
				{
					System.Diagnostics.Debug.WriteLine(v);
				}
			}
			System.Diagnostics.Debug.WriteLine("Passage dans LigneDeFrais_update envoie des données");
			return Json(new[] { ligneDeFrais }.ToDataSourceResult(request, ModelState));
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult LigneDeFrais_Destroy([DataSourceRequest]DataSourceRequest request, LigneDeFrais ligneDeFrais)
		{
			System.Diagnostics.Debug.WriteLine("Passage dans LigneDeFrais_destroy");
			if (ModelState.IsValid)
			{
				var entity = new LigneDeFrais
				{
					Id = ligneDeFrais.Id,
					Nom = ligneDeFrais.Nom,
					Somme = ligneDeFrais.Somme,
					Complete = ligneDeFrais.Complete,
					Statut = ligneDeFrais.Statut,
					ResumeFileUrl = ligneDeFrais.ResumeFileUrl,
					Filename = ligneDeFrais.Filename,
					Date = ligneDeFrais.Date,
					Type = ligneDeFrais.Type,
					Mission = ligneDeFrais.Mission,
				};

				dal.bdd.LigneDeFrais.Attach(entity);
				dal.bdd.LigneDeFrais.Remove(entity);
				dal.bdd.SaveChanges();
			}
			System.Diagnostics.Debug.WriteLine("Passage dans LigneDeFrais_destroy envoie des données");
			return Json(new[] { ligneDeFrais }.ToDataSourceResult(request, ModelState));
		}

		protected override void Dispose(bool disposing)
		{
			dal.bdd.Dispose();
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
			System.Diagnostics.Debug.WriteLine("Passage dans Index GET NoteDeFraisControlleur evnoie des données");
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
						if (vm._Frais.Complete)
						{
							dal.EnvoiNoteDeFraisChefService(vm._Collaborateur.Service.Id, vm._Collaborateur.Id, vm._Frais.Id);
							Message notif = new Message(TypeMessage.NotifLigneAller, vm._Collaborateur.Prenom + vm._Collaborateur.Nom + " - " + vm._Collaborateur.Service.Nom, vm._Frais);
                            dal.AjoutNotif(vm._Collaborateur.Service.Chef().Id, notif);
							
						}
						return View(vm);
					}
				}
				return View(vm);
			}
			return View();

		}

		public ActionResult InformationLigneDeFrais(int IdNote)
		{
			System.Diagnostics.Debug.WriteLine("Passage dans InformationLigneDeFrais Get NoteDeFraisControlleur");
			if (HttpContext.User.Identity.IsAuthenticated)
			{
				Collaborateur c = dal.ObtenirCollaborateur(HttpContext.User.Identity.Name);
				List<SelectListItem> list = new List<SelectListItem>();
				foreach (var value in Enum.GetValues(typeof(StatutLigneDeFrais)))
				{
					list.Add(new SelectListItem()
					{
						Text = value.ToString(),
						Value = ((int)value).ToString()
					});
				}
				ViewData["StatutLigne"] = list;

				List<SelectListItem> listType = new List<SelectListItem>();
				foreach (var value in Enum.GetValues(typeof(TypeLigneDeFrais)))
				{
					listType.Add(new SelectListItem()
					{
						Text = value.ToString(),
						Value = ((int)value).ToString()
					});
				}
				ViewData["TypeLigne"] = listType;

				List<SelectListItem> listMission = new List<SelectListItem>();
				foreach (var m in c.Missions)
				{
					listMission.Add(new SelectListItem()
					{
						Text = m.Nom,
						Value = ((int)m.Id).ToString()
					});
				}
				//TODO prendre que certaines missions ici
				/*foreach (var m in c.AnciennesMissions)
				{
					listMission.Add(new SelectListItem()
					{
						Text = m.Nom,
						Value = ((int)m.Id).ToString()
					});
				}*/
				ViewData["ListMission"] = listMission;
				if(c.Missions.Count != 0)
				{
					Mission  m = c.Missions.First();
					ViewData["defaultMission"] = new Mission
					{
						Id = m.Id,
						Nom = m.Nom,
						Statut = m.Statut,
					};
				}
				else
				{
					ViewData["defaultMission"] = null;
				}
			}
			System.Diagnostics.Debug.WriteLine("Passage dans InformationLigneDeFrais Get NoteDeFraisControlleur envoie des données");
			return PartialView(IdNote);
		}

		public JsonResult GetMissions(string text)
		{
			System.Diagnostics.Debug.WriteLine("Passage dans GetMissions dans NoteDeFraisControlleur");
			if (HttpContext.User.Identity.IsAuthenticated)
			{
				Collaborateur c = dal.ObtenirCollaborateur(HttpContext.User.Identity.Name);
				var missions = c.Missions.Select(m => new Mission
				{
					Nom = m.Nom,
					Id = m.Id,
					Statut = m.Statut,
				});
				System.Diagnostics.Debug.WriteLine("Passage dans GetMissions dans NoteDeFraisControlleur envoie des missions");
				return Json(missions, JsonRequestBehavior.AllowGet);
			}
			return null;
		}

		public JsonResult SaveResumeFile()
		{
			string filename = String.Empty;
			const string sessionKey = "RESUMEFILE";
			if (HttpContext.Request.Files != null && HttpContext.Request.Files.Count > 0 && HttpContext.Session != null)
			{
				List<HttpPostedFileBase> files = HttpContext.Session[sessionKey] as List<HttpPostedFileBase>;
				foreach (string fileName in HttpContext.Request.Files)
				{
					HttpPostedFileBase newFile = HttpContext.Request.Files[fileName];
					if (files == null)
					{
						files = new List<HttpPostedFileBase> { newFile };
					}
					else
					{
						files.Add(newFile);
					}
					HttpContext.Session[sessionKey] = files;
					if (newFile != null)
						filename = Path.GetFileName(newFile.FileName);
				}
			}
			System.Diagnostics.Debug.WriteLine("Passage dans SaveResumeFile, Nom du fichier enregistré : " + filename);
			return Json(new { Type = "Upload", FileName = filename }, JsonRequestBehavior.AllowGet);
		}
		public JsonResult DeleteResumeFile(string fileName)
		{
			const string sessionKey = "RESUMEFILE";
			List<HttpPostedFile> files = HttpContext.Session?[sessionKey] as List<HttpPostedFile>;
			if (files != null && files.Count > 0)
			{
				//Don't rely on browser always doing the correct thing 
				files = files.Where(f => Path.GetFileName(f.FileName) != fileName).ToList();
				HttpContext.Session[sessionKey] = files;
			}
			return Json(new { Type = "Upload", FileName = fileName }, JsonRequestBehavior.AllowGet);
		}
	}
}