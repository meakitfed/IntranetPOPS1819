using IntranetPOPS1819.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace IntranetPOPS1819.Controllers
{
    public class AdminController : Controller
    {
        public ActionResult NouveauCollabo()
        {
            return View();
        }

        [HttpPost]
        public ActionResult NouveauCollabo(Collaborateur c)
        {
            IDal dal = new Dal();
            
            int id = dal.AjoutCollaborateur(c.Nom, c.Prenom, c.Mail, c.MotDePasse).Id;
            foreach (Collaborateur col in dal.ObtenirTousLesCollaborateurs())
                System.Diagnostics.Debug.WriteLine(col.Nom);
            
            return View();
        }

        public ActionResult NouveauService()
        {
            return View();
        }

        [HttpPost]
        public ActionResult NouveauService(Service s)
        {
            IDal dal = new Dal();

            int id = dal.AjoutService(s.Nom).Id;
            foreach (Service ser in dal.ObtenirTousLesServices())
                System.Diagnostics.Debug.WriteLine(ser.Nom);

            return View();
        }
    }
}