using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.ModelBinding;
using System.Web.Script.Serialization;

namespace IntranetPOPS1819.Models
{
    public class Collaborateur
    {
		public Collaborateur()
		{
			LastUpdate = DateTime.Now;
		}

		//variables
		public int Id { get; set; }
		[Required]
		[RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage="Format incorrect")]
		public string Mail { get; set; }
		[Required]
		[Display(Name = "Mot de passe")]
		public string MotDePasse { get; set; }
		public string Nom { get; set; }
        public string Prenom { get; set; }
		public float CongesRestants { get; set; } = 10;
		//Garder ? TODO
		public bool Admin { get; set; } = false;
        public bool Chef { get; set; } = false;
        //[RegularExpression(@"^0[0-9]{9}$")]
		public string Telephone { get; set; }

		public virtual List<Mission> Missions { get; set; } = new List<Mission>();
		public virtual List<Mission> AnciennesMissions { get; set; } = new List<Mission>();
		public virtual List<Conge> Conges { get; set; } = new List<Conge>();
		public virtual List<NoteDeFrais> NotesDeFrais { get; set; } = new List<NoteDeFrais>();
		public virtual List<Message> Messages { get; set; } = new List<Message>();
		public virtual List<Message> Notifications { get; set; } = new List<Message>();

		[DataType(DataType.Date)]
		public DateTime LastUpdate { get; set; }
		public int LastUpdateNoteDeFrais { get; set; }

		/*[ForeignKey("Service")]
		public int ServiceRefId { get; set; }*/
		[Display(Name = "Service")]
		public virtual Service Service { get; set; }

        public bool Present { get; set; }
        
		public int GetNombreCongesPrisCetteAnnee()
		{
			int nb = 0;
			if(Conges != null)
			{
				foreach (Conge c in Conges)
				{
                    if (c.Debut.Year == DateTime.Now.Year  &&  c.Statut == StatutConge.Valide) nb += c.Fin.Subtract(c.Debut).Days;
				}
				return nb;
			}
			return 0;
		}

		public int GetNombreCongesEnAttente()
		{
			int nb = 0;
			if (Conges != null)
			{
				foreach (Conge c in Conges)
				{
                    if (c.Statut == StatutConge.EnCours) nb += c.Fin.Subtract(c.Debut).Days;
                }
				return nb;
			}
			return 0;
		}

        public List<DateTime> GetTousJoursCongesEnAttente()
        {
            List<DateTime> allDates = new List<DateTime>();
            if (Conges != null)
            {
                foreach (Conge c in Conges)
                {
                    if (c.Statut == StatutConge.EnCours  ||  c.Statut == StatutConge.ValideChef)
                        for (DateTime date = c.Debut.Date; date <= c.Fin.Date; date = date.AddDays(1))
                            allDates.Add(date);
                }
                return allDates;
            }
            return allDates;
        }


        public ValiditeConge isCongeValide(Conge c)
        {
            if (c.GetDuree() > CongesRestants) return ValiditeConge.errorPasAssezDeCongesRestants;
            else foreach(DateTime d1 in c.GetAllDaysInConge())
                {
                    foreach(Conge autreConge in Conges)
                    {
                        foreach (DateTime d2 in autreConge.GetAllDaysInConge()) if (d1.Date == d2.Date) return ValiditeConge.errorChevauchage;
                    }
                }

            return ValiditeConge.ok;
        }


        public List<DateTime> GetTousJoursCongesValides()
        {
            List<DateTime> allDates = new List<DateTime>();
            if (Conges != null)
            {
                foreach (Conge c in Conges)
                {
                    if (c.Statut == StatutConge.Valide)
                        for (DateTime date = c.Debut.Date; date <= c.Fin.Date; date = date.AddDays(1))
                            allDates.Add(date);
                }
                return allDates;
            }
            return allDates;
        }



        public List<String> GetJSONTousJoursCongesEnAttente()
        {
            var settings = new JsonSerializerSettings { DateFormatString = "yyyy-MM-dd" };

            List<String> allDates = new List<String>();
            if (Conges != null)
            {
                foreach (Conge c in Conges)
                {
                    if (c.Statut == StatutConge.EnCours)
                        for (DateTime date = c.Debut.Date; date <= c.Fin.Date; date = date.AddDays(1))
                            allDates.Add(JsonConvert.SerializeObject(date));
                }
                return allDates;
            }
            return allDates;
        }


        public int GetNombreCongesValidesFuturs()
		{
			int nb = 0;
			if (Conges != null)
			{
				foreach (Conge c in Conges)
				{
                    if (c.Statut == StatutConge.Valide) nb++;
				}
				return nb;
			}
			return 0;
		}

        public List<NoteDeFrais> GetNotesDeFraisAValider()
        {
            List<NoteDeFrais> liste = new List<NoteDeFrais>();
            for (int i = 0; i < Service.NotesDeFrais.Count; i++)
            {
                if(NotesDeFrais.Contains(Service.NotesDeFrais[i]))
                {
                    liste.Add(Service.NotesDeFrais[i]);
                }
            }
            return liste;
        }

        public bool isEnConge(System.DateTime date)
        {
            if (Conges != null)
            {
                foreach(Conge c in Conges)
                {
                    if (c.Statut == StatutConge.Valide && date.Date >= c.Debut.Date && date.Date <= c.Fin.Date) return true;
                }
            }

            return false;
        }

        public bool isEnConge()
        {
            return isEnConge(DateTime.Today);
        }

        public int CountNotificationsNonLues()
        {
            int nb = 0;
            foreach(Message m in Notifications) if (!m.Lu) nb++;

            return nb;
        }
    }
}

public enum ValiditeConge
{
    ok,
    errorChevauchage,
    errorPasAssezDeCongesRestants,
}