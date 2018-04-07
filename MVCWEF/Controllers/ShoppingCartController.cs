using MVCWEF.Models;
using System;
using PayPal.Api;

using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCWEF.Controllers
{
    public class ShoppingCartController : Controller
    {
        private string strCart = "Cart";
        MvcCrudDBEntities1 db = new MvcCrudDBEntities1();
        // GET: ShoppingCart
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult OrderNow(int? id)
        {
            if(id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            if(Session[strCart] == null)
            {
                List<Cart> lsCart = new List<Cart>
                {
                    new Cart(db.Products.Find(id) ,1)
                };
                Session[strCart] = lsCart;
            }
            else
            {

                List<Cart> lsCart = (List<Cart>)Session[strCart];
                int check = isExistingCheck(id);
                if (check == -1)
                    lsCart.Add(new Cart(db.Products.Find(id), 1));
                else
                    lsCart[check].Quantity++;
                Session[strCart] = lsCart;
            }
            return View("Index");
        }


        private int isExistingCheck(int? id)
        {
            List<Cart> lsCart = (List < Cart >) Session[strCart];
            for(int i=0; i<lsCart.Count; i++)
            {
                if (lsCart[i].Product.ProuductID == id) return i;

            }
            return -1;
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            int check = isExistingCheck(id);
            List<Cart> lsCart = (List<Cart>)Session[strCart];
            lsCart.RemoveAt(check);

            return View("Index");
        }

        public ActionResult UpdateCart(FormCollection frc)
        {
            string[] quantities = frc.GetValues("quantity");
            List<Cart> lsCart = (List<Cart>)Session[strCart];
            for(int i=0; i<lsCart.Count; i++)
            {
                lsCart[i].Quantity = Convert.ToInt32(quantities[i]);
            }
            Session[strCart] = lsCart;
            return View("Index");
        }

        public ActionResult CheckOut(FormCollection frc)
        {
           
            return View("CheckOut");
        }

        public ActionResult ProcessOrder(MVCWEF.Models.Order order)
        {
            User user = new User();
            user = (User)@Session["User"];
            List<Cart> lsCart = (List<Cart>)Session[strCart];
            using (MvcCrudDBEntities1 db = new MvcCrudDBEntities1())
            {
                    order.UserID = user.UserID; 
                    order.OrderDate = DateTime.Now;
                    order.PaymentType = "Cash";
                    order.StatusID = 6;
                    db.Orders.Add(order);
                    db.SaveChanges();
               
            }
            foreach (Cart cart in lsCart)
            {
                OrderDetail orderdetail = new OrderDetail()
                {
                     
                    OrderId = order.OrderId,
                    ProductId = cart.Product.ProuductID,
                    Quantity = cart.Quantity,
                    TotalPrice = (double)cart.Product.Price * (double)cart.Quantity
                };
                db.OrderDetails.Add(orderdetail);
                db.SaveChanges();
            }
            Session.Remove(strCart);
            return View("OrderSuccess");
        }


        private Payment payment;


        private Payment CreatePayment(APIContext apiContext , string redirectUrl)
        {
            var listItems = new ItemList() { items = new List<Item>()};
            List<Cart> listCarts = (List<Cart>)Session[strCart];
            foreach(var cart in listCarts)
            {
                listItems.items.Add(new Item()
                {
                    name = cart.Product.ProductName,
                    currency = "USD",
                    price = cart.Product.Price.ToString(),
                    quantity = cart.Quantity.ToString(),
                    sku = "sku"
                });
            }

            var payer = new Payer() { payment_method = "paypal" };

            var redirUrls = new RedirectUrls()
            {
                cancel_url = redirectUrl,
                return_url = redirectUrl
            };

            var details = new Details()
            {
                tax = "1",
                shipping = "2",
                subtotal = listCarts.Sum(x => x.Quantity * x.Product.Price).ToString()
            };

            var amount = new Amount()
            {
                currency = "USD",
                total = (Convert.ToDouble(details.tax)+Convert.ToDouble(details.shipping)+Convert.ToDouble(details.subtotal)).ToString(),
                details = details
            };

            var transactionList = new List<Transaction>();
            transactionList.Add(new Transaction() {

                description = "mohamed testing transaction description",
                invoice_number = Convert.ToString((new Random()).Next(100000)),
                amount = amount,
                item_list = listItems
            });

            payment = new Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = transactionList,
                redirect_urls = redirUrls
            };

            return payment.Create(apiContext);
        }

        private Payment ExecutePayment (APIContext apiContext , string payerId , string paymentId)
        {
            var paymentExecution = new PaymentExecution()
            {
                payer_id = payerId
            };
            payment = new Payment() { id = paymentId };
            return payment.Execute(apiContext, paymentExecution);
        }


        public ActionResult PaymentWithPaypal()
        {
            

            APIContext apiContext = PaypalConfiguration.GetAPIContext();

            try
            {

                MVCWEF.Models.Order order = new MVCWEF.Models.Order();
                User user = new User();
                user = (User)@Session["User"];
                List<Cart> lsCart = (List<Cart>)Session[strCart];
                using (MvcCrudDBEntities1 db = new MvcCrudDBEntities1())
                {
                    order.UserID = user.UserID;
                    order.OrderDate = DateTime.Now;
                    order.PaymentType = "Paypal";
                    order.StatusID = 6;
                    db.Orders.Add(order);
                    db.SaveChanges();

                }
                foreach (Cart cart in lsCart)
                {
                    OrderDetail orderdetail = new OrderDetail()
                    {

                        OrderId = order.OrderId,
                        ProductId = cart.Product.ProuductID,
                        Quantity = cart.Quantity,
                        TotalPrice = (double)cart.Product.Price * (double)cart.Quantity
                    };
                    db.OrderDetails.Add(orderdetail);
                    db.SaveChanges();
                }
                Session.Remove(strCart);


                string payerId = Request.Params["PayerID"];
                if(string.IsNullOrEmpty(payerId))
                {
                    //Creating Payment
                    string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority + "ShoppingCart/PaymentWithPaypal?";
                    var guid = Convert.ToString((new Random()).Next(100000));
                    var createdPayment = CreatePayment(apiContext, baseURI + "guid=" + guid);

                    //Get Links returned from paypal response to create call function
                    var links = createdPayment.links.GetEnumerator();
                    string paypalRedirectUrl = string.Empty;

                    while(links.MoveNext())
                    {
                        Links link = links.Current;
                        if(link.rel.ToLower().Trim().Equals("approval_url"))
                        {
                            paypalRedirectUrl = link.href;
                        }
                    }
                    Session.Add(guid, createdPayment.id);
                    return Redirect(paypalRedirectUrl);
                }
                else
                {
                    var guid = Request.Params["guid"];
                    var executedPayment = ExecutePayment(apiContext, payerId, Session[guid] as string);

                    if(executedPayment.state.ToLower()!="approval")
                    {
                        return View("Failure");
                    }
                }

            }catch(Exception ex)
            {
                PaypalLogger.Log("Error :" + ex.Message);
                return View("Failure");
            }

            return View("Success");
        }
    }
}