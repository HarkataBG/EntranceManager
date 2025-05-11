using EntranceManager.Models.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EntranceManager.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {

        public UsersController()
        {
        }

        //[Authorize(Roles = "Administrator")]
        //[HttpPost("promote")]
        //public async Task<IActionResult> PromoteEntranceManager([FromBody] EntranceManagerDto dto)
        //{
        //    //return Ok($"User {user.Username} is now a building manager.");
        //}
    }
}
