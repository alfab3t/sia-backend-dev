using astratech_apps_backend.DTOs.AlokasiKurikulum;
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
    public class AlokasiKurikulumController(IAlokasiKurikulumRepository repo) : ControllerBase
    {
        private readonly IAlokasiKurikulumRepository _repo = repo;

        [HttpGet("GetListAlokasiKurikulumMahasiswa")]
        [RequiresPermission("alokasi_kurikulum_mahasiswa.view")]
        public async Task<IActionResult> GetListAlokasiKurikulumMahasiswa([FromQuery] GetAllKurikulumMahasiswaRequest request)
        {
            var (list, totalData) = await _repo.GetMahasiswaKurikulumListAsync(request);

            var dataDTO = list.Select(m => new AlokasiKurikulumMahasiswaDto
            {
                Id = m.Id,
                Nama = m.Nama,
                ProgramStudi = m.ProgramStudi,
                Angkatan = m.Angkatan,
                Status = m.Status,
                Kurikulum = m.Kurikulum
            }).ToList();

            var response = new
            {
                Data = dataDTO,
                TotalData = totalData,
                request.PageNumber,
                request.PageSize
            };

            return Ok(response);
        }

        [HttpGet("GetProdiList")]
        [RequiresPermission("alokasi_kurikulum_mahasiswa.view")]
        public async Task<IActionResult> GetProdiList()
        {
            var list = await _repo.GetProdiListAsync();

            _ = list.Select(x => new
            {
                id = x.IdProgramStudi,
                nama = x.ProgramStudi
            });

            return Ok(list);
        }

        [HttpGet("GetAngkatanList")]
        [RequiresPermission("alokasi_kurikulum_mahasiswa.view")]
        public async Task<IActionResult> GetAngkatanList()
        {
            var list = await _repo.GetAngkatanListAsync();
            return Ok(list);
        }

        [HttpGet("GetKurikulumList")]
        [RequiresPermission("alokasi_kurikulum_mahasiswa.view")]
        public async Task<IActionResult> GetKurikulumList([FromQuery] string? konid)
        {
            var list = await _repo.GetKurikulumListAsync(konid ?? string.Empty);
            return Ok(list);
        }

        [HttpPost("UpdateAlokasiKurikulum")]
        [RequiresPermission("alokasi_kurikulum_mahasiswa.edit")]
        public async Task<IActionResult> UpdateAlokasiKurikulum([FromBody] UpdateAlokasiKurikulumRequest dto)
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

            var success = await _repo.UpdateAlokasiKurikulumAsync(sanitizedDto);
            if (!success)
            {
                return BadRequest(new { message = "Gagal menyimpan perubahan alokasi kurikulum." });
            }
            return Ok(new { message = "SUCCESS" });
        }
    }
}
