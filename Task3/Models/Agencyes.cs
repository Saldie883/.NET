using System;
using System.Collections.Generic;

namespace Task1
{
    public partial class Agencyes
    {
        public Agencyes()
        {
            Brockers = new HashSet<Brockers>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public byte[] License { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public int DirectorId { get; set; }

        public virtual Persons Director { get; set; }
        public virtual ICollection<Brockers> Brockers { get; set; }
    }
}
