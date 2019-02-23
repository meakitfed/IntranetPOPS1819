using System;
using System.Web;

namespace IntranetPOPS1819.Models
{
    public class Message
    {
		public int Id { get; set; }
		public string Titre { get; set; }
        public DateTime Date { get; set; }
        public string Contenu { get; set; }
		public bool Lu { get; set; }
        public TypeMessage Type { get; set; }
        public string Emetteur { get; set; }
        public string Redirection { get; set; }

        public Message()
        {
            /* Ne pas supprimer */
        }

        public Message(TypeMessage t, string emetteur, object o)
        {
            Type = t;
            Emetteur = emetteur;
            Date = DateTime.Now;
            Lu = false;
            switch (t)
            {
                case TypeMessage.NotifCongeAller:
                    Titre = "Demande de congés";
                    Contenu = "\nDu " + ((Conge)o).Debut + " au " + ((Conge)o).Fin;
                    if (((Conge)o).Statut == StatutConge.EnCours)
                        Redirection = "/ChefDeService/Index";
                    else
                        Redirection = "/RH/Index";
                    break;
                case TypeMessage.NotifCongeRetour:
                    Titre = "Votre demande de congés";
                    Contenu = "Du " + ((Conge)o).Debut + " au " + ((Conge)o).Fin + "\n" + ((Conge)o).Statut;
                    break;
                case TypeMessage.NotifLigneAller:    // TODO
                    Titre = "";
                    Contenu = "";
                    break;
                case TypeMessage.NotifLigneRetour:   // TODO
                default:
                    throw new HttpUnhandledException();
            }
        }
	}

    public enum TypeMessage
    {
        Message,
        NotifCongeAller,
        NotifCongeRetour,
        NotifLigneAller,
        NotifLigneRetour
    }
}