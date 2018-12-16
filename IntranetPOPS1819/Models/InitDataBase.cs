using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace IntranetPOPS1819.Models
{
	public class InitDataBase : DropCreateDatabaseAlways<BddContext>
	{
		protected override void Seed(BddContext context)
		{
			base.Seed(context);
		}
	}
}