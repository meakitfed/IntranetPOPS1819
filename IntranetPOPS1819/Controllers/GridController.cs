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

        public ActionResult LigneDeFrais_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<LigneDeFrais> lignedefrais = db.LigneDeFrais;
            DataSourceResult result = lignedefrais.ToDataSourceResult(request, ligneDeFrais => new {
                Id = ligneDeFrais.Id,
                Nom = ligneDeFrais.Nom,
                Somme = ligneDeFrais.Somme,
                Complete = ligneDeFrais.Complete,
                Statut = ligneDeFrais.Statut
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LigneDeFrais_Create([DataSourceRequest]DataSourceRequest request, LigneDeFrais ligneDeFrais)
        {
            if (ModelState.IsValid)
            {
                var entity = new LigneDeFrais
                {
                    Nom = ligneDeFrais.Nom,
                    Somme = ligneDeFrais.Somme,
                    Complete = ligneDeFrais.Complete,
                    Statut = ligneDeFrais.Statut
                };

                db.LigneDeFrais.Add(entity);
                db.SaveChanges();
                ligneDeFrais.Id = entity.Id;
            }

            return Json(new[] { ligneDeFrais }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LigneDeFrais_Update([DataSourceRequest]DataSourceRequest request, LigneDeFrais ligneDeFrais)
        {
            if (ModelState.IsValid)
            {
                var entity = new LigneDeFrais
                {
                    Id = ligneDeFrais.Id,
                    Nom = ligneDeFrais.Nom,
                    Somme = ligneDeFrais.Somme,
                    Complete = ligneDeFrais.Complete,
                    Statut = ligneDeFrais.Statut
                };

                db.LigneDeFrais.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { ligneDeFrais }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LigneDeFrais_Destroy([DataSourceRequest]DataSourceRequest request, LigneDeFrais ligneDeFrais)
        {
            if (ModelState.IsValid)
            {
                var entity = new LigneDeFrais
                {
                    Id = ligneDeFrais.Id,
                    Nom = ligneDeFrais.Nom,
                    Somme = ligneDeFrais.Somme,
                    Complete = ligneDeFrais.Complete,
                    Statut = ligneDeFrais.Statut
                };

                db.LigneDeFrais.Attach(entity);
                db.LigneDeFrais.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { ligneDeFrais }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
