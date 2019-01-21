using IntranetPOPS1819.Models;
using System.ComponentModel.DataAnnotations;

namespace IntranetPOPS1819.ViewModel
{
	public class NouveauServiceViewModel
	{
		public Collaborateur _Collaborateur { get; set; }
        [Required]
        [RegularExpression(@"[A-Z][a-z]+([ -][a-z]+)*", ErrorMessage = "Le nom du service doit commencer par une majuscule et ne pas contenir de chiffres")]
        public string Nom { get; set; }
		public Service _Service { get; set; }
	}
}