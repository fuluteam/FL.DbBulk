using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WeShare.PropertyWrapper;

namespace FL.DbBulk
{
    public static class Extension
    {
        /// <summary>
        /// 获取实体影射的表名
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetMappingName(this System.Type type)
        {
            var key = $"batch{type.FullName}";

            var tableName = CacheService.Get(key);
            if (string.IsNullOrEmpty(tableName))
            {
                var tableAttr = type.GetCustomAttribute<TableAttribute>();
                if (tableAttr != null)
                {
                    tableName = tableAttr.Name;
                }
                else
                {
                    tableName = type.Name;
                }
                CacheService.Add(key, tableName);
            }
            return tableName;
        }

        public static List<EntityInfo> GetMappingProperties(this System.Type type)
        {
            var key = $"ICH.King.DbBulk{type.Name}";
            var list = CacheService.Get<List<EntityInfo>>(key);
            if (list == null)
            {
                list = new List<EntityInfo>();
                foreach (var propertyInfo in type.GetProperties())
                {
                    if (!propertyInfo.PropertyType.IsValueType &&
                        propertyInfo.PropertyType.Name != "Nullable`1" && propertyInfo.PropertyType != typeof(string)) continue;
                    var temp = new EntityInfo();
                    temp.PropertyInfo = propertyInfo;
                    temp.FieldName = propertyInfo.Name;
                    var attr = propertyInfo.GetCustomAttribute<ColumnAttribute>();
                    if (attr != null)
                    {
                        temp.FieldName = attr.Name;
                    }
                    temp.GetMethod = propertyInfo.CreateGetter();
                    list.Add(temp);
                }
                CacheService.Add(key, list);
            }

            return list;
        }

        /// <summary>
        /// 创建cvs字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        /// <param name="primaryKey"></param>
        /// <returns></returns>
        public static string CreateCsv<T>(this IEnumerable<T> entities, string primaryKey = "")
        {
            var sb = new StringBuilder();
            var properties = typeof(T).GetMappingProperties().ToArray();
            foreach (var entity in entities)
            {
                for (int i = 0; i < properties.Length; i++)
                {
                    var ele = properties[i];
                    if (i != 0) sb.Append(",");
                    var value = ele.Get(entity);
                    if (ele.PropertyInfo.PropertyType.Name == "Nullable`1")
                    {
                        if (ele.PropertyInfo.PropertyType.GenericTypeArguments[0] == typeof(DateTime))
                        {
                            if (value == null)
                            {
                                sb.Append("NULL");
                            }
                            else
                            {
                                sb.Append(Convert.ToDateTime(value).ToString("yyyy-MM-dd HH:mm:ss"));
                            }
                            continue;
                        }
                    }

                    if (ele.PropertyInfo.PropertyType == typeof(DateTime))
                    {
                        sb.Append(Convert.ToDateTime(value).ToString("yyyy-MM-dd HH:mm:ss"));
                        continue;
                    }
                    //如果是主键&&string类型，且值不为空
                    if (ele.FieldName == primaryKey && ele.PropertyInfo.PropertyType == typeof(string))
                    {
                        sb.Append(Guid.NewGuid().ToString());
                        continue;
                    }
                    if (value == null)
                    {
                        continue;
                    }
                    if (ele.PropertyInfo.PropertyType == typeof(string))
                    {
                        var vStr = value.ToString();
                        if (vStr.Contains("\""))
                        {
                            vStr = vStr.Replace("\"", "\"\"");
                        }
                        if (vStr.Contains(",") || vStr.Contains("\r\n") || vStr.Contains("\n"))
                        {
                            vStr = $"\"{vStr}\"";
                        }
                        sb.Append(vStr);
                    }
                    else sb.Append(value);
                }
                sb.Append(IsWin() ? "\r\n" : "\n");
                //sb.AppendLine();
            }

            return sb.ToString();
        }

        public static bool IsWin()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        }

        public static string CreateCsv(this DataTable table)
        {
            StringBuilder sb = new StringBuilder();
            DataColumn colum;
            foreach (DataRow row in table.Rows)
            {
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    colum = table.Columns[i];
                    if (i != 0) sb.Append(",");
                    if (colum.DataType == typeof(string))
                    {
                        var vStr = row[colum].ToString();
                        if (vStr.Contains("\""))
                        {
                            vStr = vStr.Replace("\"", "\"\"");
                        }
                        if (vStr.Contains(",") || vStr.Contains("\r\n") || vStr.Contains("\n"))
                        {
                            vStr = $"\"{vStr}\"";
                        }
                        sb.Append(vStr);
                    }
                    else sb.Append(row[colum]);
                }
                sb.Append(IsWin() ? "\r\n" : "\n");
            }
            return sb.ToString();
        }


        public static DataTable ToDataTable<T>(this IEnumerable<T> list, string primaryKey = "")
        {
            var type = typeof(T);
            //获取实体映射的表名
            var mappingName = type.GetMappingName();
            var dt = new DataTable(mappingName);
            //获取实体映射的属性列表
            var columns = type.GetMappingProperties();
            dt.Columns.AddRange(columns.Select(x => new DataColumn(x.FieldName)).ToArray());
            foreach (var data in list)
            {
                var row = dt.NewRow();
                foreach (var entityInfo in columns)
                {
                    var value = entityInfo.Get(data);
                    if (primaryKey == entityInfo.FieldName && entityInfo.PropertyInfo.PropertyType == typeof(string))
                    {
                        row[entityInfo.FieldName] = value ?? Guid.NewGuid().ToString();
                    }
                    else
                    {
                        row[entityInfo.FieldName] = value;
                    }
                }
                dt.Rows.Add(row);
            }

            return dt;
        }

       
    }
}