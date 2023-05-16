using Microsoft.Xrm.Sdk;
using System;

namespace EmployeeAccountPlugin.LIB.Entities
{
    public class Account
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public OptionSetValue RelationshipType { get; set; }
    }
}