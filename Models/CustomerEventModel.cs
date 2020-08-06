using System;
using System.Collections.Generic;

namespace DativeBackend.Models {
    public class CustomerEvent {
        public virtual ICollection<Customer> Customer { get; set; }
        public int EventType { get; set; }
        public string Data { get; set; }
    }
}
