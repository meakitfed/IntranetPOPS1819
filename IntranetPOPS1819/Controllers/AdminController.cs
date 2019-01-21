using IntranetPOPS1819.Models;
using IntranetPOPS1819.ViewModel;
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
			if (HttpContext.User.Identity.IsAuthenticated)
			{
				int id = dal.AjoutCollaborateur(vm.Nom, vm.Prenom, vm.Mail, vm.Nom).Id;
				foreach (Collaborateur col in dal.ObtenirTousLesCollaborateurs())
					System.Diagnostics.Debug.WriteLine(col.Nom);

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