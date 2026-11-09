
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using RaceDay.Api.database;
using RaceDay.Api.Models;

namespace RaceDay.Api.Controllers
{
    [Route("api")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private bool EventExists(int eventID)
        {
            using var connection = database.get_connection();
            connection.Open();
            using var command = new SqlCommand("SELECT COUNT(*) FROM Events WHERE eventID=@id", connection);
            command.Parameters.AddWithValue("@id", eventID);
            return Convert.ToInt32(command.ExecuteScalar()) > 0;
        }
        //database declarations
        private readonly DatabaseConnection database;
        private readonly RaceDayData data;

        //constructor
        public CategoriesController(IConfiguration configuration)
        {
            database = new DatabaseConnection(configuration);
            data = new RaceDayData(database);
        }

        //method to view categories for an event
        //both Organisers and Participants can view categories
        [HttpGet("events/{eventID:int}/categories")]
        public IActionResult get_categories(int eventID)
        {
            //check if user is logged in
            int? userID = HttpContext.Session.GetInt32("userID");

            if (userID == null)
            {
                return Unauthorized("Please log in first.");
            }

            //try and catch
            try
            {
                return Ok(data.get_categories(eventID));
            }
            catch (Exception error)
            {
                System.Diagnostics.Debug.WriteLine(error.Message);
                return StatusCode(500, "Failed to retrieve categories.");
            }
        }

        //method to create a category
        //all authenticated Organisers can create categories
        [HttpPost("events/{eventID:int}/categories")]
        public IActionResult create_category(int eventID, CategoryRequest request)
        {
            //temp variable declarations
            int? userID = HttpContext.Session.GetInt32("userID");
            string? role = HttpContext.Session.GetString("userRole");

            //check if user is logged in
            if (userID == null)
            {
                return Unauthorized("Please log in first.");
            }

            //check if user is an Organiser
            if (role != "Organiser")
            {
                return StatusCode(403, "Only Organisers can create categories.");
            }

            //validate category information
            if (!valid_category_details(request))
            {
                return BadRequest("Please enter valid category details.");
            }

            //try and catch
            try
            {
                //check that the event exists
                if (!EventExists(eventID))
                {
                    return NotFound("Event not found.");
                }

                //get Organiser ID
                int organiserID = data.get_organiser_id(userID.Value);

                if (organiserID == 0)
                {
                    return StatusCode(403, "Organiser profile was not found.");
                }

                //store category information
                int newID = data.store_category(request, eventID, organiserID);

                if (newID <= 0)
                {
                    return StatusCode(500, "Failed to create category.");
                }

                return Ok(new
                {
                    message = "Category created successfully.",
                    categoryID = newID
                });
            }
            catch (Exception error)
            {
                System.Diagnostics.Debug.WriteLine(error.Message);
                return StatusCode(500, "Failed to create category.");
            }
        }

        //method to update a category
        //all authenticated Organisers can update categories
        [HttpPut("categories/{id:int}")]
        public IActionResult update_category(int id, CategoryRequest request)
        {
            //temp variable declarations
            int? userID = HttpContext.Session.GetInt32("userID");
            string? role = HttpContext.Session.GetString("userRole");

            //check if user is logged in
            if (userID == null)
            {
                return Unauthorized("Please log in first.");
            }

            //check if user is an Organiser
            if (role != "Organiser")
            {
                return StatusCode(403, "Only Organisers can update categories.");
            }

            //validate category information
            if (!valid_category_details(request))
            {
                return BadRequest("Please enter valid category details.");
            }

            //try and catch
            try
            {
                //connect to database
                using SqlConnection connect = database.get_connection();
                connect.Open();

                //SQL update query
                //shared organiser permissions
                string query = @"UPDATE Categories
                                 SET categoryName = @name,
                                     distance = @distance,
                                     age = @age,
                                     startTime = @start
                                 WHERE categoryID = @id
                                 ;";

                SqlCommand run_query = new SqlCommand(query, connect);

                //SQL parameters
                run_query.Parameters.AddWithValue("@name", request.categoryName.Trim());
                run_query.Parameters.AddWithValue("@distance", request.distance);
                run_query.Parameters.AddWithValue("@age", request.age);
                run_query.Parameters.AddWithValue("@start", request.startTime);
                run_query.Parameters.AddWithValue("@id", id);
                

                //execute query
                int results = run_query.ExecuteNonQuery();

                if (results > 0)
                {
                    return Ok("Category updated successfully.");
                }
                else
                {
                    return NotFound("Category not found.");
                }
            }
            catch (Exception error)
            {
                System.Diagnostics.Debug.WriteLine(error.Message);
                return StatusCode(500, "Failed to update category.");
            }
        }

        //method to delete a category
        //all authenticated Organisers can delete categories
        [HttpDelete("categories/{id:int}")]
        public IActionResult delete_category(int id)
        {
            //temp variable declarations
            int? userID = HttpContext.Session.GetInt32("userID");
            string? role = HttpContext.Session.GetString("userRole");

            //check if user is logged in
            if (userID == null)
            {
                return Unauthorized("Please log in first.");
            }

            //check if user is an Organiser
            if (role != "Organiser")
            {
                return StatusCode(403, "Only Organisers can delete categories.");
            }

            //try and catch
            try
            {
                //connect to database
                using SqlConnection connect = database.get_connection();
                connect.Open();

                //SQL delete query
                //shared organiser permissions
                string query = @"DELETE FROM Categories
                                 WHERE categoryID = @id
                                 ;";

                SqlCommand run_query = new SqlCommand(query, connect);

                //SQL parameters
                run_query.Parameters.AddWithValue("@id", id);
                

                //execute query
                int results = run_query.ExecuteNonQuery();

                if (results > 0)
                {
                    return NoContent();
                }
                else
                {
                    return NotFound("Category not found.");
                }
            }
            catch (SqlException error) when (error.Number == 547)
            {
                return Conflict("Cannot delete category because it has related records.");
            }
            catch (Exception error)
            {
                System.Diagnostics.Debug.WriteLine(error.Message);
                return StatusCode(500, "Failed to delete category.");
            }
        }

        //method to validate category information
        private static bool valid_category_details(CategoryRequest request)
        {
            if (request == null)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(request.categoryName))
            {
                return false;
            }

            if (request.distance <= 0 || request.distance > 999.99m)
            {
                return false;
            }

            if (request.age < 0)
            {
                return false;
            }

            return true;
        }
    }
}
