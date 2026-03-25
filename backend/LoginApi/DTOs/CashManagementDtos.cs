using System.ComponentModel.DataAnnotations;

namespace LoginApi.DTOs
{
    // Cassa Intelligente DTOs
    public class CassaInteligenteDto
    {
        public int Id { get; set; }
        public int IdStore { get; set; }
        public string? StoreAlias { get; set; }
        public DateTime? CountingDate { get; set; }
        public string? SecurityEnvelopeCode { get; set; }
        public decimal? CountingAmount { get; set; }
        public decimal? CountingDifference { get; set; }
        public decimal? CountingCoins { get; set; }
        public DateTime? WithdrawalDate { get; set; }
        public DateTime? CountingCourierDate { get; set; }
        public DateTime? ImportDate { get; set; }
        public DateTime? CountingAt { get; set; }
        public string? CountingBy { get; set; }
    }

    public class CreateCassaInteligenteDto
    {
        [Required(ErrorMessage = "IdStore is required")]
        [Range(1, int.MaxValue, ErrorMessage = "IdStore must be a positive integer")]
        public int IdStore { get; set; }

        [StringLength(100)]
        public string? StoreAlias { get; set; }

        public DateTime? CountingDate { get; set; }

        [StringLength(100)]
        public string? SecurityEnvelopeCode { get; set; }

        [Range(typeof(decimal), "0", "9999999.99", ErrorMessage = "CountingAmount must be between 0 and 9,999,999.99")]
        public decimal? CountingAmount { get; set; }

        [Range(typeof(decimal), "-9999999.99", "9999999.99", ErrorMessage = "CountingDifference must be between -9,999,999.99 and 9,999,999.99")]
        public decimal? CountingDifference { get; set; }

        [Range(typeof(decimal), "0", "9999999.99", ErrorMessage = "CountingCoins must be between 0 and 9,999,999.99")]
        public decimal? CountingCoins { get; set; }

        public DateTime? WithdrawalDate { get; set; }
    }

    public class UpdateCassaInteligenteDto
    {
        [StringLength(100)]
        public string? StoreAlias { get; set; }

        public DateTime? CountingDate { get; set; }

        [StringLength(100)]
        public string? SecurityEnvelopeCode { get; set; }

        [Range(typeof(decimal), "0", "9999999.99", ErrorMessage = "CountingAmount must be between 0 and 9,999,999.99")]
        public decimal? CountingAmount { get; set; }

        [Range(typeof(decimal), "-9999999.99", "9999999.99", ErrorMessage = "CountingDifference must be between -9,999,999.99 and 9,999,999.99")]
        public decimal? CountingDifference { get; set; }

        [Range(typeof(decimal), "0", "9999999.99", ErrorMessage = "CountingCoins must be between 0 and 9,999,999.99")]
        public decimal? CountingCoins { get; set; }

        public DateTime? WithdrawalDate { get; set; }
    }

    // Fondo Spese DTOs
    public class FondoSpeseDto
    {
        public int Id { get; set; }
        public int IdStore { get; set; }
        public DateTime? CountingDate { get; set; }
        public string? ExpenseType { get; set; }
        public decimal? CountingAmount { get; set; }
        public decimal? CountingCoins { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string? InvoiceNumber { get; set; }
        public string? ReasonExpenses { get; set; }
        public DateTime? CountingAt { get; set; }
        public string? CountingBy { get; set; }
    }

    public class CreateFondoSpeseDto
    {
        [Required(ErrorMessage = "IdStore is required")]
        [Range(1, int.MaxValue, ErrorMessage = "IdStore must be a positive integer")]
        public int IdStore { get; set; }

        public DateTime? CountingDate { get; set; }

        [StringLength(100)]
        public string? ExpenseType { get; set; }

        [Range(typeof(decimal), "0", "9999999.99", ErrorMessage = "CountingAmount must be between 0 and 9,999,999.99")]
        public decimal? CountingAmount { get; set; }

        [Range(typeof(decimal), "0", "9999999.99", ErrorMessage = "CountingCoins must be between 0 and 9,999,999.99")]
        public decimal? CountingCoins { get; set; }

        public DateTime? InvoiceDate { get; set; }

        [StringLength(100)]
        public string? InvoiceNumber { get; set; }

