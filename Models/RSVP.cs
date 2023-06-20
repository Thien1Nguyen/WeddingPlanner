#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
// Add this using statement to access NotMapped
using System.ComponentModel.DataAnnotations.Schema;
namespace WeddingPlanner.Models;

public class RSVP
{
    [Key]
    public int RSVSId {get;set;}
    [Required]
    public int UserId {get;set;}
    [Required]
    public int WeddingId{get;set;}
    public DateTime CreatedAt {get;set;} = DateTime.Now;  

    public User? User {get;set;}
    public Wedding? Wedding{get;set;}
}
