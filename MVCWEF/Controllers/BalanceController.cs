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

            MvcCrudDBEntities1 db = new MvcCrudDBEntities1();
            
                var order =  db.Orders.ToList().Where(o => o.StatusID == 6).ToPagedList(pagenumber, pagesize);
            

            return View(order);
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
        public ActionResult Details()
        {
            Order order = new Order();
            int id = int.Parse(this.RouteData.Values["id"].ToString()); //orderid
           MvcCrudDBEntities1 db = new MvcCrudDBEntities1() ;
            order = (Order)db.Orders.Find(id);               
                return View(order);
            
        }

        public ActionResult CheckBalance(int? page)
        {
            var pagenumber = page ?? 1;
            var pagesize = 3;
            int id = int.Parse(this.RouteData.Values["id"].ToString()); //orderid
            
            var order = new Order();
            List<double> Price = new List<double>();
            double balance = 0.0;
            double newbalance = 0.0;
            double TotalPrice = 0.0;
            var user = new User();
           
            MvcCrudDBEntities1 db = new MvcCrudDBEntities1();
            
              
              order = (Order)db.Orders.Find(id);

            IEnumerable<OrderDetail> orderdetails = db.OrderDetails.ToList().Where(orderdetail => orderdetail.OrderId == id);
            foreach (var item in orderdetails)
            {
                Price.Add(item.TotalPrice.Value);
            }
            TotalPrice = Price.Sum(x => Convert.ToDouble(x));
            user = (User)db.Users.Find(order.UserID);
            balance = user.Balance.Value;

            if (balance >= TotalPrice)
            {
                newbalance = balance - TotalPrice;
                user.Balance = newbalance;
                db.Users.Attach(user);
                db.Entry(user).Property(x => x.Balance).IsModified = true;
                db.SaveChanges();
                order.StatusID = 5;
                db.Orders.Attach(order);
                db.Entry(order).Property(x => x.StatusID).IsModified = true;
                db.SaveChanges();

                return View("SubmitSuccessfully");
            }
            else
            {

                var viewModel =
                     from pd in db.Orders
                     join p in db.OrderDetails on pd.OrderId equals p.OrderId
                     join Pr in db.Products on p.ProductId equals Pr.ProuductID
                     join U in db.Users on pd.UserID equals U.UserID
                     where pd.StatusID == 6
                     select new MyViewModel { order = pd, orderdetails = p, products = Pr, users = U };

                return View("IndexWaitingOrder", viewModel.OrderByDescending(orde => orde.order.OrderId).ToPagedList(pagenumber, pagesize));

            }
        }
    }
}