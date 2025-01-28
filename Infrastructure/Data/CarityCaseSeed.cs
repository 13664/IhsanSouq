using System.Text.Json;
using System.Text.Json.Nodes;
using Core.Entities;

namespace Infrastructure.Data;

public class CharityCaseSeed
{
  public static async Task SeedAsync(CharityCaseContext context)
  {
    if(!context.CharityCases.Any())
    {
        var charityCaseData = await File.ReadAllTextAsync("../Infrastructure/Data/SeedData/cases.json");
        var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true // This ensures JSON property names don't have to match case
            };
        var cases = JsonSerializer.Deserialize<List<CharityCase>>(charityCaseData, options);

        if(cases == null ) return;
        context.CharityCases.AddRange(cases);
        await context.SaveChangesAsync();
    }
  }
}
