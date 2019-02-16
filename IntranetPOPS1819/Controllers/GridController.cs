﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using IntranetPOPS1819.Models;

namespace IntranetPOPS1819.Controllers
{
    public class GridController : Controller
    {
        private BddContext db = new BddContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Collaborateurs_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<Collaborateur> collaborateurs = db.Collaborateurs;
            DataSourceResult result = collaborateurs.ToDataSourceResult(request, collaborateur => new {
                Id = collaborateur.Id,
                Mail = collaborateur.Mail,
                MotDePasse = collaborateur.MotDePasse,
                Nom = collaborateur.Nom,
                Prenom = collaborateur.Prenom,
                CongésRestants = collaborateur.CongésRestants,
                Admin = collaborateur.Admin,
                Chef = collaborateur.Chef,
                Telephone = collaborateur.Telephone,
                LastUpdate = collaborateur.LastUpdate,
                LastUpdateNoteDeFrais = collaborateur.LastUpdateNoteDeFrais
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Collaborateurs_Create([DataSourceRequest]DataSourceRequest request, Collaborateur collaborateur)
        {
            if (ModelState.IsValid)
            {
                var entity = new Collaborateur
                {
                    Mail = collaborateur.Mail,
                    MotDePasse = collaborateur.MotDePasse,
                    Nom = collaborateur.Nom,
                    Prenom = collaborateur.Prenom,
                    CongésRestants = collaborateur.CongésRestants,
                    Admin = collaborateur.Admin,
                    Chef = collaborateur.Chef,
                    Telephone = collaborateur.Telephone,
                    LastUpdate = collaborateur.LastUpdate,
                    LastUpdateNoteDeFrais = collaborateur.LastUpdateNoteDeFrais
                };

                db.Collaborateurs.Add(entity);
                db.SaveChanges();
                collaborateur.Id = entity.Id;
            }

            return Json(new[] { collaborateur }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Collaborateurs_Update([DataSourceRequest]DataSourceRequest request, Collaborateur collaborateur)
        {
			System.Diagnostics.Debug.WriteLine("Passage dans Collaborateurs_Update pour " + collaborateur.Nom);
			System.Diagnostics.Debug.WriteLine(collaborateur.LastUpdate);
			if (ModelState.IsValid)
            {
                var entity = new Collaborateur
                {
                    Id = collaborateur.Id,
                    Mail = collaborateur.Mail,
                    MotDePasse = collaborateur.MotDePasse,
                    Nom = collaborateur.Nom,
                    Prenom = collaborateur.Prenom,
                    CongésRestants = collaborateur.CongésRestants,
                    Admin = collaborateur.Admin,
                    Chef = collaborateur.Chef,
                    Telephone = collaborateur.Telephone,
                    LastUpdate = collaborateur.LastUpdate,
                    LastUpdateNoteDeFrais = collaborateur.LastUpdateNoteDeFrais
                };

                db.Collaborateurs.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
				System.Diagnostics.Debug.WriteLine("Chnagement effectué pour " + collaborateur.Nom);
			}
			else
			{
				var errors = ModelState.Select(x => x.Value.Errors)
									   .Where(y => y.Count > 0)
									   .ToList();
				System.Diagnostics.Debug.WriteLine("Model State isn't valid : ");
				System.Diagnostics.Debug.WriteLine(errors);
				
			}

			return Json(new[] { collaborateur }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Collaborateurs_Destroy([DataSourceRequest]DataSourceRequest request, Collaborateur collaborateur)
        {
            if (ModelState.IsValid)
            {
                var entity = new Collaborateur
                {
                    Id = collaborateur.Id,
                    Mail = collaborateur.Mail,
                    MotDePasse = collaborateur.MotDePasse,
                    Nom = collaborateur.Nom,
                    Prenom = collaborateur.Prenom,
                    CongésRestants = collaborateur.CongésRestants,
                    Admin = collaborateur.Admin,
                    Chef = collaborateur.Chef,
                    Telephone = collaborateur.Telephone,
                    LastUpdate = collaborateur.LastUpdate,
                    LastUpdateNoteDeFrais = collaborateur.LastUpdateNoteDeFrais
                };

                db.Collaborateurs.Attach(entity);
                db.Collaborateurs.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { collaborateur }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
