using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoginApi.Models
{
    [Table("ExpenseFund")]
    public class ExpenseFund
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column("id_store")]
        public int IdStore { get; set; }

        [Column("expense_type")]
        [MaxLength(100)]
        public string? ExpenseType { get; set; }

        [Column("counting_amount", TypeName = "decimal(10,2)")]
        public decimal? CountingAmount { get; set; }

        [Column("counting_coins", TypeName = "decimal(10,2)")]
        public decimal? CountingCoins { get; set; }

        [Column("invoice_date")]
        public DateTime? InvoiceDate { get; set; }

        [Column("invoice_number")]
        [MaxLength(100)]
        public string? InvoiceNumber { get; set; }

        [Column("reason_expenses")]
        [MaxLength(500)]
        public string? ReasonExpenses { get; set; }

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

        [Column("tipo")]
        [MaxLength(50)]
        public string? Tipo { get; set; }

        [Column("fornitore")]
        [MaxLength(255)]
        public string? Fornitore { get; set; }

        [Column("fattura")]
        [MaxLength(100)]
        public string? Fattura { get; set; }

        [Column("totale", TypeName = "decimal(10,2)")]
        public decimal? Totale { get; set; }

        [Column("note")]
        [MaxLength(500)]
        public string? Note { get; set; }

        [Column("user")]
        [MaxLength(100)]
        public string? User { get; set; }
    }
}
