using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FL.DbBulk.Test.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OfficeOpenXml;
using Xunit;
using Xunit.Abstractions;

namespace FL.DbBulk.Test
{
    /**
     *批量导入  速度慢
     * 先在本地导入
     * 本地到处excel
     * 代码对应字段
     * 分批导入  1w 3s
     *
     */


    public class UnitTest1
    {
        private readonly ISqlBulk _bulk;
        
        private readonly ITestOutputHelper _output;

        public UnitTest1(ITestOutputHelper output)
        {
            _output = output;
            {
                var services = new ServiceCollection();
                // services.AddDbContext<MyDbContext>(opt =>
                //         opt.UseSqlServer(
                //             "Data Source =DESKTOP-M8G4R5A\\SQLEXPRESS; Initial Catalog = Furion; User Id = user; Password = 123456;"))
                //     .AddBatchDB<MyDbContext>();
                
                services.AddDbContext<MyDbContext>(opt =>
                        opt.UseSqlServer(
                            "Data Source =******"))
                    .AddBatchDB<MyDbContext>();
                _bulk = services.BuildServiceProvider().GetService<ISqlBulk>();
            }
        }
        
        [Fact]
        public async Task Test1()
        {
            // var blogs = new List<Blog>();
            // for (int i = 0; i < 100000; i++)
            // {
            //     blogs.Add(new Blog { CreateDate = DateTime.Now, Title = "aaa" + i });
            // }
            // var sw = Stopwatch.StartNew();
            // await _bulk.InsertAsync(blogs);
            // sw.Stop();
            // _output.WriteLine(sw.ElapsedMilliseconds.ToString());
        }

        
        [Fact]
        public void Test2()
        {
            var sw1 = Stopwatch.StartNew();
            GetExcelData();
            sw1.Stop();
            var time1 = sw1.ElapsedMilliseconds.ToString();
            Debugger.Break();
            var sw2 = Stopwatch.StartNew();

            _bulk.Insert(Data);

            sw2.Stop();
            var time2 = sw2.ElapsedMilliseconds.ToString();
            Debugger.Break();
        }

        private string Path = @"C:\Users\12731\Desktop\MI_Catalog.xlsx";
        private int StartRow = 2;
        private int EndRow = 100000;
        // private int EndRow = 269161;
        private List<MiCatalog> Data = new List<MiCatalog>();

        [Fact]
        public void Test3()
        {
            GetExcelData();
            Debugger.Break();
        }

        public void GetExcelData()
        {
            using (var excel = new ExcelPackage(new FileInfo(Path)))
            {
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                var sheet = excel.Workbook.Worksheets[0];
                if (sheet == null) return;

                for (int i = StartRow; i <= EndRow; i++)
                {
                    decimal? price = null;
                    int? isYbFlag = null;

                    if (decimal.TryParse(sheet.GetValue(i, 13)?.ToString(), out var Price))
                    {
                        price = Price;
                    }

                    if (int.TryParse(sheet.GetValue(i, 22)?.ToString(), out var IsYBFlag))
                    {
                        isYbFlag = IsYBFlag;
                    }

                    var item = new MiCatalog
                    {
                        MIID = 1,
                        WorkID = 1,
                        ItemType = 10,
                        ItemID = 0,
                        ItemCode = sheet.GetValue(i, 6)?.ToString(),
                        ItemName = sheet.GetValue(i, 7)?.ToString(),
                        EItemName = sheet.GetValue(i, 8)?.ToString(),
                        Unit = sheet.GetValue(i, 9)?.ToString(),
                        Dosage = sheet.GetValue(i, 10)?.ToString(),
                        Spec = sheet.GetValue(i, 11)?.ToString(),
                        Factory = sheet.GetValue(i, 12)?.ToString(),
                        Price = price,
                        PYCode = sheet.GetValue(i, 14)?.ToString(),
                        WBCode = sheet.GetValue(i, 15)?.ToString(),
                        IsControlPrice = sheet.GetValue(i, 16)?.ToString(),
                        MaxPrice = sheet.GetValue(i, 17)?.ToString(),
                        Range = sheet.GetValue(i, 18)?.ToString(),
                        Ratio = sheet.GetValue(i, 19)?.ToString(),
                        MatchState = sheet.GetValue(i, 20)?.ToString(),
                        ItemLevel = sheet.GetValue(i, 21)?.ToString(),
                        IsYBFlag = isYbFlag,
                        NationalDrugName = sheet.GetValue(i, 23)?.ToString(),
                        IsHZYL = sheet.GetValue(i, 24)?.ToString(),
                        IsJMYB = sheet.GetValue(i, 25)?.ToString(),
                        IsZGYB = sheet.GetValue(i, 26)?.ToString(),
                        UNIQUE_ID = sheet.GetValue(i, 27)?.ToString(),
                        StateItemCode = sheet.GetValue(i, 28)?.ToString(),
                        StateItemName = sheet.GetValue(i, 29)?.ToString(),
                        ProvinceItemCode = sheet.GetValue(i, 30)?.ToString(),
                        ProvinceItemName = sheet.GetValue(i, 31)?.ToString(),
                        StandardCode = sheet.GetValue(i, 32)?.ToString(),
                        SFDA = sheet.GetValue(i, 33)?.ToString(),
                        Remark = sheet.GetValue(i, 34)?.ToString(),
                        InterfaceTypeId = sheet.GetValue(i, 35)?.ToString(),
                    };

                    Data.Add(item);
                }
            }
        }
    }
}
