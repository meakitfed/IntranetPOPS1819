using IntranetPOPS1819.Models;
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
            return View(dal.ObtenirTousLesServices().ToList());
        }


		public ActionResult InformationsCollaborateur(Collaborateur col)
		{
			return PartialView(col);
		}
    }


}