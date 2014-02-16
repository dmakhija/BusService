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
    public class DMRouteStopController : Controller
    {
        private BusServiceContext db = new BusServiceContext();

        //
        // GET: /DMRouteStop/

        public ActionResult Index()
        {
            string routeCode = "routeCode";
            string routeName = "routeName";

            //check n store quesrystring parameters
            if (Request.QueryString["routeCode"] != null)
            {
                routeCode = Request.QueryString["routeCode"];
                routeName = Request.QueryString["routeName"];

                //store the values in cookies/session for further reference
                Response.Cookies.Add(new HttpCookie("routeCodeCookie", routeCode));
                Response.Cookies.Add(new HttpCookie("routeNameCookie", routeName));
                Session.Add("routeCodeSession", routeCode);
                Session.Add("routeNameSession", routeName);
            }

            //check if route code is passed as a url parameter
            if (RouteData.Values["id"] != null)
            {
                //if yes, store it in routecode variable, cookie and session
                routeCode = RouteData.Values["id"].ToString();
                routeName = db.busRoutes.Find(routeCode).routeName;

                //store the values in cookies/session for further reference
                Response.Cookies.Add(new HttpCookie("routeCodeCookie", routeCode));
                Response.Cookies.Add(new HttpCookie("routeNameCookie", routeName));
                Session.Add("routeCodeSession", routeCode);
                Session.Add("routeNameSession", routeName);
            }            


            //if route code is not found in querystring or url parameter then,
            if (RouteData.Values["id"] == null && Request.QueryString["routeCode"] == null)
            {
                //check if it is stored in cookie
                if (Request.Cookies["routeCodeCookie"].Value != null)
                {
                    //but if route code is saved in cookie earlier then,
                    //retrieve cookie value n store it in routecode variable
                    routeCode = Request.Cookies["routeCodeCookie"].Value;
                    routeName = Request.Cookies["routeNameCookie"].Value;
                }
                else
                {
                    //if cookie is null, then check session variable
                    if (Session["routeCodeSession"] != null)
                    {
                        //if route code is stored in session then,
                        //retrieve it from session and store in routecode variable
                        routeCode = Session["routeCodeSession"].ToString();
                        routeName = Session["routeNameSession"].ToString();
                    }
                    else
                    {
                        //display error message and redirect to index action in route controller
                        TempData["message"] = "Please select a Bus Route to view its stops";
                        return RedirectToAction("Index", "DMRoute");
                    }
                }
            }
            
            //set selected bus route code and bus route name in viewbag to display it in featured section in view
            ViewBag.routeCode = routeCode;
            ViewBag.routeName = routeName;

            //now pass this routecode to linq query to fetch stops for that particular bus route
            var routeStops = from record in db.routeStops
                             where (record.busRouteCode == routeCode)
                             select record;

            //check if sort order is mentioned in the querystring
            if (Request.QueryString["OrderBy"] != null)
            {
                //check the sort order type
                if (Request.QueryString["OrderBy"] == "stopLocation")
                {
                    //sort the bus route stops listing by the order of stop location
                    routeStops = routeStops.OrderBy(r=>r.busStop.location);
                }
                else
                {

                    //sort the bus route stops listing by the order of stop number
                    routeStops = routeStops.OrderBy(r => r.busStopNumber);
                }
            }
            else
            {
                //no sort order is specified in querystring,  
                //then sort the bus route stops listing by offset times asc
                routeStops = routeStops.OrderBy(r => r.offsetMinutes);
            }
            return View(routeStops.ToList());

            /*
            //now pass this routecode to entity framework query to fetch stops for that particular bus route
            var routeStops = db.routeStops.Where(record => record.busRouteCode == routeCode);
            return View(routeStops.ToList());
             */
        }

        public ActionResult routeStopSchedule(Int32 id = 0)
        {
            Int32 routeStopId = id;
            string routeCode = Request.Cookies["routeCodeCookie"].Value;
            string routeName = Request.Cookies["routeNameCookie"].Value;

            //save the selected bus route code and name in viewbag, in order to display it in RouteStopSchedule view's featured section
            ViewBag.routeCode = routeCode;
            ViewBag.routeName = routeName;

            Int32 offsetMinutes = Convert.ToInt32((from a in db.routeStops
                          where ((a.routeStopId == routeStopId) && (a.busRouteCode == routeCode))
                          select a.offsetMinutes).First());

            TimeSpan offset = TimeSpan.FromMinutes(offsetMinutes);
                        
            var routeSchedules = (from r in db.routeSchedules
                           where (r.busRouteCode == routeCode)
                           orderby r.isWeekDay, r.startTime ascending
                           select r).ToArray();

            foreach (var item in routeSchedules)
            {
                item.startTime = item.startTime.Add(offset);
            }
            return View("routeStopSchedule", routeSchedules);
            
        }
       

        //
        // GET: /DMRouteStop/Details/5

        public ActionResult Details(int id = 0)
        {
            routeStop routestop = db.routeStops.Find(id);
            if (routestop == null)
            {
                return HttpNotFound();
            }

            //set selected bus route code and bus route name in viewbag to display it in featured section in view
            ViewBag.routeCode = routestop.busRouteCode;
            ViewBag.routeName = routestop.busRoute.routeName;
            return View(routestop);
        }

        //
        // GET: /DMRouteStop/Create

        public ActionResult Create()
        {
            //retrieve the selected bus route code and name from the cookie
            string routeCode = Request.Cookies["routeCodeCookie"].Value;
            string routeName = Request.Cookies["routeNameCookie"].Value;

            //set the dropdown list in the view            
            ViewBag.busRouteCode = new SelectList(db.busRoutes, "busRouteCode", "routeName", routeCode);
            ViewBag.busStopNumber = new SelectList(db.busStops, "busStopNumber", "location");
            
            //set selected bus route code and bus route name in viewbag to display it in featured section in view
            ViewBag.routeCode = routeCode;
            ViewBag.routeName = routeName;

            return View();
        }

        //
        // POST: /DMRouteStop/Create

        [HttpPost]
        public ActionResult Create(routeStop routestop)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.routeStops.Add(routestop);
                    db.SaveChanges();
                    TempData["message"]="Bus stop added successfully.";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    TempData["message"] = "There was an error while adding a Bus Stop :- " +
                    ex.GetBaseException().Message;
                    return RedirectToAction("Create");
                }                
            }

            ViewBag.busRouteCode = new SelectList(db.busRoutes, "busRouteCode", "routeName", routestop.busRouteCode);
            ViewBag.busStopNumber = new SelectList(db.busStops, "busStopNumber", "location", routestop.busStopNumber);

            //set selected bus route code and bus route name in viewbag to display it in featured section in view
            ViewBag.routeCode = Response.Cookies["routeCodeCookie"].Value;
            ViewBag.routeName = Response.Cookies["routeNameCookie"].Value;
            return View(routestop);
        }

        //
        // GET: /DMRouteStop/Edit/5

        public ActionResult Edit(int id = 0)
        {
            routeStop routestop = db.routeStops.Find(id);
            if (routestop == null)
            {
                return HttpNotFound();
            }
            ViewBag.busRouteCode = new SelectList(db.busRoutes, "busRouteCode", "routeName", routestop.busRouteCode);
            ViewBag.busStopNumber = new SelectList(db.busStops, "busStopNumber", "location", routestop.busStopNumber);
            
            ViewBag.routeCode = routestop.busRouteCode;
            ViewBag.routeName = routestop.busRoute.routeName;
            return View(routestop);
        }

        //
        // POST: /DMRouteStop/Edit/5

        [HttpPost]
        public ActionResult Edit(routeStop routestop)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(routestop).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["message"] = "Bus Stop Details updated successfully.";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    TempData["message"] = "There was an error while editing a Bus Stop :- " +
                    ex.GetBaseException().Message;
                    return RedirectToAction("Create");
                }
            }

            ViewBag.busRouteCode = new SelectList(db.busRoutes, "busRouteCode", "routeName", routestop.busRouteCode);
            ViewBag.busStopNumber = new SelectList(db.busStops, "busStopNumber", "location", routestop.busStopNumber);
            return View(routestop);
        }

        //
        // GET: /DMRouteStop/Delete/5

        public ActionResult Delete(int id = 0)
        {
            routeStop routestop = db.routeStops.Find(id);
            if (routestop == null)
            {
                return HttpNotFound();
            }
            ViewBag.routeCode = routestop.busRouteCode;
            ViewBag.routeName = routestop.busRoute.routeName;

            return View(routestop);
        }

        //
        // POST: /DMRouteStop/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            try
            {
                routeStop routestop = db.routeStops.Find(id);
                db.routeStops.Remove(routestop);
                db.SaveChanges();
                TempData["message"] = "Bus Stop was deletd successfully.";
            }
            catch (Exception ex)
            {
                TempData["message"] = "There was an error while deleting a Bus Stop :- " +
                ex.GetBaseException().Message;
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