using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCWEF.Controllers
{
    public class PaybalController : Controller
    {
        // GET: Paybal
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetDataPaybal()
        {
            return View();
        }
    }
}