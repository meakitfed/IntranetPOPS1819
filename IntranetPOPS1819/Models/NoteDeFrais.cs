using System;
using System.Collections.Generic;

namespace IntranetPOPS1819.Models
{
    public class NoteDeFrais
    {
        public int Id { get; set; }
        public StatutNote Statut { get; set; }
		public DateTime Date { get; set; }
		public bool Actif { get; set; } = false;
		public virtual List<LigneDeFrais> LignesDeFrais { get; set; } = new List<LigneDeFrais>();
		//public virtual Collaborateur Collaborateur { get; set; }

		public NoteDeFrais GetJSON()
		{
			NoteDeFrais n = new NoteDeFrais();
			foreach(LigneDeFrais l in this.LignesDeFrais)
			{
				n.LignesDeFrais.Add(new LigneDeFrais { Nom = l.Nom });
			}
			return n;
		}
	}

    public enum StatutNote
    {
        Brouillon,
        Enregistré
    }
}