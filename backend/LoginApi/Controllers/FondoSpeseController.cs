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
    public class FondoSpeseController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<FondoSpeseController> _logger;

        public FondoSpeseController(AppDbContext context, ILogger<FondoSpeseController> logger)
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
        public async Task<ActionResult<IEnumerable<FondoSpeseDto>>> GetAll([FromQuery] int? idStore = null)
        {
            try
            {
                var query = _context.ExpenseFund.AsQueryable();

                if (idStore.HasValue)
                {
                    query = query.Where(b => b.IdStore == idStore.Value);
                }

                var items = await query
                    .OrderByDescending(b => b.CountingDate)
                    .Select(b => new FondoSpeseDto
                    {
                        Id = b.Id,
                        IdStore = b.IdStore,
                        CountingDate = b.CountingDate,
                        ExpenseType = b.ExpenseType,
                        CountingAmount = b.CountingAmount,
                        CountingCoins = b.CountingCoins,
                        InvoiceDate = b.InvoiceDate,
                        InvoiceNumber = b.InvoiceNumber,
                        ReasonExpenses = b.ReasonExpenses,
                        CountingAt = b.CountingAt,
                        CountingBy = b.CountingBy
                    })
                    .ToListAsync();

                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving fondo spese data");
                return StatusCode(500, "Error retrieving data");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FondoSpeseDto>> GetById(int id)
        {
            try
            {
                var item = await _context.ExpenseFund.FindAsync(id);

                if (item == null)
                {
                    return NotFound();
                }

                var dto = new FondoSpeseDto
                {
                    Id = item.Id,
                    IdStore = item.IdStore,
                    CountingDate = item.CountingDate,
                    ExpenseType = item.ExpenseType,
                    CountingAmount = item.CountingAmount,
                    CountingCoins = item.CountingCoins,
                    InvoiceDate = item.InvoiceDate,
                    InvoiceNumber = item.InvoiceNumber,
                    ReasonExpenses = item.ReasonExpenses,
                    CountingAt = item.CountingAt,
                    CountingBy = item.CountingBy
                };

                return Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving fondo spese with id {Id}", id);
                return StatusCode(500, "Error retrieving data");
            }
        }

        [HttpPost]
        public async Task<ActionResult<FondoSpeseDto>> Create(CreateFondoSpeseDto dto)
        {
            try
            {
                var userRole = await GetCurrentUserRole();
                if (!CanWrite(userRole))
                {
                    return Forbid(); // 403 Forbidden for Reader users
                }

                var item = new ExpenseFund
                {
                    IdStore = dto.IdStore,
                    CountingDate = dto.CountingDate,
                    ExpenseType = dto.ExpenseType,
                    CountingAmount = dto.CountingAmount,
                    CountingCoins = dto.CountingCoins,
                    InvoiceDate = dto.InvoiceDate,
                    InvoiceNumber = dto.InvoiceNumber,
                    ReasonExpenses = dto.ReasonExpenses,
                    CountingAt = DateTime.UtcNow,
                    CountingBy = User.Identity?.Name
                };

                _context.ExpenseFund.Add(item);
                await _context.SaveChangesAsync();

                var result = new FondoSpeseDto
                {
                    Id = item.Id,
                    IdStore = item.IdStore,
                    CountingDate = item.CountingDate,
                    ExpenseType = item.ExpenseType,
                    CountingAmount = item.CountingAmount,
                    CountingCoins = item.CountingCoins,
                    InvoiceDate = item.InvoiceDate,
                    InvoiceNumber = item.InvoiceNumber,
                    ReasonExpenses = item.ReasonExpenses,
                    CountingAt = item.CountingAt,
                    CountingBy = item.CountingBy
                };

                return CreatedAtAction(nameof(GetById), new { id = item.Id }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating fondo spese");
                return StatusCode(500, "Error creating data");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateFondoSpeseDto dto)
        {
            try
            {
                var userRole = await GetCurrentUserRole();
                if (!CanWrite(userRole))
                {
                    return Forbid(); // 403 Forbidden for Reader users
                }

                var item = await _context.ExpenseFund.FindAsync(id);

                if (item == null)
                {
                    return NotFound();
                }

                item.CountingDate = dto.CountingDate;
                item.ExpenseType = dto.ExpenseType;
                item.CountingAmount = dto.CountingAmount;
                item.CountingCoins = dto.CountingCoins;
                item.InvoiceDate = dto.InvoiceDate;
                item.InvoiceNumber = dto.InvoiceNumber;
                item.ReasonExpenses = dto.ReasonExpenses;

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating fondo spese with id {Id}", id);
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

                var item = await _context.ExpenseFund.FindAsync(id);

                if (item == null)
                {
                    return NotFound();
                }

                _context.ExpenseFund.Remove(item);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting fondo spese with id {Id}", id);
                return StatusCode(500, "Error deleting data");
            }
        }
    }
}
