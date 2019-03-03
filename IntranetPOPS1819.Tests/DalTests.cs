using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IntranetPOPS1819.Models;
using System.Data.Entity;
using System.Linq;

namespace IntranetTests.Tests
{

    [TestClass]
    public class DalTests
    {

        private Dal dal;

        [TestInitialize]
        public void Init_AvantChaqueTest()
        {
            IDatabaseInitializer<BddContext> init = new DropCreateDatabaseAlways<BddContext>();
            Database.SetInitializer(init);
            init.InitializeDatabase(new BddContext());

            dal = new Dal();
            dal.InitializeBdd();
        }

        [TestCleanup]
        public void ApresChaqueTest()
        {
            dal.Dispose();
        }


        [TestMethod]
        public void InitializeBdd_MissionsAssigneesAuxServices()
        {
            Service s = dal.bdd.Services.FirstOrDefault(serv => serv.Nom == "Comptabilité");
            Assert.IsNotNull(s);

            if (s != null)
            {
                Assert.IsTrue(s.Collaborateurs.Count > 0, "no collab");
                Assert.IsTrue(s.Missions.Count > 0, "no missions");
            }
        }
    }
}
