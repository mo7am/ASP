using MVCWEF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Net;

namespace MVCWEF.Controllers
{
    public class UserRoleController : Controller
    {
        // GET: UserRole
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ViewOldOrders(int? page)
        {
            var pagenumber = page ?? 1;
            var pagesize = 3;
            using (MvcCrudDBEntities1 db = new MvcCrudDBEntities1())
            {
                User user = new User();
                user = (User)@Session["User"];
            

                var viewModel =
                     from pd in db.Orders
                     join p in db.OrderDetails on pd.OrderId equals p.OrderId
                     where pd.UserID == user.UserID
                     select  new MyViewModel { order = pd, orderdetails = p };


                return View(viewModel.OrderByDescending(order => order.order.OrderId).ToPagedList(pagenumber, pagesize));
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