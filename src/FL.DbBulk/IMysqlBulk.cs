using System.Threading.Tasks;

namespace FL.DbBulk
{
    public interface IMysqlBulk : ISqlBulk
    {
        Task InsertAsync<T>(string csvPath, string tableName = "") where T : class;
    }
}