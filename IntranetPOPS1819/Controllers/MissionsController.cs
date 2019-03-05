using IntranetPOPS1819.Models;
using IntranetPOPS1819.ViewModel;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IntranetPOPS1819.Controllers
{
    public class MissionsController : Controller
    {
        // GET: Missions
        public ActionResult Index()
        {
            IDal dal = new Dal();

            IEnumerable<SelectListItem> collabosList =
                            from category in dal.ObtenirTousLesCollaborateurs()
                            select new SelectListItem
                            {
                                Text = category.Nom,
                                Value = category.Id.ToString()
                            };
            ViewData["collabos"] = collabosList;
            

            return View();
        }


        public ActionResult MissionEditing_Read([DataSourceRequest]DataSourceRequest request)
        {
            Debug.WriteLine("Passage dans MissionEditing_Read");

            IDal dal = new Dal();

            List<MissionsViewModel> missions = new List<MissionsViewModel>();
            foreach (Mission m in dal.ObtenirCollaborateur(HttpContext.User.Identity.Name).Service.Missions)
            {
                
                missions.Add(new MissionsViewModel { Id = m.Id, Nom = m.Nom });
            }
            
            IQueryable<Mission> liste = dal.ObtenirCollaborateur(HttpContext.User.Identity.Name).Service.Missions.AsQueryable();

            DataSourceResult result = liste.ToDataSourceResult(request, data => new
            {
                data.Id,
                data.Nom,
                data.Statut
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MissionEditing_Create([DataSourceRequest] DataSourceRequest request, MissionsViewModel m)
        {
            IDal dal = new Dal();
            
            dal.AjoutMission(m.Nom, dal.ObtenirCollaborateur(HttpContext.User.Identity.Name).Service.Id);

            return Json(new[] { m }.ToDataSourceResult(request, ModelState));

        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MissionEditing_Update([DataSourceRequest] DataSourceRequest request, MissionsViewModel m)
        {
            Debug.WriteLine("Passage update");

            IDal dal = new Dal();

            dal.UpdateMission(m);

            return Json(new[] { m }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult MissionEditing_Distroy([DataSourceRequest] DataSourceRequest request, MissionsViewModel m)
        {
            Debug.WriteLine("Passage update");

            IDal dal = new Dal();

            dal.SupprimerMission(m.Id);

            return Json(new[] { m }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult Mission_Accomplie([DataSourceRequest] DataSourceRequest request, MissionsViewModel m, string nb)
        {
            Debug.WriteLine(nb);

            IDal dal = new Dal();
            Collaborateur c = dal.ObtenirCollaborateur(HttpContext.User.Identity.Name);
            dal.ChangerStatut(Convert.ToInt32(nb), StatutMission.Terminée);
            Debug.WriteLine(dal.GetMission(Convert.ToInt32(nb)).Nom + " " + dal.GetMission(Convert.ToInt32(nb)).Statut);
            return Json(new[] { m }.ToDataSourceResult(request, ModelState));
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

                return Json(collabos, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

    }
}