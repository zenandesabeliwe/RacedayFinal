
using Microsoft.AspNetCore.Mvc;
using RaceDay.Mvc.Services;
using System.Text.Json;

namespace RaceDay.Mvc.Controllers;

public class ParticipantController(ApiService api) : Controller
{
    private bool IsParticipant =>
        HttpContext.Session.GetString("Role") == "Participant";

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Enrol(int eventID)
    {
        if (!IsParticipant)
            return Forbid();

        var r = await api.Send(
            HttpContext,
            HttpMethod.Post,
            "api/Enrolments",
            new { eventID });

        TempData[r.ok ? "Success" : "Error"] =
            r.ok ? "You have enrolled successfully." : r.message;

        return RedirectToAction(
            "Details", "Events", new { id = eventID });
    }

    public async Task<IActionResult> Results()
    {
        if (!IsParticipant)
            return RedirectToAction("Login", "Account");

        var r = await api.Send(
            HttpContext,
            HttpMethod.Get,
            "api/results/me");

        if (!r.ok)
            TempData["Error"] = r.message;

        return View(
            r.data.ValueKind == JsonValueKind.Array
                ? r.data
                : default);
    }

    public async Task<IActionResult> MyEnrolments()
    {
        if (!IsParticipant)
            return RedirectToAction("Login", "Account");

        var r = await api.Send(
            HttpContext,
            HttpMethod.Get,
            "api/Enrolments/me");

        if (!r.ok)
            TempData["Error"] = r.message;

        return View(
            r.data.ValueKind == JsonValueKind.Array
                ? r.data
                : default);
    }
}
