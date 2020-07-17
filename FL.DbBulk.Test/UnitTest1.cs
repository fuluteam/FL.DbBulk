using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace FL.DbBulk.Test
{
    public class UnitTest1
    {
        private readonly ISqlBulk _bulk;

        private readonly ITestOutputHelper _output;
        public UnitTest1(ITestOutputHelper output)
        {
            _output = output;
            var services = new ServiceCollection();
            services.AddDbContext<MyDbContext>(opt => opt.UseMySql("server=10.0.0.146;Database=demo;Uid=root;Pwd=123456;Port=3306;AllowLoadLocalInfile=true"))
                .AddBatchDB<MyDbContext>();
            _bulk = services.BuildServiceProvider().GetService<ISqlBulk>();
        }

        [Fact]
        public async Task Test1()
        {
            var blogs = new List<Blog>();
            for (int i = 0; i < 100000; i++)
            {
                blogs.Add(new Blog { CreateDate = DateTime.Now, Title = "aaa" + i });
            }
            var sw = Stopwatch.StartNew();
            await _bulk.InsertAsync(blogs);
            sw.Stop();
            _output.WriteLine(sw.ElapsedMilliseconds.ToString());
            
        }
    }
    [Table("blog")]
    public class Blog
    {
        [Column("id")]
        public long Id { get; set; }
        [Column("title")]
        public string Title { get; set; }
        [Column("create_date")]
        public DateTime? CreateDate { get; set; }
    }
}
