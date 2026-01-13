namespace LoginApi.DTOs
{
    public class BustaDto
    {
        public int Id { get; set; }
        public DateTime DataRiferimento { get; set; }
        public DateTime? DataChiusura { get; set; }
        public DateTime? DataRitiro { get; set; }
        public string? Sigillo { get; set; }
        public decimal Totale { get; set; }
        public string? Note { get; set; }
        public string? UserChiusura { get; set; }
        public string? UserRitiro { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int UserId { get; set; }
    }

    public class CreateBustaDto
    {
        public DateTime DataRiferimento { get; set; }
        public DateTime? DataChiusura { get; set; }
        public DateTime? DataRitiro { get; set; }
        public string? Sigillo { get; set; }
        public decimal Totale { get; set; }
        public string? Note { get; set; }
        public string? UserChiusura { get; set; }
        public string? UserRitiro { get; set; }
    }

    public class UpdateBustaDto
    {
        public DateTime? DataRiferimento { get; set; }
        public DateTime? DataChiusura { get; set; }
        public DateTime? DataRitiro { get; set; }
        public string? Sigillo { get; set; }
        public decimal? Totale { get; set; }
        public string? Note { get; set; }
        public string? UserChiusura { get; set; }
        public string? UserRitiro { get; set; }
    }
}
