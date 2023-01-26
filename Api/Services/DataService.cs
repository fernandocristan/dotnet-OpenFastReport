using Api.Entities;

using Bogus;

namespace Api.Services
{
    public static class DataService
    {
        public static IEnumerable<Product> GetProducts(int? quantity)
        {
            return new Faker<Product>()
                .StrictMode(true)
                .RuleFor(x => x.Description, f => f.Commerce.ProductName())
                .RuleFor(x => x.EAN13, f => f.Commerce.Ean13())
                .RuleFor(x => x.Price, f => Convert.ToDecimal(f.Commerce.Price(20, 99999, 2)))
                .RuleFor(x => x.Reference, f => f.Finance.Account())
                .RuleFor(x => x.Installments, (f, p) =>
                {
                    int quantity = f.Random.Int(1, 5);
                    decimal? installmentValue = p.Price / quantity;
                    return $"{quantity}x {Decimal.Round(installmentValue.GetValueOrDefault(), 2)}";
                })
                .Generate(quantity.GetValueOrDefault() > 0 ? quantity.GetValueOrDefault() : 1);

            //return new List<Product>
            //{
            //    new Product
            //    {
            //        Description = "Óculos Prada Eyewear Collection",
            //        EAN13 = "0106947719224",
            //        Price = 2356.5m,
            //        Reference = "7Q98DD"
            //    },
            //    new Product
            //    {
            //        Description = "Óculos de Sol CARTIER 0229S 002",
            //        EAN13 = "7890003269871",
            //        Price = 1999.5m,
            //        Reference = "13698742556"
            //    },
            //    new Product
            //    {
            //        Description = "Óculos de sol GUCCI preto injetado",
            //        EAN13 = "7890006895423",
            //        Price = 2379,
            //        Reference = "GG0810S-001 53"
            //    },
            //    new Product
            //    {
            //        Description = "Metal frame 02 sunglasses in metal with mineral glass lenses gold/green",
            //        EAN13 = "7896016541236",
            //        Price = 3500,
            //        Reference = "AVD3-FGA"
            //    },
            //};
        }

        public static IEnumerable<Product> GetProducts(int? quantity, int? jumpFirstLabelsInPaper)
        {
            if (jumpFirstLabelsInPaper == 0)
                return GetProducts(quantity);

            List<Product> products = GetProducts(quantity).ToList();
            for (int i = 0; i < jumpFirstLabelsInPaper; i++)
            {
                products.Insert(i, new Product()
                {
                    Description = null,
                    EAN13 = null,
                    Price = null,
                    Reference = null
                });
            }

            return products;
        }
    }
}