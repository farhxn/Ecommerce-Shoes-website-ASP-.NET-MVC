using shoes.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Mail;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace shoes.Controllers
{
    [AllowAnonymous]
    [OutputCache(NoStore = true, Duration = 0)]
    public class HomeController : Controller
    {
        shoesEntities db = new shoesEntities();
        public ActionResult Index()
        {
            ViewBag.newProducts = db.Products.Where(x => x.P_cat == "New Product").ToList().Take(8); 
            ViewBag.hotSelling = db.Products.Where(x => x.P_cat == "Hot Selling").ToList().Take(8); 
            ViewBag.saleProduct = db.Products.Where(x => x.P_cat == "Sale").ToList().Take(8); 
            ViewBag.DealProduct = db.Products.Where(x => x.P_cat == "Bless Friday").ToList().Take(9);
          
            if(Session["User_ID"] != null && Session["User_ID"].ToString() != "")
            {
                string uid = Session["User_ID"].ToString();
                var ordersForUser = db.Orders.Where(x => x.U_id == uid).ToList();
 Session["num"] = ordersForUser.Count();

            }

            return View(db.Sliders.ToList());
        }

        public ActionResult Shop()
        {
            ViewBag.DealProduct = db.Products.Where(x => x.P_cat == "Bless Friday").ToList().Take(9);
            ViewBag.categories = db.Categories.ToList();
            ViewBag.newProductsCount = db.Products.Where(x => x.P_cat == "New Product").Count();
            ViewBag.hotSellingCount = db.Products.Where(x => x.P_cat == "Hot Selling").Count();
            ViewBag.saleProductCount = db.Products.Where(x => x.P_cat == "Sale").Count();
            ViewBag.DealProductCount = db.Products.Where(x => x.P_cat == "Bless Friday").Count();

                        if(Session["User_ID"] != null && Session["User_ID"].ToString() != "")
            {
                string uid = Session["User_ID"].ToString();
                var ordersForUser = db.Orders.Where(x => x.U_id == uid).ToList();
                Session["num"] = ordersForUser.Count();

            }
            return View(db.Products.ToList());
        }
        public ActionResult ShopDetail(int id)
        {
            ViewBag.DealProduct = db.Products.Where(x => x.P_cat == "Bless Friday").ToList().Take(9);
            var detail = db.Products.Where(x => x.P_id == id).FirstOrDefault();
            

                        if(Session["User_ID"] != null && Session["User_ID"].ToString() != "")
            {
                string uid = Session["User_ID"].ToString();
                var ordersForUser = db.Orders.Where(x => x.U_id == uid).ToList();

            }
            var reviewsForProduct = db.Reviews.Where(x => x.R_PID == id.ToString()).ToList();

            int totalRatings = reviewsForProduct.Count;
            int sumOfRatings = reviewsForProduct.Sum(r => r.R_id);
            double averageRating = totalRatings > 0 ? (double)sumOfRatings / totalRatings : 0;

            string formattedAverageRating = averageRating.ToString("0.00");
            ViewBag.AverageRating = formattedAverageRating;

            ViewBag.ReviewCount = totalRatings;
            ViewBag.ReviewList = reviewsForProduct;


                            if(Session["User_ID"] != null && Session["User_ID"].ToString() != "")
            {
                string uid = Session["User_ID"].ToString();
                var checkOrder = db.Billings.Where(x => x.U_id == uid && x.P_id == id.ToString() && x.isReview == null).ToList();
            if (checkOrder.Any())
            {
                TempData["AllowReivew"] = "Success";
            }
            else
            {
                TempData["AllowReivew"] = "Failed";
            }

            }
            else
            {
                TempData["AllowReivew"] = "Failed";
            }

            return View(detail);
        }

        [HttpPost]
        public ActionResult ShopDetail(string message,string ProId)
        {
            try
            {
               string na =  Session["User_Name"].ToString();
               string ma =  Session["User_Mail"].ToString();
                var rev = new Review {
                    R_name = na,
                    R_mail = ma,
                    R_PID = ProId,
                    R_review = message,
                    R_userId = Session["User_ID"].ToString(),
                    createdAt = DateTime.Now.ToString(),
                    updatedAt = DateTime.Now.ToString(),
                };

                db.Reviews.Add(rev);
                string uid = Session["User_ID"].ToString();
                var ordersForUser = db.Billings.Where(x => x.U_id == uid && x.P_id == ProId && x.isReview == null).ToList();
                foreach(var bi in ordersForUser)
                {
                    bi.isReview = "done";
                    bi.updatedAt = DateTime.Now.ToString();
                    db.Entry(bi).State = EntityState.Modified;
                }

                db.SaveChanges();

                return RedirectToAction("ShopDetail/"+ProId);
            }
            catch (Exception ex)
            {

            return RedirectToAction("index");
            }
            return RedirectToAction("index");
        }


        public ActionResult CategoryProducts(string CatName)
        {
            ViewBag.DealProduct = db.Products.Where(x => x.P_cat == "Bless Friday").ToList().Take(9);
            ViewBag.categories = db.Categories.ToList();
            ViewBag.newProductsCount = db.Products.Where(x => x.P_cat == "New Product").Count();
            ViewBag.hotSellingCount = db.Products.Where(x => x.P_cat == "Hot Selling").Count();
            ViewBag.saleProductCount = db.Products.Where(x => x.P_cat == "Sale").Count();
            ViewBag.DealProductCount = db.Products.Where(x => x.P_cat == "Bless Friday").Count();
            var detail = db.Products.Where(x => x.P_cat == CatName).ToList();
                        if(Session["User_ID"] != null && Session["User_ID"].ToString() != "")
            {

                string uid = Session["User_ID"].ToString();
                var ordersForUser = db.Orders.Where(x => x.U_id == uid).ToList();
                Session["num"] = ordersForUser.Count();

            }
            return View(detail);
        }


        public ActionResult Cart()
        {
            string uid = Session["User_ID"].ToString();
            var ordersForUser = db.Orders.Where(x => x.U_id == uid).ToList();
            List<collect> cartItems = new List<collect>();

            foreach (var item in ordersForUser)
            {
                cartItems.Add(new collect
                {
                    P_ID = int.Parse(item.P_id),
                    P_Qty = int.Parse(item.P_qty)
                });
            }
            Session["cart"] = cartItems;
            if (Session["cart"] != null)
            {
                List<collect> mainlist = (List<collect>)Session["cart"];
                List<Cart> viewlist = new List<Cart>();
                foreach (var item in mainlist)
                {
                    Cart obj = new Cart();
                    Product prop = db.Products.Where(x => x.P_id == item.P_ID).FirstOrDefault();
                    obj.P_ID = prop.P_id;
                    obj.P_Name = prop.P_name;
                    obj.qty = item.P_Qty;
                    obj.stock = Convert.ToInt32(prop.P_stock);
                    obj.P_Pic = prop.P_img1;
                    obj.P_Price = Convert.ToInt32((prop.P_dprice ?? prop.P_price).ToString() ?? "0");
                    obj.P_Totalprice = item.P_Qty * Convert.ToInt32((prop.P_dprice ?? prop.P_price).ToString() ?? "0");
                    viewlist.Add(obj);
                }

                Session["num"] = mainlist.Count();
              

                return View(viewlist);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public JsonResult addtocart(int id, int qty)
        {
            string uid = Session["User_ID"].ToString();
            var ordersForUser = db.Orders.Where(x => x.U_id == uid).ToList();

            List<collect> cartItems = new List<collect>();

            foreach (var item in ordersForUser)
            {
                cartItems.Add(new collect
                {
                    P_ID = int.Parse(item.P_id),
                    P_Qty = int.Parse(item.P_qty)
                });
            }
            int totalOrder = 0;

            Session["cart"] = cartItems;
            Session["num"] = ordersForUser.Count();

            var existingCartItem = cartItems.FirstOrDefault(item => item.P_ID == id);

            if (existingCartItem != null)
            {
                existingCartItem.P_Qty += qty;
                string uuid = Session["User_ID"].ToString();
                var existingOrder = db.Orders.FirstOrDefault(x => x.P_id == id.ToString() && x.U_id == uuid);
                if (existingOrder != null)
                {
                    existingOrder.P_qty = existingCartItem.P_Qty.ToString();
                    existingOrder.updatedAt = DateTime.Now.ToString();
                    db.Entry(existingOrder).State = EntityState.Modified;
            
                }
                else
                {
                    var newOrder = new Order
                    {
                        P_id = id.ToString(),
                        P_qty = existingCartItem.P_Qty.ToString(),
                        U_id = Session["User_ID"].ToString(),
                        createdAt = DateTime.Now.ToString(),
                        updatedAt = DateTime.Now.ToString(),
                    };

                    db.Orders.Add(newOrder);
                    if (Session["num"] != null && int.TryParse(Session["num"].ToString(), out int currentNum))
                    {
                        totalOrder = currentNum;
                    }
                    int newValue = totalOrder + 1;
                    Session["num"] = newValue;
                }

                db.SaveChanges();
            }
            else
            {
                collect newCartItem = new collect { P_ID = id, P_Qty = qty };
                cartItems.Add(newCartItem);

                var newOrder = new Order
                {
                    P_id = id.ToString(),
                    P_qty = qty.ToString(),
                    U_id = Session["User_ID"].ToString(),
                    createdAt = DateTime.Now.ToString(),
                    updatedAt = DateTime.Now.ToString(),
                };

                db.Orders.Add(newOrder);
                db.SaveChanges();

                if (Session["num"] != null && int.TryParse(Session["num"].ToString(), out int currentNum))
                {
                    totalOrder = currentNum;
                }
                int newValue = totalOrder + 1;
                Session["num"] = newValue;
            }
            Session["cart"] = cartItems;
            return Json(Session["cart"], JsonRequestBehavior.AllowGet);
        }


        public ActionResult cartincrese(int id)
        {
            string uid = Session["User_ID"].ToString();
            var ordersForUser = db.Orders.Where(x => x.U_id == uid && x.P_id == id.ToString()).FirstOrDefault();
            ordersForUser.P_qty = (Convert.ToInt32(ordersForUser.P_qty) + 1).ToString();
            ordersForUser.updatedAt = DateTime.Now.ToString();
            db.Entry(ordersForUser).State = EntityState.Modified;
            db.SaveChanges();

            List<collect> mainlist = (List<collect>)Session["cart"];
            int qty = 0;
            for (int i = 0; i < mainlist.Count; i++)
            {
                if (mainlist[i].P_ID == id)
                {
                    mainlist[i].P_Qty = mainlist[i].P_Qty + 1;
                    qty = mainlist[i].P_Qty;
                    break;
                }
            }
            Session["cart"] = (List<collect>)mainlist;
            return Json(qty, JsonRequestBehavior.AllowGet);
        }


        public ActionResult cartdcs(int id)
        {
            string uid = Session["User_ID"].ToString();
            var ordersForUser = db.Orders.Where(x => x.U_id == uid && x.P_id == id.ToString()).FirstOrDefault();
            ordersForUser.P_qty = (Convert.ToInt32(ordersForUser.P_qty) - 1).ToString();
            ordersForUser.updatedAt = DateTime.Now.ToString();
            db.Entry(ordersForUser).State = EntityState.Modified;
            db.SaveChanges();

            List<collect> mainlist = (List<collect>)Session["cart"];
            int qty = 0;
            for (int i = 0; i < mainlist.Count; i++)
            {
                if (mainlist[i].P_ID == id)
                {
                    if (mainlist[i].P_Qty > 0)
                    {
                        mainlist[i].P_Qty = mainlist[i].P_Qty - 1;
                        qty = mainlist[i].P_Qty;
                        break;
                    }
                    else
                    {
                        qty = 1;
                        break;
                    }

                }
            }
            Session["cart"] = (List<collect>)mainlist;
            return Json(qty, JsonRequestBehavior.AllowGet);
        }

        public ActionResult deleteCartProduct(int id)
        {
            string uid = Session["User_ID"].ToString();
            var toDelete = db.Orders.FirstOrDefault(x => x.U_id == uid && x.P_id == id.ToString());
            if (toDelete != null)
            {
                db.Orders.Remove(toDelete);
                db.SaveChanges();
            }

            List<collect> mainlist = (List<collect>)Session["cart"];

             var itemToRemove = mainlist.FirstOrDefault(i => i.P_ID == id);
            if (itemToRemove != null)
            {
                mainlist.Remove(itemToRemove);
            }

            Session["cart"] = mainlist;
            return Json(Session["cart"], JsonRequestBehavior.AllowGet);
        }

        public ActionResult CheckOut()
        {
            string uid = Session["User_ID"].ToString();
            var ordersForUser = db.Orders.Where(x => x.U_id == uid).ToList();
            List<collect> cartItems = new List<collect>();

            foreach (var item in ordersForUser)
            {
                cartItems.Add(new collect
                {
                    P_ID = int.Parse(item.P_id),
                    P_Qty = int.Parse(item.P_qty)
                });
            }
            Session["cart"] = cartItems;
            if (Session["cart"] != null)
            {
                List<collect> mainlist = (List<collect>)Session["cart"];
                List<Cart> viewlist = new List<Cart>();
                foreach (var item in mainlist)
                {
                    Cart obj = new Cart();
                    Product prop = db.Products.Where(x => x.P_id == item.P_ID).FirstOrDefault();
                    obj.P_ID = prop.P_id;
                    obj.P_Name = prop.P_name;
                    obj.qty = item.P_Qty;
                    obj.stock = Convert.ToInt32(prop.P_stock);
                    obj.P_Pic = prop.P_img1;
                    obj.P_Price = Convert.ToInt32((prop.P_dprice ?? prop.P_price).ToString() ?? "0");
                    obj.P_Totalprice = item.P_Qty * Convert.ToInt32((prop.P_dprice ?? prop.P_price).ToString() ?? "0");
                    viewlist.Add(obj);
                }

                Session["num"] = mainlist.Count();
                var mixtabs = new MultipleTableClass
                {
                    cart = viewlist
                };

                return View(mixtabs);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public ActionResult Checkout(string num, string F_name,string L_name, string zip, string add, string city)
        {
            try
            {
                string uid = Session["User_ID"].ToString();
                string OrderID = GenerateOrderID();
                Session["OrderID"] = OrderID;
                if (ModelState.IsValid)
                {
                    var existingOrder = db.Orders.Where(x => x.U_id == uid).ToList();
                    foreach (var od in existingOrder)
                    {
                        var slide = new Billing
                        {
                            B_first = F_name,
                            B_last = L_name,
                            B_address = add,
                            B_city = city,
                            B_number = num,
                            B_zip = zip,
                            O_id = OrderID,
                            U_id = uid,
                            P_id = od.P_id,
                            P_qty = od.P_qty,
                            O_status = "Pending",
                            createdAt = DateTime.Now.ToString(),
                            updatedAt = DateTime.Now.ToString(),
                        };
                        db.Billings.Add(slide);
                        
                        var toDelete = db.Orders.FirstOrDefault(x => x.U_id == uid && x.P_id == od.P_id.ToString());
                        db.Orders.Remove(toDelete);

                        int Pid = Convert.ToInt32(od.P_id);
                        int Pqty = Convert.ToInt32(od.P_qty);
                        var ordersForUser = db.Products.Where(x => x.P_id == Pid).FirstOrDefault();
                        int Pst = Convert.ToInt32(ordersForUser.P_stock);
                        ordersForUser.P_stock = (Pst - Pqty).ToString();
                        ordersForUser.updatedAt = DateTime.Now.ToString();
                        db.Entry(ordersForUser).State = EntityState.Modified;
              
                    }

                    db.SaveChanges();
                    sendOrderMail(OrderID);
                    Session["num"] = null;
                    Session["cart"] = null;
                    return RedirectToAction("Confirmation");
                }

            }
            catch (Exception e)
            {
                return RedirectToAction("checkout");
            }
            return RedirectToAction("Confirmation");
        }

        public ActionResult Confirmation()
        {
            string OID = Session["OrderID"].ToString();
            string uid = Session["User_ID"].ToString();

            var ordersForUser = db.Billings.Where(x => x.U_id == uid   && x.O_id == OID).ToList();
            List<collect> cartItems = new List<collect>();

            foreach (var item in ordersForUser)
            {
                cartItems.Add(new collect
                {
                    P_ID = int.Parse(item.P_id),
                    P_Qty = int.Parse(item.P_qty)
                });
                ViewBag.address = item.B_address;
                ViewBag.zip = item.B_zip;
                ViewBag.city = item.B_city;
            }
            Session["cart"] = cartItems;
         
            
            if (Session["cart"] != null)
            {
                List<collect> mainlist = (List<collect>)Session["cart"];
                List<Cart> viewlist = new List<Cart>();
                foreach (var item in mainlist)
                {
                    Cart obj = new Cart();
                    Product prop = db.Products.Where(x => x.P_id == item.P_ID).FirstOrDefault();
                    obj.P_ID = prop.P_id;
                    obj.P_Name = prop.P_name;
                    obj.qty = item.P_Qty;
                    obj.stock = Convert.ToInt32(prop.P_stock);
                    obj.P_Pic = prop.P_img1;
                    obj.P_Price = Convert.ToInt32((prop.P_dprice ?? prop.P_price).ToString() ?? "0");
                    obj.P_Totalprice = item.P_Qty * Convert.ToInt32((prop.P_dprice ?? prop.P_price).ToString() ?? "0");
                    viewlist.Add(obj);
                }

                Session["num"] = mainlist.Count();
                
                return View(viewlist);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(string mail,string password)
        {
            try{
                var tomail = db.Users.FirstOrDefault(x => x.U_mail == mail);
                if(tomail != null)
                {
                    if (BCrypt.Net.BCrypt.Verify(password, tomail.U_password.ToString()))
                    {
                        Session["User_ID"] = tomail.U_id;
                        Session["User_Name"] = tomail.U_name;
                        Session["User_Mail"] = tomail.U_mail;
                     if(tomail.U_role == "0")
                        {

                        return RedirectToAction("index");
                        }
                        else
                        {
                            return RedirectToAction("Index", "Admin");
                        }
                    }
                    else
                    {
                        TempData["loginUser"] = "mailNotFound";

                        return View();
                    }
                }
                else
                {
                TempData["loginUser"] = "mailNotFound";
                    return View();
                }

            }
            catch(Exception ex)
            {
                return View();
            }
        }


        public ActionResult Forget()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Forget(string mail)
        {
            try
            {
                var tomail = db.Users.FirstOrDefault(x => x.U_mail == mail);
                if (tomail != null)
                {
                    string pass = GenerateRandomPassword();
                    string hashedPassword = BCrypt.Net.BCrypt.HashPassword(pass);
                    tomail.U_password = hashedPassword;
                    tomail.updatedAt = DateTime.Now.ToString();
                    db.Entry(tomail).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["forgetpass"] = "success";
                    SendMail("New Password For Login", "\n\n\tYour New Passowrd is : "+pass+ " \n\n\tPlease enter the above given pasword with same email",mail);
                    return RedirectToAction("login");
                }
                else
                {
                    TempData["forgetpass"] = "failed";
                    return View();
                }

            }
            catch (Exception ex)
            {
                return View();
            }
        }


        public ActionResult Register()
        {
            return View();
        }
        
        [HttpPost]
        public ActionResult Register(string name,string mail,string password)
        {
            try {

                var tomail = db.Users.FirstOrDefault(x => x.U_mail == mail);
                if (tomail != null)
                {
                    TempData["userRegistered"] = "Failed";
                    return View();
                }
                else
                {
                    string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

                    var reg = new User
                    {
                        U_mail = mail,
                        U_name = name,
                        U_password = hashedPassword,
                        U_role = "0",
                        createdAt = DateTime.Now.ToString(),
                        updatedAt = DateTime.Now.ToString(),
                    };
                    db.Users.Add(reg);
                    db.SaveChanges();
                    SendMail("Account Created Successfully", "\n\n\t\t\tYour Has been Created on Karma Shoes,\n\n\t\t\t Happy Shopping :) ",mail);
                    TempData["userRegistered"] = "success";
                    return RedirectToAction("login");
                }

            } 
            
            catch (Exception ex)
            {
                return View();
            }
        }

        public ActionResult Tracking(string order)
        {
            if(order == null)
            {

            return View();
            }
            try
            {
                var ordersForUser = db.Billings.Where(x => x.O_id == order).ToList();
              
                if (ordersForUser.Any())
                {
                    List<collect> cartItems = new List<collect>();

                    foreach (var item in ordersForUser)
                    {
                        cartItems.Add(new collect
                        {
                            P_ID = int.Parse(item.P_id),
                            P_Qty = int.Parse(item.P_qty)
                        });
                        ViewBag.address = item.B_address;
                        ViewBag.zip = item.B_zip;
                        ViewBag.city = item.B_city;
                        ViewBag.orderID = item.O_id;
                        ViewBag.date = item.createdAt;
                        ViewBag.status = item.O_status;
                    }
                    Session["cart"] = cartItems;

                    if (Session["cart"] != null)
                    {
                        List<collect> mainlist = (List<collect>)Session["cart"];
                        List<Cart> viewlist = new List<Cart>();
                        foreach (var item in mainlist)
                        {
                            Cart obj = new Cart();
                            Product prop = db.Products.Where(x => x.P_id == item.P_ID).FirstOrDefault();
                            obj.P_ID = prop.P_id;
                            obj.P_Name = prop.P_name;
                            obj.qty = item.P_Qty;
                            obj.stock = Convert.ToInt32(prop.P_stock);
                            obj.P_Pic = prop.P_img1;
                            obj.P_Price = Convert.ToInt32((prop.P_dprice ?? prop.P_price).ToString() ?? "0");
                            obj.P_Totalprice = item.P_Qty * Convert.ToInt32((prop.P_dprice ?? prop.P_price).ToString() ?? "0");
                            viewlist.Add(obj);
                        }
                        TempData["orderTrack"] = "Success";
                        return View(viewlist);
                    }

                }
                else
                {
                    TempData["orderTrack"] = "Failed";
                    return View();
                }
            }
            catch (Exception ex) { 
            
                    return RedirectToAction("index");
            }
                    return RedirectToAction("index");
        
        }

        public ActionResult Contact()
        {
            return View();
        }
     
        [HttpPost]
        public ActionResult Contact(string message,string subject,string name, string email)
        {
            var ct = new Contact
            {
                C_name = name,
                C_mail = email,
                C_subject = subject,
                C_status = "Pending",
                createdAt = DateTime.Now.ToString(),
                updatedAt = DateTime.Now.ToString(),
                C_message = message
            };
            db.Contacts.Add(ct);
            db.SaveChanges();
            TempData["contactMSG"] = "success";
            return View();
        }
        
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("index");
        }

        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ChangePassword(string cp)
        {
            try
            {
                string uid = Session["User_ID"].ToString();
                int I = int.Parse(uid);
                var tomail = db.Users.FirstOrDefault(x => x.U_id == I);
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(cp);
                tomail.U_password = hashedPassword;
                tomail.updatedAt = DateTime.Now.ToString();
                db.Entry(tomail).State = EntityState.Modified;
                db.SaveChanges();
                TempData["changePassword"] = "success";
                return View();
            }   
            
            catch(Exception ex)
            {

            return View();
            }
        }


        public ActionResult SendMail(string subject,string message,string to)
        {
            string smtpServer = "smtp.gmail.com";
            string smtpUsername = "specificprojectmail@gmail.com";
            string smtpPassword = "bmtf akpv vjxc vevb";
            int smtpPort = 587;

            SmtpClient smtpClient = new SmtpClient(smtpServer)
            {
                Port = smtpPort,
                Credentials = new NetworkCredential(smtpUsername, smtpPassword),
                EnableSsl = true,
            };

            MailMessage mailMessage = new MailMessage
            {
                From = new MailAddress(smtpUsername, "Karma Shoes"),
                Subject = subject,
                Body = message,
                IsBodyHtml = false,
            };

            mailMessage.To.Add(to);

            try
            {
                smtpClient.Send(mailMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return RedirectToAction("index");
        }
    
    public ActionResult sendOrderMail(string OrderID)
        {

            string smtpServer = "smtp.gmail.com";
            string smtpUsername = "specificprojectmail@gmail.com";
            string smtpPassword = "bmtf akpv vjxc vevb";
            int smtpPort = 587;

            SmtpClient smtpClient = new SmtpClient(smtpServer)
            {
                Port = smtpPort,
                Credentials = new NetworkCredential(smtpUsername, smtpPassword),
                EnableSsl = true,
            };


            string uid = Session["User_ID"].ToString();
            string umail = Session["User_Mail"].ToString();
            var ordersForUser = db.Billings.Where(x => x.U_id == uid && x.O_id == OrderID).ToList();
            StringBuilder orderDetails = new StringBuilder();
            double total = 0;
            orderDetails.Append("<table border='1'>");
            orderDetails.Append("<tr><th>Product Name</th><th>Product Price</th><th>Quantity</th><th>Total</th></tr>");

            foreach (var item in ordersForUser)
            {
                int productId = Convert.ToInt32(item.P_id);

                Product prop = db.Products.FirstOrDefault(x => x.P_id == productId);
                int productPrice = Convert.ToInt32(prop.P_dprice ?? prop.P_price); 
                int quantity = Convert.ToInt32(item.P_qty);
                int tot = quantity * productPrice;
                total += tot;

                orderDetails.Append("<tr>");
                orderDetails.Append("<td>" + prop.P_name + "</td>");
                orderDetails.Append("<td>" + productPrice + "</td>");
                orderDetails.Append("<td>" + quantity + "</td>");
                orderDetails.Append("<td>" + tot + "</td>");
                orderDetails.Append("</tr>");
            }
            total += 250;
            orderDetails.Append("<tr><td colspan='3'><strong>Total</strong></td><td>" + total + "</td></tr>");
            orderDetails.Append("</table>");


            MailMessage mailMessage = new MailMessage
            {
                From = new MailAddress(smtpUsername, "Karma Shoes"),
                Subject = "Your Order Details",
                Body = "Thank you for your order. Here are the details of your purchase on the Order ID : "+OrderID+"  :<br/><br/>\t\t" + orderDetails.ToString()+ "<br/><br/>\t\tThank You  For Shopping Here :)",
                IsBodyHtml = true,
            };

            mailMessage.To.Add(umail);

            try
            {
                smtpClient.Send(mailMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }


            return View();
        }

        public string GenerateOrderID()
        {
            DateTime now = DateTime.Now;
            string timestamp = now.ToString("yyyyMMddHHmmss");
            Random random = new Random();
            int randomNumber = random.Next(1000, 9999);
            string orderID = timestamp + randomNumber.ToString();
            return orderID;
        }


        public static string GenerateRandomPassword(int length = 12)
        {
            Random random = new Random();
            string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()_+-=[]{}|;:,.<>?";
            var password = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                password.Append(validChars[random.Next(validChars.Length)]);
            }
            return password.ToString();
        }

    }
}