using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IntranetPOPS1819.Models;

namespace IntranetPOPS1819.ViewModel
{
	public class ComptaViewModel
	{
		public Collaborateur c {get; set;}
		public List<Service> Services { get; set; }
	}
}