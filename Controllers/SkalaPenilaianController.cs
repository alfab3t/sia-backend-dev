using astratech_apps_backend.DTOs.SkalaPenilaian;
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
    public class SkalaPenilaianController(ISkalaPenilaianRepository repo) : ControllerBase
    {
        private readonly ISkalaPenilaianRepository _repo = repo;

        [HttpGet("GetAllSkalaPenilaian")]
        [RequiresPermission("skala_penilaian.view")]
        public async Task<IActionResult> GetAll([FromQuery] GetAllSkalaPenilaianRequest dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var sanitizedDto = SanitizerHelper.EncodeObject(dto);
            if (sanitizedDto == null) return NotFound();

            var (list, totalData) = await _repo.GetAllAsync(sanitizedDto);

            var dataDto = list.Select(item => new SkalaPenilaianDto
            {
                Id = item.Id,
                RowNumber = item.RowNumber,
                Skala = item.Skala,
                Definisi = item.Definisi,
                Status = item.Status
            }).ToList();

            return Ok(new GetAllSkalaPenilaianResponse
            {
                Data = dataDto,
                TotalData = totalData,
                TotalHalaman = ((totalData - 1) / dto.PageSize) + 1
            });
        }

        [HttpGet("DetailSkalaPenilaian/{id}")]
        [RequiresPermission("skala_penilaian.view")]
        public async Task<IActionResult> GetSkalaPenilaianById(int id)
        {
            var data = await _repo.GetByIdAsync(id);
            if (data == null) return NotFound();

            return Ok(new { data });
        }

        [HttpPost("CreateSkalaPenilaian")]
        [RequiresPermission("skala_penilaian.create")]
        public async Task<IActionResult> Create([FromBody] CreateSkalaPenilaianRequest dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var username = User.FindFirstValue("namaakun");
            if (string.IsNullOrEmpty(username)) return Unauthorized();

            var sanitizedDto = SanitizerHelper.EncodeObject(dto);
            if (sanitizedDto == null) return BadRequest();

            var newId = await _repo.CreateAsync(sanitizedDto, username);
            return Ok(new { message = "SUCCESS", id = newId });
        }

        [HttpPut("UpdateSkalaPenilaian")]
        [RequiresPermission("skala_penilaian.edit")]
        public async Task<IActionResult> Update([FromBody] UpdateSkalaPenilaianRequest dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var username = User.FindFirstValue("namaakun");
            if (string.IsNullOrEmpty(username)) return Unauthorized();

            var sanitizedDto = SanitizerHelper.EncodeObject(dto);
            if (sanitizedDto == null) return BadRequest();

            var success = await _repo.UpdateAsync(sanitizedDto, username);
            if (!success) return NotFound(new { message = "Data tidak ditemukan." });

            return Ok(new { message = "SUCCESS" });
        }

        [HttpPost("SetStatusSkalaPenilaian")]
        [RequiresPermission("skala_penilaian.edit")]
        public async Task<IActionResult> SetStatus([FromBody] SetStatusSkalaRequest dto)
        {
            var username = User.FindFirstValue("namaakun");
            if (string.IsNullOrEmpty(username)) return Unauthorized();

            var success = await _repo.SetStatusAsync(dto.Id, dto.Status, username);
            if (!success) return NotFound();

            return Ok(new { message = "SUCCESS" });
        }
    }
}