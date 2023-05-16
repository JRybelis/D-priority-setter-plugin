using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EmployeeAccountPlugin
{
    public class EmployeeAccountPrioritySetterPlugin : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            var context = serviceProvider.GetService(typeof(IPluginExecutionContext)) as IPluginExecutionContext;
            var service = serviceFactory.CreateOrganizationService(context.UserId);
            var tracingService = serviceProvider.GetService(typeof(ITracingService)) as ITracingService;


            // check if call is made in pre-operation
            if (context.Stage != 20)
                throw new InvalidPluginExecutionException("Must run as pre-operation stage 20");

            if (context.MessageName != "Create" && context.MessageName != "Update")
                throw new InvalidPluginExecutionException("Registered for " + context.MessageName + " only Create and Update are supported.");

            if (context.PrimaryEntityName != "jok_employeeaccount")
                throw new InvalidPluginExecutionException("Registered for " + context.PrimaryEntityName + " entity and only jok_employeeaccount is supported.");

            // Target is on the Create request and it's the entity that contains the employee account information
            var employeeAccount = context.InputParameters["Target"] as Entity;
            if (employeeAccount == null)
                throw new InvalidPluginExecutionException("The employee account is null");

            
            if (employeeAccount.Attributes.Contains("jok_priority") && employeeAccount.Attributes["jok_priority"] != null)
            {
                if (context.MessageName == "Update")
                {
                    tracingService.Trace("Update request. Check if Priority is receiving new value.");

                    var isPriorityFieldChanged = CheckPriorityComesInUpdated(employeeAccount, service);
                    
                    if (!isPriorityFieldChanged)
                        return;
                }

                tracingService.Trace("Calculate Priority level to assign.");

                var priority = CalculatePriority(employeeAccount, service);

                if (priority < 1 || priority > 5)
                    throw new InvalidPluginExecutionException("The Priority level is outside of the range of accepted values (between 1 and 5).");

                employeeAccount.Attributes["jok_priority"] = priority;
                service.Update(employeeAccount);
            }
        }

        private bool CheckPriorityComesInUpdated(Entity employeeAccount, IOrganizationService service)
        {
            var originalRecord = service.Retrieve("jok_employeeaccount", (Guid)employeeAccount.Attributes["jok_employeeaccountId"], new ColumnSet("jok_priority"));

            var incomingOptionSetValue = ((OptionSetValue)employeeAccount.Attributes["jok_priority"]).Value;
            var originalOptionSetValue = ((OptionSetValue)originalRecord.Attributes["jok_priority"]).Value;

            return incomingOptionSetValue != originalOptionSetValue;
        }

        private int CalculatePriority(Entity employeeAccount, IOrganizationService service)
        {
            var account = QueryAccount(employeeAccount, service);
            
            if (account == null)
                return 0;

            var employeeAccounts = QueryLinkedEmployeeAccounts(account, service);

            if (employeeAccounts.Count == 0)
                return 0;

            // priority 1 is higher than priority 2
            var highestEmployeeAccountsPriority = employeeAccounts.Select(entity => entity.Attributes)
                .Where(attributes => attributes.Contains("jok_priority"))
                .Select(attributes => ((OptionSetValue)attributes["jok_priority"]).Value).Min();

            return highestEmployeeAccountsPriority;
        }

        private Entity QueryAccount(Entity employeeAccount, IOrganizationService service)
        {
            if (!employeeAccount.Attributes.Contains("jok_lookaccountup"))
                return null;

            var linkedAccount = employeeAccount.Attributes["jok_lookaccountup"] as EntityReference;

            if (linkedAccount == null)
                return null;

            var query = new QueryExpression(linkedAccount.LogicalName);

            query.Criteria.AddCondition("accountid", ConditionOperator.Equal, linkedAccount.Id);
            
            return service.RetrieveMultiple(query).Entities.FirstOrDefault();
        }

        private List<Entity> QueryLinkedEmployeeAccounts(Entity account, IOrganizationService service)
        {
            var query = new QueryExpression("jok_employeeaccount");
            query.Criteria.AddCondition("jok_lookaccountup", ConditionOperator.Equal, account.Id);

            var employeeAccounts = service.RetrieveMultiple(query).Entities;
            
            return employeeAccounts.ToList();
        }
    }
}
