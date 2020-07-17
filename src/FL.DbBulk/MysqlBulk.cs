using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;

namespace FL.DbBulk
{
    public class MysqlBulk : IMysqlBulk
    {
        private DbContext _context;

        public MysqlBulk(DbContext context)
        {
            _context = context;
        }

        public void Insert(DataTable table)
        {
            var csv = table.CreateCsv();
            var columns = new List<string>();
            foreach (DataColumn tableColumn in table.Columns)
            {
                columns.Add(tableColumn.ColumnName);
            }
            InsertCsv(csv, table.TableName, columns);
            table.Clear();
            table.Dispose();
        }

        public async Task InsertAsync(DataTable table)
        {
            var csv = table.CreateCsv();
            var columns = new List<string>();
            foreach (DataColumn tableColumn in table.Columns)
            {
                columns.Add(tableColumn.ColumnName);
            }
            await InsertCsvAsync(csv, table.TableName, columns);
            table.Clear();
            table.Dispose();
        }

        public void Insert<T>(IEnumerable<T> enumerable) where T : class
        {
            var type = typeof(T);
            var tableName = type.GetMappingName();
            var properties = type.GetMappingProperties();
            //获取主键
            var keys = _context.Model.FindEntityType(type).FindPrimaryKey().Properties;
            var primaryKey = "";
            if (keys.Count == 1)
            {
                primaryKey = keys[0].PropertyInfo.Name;
            }
            InsertCsv(enumerable.CreateCsv(primaryKey), tableName, properties.Select(x => x.FieldName).ToList());
        }

        public async Task InsertAsync<T>(IEnumerable<T> enumerable) where T : class
        {
            var type = typeof(T);
            var tableName = type.GetMappingName();
            var properties = type.GetMappingProperties();
            //获取主键
            var keys = _context.Model.FindEntityType(type).FindPrimaryKey().Properties;
            var primaryKey = "";
            if (keys.Count == 1)
            {
                primaryKey = keys[0].PropertyInfo.Name;
            }
            await InsertCsvAsync(enumerable.CreateCsv(primaryKey), tableName, properties.Select(x => x.FieldName).ToList());
        }

        public async Task InsertAsync<T>(string csv, string tableName = "") where T : class
        {
            var type = typeof(T);
            if (string.IsNullOrEmpty(tableName))
                tableName = type.GetMappingName();

            var properties = type.GetMappingProperties();

            await InsertCsvAsync(csv, tableName, properties.Select(x => x.FieldName).ToList());
        }

        private void InsertCsv(string csv, string tableName, List<string> columns)
        {
            var fileName = Path.GetTempFileName();
            File.WriteAllText(fileName, csv);
            var conn = _context.Database.GetDbConnection() as MySqlConnection;
            var loader = new MySqlBulkLoader(conn)
            {
                FileName = fileName,
                Local = true,
                LineTerminator = Extension.IsWin() ? "\r\n" : "\n",
                FieldTerminator = ",",
                FieldQuotationCharacter = '"',
                EscapeCharacter = '"',
                TableName = tableName,
                CharacterSet = "UTF8"
            };
            loader.Columns.AddRange(columns);
            loader.Load();
        }
        private async Task InsertCsvAsync(string csv, string tableName, List<string> columns)
        {
            var fileName = Path.GetTempFileName();
            await File.WriteAllTextAsync(fileName, csv);
            var conn = _context.Database.GetDbConnection() as MySqlConnection;
            var loader = new MySqlBulkLoader(conn)
            {
                FileName = fileName,
                Local = true,
                LineTerminator = Extension.IsWin() ? "\r\n" : "\n",
                FieldTerminator = ",",
                TableName = tableName,
                FieldQuotationCharacter = '"',
                EscapeCharacter = '"',
                CharacterSet = "UTF8"
            };
            loader.Columns.AddRange(columns);
            await loader.LoadAsync();
        }
    }
}