using astratech_apps_backend.DTOs.PengubahanNilai;
using astratech_apps_backend.Helpers;
using astratech_apps_backend.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Security.Claims;

namespace astratech_apps_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PengubahanNilaiController : ControllerBase
    {
        private readonly IPengubahanNilaiRepository _PengubahanNilaiRepository;

        private const string CLAIM_USERNAME = "namaakun";
        private const string CLAIM_ROLE_ID = "idrole";
        private const string CLAIM_APP_ID = "idapp";
        private const string ERROR_TOKEN_INVALID = "Token tidak valid";
        private const string ERROR_ACCESS_NOT_ALLOWED = "Akses tidak diizinkan untuk aplikasi ini";
        private const string APP_ID = "APP08";

        public PengubahanNilaiController(IPengubahanNilaiRepository PengubahanNilaiRepository)
        {
            _PengubahanNilaiRepository = PengubahanNilaiRepository;
        }

        [HttpPost("pengubahan-nilai")]
        [RequiresPermission("permintaan_pengubahan_nilai.view")]
        public async Task<IActionResult> GetDataPengubahanNilai([FromBody] PengubahanNilaiRequestDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var username = User.FindFirstValue(CLAIM_USERNAME);
            var roleId = User.FindFirstValue(CLAIM_ROLE_ID);

            if (string.IsNullOrEmpty(username)) return Unauthorized(new { message = ERROR_TOKEN_INVALID });
            if (string.IsNullOrEmpty(roleId)) return Unauthorized(new { message = ERROR_TOKEN_INVALID });
            if (dto == null) return BadRequest(new { message = "Data request tidak valid" });

            var sanitizedDto = SanitizerHelper.EncodeObject(dto)!;
            var res = await _PengubahanNilaiRepository.GetDataPengubahanNilaiAsync(sanitizedDto, username, roleId);

            if (!string.IsNullOrEmpty(res?.ErrorMessage))
                return BadRequest(new { message = res.ErrorMessage });
            return Ok(res);
        }

        [HttpPost("pengubahan-nilai/create")]
        [RequiresPermission("permintaan_pengubahan_nilai.create")]
        public async Task<IActionResult> CreatePengubahanNilai([FromBody] CreatePengubahanNilaiDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var username = User.FindFirstValue(CLAIM_USERNAME);
            if (string.IsNullOrEmpty(username)) return Unauthorized(new { message = ERROR_TOKEN_INVALID });

            var sanitizedDto = SanitizerHelper.EncodeObject(dto);
            if (sanitizedDto == null) return NotFound();
            var res = await _PengubahanNilaiRepository.CreatePengubahanNilaiAsync(sanitizedDto, username);

            if (!string.IsNullOrEmpty(res?.ErrorMessage))
                return BadRequest(new { message = res.ErrorMessage });

            return Ok(res);
        }

        [HttpGet("pengubahan-nilai/{id}")]
        [RequiresPermission("permintaan_pengubahan_nilai.view")]
        public async Task<IActionResult> GetDetailPengubahanNilai(int id)
        {
            var appId = User.FindFirstValue(CLAIM_APP_ID);
            if (appId != APP_ID) return StatusCode(403, new { message = ERROR_ACCESS_NOT_ALLOWED });

            var result = await _PengubahanNilaiRepository.GetDetailPengubahanNilaiAsync(id);

            if (!string.IsNullOrEmpty(result?.ErrorMessage))
            {
                return result.ErrorMessage.Contains("tidak ditemukan")
                    ? NotFound(new { message = result.ErrorMessage })
                    : BadRequest(new { message = result.ErrorMessage });
            }

            return Ok(result);
        }

        [HttpPut("pengubahan-nilai/{id}/send")]
        [RequiresPermission("permintaan_pengubahan_nilai.edit")]
        public async Task<IActionResult> SendPengubahanNilai(int id, [FromBody] SendPengubahanNilaiDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var username = User.FindFirstValue(CLAIM_USERNAME);
            var roleId = User.FindFirstValue(CLAIM_ROLE_ID);
            var appId = User.FindFirstValue(CLAIM_APP_ID);

            if (appId != APP_ID) return StatusCode(403, new { message = ERROR_ACCESS_NOT_ALLOWED });
            if (string.IsNullOrEmpty(username)) return Unauthorized(new { message = ERROR_TOKEN_INVALID });
            if (string.IsNullOrEmpty(roleId)) return Unauthorized(new { message = ERROR_TOKEN_INVALID });
            if (dto == null) return BadRequest(new { message = "Data request tidak valid" });

            var sanitizedDto = SanitizerHelper.EncodeObject(dto)!;
            var res = await _PengubahanNilaiRepository.SendPengubahanNilaiAsync(id, sanitizedDto, username, roleId);

            if (!res?.IsSuccess ?? true)
                return BadRequest(new { message = res?.ErrorMessage ?? "Gagal mengirim data" });

            return Ok(res);
        }

        [HttpPut("pengubahan-nilai/{id}/approve")]
        [RequiresPermission("permintaan_pengubahan_nilai.approve_reject")]
        public async Task<IActionResult> ApprovePengubahanNilai(int id)
        {
            var username = User.FindFirstValue(CLAIM_USERNAME);
            var roleId = User.FindFirstValue(CLAIM_ROLE_ID);

            if (string.IsNullOrEmpty(username)) return Unauthorized(new { message = ERROR_TOKEN_INVALID });
            if (string.IsNullOrEmpty(roleId)) return Unauthorized(new { message = ERROR_TOKEN_INVALID });

            var res = await _PengubahanNilaiRepository.ApprovePengubahanNilaiAsync(id, username, roleId);
            if (!string.IsNullOrEmpty(res?.ErrorMessage))
                return BadRequest(new { message = res.ErrorMessage });

            return Ok(res);
        }

        [HttpPut("pengubahan-nilai/{id}/reject")]
        [RequiresPermission("permintaan_pengubahan_nilai.approve_reject")]
        public async Task<IActionResult> RejectPengubahanNilai(int id, [FromBody] RejectPengubahanNilaiDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var username = User.FindFirstValue(CLAIM_USERNAME);
            var roleId = User.FindFirstValue(CLAIM_ROLE_ID);
            var appId = User.FindFirstValue(CLAIM_APP_ID);

            if (appId != APP_ID) return StatusCode(403, new { message = ERROR_ACCESS_NOT_ALLOWED });
            if (string.IsNullOrEmpty(username)) return Unauthorized(new { message = ERROR_TOKEN_INVALID });
            if (string.IsNullOrEmpty(roleId)) return Unauthorized(new { message = ERROR_TOKEN_INVALID });
            if (dto == null) return BadRequest(new { message = "Data request tidak valid" });

            var sanitizedDto = SanitizerHelper.EncodeObject(dto);
            if (sanitizedDto == null) return NotFound();
            var res = await _PengubahanNilaiRepository.RejectPengubahanNilaiAsync(id, sanitizedDto, username, roleId);

            if (!string.IsNullOrEmpty(res?.ErrorMessage))
                return BadRequest(new { message = res.ErrorMessage });

            return Ok(res);
        }

        [HttpDelete("pengubahan-nilai/{id}")]
        [RequiresPermission("permintaan_pengubahan_nilai.delete")]
        public async Task<IActionResult> DeletePengubahanNilai(int id)
        {
            var username = User.FindFirstValue(CLAIM_USERNAME);
            var roleId = User.FindFirstValue(CLAIM_ROLE_ID);
            var appId = User.FindFirstValue(CLAIM_APP_ID);

            if (appId != APP_ID) return StatusCode(403, new { message = ERROR_ACCESS_NOT_ALLOWED });
            if (string.IsNullOrEmpty(username)) return Unauthorized(new { message = ERROR_TOKEN_INVALID });
            if (string.IsNullOrEmpty(roleId)) return Unauthorized(new { message = ERROR_TOKEN_INVALID });

            var res = await _PengubahanNilaiRepository.DeletePengubahanNilaiAsync(id, username, roleId);
            if (!string.IsNullOrEmpty(res?.ErrorMessage))
                return BadRequest(new { message = res?.ErrorMessage });

            return Ok(res);
        }

        [HttpGet("konsentrasi")]
        [RequiresPermission("permintaan_pengubahan_nilai.view")]
        public async Task<IActionResult> GetListKonsentrasi()
        {
            var roleId = User.FindFirstValue(CLAIM_ROLE_ID);
            var username = User.FindFirstValue(CLAIM_USERNAME);

            if (string.IsNullOrEmpty(username)) return Unauthorized(new { message = ERROR_TOKEN_INVALID });
            if (string.IsNullOrEmpty(roleId)) return Unauthorized(new { message = ERROR_TOKEN_INVALID });

            var res = await _PengubahanNilaiRepository.GetListKonsentrasiAsync(username, roleId);
            return Ok(res);
        }

        [HttpGet("tahun-ajaran")]
        [RequiresPermission("permintaan_pengubahan_nilai.view")]
        public async Task<IActionResult> GetListTahunAjaran()
        {
            var res = await _PengubahanNilaiRepository.GetListTahunAjaranAsync();
            return Ok(res);
        }

        [HttpGet("tahun-akademik/active")]
        [RequiresPermission("permintaan_pengubahan_nilai.view")]
        public async Task<IActionResult> GetActiveTahunAkademik()
        {
            var res = await _PengubahanNilaiRepository.GetActiveTahunAkademikAsync();
            return Ok(res);
        }

        [HttpGet("mata-kuliah")]
        [RequiresPermission("permintaan_pengubahan_nilai.view")]
        public async Task<IActionResult> GetListMataKuliah([FromQuery] string tahunAjaran, [FromQuery] string semester, [FromQuery] string? roleId)
        {
            var username = User.FindFirstValue(CLAIM_USERNAME);
            var userRoleId = User.FindFirstValue(CLAIM_ROLE_ID);
            var effectiveRoleId = string.IsNullOrEmpty(roleId) ? userRoleId : roleId;

            if (string.IsNullOrEmpty(username)) return Unauthorized(new { message = ERROR_TOKEN_INVALID });
            if (string.IsNullOrEmpty(effectiveRoleId)) return Unauthorized(new { message = ERROR_TOKEN_INVALID });

            var res = await _PengubahanNilaiRepository.GetListMataKuliahAsync(tahunAjaran, semester, effectiveRoleId, username);
            return Ok(res);
        }

        [HttpGet("kelas")]
        [RequiresPermission("permintaan_pengubahan_nilai.view")]
        public async Task<IActionResult> GetListKelas([FromQuery] string tahunAjaran, [FromQuery] string semester, [FromQuery] string mataKuliahId, [FromQuery] string? roleId)
        {
            var username = User.FindFirstValue(CLAIM_USERNAME);
            var userRoleId = User.FindFirstValue(CLAIM_ROLE_ID);
            var effectiveRoleId = string.IsNullOrEmpty(roleId) ? userRoleId : roleId;

            if (string.IsNullOrEmpty(username)) return Unauthorized(new { message = ERROR_TOKEN_INVALID });
            if (string.IsNullOrEmpty(effectiveRoleId)) return Unauthorized(new { message = ERROR_TOKEN_INVALID });

            var res = await _PengubahanNilaiRepository.GetListKelasAsync(tahunAjaran, semester, mataKuliahId, effectiveRoleId, username);
            return Ok(res);
        }
    }
}