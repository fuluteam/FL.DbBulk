using System;
using FL.DbBulk;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddBatchDB<T>(this IServiceCollection services) where  T:DbContext
        {
            services.TryAddScoped<IMysqlBulk, MysqlBulk>();
            services.TryAddScoped<ISqlServerBulk, SqlServerBulk>();
            services.TryAddScoped<ISqlBulk, SqlBulk>();
            services.AddScoped<DbContext, T>();
            return services;
        }
    }
}