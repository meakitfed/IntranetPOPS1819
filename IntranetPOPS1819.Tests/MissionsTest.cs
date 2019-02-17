using System;
using System.Data.Entity;
using IntranetPOPS1819.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntranetTests.Tests
{
    [TestClass]
    public class MissionsTest
    {
        private IDal dal;

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
        public void TestAssignation_OK()
        {
            Mission m = dal.GetMission(1);
            dal.AssignerMission(m.Id, 1);
            Assert.IsTrue(dal.ObtenirCollaborateur(1).Missions.Contains(m));
        }

        [TestMethod]
        public void TestChangementStatut_OK()
        {
            Mission m = dal.GetMission(1);
            dal.ChangerStatut(m.Id, StatutMission.Annulée);
            Assert.IsTrue(dal.GetMission(m.Id).Statut == StatutMission.Annulée);
        }
    }
}
