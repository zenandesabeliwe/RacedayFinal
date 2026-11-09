using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using RaceDay.Api.database;

namespace RaceDay.Api.Controllers;

[ApiController]
[Route("api/images")]
public class ImagesController(IConfiguration configuration) : ControllerBase
{
    private readonly DatabaseConnection db = new(configuration);
    [HttpPost("events/{eventId:int}/banner")]
    [RequestSizeLimit(5 * 1024 * 1024)]
    public async Task<IActionResult> EventBanner(int eventId, IFormFile file)
    {
        var userId = HttpContext.Session.GetInt32("userID");
        if (userId == null) return Unauthorized();
        if (HttpContext.Session.GetString("userRole") != "Organiser") return Forbid();
        using (var con = db.get_connection())
        {
            await con.OpenAsync();
            using var cmd = new SqlCommand(@"SELECT COUNT(*) FROM Events e INNER JOIN Organisers o ON e.organiserID=o.organiserID WHERE e.eventID=@event AND o.userID=@user", con);
            cmd.Parameters.AddWithValue("@event", eventId); cmd.Parameters.AddWithValue("@user", userId.Value);
            if ((int)(await cmd.ExecuteScalarAsync() ?? 0) == 0) return NotFound();
        }
        return await Upload(file, "event-banners", "Events", "eventID", eventId, "bannerUrl");
    }
    [HttpPost("profile/photo")]
    [RequestSizeLimit(5 * 1024 * 1024)]
    public async Task<IActionResult> ProfilePhoto(IFormFile file)
    {
        var userId = HttpContext.Session.GetInt32("userID");
        if (userId == null) return Unauthorized();
        if (HttpContext.Session.GetString("userRole") != "Participant") return Forbid();
        return await Upload(file, "profile-photos", "Users", "userID", userId.Value, "profilePictureUrl");
    }
    private async Task<IActionResult> Upload(IFormFile file, string containerName, string table, string idColumn, int id, string urlColumn)
    {
        if (file == null || file.Length == 0 || file.Length > 5 * 1024 * 1024) return BadRequest("Choose an image under 5 MB.");
        // Validate actual image signature, not only the supplied content type.
        byte[] header = new byte[12];
        await using (var stream = file.OpenReadStream()) await stream.ReadExactlyAsync(header);
        bool jpg = header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF;
        bool png = header[0] == 0x89 && header[1] == 0x50 && header[2] == 0x4E && header[3] == 0x47;
        bool webp = System.Text.Encoding.ASCII.GetString(header, 0, 4) == "RIFF" && System.Text.Encoding.ASCII.GetString(header, 8, 4) == "WEBP";
        if (!jpg && !png && !webp) return BadRequest("Only JPG, PNG and WebP images are supported.");
        var connection = configuration["AzureStorage:ConnectionString"];
        if (string.IsNullOrWhiteSpace(connection)) return StatusCode(503, "Image storage is not configured.");
        var client = new BlobContainerClient(connection, containerName);
        await client.CreateIfNotExistsAsync(PublicAccessType.Blob);
        var blob = client.GetBlobClient($"{Guid.NewGuid():N}.{(jpg ? "jpg" : png ? "png" : "webp")}");
        await using (var stream = file.OpenReadStream())
            await blob.UploadAsync(stream, new BlobHttpHeaders { ContentType = jpg ? "image/jpeg" : png ? "image/png" : "image/webp" });
        using var con = db.get_connection(); await con.OpenAsync();
        using var cmd = new SqlCommand($"UPDATE {table} SET {urlColumn}=@url WHERE {idColumn}=@id", con);
        cmd.Parameters.AddWithValue("@url", blob.Uri.ToString()); cmd.Parameters.AddWithValue("@id", id);
        await cmd.ExecuteNonQueryAsync();
        return Ok(new { url = blob.Uri.ToString() });
    }
}
