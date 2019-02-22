using System;
using System.ComponentModel.DataAnnotations;
namespace IntranetPOPS1819.ViewModel
{
    public class ValidationCongesViewModel
    {
        // Collaborateur
        public string Nom { get; set; }
        public string Service { get; set; }
        public float CongesRestants { get; set; }

        // Congé
        public int Id { get; set; }
        [DataType(DataType.Date)]
        public DateTime Debut { get; set; }
        [DataType(DataType.Date)]
        public DateTime Fin { get; set; }
    }
}