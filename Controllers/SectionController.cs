using astratech_apps_backend.DTOs.Section;
using astratech_apps_backend.Helpers;
using astratech_apps_backend.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace astratech_apps_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SectionController(ISectionRepository repo) : ControllerBase
    {
        private readonly ISectionRepository _repo = repo;

        [HttpGet("GetAllSection")]
        [RequiresPermission("section.view")]
        public async Task<IActionResult> GetAllSection([FromQuery] GetAllSectionRequest dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var sanitizedDto = SanitizerHelper.EncodeObject(dto);
            if (sanitizedDto == null) return NotFound();

            var (list, totalData) = await _repo.GetAllAsync(sanitizedDto);

            var dataDTO = list.Select(m => new SectionDto
            {
                Id = m.Id,
                NamaSection = m.NamaSection,
                Status = m.Status
            }).ToList();

            var response = new GetAllSectionResponse
            {
                Data = dataDTO,
                TotalData = totalData,
                TotalHalaman = ((totalData - 1) / dto.PageSize) + 1
            };

            return Ok(response);
        }

        [HttpPost("CreateSection")]
        [RequiresPermission("section.create")]
        public async Task<IActionResult> CreateSection([FromBody] CreateSectionRequest dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var username = User.FindFirstValue("namaakun");

            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized();
            }

            var sanitizedDto = SanitizerHelper.EncodeObject(dto);
            if (sanitizedDto == null) return NotFound();

            var newId = await _repo.CreateAsync(dto, username);

            return Ok(new { message = "SUCCESS", id = newId });
        }

        [HttpPut("EditSection")]
        [RequiresPermission("section.edit")]
        public async Task<IActionResult> UpdateSection([FromBody] UpdateSectionRequest dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var username = User.FindFirstValue("namaakun");

            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized();
            }

            var sanitizedDto = SanitizerHelper.EncodeObject(dto);
            if (sanitizedDto == null) return NotFound();

            var success = await _repo.UpdateAsync(dto, username);
            if (!success)
            {
                return NotFound(new { message = "Data Section tidak ditemukan." });
            }
            return Ok(new { message = "SUCCESS" });
        }

        [HttpPost("SetStatusSection/{id}")]
        [RequiresPermission("section.edit")]
        public async Task<IActionResult> SetStatusSection(int id)
        {
            var username = User.FindFirstValue("namaakun");

            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized();
            }

            var success = await _repo.SetStatusAsync(id, username);
            if (!success)
            {
                return NotFound(new { message = "Data section tidak ditemukan." });
            }
            return Ok(new { message = "SUCCESS" });
        }

        [HttpGet("DetailSection/{id}")]
        [RequiresPermission("section.view")]
        public async Task<IActionResult> GetSectionById(int id)
        {
            var dataDto = await _repo.GetByIdAsync(id);
            if (dataDto == null)
            {
                return NotFound(new { message = "Data section tidak ditemukan." });
            }
            return Ok(dataDto);
        }
    }
}