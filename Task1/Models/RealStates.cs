using System;
using System.Collections.Generic;

namespace Task1
{
    public partial class RealStates
    {
        public int Id { get; set; }
        public string RegistrationNumber { get; set; }
        public string Address { get; set; }
        public int Price { get; set; }
        public byte[] Independency { get; set; }
        public int TypeId { get; set; }

        public virtual RealStateTypes Type { get; set; }
    }
}
