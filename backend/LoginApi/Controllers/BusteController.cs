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
    public class BusteController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BusteController(AppDbContext context)
        {
            _context = context;
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

        private bool CanWrite(UserRole role)
        {
            return role == UserRole.Writer || role == UserRole.Admin;
        }

        // GET: api/buste
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BustaDto>>> GetBuste(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var userId = GetCurrentUserId();
            var query = _context.Buste.Where(b => b.UserId == userId);

            if (startDate.HasValue)
            {
                query = query.Where(b => b.DataRiferimento >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(b => b.DataRiferimento <= endDate.Value);
            }

            var buste = await query
                .OrderByDescending(b => b.DataRiferimento)
                .Select(b => new BustaDto
                {
                    Id = b.Id,
                    DataRiferimento = b.DataRiferimento,
                    DataChiusura = b.DataChiusura,
                    DataRitiro = b.DataRitiro,
                    Sigillo = b.Sigillo,
                    Totale = b.Totale,
                    Note = b.Note,
                    UserChiusura = b.UserChiusura,
                    UserRitiro = b.UserRitiro,
                    CreatedAt = b.CreatedAt,
                    UpdatedAt = b.UpdatedAt,
                    UserId = b.UserId
                })
                .ToListAsync();

            return Ok(buste);
        }

        // GET: api/buste/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BustaDto>> GetBusta(int id)
        {
            var userId = GetCurrentUserId();
            var busta = await _context.Buste
                .Where(b => b.Id == id && b.UserId == userId)
                .Select(b => new BustaDto
                {
                    Id = b.Id,
                    DataRiferimento = b.DataRiferimento,
                    DataChiusura = b.DataChiusura,
                    DataRitiro = b.DataRitiro,
                    Sigillo = b.Sigillo,
                    Totale = b.Totale,
                    Note = b.Note,
                    UserChiusura = b.UserChiusura,
                    UserRitiro = b.UserRitiro,
                    CreatedAt = b.CreatedAt,
                    UpdatedAt = b.UpdatedAt,
                    UserId = b.UserId
                })
                .FirstOrDefaultAsync();

            if (busta == null)
            {
                return NotFound(new { message = "Busta not found" });
            }

            return Ok(busta);
        }

        // POST: api/buste
        [HttpPost]
        public async Task<ActionResult<BustaDto>> CreateBusta(CreateBustaDto createDto)
        {
            var userRole = await GetCurrentUserRole();
            if (!CanWrite(userRole))
            {
                return Forbid(); // 403 Forbidden for Reader users
            }

            var userId = GetCurrentUserId();
            var busta = new Busta
            {
                DataRiferimento = createDto.DataRiferimento,
                DataChiusura = createDto.DataChiusura,
                DataRitiro = createDto.DataRitiro,
                Sigillo = createDto.Sigillo,
                Totale = createDto.Totale,
                Note = createDto.Note,
                UserChiusura = createDto.UserChiusura,
                UserRitiro = createDto.UserRitiro,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Buste.Add(busta);
            await _context.SaveChangesAsync();

            var bustaDto = new BustaDto
            {
                Id = busta.Id,
                DataRiferimento = busta.DataRiferimento,
                DataChiusura = busta.DataChiusura,
                DataRitiro = busta.DataRitiro,
                Sigillo = busta.Sigillo,
                Totale = busta.Totale,
                Note = busta.Note,
                UserChiusura = busta.UserChiusura,
                UserRitiro = busta.UserRitiro,
                CreatedAt = busta.CreatedAt,
                UpdatedAt = busta.UpdatedAt,
                UserId = busta.UserId
            };

            return CreatedAtAction(nameof(GetBusta), new { id = busta.Id }, bustaDto);
        }

        // PUT: api/buste/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBusta(int id, UpdateBustaDto updateDto)
        {
            var userRole = await GetCurrentUserRole();
            if (!CanWrite(userRole))
            {
                return Forbid(); // 403 Forbidden for Reader users
            }

            var userId = GetCurrentUserId();
            var busta = await _context.Buste
                .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

            if (busta == null)
            {
                return NotFound(new { message = "Busta not found" });
            }

            if (updateDto.DataRiferimento.HasValue)
                busta.DataRiferimento = updateDto.DataRiferimento.Value;
            
            if (updateDto.DataChiusura.HasValue)
                busta.DataChiusura = updateDto.DataChiusura;
            
            if (updateDto.DataRitiro.HasValue)
                busta.DataRitiro = updateDto.DataRitiro;
            
            if (updateDto.Sigillo != null)
                busta.Sigillo = updateDto.Sigillo;
            
            if (updateDto.Totale.HasValue)
                busta.Totale = updateDto.Totale.Value;
            
            if (updateDto.Note != null)
                busta.Note = updateDto.Note;
            
            if (updateDto.UserChiusura != null)
                busta.UserChiusura = updateDto.UserChiusura;
            
            if (updateDto.UserRitiro != null)
                busta.UserRitiro = updateDto.UserRitiro;

            busta.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/buste/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBusta(int id)
        {
            var userRole = await GetCurrentUserRole();
            if (!CanWrite(userRole))
            {
                return Forbid(); // 403 Forbidden for Reader users
            }

            var userId = GetCurrentUserId();
            var busta = await _context.Buste
                .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

            if (busta == null)
            {
                return NotFound(new { message = "Busta not found" });
            }

            _context.Buste.Remove(busta);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/buste/{id}/duplicate
        [HttpPost("{id}/duplicate")]
        public async Task<ActionResult<BustaDto>> DuplicateBusta(int id)
        {
            var userRole = await GetCurrentUserRole();
            if (!CanWrite(userRole))
            {
                return Forbid(); // 403 Forbidden for Reader users
            }

            var userId = GetCurrentUserId();
            var originalBusta = await _context.Buste
                .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

            if (originalBusta == null)
            {
                return NotFound(new { message = "Busta not found" });
            }

            var newBusta = new Busta
            {
                DataRiferimento = DateTime.UtcNow,
                DataChiusura = null,
                DataRitiro = null,
                Sigillo = originalBusta.Sigillo,
                Totale = originalBusta.Totale,
                Note = originalBusta.Note,
                UserChiusura = null,
                UserRitiro = null,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Buste.Add(newBusta);
            await _context.SaveChangesAsync();

            var bustaDto = new BustaDto
            {
                Id = newBusta.Id,
                DataRiferimento = newBusta.DataRiferimento,
                DataChiusura = newBusta.DataChiusura,
                DataRitiro = newBusta.DataRitiro,
                Sigillo = newBusta.Sigillo,
                Totale = newBusta.Totale,
                Note = newBusta.Note,
                UserChiusura = newBusta.UserChiusura,
                UserRitiro = newBusta.UserRitiro,
                CreatedAt = newBusta.CreatedAt,
                UpdatedAt = newBusta.UpdatedAt,
                UserId = newBusta.UserId
            };

            return CreatedAtAction(nameof(GetBusta), new { id = newBusta.Id }, bustaDto);
        }
    }
}
