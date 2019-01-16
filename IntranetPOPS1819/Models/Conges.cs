using System;

namespace IntranetPOPS1819.Models
{
    public class Conges
    {
		public int Id { get; set; }
        public StatutConge Statut { get; set; }
        public TypeCongé Type { get; set; }
        public DateTime Debut { get; set; }
        public DateTime Fin { get; set; }
    }
}

public enum StatutConge
{
	EnCours,
	Validé,
	Refusé
}

public enum TypeCongé
{
	RTT,
	SansSolde,
	Absence
}