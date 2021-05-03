using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraPro.Repositories.Implements.AuditLogs;
using UltraPro.Repositories.Implements.RequestResponseLogs;
using UltraPro.Repositories.Implements.SingleValue;
using UltraPro.Repositories.Interfaces.AuditLogs;
using UltraPro.Repositories.Interfaces.RequestResponseLogs;
using UltraPro.Repositories.Interfaces.SingleValue;
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

            builder.RegisterType<SingleValueService>().As<ISingleValueService>()
                   .InstancePerLifetimeScope();
            builder.RegisterType<SingleValueTypeRepository>().As<ISingleValueTypeRepository>()
                   .InstancePerLifetimeScope();
            builder.RegisterType<SingleValueDetailRepository>().As<ISingleValueDetailRepository>()
                   .InstancePerLifetimeScope();
            builder.RegisterType<SingleValueUnitOfWork>().As<ISingleValueUnitOfWork>()
                   .InstancePerLifetimeScope();

            builder.RegisterType<RequestResponseLogService>().As<IRequestResponseLogService>()
                   .InstancePerLifetimeScope();
            builder.RegisterType<RequestResponseLogRepository>().As<IRequestResponseLogRepository>()
                   .InstancePerLifetimeScope();
            builder.RegisterType<RequestResponseLogUnitOfWork>().As<IRequestResponseLogUnitOfWork>()
                   .InstancePerLifetimeScope();

            builder.RegisterType<AuditLogService>().As<IAuditLogService>()
                   .InstancePerLifetimeScope();
            builder.RegisterType<AuditLogRepository>().As<IAuditLogRepository>()
                   .InstancePerLifetimeScope();
            builder.RegisterType<AuditLogUnitOfWork>().As<IAuditLogUnitOfWork>()
                   .InstancePerLifetimeScope();

            base.Load(builder);
        }
    }
}  
