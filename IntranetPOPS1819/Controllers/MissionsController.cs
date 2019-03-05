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
                List<IdentiteViewModel> collabos = new List<IdentiteViewModel>();
                foreach (Collaborateur c in m.Collaborateurs)
                    collabos.Add(new IdentiteViewModel { Nom = c.Prenom + " " + c.Nom, Id = c.Id });

                missions.Add(new MissionsViewModel { Id = m.Id, Nom = m.Nom, Collaborateurs = collabos });
            }
            if (missions.Count() > 0)
            {
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
            foreach (SelectListItem c in m.Temp)
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