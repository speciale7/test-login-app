using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoginApi.Models
{
    public class Busta
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime DataRiferimento { get; set; }

        public DateTime? DataChiusura { get; set; }

        public DateTime? DataRitiro { get; set; }

        [StringLength(100)]
        public string? Sigillo { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Totale { get; set; }

        [StringLength(500)]
        public string? Note { get; set; }

        [StringLength(100)]
        public string? UserChiusura { get; set; }

        [StringLength(100)]
        public string? UserRitiro { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }
    }
}
