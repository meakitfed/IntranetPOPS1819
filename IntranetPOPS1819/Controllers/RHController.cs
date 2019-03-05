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
    public class RHController : Controller
    {
		Dal dal = new Dal();

		// GET: RH
		public ActionResult Index()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                Collaborateur c = dal.ObtenirCollaborateur(HttpContext.User.Identity.Name);
                return View(c);
            }
            return View();
        }

		public ActionResult InformationMessage(int id)
		{
			return PartialView(dal.bdd.Messages.FirstOrDefault(m => m.Id == id));
		}

		public ActionResult Conges_ReadRH([DataSourceRequest]DataSourceRequest request)
        {
            IDal dal = new Dal();

            if (HttpContext.User.Identity.IsAuthenticated)
            {
                Collaborateur c = dal.ObtenirCollaborateur(HttpContext.User.Identity.Name);
                List<ValidationCongesViewModel> vm = new List<ValidationCongesViewModel>();

                // Cas DRH
                if (c.Chef)
                {
                    // Récupérer les demandes du service RH
                    foreach(Collaborateur col in dal.ObtenirCollaborateursService(dal.ObtenirServiceDeType(TypeService.RessourcesHumaines).Id))
                    {
                        if (col != c)
                        {
                            float[] congesPris = new float[3] { col.GetNombreRTTPrisCetteAnnee(), col.GetNombreSansSoldePrisCetteAnnee(), col.GetNombreAbsencesPrisCetteAnnee() };
                            foreach (Conge con in col.Conges)
                            {
                                if (con.Statut == StatutConge.EnCours)
                                    vm.Add(new ValidationCongesViewModel { Id = con.Id, Nom = col.Prenom + " " + col.Nom, Service = col.Service.Nom, CongesRestants = col.CongesRestants, CongesPris = congesPris, Debut = con.Debut, Fin = con.Fin });
                            }
                        }
                    }
                    // Récupérer les demandes du PDG
                    Collaborateur chef = dal.ObtenirServiceDeType(TypeService.Direction).Chef();
                    float[] congesPrisChef = new float[3] { chef.GetNombreRTTPrisCetteAnnee(), chef.GetNombreSansSoldePrisCetteAnnee(), chef.GetNombreAbsencesPrisCetteAnnee() };
                    foreach (Conge con in chef.Conges)
                    {
                        Debug.WriteLine("Statut avant validation : " + con.Type);
                        if (con.Statut == StatutConge.EnCours)
                            vm.Add(new ValidationCongesViewModel { Id = con.Id, Nom = chef.Prenom + " " + chef.Nom, Service = chef.Service.Nom, CongesRestants = chef.CongesRestants, CongesPris = congesPrisChef, Debut = con.Debut, Fin = con.Fin });
                    }
                }
                // Récupérer les demandes des collaborateurs et chefs des autres services
                foreach (Collaborateur col in dal.ObtenirTousLesCollaborateurs())
                {
                    if ((col.Service.Type != TypeService.RessourcesHumaines) && ((col != dal.ObtenirPDG())))
                    {
                        float[] congesPris = new float[3] { col.GetNombreRTTPrisCetteAnnee(), col.GetNombreSansSoldePrisCetteAnnee(), col.GetNombreAbsencesPrisCetteAnnee() };
                        foreach (Conge con in col.Conges)
                        {
                            if ((con.Statut == StatutConge.ValideChef) || (col.Chef && con.Statut == StatutConge.EnCours))
                                vm.Add(new ValidationCongesViewModel { Id = con.Id, Nom = col.Prenom + " " + col.Nom, Service = col.Service.Nom, CongesRestants = col.CongesRestants, Debut = con.Debut, Fin = con.Fin });
                        }
                    }
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

        public ActionResult Conges_ValidationRH(string nb, bool accepter)
        {
            IDal dal = new Dal();
            System.Diagnostics.Debug.WriteLine("Passage dans validationRH");
            bool refus;
            // Modification du statut du congé
            if (accepter)
            {
                dal.ChangerStatut(Convert.ToInt32(nb), StatutConge.Valide);
                refus = false;
            }
            else
            {
                dal.ChangerStatut(Convert.ToInt32(nb), StatutConge.Refuse);
                refus = true;
            }
                
            Debug.WriteLine("Statut après validation : " + dal.ObtenirConge(Convert.ToInt32(nb)).Type);
            Collaborateur c = dal.ObtenirCollaborateur(HttpContext.User.Identity.Name);

            // Recherche du collaborateur concerné
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

            //On rend ses congés au collaborateur
            if (refus && dal.ObtenirConge(Convert.ToInt32(nb)).Type == TypeConge.RTT)
            {
                dal.AjouterCongesRestants(idCollab,  dal.ObtenirConge(Convert.ToInt32(nb)).GetDuree());
            }

            // Envois de notifications
            dal.AjoutNotif(idCollab, new Message(TypeMessage.NotifCongeRetour, c.Prenom + c.Nom + " - " + c.Service.Nom, dal.ObtenirConge(Convert.ToInt32(nb))));

            return View();
        }

        public int InfosAbsents(DateTime date, string text)
        {
            IDal dal = new Dal();

            return dal.ObtenirTousLesServices().FirstOrDefault(serv => text.Contains(serv.Nom) == true).GetNombreCollaborateursEnConges(date);
        }

        public string ProportionAbsents(DateTime date, string text)
        {
            Debug.WriteLine(text);
            IDal dal = new Dal();
            Service s = dal.ObtenirTousLesServices().FirstOrDefault(serv => text.Contains(serv.Nom) == true);
            int num = s.GetNombreCollaborateursEnConges(date), den = s.Collaborateurs.Count();

            return num + " / " + den + "(" + (num/den) + "% )";

        }
    }
}