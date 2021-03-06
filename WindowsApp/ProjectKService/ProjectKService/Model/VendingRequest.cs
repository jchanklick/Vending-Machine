//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ProjectKService.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class VendingRequest
    {
        public long VendingRequestID { get; set; }
        public long CardScanResultID { get; set; }
        public System.DateTime RequestDate { get; set; }
        public string Coordinates { get; set; }
        public Nullable<int> X { get; set; }
        public Nullable<int> Y { get; set; }
        public string Status { get; set; }
        public Nullable<System.DateTime> VendStartDate { get; set; }
        public Nullable<System.DateTime> VendEndDate { get; set; }
        public string ErrorMessage { get; set; }
    
        public virtual CardScanResult CardScanResult { get; set; }
    }
}
