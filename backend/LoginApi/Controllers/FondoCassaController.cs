using LoginApi.Data;
using LoginApi.DTOs;
using LoginApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LoginApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FondoCassaController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<FondoCassaController> _logger;

        public FondoCassaController(AppDbContext context, ILogger<FondoCassaController> logger)
        {
            _context = context;
            _logger = logger;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim ?? "0");
        }

        private async Task<UserRole> GetCurrentUserRole()
        {
            var userId = GetCurrentUserId();
            var user = await _context.Users.FindAsync(userId);
            return user?.Role ?? UserRole.Reader;
        }

        private static bool CanWrite(UserRole role)
        {
            return role == UserRole.Writer || role == UserRole.Admin;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FondoCassaDto>>> GetAll([FromQuery] int? idStore = null, [FromQuery] string? cashCode = null)
        {
            try
            {
                var query = _context.CashFund.AsQueryable();

                if (idStore.HasValue)
                {
                    query = query.Where(b => b.IdStore == idStore.Value);
                }

                if (!string.IsNullOrEmpty(cashCode))
                {
                    query = query.Where(b => b.CashCode == cashCode);
                }

                var items = await query
                    .OrderByDescending(b => b.CountingDate)
                    .Select(b => new FondoCassaDto
                    {
                        Id = b.Id,
                        IdStore = b.IdStore,
                        CashCode = b.CashCode,
                        CountingDate = b.CountingDate,
                        CountingAmount = b.CountingAmount,
                        CountingCoins = b.CountingCoins,
                        CountingAt = b.CountingAt,
                        CountingBy = b.CountingBy
                    })
                    .ToListAsync();

                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving fondo cassa data");
                return StatusCode(500, "Error retrieving data");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FondoCassaDto>> GetById(int id)
        {
            try
            {
                var item = await _context.CashFund.FindAsync(id);

                if (item == null)
                {
                    return NotFound();
                }

                var dto = new FondoCassaDto
                {
                    Id = item.Id,
                    IdStore = item.IdStore,
                    CashCode = item.CashCode,
                    CountingDate = item.CountingDate,
                    CountingAmount = item.CountingAmount,
                    CountingCoins = item.CountingCoins,
                    CountingAt = item.CountingAt,
                    CountingBy = item.CountingBy
                };

                return Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving fondo cassa with id {Id}", id);
                return StatusCode(500, "Error retrieving data");
            }
        }

        [HttpPost]
        public async Task<ActionResult<FondoCassaDto>> Create(CreateFondoCassaDto dto)
        {
            try
            {
                var userRole = await GetCurrentUserRole();
                if (!CanWrite(userRole))
                {
                    return Forbid(); // 403 Forbidden for Reader users
                }

                var item = new CashFund
                {
                    IdStore = dto.IdStore,
                    CashCode = dto.CashCode,
                    CountingDate = dto.CountingDate,
                    CountingAmount = dto.CountingAmount,
                    CountingCoins = dto.CountingCoins,
                    CountingAt = DateTime.UtcNow,
                    CountingBy = User.Identity?.Name
                };

                _context.CashFund.Add(item);
                await _context.SaveChangesAsync();

                var result = new FondoCassaDto
                {
                    Id = item.Id,
                    IdStore = item.IdStore,
                    CashCode = item.CashCode,
                    CountingDate = item.CountingDate,
                    CountingAmount = item.CountingAmount,
                    CountingCoins = item.CountingCoins,
                    CountingAt = item.CountingAt,
                    CountingBy = item.CountingBy
                };

                return CreatedAtAction(nameof(GetById), new { id = item.Id }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating fondo cassa");
                return StatusCode(500, "Error creating data");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateFondoCassaDto dto)
        {
            try
            {
                var userRole = await GetCurrentUserRole();
                if (!CanWrite(userRole))
                {
                    return Forbid(); // 403 Forbidden for Reader users
                }

                var item = await _context.CashFund.FindAsync(id);

                if (item == null)
                {
                    return NotFound();
                }

                item.CashCode = dto.CashCode;
                item.CountingDate = dto.CountingDate;
                item.CountingAmount = dto.CountingAmount;
                item.CountingCoins = dto.CountingCoins;

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating fondo cassa with id {Id}", id);
                return StatusCode(500, "Error updating data");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var userRole = await GetCurrentUserRole();
                if (!CanWrite(userRole))
                {
                    return Forbid(); // 403 Forbidden for Reader users
                }

                var item = await _context.CashFund.FindAsync(id);

                if (item == null)
                {
                    return NotFound();
                }

                _context.CashFund.Remove(item);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting fondo cassa with id {Id}", id);
                return StatusCode(500, "Error deleting data");
            }
        }
    }
}
