using System;
using System.Collections.Generic;

namespace IntranetPOPS1819.Models
{
	public class NoteDeFrais
	{
		public int Id { get; set; }
		public StatutNote Statut { get; set; } = StatutNote.Brouillon;
		public TypeService typeDuService { get; set; }
		public DateTime Date { get; set; }
		public bool Actif { get; set; } = false;
		public virtual List<LigneDeFrais> LignesDeFrais { get; set; } = new List<LigneDeFrais>();
		//public virtual Collaborateur Collaborateur { get; set; }

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