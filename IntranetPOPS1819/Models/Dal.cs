﻿using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
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
        public void EnvoiNoteDeFraisChefService(int idService, int idCollab, int idNote)
        {
            Collaborateur c = bdd.Collaborateurs.FirstOrDefault(col => col.Id == idCollab);
            Service s = bdd.Services.FirstOrDefault(serv => serv.Id == idService);
            NoteDeFrais n = bdd.NotesDeFrais.FirstOrDefault(note => note.Id == idNote);
            if (c != null && s != null && n != null)
            {
                s.NotesDeFrais.Add(n);
                bdd.SaveChanges();
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
            foreach (Collaborateur c in service.Collaborateurs)
            {
                if (c.Chef)
                {
                    c.Chef = false;
                }
            }
        }
        public void AssignerChefDeService(int idCollab)
        {
            Collaborateur collab = bdd.Collaborateurs.FirstOrDefault(c => idCollab == c.Id);
            if (collab.Service != null)
            {
                EnleverChef(collab.Service.Id);
                collab.Chef = true;
            }
        }
        public void InitializeBdd()
        {
            try
            {
                Collaborateur nathan = AjoutCollaborateur("Bonnard", "Nathan", "nathan.bonnard@u-psud.fr", "mdp");
                Collaborateur brian = AjoutCollaborateur("Martin", "Brian", "admin@gmail.com", "admin");
                Collaborateur didier = AjoutCollaborateur("Degroote", "Didier", "didier@gmail.com", "dede");
                Collaborateur isabelle = AjoutCollaborateur("Soun", "Isabelle", "isabelle@gmail.com", "isa");

                Service compta = AjoutService("Comptabilité", TypeService.Comptabilité);
                AssignerService(compta.Id, didier.Id);

                Service rh = AjoutService("Ressource Humaines", TypeService.RessourcesHumaines);
                AssignerService(compta.Id, isabelle.Id);

                Service marketing = AjoutService("Marketing");
                AssignerService(compta.Id, nathan.Id);
                AssignerService(compta.Id, brian.Id);
                AssignerChefDeService(brian.Id);

                List<Mission> Missions = new List<Mission>();
                string[] labelsMission = { "Chantier Paris", "Parking Velizy", "Publicité", "Démarchage" };
                for (int j = 0; j < labelsMission.Length; j++)
                {
                    Mission m = AjoutMission(labelsMission[j], compta.Id);
                    AssignerMission(m.Id, nathan.Id);
                    AssignerMission(m.Id, brian.Id);
                }
                AjoutConge(brian.Id, new Conge { Debut = new DateTime(2019, 10, 2), Fin = new DateTime(2019, 10, 10), Statut = StatutConge.EnCours });
                AjoutConge(nathan.Id, new Conge { Debut = new DateTime(2019, 10, 3), Fin = new DateTime(2019, 10, 10), Statut = StatutConge.EnCours });
                AjoutConge(nathan.Id, new Conge { Debut = new DateTime(2019, 10, 6), Fin = new DateTime(2019, 10, 10), Statut = StatutConge.EnCours });
                AjoutConge(brian.Id, new Conge { Debut = new DateTime(2019, 10, 4), Fin = new DateTime(2019, 10, 10), Statut = StatutConge.EnCours });
                AjoutConge(brian.Id, new Conge { Debut = new DateTime(2019, 10, 5), Fin = new DateTime(2019, 10, 10), Statut = StatutConge.EnCours });
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
                NoteDeFrais n = new NoteDeFrais { Date = new DateTime(year, month, 1), Statut = StatutNote.Brouillon, Actif = false };
                c.NotesDeFrais.Add(n);
                bdd.NotesDeFrais.Add(n);
                bdd.SaveChanges();
            }
        }
        public void AjoutLigneDeFrais(int idCollab, int idNote, LigneDeFrais ligne)
        {
            Collaborateur c = bdd.Collaborateurs.FirstOrDefault(collab => collab.Id == idCollab);
            if (c != null)
            {
                NoteDeFrais note = c.NotesDeFrais.FirstOrDefault(n => n.Id == idNote);
                if (note != null)
                {
                    //ligne.Note = note;

                    note.LignesDeFrais.Add(ligne);
                    bdd.LigneDeFrais.Add(ligne);
                    System.Diagnostics.Debug.WriteLine("Création ligne de frais dans la BDD");
                    bdd.SaveChanges();
                }
            }
        }

        public void AjoutConge(int idCollab, Conge c)
        {
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

        public Collaborateur AjoutCollaborateur(string nom, string prenom, string mail, string mdp)
        {

            Collaborateur c = new Collaborateur { Nom = nom, Prenom = prenom, Mail = mail, MotDePasse = EncodeMD5(mdp) };
            bdd.Collaborateurs.Add(c);
            bdd.SaveChanges();
            System.Diagnostics.Debug.WriteLine(c.Id);
            MiseAJourNotesDeFrais(c.Id);
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

        public void AssignerService(int idService, int idCollaborateur)
        {
            Service s = bdd.Services.FirstOrDefault(serv => serv.Id == idService);
            Collaborateur c = bdd.Collaborateurs.FirstOrDefault(collab => collab.Id == idCollaborateur);
            s.Collaborateurs.Add(c);
            c.Service = s;
            bdd.SaveChanges();
        }

        public void AssignerMission(int idMission, int idCollaborateur)
        {
            Mission m = bdd.Missions.FirstOrDefault(miss => miss.Id == idMission);
            Collaborateur c = bdd.Collaborateurs.FirstOrDefault(collab => collab.Id == idCollaborateur);
            c.Missions.Add(m);
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
    }
}