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
    public class CollaborateursController : Controller
    {
        // GET: Collaborateurs
        public ActionResult Index()
        {
            IDal dal = new Dal();
            Collaborateur c = dal.ObtenirCollaborateur(HttpContext.User.Identity.Name);

            //ViewBag.Trucs = dal.ObtenirCollaborateursService(c.Service.Id);
            return View(/*new CollaborateursViewModel { Collaborateurs = dal.ObtenirCollaborateursService(c.Service.Id) }*/);
            
        }

        public ActionResult Index_Read([DataSourceRequest]DataSourceRequest request)
        {
            IDal dal = new Dal();
            BddContext db = new BddContext();
            System.Diagnostics.Debug.WriteLine("Passage dans Index_Read");

            if (HttpContext.User.Identity.IsAuthenticated)
            {
                Collaborateur c = dal.ObtenirCollaborateur(HttpContext.User.Identity.Name);
                //System.Diagnostics.Debug.WriteLine(c.Service.Nom);
                List<CollaborateursViewModel> collabs = new List<CollaborateursViewModel>();
                foreach(Collaborateur col in dal.ObtenirCollaborateursService(c.Service.Id))
                {
                    if(!col.Admin)
                        collabs.Add(new CollaborateursViewModel { Id = col.Id, Nom = col.Nom, Prenom = col.Prenom, Service = col.Service.Nom, Missions = col.Missions });
                }
                IQueryable<Collaborateur> collaborateurs = dal.ObtenirCollaborateursService(c.Service.Id).AsQueryable();
                DataSourceResult result = collaborateurs.ToDataSourceResult(request, collab => new {
                    collab.Id,
                    collab.Prenom,
                    collab.Nom,
                    collab.Mail,
                    collab.Missions
                });

                return Json(result);
            }
            return null;
        }
    }
}