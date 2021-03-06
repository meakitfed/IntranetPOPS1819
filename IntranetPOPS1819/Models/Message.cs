﻿using System;
using System.Web;

namespace IntranetPOPS1819.Models
{
    public class Message
    {
		public int Id { get; set; }
		public string Titre { get; set; }
        //public DateTime Date { get; set; }
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
            //Date = DateTime.Now;
            Lu = false;
            switch (t)
            {
                case TypeMessage.NotifCongeAller:
                    Titre = "Demande de congés";
                    //Contenu = "\nDu " + o.Debut + " au " + (o).Fin;
                    if ((o).Statut == StatutConge.EnCours)
                    {
                        Contenu = "";
                        Redirection = "/ChefDeService/Index";
                    }
                    else
                        Redirection = "/RH/Index";
                    break;
                case TypeMessage.NotifCongeRetour:
                    Titre = "Votre demande de congés";
                    Contenu = o.Statut.ToString();
                    Redirection = "/Conges/Index";
                    break;
                default:
                    throw new HttpUnhandledException();
            }
        }
		public Message(TypeMessage t, Collaborateur c, NoteDeFrais n)
		{
			Type = t;
			Emetteur = c.Prenom + c.Nom + " - " + c.Service.Nom;
			//Date = DateTime.Now;
			Lu = false;
			switch (t)
			{
				case TypeMessage.NotifNoteAller:
					Titre = "Demande de validation de note de frais";
					Contenu = "" ;
                    SetRedirectionAller(c, n);
                    break;
				case TypeMessage.NotifNoteRetour:
                    Titre = "Validation de votre note de frais";
                    Contenu = n.Date.ToString("Y");
					Redirection = "/NoteDeFrais/Index";
                    break;
				default:
					throw new HttpUnhandledException();
			}
		}

        public void SetRedirectionAller(Collaborateur c, NoteDeFrais n)
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
            else if (n.Statut == StatutNote.ValidéeParLeChef)
            {
                 Redirection = "/Compta/Index";
            }
        }
		public Message(TypeMessage t, Collaborateur c, LigneDeFrais n, bool refusé)
		{
			Type = t;
			Emetteur = c.Prenom + c.Nom + " - " + c.Service.Nom;
			//Date = DateTime.Now;
			Lu = false;
			switch (t)
			{
				case TypeMessage.NotifLigneRetour:
					
					if(refusé)
					{
						Titre = "Refus d'une ligne de frais";
					}
					else
					{
						Titre = "Validation d'une ligne de Frais";
					}
					Contenu = "nom : " + n.Nom  + " | somme : " + n.Somme;
					Redirection = "/NoteDeFrais/Index";
					break;
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
        NotifLigneRetour,
		NotifNoteAller,
		NotifNoteRetour
	}
}