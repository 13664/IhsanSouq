using Core.Entities;

namespace Core;

public class CharityCaseSpecification: BaseSpecification<CharityCase>
{
    public CharityCaseSpecification(CharityCaseSpecParams specParams) : base(x => 
        (string.IsNullOrEmpty(specParams.Search) || x.Title.ToLower().Contains(specParams.Search)) &&
        (specParams.Categories.Count == 0 || specParams.Categories.Contains(x.Category) ) &&
        (specParams.Levels.Count == 0 || specParams.Levels.Contains(x.UrgencyLevel))
        )
    {

      ApplyPaging(specParams.PageSize * (specParams.PageIndex - 1), specParams.PageSize);
      switch (specParams.Sort)
      {
        case "dateAsc":
            AddOrderBy(x => x.EndDate);
            break;
        case "dateDesc":
            AddOrderByDescending(x => x.EndDate);
            break;   
        default:
        AddOrderBy(x => x.EndDate);
            break;
      }
    }
}
