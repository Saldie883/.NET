using System;
using System.Collections.Generic;

namespace Task1
{
    public partial class RealStateTypes
    {
        public RealStateTypes()
        {
            RealStates = new HashSet<RealStates>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<RealStates> RealStates { get; set; }
    }
}
