﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ClassProject.Models;

namespace ClassProject.Controllers
{
    public class publishersController : Controller
    {
        private pubsEntities db = new pubsEntities();

        // GET: publishers
        public ActionResult Index(string name,string city ,string country, string state)
        {
            var publishers = db.publishers.Include(p => p.pub_info).ToList();
      
            var publisher_names= publishers.Select(p => p.pub_name).Distinct().ToList();
            var publisher_cities = publishers.Select(p => p.city).Distinct().ToList();
            var publisher_states = publishers.Select(p => p.state).Distinct().ToList();
            var publisher_countries = publishers.Select(p => p.country).Distinct().ToList();

            ViewBag.publisher_names = publisher_names;
            ViewBag.publisher_cities = publisher_cities;
            ViewBag.publisher_states = publisher_states;
            ViewBag.publisher_countries = publisher_countries;

            if (!String.IsNullOrEmpty(name))
            {
                publishers = publishers.Where(p => p.pub_name.Contains(name)).ToList();
                ViewBag.name = name;
            }
            if (!String.IsNullOrEmpty(city))
            {
                publishers = publishers.Where(p => p.city.Contains(city)).ToList();
                ViewBag.city = city;
            }
            if (!String.IsNullOrEmpty(country))
            {
                publishers = publishers.Where(p => p.country.Contains(country)).ToList();
                ViewBag.country = country;
            }
            if (!String.IsNullOrEmpty(state))
            {
                publishers = publishers.Where(p => p.state.Contains(state)).ToList();
                ViewBag.state = state;
            }
          
            return View(publishers);
        }

        // GET: publishers/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            publisher publisher = db.publishers.Find(id);
            if (publisher == null)
            {
                return HttpNotFound();
            }
            return View(publisher);
        }

        // GET: publishers/Create
        public ActionResult Create()
        {
            ViewBag.pub_id = new SelectList(db.pub_info, "pub_id", "pr_info");
            return View();
        }

        // POST: publishers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "pub_id,pub_name,city,state,country")] publisher publisher)
        {
            if (ModelState.IsValid)
            {
                db.publishers.Add(publisher);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.pub_id = new SelectList(db.pub_info, "pub_id", "pr_info", publisher.pub_id);
            return View(publisher);
        }

        // GET: publishers/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            publisher publisher = db.publishers.Find(id);
            if (publisher == null)
            {
                return HttpNotFound();
            }
            ViewBag.pub_id = new SelectList(db.pub_info, "pub_id", "pr_info", publisher.pub_id);
            return View(publisher);
        }

        // POST: publishers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "pub_id,pub_name,city,state,country")] publisher publisher)
        {
            if (ModelState.IsValid)
            {
                db.Entry(publisher).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.pub_id = new SelectList(db.pub_info, "pub_id", "pr_info", publisher.pub_id);
            return View(publisher);
        }

        // GET: publishers/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            publisher publisher = db.publishers.Find(id);
            if (publisher == null)
            {
                return HttpNotFound();
            }
            return View(publisher);
        }

        // POST: publishers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            publisher publisher = db.publishers.Find(id);
            publisher.Delete(db);
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

        [AcceptVerbs("GET", "POST")]
        public JsonResult VerifyPublisherId(string pub_id, string editMode)
        {
            if (editMode == "edit")
                return Json(true, JsonRequestBehavior.AllowGet);

            if (pub_id != null && db.publishers.Where(item => item.pub_id == pub_id).Count() != 0)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}
