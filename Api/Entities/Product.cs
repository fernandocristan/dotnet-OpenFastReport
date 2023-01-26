namespace Api.Entities
{
    public class Product
    {
        public string Description { get; set; }
        public string EAN13 { get; set; }
        public string Installments { get; set; }
        public decimal? Price { get; set; }
        public string Reference { get; set; }
    }
}