using shoes.Models;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Text;

namespace shoes.Controllers
{
    public class AdminController : Controller
    {
        shoesEntities db = new shoesEntities();

        public ActionResult Index()
        {
            TempData["active"] = "dashboard";
            TempData["activeTab"] = "home";
            ViewBag.Product = db.Products.ToList().Count();
            ViewBag.Orders = db.Billings.ToList().Count();
            ViewBag.Slider = db.Sliders.Where(x=>x.S_status == "1").ToList().Count();
            ViewBag.User = db.Users.Where(x=>x.U_role == "0").ToList().Count();
            var distinctOrders = db.Billings
    .GroupBy(b => b.O_id)
    .Select(group => group.FirstOrDefault())
    .ToList();
            return View(distinctOrders);
        }

//Sliders       
        public ActionResult AddSlider()
        {
            TempData["active"] = "slider";
            TempData["activeTab"] = "addslider";
            return View();
        }

        [HttpPost]
        public ActionResult AddSlider(Slider sld, HttpPostedFileBase file, string status)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var slide = new Slider
                    {
                        S_name = sld.S_name,
                        S_desc = sld.S_desc,
                        S_status = status,
                        createdAt = sld.createdAt,
                        updatedAt = sld.updatedAt,
                    };

                    if (file != null && file.ContentLength > 0)
                    {
                        var h = Server.MapPath(Path.Combine("~/assets/uploads/Sliders/" + file.FileName));
                        file.SaveAs(h);
                        slide.S_img = file.FileName;
                    }

