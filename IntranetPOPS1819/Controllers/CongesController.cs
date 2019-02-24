using IntranetPOPS1819.Models;
using IntranetPOPS1819.ViewModel;
using System;
using System.Web.Mvc;
namespace IntranetPOPS1819.Controllers
{
    public class CongesController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            IDal dal = new Dal();
            CongesViewModel vm = new CongesViewModel { /*_Authentifie = HttpContext.User.Identity.IsAuthenticated */};
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                vm._Collaborateur = dal.ObtenirCollaborateur(HttpContext.User.Identity.Name);
            }
            
            return View(vm);
        }

        [HttpPost]
        public ActionResult Index(CongesViewModel vm)
        {
            IDal dal = new Dal();
            Collaborateur col = dal.ObtenirCollaborateur(HttpContext.User.Identity.Name);
            vm._Collaborateur = col;
            System.Diagnostics.Debug.WriteLine(vm._Collaborateur.Nom);
            string txt = "Service : " + vm._Collaborateur.Service.Nom + "\n";
            Message notif = new Message(TypeMessage.NotifCongeAller, col.Prenom + col.Nom + " - " + col.Service.Nom, vm._Conge);

            if (HttpContext.User.Identity.IsAuthenticated)
            {
                dal.AjoutNotif(col.Service.Chef().Id, notif);
                dal.AjoutConge(col.Id, vm._Conge);
                dal.EnvoiCongeChef(col.Service.Id, col.Id, vm._Conge.Id);
            }
            return View(vm);
        }

       public string DemandeConge(DateTime Debut, DateTime Fin)
       {
            IDal dal = new Dal();
            Collaborateur col = dal.ObtenirCollaborateur(HttpContext.User.Identity.Name);

            Conge conge = new Conge { Debut = Debut, Fin = Fin, Type = TypeConge.RTT };
            System.Diagnostics.Debug.WriteLine(col + " " + conge + " " +  Debut+ " " + Fin );
            return col.isCongeValide(conge).ToString();
        }


    }
}