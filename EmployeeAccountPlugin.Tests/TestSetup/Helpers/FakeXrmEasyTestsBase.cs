using FakeXrmEasy.Abstractions;
using FakeXrmEasy.Abstractions.Enums;
using FakeXrmEasy.Middleware;
using FakeXrmEasy.Middleware.Crud;
using FakeXrmEasy.Plugins;
using Microsoft.Xrm.Sdk;

namespace EmployeeAccountPlugin.UnitTests.TestSetup.Helpers
{
    public class FakeXrmEasyTestsBase
    {
        protected readonly IXrmFakedContext context;
        protected readonly IOrganizationService service;
        protected readonly XrmFakedPluginExecutionContext pluginContext;

        public FakeXrmEasyTestsBase()
        {
            context = MiddlewareBuilder
                .New()
                .AddCrud()
                .UseCrud()
                .SetLicense(FakeXrmEasyLicense.NonCommercial)
                .Build();

            service = context.GetOrganizationService();
            pluginContext = context.GetDefaultPluginContext();
        }

    }
}