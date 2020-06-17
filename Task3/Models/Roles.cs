using System;
using System.Collections.Generic;

namespace Task1
{
    public partial class Roles
    {
        public Roles()
        {
            Persons = new HashSet<Persons>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Persons> Persons { get; set; }
    }
}
