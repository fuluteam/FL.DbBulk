using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FL.DbBulk
{
    public class SqlBulk : ISqlBulk
    {
        private ISqlBulk _bulk;
        public SqlBulk(DbContext context, IServiceProvider provider)
        {
            if (context.Database.IsMySql())
            {
                _bulk = provider.GetService<IMysqlBulk>();
            }
            else if (context.Database.IsSqlServer())
            {
                _bulk = provider.GetService<ISqlServerBulk>();
            }
        }

        public void Insert(DataTable table)
        {
            _bulk.Insert(table);
        }

        public async Task InsertAsync(DataTable table)
        {
            await _bulk.InsertAsync(table);
        }

        public void Insert<T>(IEnumerable<T> enumerable) where T : class
        {
            _bulk.Insert(enumerable);
        }

        public async Task InsertAsync<T>(IEnumerable<T> enumerable) where T : class
        {
            await _bulk.InsertAsync(enumerable);
        }

    }
}