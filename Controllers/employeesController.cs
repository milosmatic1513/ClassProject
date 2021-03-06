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
    public class employeesController : Controller
    {
        private pubsEntities db = new pubsEntities();

        // GET: employees
        public ActionResult Index(string firstname, string minit, string lastname, string job_lvl, string hire_date_from,string hire_date_to, string job_desc, string pub_name, string orderby, string order)
        {
            List<employee> employees = db.employees.Include(e => e.job).Include(e => e.publisher).ToList();
            //set a list of available minit
            var minitList = employees.Select(s => s.minit).Distinct();
            //set a list of available job Descriptions
            var joblist = employees.Select(s => s.job.job_desc).Distinct();
            //set a list of available job Descriptions
            var pub_names = employees.Select(s => s.publisher.pub_name).Distinct();

            //Add Selections to Viewbag	
            ViewBag.minitList = minitList;
            ViewBag.joblist = joblist;
            ViewBag.pub_names = pub_names;

            //add orderby value to viebag
            ViewBag.orderby = orderby;
            //add order value to viebag
            ViewBag.order = order;

            //Apply Filters 	
            if (!String.IsNullOrEmpty(firstname))
            {
                employees = employees.Where(s => s.fname.ToLower().Contains(firstname.ToLower())).ToList();
                ViewBag.firstname = firstname;
            }
            if (!String.IsNullOrEmpty(minit))
            {
                employees = employees.Where(s => s.minit.Contains(minit)).ToList();
                ViewBag.minit = minit;

            }
            if (!String.IsNullOrEmpty(lastname))
            {
                employees = employees.Where(s => s.lname.ToLower().Contains(lastname.ToLower())).ToList();
                ViewBag.lastname = lastname;
            }
            if (!String.IsNullOrEmpty(job_lvl))
            {
                employees = employees.Where(s => s.job_lvl == Int32.Parse(job_lvl)).ToList();
                ViewBag.job_lvl = job_lvl;
            }
            if (!String.IsNullOrEmpty(hire_date_from))
            {
                employees = employees.Where(s => DateTime.Compare(s.hire_date, DateTime.ParseExact(hire_date_from, "yyyy-MM-dd", null)) >= 0 ).ToList();  // s.hire_date is later or the same date as hire_date
                ViewBag.hire_date_from = hire_date_from;
            }
            if (!String.IsNullOrEmpty(hire_date_to))
            {
                employees = employees.Where(s => DateTime.Compare(s.hire_date, DateTime.ParseExact(hire_date_to, "yyyy-MM-dd", null)) <= 0).ToList();  // s.hire_date is later or the same date as hire_date
                ViewBag.hire_date_to = hire_date_to;
            }
            if (!String.IsNullOrEmpty(job_desc))
            {
                employees = employees.Where(s => s.job.job_desc.Contains(job_desc)).ToList();
                ViewBag.job_desc = job_desc;
            }
            if (!String.IsNullOrEmpty(pub_name))
            {
                employees = employees.Where(s => s.publisher.pub_name.Contains(pub_name)).ToList();
                ViewBag.pub_name = pub_name;
            }


            //Ordering 
            if (orderby == "firstname")
            {
                employees = employees.OrderBy(s => s.fname).ToList();
            }
            else if (orderby == "lastname")
            {
                employees = employees.OrderBy(s => s.lname).ToList();
            }
            else if (orderby == "minit")
            {
                employees = employees.OrderBy(s => s.minit).ToList();
            }
            else if (orderby == "job_lvl")
            {
                employees = employees.OrderBy(s => s.job_lvl).ToList();
            }
            else if (orderby == "hire_date")
            {
                employees = employees.OrderBy(s => s.hire_date).ToList();
            }
            else if (orderby == "job_desc")
            {
                employees = employees.OrderBy(s => s.job.job_desc).ToList();
            }
            else if (orderby == "pub_name")
            {
                employees = employees.OrderBy(s => s.publisher.pub_name).ToList();
            }

            //check the order of the list
            if (order == "desc")
            {
                employees.Reverse();
            }

            return View(employees.ToList());
        }

        // GET: employees/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            employee employee = db.employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }

            ViewBag.ReferUrl = ReferenceUrl.ReferUrl(Request);
            return View(employee);
        }

        // GET: employees/Create
        public ActionResult Create(string pub_id)
        {
            ViewBag.job_id = new SelectList(db.jobs, "job_id", "job_desc");

            if (pub_id != null)
                ViewBag.pub_id = new SelectList(db.publishers.Where(item => item.pub_id == pub_id), "pub_id", "pub_name");
            else
                ViewBag.pub_id = new SelectList(db.publishers, "pub_id", "pub_name");

            ViewBag.ReferUrl = ReferenceUrl.ReferUrl(Request);
            return View();
        }

        // POST: employees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "emp_id,fname,minit,lname,job_id,job_lvl,pub_id,hire_date")] employee employee, string referUrl)
        {
            if (ModelState.IsValid)
            {
                db.employees.Add(employee);
                db.SaveChanges();

                return RedirectUrl(referUrl);
            }

            ViewBag.job_id = new SelectList(db.jobs, "job_id", "job_desc", employee.job_id);
            ViewBag.pub_id = new SelectList(db.publishers, "pub_id", "pub_name", employee.pub_id);
            return View(employee);
        }

        // GET: employees/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            employee employee = db.employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            ViewBag.job_id = new SelectList(db.jobs, "job_id", "job_desc", employee.job_id);
            ViewBag.pub_id = new SelectList(db.publishers, "pub_id", "pub_name", employee.pub_id);

            ViewBag.ReferUrl = ReferenceUrl.ReferUrl(Request);
            return View(employee);
        }

        // POST: employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "emp_id,fname,minit,lname,job_id,job_lvl,pub_id,hire_date")] employee employee, string referUrl)
        {
            if (ModelState.IsValid)
            {
                db.Entry(employee).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectUrl(referUrl);
            }
            ViewBag.job_id = new SelectList(db.jobs, "job_id", "job_desc", employee.job_id);
            ViewBag.pub_id = new SelectList(db.publishers, "pub_id", "pub_name", employee.pub_id);
            return View(employee);
        }

        // GET: employees/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            employee employee = db.employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }

            ViewBag.ReferUrl = ReferenceUrl.ReferUrl(Request);
            return View(employee);
        }

        // POST: employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id, string referUrl)
        {
            employee employee = db.employees.Find(id);
            employee.Delete(db);
            db.SaveChanges();

            return RedirectUrl(referUrl);
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
        public JsonResult VerifyEmployeeId(string emp_id, string editMode)
        {
            if (editMode == "edit")
                return Json(true, JsonRequestBehavior.AllowGet);

            if (emp_id != null && db.employees.Where(item => item.emp_id == emp_id).Count() != 0)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [NonAction]
        private ActionResult RedirectUrl(string referUrl)
        {
            if (string.IsNullOrWhiteSpace(referUrl))
                return RedirectToAction("Index"); // if referUrl is missing redirect to Index
            else
                return Redirect(referUrl); // else redirect to referUrl
        }
    }
}
