using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WeddingPlanner.Models;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
namespace WeddingPlanner.Controllers;

[SessionCheck]
public class WeddingController : Controller
{
    private readonly ILogger<WeddingController> _logger;
    private MyContext _context; 
    public WeddingController(ILogger<WeddingController> logger, MyContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpGet("/weddings")]
    public IActionResult IndexWedding()
    {
        List<Wedding> allWeddings = _context.Weddings.Include(rsvp =>rsvp.RSVPs).ToList();
        return View("IndexWedding", allWeddings);
    }

    //New wedding form page
    [HttpGet("/weddings/new")]
    public IActionResult NewWedding()
    {
        return View("NewWedding");
    }

    //wedding post route
    [HttpPost("/weddings/create")]
    public IActionResult CreateWedding(Wedding newWedding)
    {
        if(!ModelState.IsValid)
        {
            return View("NewWedding", newWedding);
        }
        _context.Weddings.Add(newWedding);
        _context.SaveChanges();
        return RedirectToAction("IndexWedding");
    }

    [HttpGet("weddings/{weddingId}")]
    public IActionResult ShowWedding(int weddingId)
    {
        //add a validator for the single wedding route
        Wedding? wedding = _context.Weddings.Include(w => w.RSVPs).ThenInclude(RSVP => RSVP.User).FirstOrDefault(w => w.WeddingId == weddingId);
        return View("ShowWedding", wedding);
    }

    [HttpGet("weddings/{weddingId}/edit")]
    public IActionResult EditWedding(int weddingId)
    {
        Wedding? wedding = _context.Weddings.FirstOrDefault(w => w.WeddingId == weddingId);
        if(wedding == null)
        {
            return RedirectToAction("IndexWedding");
        }
        return View("EditWedding", wedding);
    }

    [HttpPost("weddings/{weddingId}/update")]
    public IActionResult UpdateWedding(Wedding updatedWedding, int weddingId)
    {
        if(!ModelState.IsValid)
        {
            return EditWedding(weddingId);
        }

        Wedding? wedding = _context.Weddings.FirstOrDefault(w => w.WeddingId == weddingId);
        if(wedding== null)
        {
            return RedirectToAction("IndexWedding");
        }

        wedding.WedderOne = updatedWedding.WedderOne;
        wedding.WedderTwo = updatedWedding.WedderTwo;
        wedding.Date = updatedWedding.Date;
        wedding.Address = updatedWedding.Address;
        wedding.UserId = updatedWedding.UserId;
        wedding.UpdatedAt = DateTime.Now;

        _context.SaveChanges();

        return RedirectToAction("IndexWedding");
    }

    [HttpPost("weddings/{weddingId}/destroy")]
    public IActionResult DeleteWedding(int weddingId)
    {
        Wedding? wedding = _context.Weddings.FirstOrDefault(w => w.WeddingId == weddingId);
        if(wedding != null)
        {
            _context.Weddings.Remove(wedding);
            _context.SaveChanges();
            return RedirectToAction("IndexWedding", "Wedding");
        }
        return RedirectToAction("IndexWedding");
    }
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

public class SessionCheckAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        // Find the session, but remember it may be null so we need int?
        int? userId = context.HttpContext.Session.GetInt32("UUID");
        // Check to see if we got back null
        if(userId == null)
        {
            // Redirect to the Index page if there was nothing in session
            // "Home" here is referring to "HomeController", you can use any controller that is appropriate here
            context.Result = new RedirectToActionResult("Index", "Home", null);
        }
    }
}
