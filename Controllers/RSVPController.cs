using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WeddingPlanner.Models;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
namespace WeddingPlanner.Controllers;

[SessionCheck]
public class RSVPController : Controller
{
    private readonly ILogger<RSVPController> _logger;
    private MyContext _context; 
    public RSVPController(ILogger<RSVPController> logger, MyContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpPost("RSVP/toggle")]
    public IActionResult ToggleRSVP(int weddingId)
    {
        if(!ModelState.IsValid)
        {
            return RedirectToAction("IndexWedding", "Wedding");
        }
        int? userId = HttpContext.Session.GetInt32("UUID");
        RSVP existingRSVP = _context.RSVPs.FirstOrDefault(rsvp => rsvp.UserId == userId && rsvp.WeddingId == weddingId);
        
        if(existingRSVP == null)
        {
            RSVP newRSVP = new RSVP()
            {
                UserId = (int)userId,
                WeddingId = weddingId
            };
            
            _context.RSVPs.Add(newRSVP);
        }
        else
        {
            _context.RSVPs.Remove(existingRSVP);
        }
        
        _context.SaveChanges();
        return RedirectToAction("IndexWedding", "Wedding");
    }
}