using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntranetPOPS1819.Models
{
    public class Message
    {
		public int Id { get; set; }
		public string Titre { get; set; }
        public DateTime Date { get; set; }
        public string Contenu { get; set; }
    }
}