using astratech_apps_backend.DTOs.Institusi;
using astratech_apps_backend.DTOs.Jam_Minus;
using astratech_apps_backend.DTOs.JamMinusController;
using astratech_apps_backend.Helpers;
using astratech_apps_backend.Repositories.Interfaces;
using astratech_apps_backend.DTOs.JamMinus;
using astratech_apps_backend.Services.Implementations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace astratech_apps_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class JamMinusController(
        IJamMinusRepository repo,
        ImportJamMinusService importService) : ControllerBase
    {
        private readonly IJamMinusRepository _repo = repo;
        private readonly ImportJamMinusService _importService = importService;

        [HttpGet("GetAllJamMinus")]
        [RequiresPermission("jam_minus.view")]
        public async Task<IActionResult> GetAllJamMinus([FromQuery] GetAllJamMinusRequest dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var username = User.FindFirstValue("namaakun");
            if (string.IsNullOrEmpty(username)) return Unauthorized();
            dto.Username = username;

            var sanitizedDto = SanitizerHelper.EncodeObject(dto);
            if (sanitizedDto == null) return NotFound();

            var (list, totalData) = await _repo.GetAllAsync(sanitizedDto);

            var response = new GetAllJamMinusResponse
            {
                ListData = list.Select(jamminus => new JamMinusDto
                {
                    MahasiswaId = jamminus.mahasiswaID,
                    Nama = jamminus.Nama,
                    Prodi = jamminus.Prodi,
                    TahunAkademik = jamminus.TahunAkademik,
                    Semester = jamminus.Semester,
                    Kelas = jamminus.Kelas,
                    TotalJamMinus = jamminus.TotalJamMinus
                }).ToList(),

                TotalData = totalData,
                TotalHalaman = dto.PageSize > 0
                                ? ((int)totalData - 1) / dto.PageSize + 1
                                : 1
            };

            return Ok(response);
        }

        [HttpGet("DetailMahasiswa/{id}")]
        [RequiresPermission("jam_minus.view")]
        public async Task<IActionResult> GetDetailMahasiswa(string id, [FromQuery] string tahunAjaran, [FromQuery] string semester)
        {
            if (string.IsNullOrEmpty(tahunAjaran) || string.IsNullOrEmpty(semester))
            {
                return BadRequest(new { message = "Parameter tahunAjaran dan semester wajib diisi." });
            }

            var dataDto = await _repo.GetByIdAsync(id, tahunAjaran, semester);

            if (dataDto == null) return NotFound(new { message = "Data mahasiswa tidak ditemukan." });

            return Ok(dataDto);
        }

        [HttpGet("GetListHistory")]
        [RequiresPermission("jam_minus.view")]
        public async Task<IActionResult> GetListHistory([FromQuery] string id, [FromQuery] string tahunAjaran, [FromQuery] string semester)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(tahunAjaran) || string.IsNullOrEmpty(semester))
            {
                return BadRequest(new { message = "Parameter tidak lengkap." });
            }

            var list = await _repo.GetAllAsync(id, tahunAjaran, semester);
            return Ok(new { Data = list });
        }

        [HttpPost("CreateJamMinus")]
        [RequiresPermission("jam_minus.create")]
        public async Task<IActionResult> CreateJamMinus([FromBody] CreateJamMinus dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var username = User.FindFirstValue("namaakun");
            if (string.IsNullOrEmpty(username)) return Unauthorized();

            var sanitizedDto = SanitizerHelper.EncodeObject(dto);
            if (sanitizedDto == null) return NotFound();

            try
            {
                await _repo.CreateAsync(sanitizedDto, username);
                return Ok(new { message = "Berhasil menyimpan data." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("EditJamMinus")]
        [RequiresPermission("jam_minus.edit")]
        public async Task<IActionResult> UpdateJamMinus([FromBody] UpdateJamMinusRequest dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var username = User.FindFirstValue("namaakun");
            if (string.IsNullOrEmpty(username)) return Unauthorized();

            var sanitizedDto = SanitizerHelper.EncodeObject(dto);
            if (sanitizedDto == null) return NotFound();

            var success = await _repo.UpdateAsync(sanitizedDto, username);
            if (!success) return NotFound(new { message = "Data Jam Minus tidak ditemukan." });

            return Ok(new { message = "SUCCESS" });
        }

        [HttpGet("GetListKonsentrasi")]
        public async Task<IActionResult> GetListKonsentrasi()
        {
            try
            {
                var username = User.FindFirstValue("namaakun") ?? string.Empty;
                var roleId = User.FindFirstValue("rolid") ?? User.FindFirstValue("role") ?? string.Empty;
                var result = await _repo.GetListKonsentrasiAsync(username, roleId);
                return Ok(new { status = 200, message = "Success", data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = 400, message = ex.Message });
            }
        }

        [HttpGet("GetListKelas")]
        public async Task<IActionResult> GetListKelas(string idProdi, string tahunAjaran)
        {
            try
            {
                if (string.IsNullOrEmpty(idProdi) || string.IsNullOrEmpty(tahunAjaran))
                {
                    return BadRequest(new { status = 400, message = "Parameter Prodi dan Tahun Ajaran wajib diisi." });
                }
                var result = await _repo.GetListKelasAsync(idProdi, tahunAjaran);
                return Ok(new { status = 200, message = "Success", data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = 400, message = ex.Message });
            }
        }

        [HttpDelete("delete")]
        [RequiresPermission("jam_minus.delete")]
        public async Task<IActionResult> DeleteAsync([FromBody] DeleteJamMinusRequest deleteDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            string modifBy = User.FindFirst("namaakun")?.Value ?? "";
            if (string.IsNullOrEmpty(modifBy)) return Unauthorized(new { message = "User tidak teridentifikasi." });

            var result = await _repo.DeleteAsync(deleteDto, modifBy);
            if (!result) return NotFound(new { message = "Data Jam Minus tidak ditemukan." });

            return Ok(new { message = "Data Jam Minus berhasil dihapus." });
        }

        [HttpGet("GetListMahasiswaByKelas")]
        public async Task<IActionResult> GetListMahasiswaAsync(string IdKelas)
        {
            try
            {
                if (string.IsNullOrEmpty(IdKelas))
                {
                    return BadRequest(new { status = 400, message = "Parameter Id Kelas wajib diisi." });
                }
                var result = await _repo.GetListMahasiswaAsync(IdKelas);
                return Ok(new { status = 200, message = "Success", data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = 400, message = ex.Message });
            }
        }

        [HttpPost("ImportExcel")]
        [RequiresPermission("jam_minus.import")]
        public async Task<IActionResult> ImportExcel([FromForm] ImportJamMinusRequest dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var username = User.FindFirstValue("namaakun");
            if (string.IsNullOrEmpty(username)) return Unauthorized();

            var (successCount, failedCount, messageResult) = await _importService.ProcessImportAsync(dto, username);

            return Ok(new
            {
                success = successCount,
                failed = failedCount,
                message = messageResult
            });
        }

        [HttpGet("GetActiveTahunAkademik")]
        [RequiresPermission("jam_minus.create")]
        public async Task<IActionResult> GetActiveTahunAkademik([FromQuery] DateTime dateToCheck)
        {
            try
            {
                var result = await _repo.GetActiveTahunAkademikAsync(dateToCheck);

                if (result != null)
                {
                    return Ok(new
                    {
                        status = 200,
                        message = "Success",
                        data = result
                    });
                }
                else
                {
                    return NotFound(new
                    {
                        status = 404,
                        message = "Tahun Akademik tidak ditemukan untuk tanggal tersebut."
                    });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = 400,
                    message = "Terjadi kesalahan server: " + ex.Message
                });
            }
        }
    }
}