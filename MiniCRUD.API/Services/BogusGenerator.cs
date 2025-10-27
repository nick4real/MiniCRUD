using Bogus;
using MiniCRUD.Domain.Models;

namespace MiniCRUD.API.Services
{
    public static class BogusGenerator
    {
        public static User GenUser()
        {
            var user_f = new Faker<User>()
                .RuleFor(u => u.Id, f => Guid.NewGuid())
                .RuleFor(u => u.Login, f => f.Internet.UserName())
                .RuleFor(u => u.PasswordHash, f => f.Internet.Password())
                .RuleFor(u => u.AvatarUrl, f => f.Internet.Avatar())
                .RuleFor(u => u.Email, f => f.Internet.Email())
                .RuleFor(u => u.LastKnownIP, f => f.Internet.Ip());

            return user_f.Generate();
        }

        public static Product GenProduct()
        {
            var product_f = new Faker<Product>()
                .RuleFor(p => p.Id, f => Guid.NewGuid())
                .RuleFor(p => p.Name, f => f.Commerce.ProductName())
                .RuleFor(p => p.Description, f => f.Commerce.ProductDescription())
                .RuleFor(p => p.Adjective, f => f.Commerce.ProductAdjective())
                .RuleFor(p => p.Material, f => f.Commerce.ProductMaterial())
                .RuleFor(p => p.Price, f => f.Random.Decimal(1, 1000));

            return product_f.Generate();
        }
    }
}
