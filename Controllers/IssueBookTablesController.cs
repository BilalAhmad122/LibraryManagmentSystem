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
    public class IssueBookTablesController : Controller
    {
        private OnlineMgtSystemEntities db = new OnlineMgtSystemEntities();

        // GET: IssueBookTables
        public ActionResult IssueBooks()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["UserID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            var issueBookTables = db.IssueBookTables.Include(i => i.BookTable).Include(i => i.EmployeeTable).Include(i => i.UserTable).Where(b=>b.Status==true&&b.ReservedNoOfCopies==false);
            return View(issueBookTables.ToList());
        }

        public ActionResult ReserveBooks()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["UserID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            var issueBookTables = db.IssueBookTables.Include(i => i.BookTable).Include(i => i.EmployeeTable).Include(i => i.UserTable).Where(b => b.Status == false && b.ReservedNoOfCopies == true && (b.ReturnDate - DateTime.Now.Date).Days <= 3);
            return View(issueBookTables.ToList());
        }
        public ActionResult ReturnPendingBooks()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["UserID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            List<IssueBookTable> lisit = new List<IssueBookTable>();
            var issueBookTables = db.IssueBookTables.Where(b => b.Status == true && b.ReservedNoOfCopies == true).ToList();
            foreach(var item in issueBookTables)
            {
                var returndate = item.ReturnDate;
                int noofdays= (returndate - DateTime.Now).Days;
                if (noofdays<=3)
                {
                    lisit.Add(new IssueBookTable
                    {
                        BookID = item.BookID,
                        BookTable = item.BookTable,
                        Description = item.Description,
                        EmployeeID = item.EmployeeID,
                        EmployeeTable = item.EmployeeTable,
                        IssueBookID = item.IssueBookID,
                        IssueCopies = item.IssueCopies,
                        IssueDate = item.IssueDate,
                        ReservedNoOfCopies = item.ReservedNoOfCopies,
                        ReturnDate = item.ReturnDate,
                        Status = item.Status,
                        UserID = item.UserID,
                        UserTable = item.UserTable
                    });
                }
            }
            
            return View(lisit.ToList());
        }

        // GET: IssueBookTables/Details/5
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
            IssueBookTable issueBookTable = db.IssueBookTables.Find(id);
            if (issueBookTable == null)
            {
                return HttpNotFound();
            }
            return View(issueBookTable);
        }

        // GET: IssueBookTables/Create
        public ActionResult Create()
        {
            ViewBag.IssueBookID = new SelectList(db.BookTables, "BookID", "BookType","0");
            ViewBag.EmployeeID = new SelectList(db.EmployeeTables, "EmployeeID", "FullName", "0");
            ViewBag.UserID = new SelectList(db.UserTables, "UserID", "UserName", "0");
            return View();
        }

        // POST: IssueBookTables/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IssueBookTable issueBookTable)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["UserID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            int userid = Convert.ToInt32(Convert.ToString(Session["UserID"]));
            issueBookTable.UserID = userid;
            if (ModelState.IsValid)
            {
                var find = db.IssueBookTables.Where(b => b.ReturnDate >= DateTime.Now && b.BookID == issueBookTable.BookID && (b.Status==true||b.ReservedNoOfCopies==true)).ToList();
                int issueBook = 0;
                foreach(var item in find)
                {
                    issueBook = issueBook + item.IssueCopies;
                }
                var stockBook = db.BookTables.Where(b => b.BookID == issueBook).FirstOrDefault();
                if ((issueBook==stockBook.NoOfCopies)||(issueBook + issueBookTable.IssueCopies>stockBook.NoOfCopies))
                {
                    ViewBag.Message = "Stock is Empety !";
                    return View(issueBookTable);
                }


                db.IssueBookTables.Add(issueBookTable);
                db.SaveChanges();
                ViewBag.Message = "Book Is Issue Successfully !";
                return RedirectToAction("Index");
            }

            ViewBag.IssueBookID = new SelectList(db.BookTables, "BookID", "BookType", issueBookTable.IssueBookID);
            ViewBag.EmployeeID = new SelectList(db.EmployeeTables, "EmployeeID", "FullName", issueBookTable.EmployeeID);
            ViewBag.UserID = new SelectList(db.UserTables, "UserID", "UserName", issueBookTable.UserID);
            return View(issueBookTable);
        }

        // GET: IssueBookTables/Edit/5
      

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
