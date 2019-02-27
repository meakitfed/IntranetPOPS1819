using System;
using System.Collections.Generic;
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


        public int GetDuree()
        {
            return (Fin - Debut).Days + 1;
        }

        public List<DateTime> GetAllDaysInConge()
        {
            List<DateTime> allDates = new List<DateTime>();

            for (DateTime date = Debut.Date; date <= Fin.Date; date = date.AddDays(1))
                allDates.Add(date);

            return allDates;
        }

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