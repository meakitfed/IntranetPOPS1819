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

		public List<Collaborateur> ObtenirTousLesCollaborateurs()
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

            Collaborateur nathan = new Collaborateur { Mail = "nathan.bonnard@u-psud.fr", Nom = "bonnard", Prenom = "nathan", MotDePasse = EncodeMD5("mdp") };
			Collaborateur brian = new Collaborateur { Mail = "admin@gmail.com", Nom = "Martin", Prenom = "Brian", MotDePasse = EncodeMD5("admin"), Admin = true };
            Collaborateur didier = new Collaborateur { Mail = "didier@gmail.com", Nom = "Degroote", Prenom = "Didier", MotDePasse = EncodeMD5("dede") };
            Collaborateur isabelle = new Collaborateur { Mail = "isabelle@gmail.com", Nom = "Soun", Prenom = "Isabelle", MotDePasse = EncodeMD5("isa") };
            List<Collaborateur> collabos = new List<Collaborateur>
            {
                nathan,
                brian,
                didier,
                isabelle
            };

            NoteDeFrais n1 = new NoteDeFrais { Actif = true, Date = new DateTime(2019, 1, 1), Statut = StatutNote.Brouillon };
			NoteDeFrais n2 = new NoteDeFrais { Actif = false, Date = new DateTime(2018, 12, 1), Statut = StatutNote.Enregistré };
			NoteDeFrais n3 = new NoteDeFrais { Actif = false, Date = new DateTime(2018, 11, 1), Statut = StatutNote.Enregistré };
			NoteDeFrais n4 = new NoteDeFrais { Actif = false, Date = new DateTime(2018, 10, 1), Statut = StatutNote.Enregistré };
			NoteDeFrais n5 = new NoteDeFrais { Actif = false, Date = new DateTime(2018, 9, 1), Statut = StatutNote.Enregistré };
			NoteDeFrais n6 = new NoteDeFrais { Actif = false, Date = new DateTime(2018, 8, 1), Statut = StatutNote.Enregistré };
			NoteDeFrais n7 = new NoteDeFrais { Actif = false, Date = new DateTime(2018, 7, 1), Statut = StatutNote.Enregistré };

			nathan.NotesDeFrais.Add(n1);
			nathan.NotesDeFrais.Add(n2);
			nathan.NotesDeFrais.Add(n3);
			nathan.NotesDeFrais.Add(n4);
			nathan.NotesDeFrais.Add(n5);
			nathan.NotesDeFrais.Add(n6);
			nathan.NotesDeFrais.Add(n7);

            Service compta = new Service { Nom = "Comptabilité", Chef = didier };
            Service rh = new Service { Nom = "RH", Chef = isabelle };
            List<Service> services = new List<Service>();
            services.Add(compta);
            services.Add(rh);

            Random r = new Random();
			List<Mission> Missions = new List<Mission>();
			string[] labelsMission = { "Chantier Paris", "Parking Velizy", "Publicité", "Démarchage" };
			for (int j = 0; j < labelsMission.Length; j++)
			{
				int rand = r.Next(0, labelsMission.Length);
				Missions.Add(new Mission { Nom = labelsMission[rand], Service = compta, Statut = StatutMission.EnCours});
			}

			string[] labelsLigne = { "Restaurant", "Taxi", "Avion", "Péage", "Essence" };
			foreach (NoteDeFrais n in nathan.NotesDeFrais)
			{
				for (int j = 0; j < 5; j++)
				{
					int rand = r.Next(0, labelsLigne.Length);
					int rand2 = r.Next(0, Missions.Count);
					LigneDeFrais ligne = new LigneDeFrais { Nom = labelsLigne[rand], Complete = true, Mission = Missions[rand2], Somme = rand * rand2 * 5, Statut = (n.Actif ? StatutLigneDeFrais.EnAttente : StatutLigneDeFrais.Validée) };
					n.LignesDeFrais.Add(ligne);
				}
			}
			foreach(Mission m in Missions)
			{
				nathan.Missions.Add(m);
				bdd.Missions.Add(m);
			}
			foreach (NoteDeFrais n in nathan.NotesDeFrais)
			{
				bdd.NotesDeFrais.Add(n);
			}

            foreach(Service s in services)
                bdd.Services.Add(s);
            foreach(Collaborateur c in collabos)
                bdd.Collaborateurs.Add(c);
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