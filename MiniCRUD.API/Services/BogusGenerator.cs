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
                .RuleFor(u => u.AvatarUrl, f => f.Internet.Avatar());

            return user_f.Generate();
        }
    }
}
