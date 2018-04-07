using MVCWEF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace MVCWEF.Controllers
{
    public class ItemsForVisitorController : Controller
    {
        // GET: ItemsForVisitor
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult ViewAllDrinks(int? page)
        {
            var pagenumber = page ?? 1;
            var pagesize = 1000;
            using (MvcCrudDBEntities1 db = new MvcCrudDBEntities1())
            {

                var orderlist = db.Products.ToList<Product>().Where(product => product.TypeID == 3).ToPagedList(pagenumber, pagesize);

                return View(orderlist);
            }
        }
        [HttpGet]
        public ActionResult ViewAllFood(int? page)
        {
            var pagenumber = page ?? 1;
            var pagesize = 1000;
            using (MvcCrudDBEntities1 db = new MvcCrudDBEntities1())
            {

                var orderlist = db.Products.ToList<Product>().Where(product => product.TypeID == 4).ToPagedList(pagenumber, pagesize);

                return View(orderlist);
            }
        }
    }
}