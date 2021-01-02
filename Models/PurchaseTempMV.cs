using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LibraryManagmentSystem.Models
{
    public class PurchaseTempMV
    {

        public int PurTempID { get; set; }
    
        public int BookID { get; set; }
    
        public int Quty { get; set; }
   
        public double UnitPrice { get; set; }
    }
}