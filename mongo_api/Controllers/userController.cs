using Microsoft.AspNetCore.Mvc;
using mongo_api.Model;
using mongo_api.Services;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace mongo_api.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class userController : ControllerBase
    {
        private readonly mongoService _mongoService;
        public userController(mongoService mongoService)
        {
            _mongoService = mongoService;
        }

        [HttpGet]
        public async Task<IActionResult> Getusers()
        {
            var users = await _mongoService.Users.Find(_ => true).ToListAsync();
            return Ok(users);
        }

        [HttpGet("by-sortedby")]
        public async Task<IActionResult> GetSorted(string? sortedby)
        {
            var query = _mongoService.Users.AsQueryable();

            if (sortedby == "name")
            {
                query = query.OrderBy(u => u.name);
            }
            else if (sortedby == "email")
            {
                query = query.OrderBy(u => u.email);
            }
            else if (sortedby == "age")
            {
                query = query.OrderBy(u => u.age);
            }
            else
            {
                query = query.OrderByDescending(u => u.CreatedAt);
            }

            var res = await query.ToListAsync();
            return Ok(res);
        }

        [HttpGet("by-email")]
        public async Task<IActionResult> Getbyemail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) {
                return BadRequest("Email is Required");
            }
            var user = await _mongoService.Users.Find(u => u.email == email).FirstOrDefaultAsync();
            if (user == null) {
                return NotFound("User Not Found");
            }

            return Ok(user);
        }

        [HttpGet("by-name")]
        public async Task<IActionResult> GetbyName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest("User name is Required");
            }
            var user = await _mongoService.Users.Find(u => u.name == name).FirstOrDefaultAsync();
            if (user == null)
            {
                return NotFound("User Not Found");
            }

            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> createUser(User user)
        {
            if (string.IsNullOrWhiteSpace(user.name) || string.IsNullOrWhiteSpace(user.email))
            {
                return BadRequest("Name and Email required");
            }
            var existing = await _mongoService.Users.Find(u => u.email == user.email).FirstOrDefaultAsync();

            if (existing != null)
            {
                return BadRequest("User already exist");
            }

            user.CreatedAt = DateTime.UtcNow;
            await _mongoService.Users.InsertOneAsync(user);
            return Ok(user);
        }

        [HttpPut("{email}")]
        public async Task<IActionResult> updateByemail(string email, [FromBody] User user)
        {
            var exist = await _mongoService.Users.Find(u => u.email == email).FirstOrDefaultAsync();
            
            if (exist == null)
            {
                return NotFound("User not found");
            }
            var updatedUser = Builders<User>.Update
                .Set(s => s.name, user.name)
                .Set(s => s.age, user.age);
            var Res = await _mongoService.Users.UpdateOneAsync(u => u.email == email, updatedUser);
            return Ok("Sucess");
        }

        [HttpDelete("by-email")]
        public async Task<IActionResult> deleteUserbyemail(string email)
        {
            var res = await _mongoService.Users.DeleteOneAsync(u => u.email == email);
            if (res.DeletedCount != 0)
            {
                return NoContent();
            }
            return NotFound();
        }

        [HttpDelete("by-name")]
        public async Task<IActionResult> deleteUserbyName(string name)
        {
            var res = await _mongoService.Users.DeleteOneAsync(u => u.name == name);
            if (res.DeletedCount != 0)
            {
                return NoContent();
            }
            return NotFound();
        }
    }
}
