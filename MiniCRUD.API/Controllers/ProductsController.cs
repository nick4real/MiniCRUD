using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniCRUD.API.Infrastructure;
using MiniCRUD.API.Services;
using MiniCRUD.Domain.Models;

namespace MiniCRUD.API.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ILogger<UsersController> _logger;
    private readonly AppDbContext _dbContext;

    public ProductsController(ILogger<UsersController> logger, AppDbContext dbContext)
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
                var products = await _dbContext.Products
                    .AsNoTracking()
                    .ToListAsync(ct);

                if (products == null || !products.Any())
                {
                    await transaction.RollbackAsync(ct);
                    return NotFound(null);
                }

                await transaction.CommitAsync(ct);
                return Ok(products);
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
                var product = await _dbContext.Products
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Id == id, ct);

                if (product == null)
                {
                    await transaction.RollbackAsync(ct);
                    return NotFound();
                }

                await transaction.CommitAsync(ct);
                return Ok(product);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync(ct);
                throw;
            }
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create(Product product, CancellationToken ct)
    {
        var executionStrategy = _dbContext.Database.CreateExecutionStrategy();
        return await executionStrategy.ExecuteAsync<IActionResult>(async () =>
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(ct);
            try
            {
                if (String.IsNullOrWhiteSpace(product.Name) 
                || String.IsNullOrWhiteSpace(product.Description)
                || String.IsNullOrWhiteSpace(product.Adjective)
                || String.IsNullOrWhiteSpace(product.Material)
                )
                    return BadRequest();

                var found = await _dbContext.Products
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Id == product.Id, ct);
                if (found != null) return BadRequest();

                product.Id = Guid.NewGuid();

                await _dbContext.Products.AddAsync(product, ct);
                await _dbContext.SaveChangesAsync(ct);

                //TODO: fluent validation?

                await transaction.CommitAsync(ct);

                return Ok(product);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync(ct);
                throw;
            }
        });

        
    }

    [HttpPut]
    public async Task<IActionResult> Update(Product product, CancellationToken ct)
    {
        var executionStrategy = _dbContext.Database.CreateExecutionStrategy();
        return await executionStrategy.ExecuteAsync<IActionResult>(async () =>
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(ct);
            try
            {
                var found = await _dbContext.Products.FirstAsync(p => p.Id == product.Id, ct);
                if (found == null)
                {
                    await transaction.RollbackAsync(ct);
                    return NotFound();
                }

                found.Name = product.Name;
                found.Description = product.Description;
                found.Price = product.Price;
                found.Adjective = product.Adjective;
                found.Material = product.Material;

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
                var product = await _dbContext.Products.FirstAsync(p => p.Id == id, ct);
                if (product == null)
                {
                    await transaction.RollbackAsync(ct);
                    return NotFound();
                }

                _dbContext.Products.Remove(product);

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
