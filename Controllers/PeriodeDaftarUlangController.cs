using astratech_apps_backend.DTOs.PeriodeDaftarUlang;
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
    public class PeriodeDaftarUlangController(IPeriodeDaftarUlangRepository repo) : ControllerBase
    {
        private readonly IPeriodeDaftarUlangRepository _repo = repo;

        [HttpGet("GetAllPeriodeDaftarUlang")]
        [RequiresPermission("periode_daftar_ulang.view")]
        public async Task<IActionResult> GetAllPeriode([FromQuery] GetAllPeriodeDaftarUlangRequest dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var sanitizedDto = SanitizerHelper.EncodeObject(dto);
            if (sanitizedDto == null) return NotFound();

            var (list, totalData) = await _repo.GetAllAsync(sanitizedDto);

            var dataDtos = list.Select(model => new PeriodeDaftarUlangDto
            {
                RowNumber = model.RowNumber,
                Id = model.Id,
                TahunAjaran = model.TahunAjaran,
                TanggalMulai = model.TanggalMulai,
                TanggalAkhir = model.TanggalAkhir
            }).ToList();

            var response = new GetAllPeriodeDaftarUlangResponse
            {
                Data = dataDtos,
                TotalData = totalData,
                TotalHalaman = (totalData > 0) ? ((totalData - 1) / dto.PageSize) + 1 : 0
            };
            return Ok(response);
        }

        [HttpGet("PeriodeDaftarUlang/{id}")]
        [RequiresPermission("periode_daftar_ulang.view")]
        public async Task<IActionResult> GetPeriodeById(int id)
        {
            var data = await _repo.GetByIdAsync(id);
            if (data == null)
            {
                return NotFound(new { message = "Data periode daftar ulang tidak ditemukan." });
            }

            var dto = new PeriodeDaftarUlangDto
            {
                Id = data.Id,
                TahunAjaran = data.TahunAjaran,
                TanggalMulai = data.TanggalMulai,
                TanggalAkhir = data.TanggalAkhir
            };

            return Ok(dto);
        }

        [HttpGet("GetTahunAjaran")]
        [RequiresPermission("periode_daftar_ulang.view")]
        public async Task<IActionResult> GetTahunAjaranForDropdown()
        {
            var result = await _repo.GetListTahunAjaranAsync();
            return Ok(result);
        }

        [HttpPost("CreatePeriodeDaftarUlang")]
        [RequiresPermission("periode_daftar_ulang.create")]
        public async Task<IActionResult> CreatePeriode([FromBody] CreatePeriodeDaftarUlangRequest dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (dto.TanggalMulai > dto.TanggalAkhir)
            {
                (dto.TanggalMulai, dto.TanggalAkhir) = (dto.TanggalAkhir, dto.TanggalMulai);
            }

            if (!await _repo.CheckPeriodeDaftarUlangAsync(dto.TahunAjaran))
            {
                return BadRequest(new { message = "Periode daftar ulang pada tahun akademik tersebut tidak dapat dibuat. Pastikan kelas mahasiswa sudah dibuat sebelumnya atau periode sudah pernah dibuat sebelumnya." });
            }

            var username = User.FindFirstValue("namaakun");
            if (string.IsNullOrEmpty(username)) return Unauthorized();

            var sanitizedDto = SanitizerHelper.EncodeObject(dto);
            if (sanitizedDto == null) return NotFound();

            var success = await _repo.CreateAsync(sanitizedDto!, username);
            if (!success)
            {
                return BadRequest(new { message = "Gagal menyimpan data." });
            }

            return Ok(new { message = "SUCCESS" });
        }

        [HttpPut("EditPeriodeDaftarUlang")]
        [RequiresPermission("periode_daftar_ulang.edit")]
        public async Task<IActionResult> UpdatePeriode([FromBody] UpdatePeriodeDaftarUlangRequest dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (dto.TanggalMulai > dto.TanggalAkhir)
            {
                (dto.TanggalMulai, dto.TanggalAkhir) = (dto.TanggalAkhir, dto.TanggalMulai);
            }

            var username = User.FindFirstValue("namaakun");
            if (string.IsNullOrEmpty(username)) return Unauthorized();

            var sanitizedDto = SanitizerHelper.EncodeObject(dto);
            if (sanitizedDto == null) return NotFound();

            var success = await _repo.UpdateAsync(sanitizedDto!, username);
            if (!success)
            {
                return BadRequest(new { message = "Gagal memperbarui data periode daftar ulang." });
            }

            return Ok(new { message = "SUCCESS" });
        }
    }
}