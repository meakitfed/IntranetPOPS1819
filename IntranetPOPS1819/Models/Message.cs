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

        public Message(TypeMessage t, string emetteur, Conge o)
        {
            Type = t;
            Emetteur = emetteur;
            Date = DateTime.Now;
            Lu = false;
            switch (t)
            {
                case TypeMessage.NotifCongeAller:
                    Titre = "Demande de congés";
                    Contenu = "\nDu " + o.Debut + " au " + ((Conge)o).Fin;
                    if (((Conge)o).Statut == StatutConge.EnCours)
                        Redirection = "/ChefDeService/Index";
                    else
                        Redirection = "/RH/Index";
                    break;
                case TypeMessage.NotifCongeRetour:
                    Titre = "Votre demande de congés";
                    Contenu = "Du " + o.Debut + " au " + ((Conge)o).Fin + "\n" + ((Conge)o).Statut;
                    break;
                default:
                    throw new HttpUnhandledException();
            }
        }
		public Message(TypeMessage t, Collaborateur c, NoteDeFrais n)
		{
			Type = t;
			Emetteur = c.Prenom + c.Nom + " - " + c.Service.Nom;
			Date = DateTime.Now;
			Lu = false;
			switch (t)
			{
				case TypeMessage.NotifNoteAller:
					Titre = "Demande de validation de note de frais";
					Contenu = "" ;
					//SetRedirection(c, n);
					break;
				case TypeMessage.NotifNoteRetour:
					break;
				case TypeMessage.NotifLigneAller:
					break;
				case TypeMessage.NotifLigneRetour:
					break;
				default:
					throw new HttpUnhandledException();
			}
		}
		public void SetRedirection(Collaborateur c, NoteDeFrais n)
		{
			if (n.Statut == StatutNote.EnAttenteDeValidation)
			{
				if (c.Service.Type == TypeService.Direction)
				{
					Redirection = "/Compta/Index";
				}
				else if (c.Service.Type == TypeService.Comptabilité)
				{
					if (c.Chef)
					{
						//TODO CHanger maybe ? Le PDG a peut-être une interface unique ? 
						Redirection = "/ChefDeService/Index";
					}
					else
					{
						Redirection = "/ChefDeService/Index";
					}
				}
				else
				{
					if (c.Chef)
					{
						Redirection = "/Compta/Index";
					}
					else
					{
						Redirection = "/ChefDeService/Index";
					}
				}
			}
			else if(n.Statut == StatutNote.ValidéeParLeChef)
			{
				if (c.Service.Type == TypeService.Direction)
				{
					Redirection = "/Compta/Index";
				}
				else if (c.Service.Type == TypeService.Comptabilité)
				{
					if (c.Chef)
					{
						//TODO CHanger maybe ? Le PDG a peut-être une interface unique ? 
						Redirection = "/ChefDeService/Index";
					}
					else
					{
						Redirection = "/ChefDeService/Index";
					}
				}
				else
				{
					if (c.Chef)
					{
						Redirection = "/Compta/Index";
					}
					else
					{
						Redirection = "/ChefDeService/Index";
					}
				}
			}
			else if (n.Statut == StatutNote.Validée)
			{

			}
		}
	}

    public enum TypeMessage
    {
        Message,
        NotifCongeAller,
        NotifCongeRetour,
        NotifLigneAller,
        NotifLigneRetour,
		NotifNoteAller,
		NotifNoteRetour
	}
}