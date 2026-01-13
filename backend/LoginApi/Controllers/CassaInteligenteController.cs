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
    public class CassaInteligenteController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<CassaInteligenteController> _logger;

        public CassaInteligenteController(AppDbContext context, ILogger<CassaInteligenteController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CassaInteligenteDto>>> GetAll([FromQuery] int? idStore = null)
        {
            try
            {
                var query = _context.BanconoteWithdrawalAutomatic.AsQueryable();

                if (idStore.HasValue)
                {
                    query = query.Where(b => b.IdStore == idStore.Value);
                }

                var items = await query
                    .OrderByDescending(b => b.CountingDate)
                    .Select(b => new CassaInteligenteDto
                    {
                        Id = b.Id,
                        IdStore = b.IdStore,
                        StoreAlias = b.StoreAlias,
                        CountingDate = b.CountingDate,
                        SecurityEnvelopeCode = b.SecurityEnvelopeCode,
                        CountingAmount = b.CountingAmount,
                        CountingDifference = b.CountingDifference,
                        CountingCoins = b.CountingCoins,
                        WithdrawalDate = b.WithdrawalDate,
                        CountingCourierDate = b.CountingCourierDate,
                        ImportDate = b.ImportDate,
                        CountingAt = b.CountingAt,
                        CountingBy = b.CountingBy
                    })
                    .ToListAsync();

                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving cassa intelligente data");
                return StatusCode(500, "Error retrieving data");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CassaInteligenteDto>> GetById(int id)
        {
            try
            {
                var item = await _context.BanconoteWithdrawalAutomatic.FindAsync(id);

                if (item == null)
                {
                    return NotFound();
                }

                var dto = new CassaInteligenteDto
                {
                    Id = item.Id,
                    IdStore = item.IdStore,
                    StoreAlias = item.StoreAlias,
                    CountingDate = item.CountingDate,
                    SecurityEnvelopeCode = item.SecurityEnvelopeCode,
                    CountingAmount = item.CountingAmount,
                    CountingDifference = item.CountingDifference,
                    CountingCoins = item.CountingCoins,
                    WithdrawalDate = item.WithdrawalDate,
                    CountingCourierDate = item.CountingCourierDate,
                    ImportDate = item.ImportDate,
                    CountingAt = item.CountingAt,
                    CountingBy = item.CountingBy
                };

                return Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving cassa intelligente with id {Id}", id);
                return StatusCode(500, "Error retrieving data");
            }
        }

        [HttpPost]
        public async Task<ActionResult<CassaInteligenteDto>> Create(CreateCassaInteligenteDto dto)
        {
            try
            {
                var item = new BanconoteWithdrawalAutomatic
                {
                    IdStore = dto.IdStore,
                    StoreAlias = dto.StoreAlias,
                    CountingDate = dto.CountingDate,
                    SecurityEnvelopeCode = dto.SecurityEnvelopeCode,
                    CountingAmount = dto.CountingAmount,
                    CountingDifference = dto.CountingDifference,
                    CountingCoins = dto.CountingCoins,
                    WithdrawalDate = dto.WithdrawalDate,
                    CountingAt = DateTime.UtcNow,
                    CountingBy = User.Identity?.Name
                };

                _context.BanconoteWithdrawalAutomatic.Add(item);
                await _context.SaveChangesAsync();

                var result = new CassaInteligenteDto
                {
                    Id = item.Id,
                    IdStore = item.IdStore,
                    StoreAlias = item.StoreAlias,
                    CountingDate = item.CountingDate,
                    SecurityEnvelopeCode = item.SecurityEnvelopeCode,
                    CountingAmount = item.CountingAmount,
                    CountingDifference = item.CountingDifference,
                    CountingCoins = item.CountingCoins,
                    WithdrawalDate = item.WithdrawalDate,
                    CountingAt = item.CountingAt,
                    CountingBy = item.CountingBy
                };

                return CreatedAtAction(nameof(GetById), new { id = item.Id }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating cassa intelligente");
                return StatusCode(500, "Error creating data");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateCassaInteligenteDto dto)
        {
            try
            {
                var item = await _context.BanconoteWithdrawalAutomatic.FindAsync(id);

                if (item == null)
                {
                    return NotFound();
                }

                item.StoreAlias = dto.StoreAlias;
                item.CountingDate = dto.CountingDate;
                item.SecurityEnvelopeCode = dto.SecurityEnvelopeCode;
                item.CountingAmount = dto.CountingAmount;
                item.CountingDifference = dto.CountingDifference;
                item.CountingCoins = dto.CountingCoins;
                item.WithdrawalDate = dto.WithdrawalDate;

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating cassa intelligente with id {Id}", id);
                return StatusCode(500, "Error updating data");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var item = await _context.BanconoteWithdrawalAutomatic.FindAsync(id);

                if (item == null)
                {
                    return NotFound();
                }

                _context.BanconoteWithdrawalAutomatic.Remove(item);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting cassa intelligente with id {Id}", id);
                return StatusCode(500, "Error deleting data");
            }
        }
    }
}
