using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Webapi.shared;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Cors;
using Newtonsoft.Json;

namespace Webapi.Controllers
{
	[Route("api/[controller]")]
    [ApiController]
	public class Reviews : ControllerBase
	{
		private readonly Context _context;
		private readonly ILogger<Reviews> _logger;

		public Reviews(Context context, ILogger<Reviews> logger)
		{
			_context = context;
			_logger = logger;
		}

		// GET: api/Reviews
		[HttpGet]
		public async Task<ActionResult<IEnumerable<Review>>> GetReviews()
		{
			_logger.LogInformation("Getting all reviews");
			var review = await _context.Review.Include(r => r.Account).ToListAsync();
			return review == null ? NotFound() : Ok(JsonConvert.SerializeObject(review));
		}

		// GET: api/Reviews/5
		[HttpGet("{id}")]
		public async Task<ActionResult<Review>> GetReview(int id)
		{
			_logger.LogInformation("Getting review with ID: {id}", id);
			var review = await _context.Review.Include(r => r.Account)
				.FirstOrDefaultAsync(r => r.ReviewID == id);

			if (review == null)
			{
				_logger.LogWarning("Review with ID: {id} not found", id);
				return NotFound();
			}

			return Ok(JsonConvert.SerializeObject(review));
		}

		// PUT: api/Reviews/5
		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateReview(int id, Review review)
		{
			if (id != review.ReviewID)
			{
				_logger.LogError("Review ID mismatch for update");
				return BadRequest();
			}

			_context.Entry(review).State = EntityState.Modified;

			try
			{
				await _context.SaveChangesAsync();
				_logger.LogInformation("Review with ID: {id} updated", id);
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!ReviewExists(id))
				{
					_logger.LogWarning("Review with ID: {id} does not exist", id);
					return NotFound();
				}
				else
				{
					_logger.LogError("Error updating review with ID: {id}", id);
					throw;
				}
			}

			return NoContent();
		}

		// POST: api/Reviews
		[HttpPost]
		public async Task<ActionResult<Review>> CreateReview(Review review)
		{
			_context.Review.Add(review);
			await _context.SaveChangesAsync();

			_logger.LogInformation("Review created with ID: {id}", review.ReviewID);
			return CreatedAtAction(nameof(GetReview), new { id = review.ReviewID }, review);
		}

		// DELETE: api/Reviews/5
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteReview(int id)
		{
			var review = await _context.Review.FindAsync(id);
			if (review == null)
			{
				_logger.LogWarning("Review with ID: {id} not found for deletion", id);
				return NotFound();
			}

			_context.Review.Remove(review);
			await _context.SaveChangesAsync();

			_logger.LogInformation("Review with ID: {id} deleted", id);
			return NoContent();
		}

		private bool ReviewExists(int id)
		{
			return _context.Review.Any(e => e.ReviewID == id);
		}
	}
}
