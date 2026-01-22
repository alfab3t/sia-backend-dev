using astratech_apps_backend.DTOs.Remedial;
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
    public class RemedialController : ControllerBase
    {
        private readonly IRemedialRepository _remedialRepository;
        private const string SuccessMessage = "SUCCESS";

        public RemedialController(IRemedialRepository remedialRepository)
        {
            _remedialRepository = remedialRepository;
        }

        private (string username, string roleId) GetUserInfo()
        {
            var username = User.FindFirstValue("namaakun");
            var roleId = User.FindFirstValue("idrole");

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(roleId))
            {
                throw new UnauthorizedAccessException("Informasi user tidak ditemukan di token");
            }

            return (username, roleId);
        }

        [HttpGet("GetDataRemedial")] 
        [RequiresPermission("remedial.view")]
        public async Task<IActionResult> GetDataRemedial([FromQuery] GetAllRemedialRequest dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var sanitizedDto = SanitizerHelper.EncodeObject(dto);
            if (sanitizedDto == null) return NotFound();

            var (username, roleId) = GetUserInfo();

            var (list, totalData) = await _remedialRepository.GetDataRemedialAsync(sanitizedDto, username ?? "", roleId ?? "");

            var dataDTO = list.Select(remedial => new RemedialDto
            {
                NilaiMahasiswaId = remedial.NilaiMahasiswaId,
                MahasiswaId = remedial.MahasiswaId,
                MahasiswaNama = remedial.MahasiswaNama,
                NilaiAkhir = remedial.NilaiAkhir,
                AngkaMutu = remedial.AngkaMutu,
                NilaiRemedial = remedial.NilaiRemedial,
                AngkaMutuRemedial = remedial.AngkaMutuRemedial,
                CanEdit = remedial.CanEdit
            }).ToList();

            return Ok(new {
                success = true,
                data = dataDTO,
                totalData,
                totalHalaman = ((totalData - 1) / (dto.PageSize > 0 ? dto.PageSize : 10)) + 1
            });
        }

        [HttpGet("GetListKonsentrasi")]
        [RequiresPermission("remedial.view")]
        public async Task<IActionResult> GetListKonsentrasi()
        {
            try
            {
                var (username, roleId) = GetUserInfo();
                var data = await _remedialRepository.GetListKonsentrasiAsync(username ?? "", roleId ?? "");
                
                return Ok(new { 
                    success = true, 
                    data, 
                    message = SuccessMessage 
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });            
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error: {ex.Message}" });
            }
        }

        [HttpGet("GetListTahunAjaran")]
        [RequiresPermission("remedial.view")]
        public async Task<IActionResult> GetListTahunAjaran()
        {
            try
            {
                var data = await _remedialRepository.GetListTahunAjaranAsync();
                
                return Ok(new { 
                    success = true, 
                    data, 
                    message = SuccessMessage 
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error: {ex.Message}" });
            }
        }

        [HttpGet("GetListMataKuliah")]
        [RequiresPermission("remedial.view")]
        public async Task<IActionResult> GetListMataKuliah([FromQuery] GetAllRemedialRequest dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var sanitizedDto = SanitizerHelper.EncodeObject(dto);
            if (sanitizedDto == null) return NotFound();

            try
            {
                var (username, roleId) = GetUserInfo();

                var data = await _remedialRepository.GetListMataKuliahAsync(
                    sanitizedDto.KonsentrasiId ?? "",
                    sanitizedDto.TahunAjaran ?? "",
                    sanitizedDto.Semester ?? "",
                    username,
                    roleId);

                return Ok(new { 
                    success = true, 
                    data, 
                    message = SuccessMessage 
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error: {ex.Message}" });

            }
        }

        [HttpGet("GetListKelas")]
        [RequiresPermission("remedial.view")]
        public async Task<IActionResult> GetListKelas([FromQuery] string mataKuliahId)
        {
            try
            {
                var (username, roleId) = GetUserInfo();

                var data = await _remedialRepository.GetListKelasAsync(
                    mataKuliahId, username, roleId);

                return Ok(new { 
                    success = true, 
                    data, 
                    message = SuccessMessage
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error: {ex.Message}" });

            }
        }

        [HttpGet("GetDosen")]
        [RequiresPermission("remedial.view")]
        public async Task<IActionResult> GetDosen([FromQuery] string mataKuliahId, [FromQuery] string kelas)
        {
            try
            {
                var dosen = await _remedialRepository.GetDosenByKelasAsync(mataKuliahId, kelas);
                return Ok(new { success = true, data = new { namaDosen = dosen } });
            }
            catch (FormatException)
            {
                return BadRequest(new { message = "ID mata kuliah tidak valid." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error: {ex.Message}" });
            }
        }

        [HttpPut("UpdateNilaiRemedial")]
        [RequiresPermission("remedial.edit")]
        public async Task<IActionResult> UpdateNilaiRemedial([FromBody] UpdateRemedialRequest dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var sanitizedDto = SanitizerHelper.EncodeObject(dto);
            if (sanitizedDto == null) return NotFound();

            try
            {
                var (username, roleId) = GetUserInfo();

                var success = await _remedialRepository.UpdateNilaiRemedialAsync(sanitizedDto, username, roleId);

                if (!success)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Gagal mengupdate nilai remedial"
                    });
                }

                return Ok(new { 
                    success = true,
                    message = SuccessMessage
                });

            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error: {ex.Message}" });
            }
        }

        [HttpGet("ValidateEditPermission")]
        [RequiresPermission("remedial.edit")]
        public async Task<IActionResult> ValidateEditPermission([FromQuery] GetAllRemedialRequest dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var sanitizedDto = SanitizerHelper.EncodeObject(dto);
            if (sanitizedDto == null) return NotFound();

            try
            {
                var (username, roleId) = GetUserInfo();

                var canEdit = await _remedialRepository.CanEditRemedialAsync(
                sanitizedDto.TahunAjaran ?? "", 
                sanitizedDto.Semester ?? "", 
                sanitizedDto.IdMataKuliah ?? "", 
                sanitizedDto.Kelas ?? "", 
                username, 
                roleId);

                return Ok(new { 
                    success = true, 
                    data = new { canEdit } 
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error: {ex.Message}" });
            }
        }

        [HttpGet("ActivePeriode")]
        [RequiresPermission("remedial.view")]
        public async Task<IActionResult> GetActivePeriode()
        {
            try
            {
                var (tahunAjaran, semester) = await _remedialRepository.GetActivePeriodeAsync();
    
                return Ok(new
                {
                    success = true,
                    data = new { tahunAjaran, semester },
                    message = "Periode aktif berhasil diambil"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error: {ex.Message}" });
            }
        }

        [HttpGet("ActivePenilaianPeriod")]
        [RequiresPermission("remedial.view")]
        public async Task<IActionResult> GetActivePenilaianPeriod()
        {
            try
            {
                var (tahunAjaran, semester) = await _remedialRepository.GetActivePenilaianPeriodAsync();

                return Ok(new
                {
                    success = true,
                    data = new { tahunAjaran, semester },
                    message = "Periode penilaian aktif berhasil diambil"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error: {ex.Message}" });
            }
        }

        [HttpGet("ActiveAcademicYear")]
        [RequiresPermission("remedial.view")]
        public async Task<IActionResult> GetActiveAcademicYear([FromQuery] DateTime? tanggalReferensi = null)
        {
            try
            {
                var (tahunAjaran, semester) = await _remedialRepository.GetActiveAcademicYearAsync(tanggalReferensi);

                return Ok(new
                {
                    success = true,
                    data = new { tahunAjaran, semester },
                    message = "Tahun akademik aktif berhasil diambil"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error: {ex.Message}" });
            }
        }
    }
}