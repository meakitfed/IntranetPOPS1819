using System;
using System.Collections.Generic;

namespace IntranetPOPS1819.Models
{
    public interface IDal : IDisposable
    {
        // Collaborateurs
        Collaborateur ObtenirCollaborateur(int id);
        Collaborateur ObtenirCollaborateur(string idString);
        List<Collaborateur> ObtenirTousLesCollaborateurs();
        List<Collaborateur> ObtenirCollaborateursService(int id);
        Collaborateur AjoutCollaborateur(string nom, string prenom, string mail, string mdp, string tel, bool chef = false, bool admin = false);
        Collaborateur AjoutCollaborateur(string nom, string prenom, string mail, string mdp, bool chef = false, bool admin = false);
        void AssignerChefDeService(int idCollab);
		Collaborateur ObtenirDirecteurFinancier();
		Collaborateur ObtenirPDG();

		// Services
		Service ObtenirServiceDeType(TypeService type);
        List<Service> ObtenirTousLesServices();
		Service AjoutService(string nom, TypeService type = TypeService.ServiceLambda);
		void AssignerService(int idService, int idCollaborateur);
		void EnleverChef(int idService);

        // Missions
        Mission GetMission(int idMission);
        Mission AjoutMission(string nom, int serviceId);
        Mission AjoutMission(string nom);
        void AssignerMission(int idMission, int idCollaborateur);
        void ChangerStatut(int id, StatutMission statut);

        // Authentification
        Collaborateur Authentifier(string mail, string motDePasse);

        // Notes de frais
        void AjoutNoteDeFrais(int year, int idCollab, int month);
        void MiseAJourNotesDeFrais(string idString);
        void MiseAJourNotesDeFrais(int IdCollaborateur);
        void AjoutLigneDeFrais(int idCollab, int idNote, LigneDeFrais ligne);
        void EnvoiNoteDeFraisChefService(int idService, int idCollab, int idNote);
        void ChangerStatutLigneDeFrais(int idLigne, StatutLigneDeFrais statut);
		void ChangerMissionLigneDeFrais(int idLigne, int idMission);
		void EnvoiNoteDeFrais(int idService, int idCollab, int idNote);

		// Congés
		void AjoutConge(int idCollab, Conge c);
        void AjoutConge(int idCollab, Conge c, TypeConge type);
        void ChangerStatut(int id, StatutConge s);                                      // Testé
        void EnvoiCongeChef(int idService, int idCollab, int idConge);
        void ValiderConge(int idCollab, int idConge);                                   // Testé
        void ModifierCongesRestant(int id, float jours);                                // Testé
        Conge ObtenirConge(int id);
        void SupprimerDemandeConge(int idCollab, int idConge);

        // Notifications
        void AjoutNotif(int idCollab, Message m);
		void EnvoiDemandeInformation(Message m);

		// BD
		void InitializeBdd();
        
	}
}
