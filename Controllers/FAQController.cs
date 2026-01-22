using astratech_apps_backend.DTOs.Bantuan.FAQ;
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
    public class FAQController(IFAQRepository repo) : ControllerBase
    {
        private readonly IFAQRepository _repo = repo;

        [HttpGet("GetRolesFaq")]
        public async Task<IActionResult> GetRoles()
        {
            try
            {
                var roles = await _repo.GetRolesAsync();
                return Ok(roles);
            }
            catch
            {
                return StatusCode(500, new { message = "Gagal mengambil data role." });
            }
        }

        [HttpGet("GetAllFaq")]
        [RequiresPermission("frequently_asked_questions.view")]
        public async Task<IActionResult> GetAllFAQ([FromQuery] GetAllFAQRequest dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userRoleId = User.FindFirstValue("idrole");
            var username = User.FindFirstValue("namaakun");

            if (string.IsNullOrEmpty(userRoleId) || string.IsNullOrEmpty(username))
            {
                return Unauthorized();
            }

            var sanitizedDto = SanitizerHelper.EncodeObject(dto);
            if (sanitizedDto == null) return NotFound();

            var (list, totalData) = await _repo.GetAllAsync(sanitizedDto, userRoleId, username);

            var dataDTO = list.Select(m => new FAQDto
            {
                FaqId = m.FaqId,
                Pertanyaan = m.Pertanyaan,
                Jawaban = m.Jawaban,
                Status = m.Status,
                NamaKategori = m.NamaKategori ?? "-"
            }).ToList();

            var response = new GetAllFAQResponse
            {
                Data = dataDTO,
                TotalData = totalData,
                TotalHalaman = dto.PageSize > 0 ? ((totalData - 1) / dto.PageSize) + 1 : 1
            };

            return Ok(response);
        }

        [HttpGet("DetailFaq/{id}")]
        [RequiresPermission("frequently_asked_questions.view")]
        public async Task<IActionResult> GetFAQById(int id)
        {
            var userRoleId = User.FindFirstValue("idrole");

            if (string.IsNullOrEmpty(userRoleId))
            {
                return Unauthorized();
            }

            var data = await _repo.GetByIdAsync(id, userRoleId);

            if (data == null)
            {
                return NotFound(new { message = "Data FAQ tidak ditemukan atau Anda tidak memiliki akses." });
            }
            return Ok(data);
        }

        [HttpPost("CreateFaq")]
        [RequiresPermission("frequently_asked_questions.create")]
        public async Task<IActionResult> CreateFAQ([FromBody] CreateFAQRequest dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var username = User.FindFirstValue("namaakun");
            var creatorRoleId = User.FindFirstValue("idrole");

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(creatorRoleId))
            {
                return Unauthorized();
            }

            var sanitizedDto = SanitizerHelper.EncodeObject(dto);
            if (sanitizedDto == null) return NotFound();

            var newId = await _repo.CreateAsync(sanitizedDto, username, creatorRoleId);

            return Ok(new { message = "SUCCESS", id = newId });
        }

        [HttpPut("UpdateFaq")]
        [RequiresPermission("frequently_asked_questions.edit")]
        public async Task<IActionResult> UpdateFAQ([FromBody] UpdateFAQRequest dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var username = User.FindFirstValue("namaakun");
            var userRoleId = User.FindFirstValue("idrole");

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(userRoleId))
            {
                return Unauthorized();
            }

            var sanitizedDto = SanitizerHelper.EncodeObject(dto);
            if (sanitizedDto == null) return NotFound();

            var success = await _repo.UpdateAsync(sanitizedDto, username, userRoleId);

            if (!success)
            {
                return NotFound(new { message = "Data FAQ tidak ditemukan atau gagal diupdate." });
            }
            return Ok(new { message = "SUCCESS" });
        }

        [HttpDelete("DeleteFaq/{id}")]
        [RequiresPermission("frequently_asked_questions.delete")]
        public async Task<IActionResult> DeleteFAQ(int id)
        {
            var username = User.FindFirstValue("namaakun");

            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized();
            }

            var success = await _repo.DeleteAsync(id, username);
            if (!success)
            {
                return NotFound(new { message = "Data FAQ tidak ditemukan atau gagal dihapus." });
            }
            return Ok(new { message = "SUCCESS" });
        }
    }
}