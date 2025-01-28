using System.Security.Cryptography.X509Certificates;
using Core.Entities;

namespace Core.Specifications;

public class LevelListSpecification : BaseSpecification<CharityCase, string>
{
  public LevelListSpecification(){
    AddSelect(x => x.UrgencyLevel);
    ApplyDistinct();
  }

}
