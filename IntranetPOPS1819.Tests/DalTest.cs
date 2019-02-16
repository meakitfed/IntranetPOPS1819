using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IntranetPOPS1819.Models;
using System.Data.Entity;
using System.Collections.Generic;

namespace IntranetPOPS1819.Tests
{
    [TestClass]
    public class DalTest
    {
			private IDal dal;

			[TestInitialize]
            public void Init_AvantChaqueTest()
            {
				IDatabaseInitializer<BddContext> init = new DropCreateDatabaseAlways<BddContext>();
				Database.SetInitializer(init);
				init.InitializeDatabase(new BddContext());

				dal = new Dal();
			}

			[TestCleanup]
			public void ApresChaqueTest()
			{
				dal.Dispose();
			}

			[TestMethod]
            public void CreerCreerCollaborateur_AvecUnNouveauCreerCollaborateur_ObtientTousLesCreerCollaborateurRenvoitBienLeCreerCollaborateur()
            {
                    dal.AjoutCollaborateur("Minh", "Nguyen", "minh.nguyen@u-psud.fr", "mdp");
                    List<Collaborateur> collab = dal.ObtenirTousLesCollaborateurs();

                    Assert.IsNotNull(collab);
                    Assert.AreEqual(1, collab.Count);
                    Assert.AreEqual("Minh", collab[0].Nom);
                    Assert.AreEqual("Nguyen", collab[0].Prenom);
                    Assert.AreEqual("minh.nguyen@u-psud.fr", collab[0].Mail);
            }

			[TestMethod]
			public void CreerService_CreerMissionAvecService_MissionPresentDansService()
			{
					Service s = dal.AjoutService("Comptabilité");
					Mission m = dal.AjoutMission("ProjetGL", s.Id);

					Assert.AreEqual(s.Missions[0], m);
					Assert.AreEqual(s, m.Service);
			}

			[TestMethod]
			public void CreerCollaborateur_CheckConnexion()
			{
				Collaborateur c = dal.AjoutCollaborateur("Minh", "Nguyen", "minh.nguyen@u-psud.fr", "bonmotdepasse");
				Assert.AreEqual(null, dal.Authentifier("minh.nguyen@u-psud.fr", "mauvaismotdepasse"));
				Assert.AreEqual(c, dal.Authentifier("minh.nguyen@u-psud.fr", "bonmotdepasse"));
			}

			[TestMethod]
			public void AssignerServiceACollaborateur()
			{
				Service compta = dal.AjoutService("Comptabilité");
				Collaborateur n = dal.AjoutCollaborateur("Minh", "Nguyen", "minh.nguyen@u-psud.fr", "bonmotdepasse");
				dal.AssignerService(compta.Id, n.Id);
				Assert.AreEqual(compta, dal.ObtenirTousLesCollaborateurs()[0].Service);
			}
		}
}
