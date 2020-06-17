using System;
using System.Collections.Generic;

namespace Task1
{
    public partial class Persons
    {
        public Persons()
        {
            Agencyes = new HashSet<Agencyes>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int RoleId { get; set; }

        public virtual Roles Role { get; set; }
        public virtual ICollection<Agencyes> Agencyes { get; set; }
    }
}
