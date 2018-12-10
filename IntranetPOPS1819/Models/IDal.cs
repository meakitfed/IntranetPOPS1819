using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPOPS1819.Models
{
    public interface IDal : IDisposable
    {
        List<Collaborateurs> ObtenirTousLesCollaborateur();
		Collaborateurs AjoutCollaborateur(string nom, string prenom, string mail, string mdp);
		Service AjoutService(string nom);
		Mission AjoutMission(string nom, int serviceId);
		Mission AjoutMission(string nom);
		Collaborateurs Authentifier(string mail, string motDePasse);
	}
}
