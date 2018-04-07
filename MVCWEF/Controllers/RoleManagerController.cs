using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using MVCWEF.Models;
using PagedList;

namespace MVCWEF.Controllers
{
    public class RoleManagerController : Controller
    {
        // GET: RoleManager
        public ActionResult Index()
        {
            return View(); 
        }





        /* public ActionResult ViewAll(int? page)
         {
             var pagenumber = page ?? 1;
             var pagesize = 3;
             using (MvcCrudDBEntities1 db = new MvcCrudDBEntities1())
             {

                 var orderlist = db.Users.ToList<User>().Where(user => user.TypeID == 2).ToPagedList(pagenumber, pagesize);

                 return View(orderlist);
             }
         }*/

        public ActionResult ViewAll()
        {
          
                return View(GetAllEmployee());
            
        }

        IEnumerable<User> GetAllEmployee()
        {
            using (MvcCrudDBEntities1 db = new MvcCrudDBEntities1())
            {
                return db.Users.ToList<User>().Where(user => user.TypeID == 2 && user.StatusID == 2);
            }

        }

       

        public ActionResult AddOrEdit(int id = 0)
        {
            User emp = new User();
            if (id != 0)
            {
                using (MvcCrudDBEntities1 db = new MvcCrudDBEntities1())
                {
                    emp = db.Users.Where(x => x.UserID == id).FirstOrDefault<User>();
                }
            }
            return View(emp);
        }

