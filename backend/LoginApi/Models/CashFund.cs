using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoginApi.Models
{
    [Table("CashFund")]
    public class CashFund
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column("id_store")]
        public int IdStore { get; set; }

        [Column("cash_code")]
        [MaxLength(100)]
        public string? CashCode { get; set; }

        [Column("counting_amount", TypeName = "decimal(10,2)")]
        public decimal? CountingAmount { get; set; }

        [Column("counting_coins", TypeName = "decimal(10,2)")]
        public decimal? CountingCoins { get; set; }

        [Column("counting_at")]
        public DateTime? CountingAt { get; set; }

        [Column("counting_by")]
        [MaxLength(100)]
        public string? CountingBy { get; set; }

        [Column("counting_date")]
        public DateTime? CountingDate { get; set; }

        [Column("counting_time")]
        [MaxLength(10)]
        public string? CountingTime { get; set; }

        [Column("cassa")]
        [MaxLength(50)]
        public string? Cassa { get; set; }

        [Column("totale", TypeName = "decimal(10,2)")]
        public decimal? Totale { get; set; }

        [Column("user")]
        [MaxLength(100)]
        public string? User { get; set; }

        [Column("notes")]
        [MaxLength(500)]
        public string? Notes { get; set; }
    }
}
