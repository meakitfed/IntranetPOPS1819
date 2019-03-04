using IntranetPOPS1819.Models;
using IntranetPOPS1819.ViewModel;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
            //Collaborateur c = dal.ObtenirCollaborateur(HttpContext.User.Identity.Name);
            System.Diagnostics.Debug.WriteLine("Valider la ligne" + Id);
			LigneDeFrais ligne = dal.bdd.LigneDeFrais.FirstOrDefault(l => l.Id == Id);
			Collaborateur col = dal.bdd.Collaborateurs.FirstOrDefault(c => c.Id == ligne.IdCollab);
			System.Diagnostics.Debug.WriteLine("Ligne du collaborateur " + col.Nom + " " + col.Prenom + " du service " + col.Service.Nom + " et chef ? " + col.Chef);
			if ((col.Service.Type == TypeService.Comptabilité || col.Service.Type == TypeService.RessourcesHumaines) && col.Chef)
			{
				dal.ChangerStatutLigneDeFrais(Id, StatutLigneDeFrais.Validée);
			}
			else
			{
				dal.ChangerStatutLigneDeFrais(Id, StatutLigneDeFrais.ValidéeChef);
			}
			//TODO
			
			foreach(NoteDeFrais n in col.NotesDeFrais)
			{
				if (n.LignesDeFrais.Contains(ligne))
				{
					if(n.EstValidéeParLeChef())
					{
						System.Diagnostics.Debug.WriteLine("Envoi de la note, elle est bien validée par le chef" + Id);
						dal.EnvoiNoteDeFrais(col.Service.Id, col.Id, n.Id);
						return Json(null, JsonRequestBehavior.AllowGet);
					}
					else
					{
						return null;
					}
				}
			}

            //dal.ValiderConge(c.Id, new Message { Titre = "Validation de Note", Date = DateTime.Now, Contenu = "Une Ligne de frais a été validée par le chef de service"})
			return null;
		}

		public ActionResult RefuserLigne(int Id)
		{
			System.Diagnostics.Debug.WriteLine("Refuser La ligne" + Id);
			dal.ChangerStatutLigneDeFrais(Id, StatutLigneDeFrais.Refusée);
			return Json(null, JsonRequestBehavior.AllowGet);
		}
		public ActionResult LigneDeFrais_Read([DataSourceRequest]DataSourceRequest request, int idCol)
		{
                List<LigneDeFrais> l = new List<LigneDeFrais>();
                foreach (NoteDeFrais n in dal.ObtenirCollaborateur(HttpContext.User.Identity.Name).Service.NotesDeFrais)
                {
                    l.AddRange(n.LignesDeFrais.Where(s => (s.Statut != StatutLigneDeFrais.Validée && s.Statut != StatutLigneDeFrais.ValidéeChef && s.IdCollab == idCol)));
                }
                IQueryable<LigneDeFrais> lignedefrais = l.AsQueryable();
                /*IQueryable<LigneDeFrais> lignedefrais = dal.ObtenirCollaborateur(HttpContext.User.Identity.Name).Service.NotesDeFrais.FirstOrDefault(n => n.Id == IdNote).LignesDeFrais.AsQueryable();           /*foreach(LigneDeFrais l in lignedefrais)*/
                /*System.Diagnostics.Debug.WriteLine(l.Mission.Nom);*/
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

        public ActionResult GetInfoNote(int idNote, int idCol)
        {
            Dal dal = new Dal();
            NoteDeFrais note = dal.bdd.NotesDeFrais.FirstOrDefault(n => n.Id == idNote);
            Collaborateur col = dal.ObtenirCollaborateur(idCol);

            return Json(new { nomCol = col.Nom, prenomCol = col.Prenom, date = note.Actif, sommeValidee = note.GetSommeValidee() });
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
				vm.ListeCollab = vm._Collaborateur.Service.Collaborateurs;
				if(vm._Collaborateur.Service.Type == TypeService.Direction && vm._Collaborateur.Chef)
				{
					vm.ListeCollab.AddRange(dal.getChefRhEtCompta());
				}
				foreach (Collaborateur c in vm.ListeCollab)
				{
					int a = 0;
					int b = 0;
					foreach (NoteDeFrais n in vm._Collaborateur.Service.NotesDeFrais)
					{
						if(c.NotesDeFrais.Contains(n))
						{
							a += n.NbrRefusé();
							b += n.NbrEnAttente();
						}
					}
					vm.nbrRefusé.Add(a);
					vm.nbrEnAttente.Add(b);
				}
				
                vm.Missions = new List<MissionsViewModel>();

                IEnumerable<SelectListItem> collabosList =
                                from category in dal.ObtenirTousLesCollaborateurs()
                                select new SelectListItem
                                {
                                    Text = category.Nom,
                                    Value = category.Id.ToString()
                                };
                ViewData["collabos"] = collabosList;

                foreach (Mission m in vm._Collaborateur.Service.Missions)
                    vm.Missions.Add(new MissionsViewModel { Id = m.Id, Nom = m.Nom});
				return View(vm);
			}
			return View();
        }

		public ActionResult InformationLigneDeFraisSelection(int idCol)
		{
			return PartialView(idCol);
		}

        public void ValiderConges(int idConge)
        {
            Debug.WriteLine("Pas inutile");
            dal.ChangerStatut(idConge, StatutConge.Valide);

            Collaborateur c = dal.ObtenirCollaborateur(HttpContext.User.Identity.Name);
            List<Collaborateur> collabs = dal.ObtenirCollaborateursService(c.Service.Id);
            int idCollab = 0;
            foreach(Collaborateur col in collabs)
            {
                if (col.Conges.First(con => con.Id == idConge) != null)
                {
                    idCollab = col.Id;
                    break;
                }
            }

            dal.AjoutNotif(idCollab, new Message(TypeMessage.NotifCongeRetour, c.Prenom + c.Nom + " - " + c.Service.Nom, dal.ObtenirConge(idConge)));
            foreach (Collaborateur rh in dal.ObtenirCollaborateursService(dal.ObtenirServiceDeType(TypeService.RessourcesHumaines).Id))
            {
                dal.AjoutNotif(rh.Id, new Message(TypeMessage.NotifCongeAller, dal.ObtenirCollaborateur(idCollab).Nom + dal.ObtenirCollaborateur(idCollab).Prenom + " - " + c.Service.Nom, dal.ObtenirConge(idConge)));
            }
        }

        public ActionResult Conges_Read2([DataSourceRequest]DataSourceRequest request)
        {
            System.Diagnostics.Debug.WriteLine("Passage dans Conges_Read");

            if (HttpContext.User.Identity.IsAuthenticated)
            {
                Collaborateur c = dal.ObtenirCollaborateur(HttpContext.User.Identity.Name);
                List<ValidationCongesViewModel> vm = new List<ValidationCongesViewModel>();
                foreach (Collaborateur col in dal.ObtenirCollaborateursService(c.Service.Id))
                {
                    if (col != c)
                    {
                        foreach (Conge con in col.Conges)
                        {
                            if (con.Statut == StatutConge.EnCours)
                                vm.Add(new ValidationCongesViewModel { Id = con.Id, Nom = col.Prenom + " " + col.Nom, Service = col.Service.Nom, CongesRestants = col.CongesRestants, Debut = con.Debut, Fin = con.Fin});
                        }
                    }
                }
                Collaborateur drh = dal.ObtenirDRH();
                foreach (Conge con in drh.Conges)
                {
                    if (con.Statut == StatutConge.EnCours)
                        vm.Add(new ValidationCongesViewModel { Id = con.Id, Nom = drh.Prenom + " " + drh.Nom, Service = drh.Service.Nom, CongesRestants = drh.CongesRestants, Debut = con.Debut, Fin = con.Fin });
                }
                IQueryable<ValidationCongesViewModel> demandes = vm.AsQueryable();
                DataSourceResult result = demandes.ToDataSourceResult(request, data => new {
                    data.Id,
                    data.Debut,
                    data.Fin,
                    data.Nom,
                    data.Service,
                    data.CongesRestants
                });

                return Json(result);
            }

            return null;
        }

        public ActionResult Conges_Validation2(string nb, bool accepter)
        {
            // Modification du statut du congé
            if (accepter)
                dal.ChangerStatut(Convert.ToInt32(nb), StatutConge.ValideChef);
            else
                dal.ChangerStatut(Convert.ToInt32(nb), StatutConge.Refuse);

            // Recherche du collaborateur concerné
            Collaborateur c = dal.ObtenirCollaborateur(HttpContext.User.Identity.Name);
            List<Collaborateur> collabs = dal.ObtenirTousLesCollaborateurs();
            int idCollab = 0;
            foreach (Collaborateur col in collabs)
            {
                foreach (Conge con in col.Conges)
                {
                    if (con.Id == Convert.ToInt32(nb))
                    {
                        idCollab = col.Id;
                        break;
                    }
                }
            }

            // Envois de notifications
            if(dal.ObtenirCollaborateur(idCollab).Service != c.Service)
            {
                dal.ChangerStatut(Convert.ToInt32(nb), StatutConge.Valide);
                dal.AjoutNotif(idCollab, new Message(TypeMessage.NotifCongeRetour, c.Prenom + " " + c.Nom + " - " + c.Service.Nom, dal.ObtenirConge(Convert.ToInt32(nb))));
            }
            else
            {
                dal.AjoutNotif(idCollab, new Message(TypeMessage.NotifCongeRetour, c.Prenom + " " + c.Nom + " - " + c.Service.Nom, dal.ObtenirConge(Convert.ToInt32(nb))));
                if (dal.ObtenirConge(Convert.ToInt32(nb)).Statut == StatutConge.ValideChef)
                {
                    foreach (Collaborateur rh in dal.ObtenirCollaborateursService(dal.ObtenirServiceDeType(TypeService.RessourcesHumaines).Id))
                    {
                        dal.AjoutNotif(rh.Id, new Message(TypeMessage.NotifCongeAller, dal.ObtenirCollaborateur(idCollab).Nom + " " + dal.ObtenirCollaborateur(idCollab).Prenom + " - " + c.Service.Nom, dal.ObtenirConge(Convert.ToInt32(nb))));
                    }
                }
            }
            
            return View();
        }

        public ActionResult MissionEditing_Read([DataSourceRequest]DataSourceRequest request)
        {
            Debug.WriteLine("Passage dans MissionEditing_Read");

            IDal dal = new Dal();

            List<MissionsViewModel> missions = new List<MissionsViewModel>();
            foreach (Mission m in dal.ObtenirCollaborateur(HttpContext.User.Identity.Name).Service.Missions)
            {
                List<IdentiteViewModel> collabos = new List<IdentiteViewModel>();
                foreach (Collaborateur c in m.Collaborateurs)
                    collabos.Add(new IdentiteViewModel { Nom = c.Prenom + " " + c.Nom, Id = c.Id });

                missions.Add(new MissionsViewModel { Id = m.Id, Nom = m.Nom, Collaborateurs = collabos });
            }
            if(missions.Count() > 0) { 
                Debug.WriteLine(missions[0].Collaborateurs.Count() + " collabos associés à cette mission");

            missions[0].Collabs.Add("Bob");
            missions[0].Collabs.Add("Hnery");
            }
            IQueryable<MissionsViewModel> liste = missions.AsQueryable();

            DataSourceResult result = liste.ToDataSourceResult(request, data => new
            {
                data.Id,
                data.Nom,
                data.Statut,
                data.Collaborateurs,
                data.Collabs
            });

            return Json(result);
        }
        
        public ActionResult MissionsEditingCreate(MissionsViewModel m)
        {


            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MissionEditing_Update([DataSourceRequest] DataSourceRequest request, MissionsViewModel m)
        {
            Debug.WriteLine("Passage update");
            Debug.WriteLine(m.Temp.Count());
            foreach(SelectListItem c in m.Temp)
            {
                if (c == null) Debug.WriteLine("suicide");
                else
                    Debug.WriteLine(c.Text + c.Value);
            }

            IDal dal = new Dal();

            dal.UpdateMission(m);

            return View();
        }

        public JsonResult GetCollaborateurs(string text)
        {
            IDal dal = new Dal();
            Debug.WriteLine("Passage dans GetCollaborateurs");
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                var collabos = dal.ObtenirCollaborateursService(dal.ObtenirCollaborateur(HttpContext.User.Identity.Name).Service.Id).Select(c => new Collaborateur
                {
                    Id = c.Id,
                    Nom = c.Nom
                });

                Debug.WriteLine(collabos.Count());

                return Json(collabos, JsonRequestBehavior.AllowGet);
            }
            return null;
        }
    }
}