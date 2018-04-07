using MVCWEF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace MVCWEF.Controllers
{
    public class BalanceInfoController : Controller
    {

        // GET: BalanceInfo
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IndexWaitingOrder()
        {
            return View();
        }

        public ActionResult ViewAllOrderWaiting(int? page)
        {
            var pagenumber = page ?? 1 ;
            var pagesize = 5;
            using (MvcCrudDBEntities1 db = new MvcCrudDBEntities1())
            {
                var orderlist = db.Orders.Where(order => order.StatusID == 6).OrderByDescending(order => order.OrderId).ToPagedList(pagenumber, pagesize);

            return View(orderlist);
            }
        }


        public Nullable<double> GetAllEmployee(int id)
        {
            User emp = new User();
            using (MvcCrudDBEntities1 db = new MvcCrudDBEntities1())
                {
                    emp = db.Users.Where(x => x.UserID == id).FirstOrDefault<User>();
                }
            return emp.Balance;

        }


        IEnumerable<Order> GetAllOrder()
        {
            using (MvcCrudDBEntities1 db = new MvcCrudDBEntities1())
            {
                return db.Orders.ToList().Where(order => order.StatusID == 6);
            }

        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            using (MvcCrudDBEntities1 db = new MvcCrudDBEntities1())
            {
                var orderdetails = db.OrderDetails.Find(id);
                if (orderdetails == null)
                {
                    return HttpNotFound();
                }
               

                return View(orderdetails);
            }
        }
    }
}