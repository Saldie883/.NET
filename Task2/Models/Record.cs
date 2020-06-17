using System;
using System.Collections.Generic;
using System.Text;

namespace Task2.Models
{
    public class Record
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int ClientId { get; set; }
        public int CarId { get; set; }
        public bool IsReturnedInTime { get; set; }
        public bool IsReturned { get; set; }
        public double Pre_Price { get; set; }
        public double BillPrice { get; set; }
    }
}