        [StringLength(500)]
        public string? ReasonExpenses { get; set; }
    }

    public class UpdateFondoSpeseDto
    {
        public DateTime? CountingDate { get; set; }

        [StringLength(100)]
        public string? ExpenseType { get; set; }

        [Range(typeof(decimal), "0", "9999999.99", ErrorMessage = "CountingAmount must be between 0 and 9,999,999.99")]
        public decimal? CountingAmount { get; set; }

        [Range(typeof(decimal), "0", "9999999.99", ErrorMessage = "CountingCoins must be between 0 and 9,999,999.99")]
        public decimal? CountingCoins { get; set; }

        public DateTime? InvoiceDate { get; set; }

        [StringLength(100)]
        public string? InvoiceNumber { get; set; }

        [StringLength(500)]
        public string? ReasonExpenses { get; set; }
    }

    // Fondo Cassa DTOs
    public class FondoCassaDto
    {
        public int Id { get; set; }
        public int IdStore { get; set; }
        public string? CashCode { get; set; }
        public DateTime? CountingDate { get; set; }
        public decimal? CountingAmount { get; set; }
        public decimal? CountingCoins { get; set; }
        public DateTime? CountingAt { get; set; }
        public string? CountingBy { get; set; }
    }

    public class CreateFondoCassaDto
    {
        [Required(ErrorMessage = "IdStore is required")]
        [Range(1, int.MaxValue, ErrorMessage = "IdStore must be a positive integer")]
        public int IdStore { get; set; }

        [StringLength(50)]
        public string? CashCode { get; set; }

        public DateTime? CountingDate { get; set; }

        [Range(typeof(decimal), "0", "9999999.99", ErrorMessage = "CountingAmount must be between 0 and 9,999,999.99")]
        public decimal? CountingAmount { get; set; }

        [Range(typeof(decimal), "0", "9999999.99", ErrorMessage = "CountingCoins must be between 0 and 9,999,999.99")]
        public decimal? CountingCoins { get; set; }
    }

    public class UpdateFondoCassaDto
    {
        [StringLength(50)]
        public string? CashCode { get; set; }

        public DateTime? CountingDate { get; set; }

        [Range(typeof(decimal), "0", "9999999.99", ErrorMessage = "CountingAmount must be between 0 and 9,999,999.99")]
        public decimal? CountingAmount { get; set; }

        [Range(typeof(decimal), "0", "9999999.99", ErrorMessage = "CountingCoins must be between 0 and 9,999,999.99")]
        public decimal? CountingCoins { get; set; }
    }

    // Sovv. Monetaria DTOs
    public class SovvMonetariaDto
    {
        public int Id { get; set; }
        public int IdStore { get; set; }
        public DateTime? CountingDate { get; set; }
        public decimal? CountingAmount { get; set; }
        public string? CountingNote { get; set; }
        public decimal? CountingCoins { get; set; }
        public DateTime? CountingAt { get; set; }
        public string? CountingBy { get; set; }
    }

    public class CreateSovvMonetariaDto
    {
        [Required(ErrorMessage = "IdStore is required")]
        [Range(1, int.MaxValue, ErrorMessage = "IdStore must be a positive integer")]
        public int IdStore { get; set; }

        public DateTime? CountingDate { get; set; }

        [Range(typeof(decimal), "0", "9999999.99", ErrorMessage = "CountingAmount must be between 0 and 9,999,999.99")]
        public decimal? CountingAmount { get; set; }

        [StringLength(500)]
        public string? CountingNote { get; set; }

        [Range(typeof(decimal), "0", "9999999.99", ErrorMessage = "CountingCoins must be between 0 and 9,999,999.99")]
        public decimal? CountingCoins { get; set; }
    }

    public class UpdateSovvMonetariaDto
    {
        public DateTime? CountingDate { get; set; }

        [Range(typeof(decimal), "0", "9999999.99", ErrorMessage = "CountingAmount must be between 0 and 9,999,999.99")]
        public decimal? CountingAmount { get; set; }

        [StringLength(500)]
        public string? CountingNote { get; set; }

        [Range(typeof(decimal), "0", "9999999.99", ErrorMessage = "CountingCoins must be between 0 and 9,999,999.99")]
        public decimal? CountingCoins { get; set; }
    }
}
