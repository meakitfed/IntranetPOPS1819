using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
		void AssignerService(int idService, int idCollaborateur);
		void InitializeBdd();
	}
}
