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
    
    public partial class routeSchedule
    {
        public routeSchedule()
        {
            this.trips = new HashSet<trip>();
        }
    
        public int routeScheduleId { get; set; }
        public string busRouteCode { get; set; }
        public System.TimeSpan startTime { get; set; }
        public bool isWeekDay { get; set; }
        public string comments { get; set; }
    
        public virtual busRoute busRoute { get; set; }
        public virtual ICollection<trip> trips { get; set; }
    }
}
