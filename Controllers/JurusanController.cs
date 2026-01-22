
using astratech_apps_backend.DTOs.Jurusan;
using astratech_apps_backend.Helpers;
using astratech_apps_backend.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Security.Claims;

namespace astratech_apps_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class JurusanController(IJurusanRepository repo) : ControllerBase
    {
        private readonly IJurusanRepository _repo = repo;

        [HttpGet("GetAllJurusan")]
        [RequiresPermission("jurusan.view")]
                    
        public async Task<IActionResult> GetAllJurusan([FromQuery] GetAllJurusanRequest dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var sanitizedDto = SanitizerHelper.EncodeObject(dto);
            if (sanitizedDto == null) return NotFound();
            var (list, totalData) = await _repo.GetAllAsync(sanitizedDto);
            var dataDTO = list.Select(jurusan => new JurusanDto
            {
                Id = jurusan.Id,
                NamaJurusan = jurusan.NamaJurusan,
                NamaKepalaJurusan = jurusan.NamaKepalaJurusan,
                NoNPK = jurusan.NoNPK,
                Status = jurusan.Status,
                JumlahProdi = jurusan.JumlahProdi
            }).ToList();
            var response = new GetAllJurusanResponse
            {
                Data = dataDTO,
                TotalData = totalData,
                TotalHalaman = ((totalData - 1) / dto.PageSize) + 1
            };
            return Ok(response);
        }

        [HttpGet("GetListKaryawan")]
        [RequiresPermission("jurusan.view")]
        public async Task<IActionResult> GetListKaryawan()
        {
            try
            {
                var karyawanList = await _repo.GetListKaryawanAsync();

                var response = karyawanList.Select(karyawan => new
                {
                    KaryawanId = karyawan.KaryawanId,
                    KaryawanNama = karyawan.KaryawanNama,
                    Nama = karyawan.Nama
                });

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Gagal mengambil data karyawan", error = ex.Message });
            }
        }

        [HttpGet("DetailJurusan/{id}")]
        [RequiresPermission("jurusan.view")]
        public async Task<IActionResult> GetJurusanById(short id)
        {
            var dataDto = await _repo.GetByIdAsync(id);
            if (dataDto == null)
            {
                return NotFound(new { message = "Data Jurusan tidak ditemukan." });
            }
            return Ok(dataDto);
        }

        [HttpGet("GetListJurusan")]
        [RequiresPermission("jurusan.view")]
        public async Task<IActionResult> GetListJurusan()
        {
            var result = await _repo.GetListJurusanAsync();

            var response = result.Select(jurusan => new
            {
                id = jurusan.Id,
                namaJurusan = jurusan.NamaJurusan
            });

            return Ok(response);
        }

        [HttpPost("CreateJurusan")]
        [RequiresPermission("jurusan.create")]
        public async Task<IActionResult> CreateJurusan([FromBody] CreateJurusanRequest dto)
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

        [HttpPut("EditJurusan")]
        [RequiresPermission("jurusan.edit")]

        public async Task<IActionResult> UpdateJurusan([FromBody] UpdateJurusanRequest dto)
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
                return NotFound(new { message = "Data Jurusan tidak ditemukan." });
            }
            return Ok(new { message = "SUCCESS" });
        }

        [HttpPost("SetStatusJurusan/{id}")]
        [RequiresPermission("jurusan.edit")]
        public async Task<IActionResult> SetStatusJurusan(short id)
        {
            var username = User.FindFirstValue("namaakun");

            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized();
            }

            var success = await _repo.SetStatusAsync(id, username);
            if (!success)
            {
                return NotFound(new { message = "Data Jurusan tidak ditemukan." });
            }
            return Ok(new { message = "SUCCESS" });
        } 
    }
}
