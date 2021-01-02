using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Data_Base_Layer;

namespace LibraryManagmentSystem.Controllers
{
    public class SuplierTablesController : Controller
    {
        private OnlineMgtSystemEntities db = new OnlineMgtSystemEntities();

        // GET: SuplierTables
        public ActionResult Index()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["UserID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            var suplierTables = db.SuplierTables.Include(s => s.UserTable);
            return View(suplierTables.ToList());
        }

        // GET: SuplierTables/Details/5
        public ActionResult Details(int? id)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["UserID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SuplierTable suplierTable = db.SuplierTables.Find(id);
            if (suplierTable == null)
            {
                return HttpNotFound();
            }
            return View(suplierTable);
        }

        // GET: SuplierTables/Create
        public ActionResult Create()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["UserID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            return View();
        }

        // POST: SuplierTables/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SuplierTable suplierTable)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["UserID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            int userid = Convert.ToInt32(Convert.ToString(Session["UserID"]));
            suplierTable.UserID = userid;
            if (ModelState.IsValid)
            {
                var find = db.SuplierTables.Where(s => s.SuplierName == suplierTable.SuplierName && s.ContactNo == suplierTable.ContactNo).FirstOrDefault();
                if (find == null)
                {
                    db.SuplierTables.Add(suplierTable);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Massege = "Suplier Alredy Registerd";
                }
            }

            return View(suplierTable);
        }

        // GET: SuplierTables/Edit/5
        public ActionResult Edit(int? id)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["UserID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SuplierTable suplierTable = db.SuplierTables.Find(id);
            if (suplierTable == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserID = new SelectList(db.UserTables, "UserID", "UserName", suplierTable.UserID);
            return View(suplierTable);
        }

        // POST: SuplierTables/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SuplierTable suplierTable)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["UserID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            int userid = Convert.ToInt32(Convert.ToString(Session["UserID"]));
            suplierTable.UserID = userid;
            if (ModelState.IsValid)
            {
                db.Entry(suplierTable).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UserID = new SelectList(db.UserTables, "UserID", "UserName", suplierTable.UserID);
            return View(suplierTable);
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
