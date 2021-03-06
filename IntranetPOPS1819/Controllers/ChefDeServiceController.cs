﻿using IntranetPOPS1819.Models;
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
			Collaborateur c = dal.ObtenirCollaborateur(HttpContext.User.Identity.Name);
			System.Diagnostics.Debug.WriteLine("Valider la ligne" + Id);
			LigneDeFrais ligne = dal.bdd.LigneDeFrais.FirstOrDefault(l => l.Id == Id);
			Collaborateur col = dal.bdd.Collaborateurs.FirstOrDefault(co => co.Id == ligne.IdCollab);
			System.Diagnostics.Debug.WriteLine("Ligne du collaborateur " + col.Nom + " " + col.Prenom + " du service " + col.Service.Nom + " et chef ? " + col.Chef);
			if ((col.Service.Type == TypeService.Comptabilité || col.Service.Type == TypeService.RessourcesHumaines) && col.Chef)
			{
				dal.ChangerStatutLigneDeFrais(Id, StatutLigneDeFrais.Validée);
			}
			else
			{
				dal.ChangerStatutLigneDeFrais(Id, StatutLigneDeFrais.ValidéeChef);
			}

			dal.AjoutNotif(ligne.IdCollab, new Message(TypeMessage.NotifLigneRetour, c, ligne, false));

			foreach (NoteDeFrais n in col.NotesDeFrais)
			{
				if (n.LignesDeFrais.Contains(ligne))
				{
					if (n.EstValidéeParLeChef())
					{
						System.Diagnostics.Debug.WriteLine("Envoi de la note, elle est bien validée par le chef" + Id);
						dal.EnvoiNoteDeFrais(col.Service.Id, col.Id, n.Id);
						dal.EnleverNoteDeFraisDuService(n.Id, c.Service.Id);
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

		public ActionResult RefuserLigne(int Id, string message = "")
		{
			System.Diagnostics.Debug.WriteLine("Refuser La ligne" + Id);
			dal.ChangerStatutLigneDeFrais(Id, StatutLigneDeFrais.Refusée);
			dal.MessageDeRefusLigneDefrais(Id, message);

			Collaborateur c = dal.ObtenirCollaborateur(HttpContext.User.Identity.Name);
			LigneDeFrais l = dal.bdd.LigneDeFrais.FirstOrDefault(co => co.Id == Id);
			dal.AjoutNotif(l.IdCollab, new Message(TypeMessage.NotifLigneRetour, c, l, true));
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
				Commentaire = ligneDeFrais.Commentaire,
				Note = new NoteDeFrais
				{
					Date = ligneDeFrais.Note.Date
				} });

                return Json(result);
        }

        public ActionResult GetInfoNote(int idNote, int idCol)
        {
            System.Diagnostics.Debug.WriteLine("GetInfoNote " + idNote + " " + idCol);
            Dal dal = new Dal();
            NoteDeFrais note = dal.bdd.NotesDeFrais.FirstOrDefault(n => n.Id == idNote);
            //System.Diagnostics.Debug.WriteLine("j'ai la  note");
            Collaborateur col = dal.ObtenirCollaborateur(idCol);
            //System.Diagnostics.Debug.WriteLine("j'ai lle colabo");

            return Json(new { nomCol = col.Nom, prenomCol = col.Prenom, date = note.Date, sommeValidee = note.GetSommeValidee() }, JsonRequestBehavior.AllowGet);
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
            Debug.WriteLine("Passage dans Conges_Read");

            if (HttpContext.User.Identity.IsAuthenticated)
            {
                Collaborateur c = dal.ObtenirCollaborateur(HttpContext.User.Identity.Name);
                List<ValidationCongesViewModel> vm = new List<ValidationCongesViewModel>();
                foreach (Collaborateur col in dal.ObtenirCollaborateursService(c.Service.Id))
                {
                    if (col != c)
                    {
                        float[] congesPris = new float[3] { col.GetNombreRTTPrisCetteAnnee(), col.GetNombreSansSoldePrisCetteAnnee(), col.GetNombreAbsencesPrisCetteAnnee() };

                        foreach (Conge con in col.Conges)
                        {
                            if (con.Statut == StatutConge.EnCours)
                                vm.Add(new ValidationCongesViewModel { Id = con.Id, Nom = col.Prenom + " " + col.Nom, Service = col.Service.Nom, CongesRestants = col.CongesRestants, CongesPris = congesPris, Debut = con.Debut, Fin = con.Fin});
                        }
                    }
                }
                Collaborateur drh = dal.ObtenirDRH();
                foreach (Conge con in drh.Conges)
                {
                    float[] congesPris = new float[3] { drh.GetNombreRTTPrisCetteAnnee(), drh.GetNombreSansSoldePrisCetteAnnee(), drh.GetNombreAbsencesPrisCetteAnnee() };
                    if (con.Statut == StatutConge.EnCours)
                        vm.Add(new ValidationCongesViewModel { Id = con.Id, Nom = drh.Prenom + " " + drh.Nom, Service = drh.Service.Nom, CongesRestants = drh.CongesRestants, CongesPris = congesPris, Debut = con.Debut, Fin = con.Fin });
                }
                IQueryable<ValidationCongesViewModel> demandes = vm.AsQueryable();
                DataSourceResult result = demandes.ToDataSourceResult(request, data => new {
                    data.Id,
                    data.Debut,
                    data.Fin,
                    data.Type,
                    data.Nom,
                    data.Service,
                    data.CongesRestants,
                    data.CongesPris
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

        public int InfosAbsents(DateTime date)
        {
            IDal dal = new Dal();
            Collaborateur c = dal.ObtenirCollaborateur(HttpContext.User.Identity.Name);

            return c.Service.GetNombreCollaborateursEnConges(date);
        }

        public string ProportionAbsents(DateTime date, string text)
        {
            Debug.WriteLine(text);
            IDal dal = new Dal();
            Service s = dal.ObtenirTousLesServices().FirstOrDefault(serv => text.Contains(serv.Nom) == true);
            int num = s.GetNombreCollaborateursEnConges(date), den = s.Collaborateurs.Count();

            return num + " / " + den + "(" + (num / den) + "% )";

        }
    }
}