                    db.Sliders.Add(slide);
                    db.SaveChanges();
                    TempData["Success"] = "Success";
                    return RedirectToAction("addslider");
                }

            }
            catch (Exception e)
            {
                TempData["Error"] = "Error";
                TempData["Msg"] = e.ToString();
                return RedirectToAction("addslider");
            }

            return RedirectToAction("addslider");
        }


        public ActionResult ListSlider()
        {
            TempData["active"] = "slider";
            TempData["activeTab"] = "listslider";
            return View(db.Sliders.ToList());
        }

        public ActionResult EditSlider(int id)
        {
            TempData["active"] = "slider";
            TempData["activeTab"] = "listslider";
            var editSlider = db.Sliders.Where(x => x.S_id == id).FirstOrDefault();
            return View(editSlider);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditSlider(Slider sld, HttpPostedFileBase file, string status)
        {
            TempData["active"] = "slider";
            TempData["activeTab"] = "listslider";

            try
            {
                var existingSlider = db.Sliders.Find(sld.S_id);

                if (existingSlider != null)
                {
                    existingSlider.S_name = sld.S_name;
                    existingSlider.S_desc = sld.S_desc;
                    existingSlider.S_status = status;
                    existingSlider.updatedAt = sld.updatedAt;

                    if (file != null && file.ContentLength > 0)
                    {
                        var imagePath = Server.MapPath("~/assets/uploads/Sliders/" + file.FileName);
                        file.SaveAs(imagePath);
                        existingSlider.S_img = file.FileName;
                    }
                    else
                    {
                        existingSlider.S_img = sld.S_img;
                    }

                    db.Entry(existingSlider).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["Success"] = "Updated Success";
                    return RedirectToAction("listslider");
                }
                else
                {
                    TempData["Error"] = "Error";
                    TempData["Msg"] = "Slider not found!";
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                TempData["Error"] = "Error";
                TempData["Msg"] = "Concurrency conflict occurred!";
            }
            return View(sld);
        }

        public ActionResult DeleteSlider(int id)
        {
            try
            {
                var sliderToDelete = db.Sliders.Find(id);
                
                if (sliderToDelete != null)
                {
                     db.Sliders.Remove(sliderToDelete);
                    db.SaveChanges(); 

                    return Json(new { success = true,
                        message = "Slider found" });
                    }
                else
                {
                    return Json(new { success = false,
                        message = "Slider not found" });
                    }
            }
            catch (Exception ex)
            {
                 return Json(new { success = false, message = $"Error deleting slider: {ex.Message}" });
            }
        }


        //Cateories 

        public ActionResult AddCategory()
        {
            TempData["active"] = "Category";
            TempData["activeTab"] = "addCategory";
            return View();
        }

        [HttpPost]
        public ActionResult AddCategory(Category cat)
        {
            try
            {
                if (ModelState.IsValid)
                { 
                    db.Categories.Add(cat);
                    db.SaveChanges();
                    TempData["Success"] = "Success";
                    return RedirectToAction("addcategory");
                }

            }
            catch (Exception e)
            {
                TempData["Error"] = "Error";
                TempData["Msg"] = e.ToString();
                return RedirectToAction("addcategory");
            }

            return RedirectToAction("addcategory");
        }

        public ActionResult ListCategory()
        {
            TempData["active"] = "Category";
            TempData["activeTab"] = "listCategory";
            return View(db.Categories.ToList());
        }

        public ActionResult EditCategory(int id)
        {
            TempData["active"] = "Category";
            TempData["activeTab"] = "listCategory";
            var editSlider = db.Categories.Where(x => x.C_id == id).FirstOrDefault();
            return View(editSlider);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCategory(Category cat)
        {
            TempData["active"] = "category";
            TempData["activeTab"] = "listcategory";

            try
            {
                    db.Entry(cat).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["Success"] = "Updated Success";
                    return RedirectToAction("listcategory");
             
            }
            catch (DbUpdateConcurrencyException ex)
            {
                TempData["Error"] = "Error";
                TempData["Msg"] = "Concurrency conflict occurred!";
            }
            return View(cat);
        }

        public ActionResult DeleteCategory(int id)
        {
            try
            {
                var toDelete = db.Categories.Find(id);

                if (toDelete != null)
                {
                    db.Categories.Remove(toDelete);
                    db.SaveChanges();

                    return Json(new
                    {
                        success = true,
                        message = "Slider found"
                    });
                }
                else
                {
                    return Json(new
                    {
                        success = false,
                        message = "Slider not found"
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error deleting slider: {ex.Message}" });
            }
        }


        //Product

        public ActionResult AddProduct()
        {
            TempData["active"] = "product";
            TempData["activeTab"] = "addproduct";
            ViewBag.options = db.Categories.ToList();

            return View();
        }

        [HttpPost]
        public ActionResult AddProduct(Product pdt, HttpPostedFileBase file1, HttpPostedFileBase file2, HttpPostedFileBase file3, string cat)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var pro = new Product
                    {
                        P_name = pdt.P_name,
                        P_desc = pdt.P_desc,
                        P_cat = cat,
                        P_price = pdt.P_price,
                        P_stock = pdt.P_stock,
                        P_dprice = pdt.P_dprice,
                        createdAt = pdt.createdAt,
                        updatedAt = pdt.updatedAt,
                    };

                    if (file1 != null && file1.ContentLength > 0)
                    {
                        var h = Server.MapPath(Path.Combine("~/assets/uploads/Products/1" + file1.FileName));
                        file1.SaveAs(h);
                        pro.P_img1 = "1"+file1.FileName;
                    }

                    if (file2 != null && file2.ContentLength > 0)
                    {
                        var g = Server.MapPath(Path.Combine("~/assets/uploads/Products/2" + file2.FileName));
                        file2.SaveAs(g);
                        pro.P_img2 = "2"+file2.FileName;
                    }

                    if (file3 != null && file3.ContentLength > 0)
                    {
                        var i = Server.MapPath(Path.Combine("~/assets/uploads/Products/3" + file3.FileName));
                        file3.SaveAs(i);
                        pro.P_img3 = "3" + file3.FileName;
                    }

                    db.Products.Add(pro);
                    db.SaveChanges();
                    TempData["Success"] = "Success";
                    return RedirectToAction("addproduct");
                }

            }
            catch (Exception e)
            {
                TempData["Error"] = "Error";
                TempData["Msg"] = e.ToString();
                return RedirectToAction("addslider");
            }

            return RedirectToAction("addslider");
        }

        public ActionResult ListProduct()
        {
            TempData["active"] = "product";
            TempData["activeTab"] = "listproduct";
            return View(db.Products.ToList());
        }

        public ActionResult EditProduct(int id)
        {
            TempData["active"] = "product";
            TempData["activeTab"] = "listproduct";
            var editSlider = db.Products.Where(x => x.P_id == id).FirstOrDefault();
            ViewBag.options = db.Categories.ToList();
            return View(editSlider);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProduct(Product pdt, HttpPostedFileBase file1, HttpPostedFileBase file2, HttpPostedFileBase file3, string cat)
        {
            TempData["active"] = "product";
            TempData["activeTab"] = "listproduct";
            ViewBag.options = db.Categories.ToList();

            try
            {
                var exist = db.Products.Find(pdt.P_id);

                if (exist != null)
                {
                    exist.P_name = pdt.P_name;
                    exist.P_desc = pdt.P_desc;
                    exist.P_cat = cat;
                    exist.P_price = pdt.P_price;
                    exist.P_stock = pdt.P_stock;
                    exist.P_dprice = pdt.P_dprice;

                    exist.updatedAt = pdt.updatedAt;

                    if (file1 != null && file1.ContentLength > 0)
                    {
                        var h = Server.MapPath(Path.Combine("~/assets/uploads/Products/1" + file1.FileName));
                        file1.SaveAs(h);
                        exist.P_img1 = "1" + file1.FileName;
                    }
                    else
                    {
                        exist.P_img1 = pdt.P_img1;
                    }

                    if (file2 != null && file2.ContentLength > 0)
                    {
                        var g = Server.MapPath(Path.Combine("~/assets/uploads/Products/2" + file2.FileName));
                        file2.SaveAs(g);
                        exist.P_img2 = "2" + file2.FileName;
                    }
                    else
                    {
                        exist.P_img2 = pdt.P_img2;
                    }

                    if (file3 != null && file3.ContentLength > 0)
                    {
                        var i = Server.MapPath(Path.Combine("~/assets/uploads/Products/3" + file3.FileName));
                        file3.SaveAs(i);
                        exist.P_img3 = "3" + file3.FileName;
                    }
                    else
                    {
                        exist.P_img3 = pdt.P_img3;
                    }

                    db.Entry(exist).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["Success"] = "Updated Success";
                    return RedirectToAction("listproduct");
                }
                else
                {
                    TempData["Error"] = "Error";
                    TempData["Msg"] = "Product not found!";
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                TempData["Error"] = "Error";
                TempData["Msg"] = "Concurrency conflict occurred!";
            }
            return View(pdt);
        }

        public ActionResult DeleteProduct(int id)
        {
            try
            {
                var toDelete = db.Products.Find(id);

                if (toDelete != null)
                {
                    db.Products.Remove(toDelete);
                    db.SaveChanges();

                    return Json(new
                    {
                        success = true,
                        message = "Product found"
                    });
                }
                else
                {
                    return Json(new
                    {
                        success = false,
                        message = "Product not found"
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error deleting Product: {ex.Message}" });
            }
        }


        //Reviews
        public ActionResult ListReviews()
        {
            TempData["active"] = "product";
            TempData["activeTab"] = "listproduct";


            return View(db.Reviews.ToList());
        }

        public ActionResult DeleteReview(int id)
        {
            try
            {
                var toDelete = db.Reviews.Find(id);

                if (toDelete != null)
                {
                    db.Reviews.Remove(toDelete);
                    db.SaveChanges();

                    return Json(new
                    {
                        success = true,
                        message = "Review found"
                    });
                }
                else
                {
                    return Json(new
                    {
                        success = false,
                        message = "Product not found"
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error deleting Product: {ex.Message}" });
            }
        }


        //Contact
        public ActionResult ListContact()
        {
            TempData["active"] = "product";
            TempData["activeTab"] = "listproduct";


            return View(db.Contacts.ToList());
        }

        public ActionResult ChangeStatus(int id)
        {
            try
            {
                var toDelete = db.Contacts.Find(id);

                if (toDelete != null)
                {
                    toDelete.C_status = "Issuse Resolved";
                    toDelete.updatedAt = DateTime.Now.ToString();
                    db.Entry(toDelete).State = EntityState.Modified;
                    db.SaveChanges();
                    SendMail("Update On your Query", "THe query is resolved", toDelete.C_mail);
                    return Json(new
                    {
                        success = true,
                        message = "Review found"
                    });
                }
                else
                {
                    return Json(new
                    {
                        success = false,
                        message = "Product not found"
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error deleting Product: {ex.Message}" });
            }
        }
        
        
        
        //Orders
        public ActionResult ListOrders()
        {
            TempData["active"] = "product";
            TempData["activeTab"] = "listproduct";

            var distinctOrders = db.Billings
                .GroupBy(b => b.O_id)
                .Select(group => group.FirstOrDefault())
                .ToList();

            return View(distinctOrders);
        }

        public ActionResult ChangeOrderStatus(int id)
        {
            try
            {
                string uid = "";
                var order = db.Billings.Find(id);
                var toDelete = db.Billings.Where(x => x.O_id == order.O_id).ToList();
                if (toDelete.Any())
                {
                    foreach (var it in toDelete)
                    {
                        if (it != null)
                        {
                            it.O_status = it.O_status == "Pending" ? "Shipped" : it.O_status == "Shipped" ? "Delivered" : "";
                            it.updatedAt = DateTime.Now.ToString();

                            db.Entry(it).State = EntityState.Modified;
                            db.SaveChanges();
                            uid = it.U_id;                  
                        }
                    }

                    db.SaveChanges();
                    
                    var toMail = db.Users.Find(int.Parse(order.U_id));
                    string msg = "Your Order # " + order.O_id + " Has been " +
     (order.O_status == "Pending" ? "Shipped" : order.O_status == "Shipped" ? "Delivered" : "") +
     "\n and will shortly be delivered";

                    SendMail("Order Update", msg, toMail.U_mail);
                
                    return Json(new
                    {
                        success = true,
                        message = "Review found"
                    });
                }
                else
                {
                    return Json(new
                    {
                        success = false,
                        message = "Product not found"
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error deleting Product: {ex.Message}" });
            }
        }


        //Users
        public ActionResult ListUsers()
        {
            TempData["active"] = "product";
            TempData["activeTab"] = "listuser";


            return View(db.Users.Where(x=>x.U_role=="0").ToList());
        }



        public ActionResult sendOrderMail(string OrderID, string uid, string umail)
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
                Body = "Thank you for your order. Here are the details of your purchase on the Order ID : " + OrderID + "  :<br/><br/>\t\t" + orderDetails.ToString() + "<br/><br/>\t\tThank You  For Shopping Here :)",
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



        public ActionResult SendMail(string subject, string message, string to)
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


    }
}