using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntranetPOPS1819.Models
{
    public class Congés
    {
		public int Id { get; set; }
        public StatutCongé Statut { get; set; }
        public TypeCongé Type { get; set; }
        public DateTime Date { get; set; }
        public int Durée { get; set; }
    }
}
public enum StatutCongé
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