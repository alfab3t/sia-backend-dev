using astratech_apps_backend.DTOs.JenisSurat;
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
    public class JenisSuratController(IJenisSuratRepository repo) : ControllerBase
    {
        private readonly IJenisSuratRepository _repo = repo;
        [HttpGet("GetAllJenisSurat")]
        [RequiresPermission("jenis_surat.view")] 
        public async Task<IActionResult> GetAllJenisSurat([FromQuery] GetAllJenisSuratRequest dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var sanitizedDto = SanitizerHelper.EncodeObject(dto);
            if (sanitizedDto == null) return NotFound();

            var (list, totalData) = await _repo.GetAllAsync(sanitizedDto);

            var dataDTO = list.Select(m => new JenisSuratDto
            {
                Id = m.Id,
                NamaSurat = m.NamaSurat,
                FormatNoSurat = m.FormatNoSurat,
                FormatSurat = m.FormatSurat,
                Status = m.Status,
                AllowMahasiswaDisplay = m.AllowMahasiswa == 1 ? "Ya" : "Tidak"
            }).ToList();

            var response = new GetAllJenisSuratResponse
            {
                Data = dataDTO,
                TotalData = totalData,
                TotalHalaman = ((totalData - 1) / dto.PageSize) + 1
            };

            return Ok(response);
        }

        [HttpGet("DetailJenisSurat/{id}")]
        [RequiresPermission("jenis_surat.view")]
        public async Task<IActionResult> GetJenisSuratById(int id)
        {
            var data = await _repo.GetByIdAsync(id);
            if (data == null)
            {
                return NotFound(new { message = "Data jenis surat tidak ditemukan." });
            }

            return Ok(new
            {
                data.Id,
                data.NamaSurat,
                data.FormatNoSurat,
                data.FormatSurat,
                data.AllowMahasiswa
            });
        }

        [HttpPut("EditJenisSurat")]
        [RequiresPermission("jenis_surat.edit")]
        public async Task<IActionResult> UpdateJenisSurat([FromBody] UpdateJenisSuratRequest dto)
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
            if (sanitizedDto == null) return BadRequest(new { message = "Data tidak valid" });

            var success = await _repo.UpdateAsync(sanitizedDto, username);
            if (!success)
            {
                return NotFound(new { message = "Gagal mengubah data. Id tidak ditemukan" });
            }

            return Ok(new { message = "SUCCESS" });
        }

        [HttpPost("SetStatusJenisSurat/{id}")]
        [RequiresPermission("jenis_surat.edit")]
        public async Task<IActionResult> SetStatusJenisSurat(int id)
        {
            var username = User.FindFirstValue("namaakun");
            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized();
            }

            var success = await _repo.SetStatusAsync(id, username);

            if (!success)
            {
                return NotFound(new { message = "Data jenis surat tidak ditemukan." });
            }

            return Ok(new { message = "SUCCESS" });
        }
    }
}