using IntranetPOPS1819.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IntranetPOPS1819.ViewModel
{
	public class OngletNoteDeFraisViewModel
	{
		public Collaborateur Collaborateur { get; set; }
		public LigneDeFrais Frais { get; set; }
		public bool Authentifie { get; set; }
	}
}