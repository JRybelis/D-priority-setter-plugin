using System;
using Microsoft.Xrm.Sdk;

namespace EmployeeAccountPlugin.LIB.Entities
{
    public class EmployeeAccount
    {
        public Guid Jok_EmployeeAccountId { get; set; }
        public string Jok_Name { get; set; }
        public OptionSetValue Jok_Priority { get; set; }
        public EntityReference Jok_LookAccountUp { get; set; }


        // standard metadata fields
        public DateTime CreatedOn { get; set; }
        public EntityReference CreatedBy { get; set; }
        public EntityReference CreatedOnBehalfBy { get; set; }
        public EntityReference ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public EntityReference ModifiedOnBehalfBy { get; set; }
        public Guid Organizationid { get; set; }
        public DateTime OverridenCreatedOn { get; set; }
        public int StateCode { get; set; }
        public int StatusCode { get; set; }
        public int TimeZoneRuleVersionNumber { get; set; }
        public int UtcConversionTimeZoneCode { get; set; }
        public long VersionNumber { get; set; }

    }
}
