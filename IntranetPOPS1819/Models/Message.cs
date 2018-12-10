using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntranetPOPS1819.Models
{
    public class Message
    {
		public int Id { get; set; }
		public String Titre { get; set; }
        public DateTime Date { get; set; }
        public String Contenu { get; set; }
    }
}