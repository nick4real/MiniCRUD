using Bogus;
using Microsoft.EntityFrameworkCore;
using MiniCRUD.Domain.Models;

namespace MiniCRUD.API.Infrastructure
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseAsyncSeeding(async (context, _, ct) =>
            {
                if (!context.Set<User>().Any())
                {
                    var user_f = new Faker<User>()
                        .RuleFor(p => p.Id, f => Guid.NewGuid())
                        .RuleFor(p => p.Login, f => f.Internet.UserName())
                        .RuleFor(p => p.PasswordHash, f => f.Internet.Password())
                        .RuleFor(p => p.AvatarUrl, f => f.Internet.Avatar())
                        .RuleFor(u => u.Email, f => f.Internet.Email())
                        .RuleFor(u => u.LastKnownIP, f => f.Internet.Ip());

                    var users = user_f.Generate(20);

                    await context.Set<User>().AddRangeAsync(users, ct);
                    await context.SaveChangesAsync(ct);
                }

                if (!context.Set<Product>().Any())
                {
                    var product_f = new Faker<Product>()
                        .RuleFor(p => p.Id, f => Guid.NewGuid())
                        .RuleFor(p => p.Name, f => f.Commerce.ProductName())
                        .RuleFor(p => p.Description, f => f.Commerce.ProductDescription())
                        .RuleFor(p => p.Adjective, f => f.Commerce.ProductAdjective())
                        .RuleFor(p => p.Material, f => f.Commerce.ProductMaterial())
                        .RuleFor(p => p.Price, f => f.Random.Decimal(1, 1000));

                    var products = product_f.Generate(40);

                    await context.Set<Product>().AddRangeAsync(products, ct);
                    await context.SaveChangesAsync(ct);
                }
            });
        }
    }
}
