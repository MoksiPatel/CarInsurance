using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CarInsurance.Models;

namespace CarInsurance.Controllers
{
    public class InsureeController : Controller
    {
        private InsuranceEntities db = new InsuranceEntities();

        public ActionResult Admin()
        {
            var allInsurees = db.Insurees.ToList(); // Retrieve all Insurees from the database
            return View(allInsurees);
        }

        // GET: Insuree
        public ActionResult Index()
        {
            return View(db.Insurees.ToList());
        }

        // GET: Insuree/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Insuree insuree = db.Insurees.Find(id);
            if (insuree == null)
            {
                return HttpNotFound();
            }
            return View(insuree);
        }

        // GET: Insuree/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Insuree/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性。有关
        // 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,FirstName,LastName,EmailAddress,DateOfBirth,CarYear,CarMake,CarModel,DUI,SpeedingTickets,CoverageType,Quote")] Insuree insuree)
        {
            if (ModelState.IsValid)
            {
                // Start with a base of $50 / month.
                decimal monthlyTotal = 50;
                // Age criteria
                if (insuree.DateOfBirth.Year > DateTime.Now.Year - 18)
                {
                    // If the user is 18 or under, add $100 to the monthly total.
                    monthlyTotal += 100;
                }
                else if (insuree.DateOfBirth.Year >= DateTime.Now.Year - 25)
                {
                    // If the user is from 19 to 25, add $50 to the monthly total.
                    monthlyTotal += 50;
                }
                else
                {
                    // If the user is 26 or older, add $25 to the monthly total.
                    monthlyTotal += 25;
                }

                // Car year criteria
                if (insuree.CarYear < 2000)
                {
                    // If the car's year is before 2000, add $25 to the monthly total.
                    monthlyTotal += 25;
                }
                else if (insuree.CarYear > 2015)
                {
                    // If the car's year is after 2015, add $25 to the monthly total.
                    monthlyTotal += 25;
                }

                // Car make and model criteria
                if (insuree.CarMake == "Porsche")
                {
                    // If the car's Make is a Porsche, add $25 to the price.
                    monthlyTotal += 25;

                    if (insuree.CarModel == "911 Carrera")
                    {
                        // If the car's Make is a Porsche and its model is a 911 Carrera, add an additional $25 to the price.
                        monthlyTotal += 25;
                    }
                }

                // Add $10 to the monthly total for every speeding ticket the user has.
                monthlyTotal += 10 * insuree.SpeedingTickets;

                // If the user has ever had a DUI, add 25% to the total.
                if (insuree.DUI)
                {
                    monthlyTotal *= 1.25m;
                }

                // If it's full coverage, add 50% to the total.
                if (insuree.CoverageType)
                {
                    monthlyTotal *= 1.5m;
                }

                // Update the Quote property in the model
                insuree.Quote = monthlyTotal;

                db.Insurees.Add(insuree);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(insuree);
        }

        // GET: Insuree/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Insuree insuree = db.Insurees.Find(id);
            if (insuree == null)
            {
                return HttpNotFound();
            }
            return View(insuree);
        }

        // POST: Insuree/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性。有关
        // 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FirstName,LastName,EmailAddress,DateOfBirth,CarYear,CarMake,CarModel,DUI,SpeedingTickets,CoverageType,Quote")] Insuree insuree)
        {
            if (ModelState.IsValid)
            {
                db.Entry(insuree).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(insuree);
        }

        // GET: Insuree/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Insuree insuree = db.Insurees.Find(id);
            if (insuree == null)
            {
                return HttpNotFound();
            }
            return View(insuree);
        }

        // POST: Insuree/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Insuree insuree = db.Insurees.Find(id);
            db.Insurees.Remove(insuree);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