        [HttpPost]
        public ActionResult AddOrEdit(User emp)
        {
            try
            {
                if (emp.ImageUpload != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(emp.ImageUpload.FileName);
                    string extension = Path.GetExtension(emp.ImageUpload.FileName);
                    fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    emp.Image = "~/AppFiles/Images/" + fileName;
                    emp.ImageUpload.SaveAs(Path.Combine(Server.MapPath("~/AppFiles/Images/"), fileName));
                }

                using (MvcCrudDBEntities1 db = new MvcCrudDBEntities1())
                {
                    if (emp.UserID == 0)
                    {
                        emp.TypeID = 2;
                        emp.StatusID = 2;
                        db.Users.Add(emp);
                        db.SaveChanges();
                    }
                    else
                    {
                        emp.TypeID = 2;
                        emp.StatusID = 2;
                        db.Entry(emp).State = EntityState.Modified;
                        db.SaveChanges();

                    }
                }
                return Json(new { success = true, html = GlobalClass.RenderRazorViewToString(this, "ViewAll", GetAllEmployee()), message = "Submitted Successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Delete(int id)
        {
            try
            {
                using (MvcCrudDBEntities1 db = new MvcCrudDBEntities1())
                {
                    User emp = db.Users.Where(x => x.UserID == id).FirstOrDefault<User>();
                    db.Users.Remove(emp);
                    db.SaveChanges();
                }
                return Json(new { success = true, html = GlobalClass.RenderRazorViewToString(this, "ViewAll", GetAllEmployee()), message = "Deleted Successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Block(int id = 0)
        {
            User emp = new User();
            if (id != 0)
            {
                using (MvcCrudDBEntities1 db = new MvcCrudDBEntities1())
                {
                    emp = db.Users.Where(x => x.UserID == id).FirstOrDefault<User>();
                }
            }
            return View(emp);
        }

        [HttpPost]
        public ActionResult Block(User emp)
        {
            try
            {
                if (emp.ImageUpload != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(emp.ImageUpload.FileName);
                    string extension = Path.GetExtension(emp.ImageUpload.FileName);
                    fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    emp.Image = "~/AppFiles/Images/" + fileName;
                    emp.ImageUpload.SaveAs(Path.Combine(Server.MapPath("~/AppFiles/Images/"), fileName));
                }

                using (MvcCrudDBEntities1 db = new MvcCrudDBEntities1())
                {
                    if (emp.UserID == 0)
                    {
                        emp.TypeID = 2;
                        emp.StatusID = 1;
                        db.Users.Add(emp);
                        db.SaveChanges();
                    }
                    else
                    {
                        emp.TypeID = 2;
                        emp.StatusID = 1;
                        db.Entry(emp).State = EntityState.Modified;
                        db.SaveChanges();

                    }
                }
                return Json(new { success = true, html = GlobalClass.RenderRazorViewToString(this, "ViewAll", GetAllEmployee()), message = "Submitted Successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }








        [HttpPost]
        public ActionResult UpdateProfile(User user)
        {
            try
            {
                using (MvcCrudDBEntities1 db = new MvcCrudDBEntities1())
                {

                    User usersession = new User();
                    usersession = (User)@Session["User"];



                    var emp = db.Users.Where(x => x.UserID == usersession.UserID).FirstOrDefault<User>();
                    if (emp != null)
                    {
                        emp.Fname = user.Fname;
                        emp.Lname = user.Lname;
                        emp.Email = user.Email;
                        emp.Address = user.Address;
                        emp.Phone = user.Phone;

                        usersession.Password = user.Password;
                        usersession.Password = user.ConfirmPassword;
                        usersession.Balance = user.Balance;
                        usersession.StatusID = user.StatusID;
                        usersession.TypeID = user.TypeID;
                        usersession.Image = user.Image;

                        db.SaveChanges();
                        Session["User"] = user;
                    }
                }
                return RedirectToAction("Profile", "Main");
            }
             

            catch (DbEntityValidationException ex)
            {
                // Retrieve the error messages as a list of strings.
                var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage);

                // Join the list to a single string.
                var fullErrorMessage = string.Join("; ", errorMessages);

                // Combine the original exception message with the new one.
                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                // Throw a new DbEntityValidationException with the improved exception message.
                throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
            }

           
        }


        [HttpPost]
        public ActionResult UpdatePassword(User user)
        {
            MvcCrudDBEntities1 db = new MvcCrudDBEntities1();

            User usersession = new User();
            usersession = (User)@Session["User"];



            var emp = db.Users.Where(x => x.UserID == usersession.UserID).FirstOrDefault<User>();
            if (emp != null)
            {
                usersession.Fname = emp.Fname;
                usersession.Lname = emp.Lname;
                usersession.Email = emp.Email;
                usersession.Address = emp.Address;
                usersession.Phone = emp.Phone;
                usersession.Balance = emp.Balance;
                usersession.StatusID = emp.StatusID;
                usersession.TypeID = emp.TypeID;
                usersession.Image = emp.Image;

                emp.Password = user.Password;
               

                db.SaveChanges();

                Session["User"] = user.Password;
                Session["User"] = emp;

                Console.Write("mohamed");
            }
            Console.Write("salah");
            return RedirectToAction("Profile", "Main");

        }





        public ActionResult IndexProduct()
        {
            return View(); 
        }



        /*public ActionResult ViewAllProduct(int? page)
        {
            var pagenumber = page ?? 1;
            var pagesize = 3;
            using (MvcCrudDBEntities1 db = new MvcCrudDBEntities1())
            {

                var orderlist = db.Products.ToList<Product>().ToPagedList(pagenumber, pagesize);

                return View(orderlist);
            }
        }*/




        
        /*public ActionResult ViewAllProduct(string productid)
         {
            try
            {
                IQueryable<Product> products;
                using (MvcCrudDBEntities1 db = new MvcCrudDBEntities1())
                {
                    products = from p in db.Products select p;

                    if (!String.IsNullOrEmpty(productid))
                    {
                        products = db.Products.Where(p => p.ProuductID == Int32.Parse(productid));
                    }
                

                }
                return View(products);
            }
               
            catch (Exception e)
            {
                throw;
            }
            
        }*/

        public ActionResult ViewAllProduct()
        {
           
                return View(GetAllProduct());
        }

       /* public ActionResult ViewSpecific(int id)
        {
            using (MvcCrudDBEntities1 db = new MvcCrudDBEntities1())
            {
                var products = db.Products.Where(p => p.ProuductID == id);

                return View(products);
            }
            
        }*/

        IEnumerable<Product> GetAllProduct()
        {
            using (MvcCrudDBEntities1 db = new MvcCrudDBEntities1())
            {
                return db.Products.ToList<Product>();
            }

        }


        public ActionResult AddOrEditProduct(int id = 0)
        {
            Product product = new Product();
            if (id != 0)
            {
                using (MvcCrudDBEntities1 db = new MvcCrudDBEntities1())
                {
                    product = db.Products.Where(x => x.ProuductID == id).FirstOrDefault<Product>();
                }
            }
            return View(product);
        }

        [HttpPost]
        public ActionResult AddOrEditProduct(Product product )
        {
            try
            {
                if (product.ImageUpload != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(product.ImageUpload.FileName);
                    string extension = Path.GetExtension(product.ImageUpload.FileName);
                    fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    // product.Image = Encoding.ASCII.GetBytes("~/AppFiles/Images/" + fileName);
                    product.Image = "~/AppFiles/Images/" + fileName;
                    product.ImageUpload.SaveAs(Path.Combine(Server.MapPath("~/AppFiles/Images/"), fileName));
                }
                using (MvcCrudDBEntities1 db = new MvcCrudDBEntities1())
                {
                    if (product.ProuductID == 0)
                    {
                        
                        db.Products.Add(product);
                        db.SaveChanges();
                    }
                    else
                    {
                        db.Entry(product).State = EntityState.Modified;
                        db.SaveChanges();

                    }
                }
                return Json(new { success = true, html = GlobalClass.RenderRazorViewToString(this, "ViewAllProduct", GetAllProduct()), message = "Submitted Successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult DeleteProduct(int id)
        {
            try
            {
                using (MvcCrudDBEntities1 db = new MvcCrudDBEntities1())
                {
                    Product product = db.Products.Where(x => x.ProuductID == id).FirstOrDefault<Product>();
                    db.Products.Remove(product);
                    db.SaveChanges();
                }
                return Json(new { success = true, html = GlobalClass.RenderRazorViewToString(this, "ViewAllProduct", GetAllProduct()), message = "Deleted Successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}