﻿using IntranetPOPS1819.Models;
using IntranetPOPS1819.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace IntranetPOPS1819.Controllers
{
    public class NoteDeFraisController : Controller
    {
		private IDal dal;

		public NoteDeFraisController() : this(new Dal())
		{

		}

		private NoteDeFraisController(IDal dalIoc)
		{
			dal = dalIoc;
		}

		[Authorize]
		public ActionResult Index()
		{
			System.Diagnostics.Debug.WriteLine("Passage dans Index GET NoteDeFraisControlleur");
			OngletNoteDeFraisViewModel vm = new OngletNoteDeFraisViewModel { _Authentifie = HttpContext.User.Identity.IsAuthenticated };
			if (HttpContext.User.Identity.IsAuthenticated)
			{
				dal.MiseAJourNotesDeFrais(HttpContext.User.Identity.Name);
				vm._Collaborateur = dal.ObtenirCollaborateur(HttpContext.User.Identity.Name);
			}
			return View(vm);
		}

		[HttpPost]
		public ActionResult Index(OngletNoteDeFraisViewModel vm)
		{
			if (HttpContext.User.Identity.IsAuthenticated)
			{
				vm._Collaborateur = dal.ObtenirCollaborateur(HttpContext.User.Identity.Name);
				System.Diagnostics.Debug.WriteLine("Passage dans Index HttpPost NoteDeFraisControlleur");
				//TODO valider le form?
				System.Diagnostics.Debug.WriteLine("Form pour créer une ligne de frais accepté");
				vm._Frais.Mission = dal.GetMission(vm._IdMission);
				foreach (NoteDeFrais n in vm._Collaborateur.NotesDeFrais)
				{
					if(n.Id == vm._IdNoteDeFrais)
					{
						dal.AjoutLigneDeFrais(vm._Collaborateur.Id, vm._IdNoteDeFrais, vm._Frais);
						if(vm._Frais.Complete)
						{
							dal.EnvoiLigneDeFraisChefService(vm._Collaborateur.Service.Id, vm._Collaborateur.Id, vm._Frais.Id);
							System.Diagnostics.Debug.WriteLine("Nombre de lignes :" + vm._Collaborateur.Service.LigneDeFrais.Count);
							System.Diagnostics.Debug.WriteLine("Collaborateur de la ligne : " + vm._Frais.Note.Collaborateur.Nom);
							IDal d = new Dal();
							System.Diagnostics.Debug.WriteLine("Collaborateur de la ligne test : " + d.ObtenirCollaborateur(vm._Collaborateur.Id).Service.LigneDeFrais[0].Note.Collaborateur.Nom);
						}
						return View(vm);
					}
				}
				return View(vm);
			}
			return View();

		}

		public ActionResult InformationLigneDeFrais(int idNote = default(int), int idLigne = default(int))
		{
			System.Diagnostics.Debug.WriteLine("Passage dans InformationLigneDeFrais Get NoteDeFraisControlleur");
			Collaborateur c = dal.ObtenirCollaborateur(HttpContext.User.Identity.Name);
			if(idNote != default(int) && idLigne != default(int))
			{
				LigneDeFrais ligne = c.NotesDeFrais.FirstOrDefault(n => n.Id == idNote).LignesDeFrais.FirstOrDefault(l => l.Id == idLigne);
				return PartialView(ligne);
			}
			return PartialView(null);
		}
	}
}