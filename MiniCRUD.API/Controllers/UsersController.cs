using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniCRUD.API.Infrastructure;
using MiniCRUD.Domain.Models;

namespace MiniCRUD.API.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly ILogger<UsersController> _logger;
    private readonly AppDbContext _dbContext;

    public UsersController(ILogger<UsersController> logger, AppDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var users = await _dbContext.Users.ToListAsync(ct);
        if (users == null || !users.Any())
            return NotFound();

        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var user = await _dbContext.Users.FirstAsync(u => u.Id == id, ct);
        if (user == null)
            return NotFound();

        return Ok(user);
    }

    [HttpPost]
    public async Task<IActionResult> Create(User user, CancellationToken ct)
    {
        await _dbContext.Users.AddAsync(user, ct);
        await _dbContext.SaveChangesAsync(ct);
        return Ok();
    }

    [HttpPut]
    public async Task<IActionResult> Update(User user, CancellationToken ct)
    {
        var found = await _dbContext.Users.FirstAsync(u => u.Id == user.Id, ct);
        if (found == null)
            return NotFound();

        found.Login = user.Login;
        found.PasswordHash = user.PasswordHash;
        found.AvatarUrl = user.AvatarUrl;

        await _dbContext.SaveChangesAsync(ct);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var user = await _dbContext.Users.FirstAsync(u => u.Id == id, ct);
        if (user == null)
            return NotFound();

        _dbContext.Users.Remove(user);
        await _dbContext.SaveChangesAsync(ct);
        return Ok();
    }
}
