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
		public void MiseAJourNotesDeFrais(string idString)
		{
			int id;
			if (int.TryParse(idString, out id))
				MiseAJourNotesDeFrais(id);
		}

		public void MiseAJourNotesDeFrais(int IdCollaborateur)
		{
			Collaborateur c = bdd.Collaborateurs.FirstOrDefault(collab => collab.Id == IdCollaborateur);
			if (c != null)
			{
				if (DateTime.Today != c.LastUpdate.Date)
				{
					if (c.NotesDeFrais.Count == 0)
					{
						AjoutNoteDeFrais(c.LastUpdate.Year, IdCollaborateur, c.LastUpdate.Month);
					}
					DateTime d = c.LastUpdate;
					d = d.AddMonths(1);
					while (d < DateTime.Now)
					{
						AjoutNoteDeFrais(d.Year, IdCollaborateur, d.Month);
						d = d.AddMonths(1);
					}

					foreach (NoteDeFrais n in c.NotesDeFrais)
					{
						n.Actif = false;
					}
					c.NotesDeFrais[c.NotesDeFrais.Count - 1].Actif = true;
					c.LastUpdate = DateTime.Now;
				}
				else
				{
					System.Diagnostics.Debug.WriteLine("Passage MiseAJourNotesDeFrais, sans mise à jour");
				}
				bdd.SaveChanges();
			}
		}
		public void InitializeBdd()
		{
            Collaborateur nathan = new Collaborateur { Mail = "nathan.bonnard@u-psud.fr", Nom = "bonnard", Prenom = "nathan", MotDePasse = EncodeMD5("mdp") };
			nathan.LastUpdate = new DateTime(2018, 1, 1);
			Collaborateur brian = new Collaborateur { Mail = "admin@gmail.com", Nom = "Martin", Prenom = "Brian", MotDePasse = EncodeMD5("admin"), Admin = true };
			brian.LastUpdate = new DateTime(2017, 1, 1);
			Collaborateur didier = new Collaborateur { Mail = "didier@gmail.com", Nom = "Degroote", Prenom = "Didier", MotDePasse = EncodeMD5("dede"), Chef = true };
            Collaborateur isabelle = new Collaborateur { Mail = "isabelle@gmail.com", Nom = "Soun", Prenom = "Isabelle", MotDePasse = EncodeMD5("isa"), Chef = true };

            Service compta = new Service { Nom = "Comptabilité", Collaborateurs = { didier } };
            Service rh = new Service { Nom = "RH" };
            
			didier.Service = compta;
			isabelle.Service = rh;

			List<Service> services = new List<Service>
			{
				compta,
				rh
			};
			List<Collaborateur> collabos = new List<Collaborateur>
            {
                nathan,
                brian,
                didier,
                isabelle
            };
			

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

            foreach(Service s in services)
                bdd.Services.Add(s);
            foreach(Collaborateur c in collabos)
                bdd.Collaborateurs.Add(c);
            bdd.SaveChanges();
        }
		public void AjoutNoteDeFrais(int year, int idCollab, int month)
		{
			Collaborateur c = bdd.Collaborateurs.FirstOrDefault(collab => collab.Id == idCollab);
			if (c != null)
			{
				c.NotesDeFrais.Add(new NoteDeFrais { Date = new DateTime(year, month, 1), Statut = StatutNote.Brouillon, Actif = false });
				bdd.SaveChanges();
			}
		}
		public void AjoutLigneDeFrais(int idCollab, int idNote, LigneDeFrais ligne)
		{
			Collaborateur c = bdd.Collaborateurs.FirstOrDefault(collab => collab.Id == idCollab);
			if(c != null)
			{
				NoteDeFrais note = c.NotesDeFrais.FirstOrDefault(n => n.Id == idNote);
				if(note != null)
				{
					note.LignesDeFrais.Add(ligne);
					System.Diagnostics.Debug.WriteLine("Création ligne de frais dans la BDD");
					bdd.SaveChanges();
				}
			}
		}

        public void AjoutNotif(int idCollab, Message m)
        {
            Collaborateur c = bdd.Collaborateurs.FirstOrDefault(collab => collab.Id == idCollab);
            if (c != null)
            {
                c.Notifications.Add(m);
                bdd.SaveChanges();
            }
        }

        public Mission GetMission(int idMission)
		{
			return bdd.Missions.FirstOrDefault(m => m.Id == idMission);
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