using System;
using System.Collections.Generic;

namespace Task1
{
    public partial class Operations
    {
        public Operations()
        {
            Invoices = new HashSet<Invoices>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Invoices> Invoices { get; set; }
    }
}
