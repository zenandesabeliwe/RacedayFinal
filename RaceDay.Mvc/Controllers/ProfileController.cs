using Microsoft.AspNetCore.Mvc;
using RaceDay.Mvc.Services;
using System.Text.Json;
namespace RaceDay.Mvc.Controllers;
public class ProfileController(ApiService api):Controller
{
    public async Task<IActionResult> Index(){if(HttpContext.Session.GetString("Role")==null)return RedirectToAction("Login","Account");var r=await api.Send(HttpContext,HttpMethod.Get,"api/Users/profile");if(!r.ok)TempData["Error"]=r.message;return View(r.data);}
    [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Update(string fullName,string phoneNumber){if(HttpContext.Session.GetString("Role")==null)return RedirectToAction("Login","Account");var r=await api.Send(HttpContext,HttpMethod.Put,"api/Users/profile",new {fullName,phoneNumber});TempData[r.ok?"Success":"Error"]=r.ok?"Profile updated.":r.message;return RedirectToAction("Index");}
}
