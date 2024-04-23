using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RedDish.Models;

namespace RedDish.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenusController : ControllerBase
    {
        private readonly RedDishDbContext _context;

        public MenusController(RedDishDbContext context)
        {
            _context = context;
        }

        [HttpGet("")]
        public IActionResult GetMenu() 
        { 
            return Ok(new { menu = _context.Menus.ToList() });
        }
    }
}
