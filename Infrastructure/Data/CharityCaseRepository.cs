using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Data;

public class CharityCaseRepository(CharityCaseContext context, ILogger<CharityCaseRepository> logger) : ICharityCaseRepository
{
  public void AddCharityCase(CharityCase charityCase)
  {

    context.CharityCases.Add(charityCase);
  }


  public bool CharityCaseExits(int id)
  {
    return context.CharityCases.Any(x => x.Id == id);
  }

  public void DeleteCharityCase(CharityCase charityCase)
  {
    context.CharityCases.Remove(charityCase);
  }

  public async Task<CharityCase> GetCharityCaseByIdAsync(int id)
    {
        var charityCase = await context.CharityCases
            .Include(c => c.Payments)
            .FirstOrDefaultAsync(c => c.Id == id);

        var payments = await context.Payments
            .Where(p => p.CharityCaseId == id)
        .ToListAsync();

        logger.LogInformation($"Payments count: {payments.Count}");

        return charityCase;
    }

  public async Task<IReadOnlyList<CharityCase>> GetCharityCasessAsync(string? category, string? urgencyLevel, string? sort)
  {
    var query = context.CharityCases.AsQueryable();

    if(!string.IsNullOrWhiteSpace(category))
        query = query.Where(x => x.Category == category);
    if(!string.IsNullOrWhiteSpace(urgencyLevel))
        query = query.Where(x => x.UrgencyLevel == urgencyLevel);
    
    query = sort switch{
      "dateAsc" => query.OrderBy(x => x.EndDate),
      "dateDesc" => query.OrderByDescending(x => x.EndDate),
      _ =>  query.OrderBy(x => x.EndDate) 
    };
      

    return await query.ToListAsync();
  }

    public async Task<IReadOnlyList<string>> GetCharityCategoriesAsync()
    {
        return await context.CharityCases.Select(x => x.Category).Distinct().ToListAsync();
    }

    public async Task<IReadOnlyList<string>> GetCharityLevelAsync()
    {
        return await context.CharityCases.Select(x => x.UrgencyLevel).Distinct().ToListAsync();
    }

    public async Task<bool> SaveChangesAsync()
  {
    return await context.SaveChangesAsync() > 0;

  }

  public void UpdateCharityCase(CharityCase charityCase)
  {
    context.Entry(charityCase).State = EntityState.Modified;
  }
    public async Task<IReadOnlyList<Payment>> GetPaymentsByCharityCaseIdAsync(int charityCaseId)
    {
        return await context.Payments
            .Where(p => p.CharityCaseId == charityCaseId)
            .ToListAsync();
    }
}
