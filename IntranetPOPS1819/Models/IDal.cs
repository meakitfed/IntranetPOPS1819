using System;
using System.Collections.Generic;

namespace IntranetPOPS1819.Models
{
    public interface IDal : IDisposable
    {
        List<Collaborateur> ObtenirTousLesCollaborateurs();
		List<Service> ObtenirTousLesServices();
		Collaborateur AjoutCollaborateur(string nom, string prenom, string mail, string mdp);
		Service AjoutService(string nom);
		Mission AjoutMission(string nom, int serviceId);
		Mission AjoutMission(string nom);
		Collaborateur Authentifier(string mail, string motDePasse);
		Collaborateur ObtenirCollaborateur(int id);
		Collaborateur ObtenirCollaborateur(string idString);
		Mission GetMission(int idMission);
		void AjoutNoteDeFrais(int year, int idCollab, int month);
		void MiseAJourNotesDeFrais(string idString);
		void MiseAJourNotesDeFrais(int IdCollaborateur);
		void AjoutLigneDeFrais(int idCollab, int idNote, LigneDeFrais ligne);
        void AjoutNotif(int idCollab, Message m);
        void AssignerService(int idService, int idCollaborateur);
		void InitializeBdd();
		Service ObtenirServiceDeType(TypeService type);
		void EnvoiLigneDeFraisChefService(int idService, int idCollab, int idLigne);
	}
}
