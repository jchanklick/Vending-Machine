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
    
    public partial class Error
    {
        public long ErrorID { get; set; }
        public string EntityName { get; set; }
        public Nullable<long> EntityID { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorStackTrace { get; set; }
        public string ChildErrorMessage { get; set; }
        public string ChildErrorStackTrace { get; set; }
        public System.DateTime Created { get; set; }
    }
}
