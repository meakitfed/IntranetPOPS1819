﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace IntranetPOPS1819.Models
{
	public class Service
	{
		public Service()
		{
			Missions = new List<Mission>();
			Collaborateurs = new List<Collaborateur>();
		}

        public int Id { get; set; }
        public string Nom { get; set; }
		public virtual List<Mission> Missions { get; set; }
		public virtual List<Collaborateur> Collaborateurs { get; set; }
		public TypeService Type { get; set; } = TypeService.ServiceLambda;

		public virtual List<LigneDeFrais> LigneDeFrais { get; set; } = new List<LigneDeFrais>();
        public virtual List<Conge> Conges { get; set; } = new List<Conge>();
        /*public int ChefRefId { get; set; }
		[ForeignKey("ChefRefId")]
		public virtual Collaborateur Chef { get; set; }*/


        //Garder ? TODO
        public int NbAbsents { get; set; }

        public Collaborateur Chef()
        {
            foreach(Collaborateur c in Collaborateurs)
            {
                if (c.Chef) return c;
            }

            return null;
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

        public int GetNombreCollaborateursEnConges(System.DateTime date)
        {
            int NbAbsents = 0;

            if (Collaborateurs != null)
            {
                foreach(Collaborateur c in Collaborateurs)
                {
                    if (c.isEnConge(date)) NbAbsents++;
                }

                return NbAbsents;
            }
            else return 0;
        }

        public int GetNombreCollaborateursEnConges()
        {
            return GetNombreCollaborateursEnConges(System.DateTime.Today);
        }
    }

	public enum TypeService
	{
		Comptabilité,
		RessourcesHumaines,
		Direction,
		ServiceLambda
	}
}