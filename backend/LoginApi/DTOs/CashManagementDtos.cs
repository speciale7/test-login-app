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
        public int IdStore { get; set; }
        public string? StoreAlias { get; set; }
        public DateTime? CountingDate { get; set; }
        public string? SecurityEnvelopeCode { get; set; }
        public decimal? CountingAmount { get; set; }
        public decimal? CountingDifference { get; set; }
        public decimal? CountingCoins { get; set; }
        public DateTime? WithdrawalDate { get; set; }
    }

    public class UpdateCassaInteligenteDto
    {
        public string? StoreAlias { get; set; }
        public DateTime? CountingDate { get; set; }
        public string? SecurityEnvelopeCode { get; set; }
        public decimal? CountingAmount { get; set; }
        public decimal? CountingDifference { get; set; }
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
        public int IdStore { get; set; }
        public DateTime? CountingDate { get; set; }
        public string? ExpenseType { get; set; }
        public decimal? CountingAmount { get; set; }
        public decimal? CountingCoins { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string? InvoiceNumber { get; set; }
        public string? ReasonExpenses { get; set; }
    }

    public class UpdateFondoSpeseDto
    {
        public DateTime? CountingDate { get; set; }
        public string? ExpenseType { get; set; }
        public decimal? CountingAmount { get; set; }
        public decimal? CountingCoins { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string? InvoiceNumber { get; set; }
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
        public int IdStore { get; set; }
        public string? CashCode { get; set; }
        public DateTime? CountingDate { get; set; }
        public decimal? CountingAmount { get; set; }
        public decimal? CountingCoins { get; set; }
    }

    public class UpdateFondoCassaDto
    {
        public string? CashCode { get; set; }
        public DateTime? CountingDate { get; set; }
        public decimal? CountingAmount { get; set; }
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
        public int IdStore { get; set; }
        public DateTime? CountingDate { get; set; }
        public decimal? CountingAmount { get; set; }
        public string? CountingNote { get; set; }
        public decimal? CountingCoins { get; set; }
    }

    public class UpdateSovvMonetariaDto
    {
        public DateTime? CountingDate { get; set; }
        public decimal? CountingAmount { get; set; }
        public string? CountingNote { get; set; }
        public decimal? CountingCoins { get; set; }
    }
}
