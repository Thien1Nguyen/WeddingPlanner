using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WeddingPlanner.Models;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Identity;
namespace WeddingPlanner.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private MyContext _context; 
    public HomeController(ILogger<HomeController> logger, MyContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpGet("")]
    public IActionResult Index()
    {
        return View("Index");
    }

    [HttpPost("register")]
    public IActionResult Register(User newUser)
    {
        if(!ModelState.IsValid)
        {
            return View("Index");
        }
        // Initializing a PasswordHasher object, providing our User class as its type            
        PasswordHasher<User> Hasher = new PasswordHasher<User>();   
        // Updating our newUser's password to a hashed version         
        newUser.Password = Hasher.HashPassword(newUser, newUser.Password);            
        //Save your user object to the database 
        _context.Users.Add(newUser);
        _context.SaveChanges();
        HttpContext.Session.SetInt32("UUID", newUser.UserId);
        HttpContext.Session.SetString("UUID", newUser.FirstName);
        return RedirectToAction("IndexWedding", "Wedding");
    }

    [HttpPost("login")]
    public IActionResult Login(LoginUser userSubmission)
    {
        if(!ModelState.IsValid)
        {
            return View("Index",userSubmission);
        }

        User? userInDb = _context.Users.FirstOrDefault(u => u.Email == userSubmission.LoginEmail);
        if(userInDb == null)        
        {            
            // Add an error to ModelState and return to View!            
            ModelState.AddModelError("LoginEmail", "Invalid Email/Password");            
            return View("Index");        
        }
        // Otherwise, we have a user, now we need to check their password                 
        // Initialize hasher object        
        PasswordHasher<LoginUser> hasher = new PasswordHasher<LoginUser>();                    
        // Verify provided password against hash stored in db        
        var result = hasher.VerifyHashedPassword(userSubmission, userInDb.Password, userSubmission.LoginPassword);
        if(result == 0)        
        {            
            ModelState.AddModelError("LoginPassword", "Invalid Email/Password");            
            return View("Index");        
        }
        HttpContext.Session.SetString("UUName", userInDb.FirstName);
        HttpContext.Session.SetInt32("UUID", userInDb.UserId);
        return RedirectToAction("IndexWedding", "Wedding");
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
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