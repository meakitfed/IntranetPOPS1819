using IntranetPOPS1819.Models;
using System.Collections.Generic;

namespace IntranetPOPS1819.ViewModel
{
	public class ChefDeServiceViewModel
	{
		public Collaborateur _Collaborateur { get; set; }
        public List<MissionsViewModel> Missions { get; set; }
	}
}