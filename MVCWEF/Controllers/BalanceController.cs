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
    public class BalanceController : Controller
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
            var pagenumber = page ?? 1;
            var pagesize = 3;
            using (MvcCrudDBEntities1 db = new MvcCrudDBEntities1())
            {

                var viewModel =
                    from pd in db.Orders
                    join p  in db.OrderDetails on pd.OrderId equals p.OrderId 
                    join Pr in db.Products on p.ProductId equals Pr.ProuductID
                    join U  in db.Users on pd.UserID equals U.UserID
                    where pd.StatusID == 6
                    select new MyViewModel { order = pd, orderdetails = p , products = Pr , users = U};



                return View(viewModel.OrderByDescending(order => order.order.OrderId).ToPagedList(pagenumber, pagesize));
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