using System;
using System.ComponentModel.DataAnnotations;

namespace IntranetPOPS1819.Models
{
    public class Conge
    {
		public int Id { get; set; }

        public StatutConge Statut { get; set; }

        [Required]
        public TypeConge Type { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date de début")]
        public DateTime Debut { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date de fin")]
        public DateTime Fin { get; set; }

    }
}

public enum StatutConge
{
	EnCours,
    ValideChef,
	Valide,
	Refuse
}

public enum TypeConge
{
	RTT,
	SansSolde,
	Absence
}