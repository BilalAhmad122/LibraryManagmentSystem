using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LibraryManagmentSystem.Models
{
    public class BookMV
    {
        public int BookID { get; set; }
        public int UserID { get; set; }
        public int DepartmentID { get; set; }
        public int BookTypeID { get; set; }
        public string BookType { get; set; }
        public string BookName { get; set; }
        public string ShotDescription { get; set; }
        public string Auther { get; set; }
        public string BookTitle { get; set; }
        public double Adition { get; set; }
        public int NoOfCopies { get; set; }
        public System.DateTime RegDate { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }

    }
}