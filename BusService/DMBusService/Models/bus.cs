//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DMBusService.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class bus
    {
        public bus()
        {
            this.trips = new HashSet<trip>();
        }
    
        public int busId { get; set; }
        public int busNumber { get; set; }
        public string status { get; set; }
        public string comments { get; set; }
    
        public virtual ICollection<trip> trips { get; set; }
    }
}
