using astratech_apps_backend.DTOs.Institusi;
using astratech_apps_backend.DTOs.JamPlus;
using astratech_apps_backend.DTOs.JamPlusController;
using astratech_apps_backend.Helpers;
using astratech_apps_backend.Repositories.Implementations;
using astratech_apps_backend.Repositories.Interfaces;
using astratech_apps_backend.Services.Implementations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace astratech_apps_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class JamPlusController(IJamPlusRepository repo, ImportJamPlusService importService) : ControllerBase
    {
        private readonly IJamPlusRepository _repo = repo;
        private readonly ImportJamPlusService _importService = importService;

        [HttpGet("GetAllJamPlus")]
        [RequiresPermission("jam_plus.view")]
        public async Task<IActionResult> GetAllJamPlus([FromQuery] GetAllJamPlusRequest dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var username = User.FindFirstValue("namaakun");
            if (string.IsNullOrEmpty(username)) return Unauthorized();
            dto.Username = username;

            var sanitizedDto = SanitizerHelper.EncodeObject(dto);
            if (sanitizedDto == null) return NotFound();

            var (list, totalData) = await _repo.GetAllAsync(sanitizedDto);

            var response = new GetAllJamPlusResponse
            {
                ListData = [.. list.Select(jamplus => new JamPlusDto
                {
                    MahasiswaId = jamplus.MahasiswaId,
                    Nama = jamplus.Nama,
                    Prodi = jamplus.Prodi,
                    TahunAkademik = jamplus.TahunAkademik,
                    Semester = jamplus.Semester,
                    Kelas = jamplus.Kelas,
                    TotalJamPlus = jamplus.TotalJamPlus
                })],
                TotalData = totalData,
                TotalHalaman = dto.PageSize > 0 ? ((int)totalData - 1) / dto.PageSize + 1 : 1
            };

            return Ok(response);
        }

        [HttpGet("DetailMahasiswa/{id}")]
        [RequiresPermission("jam_plus.view")]
        public async Task<IActionResult> GetDetailMahasiswa(string id, [FromQuery] string tahunAjaran, [FromQuery] string semester)
        {
            if (string.IsNullOrEmpty(tahunAjaran) || string.IsNullOrEmpty(semester))
            {
                return BadRequest(new { message = "Parameter Tahun Ajaran dan Semester wajib diisi." });
            }

            var dataDto = await _repo.GetByIdAsync(id, tahunAjaran, semester);

            if (dataDto == null) return NotFound(new { message = "Data mahasiswa tidak ditemukan." });

            return Ok(dataDto);
        }

        [HttpGet("GetListHistory")]
        [RequiresPermission("jam_plus.view")]
        public async Task<IActionResult> GetListHistory([FromQuery] string id, [FromQuery] string tahunAjaran, [FromQuery] string semester)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(tahunAjaran) || string.IsNullOrEmpty(semester))
            {
                return BadRequest(new { message = "Parameter tidak lengkap." });
            }

            var list = await _repo.GetAllAsync(id, tahunAjaran, semester);
            return Ok(new { Data = list });
        }

        [HttpPost("CreateJamPlus")]
        [RequiresPermission("jam_plus.create")]
        public async Task<IActionResult> CreateJamPlus([FromBody] CreateJamplus dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var username = User.FindFirstValue("namaakun");
            if (string.IsNullOrEmpty(username)) return Unauthorized();

            var sanitizedDto = SanitizerHelper.EncodeObject(dto);
            if (sanitizedDto == null) return NotFound();

            var rowsAffected = await _repo.CreateAsync(sanitizedDto, username);

            if (rowsAffected > 0) return Ok(new { message = "SUCCESS" });
            else return BadRequest(new { message = "Gagal menyimpan data." });
        }

        [HttpPut("EditJamPlus")]
        [RequiresPermission("jam_plus.edit")]
        public async Task<IActionResult> UpdateJamPlus([FromBody] UpdateJamPlusRequest dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var username = User.FindFirstValue("namaakun");
            if (string.IsNullOrEmpty(username)) return Unauthorized();

            var sanitizedDto = SanitizerHelper.EncodeObject(dto);
            if (sanitizedDto == null) return NotFound();

            var success = await _repo.UpdateAsync(dto, username);
            if (!success) return NotFound(new { message = "Data Jam Plus tidak ditemukan." });

            return Ok(new { message = "SUCCESS" });
        }

        [HttpPost("ImportExcel")]
        [RequiresPermission("jam_plus.import")]

        public async Task<IActionResult> ImportExcel([FromForm] ImportJamPlusRequest dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var username = User.FindFirstValue("namaakun");
            if (string.IsNullOrEmpty(username)) return Unauthorized();

           
            var result = await _importService.ProcessImportAsync(dto, username);

            return Ok(new
            {
                success = result.Success,
                failed = result.Failed,
                message = result.Message,
                errors = result.ErrorDetails 
            });
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

        [HttpDelete("delete")]
        [RequiresPermission("jam_plus.delete")]
        public async Task<IActionResult> DeleteJamPlus([FromBody] DeleteJamPlus deleteDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);


            string? modifBy = User.FindFirst("namaakun")?.Value;

            if (string.IsNullOrEmpty(modifBy)) return Unauthorized(new { message = "User tidak teridentifikasi." });

            var result = await _repo.DeleteJamPlusAsync(deleteDto, modifBy);

            if (!result) return NotFound(new { message = "Data Jam Plus tidak ditemukan." });

            return Ok(new { message = "Data Jam Plus berhasil dihapus." });
        }


        [HttpGet("GetActiveTahunAkademik")]
        [RequiresPermission("jam_plus.create")]
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