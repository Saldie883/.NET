using System;
using System.Collections.Generic;

namespace Task1
{
    public partial class Brockers
    {
        public Brockers()
        {
            Invoices = new HashSet<Invoices>();
        }

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public int AgencyId { get; set; }

        public virtual Agencyes Agency { get; set; }
        public virtual ICollection<Invoices> Invoices { get; set; }
    }
}
