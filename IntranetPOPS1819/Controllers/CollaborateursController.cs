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


		public ActionResult InformationsCollaborateur(int idCol)
		{
			IDal dal = new Dal();
			Collaborateur col = dal.ObtenirCollaborateur(idCol);

			return PartialView(col);
		}

        public int Licencier(int idCol)
        {
            IDal dal = new Dal();
            dal.SupprimerCollaborateur(idCol);

            return 0;
        }

        public int ChangerService(int idCol, int idSer)
        {
            IDal dal = new Dal();
            dal.AssignerService(idSer, idCol);

            return 0;
        }

       
    }


}