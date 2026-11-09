
using Microsoft.AspNetCore.Mvc;
using RaceDay.Mvc.Services;

namespace RaceDay.Mvc.Controllers;

public class AccountController(ApiService api) : Controller
{
    // LOGIN PAGE
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    // LOGIN USER
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(
        string emailAddress,
        string password)
    {
        var r = await api.Send(
            HttpContext,
            HttpMethod.Post,
            "api/Auth/login",
            new { emailAddress, password });

        if (!r.ok)
        {
            ModelState.AddModelError("", r.message);
            return View();
        }

        // Get the user's role from the API
        var role = r.data.TryGetProperty(
            "userRole", out var roleProp)
            ? roleProp.GetString()
            : null;

        if (role != "Organiser" && role != "Participant")
        {
            ModelState.AddModelError(
                "",
                "Login response is missing a valid role.");

            return View();
        }

        // Get the user's full name from the API
        var fullName = r.data.TryGetProperty(
            "fullName", out var nameProp)
            ? nameProp.GetString()
            : null;

        // Extract the first name
        var firstName = fullName?
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .FirstOrDefault() ?? "Runner";

        // Save user information in the MVC session
        HttpContext.Session.SetString("Role", role);
        HttpContext.Session.SetString("FullName", fullName ?? "");
        HttpContext.Session.SetString("FirstName", firstName);

        // Redirect according to role
        if (role == "Organiser")
        {
            return RedirectToAction("Index", "Organiser");
        }

        return RedirectToAction("Index", "Events");
    }

    // REGISTRATION PAGE
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    // REGISTER USER
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(
        string fullName,
        string emailAddress,
        string phoneNumber,
        string password,
        string userRole)
    {
        var r = await api.Send(
            HttpContext,
            HttpMethod.Post,
            "api/Auth/register",
            new
            {
                fullName,
                emailAddress,
                phoneNumber,
                password,
                userRole
            });

        if (!r.ok)
        {
            ModelState.AddModelError("", r.message);
            return View();
        }

        TempData["Success"] =
            "Account created successfully. Please log in.";

        return RedirectToAction("Login");
    }

    // LOGOUT USER
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await api.Send(
            HttpContext,
            HttpMethod.Post,
            "api/Auth/logout");

        HttpContext.Session.Clear();

        return RedirectToAction("Index", "Home");
    }
}
