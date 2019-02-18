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
using Newtonsoft.Json;

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
                CongesRestants = collaborateur.CongesRestants,
                Admin = collaborateur.Admin,
                Chef = collaborateur.Chef,
                Telephone = collaborateur.Telephone,
                LastUpdate = collaborateur.LastUpdate,
                LastUpdateNoteDeFrais = collaborateur.LastUpdateNoteDeFrais,
				Service = collaborateur.Service,

			});
			/*JsonSerializerSettings serializationSettings = new JsonSerializerSettings()
			{
				ReferenceLoopHandling = ReferenceLoopHandling.Ignore
			};

			result = JsonConvert.SerializeObject(result, serializationSettings);*/
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
