using astratech_apps_backend.DTOs.Kuesioner;
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
    public class KuesionerController(IKuesionerRepository repo) : ControllerBase
    {
        private readonly IKuesionerRepository _repo = repo;
        private const string namaAkun = "namaAkun";

        [HttpGet("GetAllKuesioner")]
        [RequiresPermission("kuesioner.view")]
        public async Task<IActionResult> GetAll([FromQuery] GetAllKuesionerRequest dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var sanitizedDto = SanitizerHelper.EncodeObject(dto);
            if (sanitizedDto == null) return NotFound();

            var username = User.FindFirstValue(namaAkun);
            var roleId = User.FindFirstValue("idrole");

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(roleId)) return NotFound();

            var (list, totalData) = await _repo.GetAllAsync(dto, username, roleId);

            return Ok(new
            {
                data = list,
                totalData,
                totalHalaman = ((totalData - 1) / dto.PageSize) + 1
            });
        }

        [HttpGet("GetUntukDiisi/{id}")]
        [RequiresPermission("kuesioner.create")]
        public async Task<IActionResult> GetUntukDiisi(int id)
        {
            var username = User.FindFirstValue("namaakun");
            if (string.IsNullOrEmpty(username)) return Unauthorized();

            var result = await _repo.GetDetailForUserAsync(id, username);

            if (result == null) return NotFound(new { message = "Kuesioner tidak ditemukan." });

            return Ok(result);
        }

        [HttpPost("GenerateKuesioner")]
        [RequiresPermission("kuesioner.create")]
        public async Task<IActionResult> Generate([FromBody] GenerateKuesionerRequest dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var username = User.FindFirstValue(namaAkun);

            var sanitizedDto = SanitizerHelper.EncodeObject(dto);
            if (sanitizedDto == null) return NotFound();

            var (success, message) = await _repo.GenerateKuesionerAsync(dto, username!);
            if (!success) return BadRequest(new { message });

            return Ok(new { message });
        }

        [HttpGet("GetHasilKuesioner/{id}")]
        [RequiresPermission("kuesioner.view")]
        public async Task<IActionResult> GetHasil(int id)
        {
            if (id <= 0) return BadRequest(new { message = "ID Kuesioner tidak valid." });

            var result = await _repo.GetResultForAdminAsync(id);

            if (result == null)
                return NotFound(new { message = "Data kuesioner tidak ditemukan." });

            return Ok(result);
        }

        [HttpPost("SubmitKuesioner")]
        [RequiresPermission("kuesioner.create")]
        public async Task<IActionResult> Submit([FromBody] SubmitKuesionerRequest dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var username = User.FindFirstValue(namaAkun);

            var sanitizedDto = SanitizerHelper.EncodeObject(dto);
            if (sanitizedDto == null) return NotFound();

            var (success, message) = await _repo.SubmitJawabanAsync(dto, username!);

            if (!success) return BadRequest(new { message });
            return Ok(new { message });
        }

        [HttpGet("ExportKuesioner/{jenisExport}")]
        [RequiresPermission("kuesioner.export")]
        public async Task<IActionResult> Export(string jenisExport, [FromQuery] string tahunAkademik, [FromQuery] string semester)
        {
            if (string.IsNullOrEmpty(tahunAkademik) || string.IsNullOrEmpty(semester))
            {
                return BadRequest(new { message = "Tahun Akademik dan Semester wajib diisi." });
            }

            var username = User.FindFirstValue(namaAkun);
            var roleId = User.FindFirstValue("idrole");

            var request = new GetExportDataRequest
            {
                JenisExport = jenisExport,
                TahunAkademik = tahunAkademik,
                Semester = semester,
                Username = username!,
                RoleId = roleId!
            };

            var data = await _repo.GetExportDataAsync(request);
            return Ok(data);
        }

    }
}