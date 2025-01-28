using Core.Entities;

namespace Core.Specifications;

public class CategoryListSpecification : BaseSpecification<CharityCase, string>
{
  public CategoryListSpecification()
  {
    AddSelect(x => x.Category);
    ApplyDistinct();
  }

}
