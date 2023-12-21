using shoes.Models;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace shoes.Controllers
{
    public class AdminController : Controller
    {
        shoesEntities db = new shoesEntities();

        // GET: Admin
        public ActionResult Index()
        {
            TempData["active"] = "dashboard";
            TempData["activeTab"] = "index";
            return View();
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
            TempData["active"] = "category";
            TempData["activeTab"] = "addcategory";
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
            TempData["active"] = "category";
            TempData["activeTab"] = "listcategory";
            return View(db.Categories.ToList());
        }

        public ActionResult EditCategory(int id)
        {
            TempData["active"] = "slider";
            TempData["activeTab"] = "listcategory";
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
                //var existingSlider = db.Sliders.Find(cat.S_id);

                //if (existingSlider != null)
                //{
                    //existingSlider.S_name = sld.S_name;
                    //existingSlider.S_desc = sld.S_desc;
                    //existingSlider.S_status = status;
                    //existingSlider.updatedAt = sld.updatedAt;

                    //if (file != null && file.ContentLength > 0)
                    //{
                    //    var imagePath = Server.MapPath("~/assets/uploads/Sliders/" + file.FileName);
                    //    file.SaveAs(imagePath);
                    //    existingSlider.S_img = file.FileName;
                    //}

                    db.Entry(cat).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["Success"] = "Updated Success";
                    return RedirectToAction("listslider");
                //}
                //else
                //{
                //    TempData["Error"] = "Error";
                //    TempData["Msg"] = "Slider not found!";
                //}
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

    }
}