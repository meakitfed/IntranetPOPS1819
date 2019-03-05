using IntranetPOPS1819.Models;
using IntranetPOPS1819.ViewModel;
using System;
using System.Diagnostics;
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
            foreach(Conge c in vm._Collaborateur.Conges)
            {
                if (c.Statut == StatutConge.Refuse) Debug.WriteLine("Refusé");
                if(c.Statut == StatutConge.ValideChef) Debug.WriteLine("Validé");
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

        public string DemandeConge(DateTime Debut, DateTime Fin, string type)
        {
            System.Diagnostics.Debug.WriteLine(type);
            TypeConge typeConge;
            if (type == "rtt") typeConge = TypeConge.RTT;
            else if (type == "sans") typeConge = TypeConge.SansSolde;
            else if (type == "abs") typeConge = TypeConge.Absence;
            else return "erreurTypeConge";

            IDal dal = new Dal();
            Collaborateur col = dal.ObtenirCollaborateur(HttpContext.User.Identity.Name);

            Conge conge = new Conge { Debut = Debut, Fin = Fin, Type = TypeConge.RTT };
            //System.Diagnostics.Debug.WriteLine(col + " " + conge + " " +  Debut+ " " + Fin );

            string txt = "Service : " + col.Nom + "\n";
            Message notif = new Message(TypeMessage.NotifCongeAller, col.Prenom + col.Nom + " - " + col.Service.Nom, conge);

            ValiditeConge validite = col.isCongeValide(conge);
            

            if (HttpContext.User.Identity.IsAuthenticated && validite == ValiditeConge.ok)
            {
                if (col.Chef)
                {
                    if (col.Service.Type == TypeService.RessourcesHumaines)
                        dal.AjoutNotif(dal.ObtenirPDG().Id, notif);
                    else
                    {
                        foreach (Collaborateur c in dal.ObtenirCollaborateursService(dal.ObtenirServiceDeType(TypeService.RessourcesHumaines).Id))
                        {
                            dal.AjoutNotif(c.Id, notif);
                        }
                    }
                }
                else
                    dal.AjoutNotif(col.Service.Chef().Id, notif);
                dal.AjoutConge(col.Id, conge);
            }

            return validite.ToString();
        }

        public int SupprimerConge(int idConge)
        {
        
            IDal dal = new Dal();
            Collaborateur col = dal.ObtenirCollaborateur(HttpContext.User.Identity.Name);
            Conge c = dal.ObtenirConge(idConge);

            //TODO attention dangereux
            if (c.Statut == StatutConge.EnCours || c.Statut == StatutConge.ValideChef)
            {
                dal.SupprimerDemandeConge(col.Id,  idConge);
            }
            else return -1;

            return 0;

        }


    }
}