using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FL.DbBulk.Test.Entity
{
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