using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniCRUD.API.Infrastructure;
using MiniCRUD.API.Services;
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
                if (String.IsNullOrWhiteSpace(user.Login) 
                || String.IsNullOrWhiteSpace(user.PasswordHash)
                || String.IsNullOrWhiteSpace(user.Email)
                )
                    return BadRequest();

                var found = await _dbContext.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Id == user.Id, ct);
                if (found != null) return BadRequest();

                var found2 = await _dbContext.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Login == user.Login, ct);
                if (found2 != null) return BadRequest();

                var newUser = BogusGenerator.GenUser();
                newUser.Login = user.Login;
                newUser.PasswordHash = user.PasswordHash;
                newUser.Email = user.Email;

                await _dbContext.Users.AddAsync(newUser, ct);
                await _dbContext.SaveChangesAsync(ct);

                //TODO: fluent validation?

                await transaction.CommitAsync(ct);

                return Ok(newUser);
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
                if (!user.Email.Contains('@')) 
                    return BadRequest();

                var found = await _dbContext.Users.FirstAsync(u => u.Id == user.Id, ct);
                if (found == null)
                {
                    await transaction.RollbackAsync(ct);
                    return NotFound();
                }

                found.Login = user.Login;
                found.PasswordHash = user.PasswordHash;
                found.Email = user.Email;

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
