﻿using IntranetPOPS1819.Models;
using IntranetPOPS1819.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IntranetPOPS1819.Controllers
{
	public class UserPageController : Controller
	{
		private IDal dal;

		public UserPageController() : this(new Dal())
		{

		}

		private UserPageController(IDal dalIoc)
		{
			dal = dalIoc;
		}

		public int SubmitDemandeInfo(string DemandeInformation, string DemandeTitre)
		{
			System.Diagnostics.Debug.WriteLine(DemandeTitre + DemandeInformation);
			if (HttpContext.User.Identity.IsAuthenticated)
			{
				Collaborateur c = dal.ObtenirCollaborateur(HttpContext.User.Identity.Name);
				dal.EnvoiDemandeInformation(new Message { Contenu = DemandeInformation, Emetteur = c.Prenom + " " + c.Nom });
				return 1;
			}
			return 0;
		}

		[Authorize]
		public ActionResult Profil()
        {
            CollaborateurViewModel viewModel = new CollaborateurViewModel { _Authentifie = HttpContext.User.Identity.IsAuthenticated };
            if(HttpContext.User.Identity.IsAuthenticated)
            {
                viewModel._Collaborateur = dal.ObtenirCollaborateur(HttpContext.User.Identity.Name);
            }
            return View(viewModel);
        }
    }
}