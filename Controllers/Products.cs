using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Webapi.shared;
using Webapi.shared.DTOs;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Cors;
using Newtonsoft.Json;

namespace Webapi.Controllers
{
	[Route("api/[controller]")]
    [ApiController]
	public class Products : ControllerBase
	{
		private readonly Context _context;
		private readonly ILogger<Products> _logger;

		public Products(Context context, ILogger<Products> logger)
		{
			_context = context;
			_logger = logger;
		}

		// GET: api/Products
		[HttpGet]
		public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts(int limit)
		{
			try
			{
				// Handle invalid limit values
				if (limit <= 0)
				{
					_logger.LogInformation("Getting all products without limit.");
					// Get limited number of products
					var products = await _context.Product
						.Include(p => p.ProductData)
						.Include(p => p.ProductDetails)
						.Include(p => p.ProductImage)
						.Include(p => p.ProductReviews)
						.Include(p => p.ProductTag)
						.Include(p => p.Account)
						.Select(p => new ProductDto
						{
							ProductName = p.ProductDetails.ProductName,
							ProductID = p.ProductID,
							ProductPrice = p.ProductData.ProductPrice,
							ProductStock = p.ProductData.ProductStock,
							ProductDescription = p.ProductDetails.ProductDescriptionSmall,
							Productimg = p.ProductImage.SmallImage,
							ProductQuantity = p.ProductData.ProductStock,
							Productreviewcount = p.ProductReviews.Count(),
							Producttags = p.ProductTag.TagName,
							NumberofSold = p.ProductData.ProductSold,
							CreatedAt = p.ProductData.CreatedAt,
							isArchived = p.ProductData.IsArchived,
							isFeatured = p.ProductData.IsFeatured,
							countreviews = p.ProductReviews.Count(),
							countstars = p.ProductReviews.Select(p => p.Stars).Average().ToString("0.0"),
							CategoryIcon = p.ProductCategory.CategoryIcon,
							CategoryName = p.ProductCategory.CategoryName
						})
						.ToListAsync();

					if (products == null || !products.Any())
					{
						return NotFound("No products found.");
					}

					return Ok(JsonConvert.SerializeObject(products)); // Directly return the products
				}
				else
				{
					_logger.LogInformation($"Getting {limit} products.");
					// Get limited number of products
					var products = await _context.Product
						.Include(p => p.ProductData)
						.Include(p => p.ProductDetails)
						.Include(p => p.ProductImage)
						.Include(p => p.ProductReviews)
						.Include(p => p.ProductTag)
						.Include(p => p.Account)
						.Select(p => new ProductDto
						{
							ProductName = p.ProductDetails.ProductName,
							ProductID = p.ProductID,
							ProductPrice = p.ProductData.ProductPrice,
							ProductStock = p.ProductData.ProductStock,
							ProductDescription = p.ProductDetails.ProductDescriptionSmall,
							Productimg = p.ProductImage.SmallImage,
							ProductQuantity = p.ProductData.ProductStock,
							Productreviewcount = p.ProductReviews.Count(),
							Producttags = p.ProductTag.TagName,
							NumberofSold = p.ProductData.ProductSold,
							CreatedAt = p.ProductData.CreatedAt,
							isArchived = p.ProductData.IsArchived,
							isFeatured = p.ProductData.IsFeatured,
							countreviews = p.ProductReviews.Count(),
							countstars = p.ProductReviews.Select(p => p.Stars).Average().ToString("0.0"),
							CategoryIcon = p.ProductCategory.CategoryIcon,
							CategoryName = p.ProductCategory.CategoryName
						})
						.Take(limit)
						.ToListAsync();

					if (products == null || !products.Any())
					{
						return NotFound("No products found.");
					}

					return Ok(JsonConvert.SerializeObject(products)); // Directly return the products
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An error occurred while fetching products.");
				return StatusCode(500, "Internal server error.");
			}
		}

		// GET: api/Products/5
		[HttpGet("{id}")]
		public async Task<ActionResult<Product>> GetProduct(int id)
		{
			_logger.LogInformation("Getting product with ID: {id}", id);
            var product = await _context.Product
                        .Include(p => p.ProductData)
                        .Include(p => p.ProductDetails)
                        .Include(p => p.ProductImage)
                        .Include(p => p.ProductReviews)
                        .Include(p => p.ProductTag)
                        .Include(p => p.Account)
                        .Select(p => new ProductDto
                        {
                            ProductName = p.ProductDetails.ProductName,
                            ProductID = p.ProductID,
                            ProductPrice = p.ProductData.ProductPrice,
                            ProductStock = p.ProductData.ProductStock,
                            ProductDescription = p.ProductDetails.ProductDescriptionSmall,
                            Productimg = p.ProductImage.SmallImage,
                            ProductQuantity = p.ProductData.ProductStock,
                            Productreviewcount = p.ProductReviews.Count(),
                            Producttags = p.ProductTag.TagName,
                            NumberofSold = p.ProductData.ProductSold,
                            CreatedAt = p.ProductData.CreatedAt,
                            isArchived = p.ProductData.IsArchived,
                            isFeatured = p.ProductData.IsFeatured,
                            countreviews = p.ProductReviews.Count(),
                            countstars = p.ProductReviews.Select(p => p.Stars).Average().ToString("0.0"),
                            CategoryIcon = p.ProductCategory.CategoryIcon,
                            CategoryName = p.ProductCategory.CategoryName
                        })
						.FirstOrDefaultAsync(p => p.ProductID == id);

			if (product == null)
			{
				_logger.LogWarning("Product with ID: {id} not found", id);
				return NotFound("Product item Not Found");
			}

			return Ok(JsonConvert.SerializeObject(product));
		}

		// PUT: api/Products/5
		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateProduct(int id, Product product)
		{
			if (id != product.ProductID)
			{
				_logger.LogError("Product ID mismatch for update");
				return BadRequest();
			}

			_context.Entry(product).State = EntityState.Modified;

			try
			{
				await _context.SaveChangesAsync();
				_logger.LogInformation("Product with ID: {id} updated", id);
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!ProductExists(id))
				{
					_logger.LogWarning("Product with ID: {id} does not exist", id);
					return NotFound();
				}
				else
				{
					_logger.LogError("Error updating product with ID: {id}", id);
					throw;
				}
			}

			return NoContent();
		}

