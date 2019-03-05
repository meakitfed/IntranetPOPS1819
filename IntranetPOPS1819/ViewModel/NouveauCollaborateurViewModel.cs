using IntranetPOPS1819.Models;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace IntranetPOPS1819.ViewModel
{
	public class NouveauCollaborateurViewModel
	{
        [Required]
        [RegularExpression(@"[A-Z][a-z]+([ -][a-z]+)*", ErrorMessage = "Le nom doit commencer par une majuscule et ne pas contenir de chiffres")]
		public string Nom { get; set; }
        [Required]
        [RegularExpression(@"[A-Z][a-z]+([ -][a-z]+)*", ErrorMessage = "Le prénom doit commencer par une majuscule et ne pas contenir de chiffres")]
        public string Prenom { get; set; }
        [Required]
        [RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage = "Format incorrect")]
        public string Mail { get; set; }
        public string Service { get; set; }
        public Collaborateur _Collaborateur { get; set; }
	}
}