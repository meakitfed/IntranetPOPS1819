using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IntranetPOPS1819.Models
{
	public class NoteDeFrais
	{
		public int Id { get; set; }
		public StatutNote Statut { get; set; } = StatutNote.Brouillon;
		public TypeService typeDuService { get; set; }
		[Display(Name = "Note De Frais Associé")]
		public DateTime Date { get; set; }
		public bool Actif { get; set; } = false;
		public virtual List<LigneDeFrais> LignesDeFrais { get; set; } = new List<LigneDeFrais>();
        //public virtual Collaborateur Collaborateur { get; set; }

        public int NbrRefusé()
        {
            int i = 0;
            foreach (LigneDeFrais l in LignesDeFrais)
            {
                if (l.Statut == StatutLigneDeFrais.Refusée)
                {
                    i++;
                }
            }
            return i;
        }
        public int NbrValidée()
        {
            int i = 0;
            foreach (LigneDeFrais l in LignesDeFrais)
            {
                if (l.Statut == StatutLigneDeFrais.Validée)
                {
                    i++;
                }
            }
            return i;
        }


        public int GetSommeValidee()
        {
            int somme = 0;
            foreach(LigneDeFrais ligne in LignesDeFrais)
            {
                if (ligne.Statut == StatutLigneDeFrais.Validée) somme += ligne.Somme;
            }

            return somme;
        }


        public int NbrValidéeChef()
        {
            int i = 0;
            foreach (LigneDeFrais l in LignesDeFrais)
            {
                if (l.Statut == StatutLigneDeFrais.ValidéeChef)
                {
                    i++;
                }
            }
            return i;
        }
        public int NbrEnAttente()
        {
            int i = 0;
            foreach (LigneDeFrais l in LignesDeFrais)
            {
                if (l.Statut == StatutLigneDeFrais.EnAttente)
                {
                    i++;
                }
            }
            return i;
        }
        public bool EstValidéeParLeChef()
		{
			foreach(LigneDeFrais l in LignesDeFrais)
			{
				if(l.Statut != StatutLigneDeFrais.ValidéeChef)
				{
					return false;
				}
			}
			return true;
		}
		public bool EstValidée()
		{
			foreach (LigneDeFrais l in LignesDeFrais)
			{
				if (l.Statut != StatutLigneDeFrais.Validée)
				{
					return false;
				}
			}
			return true;
		}
	}

    public enum StatutNote
    {
        Brouillon,
        EnAttenteDeValidation,
		ValidéeParLeChef,
		Validée,
    }
}