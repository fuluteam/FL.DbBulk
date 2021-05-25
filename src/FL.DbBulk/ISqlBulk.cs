using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace FL.DbBulk
{
    public interface ISqlBulk
    {
        IEnumerable<T> Select<T>() where T : class;

        /// <summary>
        /// 批量导入数据
        /// </summary>
        /// <param name="table">数据源</param>
        void Insert(DataTable table);
        /// <summary>
        /// 批量导入数据
        /// </summary>
        /// <param name="table">数据源</param>
        Task InsertAsync(DataTable table);

        void Insert<T>(IEnumerable<T> enumerable) where T : class;
        Task InsertAsync<T>(IEnumerable<T> enumerable) where T : class;
    }
}