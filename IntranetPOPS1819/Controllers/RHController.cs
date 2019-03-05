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
                            foreach (Conge con in col.Conges)
                            {
                                if (con.Statut == StatutConge.EnCours)
                                    vm.Add(new ValidationCongesViewModel { Id = con.Id, Nom = col.Prenom + " " + col.Nom, Service = col.Service.Nom, CongesRestants = col.CongesRestants, Debut = con.Debut, Fin = con.Fin });
                            }
                        }
                    }
                    // Récupérer les demandes du PDG
                    Collaborateur chef = dal.ObtenirServiceDeType(TypeService.Direction).Chef();
                    foreach (Conge con in chef.Conges)
                    {
                        if(con.Statut == StatutConge.EnCours)
                            vm.Add(new ValidationCongesViewModel { Id = con.Id, Nom = chef.Prenom + " " + chef.Nom, Service = chef.Service.Nom, CongesRestants = chef.CongesRestants, Debut = con.Debut, Fin = con.Fin });
                    }
                }
                // Récupérer les demandes des collaborateurs et chefs des autres services
                foreach (Collaborateur col in dal.ObtenirTousLesCollaborateurs())
                {
                    if ((col.Service.Type != TypeService.RessourcesHumaines))
                    {
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
                    data.CongesRestants
                });

                return Json(result);
            }

            return null;
        }

        public ActionResult Conges_ValidationRH(string nb, bool accepter)
        {
            IDal dal = new Dal();
            System.Diagnostics.Debug.WriteLine("Passage dans validationRH");
            // Modification du statut du congé
            if (accepter)
                dal.ChangerStatut(Convert.ToInt32(nb), StatutConge.Valide);
            else
                dal.ChangerStatut(Convert.ToInt32(nb), StatutConge.Refuse);

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

            // Envois de notifications
            dal.AjoutNotif(idCollab, new Message(TypeMessage.NotifCongeRetour, c.Prenom + c.Nom + " - " + c.Service.Nom, dal.ObtenirConge(Convert.ToInt32(nb))));

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

            return num + " / " + den + "(" + (num/den) + "% )";

        }
    }
}