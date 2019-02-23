using IntranetPOPS1819.Models;
using IntranetPOPS1819.ViewModel;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace IntranetPOPS1819.Controllers
{
    public class RHController : Controller
    {
        // GET: RH
        public ActionResult Index()
        {
            IDal dal = new Dal();

            if (HttpContext.User.Identity.IsAuthenticated)
            {
                ChefDeServiceViewModel vm = new ChefDeServiceViewModel { _Collaborateur = dal.ObtenirCollaborateur(HttpContext.User.Identity.Name)};
                return View(vm);
            }
            return View();
        }

        public ActionResult Conges_ReadRH([DataSourceRequest]DataSourceRequest request)
        {
            IDal dal = new Dal();

            if (HttpContext.User.Identity.IsAuthenticated)
            {
                List<ValidationCongesViewModel> vm = new List<ValidationCongesViewModel>();
                foreach (Collaborateur col in dal.ObtenirTousLesCollaborateurs())
                {
                    if (!((col.Service.Type != TypeService.RessourcesHumaines) && col.Chef))
                    {
                        foreach (Conge con in col.Conges)
                        {
                            if (con.Statut == StatutConge.ValideChef)
                                vm.Add(new ValidationCongesViewModel { Id = con.Id, Nom = col.Prenom + " " + col.Nom, Service = col.Service.Nom, CongesRestants = col.CongesRestants, Debut = con.Debut, Fin = con.Fin });
                        }
                    }
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

        public ActionResult Conges_ValidationRH(string nb, bool accepter)
        {
            IDal dal = new Dal();
            System.Diagnostics.Debug.WriteLine("Passage dans validationRH");
            // Modification du statut du congé
            if (accepter)
                dal.ChangerStatut(Convert.ToInt32(nb), StatutConge.Valide);
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
            dal.AjoutNotif(idCollab, new Message(TypeMessage.NotifCongeRetour, c.Prenom + c.Nom + " - " + c.Service.Nom, dal.ObtenirConge(Convert.ToInt32(nb))));

            return View();
        }
    }
}