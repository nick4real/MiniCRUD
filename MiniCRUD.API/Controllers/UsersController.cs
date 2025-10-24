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
        var execStrategy = _dbContext.Database.CreateExecutionStrategy();
        return await execStrategy.ExecuteAsync<IActionResult>(async () =>
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(ct);
            try
            {
                var users = await _dbContext.Users
                    .AsNoTracking()
                    .ToListAsync(ct);

                if (users == null || !users.Any())
                {
                    await transaction.RollbackAsync(ct);
                    return NotFound(null);
                }

                await transaction.CommitAsync(ct);
                return Ok(users);
            }
            catch
            {
                await transaction.RollbackAsync(ct);
                throw;
            }
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var executionStrategy = _dbContext.Database.CreateExecutionStrategy();
        return await executionStrategy.ExecuteAsync<IActionResult>(async () =>
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(ct);
            try
            {
                var user = await _dbContext.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Id == id, ct);

                if (user == null)
                {
                    await transaction.RollbackAsync(ct);
                    return NotFound();
                }

                await transaction.CommitAsync(ct);
                return Ok(user);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync(ct);
                throw;
            }
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create(User user, CancellationToken ct)
    {
        var executionStrategy = _dbContext.Database.CreateExecutionStrategy();
        return await executionStrategy.ExecuteAsync<IActionResult>(async () =>
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(ct);
            try
            {
                await _dbContext.Users.AddAsync(user, ct);
                await _dbContext.SaveChangesAsync(ct);

                //TODO: validation?

                await transaction.CommitAsync(ct);

                return Ok();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync(ct);
                throw;
            }
        });

        
    }

    [HttpPut]
    public async Task<IActionResult> Update(User user, CancellationToken ct)
    {
        var executionStrategy = _dbContext.Database.CreateExecutionStrategy();
        return await executionStrategy.ExecuteAsync<IActionResult>(async () =>
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(ct);
            try
            {
                var found = await _dbContext.Users.FirstAsync(u => u.Id == user.Id, ct);
                if (found == null)
                {
                    await transaction.RollbackAsync(ct);
                    return NotFound();
                }

                found.Login = user.Login;
                found.PasswordHash = user.PasswordHash;
                found.AvatarUrl = user.AvatarUrl;

                await _dbContext.SaveChangesAsync(ct);
                await transaction.CommitAsync(ct);
                return Ok();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync(ct);
                throw;
            }
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var executionStrategy = _dbContext.Database.CreateExecutionStrategy();
        return await executionStrategy.ExecuteAsync<IActionResult>(async () =>
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(ct);
            try
            {
                var user = await _dbContext.Users.FirstAsync(u => u.Id == id, ct);
                if (user == null)
                {
                    await transaction.RollbackAsync(ct);
                    return NotFound();
                }

                _dbContext.Users.Remove(user);

                await _dbContext.SaveChangesAsync(ct);
                await transaction.CommitAsync(ct);
                return Ok();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync(ct);
                throw;
            }
        });
    }
}
