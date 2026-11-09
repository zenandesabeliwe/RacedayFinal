using Microsoft.AspNetCore.Mvc;
using RaceDay.Mvc.Services;
using System.Text.Json;
namespace RaceDay.Mvc.Controllers;
public class OrganiserController(ApiService api) : Controller
{
    private bool IsOrganiser => HttpContext.Session.GetString("Role") == "Organiser";
    public async Task<IActionResult> Index() { if (!IsOrganiser) return RedirectToAction("Login", "Account"); var r = await api.Send(HttpContext, HttpMethod.Get, "api/Events"); if (!r.ok) TempData["Error"] = r.message; return View(r.data.ValueKind == JsonValueKind.Array ? r.data : default); }
    [HttpGet] public IActionResult Create() { if (!IsOrganiser) return RedirectToAction("Login", "Account"); return View(); }
    [HttpPost, ValidateAntiForgeryToken] public async Task<IActionResult> Create(string eventName, string description, DateTime eventDate, string location, decimal distance, string eventType, string? categoryName, decimal? categoryDistance, int? categoryAge, string? categoryStartTime)
    {
        if (!IsOrganiser) return Forbid();
        if (!string.IsNullOrWhiteSpace(categoryName) && (!categoryDistance.HasValue || categoryDistance <= 0 || !categoryAge.HasValue || categoryAge < 0 || !TimeSpan.TryParse(categoryStartTime, out _)))
        {
            ModelState.AddModelError("", "Complete the category distance, minimum age and start time, or leave the category name blank.");
            return View();
        }
        var r = await api.Send(HttpContext, HttpMethod.Post, "api/Events", new {eventName,description,eventDate,location,distance,eventType});
        if (!r.ok) { ModelState.AddModelError("",r.message); return View(); }
        if (!string.IsNullOrWhiteSpace(categoryName) && r.data.ValueKind == JsonValueKind.Object && r.data.TryGetProperty("eventID", out var eventIdProperty) && eventIdProperty.TryGetInt32(out var newEventID))
        {
            var category = await api.Send(HttpContext, HttpMethod.Post, $"api/events/{newEventID}/categories", new { categoryName, distance = categoryDistance!.Value, age = categoryAge!.Value, startTime = TimeSpan.Parse(categoryStartTime!) });
            TempData[category.ok ? "Success" : "Error"] = category.ok ? "Event and category created successfully." : $"Event created, but category could not be saved: {category.message}. Add it from Categories.";
        }
        else TempData["Success"] = "Event created successfully.";
        return RedirectToAction("Index");
    }
    [HttpGet] public async Task<IActionResult> Edit(int id) { if (!IsOrganiser) return RedirectToAction("Login", "Account"); var r=await api.Send(HttpContext,HttpMethod.Get,$"api/Events/{id}"); if(!r.ok) {TempData["Error"]=r.message;return RedirectToAction("Index");} return View(r.data); }
    [HttpPost, ValidateAntiForgeryToken] public async Task<IActionResult> Edit(int id,string eventName,string description,DateTime eventDate,string location,decimal distance,string eventType) {if(!IsOrganiser)return Forbid();var r=await api.Send(HttpContext,HttpMethod.Put,$"api/Events/{id}",new {eventName,description,eventDate,location,distance,eventType});TempData[r.ok?"Success":"Error"]=r.ok?"Event updated successfully.":r.message;return RedirectToAction("Index");}
    [HttpPost, ValidateAntiForgeryToken] public async Task<IActionResult> Delete(int id) {if(!IsOrganiser)return Forbid();var r=await api.Send(HttpContext,HttpMethod.Delete,$"api/Events/{id}");TempData[r.ok?"Success":"Error"]=r.ok?"Event deleted successfully.":r.message;return RedirectToAction("Index");}
    public async Task<IActionResult> Categories(int id) {if(!IsOrganiser)return RedirectToAction("Login","Account");ViewBag.EventID=id;var r=await api.Send(HttpContext,HttpMethod.Get,$"api/events/{id}/categories");if(!r.ok)TempData["Error"]=r.message;return View(r.data.ValueKind==JsonValueKind.Array?r.data:default);}
    [HttpPost, ValidateAntiForgeryToken] public async Task<IActionResult> AddCategory(int eventID,string categoryName,decimal distance,int age,string startTime) {if(!IsOrganiser)return Forbid();if(!TimeSpan.TryParse(startTime,out var time)){TempData["Error"]="Enter a valid start time.";return RedirectToAction("Categories",new{id=eventID});}var r=await api.Send(HttpContext,HttpMethod.Post,$"api/events/{eventID}/categories",new {categoryName,distance,age,startTime=time});TempData[r.ok?"Success":"Error"]=r.ok?"Category created successfully.":r.message;return RedirectToAction("Categories",new{id=eventID});}
    [HttpPost, ValidateAntiForgeryToken] public async Task<IActionResult> DeleteCategory(int id,int eventID){if(!IsOrganiser)return Forbid();var r=await api.Send(HttpContext,HttpMethod.Delete,$"api/categories/{id}");TempData[r.ok?"Success":"Error"]=r.ok?"Category deleted.":r.message;return RedirectToAction("Categories",new{id=eventID});}
    public async Task<IActionResult> Enrolments(int id) {if(!IsOrganiser)return RedirectToAction("Login","Account");ViewBag.EventID=id;var r=await api.Send(HttpContext,HttpMethod.Get,$"api/events/{id}/enrolments");if(!r.ok)TempData["Error"]=r.message;return View(r.data.ValueKind==JsonValueKind.Array?r.data:default);}
    [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> RecordResult(int eventID,int enrolmentID,string finishTime,int finishPosition){if(!IsOrganiser)return Forbid();if(!TimeSpan.TryParse(finishTime,out var time)){TempData["Error"]="Invalid finish time.";return RedirectToAction("Enrolments",new{id=eventID});}var r=await api.Send(HttpContext,HttpMethod.Post,$"api/enrolments/{enrolmentID}/results",new {finishTime=time,finishPosition});TempData[r.ok?"Success":"Error"]=r.ok?"Result recorded.":r.message;return RedirectToAction("Enrolments",new{id=eventID});}
    public async Task<IActionResult> Results(int id){if(!IsOrganiser)return RedirectToAction("Login","Account");ViewBag.EventID=id;var r=await api.Send(HttpContext,HttpMethod.Get,$"api/events/{id}/results");if(!r.ok)TempData["Error"]=r.message;return View(r.data.ValueKind==JsonValueKind.Array?r.data:default);}
}
