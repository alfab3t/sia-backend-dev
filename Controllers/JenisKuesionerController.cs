using astratech_apps_backend.DTOs.JenisKuesioner;
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
    public class JenisKuesionerController(IJenisKuesionerRepository repo) : ControllerBase
    {
        private readonly IJenisKuesionerRepository _repo = repo;

        [HttpGet("GetAllJenisKuesioner")]
        [RequiresPermission("jenis_kuesioner.view")]
        public async Task<IActionResult> GetAll([FromQuery] GetAllJenisKuesionerRequest dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var sanitizedDto = SanitizerHelper.EncodeObject(dto);
            if (sanitizedDto == null) return NotFound();

            var (list, totalData) = await _repo.GetAllAsync(sanitizedDto);
            var dataDto = list.Select(m => new JenisKuesionerDto
            {
                Id = m.Id,
                RowNumber = m.RowNumber,
                NamaJenis = m.NamaJenis,
                Status = m.Status
            }).ToList();

            return Ok(new GetAllJenisKuesionerResponse
            {
                Data = dataDto,
                TotalData = totalData,
                TotalHalaman = ((totalData - 1) / dto.PageSize) + 1
            });
        }

        [HttpGet("DetailJenisKuesioner/{id}")]
        [RequiresPermission("jenis_kuesioner.view")]
        public async Task<IActionResult> GetJenisKuesionerById(int id)
        {
            var data = await _repo.GetByIdAsync(id);
            if (data == null) return NotFound();

            return Ok(new { data });
        }

        [HttpPost("CreateJenisKuesioner")]
        [RequiresPermission("jenis_kuesioner.create")]
        public async Task<IActionResult> Create([FromBody] CreateJenisKuesionerRequest dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var username = User.FindFirstValue("namaakun");
            if (string.IsNullOrEmpty(username)) return Unauthorized();

            var sanitizedDto = SanitizerHelper.EncodeObject(dto);
            if (sanitizedDto == null) return BadRequest();

            var newId = await _repo.CreateAsync(sanitizedDto, username);
            return Ok(new { message = "SUCCESS", id = newId });
        }

        [HttpPut("UpdateJenisKuesioner")]
        [RequiresPermission("jenis_kuesioner.edit")]
        public async Task<IActionResult> Update([FromBody] UpdateJenisKuesionerRequest dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var username = User.FindFirstValue("namaakun");
            if (string.IsNullOrEmpty(username)) return Unauthorized();

            var sanitizedDto = SanitizerHelper.EncodeObject(dto);
            if (sanitizedDto == null) return BadRequest();

            var success = await _repo.UpdateAsync(sanitizedDto, username);
            if (!success) return NotFound(new { message = "Data not found." });

            return Ok(new { message = "SUCCESS" });
        }

        [HttpPost("SetStatusJenisKuesioner")]
        [RequiresPermission("skala_penilaian.edit")]
        public async Task<IActionResult> SetStatus([FromBody] SetStatusJenisRequest dto)
        { 
            var username = User.FindFirstValue("namaakun");
            if (string.IsNullOrEmpty(username)) return Unauthorized();

            var success = await _repo.SetStatusAsync(dto.Id, dto.Status, username);
            if (!success) return NotFound();

            return Ok(new { message = "SUCCESS" });
        }
    }
}