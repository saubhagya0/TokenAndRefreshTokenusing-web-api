using JWTRefreshTokenNew.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JWTRefreshTokenNew.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SignupController : ControllerBase
    {
        MyAuthContext db = new();
        [HttpGet]
        
        public async Task<IActionResult> Getalldata()
        {
          
            var data = await Task.FromResult(new List<string> { "Saubhagya", "Das", "sau123@gmail.com", "123", "12345679" });
            return Ok(data);
        }
    }
}
