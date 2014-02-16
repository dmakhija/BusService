using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DMBusService.Models;

namespace DMBusService.Controllers
{
    public class DMRouteController : Controller
    {
        private BusServiceContext db = new BusServiceContext();

        //
        // GET: /DMRoute/

        public ActionResult Index()
        {
           // TempData["message"] = "tempdata";
            return View(db.busRoutes.ToList());
        }

        //
        // GET: /DMRoute/Details/5

        public ActionResult Details(string id = null)
        {
            busRoute busroute = db.busRoutes.Find(id);
            if (busroute == null)
            {
                return HttpNotFound();
            }
            return View(busroute);
        }

        //
        // GET: /DMRoute/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /DMRoute/Create

        [HttpPost]
        public ActionResult Create(busRoute busroute)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.busRoutes.Add(busroute);
                    db.SaveChanges();
                    TempData["message"] = "Bus Route Added Successfully.";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    
                    //throw;
                    TempData["message"] = "There was an error while adding a Bus Route :- "  +
                    ex.GetBaseException().Message;
                    return RedirectToAction("Create");
                }
            }
            return View(busroute);
        }

        //
        // GET: /DMRoute/Edit/5

        public ActionResult Edit(string id = null)
        {           
            busRoute busroute = db.busRoutes.Find(id);
            if (busroute == null)
            {
                return HttpNotFound();
            }
            return View(busroute);
        }

        //
        // POST: /DMRoute/Edit/5

        [HttpPost]
        public ActionResult Edit(busRoute busroute)
        {   
            if (ModelState.IsValid)
            {
                try 
	            {	        
		            db.Entry(busroute).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["message"] = "Bus Route was modified successfully.";
                    return RedirectToAction("Index");
	            }
	            catch (Exception ex)
	            {
                    TempData["message"] = "There was an error while modifying the selected Bus Route :- " + ex.GetBaseException().Message;
		            //throw;
	            }
                
            }
            return View(busroute);
        }

        //
        // GET: /DMRoute/Delete/5

        public ActionResult Delete(string id = null)
        {
            busRoute busroute = db.busRoutes.Find(id);
            if (busroute == null)
            {
                return HttpNotFound();
            }
            return View(busroute);
        }

        //
        // POST: /DMRoute/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(string id)
        {
            try
            {
                busRoute busroute = db.busRoutes.Find(id);
                db.busRoutes.Remove(busroute);
                db.SaveChanges();
                TempData["message"] = "Bus Route was deleted successfully.";
            }
            catch (Exception ex)
            {
                
                //throw;
                TempData["message"] = "There was an error while deleting the Bus Route :- " + ex.GetBaseException().Message; 
            }
           
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}