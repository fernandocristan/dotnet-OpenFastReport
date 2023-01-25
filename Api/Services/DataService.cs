using Api.Entities;

namespace Api.Services
{
    public static class DataService
    {
        public static IEnumerable<Product> GetProducts()
        {
            return new List<Product>
            {
                new Product
                {
                    Description = null,
                    EAN13 = null,
                    Price = null,
                    Reference = null
                },
                new Product
                {
                    Description = "Óculos Prada Eyewear Collection",
                    EAN13 = "0106947719224",
                    Price = 2356.5m,
                    Reference = "7Q98DD"
                },
                new Product
                {
                    Description = "Óculos de Sol CARTIER 0229S 002",
                    EAN13 = "7890003269871",
                    Price = 1999.5m,
                    Reference = "13698742556"
                },
                new Product
                {
                    Description = "Óculos de sol GUCCI preto injetado",
                    EAN13 = "7890006895423",
                    Price = 2379,
                    Reference = "GG0810S-001 53"
                },
                new Product
                {
                    Description = "Metal frame 02 sunglasses in metal with mineral glass lenses gold/green",
                    EAN13 = "7896016541236",
                    Price = 3500,
                    Reference = "AVD3-FGA"
                },
            };
        }

        public static IEnumerable<Product> GetProducts(int jumpFirstRecords)
        {
            List<Product> products = GetProducts().ToList();
            for (int i = 0; i < jumpFirstRecords; i++)
            {
                products.Insert(i, new Product());
            }

            return products;
        }
    }
}