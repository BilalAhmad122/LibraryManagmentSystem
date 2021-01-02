using Data_Base_Layer;
using LibraryManagmentSystem.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace LibraryManagmentSystem.Controllers
{
    public class PurchaseController : Controller
    {
        private OnlineMgtSystemEntities db = new OnlineMgtSystemEntities();

        public int Qty { get; private set; }

        public ActionResult NewPurchase()
        {

            if (string.IsNullOrEmpty(Convert.ToString(Session["UserID"])))
            {

                return RedirectToAction("Login", "Home");
            }
            double totalamount = 0;
            var temppur = db.PurTemDetailTables.ToList();
            foreach (var item in temppur)
            {
                totalamount += (item.Quty * item.UnitPrice);
            }
            ViewBag.TotalAmount = totalamount;
            return View(temppur);
        }
        [HttpPost]
        public ActionResult AddItem(int BID, float Price)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            var find = db.PurchaseDetailTables.Where(i => i.BookID == BID).FirstOrDefault();
            if (find == null)
            {
                if (BID > 0 && Price > 0)
                {
                    var newitem = new PurchaseDetailTable()
                    {
                        BookID = BID,
                        Qty = Qty,
                        UnitPrice = Price
                    };
                    db.PurchaseDetailTables.Add(newitem);
                    db.SaveChanges();
                    ViewBag.Message = "Book Add Successfuly";
                }
            }
            else
            {
                ViewBag.Message = "Alredy Exist plz Check";
            }
            return RedirectToAction("NewPurchase");
        }
        public ActionResult DeletConfirm(int? id)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            var book = db.PurchaseDetailTables.Find(id);
            if (book != null)
            {
                db.Entry(book).State = System.Data.Entity.EntityState.Deleted;
                db.SaveChanges();
                ViewBag.Message = "Deleted Successfully";
                return RedirectToAction("NewPurchase");
            }
            ViewBag.Message = "Some unexpected Issue Occoured";
            return View("NewPurchase");
        }
        [HttpPost]
        public ActionResult GetBooks()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["UserID"])))
            {
                return RedirectToAction("Login", "Home");

            }
            List<BookMV> List = new List<BookMV>();
            var booklist = db.BookTables.ToList();
            foreach (var item in booklist)
            {
                List.Add(new BookMV { BookName = item.BookName, BookID = item.BookID });

            }
            return Json(new { data = List }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CancelPurchase()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["UserID"])))
            {
                return RedirectToAction("Login", "Home");

            }
            var list = db.PurchaseDetailTables.ToList();
            bool Cancelstatus = false;
            foreach (var item in list)
            {
                db.Entry(item).State = System.Data.Entity.EntityState.Deleted;
                int noofrecords = db.SaveChanges();
                if (Cancelstatus == false)
                {
                    if (noofrecords > 0)
                    {
                        Cancelstatus = true;
                    }
                }
            }
            if (Cancelstatus == true)
            {
                ViewBag.Message = "Purchase is Canceled";
                return RedirectToAction("NewPurchase");
            }
            ViewBag.Message = "Some Unexpected issue occure, please cotact to concern person!";
            return RedirectToAction("NewPurchase");
        }

        public ActionResult SelectSuplier()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["UserID"])))
            {
                return RedirectToAction("Login", "Home");
            }
             var purchasedetails = db.PurTemDetailTables.ToList();
            if (purchasedetails.Count == 0)
            {
                ViewBag.Message = "Purchase is Canceled";
                return RedirectToAction("NewPurchase");
            }
            var suplier = db.SuplierTables.ToList();

            return View(suplier);
        }
        [HttpPost]
        public ActionResult PurchaseConfirm(FormCollection collection)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["UserID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            int userid = Convert.ToInt32(Convert.ToString(Session["userid"]));
            int supplierid = 0;
            String[] keys = collection.AllKeys;
            foreach (var name in keys)
            {
                if (name.Contains("name"))
                {
                    string idname = name;
                    string[] vlaueids = idname.Split(' ');
                    supplierid = Convert.ToInt32(vlaueids[1]);
                }
            }
            var purchasedetails = db.PurchaseDetailTables.ToList();
            double totalamount = 0;
            foreach (var item in purchasedetails)
            {
                totalamount = totalamount + (item.Qty * item.UnitPrice);

            }
            if (totalamount == 0)
            {
                ViewBag.Message = "Purchase Cart Empty !";
                return View("Purchase");
            }
            var purchaseheader = new PurchaseTable();
            purchaseheader.SuplierID = supplierid;
            purchaseheader.PurchaseDate = DateTime.Now;
            purchaseheader.PurchaseAmount = totalamount;
            purchaseheader.UserID = userid;
            db.PurchaseTables.Add(purchaseheader);
            db.SaveChanges();
            foreach(var item in purchasedetails)
            {
                var purdetails = new PurchaseDetailTable() {
                    BookID = item.BookID,
                    PurchaseID = purchaseheader.purchaseID,
                    Qty = item.Qty,
                    UnitPrice=item.UnitPrice
                };
                db.PurchaseDetailTables.Add(purdetails);
                db.SaveChanges();
                var updatebookstock = db.BookTables.Find(item.BookID);
                updatebookstock.NoOfCopies = updatebookstock.NoOfCopies + item.Qty;
                updatebookstock.Price = item.UnitPrice;
                db.Entry(updatebookstock).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

            }
            db.PurTemDetailTables.ToList().ForEach(x =>
            {
                db.Entry(x).State = System.Data.Entity.EntityState.Deleted;
            });
            db.SaveChanges();
            ViewBag.Message = "Purchase Confirm Successfully !";
            return RedirectToAction("Allpurchases");
        }
        public ActionResult Allpurchases()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["UserID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            var list = db.PurchaseTables.ToList();
            return View(list);

        }
    public ActionResult PurchaseDetailView(int?id)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["UserID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var purchaseDetails = db.PurchaseDetailTables.Where(b=>b.PurchaseID==id);
            if (purchaseDetails == null)
            {
                return HttpNotFound();
            }
            return View(purchaseDetails);
        }


    }
}