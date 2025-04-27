namespace Core.Entities;

public class CharityCase : BaseEntity
{

  public required string Title { get; set; } // Title of the aid request
  public required string Description { get; set; } // Detailed description of the request

  public decimal AmountRequested { get; set; } // Total financial assistance needed
  public decimal AmountCollected { get; set; } // Amount collected so far
  public required string Category { get; set; } // Type of assistance (e.g., medical, education)
  public required string UrgencyLevel { get; set; }
  public DateTime RequestDate { get; set; } // Date when the request was created
  public DateTime? EndDate { get; set; } // Deadline for fundraising
  public bool IsActive { get; set; } // Indicates if the request is active
  public required string ImageUrl { get; set; } // Image associated with the request
  public required string BeneficiaryName { get; set; } // Name of the individual/group needing assistance
  public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
