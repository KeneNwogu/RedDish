using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RedDish.DTOs;
using RedDish.Models;
using System.Security.Claims;

namespace RedDish.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly RedDishDbContext _context;

        public OrdersController(RedDishDbContext context)
        {
            _context = context;
        }

        [HttpGet("")]
        [Authorize]
        public IActionResult GetCurrentOrder()
        {
            int userId = int.Parse(HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value);

            Order? order = _context.Orders
                .Include(x => x.OrderItems)
                .ThenInclude(i => i.MenuItem)
                .FirstOrDefault(x => x.UserId == userId && x.CurrentOrder == true);

            return Ok(new { order });
        }

        [HttpPost("")]
        [Authorize]
        public IActionResult EditOrder(OrderItemDto orderItemDto)
        {
            int userId = int.Parse(HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value);

            Order? currentOrder = _context.Orders.FirstOrDefault(o => o.CurrentOrder == true);

            if (currentOrder == null) {
                currentOrder = new Order
                {
                    CurrentOrder = true,
                    PaymentSuccess = true,
                    PaymentStatus = "pending",
                    Status = "pending",
                    UserId = userId
                };

                _context.Orders.Add(currentOrder);
                _context.SaveChanges();
            }

            // check if menu item exists
            Menu? menu = _context.Menus.FirstOrDefault(m => m.Id == orderItemDto.MenuId);
            if (menu == null) return BadRequest(new { message = "Invalid Menu item provided" });

            // upsert order items based on current order id
            OrderItem? item = _context.OrderItems.FirstOrDefault(i => i.OrderId == currentOrder.Id && i.MenuId == orderItemDto.MenuId);
            if(item == null)
            {
                item = new OrderItem
                {
                    MenuId = orderItemDto.MenuId,
                    Quantity = orderItemDto.Quantity,
                    OrderId = currentOrder.Id,
                    UnitPrice = menu.Price
                };

                _context.OrderItems.Add(item);
                _context.SaveChanges();
            }

            else
            {
                item.Quantity = orderItemDto.Quantity;
                _context.SaveChanges();
            }

            // re-calculate total amount, tax and discounts
            List<OrderItem> currentItems = _context.OrderItems.Where(i => i.OrderId == currentOrder.Id).ToList();
            decimal totalAmount = currentItems.Aggregate(0m, (acc, i) => acc + (i.Quantity * i.UnitPrice));

            currentOrder.TotalAmount = totalAmount;
            currentOrder.TaxCharges = 0.1m * totalAmount; // 10% tax
            if (currentOrder.TotalAmount > 50) currentOrder.Discount = 0.05m * totalAmount;
            else currentOrder.Discount = 0;

            _context.SaveChanges();
            return Ok(new { item });
        }
    }
}
