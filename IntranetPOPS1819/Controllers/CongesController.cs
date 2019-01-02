using IntranetPOPS1819.Models;
using System.Web.Mvc;

namespace IntranetPOPS1819.Controllers
{
    public class CongesController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(Conges c)
        {
            return View();
        }
    }
}