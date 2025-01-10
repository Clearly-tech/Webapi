using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Webapi.shared.DTOs;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Cors;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webapi.shared;

namespace Webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Orders : ControllerBase
    {
        private readonly Context _context;
        private readonly ILogger<Orders> _logger;

        public Orders(Context context, ILogger<Orders> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<orderDto>>> GetOrders()
        {
            _logger.LogInformation("Getting all orders");
            var orders = await _context.Order
                .Select(o => new orderDto
				{
                    OrderID = o.OrderID,
                    OrderDate = o.OrderDate,
                    TotalAmount = o.TotalAmount,
                    OrderStatus = o.OrderStatus.Status.ToString(),
                    OrderDetails = o.OrderDetails.Select(od => new OrderDetailDto
                    {
                        OrderDetailID = od.OrderDetailID,
                        Quantity = od.Quantity,
                        TotalPrice = od.TotalPrice,
                        ProductID = od.ProductID,
                        ProductName = od.Product.ProductDetails.ProductName // Include product name if needed
                    }).ToList()
                })
                .ToListAsync();

            if (orders == null || !orders.Any())
            {
                return NotFound("No orders found.");
            }

            return Ok(orders);
        }

        // GET: api/OrdersByAcc/{id}
        [HttpGet("OrdersByAcc/{id}")]
        public async Task<ActionResult<IEnumerable<orderDto>>> GetOrdersByAccID(string id)
        {
            _logger.LogInformation("Getting orders for account ID: {id}", id);

            var orders = await _context.Order
                .Where(o => o.Id == id)
                .Select(o => new orderDto
				{
                    OrderID = o.OrderID,
                    OrderDate = o.OrderDate,
                    TotalAmount = o.TotalAmount,
                    OrderStatus = o.OrderStatus.Status.ToString(),
                    OrderDetails = o.OrderDetails.Select(od => new OrderDetailDto
                    {
                        OrderDetailID = od.OrderDetailID,
                        Quantity = od.Quantity,
                        TotalPrice = od.TotalPrice,
                        ProductID = od.ProductID,
                        ProductName = od.Product.ProductDetails.ProductName // Include product name if needed
                    }).ToList()
                })
                .ToListAsync();

            if (orders == null || !orders.Any())
            {
                _logger.LogWarning("No orders found for account ID: {id}", id);
                return NotFound("Account doesn't have any orders.");
            }

            return Ok(orders);
        }

        // GET: api/Orders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<orderDto>> GetOrder(int id)
        {
            _logger.LogInformation("Getting order with ID: {id}", id);
            var order = await _context.Order
                .Where(o => o.OrderID == id)
                .Select(o => new orderDto
				{
                    OrderID = o.OrderID,
                    OrderDate = o.OrderDate,
                    TotalAmount = o.TotalAmount,
                    OrderStatus = o.OrderStatus.Status,
                    OrderDetails = o.OrderDetails.Select(od => new OrderDetailDto
                    {
                        OrderDetailID = od.OrderDetailID,
                        Quantity = od.Quantity,
                        TotalPrice = od.TotalPrice,
                        ProductID = od.ProductID,
                        ProductName = od.Product.ProductDetails.ProductName // Include product name if needed
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (order == null)
            {
                _logger.LogWarning("Order with ID: {id} not found", id);
                return NotFound("Order not found.");
            }

            return Ok(order);
        }

        // PUT: api/Orders/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, Order order)
        {
            if (id != order.OrderID)
            {
                _logger.LogError("Order ID mismatch for update");
                return BadRequest();
            }

            _context.Entry(order).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Order with ID: {id} updated", id);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
                {
                    _logger.LogWarning("Order with ID: {id} does not exist", id);
                    return NotFound();
                }
                else
                {
                    _logger.LogError("Error updating order with ID: {id}", id);
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Orders
        [HttpPost]
        public async Task<ActionResult<orderDto>> CreateOrder(Order order)
        {
            _context.Order.Add(order);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Order created with ID: {id}", order.OrderID);
            return CreatedAtAction(nameof(GetOrder), new { id = order.OrderID }, order);
        }

        // DELETE: api/Orders/5
        [HttpDelete("{id}")]
        [EnableCors("AllowSpecificOrigin")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Order.FindAsync(id);
            if (order == null)
            {
                _logger.LogWarning("Order with ID: {id} not found for deletion", id);
                return NotFound();
            }

            _context.Order.Remove(order);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Order with ID: {id} deleted", id);
            return NoContent();
        }

        private bool OrderExists(int id)
        {
            return _context.Order.Any(e => e.OrderID == id);
        }
    }
}