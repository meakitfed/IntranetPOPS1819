using IntranetPOPS1819.Models;

namespace IntranetPOPS1819.ViewModel
{
	public class OngletNoteDeFraisViewModel
	{
		public Collaborateur _Collaborateur { get; set; }
		public LigneDeFrais _Frais { get; set; }
		public bool _Authentifie { get; set; }
		public int _IdMission { get; set; }
		public int _IdNoteDeFrais { get; set; }
	}
}