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

        public ActionResult LigneDeFrais_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<LigneDeFrais> lignedefrais = db.LigneDeFrais;
            DataSourceResult result = lignedefrais.ToDataSourceResult(request, ligneDeFrais => new {
                Id = ligneDeFrais.Id,
                Nom = ligneDeFrais.Nom,
                Somme = ligneDeFrais.Somme,
                Type = ligneDeFrais.Type,
                Complete = ligneDeFrais.Complete,
                Statut = ligneDeFrais.Statut,
                Date = ligneDeFrais.Date,
                ResumeFileUrl = ligneDeFrais.ResumeFileUrl,
                Filename = ligneDeFrais.Filename
            });

            return Json(result);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
