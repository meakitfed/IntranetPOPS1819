using IntranetPOPS1819.Models;
using IntranetPOPS1819.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;

namespace IntranetPOPS1819.Controllers
{
    public class AdminController : Controller
    {
		private IDal dal;
		
		public AdminController()
		{
			dal= new Dal();
		}

        public ActionResult NouveauCollabo()
        {
            dal = new Dal();
            ViewBag.Service = new SelectList(dal.ObtenirTousLesServices(), "Id", "Nom");
            IEnumerable<SelectListItem> servicesList =
                                from category in dal.ObtenirTousLesServices()
                                select new SelectListItem
                                {
                                    Text = category.Nom,
                                    Value = category.Id.ToString()
                                };
            ViewData["services"] = servicesList;

            if (HttpContext.User.Identity.IsAuthenticated)
			{
				NouveauCollaborateurViewModel vm = new NouveauCollaborateurViewModel
				{
                    _Collaborateur = dal.ObtenirCollaborateur(HttpContext.User.Identity.Name)
				};
                
                return View(vm);
			}
			return View();
		}

        [HttpPost]
        public ActionResult NouveauCollabo(NouveauCollaborateurViewModel vm)
        {
            Debug.WriteLine(vm.Service);
            int id = Convert.ToInt32(vm.Service);


            if (HttpContext.User.Identity.IsAuthenticated)
			{
				dal.AjoutCollaborateur(vm.Nom, vm.Prenom, vm.Mail, vm.Nom, id);
				foreach (Collaborateur col in dal.ObtenirTousLesCollaborateurs())
					Debug.WriteLine(col.Nom);

				vm._Collaborateur = dal.ObtenirCollaborateur(HttpContext.User.Identity.Name);
				return View(vm);
			}
			return View();
        }

        public ActionResult NouveauService()
        {
			if (HttpContext.User.Identity.IsAuthenticated)
			{
				NouveauServiceViewModel vm = new NouveauServiceViewModel
				{
					_Collaborateur = dal.ObtenirCollaborateur(HttpContext.User.Identity.Name)
				};
				return View(vm);
			}
			return View();
        }

        [HttpPost]
        public ActionResult NouveauService(NouveauServiceViewModel vm)
        {
			if (HttpContext.User.Identity.IsAuthenticated)
			{
				vm._Collaborateur = dal.ObtenirCollaborateur(HttpContext.User.Identity.Name);
				dal.AjoutService(vm.Nom);
				foreach (Service ser in dal.ObtenirTousLesServices())
					System.Diagnostics.Debug.WriteLine(ser.Nom);
				return View(vm);
			}
            return View();
        }
    }
}