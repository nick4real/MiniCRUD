using Bogus;
using Microsoft.EntityFrameworkCore;
using MiniCRUD.Domain.Models;

namespace MiniCRUD.API.Infrastructure
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
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
                        .RuleFor(p => p.AvatarUrl, f => f.Internet.Avatar());

                    var users = user_f.Generate(10);

                    await context.Set<User>().AddRangeAsync(users, ct);
                    await context.SaveChangesAsync(ct);
                }
            });
        }
    }
}
