using Webapi.shared;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using System.Linq;
using Newtonsoft.Json;
using Castle.Core.Resource;
using Castle.Core.Logging;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.AspNetCore.Cors;
namespace Webapi.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class Search : ControllerBase
    {
        private readonly Context _context;
        private readonly ILogger<Search> _logger;

        public Search(Context context, ILogger<Search> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        [Route("getitems")]

        public IActionResult GetItems(string query)
        {
            var results = _context.Product
                .Include(p => p.ProductCategory)
                .Include(p => p.ProductData)
                .Include(p => p.ProductImage)
                .Include(p => p.ProductReviews)
                .Include(p => p.ProductTag)
                .Include(p => p.Account)
                .Include(p => p.ProductDetails)
                .Where(p => p.ProductDetails
                    .ProductName
                        .ToLower()
                        .Contains(query.ToLower()))
                .Take(5)
                .ToList();

            return Ok(results);

        }

    }



}
