#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
// Add this using statement to access NotMapped
using System.ComponentModel.DataAnnotations.Schema;
namespace WeddingPlanner.Models;

public class Wedding
{
    [Key]
    public int WeddingId {get;set;}
    [Required]
    public string WedderOne {get;set;}
    [Required]
    public string WedderTwo {get;set;}
    [Required]
    [Future]
    public DateTime? Date {get;set;}
    [Required]
    public string Address {get;set;}

    public DateTime CreatedAt {get;set;} = DateTime.Now;        
    public DateTime UpdatedAt {get;set;} = DateTime.Now;
    public int UserId {get;set;} 

    public User? Creator { get; set; }
    public List<RSVP>? RSVPs {get;set;} = new List<RSVP>();
}

public class FutureAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
    	// Though we have Required as a validation, sometimes we make it here anyways
    	// In which case we must first verify the value is not null before we proceed
        if(value == null)
        {
    	    // If it was, return the required error
            return new ValidationResult("A date for the wedding is required!");
        }

    	if(DateTime.Now > (DateTime) value)
        {
    	    // If yes, throw an error
            return new ValidationResult("The Wedding Can't be in the past!");
        } else {
    	    // If no, proceed
            return ValidationResult.Success;
        }
    }
}