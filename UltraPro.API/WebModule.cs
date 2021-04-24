using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraPro.Services.Implements;
using UltraPro.Services.Interfaces;

namespace UltraPro.API
{
    public class WebModule : Module
    {
        private readonly string _connectionString;
        private readonly string _migrationAssemblyName;

        public WebModule(string connectionString, string migrationAssemblyName)
        {
            _connectionString = connectionString;
            _migrationAssemblyName = migrationAssemblyName;
        }

        protected override void Load(ContainerBuilder builder)
        {
            //builder.RegisterType<ApplicationUserService>().As<IApplicationUserService>()
            //       .InstancePerLifetimeScope();
            //builder.RegisterType<ApplicationRoleService>().As<IApplicationRoleService>()
            //       .InstancePerLifetimeScope();

            base.Load(builder);
        }
    }
}  
