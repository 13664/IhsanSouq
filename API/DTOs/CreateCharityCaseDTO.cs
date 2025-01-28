using System.ComponentModel.DataAnnotations;

namespace API;

public class CreateCharityCaseDTO
{
  [Required]
  public  string Title { get; set; } = string.Empty; // Title of the aid request
   [Required]
  public  string Description { get; set; } = string.Empty; // Detailed description of the request
  [Range(100, double.MaxValue, ErrorMessage ="Amount requested  must be greater than 0")]
  public decimal AmountRequested { get; set; } // Total financial assistance needed
  [Range(0, double.MaxValue, ErrorMessage ="Amount must be greater than 0")]
  public decimal AmountCollected { get; set; } // Amount collected so far
   [Required]
  public  string Category { get; set; } = string.Empty;// Type of assistance (e.g., medical, education)
   [Required]
  public  string UrgencyLevel { get; set; } = string.Empty;
   [Required]
  public DateTime RequestDate { get; set; } // Date when the request was created
   [Required]
  public DateTime? EndDate { get; set; } // Deadline for fundraising
  public bool IsActive { get; set; } // Indicates if the request is active
   [Required]
  public  string ImageUrl { get; set; } = string.Empty;// Image associated with the request
   [Required]
  public  string BeneficiaryName { get; set; } = string.Empty;// Name of the individual/group needing assistance

  
}
