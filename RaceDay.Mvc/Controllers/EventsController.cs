
using Microsoft.AspNetCore.Mvc;
using RaceDay.Mvc.Services;
using System.Text.Json;

namespace RaceDay.Mvc.Controllers;

public class EventsController(ApiService api) : Controller
{
    public async Task<IActionResult> Index()
    {
        string? role = HttpContext.Session.GetString("Role");

        if (role == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var response = await api.Send(
            HttpContext,
            HttpMethod.Get,
            "api/Events");

        if (response.ok == false)
        {
            TempData["Error"] = response.message;
        }

        if (response.data.ValueKind == JsonValueKind.Array)
        {
            return View(response.data);
        }

        return View(default(JsonElement));
    }

    public async Task<IActionResult> Details(int id)
    {
        string? role = HttpContext.Session.GetString("Role");

        if (role == null)
        {
            return RedirectToAction("Login", "Account");
        }

        // Get the selected race
        var response = await api.Send(
            HttpContext,
            HttpMethod.Get,
            $"api/Events/{id}");

        if (response.ok == false)
        {
            return RedirectToAction("Index");
        }

        // Get race categories
        var categories = await api.Send(
            HttpContext,
            HttpMethod.Get,
            $"api/events/{id}/categories");

        if (categories.data.ValueKind == JsonValueKind.Array)
        {
            ViewData["Categories"] = categories.data;
        }
        else
        {
            ViewData["Categories"] = default(JsonElement);
        }

        if (categories.ok == false)
        {
            TempData["Error"] = categories.message;
        }

        // Check whether the participant has enrolled
        bool alreadyEnrolled = false;
        bool enrolmentCheckSucceeded = false;

        if (role == "Participant")
        {
            var enrolments = await api.Send(
                HttpContext,
                HttpMethod.Get,
                "api/Enrolments/me");

            if (enrolments.ok == true &&
                enrolments.data.ValueKind == JsonValueKind.Array)
            {
                enrolmentCheckSucceeded = true;

                foreach (JsonElement enrolment in
                         enrolments.data.EnumerateArray())
                {
                    int enrolledEventID =
                        enrolment.GetProperty("eventID").GetInt32();

                    if (enrolledEventID == id)
                    {
                        alreadyEnrolled = true;
                        break;
                    }
                }
            }
            else
            {
                TempData["Error"] =
                    "Could not check your enrolment status.";
            }
        }

        ViewBag.AlreadyEnrolled = alreadyEnrolled;
        ViewBag.EnrolmentCheckSucceeded = enrolmentCheckSucceeded;

        return View(response.data);
    }
}
