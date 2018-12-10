using IntranetPOPS1819.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntranetPOPS1819.ViewModel
{
	public class CollaborateurViewModel
	{
		public Collaborateurs Collaborateur { get; set; }
		public bool Authentifie { get; set; }
	}
}