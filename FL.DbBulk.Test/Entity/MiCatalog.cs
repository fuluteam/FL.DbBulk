using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FL.DbBulk.Test.Entity
{
    [Table("MI_Catalog")]
    public class MiCatalog
    {
        [Column("ID")] 
        public int Id { get; set; }

        [Column("MIID")] 
        public int MIID { get; set; }

        [Column("WorkID")] 
        public int WorkID { get; set; }
        
        public int ItemType { get; set; }
        public int ItemID { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string EItemName { get; set; }
        public string Unit { get; set; }
        public string Dosage { get; set; }
        public string Spec { get; set; }
        public string Factory { get; set; }
        public decimal? Price { get; set; }
        public string PYCode { get; set; }
        public string WBCode { get; set; }
        public string IsControlPrice { get; set; }
        public string MaxPrice { get; set; }
        public string Range { get; set; }
        public string Ratio { get; set; }
        public string MatchState { get; set; }
        public string ItemLevel { get; set; }
        public int? IsYBFlag { get; set; }
        public string NationalDrugName { get; set; }
        public string IsHZYL { get; set; }
        public string IsJMYB { get; set; }
        public string IsZGYB { get; set; }
        public string UNIQUE_ID { get; set; }
        public string StateItemCode { get; set; }
        public string StateItemName { get; set; }
        public string ProvinceItemCode { get; set; }
        public string ProvinceItemName { get; set; }
        public string StandardCode { get; set; }
        public string SFDA { get; set; }
        public string Remark { get; set; }
        public string InterfaceTypeId { get; set; }
    }
}