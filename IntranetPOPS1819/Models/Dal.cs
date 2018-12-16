using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace IntranetPOPS1819.Models
{
    public class Dal : IDal
    {
        private BddContext bdd;

        public Dal()
        {
            bdd = new BddContext();
        }

        public void Dispose()
        {
            bdd.Dispose();
        }

		public List<Collaborateur> ObtenirTousLesCollaborateur()
        {
            return bdd.Collaborateurs.ToList();
        }

        public List<Mission> ObtenirToutesLesMissions()
        {
            return bdd.Missions.ToList();
        }

        public List<Service> ObtenirTousLesServices()
        {
            return bdd.Services.ToList();
        }

		public void InitializeBdd()
		{
			Service Comptabilité = new Service { Nom = "Comptabilité" };
			Collaborateur nathan = new Collaborateur { Mail = "nathan.bonnard@u-psud.fr", Nom = "bonnard", Prenom = "nathan", MotDePasse = EncodeMD5("mdp"), Service = Comptabilité };
			Collaborateur brian = new Collaborateur { Mail = "admin@gmail.com", Nom = "Martin", Prenom = "Brian", MotDePasse = EncodeMD5("admin"), Service = Comptabilité };

			NoteDeFrais n1 = new NoteDeFrais { Actif = true, Date = DateTime.Today, Statut = StatutNote.Brouillon };
			NoteDeFrais n2 = new NoteDeFrais { Actif = false, Date = DateTime.Today, Statut = StatutNote.Enregistré };
			NoteDeFrais n3 = new NoteDeFrais { Actif = false, Date = DateTime.Today, Statut = StatutNote.Enregistré };

			nathan.NotesDeFrais.Add(n1);
			nathan.NotesDeFrais.Add(n2);
			nathan.NotesDeFrais.Add(n3);

			bdd.Services.Add(Comptabilité);
			bdd.Collaborateurs.Add(nathan);
			bdd.Collaborateurs.Add(brian);
			bdd.SaveChanges();
		}

		public Collaborateur AjoutCollaborateur(string nom, string prenom, string mail, string mdp)
        {
			Collaborateur c = new Collaborateur { Nom = nom, Prenom = prenom, Mail = mail , MotDePasse = EncodeMD5(mdp)};
			bdd.Collaborateurs.Add(c);
            bdd.SaveChanges();
			return c;
        }

		public Service AjoutService(string nom)
		{
			Service s = new Service { Nom = nom };
			s.Collaborateurs = new List<Collaborateur>();
			s.Missions = new List<Mission>();
			bdd.Services.Add(s);
			bdd.SaveChanges();
			return s;
		}

		public Mission AjoutMission(string nom, int serviceId)
		{
			Service s = bdd.Services.FirstOrDefault(serv => serv.Id == serviceId);
			Mission m = new Mission { Nom = nom, Service = s };
			bdd.Missions.Add(m);
			bdd.SaveChanges();
			return m;
		}

		public Mission AjoutMission(string nom)
		{
			Mission m = new Mission { Nom = nom };
			bdd.Missions.Add(m);
			bdd.SaveChanges();
			return m;
		}

		public Collaborateur ObtenirCollaborateur(int id)
		{
			return bdd.Collaborateurs.FirstOrDefault(u => u.Id == id);
		}

		public void AssignerService(int idService, int idCollaborateur)
		{
			Service s = bdd.Services.FirstOrDefault(serv => serv.Id == idService);
			Collaborateur c = bdd.Collaborateurs.FirstOrDefault(collab => collab.Id == idCollaborateur);
			c.Service = s;
			bdd.SaveChanges();
		}

		public Collaborateur ObtenirCollaborateur(string idString)
		{
			int id;
			if (int.TryParse(idString, out id))
				return ObtenirCollaborateur(id);
			return null;
		}

		public Collaborateur Authentifier(string mail, string motDePasse)
		{
			string motDePasseEncode = EncodeMD5(motDePasse);
			return bdd.Collaborateurs.FirstOrDefault(u => u.Mail == mail && u.MotDePasse == motDePasseEncode);
		}

		private string EncodeMD5(string motDePasse)
		{
			string motDePasseSel = "Encodage123" + motDePasse + "IntranetPOPS";
			return BitConverter.ToString(new MD5CryptoServiceProvider().ComputeHash(ASCIIEncoding.Default.GetBytes(motDePasseSel)));
		}
	}
}