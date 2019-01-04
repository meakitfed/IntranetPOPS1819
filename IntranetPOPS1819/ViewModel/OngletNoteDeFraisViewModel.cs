using IntranetPOPS1819.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IntranetPOPS1819.ViewModel
{
	public class OngletNoteDeFraisViewModel
	{
		public Collaborateur _Collaborateur { get; set; }
		public LigneDeFrais _Frais { get; set; }
		public bool _Authentifie { get; set; }
	}
}