		// POST: api/Products
		[HttpPost]
		public async Task<ActionResult<Product>> CreateProduct(Product product)
		{
			_context.Product.Add(product);
			await _context.SaveChangesAsync();

			_logger.LogInformation("Product created with ID: {id}", product.ProductID);
			return CreatedAtAction(nameof(GetProduct), new { id = product.ProductID }, product);
		}

		// DELETE: api/Products/5
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteProduct(int id)
		{
			var product = await _context.Product.FindAsync(id);
			if (product == null)
			{
				_logger.LogWarning("Product with ID: {id} not found for deletion", id);
				return NotFound();
			}

			_context.Product.Remove(product);
			await _context.SaveChangesAsync();

			_logger.LogInformation("Product with ID: {id} deleted", id);
			return NoContent();
		}


		[HttpGet("Category")]
		public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategory(int limit)
		{
			var categoriesWithCounts = await _context.ProductCategory
				.Select(c => new CategoryDto
				{
					CategoryId = c.ProductCategoryID,
					CategoryName = c.CategoryName,
					CategoryIcon = c.CategoryIcon,
					ProductCount = c.Products.Count() // Count products in each category
				})
				.Take(limit)
				.ToListAsync();

			if (categoriesWithCounts == null || !categoriesWithCounts.Any())
			{
				_logger.LogWarning("Couldn't get product categories");
				return NotFound();
			}

			return Ok(JsonConvert.SerializeObject(categoriesWithCounts));
		}


		private bool ProductExists(int id)
		{
			return _context.Product.Any(e => e.ProductID == id);
		}
	}
}
