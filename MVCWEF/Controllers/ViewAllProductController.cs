using MVCWEF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace MVCWEF.Controllers
{
    public class ViewAllProductController : Controller
    {
        // GET: ViewAllProduct
        public ActionResult Index()
        {
            return View();
        }

      

        public ActionResult ViewAll(int? page)
        {
            var pagenumber = page ?? 1;
            var pagesize = 3;
            using (MvcCrudDBEntities1 db = new MvcCrudDBEntities1())
            {
              
                var orderlist = db.Products.ToList<Product>().Where(product => product.TypeID == 4).ToPagedList(pagenumber, pagesize);

                return View(orderlist);
            }
        }


        public ActionResult ViewAllForVisitor()
        {
            return View(GetAllProduct());
        }

        IEnumerable<Product> GetAllProduct()
        {
            using (MvcCrudDBEntities1 db = new MvcCrudDBEntities1())
            {
                return db.Products.ToList<Product>().Where(product=> product.TypeID == 4);
            }

        }
    }
}