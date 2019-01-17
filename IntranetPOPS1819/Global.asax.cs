using IntranetPOPS1819.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace IntranetPOPS1819
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);

			IDatabaseInitializer<BddContext> init = new DropCreateDatabaseAlways<BddContext>();
			Database.SetInitializer(init);
			init.InitializeDatabase(new BddContext());
			Dal d = new Dal();
			d.InitializeBdd();

		}
    }
}
