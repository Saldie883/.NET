using System;
using System.Collections.Generic;

namespace Task1
{
    public partial class Invoices
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int OperationId { get; set; }
        public int BrockerId { get; set; }
        public int Bail { get; set; }
        public int ClientId { get; set; }
        public string RealStateAddress { get; set; }

        public virtual Brockers Brocker { get; set; }
        public virtual Operations Operation { get; set; }
    }
}
