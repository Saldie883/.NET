using System;
using System.Collections.Generic;
using System.Text;

namespace Task3.Models
{
    internal class Car
    {
        public int Id { get; set; }
        public string Brand { get; set; }
        public int Price { get; set; }
        public CarType Type { get; set; }
    }
}