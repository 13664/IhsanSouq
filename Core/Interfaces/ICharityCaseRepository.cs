using Core.Entities;

namespace Core.Interfaces;

public interface ICharityCaseRepository
{
  Task<CharityCase> GetCharityCaseByIdAsync(int id);
  Task<IReadOnlyList<CharityCase>> GetCharityCasessAsync(string? category, string? urgencyLevel, string? sort);
  Task<IReadOnlyList<string>> GetCharityCategoriesAsync();

  Task<IReadOnlyList<string>> GetCharityLevelAsync();
  void AddCharityCase(CharityCase charityCase);
  void UpdateCharityCase(CharityCase charityCase);
  void DeleteCharityCase(CharityCase charityCase);
  bool CharityCaseExits(int id);
  Task<bool> SaveChangesAsync();


}
