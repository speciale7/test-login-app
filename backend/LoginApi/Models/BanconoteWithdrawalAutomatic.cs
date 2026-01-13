using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoginApi.Models
{
    [Table("BanconoteWithdrawalAutomatic")]
    public class BanconoteWithdrawalAutomatic
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column("id_store")]
        public int IdStore { get; set; }

        [Column("store_alias")]
        [MaxLength(100)]
        public string? StoreAlias { get; set; }

        [Column("security_envelope_code")]
        [MaxLength(100)]
        public string? SecurityEnvelopeCode { get; set; }

        [Column("counting_amount", TypeName = "decimal(10,2)")]
        public decimal? CountingAmount { get; set; }

        [Column("counting_difference", TypeName = "decimal(10,2)")]
        public decimal? CountingDifference { get; set; }

        [Column("counting_coins", TypeName = "decimal(10,2)")]
        public decimal? CountingCoins { get; set; }

        [Column("withdrawal_date")]
        public DateTime? WithdrawalDate { get; set; }

        [Column("counting_courier_date")]
        public DateTime? CountingCourierDate { get; set; }

        [Column("import_date")]
        public DateTime? ImportDate { get; set; }

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

        [Column("user")]
        [MaxLength(100)]
        public string? User { get; set; }

        [Column("totale", TypeName = "decimal(10,2)")]
        public decimal? Totale { get; set; }

        [Column("note")]
        [MaxLength(500)]
        public string? Note { get; set; }
    }
}
