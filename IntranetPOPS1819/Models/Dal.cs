using IntranetPOPS1819.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace IntranetPOPS1819.Models
{
	public class Dal : IDal
	{
		public BddContext bdd;

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

		public List<Collaborateur> ObtenirCollaborateursService(int id)
		{
			return bdd.Services.First(s => s.Id == id).Collaborateurs.ToList();
		}

		public List<Mission> ObtenirToutesLesMissions()
		{
			return bdd.Missions.ToList();
		}

		public List<Service> ObtenirTousLesServices()
		{
			return bdd.Services.ToList();
		}
		public void ChangerStatutLigneDeFrais(int idLigne, StatutLigneDeFrais statut)
		{
			LigneDeFrais ligne = bdd.LigneDeFrais.FirstOrDefault(l => l.Id == idLigne);
			ligne.Statut = statut;
			//ligne.Note.Collaborateur.Service.LigneDeFrais.Remove(ligne);
			bdd.SaveChanges();
		}
		public void MiseAJourNotesDeFrais(string idString)
		{
			int id;
			if (int.TryParse(idString, out id))
				MiseAJourNotesDeFrais(id);
		}
		public Service ObtenirServiceDeType(TypeService type)
		{
			return bdd.Services.FirstOrDefault(t => t.Type == type);
		}
		public Collaborateur ObtenirDirecteurFinancier()
		{
			Service service = bdd.Services.FirstOrDefault(s => s.Type == TypeService.Comptabilité);
			if(service != null)
			{
				return service.Chef();
			}
			return null;
		}
		public Collaborateur ObtenirPDG()
		{
			Service service = bdd.Services.FirstOrDefault(s => s.Type == TypeService.Direction);
			if (service != null)
			{
				return service.Chef();
			}
			return null;
		}

        public Collaborateur ObtenirDRH()
        {
            Service service = bdd.Services.FirstOrDefault(s => s.Type == TypeService.RessourcesHumaines);
            if (service != null)
            {
                return service.Chef();
            }
            return null;
        }

        public void SupprimerCollaborateur(int idCol)
        {
            Collaborateur col = bdd.Collaborateurs.First(c => c.Id == idCol);
            col.Licencie = true;
            bdd.SaveChanges();
        }

        public void EnvoiNoteDeFrais(int idService, int idCollab, int idNote)
		{
			Collaborateur c = bdd.Collaborateurs.FirstOrDefault(col => col.Id == idCollab);
			Service s = bdd.Services.FirstOrDefault(serv => serv.Id == idService);
			NoteDeFrais n = bdd.NotesDeFrais.FirstOrDefault(note => note.Id == idNote);
			if (c != null && s != null && n != null)
			{
				switch(n.Statut)
				{
					case StatutNote.Brouillon:
						n.Statut = StatutNote.EnAttenteDeValidation;
						if (c.Service.Type == TypeService.Direction)
						{
							//Cas PDG
							Collaborateur directeurFinancier = ObtenirDirecteurFinancier();
							AjoutNotif(directeurFinancier.Id, new Message(TypeMessage.NotifNoteAller, c, n));
							directeurFinancier.Service.NotesDeFrais.Add(n);
						}
						else if (c.Service.Type == TypeService.Comptabilité)
						{
							if (c.Chef)
							{
								Collaborateur pdg = ObtenirPDG();
								AjoutNotif(pdg.Id, new Message(TypeMessage.NotifNoteAller, c, n));
								pdg.Service.NotesDeFrais.Add(n);
								//Cas chef de service compta
							}
							else
							{
								Collaborateur comptaChef = c.Service.Chef();
								AjoutNotif(comptaChef.Id, new Message(TypeMessage.NotifNoteAller, c, n));
								comptaChef.Service.NotesDeFrais.Add(n);
								//Cas collab compta
							}
						}
						else
						{
							if (c.Chef)
							{
								Service compta = bdd.Services.FirstOrDefault(serv => serv.Type == TypeService.Comptabilité);
								//TODO envoyer notif à tout le service compta ? 
								//Collaborateur pdg = ObtenirPDG();
								//AjoutNotif(pdg.Id, new Message(TypeMessage.NotifNoteAller, c, n));
								compta.NotesDeFrais.Add(n);
								//Cas chef de service d'un autre service
							}
							else
							{
								Collaborateur chef = c.Service.Chef();
								AjoutNotif(chef.Id, new Message(TypeMessage.NotifNoteAller, c, n));
								chef.Service.NotesDeFrais.Add(n);
								//Cas collab d'un autre service
							}
						}
						break;
					case StatutNote.EnAttenteDeValidation:
						if (c.Service.Type == TypeService.Direction)
						{
							n.Statut = StatutNote.Validée;
							AjoutNotif(c.Id, new Message(TypeMessage.NotifNoteRetour, c, n));
							//Cas PDG
						}
						else if (c.Service.Type == TypeService.Comptabilité)
						{
							if (c.Chef)
							{
								n.Statut = StatutNote.Validée;
								AjoutNotif(c.Id, new Message(TypeMessage.NotifNoteRetour, c, n));
								//Cas chef de service compta
							}
							else
							{
								n.Statut = StatutNote.Validée;
								AjoutNotif(c.Id, new Message(TypeMessage.NotifNoteRetour, c, n));
								//Cas collab compta
							}
						}
						else
						{
							if (c.Chef)
							{
								n.Statut = StatutNote.Validée;
								AjoutNotif(c.Id, new Message(TypeMessage.NotifNoteRetour, c, n));
								//Cas chef de service d'un autre service
							}
							else
							{
								n.Statut = StatutNote.ValidéeParLeChef;
								AjoutNotif(c.Id, new Message(TypeMessage.NotifNoteRetour, c, n));
								//TODO envoyer notif à toute la compta faire fct
								//AjoutNotif(c.Id, new Message(TypeMessage.NotifNoteAller, c, n));
								Service compta = bdd.Services.FirstOrDefault(serv => serv.Type == TypeService.Comptabilité);
								compta.NotesDeFrais.Add(n);
								//Cas collab d'un autre service
							}
						}
						break;
					case StatutNote.ValidéeParLeChef:
						n.Statut = StatutNote.Validée;
						AjoutNotif(c.Id, new Message(TypeMessage.NotifNoteRetour, c, n));
						System.Diagnostics.Debug.WriteLine("Valider d'une note par le service Compta");
						break;
					default:
						break;
				}
			}
			bdd.SaveChanges();
		}
		public void EnvoiNoteDeFraisChefService(int idService, int idCollab, int idNote)
		{
			Collaborateur c = bdd.Collaborateurs.FirstOrDefault(col => col.Id == idCollab);
			Service s = bdd.Services.FirstOrDefault(serv => serv.Id == idService);
			NoteDeFrais n = bdd.NotesDeFrais.FirstOrDefault(note => note.Id == idNote);
			if (c != null && s != null && n != null)
			{
				s.NotesDeFrais.Add(n);
			}
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

		public void ChangerMissionLigneDeFrais(int idLigne, int idMission)
		{
			Mission mission = bdd.Missions.FirstOrDefault(m => m.Id == idMission);
			LigneDeFrais ligne = bdd.LigneDeFrais.FirstOrDefault(l => l.Id == idLigne);
			ligne.Mission = mission;
			bdd.SaveChanges();
		}

		public void EnleverChef(int idService)
		{
			Service service = bdd.Services.FirstOrDefault(c => idService == c.Id);
			foreach(Collaborateur c in service.Collaborateurs)
			{
				if(c.Chef)
				{
					c.Chef = false;
				}
			}
		}
		public List<Collaborateur> getChefRhEtCompta()
		{
			List<Collaborateur> l = new List<Collaborateur>();
			Collaborateur compta = bdd.Collaborateurs.FirstOrDefault(c => (c.Service.Type == TypeService.Comptabilité && c.Chef == true));
			if(compta != null) l.Add(compta);
			Collaborateur rh = bdd.Collaborateurs.FirstOrDefault(c => (c.Service.Type == TypeService.RessourcesHumaines && c.Chef == true));
			if (compta != null) l.Add(rh);
			return l;
		}
		public void AssignerChefDeService(int idCollab)
		{
			Collaborateur collab = bdd.Collaborateurs.FirstOrDefault(c => idCollab == c.Id);
			if(collab.Service != null)
			{
				EnleverChef(collab.Service.Id);
				collab.Chef = true;
			}
		}
		public void EnleverNoteDeFraisDuService(int IdNote, int IdService)
		{
			Service s = bdd.Services.FirstOrDefault(serv => serv.Id == IdService);
			NoteDeFrais n = bdd.NotesDeFrais.FirstOrDefault(note => note.Id == IdNote);
			s.NotesDeFrais.Remove(n);
			bdd.SaveChanges();
		}
		public void MessageDeRefusLigneDefrais(int IdLigne, string message)
		{
			LigneDeFrais ligne = bdd.LigneDeFrais.FirstOrDefault(l => l.Id == IdLigne);
			ligne.Commentaire = message;
			System.Diagnostics.Debug.WriteLine("Attribution d'une message de refus à la ligne " + ligne.Nom + " : " + message);
			bdd.SaveChanges();
		}
		public void EnvoiDemandeInformation(Message m)
		{
			Service service = bdd.Services.FirstOrDefault(c => TypeService.RessourcesHumaines == c.Type);
			service.Messages.Add(m);
			bdd.SaveChanges();
		}
		public void InitializeBdd()
		{
            try
            {
                //création collaborateurs
				Collaborateur nathan = AjoutCollaborateur("Bonnard", "Nathan", "nathan.bonnard@u-psud.fr", "mdp", "06 06 12 09 11");
				Collaborateur brian = AjoutCollaborateur("Martin", "Brian", "admin@gmail.com", "admin", "07 06 12 09 83", admin: true);

                Collaborateur didier = AjoutCollaborateur("Degroote", "Didier", "didier@gmail.com", "dede", "06 54 12 09 83");
                Collaborateur coco = AjoutCollaborateur("Manscour", "Corentin", "coconacros@gmail.com", "coco", "07 06 06 06 06");
                Collaborateur isabelle = AjoutCollaborateur("Soun", "Isabelle", "isabelle@gmail.com", "isa", "07 06 12 09 83");
                Collaborateur marie = AjoutCollaborateur("Henriot", "Marie-Christine", "marie@gmail.com", "mch", "06 13 63 32 18");
				Collaborateur jean = AjoutCollaborateur("Monrant", "Jean", "jean@gmail.com", "mdp", "06 28 15 32 25");
				Collaborateur luc = AjoutCollaborateur("Baton", "Luc", "luc@gmail.com", "mdp", "06 13 47 32 89");
                Collaborateur john = AjoutCollaborateur("Licencié", "John", "john@gmail.com", "mdp", "06 65 54 54 54");
                Collaborateur jeanne = AjoutCollaborateur("Jean", "Jeanne", "jeanne@gmail.com", "jeanne", "06 65 54 54 54");

                //création services
                Service direction = AjoutService("Direction", TypeService.Direction);
                AssignerService(direction.Id, marie.Id);
                AssignerChefDeService(marie.Id);

                Service compta = AjoutService("Comptabilité", TypeService.Comptabilité);
				AssignerService(compta.Id, didier.Id);
                AssignerService(compta.Id, coco.Id);
				AssignerService(compta.Id, luc.Id);
				AssignerService(compta.Id, jean.Id);
				AssignerChefDeService(didier.Id);

                Service rh = AjoutService("Ressource Humaines", TypeService.RessourcesHumaines);
				AssignerService(rh.Id, isabelle.Id);
                AssignerChefDeService(isabelle.Id);
                AssignerService(rh.Id, john.Id);
                AssignerService(rh.Id, jeanne.Id);

                string Demande =
				"Bonjour, \n" +
				"J'ai des inquiétudes pour ce qui est du projet Parking Velizy, je n'ai toujours pas de nouvelles de l'agent commercial\n" +
				"Serait-il possible de prendre contact par un autre biais ?\n" +
				"Merci d'avance,\n" +
				"Nathan.\n";
				EnvoiDemandeInformation(new Message { Contenu = (Demande), Emetteur = "Nathan" });
				EnvoiDemandeInformation(new Message { Contenu = "Salut toi 2", Emetteur = "Nathan 2 " });
				EnvoiDemandeInformation(new Message { Contenu = "Salut toi 3", Emetteur = "Nathan 3" });

				Service marketing = AjoutService("Marketing");
				AssignerService(marketing.Id, nathan.Id);
				AssignerService(marketing.Id, brian.Id);
				AssignerChefDeService(brian.Id);

                //??
				MiseAJourNotesDeFrais(nathan.Id);
				MiseAJourNotesDeFrais(brian.Id);
				MiseAJourNotesDeFrais(didier.Id);
				MiseAJourNotesDeFrais(isabelle.Id);
                MiseAJourNotesDeFrais(marie.Id);
                MiseAJourNotesDeFrais(coco.Id);
				MiseAJourNotesDeFrais(jean.Id);
				MiseAJourNotesDeFrais(luc.Id);
                MiseAJourNotesDeFrais(john.Id);
                MiseAJourNotesDeFrais(jeanne.Id);

                //tout le monde se voit assigner toutes les missions
                List<Mission> Missions = new List<Mission>();
				string[] labelsMission = { "Chantier Paris", "Parking Velizy", "Publicité", "Démarchage" };
				for (int j = 0; j < labelsMission.Length; j++)
				{
					Mission m = AjoutMission(labelsMission[j], compta.Id);
					AssignerMission(m.Id, nathan.Id);
					AssignerMission(m.Id, brian.Id);
                    AssignerMission(m.Id, didier.Id);
                    AssignerMission(m.Id, isabelle.Id);
                    AssignerMission(m.Id, marie.Id);
                    AssignerMission(m.Id, coco.Id);
                }
                Mission edm = AjoutMission("Etude de marché", marketing.Id);
                Mission es = AjoutMission("Enquête satisfaction", marketing.Id);
                AssignerMission(edm.Id, brian.Id);
                AssignerMission(edm.Id, nathan.Id);
                AssignerMission(es.Id, brian.Id);


                //Gestion congé
                AjoutConge(brian.Id, new Conge { Debut = new DateTime(2019, 10, 2), Fin = new DateTime(2019, 10, 10), Statut = StatutConge.EnCours });
				AjoutConge(nathan.Id, new Conge { Debut = new DateTime(2019, 10, 3), Fin = new DateTime(2019, 10, 10), Statut = StatutConge.EnCours });
				AjoutConge(nathan.Id, new Conge { Debut = new DateTime(2019, 10, 6), Fin = new DateTime(2019, 10, 10), Statut = StatutConge.EnCours });
				AjoutConge(brian.Id, new Conge { Debut = new DateTime(2019, 10, 4), Fin = new DateTime(2019, 10, 10), Statut = StatutConge.EnCours });
				AjoutConge(brian.Id, new Conge { Debut = new DateTime(2019, 10, 5), Fin = new DateTime(2019, 10, 10), Statut = StatutConge.EnCours });

                SupprimerCollaborateur(john.Id);
			}
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    System.Diagnostics.Debug.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        System.Diagnostics.Debug.WriteLine("- Property: \"{0}\", Value: \"{1}\", Error: \"{2}\"",
                            ve.PropertyName,
                            eve.Entry.CurrentValues.GetValue<object>(ve.PropertyName),
                            ve.ErrorMessage);
                    }
                }
                //throw;
            }
        }

		public void AjoutNoteDeFrais(int year, int idCollab, int month)
		{
			Collaborateur c = bdd.Collaborateurs.FirstOrDefault(collab => collab.Id == idCollab);
			if (c != null)
			{
				NoteDeFrais n = new NoteDeFrais { Date = new DateTime(year, month, 1), Statut = StatutNote.Brouillon, Actif = false, typeDuService = c.Service.Type };
				c.NotesDeFrais.Add(n);
				bdd.NotesDeFrais.Add(n);
				bdd.SaveChanges();
			}
		}
		public void AjoutLigneDeFrais(int idCollab, int idNote, LigneDeFrais ligne)
		{
			Collaborateur c = bdd.Collaborateurs.FirstOrDefault(collab => collab.Id == idCollab);
			System.Diagnostics.Debug.WriteLine("AjoutLigneDeFrais");
			if (c != null)
			{
				System.Diagnostics.Debug.WriteLine("c != null");
				NoteDeFrais note = c.NotesDeFrais.FirstOrDefault(n => n.Id == idNote);
				if(note != null)
				{
					System.Diagnostics.Debug.WriteLine("note != null");
					ligne.IdCollab = idCollab;
                    ligne.IdNote = idNote;
                    note.LignesDeFrais.Add(ligne);
					bdd.LigneDeFrais.Add(ligne);
					System.Diagnostics.Debug.WriteLine("Création ligne de frais dans la BDD");
					bdd.SaveChanges();
				}
			}
		}

        public int SupprimerNotif(int idNotif)
        {
            bdd.Messages.Remove(bdd.Messages.First(m => m.Id == idNotif));
            bdd.SaveChanges();
            return 0;
        }

        public void AjoutConge(int idCollab, Conge c)
        {
            c.Type = TypeConge.Defaut;
            bdd.Collaborateurs.FirstOrDefault(collab => collab.Id == idCollab).Conges.Add(c);
            bdd.Conges.Add(c);
            bdd.Collaborateurs.FirstOrDefault(collab => collab.Id == idCollab).CongesRestants -= c.GetDuree();
            bdd.SaveChanges();
        }

        public void AjoutConge(int idCollab, Conge c, TypeConge type)
        {
            c.Type = type;
            bdd.Collaborateurs.FirstOrDefault(collab => collab.Id == idCollab).Conges.Add(c);
            bdd.Conges.Add(c);
            bdd.Collaborateurs.FirstOrDefault(collab => collab.Id == idCollab).CongesRestants -= c.GetDuree();
            bdd.SaveChanges();
        }

        public void EnvoiCongeChef(int idService, int idCollab, int idConge)
        {
            Collaborateur c = bdd.Collaborateurs.FirstOrDefault(col => col.Id == idCollab);
            Service s = bdd.Services.FirstOrDefault(serv => serv.Id == idService);
            Conge l = bdd.Conges.FirstOrDefault(conge => conge.Id == idConge);
            
            if (c != null && s != null && l != null)
            {
                List<Conge> liste = bdd.Services.FirstOrDefault(serv => serv.Id == idService).Conges;
                liste.Add(l);
                bdd.Services.FirstOrDefault(serv => serv.Id == idService).Conges.Add(l);
                bdd.SaveChanges();
            }
        }
        
        public void AjoutNotif(int idCollab, Message m)
        {
            Collaborateur c = bdd.Collaborateurs.FirstOrDefault(collab => collab.Id == idCollab);
            if (c != null)
            {
                c.Notifications.Insert(0, m);

                bdd.SaveChanges();
            }
        }

        public Mission GetMission(int idMission)
		{
			return bdd.Missions.FirstOrDefault(m => m.Id == idMission);
		}

        //comme l'autre ajoutCol, mais avec le telephone en plus
		public Collaborateur AjoutCollaborateur(string nom, string prenom, string mail, string mdp, string tel, bool chef = false, bool admin = false)
        {
			
			Collaborateur c = new Collaborateur { Nom = nom, Prenom = prenom, Mail = mail , MotDePasse = EncodeMD5(mdp), Telephone = tel, Chef = chef, Admin = admin};
			bdd.Collaborateurs.Add(c);
			bdd.SaveChanges();
			return c;
        }

        public Collaborateur AjoutCollaborateur(string nom, string prenom, string mail, string mdp, bool chef = false, bool admin = false)
        {

            Collaborateur c = new Collaborateur { Nom = nom, Prenom = prenom, Mail = mail, MotDePasse = EncodeMD5(mdp), Chef = chef, Admin = admin };
            bdd.Collaborateurs.Add(c);
            bdd.SaveChanges();
            return c;
        }

        public Collaborateur AjoutCollaborateur(string nom, string prenom, string mail, string mdp, int idService, bool chef = false, bool admin = false)
        {

            Collaborateur c = new Collaborateur { Nom = nom, Prenom = prenom, Mail = mail, MotDePasse = EncodeMD5(mdp), Service = bdd.Services.First(serv => serv.Id == idService), Chef = chef, Admin = admin };
            bdd.Collaborateurs.Add(c);
            bdd.SaveChanges();
            return c;
        }

        public Service AjoutService(string nom, TypeService type = TypeService.ServiceLambda)
		{
			Service s = new Service { Nom = nom };
			s.Collaborateurs = new List<Collaborateur>();
			s.Missions = new List<Mission>();
			s.Type = type;
			bdd.Services.Add(s);
			bdd.SaveChanges();
			return s;
		}

		public Mission AjoutMission(string nom, int serviceId)
		{
			Service s = bdd.Services.FirstOrDefault(serv => serv.Id == serviceId);
			Mission m = new Mission { Nom = nom/*, Service = s */};
            s.Missions.Add(m);
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

        public Conge ObtenirConge(int id)
        {
            return bdd.Conges.First(c => c.Id == id);
        }

        public void ChangerStatut(int id, StatutConge s)
        {
            //System.Diagnostics.Debug.WriteLine("Changement de statut");
            bdd.Conges.FirstOrDefault(u => u.Id == id).Statut = s;
            //System.Diagnostics.Debug.WriteLine(bdd.Conges.FirstOrDefault(u => u.Id == id).Statut);
            //System.Diagnostics.Debug.WriteLine(bdd.Collaborateurs.Find(3).Conges.First(con => con.Id == id).Statut);
            bdd.SaveChanges();
        }

        public void ChangerStatut(int id, StatutMission s)
        {
            bdd.Missions.FirstOrDefault(m => m.Id == id).Statut = s;
            bdd.SaveChanges();
        }

        public void SupprimerMission(int id)
        {
            bdd.Missions.Remove(bdd.Missions.FirstOrDefault(m => m.Id == id));
            bdd.SaveChanges();
        }

        public void ValiderConge(int idCollab, int idConge)
        {
            Conge conge = bdd.Collaborateurs.First(col => col.Id == idCollab).Conges.FirstOrDefault(con => con.Id == idConge);
            ChangerStatut(conge.Id, StatutConge.Valide);
            int duree = (conge.Fin - conge.Debut).Days;
            ModifierCongesRestant(idCollab, duree);
            bdd.SaveChanges();
        }

        public void ModifierCongesRestant(int id, float jours)
        {
            bdd.Collaborateurs.First(c => c.Id == id).CongesRestants -= jours;
        }

        //A n'utiliser qu'à l'initialisation de la base de données, ou à la création d'un collaborateur
        public void AssignerService(int idService, int idCollaborateur)
		{
			Service s = bdd.Services.FirstOrDefault(serv => serv.Id == idService);
			Collaborateur c = bdd.Collaborateurs.FirstOrDefault(collab => collab.Id == idCollaborateur);
			s.Collaborateurs.Add(c);
			c.Service = s;
			bdd.SaveChanges();
		}

        //public void ChangerService(int idService, int idCollaborateur)
        //{
        //    Service s = bdd.Services.FirstOrDefault(serv => serv.Id == idService);
        //    Collaborateur c = bdd.Collaborateurs.FirstOrDefault(collab => collab.Id == idCollaborateur);
        //    s.Collaborateurs.Add(c);
        //    c.Service = s;
        //    bdd.SaveChanges();
        //}

        public void AssignerMission(int idMission, int idCollaborateur)
        {
            Mission m = bdd.Missions.FirstOrDefault(miss => miss.Id == idMission);
            Collaborateur c = bdd.Collaborateurs.FirstOrDefault(collab => collab.Id == idCollaborateur);
            c.Missions.Add(m);
            m.Collaborateurs.Add(c);
            bdd.SaveChanges();
        }

        public Collaborateur ObtenirCollaborateur(string idString)
		{
            if (int.TryParse(idString, out int id))
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

        public void SupprimerDemandeConge(int idCollab, int idConge)
        {
            Conge theConge = ObtenirConge(idConge);
            bdd.Collaborateurs.FirstOrDefault(collab => collab.Id == idCollab).CongesRestants += theConge.GetDuree();
            bdd.Conges.Remove(bdd.Conges.FirstOrDefault(c => c.Id == idConge));

            bdd.SaveChanges();
        }

        public void UpdateMission(MissionsViewModel m)
        {
            Mission mission = bdd.Missions.FirstOrDefault(miss => miss.Id == m.Id);
            mission.Nom = m.Nom;
            //mission.Statut = m.Statut;

            bdd.SaveChanges();
        }
	}
}