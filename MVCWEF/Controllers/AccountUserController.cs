using MVCWEF.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace MVCWEF.Controllers
{
    public class AccountUserController : Controller
    {

       private MvcCrudDBEntities1 mde = new MvcCrudDBEntities1();

        //
        // GET: /AccountUser/
        public ActionResult Index()
        {
            return View();
        }

        //[Route("Main/Index")]
        [HttpPost]
        public ActionResult Login(User user)
        {
            using (mde = new MvcCrudDBEntities1())
            {
                var dataItem = mde.Users.Where(x => x.Email.Equals(user.Email) && x.Password.Equals(user.Password)).FirstOrDefault();
                if (dataItem != null)
                {
                    FormsAuthentication.SetAuthCookie(dataItem.Email, false);
                    if (dataItem.TypeID == 1)
                    {
                        Session["User"] = dataItem;
                        return RedirectToAction("IndexManager", "Main");
                    }
                    else if(dataItem.TypeID == 2)
                    {
                        Session["User"] = dataItem;
                        return RedirectToAction("IndexAdmin", "Main");
                    }
                   
                    else
                    {
                        Session["User"] = dataItem;
                        return RedirectToAction("Index", "Main");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Invalid Username OR Password");
                    return View("Index" , dataItem);
                }
            }
        }


        public ActionResult LogOut()
        {
            Session.Clear();
            Session.Abandon();
            Session.RemoveAll();

            FormsAuthentication.SignOut();


            this.Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
            this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            this.Response.Cache.SetNoStore();

            return RedirectToAction("Index", "AccountUser");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegisterForm(User user)
        {
            try
            {
                if (user.ImageUpload != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(user.ImageUpload.FileName);
                    string extension = Path.GetExtension(user.ImageUpload.FileName);
                    fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    // product.Image = Encoding.ASCII.GetBytes("~/AppFiles/Images/" + fileName);
                    user.Image = "~/AppFiles/Images/" + fileName;
                    user.ImageUpload.SaveAs(Path.Combine(Server.MapPath("~/AppFiles/Images/"), fileName));
                }

                using (MvcCrudDBEntities1 dc = new MvcCrudDBEntities1())
                {
                    double balance = 0.0;
                    //you should check duplicate registration here 
                    var dataItem = mde.Users.Where(x => x.Email.Equals(user.Email)).FirstOrDefault();
                    if (dataItem == null)
                    {
                        user.TypeID = 2;
                        user.StatusID = 2;
                        dc.Users.Add(user);
                        dc.SaveChanges();
                        Session["User"] = user;

                        if (ModelState.IsValid)
                        {
                            var senderEmail = new MailAddress("online.restruant@gmail.com", "Restruant Owner");
                            var reciverEmail = new MailAddress(user.Email, user.Fname+ " " + user.Lname);

                            var password = "owner.restruant123";
                            var sub = "Online Restruant";
                            var body = "Welcome to you in our Restruant you can select any item online with cash or paypal" +
                                " if there is any problem please contact us in this email :" +
                                " online.restruant@gmail.com" +
                                " Thank You";


                            var smtp = new SmtpClient
                            {
                                Host = "smtp.gmail.com",
                                Port = 587,
                                EnableSsl = true,
                                DeliveryMethod = SmtpDeliveryMethod.Network,
                                UseDefaultCredentials = false,
                                Credentials = new NetworkCredential(senderEmail.Address, password)
                            };

                            using (var message = new MailMessage(senderEmail, reciverEmail)
                            {
                                Subject = sub,
                                Body = body
                            })
                            {
                                smtp.Send(message);
                            }

                        }

                        return RedirectToAction("IndexAdmin", "Main");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Email Is Already Exist");
                    }
                }

            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        System.Console.WriteLine("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
            }
            ModelState.AddModelError("", "Invalid Registration");
            return View("Index", user);
        }

    }
}



/*Name	^[a-zA-Z''-'\s]{1,40}$	John Doe
O'Dell	Validates a name. Allows up to 40 uppercase and lowercase characters and a few special characters that are common to some names. You can modify this list.
Social Security Number	^\d{3}-\d{2}-\d{4}$	111-11-1111	Validates the format, type, and length of the supplied input field.The input must consist of 3 numeric characters followed by a dash, then 2 numeric characters followed by a dash, and then 4 numeric characters.
Phone Number    ^[01]?[- .]?(\([2 - 9]\d{ 2}\)|[2-9]\d{2})[- .]?\d{3}[- .]?\d{4}$	(425) 555-0123
425-555-0123
425 555 0123
1-425-555-0123	Validates a U.S.phone number.It must consist of 3 numeric characters, optionally enclosed in parentheses, followed by a set of 3 numeric characters and then a set of 4 numeric characters.
E-mail  ^(?("")("".+?""@)|(([0 - 9a - zA - Z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-zA-Z])@))(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,6}))$	someone@example.com	Validates an e-mail address.
URL	^(ht|f)tp(s ?)\:\/\/[0-9a-zA-Z] ([-.\w]*[0 - 9a - zA - Z])* (:(0-9)*)* (\/?)([a - zA - Z0 - 9\-\.\?\,\'\/\\\+&amp;%\$#_]*)?$	http://www.microsoft.com	Validates a URL
          ZIP Code ^ (\d{5}-\d{4}|\d{5}|\d{9})$|^([a - zA - Z]\d[a - zA - Z] \d[a - zA - Z]\d)$	12345	Validates a U.S.ZIP Code.The code must consist of 5 or 9 numeric characters.
Password    (?!^[0-9]*$)(?!^[a-zA-Z]*$)^([a - zA - Z0 - 9]{8,10})$	 	Validates a strong password.It must be between 8 and 10 characters, contain at least one digit and one alphabetic character, and must not contain special characters.
Non- negative integer	^\d+$	0
986	Validates that the field contains an integer greater than zero.
Currency (non- negative)    ^\d+(\.\d\d)?$	1.00	Validates a positive currency amount.If there is a decimal point, it requires 2 numeric characters after the decimal point.For example, 3.00 is valid but 3.1 is not.
    Currency (positive or negative) ^(-)?\d+(\.\d\d)?$	1.20	Validates for a positive or negative currency amount.If there is a decimal point, it requires 2 numeric characters after the decimal point.
    */