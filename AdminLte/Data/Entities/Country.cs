namespace AdminLte.Data.Entities
{
    public class Country
    {
        public int Id { get; set; }
        public string ShortName { get; set; }
        public string Name { get; set; }
        public string? Iso3 { get; set; }
        public string? NumberCode { get; set; }
        public string? PhoneCode { get; set; }
        public string IsDefault { get; set; }
    }
}
