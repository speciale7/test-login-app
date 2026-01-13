using LoginApi.Data;
using LoginApi.DTOs;
using LoginApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LoginApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SovvMonetariaController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<SovvMonetariaController> _logger;

        public SovvMonetariaController(AppDbContext context, ILogger<SovvMonetariaController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SovvMonetariaDto>>> GetAll([FromQuery] int? idStore = null)
        {
            try
            {
                var query = _context.MonetaryFund.AsQueryable();

                if (idStore.HasValue)
                {
                    query = query.Where(b => b.IdStore == idStore.Value);
                }

                var items = await query
                    .OrderByDescending(b => b.CountingDate)
                    .Select(b => new SovvMonetariaDto
                    {
                        Id = b.Id,
                        IdStore = b.IdStore,
                        CountingDate = b.CountingDate,
                        CountingAmount = b.CountingAmount,
                        CountingNote = b.CountingNote,
                        CountingCoins = b.CountingCoins,
                        CountingAt = b.CountingAt,
                        CountingBy = b.CountingBy
                    })
                    .ToListAsync();

                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving sovv monetaria data");
                return StatusCode(500, "Error retrieving data");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SovvMonetariaDto>> GetById(int id)
        {
            try
            {
                var item = await _context.MonetaryFund.FindAsync(id);

                if (item == null)
                {
                    return NotFound();
                }

                var dto = new SovvMonetariaDto
                {
                    Id = item.Id,
                    IdStore = item.IdStore,
                    CountingDate = item.CountingDate,
                    CountingAmount = item.CountingAmount,
                    CountingNote = item.CountingNote,
                    CountingCoins = item.CountingCoins,
                    CountingAt = item.CountingAt,
                    CountingBy = item.CountingBy
                };

                return Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving sovv monetaria with id {Id}", id);
                return StatusCode(500, "Error retrieving data");
            }
        }

        [HttpPost]
        public async Task<ActionResult<SovvMonetariaDto>> Create(CreateSovvMonetariaDto dto)
        {
            try
            {
                var item = new MonetaryFund
                {
                    IdStore = dto.IdStore,
                    CountingDate = dto.CountingDate,
                    CountingAmount = dto.CountingAmount,
                    CountingNote = dto.CountingNote,
                    CountingCoins = dto.CountingCoins,
                    CountingAt = DateTime.UtcNow,
                    CountingBy = User.Identity?.Name
                };

                _context.MonetaryFund.Add(item);
                await _context.SaveChangesAsync();

                var result = new SovvMonetariaDto
                {
                    Id = item.Id,
                    IdStore = item.IdStore,
                    CountingDate = item.CountingDate,
                    CountingAmount = item.CountingAmount,
                    CountingNote = item.CountingNote,
                    CountingCoins = item.CountingCoins,
                    CountingAt = item.CountingAt,
                    CountingBy = item.CountingBy
                };

                return CreatedAtAction(nameof(GetById), new { id = item.Id }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating sovv monetaria");
                return StatusCode(500, "Error creating data");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateSovvMonetariaDto dto)
        {
            try
            {
                var item = await _context.MonetaryFund.FindAsync(id);

                if (item == null)
                {
                    return NotFound();
                }

                item.CountingDate = dto.CountingDate;
                item.CountingAmount = dto.CountingAmount;
                item.CountingNote = dto.CountingNote;
                item.CountingCoins = dto.CountingCoins;

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating sovv monetaria with id {Id}", id);
                return StatusCode(500, "Error updating data");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var item = await _context.MonetaryFund.FindAsync(id);

                if (item == null)
                {
                    return NotFound();
                }

                _context.MonetaryFund.Remove(item);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting sovv monetaria with id {Id}", id);
                return StatusCode(500, "Error deleting data");
            }
        }
    }
}
