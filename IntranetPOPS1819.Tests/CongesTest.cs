using IntranetPOPS1819.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Entity;

namespace IntranetTests.Tests
{
    [TestClass]
    public class CongesTest
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
        public void TestValidationFinale_OK()
        {
            Conge c = new Conge { Debut = new System.DateTime(2019, 2, 15), Fin = new System.DateTime(2019, 2, 17), Type = TypeConge.RTT };
            dal.AjoutConge(1, c);
            dal.ValiderConge(1, dal.ObtenirCollaborateur(1).Conges[0].Id);
            Assert.AreEqual(dal.ObtenirCollaborateur(1).Conges[0].Statut, StatutConge.Valide);
            Assert.AreEqual(dal.ObtenirCollaborateur(1).CongesRestants, 10);
        }
    }
}
