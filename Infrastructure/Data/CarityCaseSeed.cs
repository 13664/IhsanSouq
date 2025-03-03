using System.Text.Json;
using System.Text.Json.Nodes;
using Core.Entities;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Data;

public class CharityCaseSeed
{
  public static async Task SeedAsync(CharityCaseContext context, UserManager<AppUser> userManager)
  {

    if(!userManager.Users.Any(x => x.UserName == "admin@test.com")){
      var user = new AppUser{
        UserName = "admin@test.com",
        Email = "admin@test.com"
      };
      await userManager.CreateAsync(user, "Pa$$0rd");
      await userManager.AddToRoleAsync(user, "Admin");
    }

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
