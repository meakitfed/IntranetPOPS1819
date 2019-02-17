using System;
using System.Collections.Generic;
using System.Data.Entity;
using IntranetPOPS1819.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntranetTests.Tests
{
    [TestClass]
    public class CollaborateursTests
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
        public void CreerCreerCollaborateur_AvecUnNouveauCreerCollaborateur_ObtientTousLesCreerCollaborateurRenvoitBienLeCreerCollaborateur()
        {
            Collaborateur n = dal.AjoutCollaborateur("Minh", "Nguyen", "minh.nguyen@u-psud.fr", "mdp");

            Assert.IsNotNull(n);
            Assert.AreEqual("Minh", dal.ObtenirCollaborateur(n.Id).Nom);
            Assert.AreEqual("Nguyen", dal.ObtenirCollaborateur(n.Id).Prenom);
            Assert.AreEqual("minh.nguyen@u-psud.fr", dal.ObtenirCollaborateur(n.Id).Mail);
        }

        [TestMethod]
        public void AssignerServiceACollaborateur()
        {
            Service compta = dal.AjoutService("Comptabilité");
            Collaborateur n = dal.AjoutCollaborateur("Minh", "Nguyen", "minh.nguyen@u-psud.fr", "bonmotdepasse");
            dal.AssignerService(compta.Id, n.Id);
            Assert.AreEqual(compta, dal.ObtenirCollaborateur(n.Id).Service);
        }
    }
}
