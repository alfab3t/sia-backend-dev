using astratech_apps_backend.DTOs.MasterIndustri;
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

    public class MasterIndustriController(IMasterIndustriRepository repo) : ControllerBase
    {
        private readonly IMasterIndustriRepository _repo = repo;

        [HttpGet("GetAllMasterIndustri")]
        [RequiresPermission("master_industri.view")]
        public async Task<IActionResult> GetAllMasterIndustri([FromQuery] GetAllMasterIndustriRequest dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var sanitizedDto = SanitizerHelper.EncodeObject(dto);
            if (sanitizedDto == null)
                return NotFound();

            var (list, totalData) = await _repo.GetAllAsync(sanitizedDto);

            var dataDTO = list.Select(x => new MasterIndustriDto
            {
                Id = x.Id,
                RowNumber = x.RowNumber,
                NamaIndustri = x.NamaIndustri,
                Cabang = x.Cabang,
                Grup = x.Grup,
                Status = x.Status
            }).ToList();

            var response = new GetAllMasterIndustriResponse
            {
                Data = dataDTO,
                TotalData = totalData,
                TotalHalaman = dto.PageSize > 0
                    ? ((totalData - 1) / dto.PageSize) + 1
                    : 0
            };

            return Ok(response);
        }

        [HttpGet("DetailMasterIndustri/{id}")]
        [RequiresPermission("master_industri.view")]
        public async Task<IActionResult> GetMasterIndustriById(int id)
        {
            var dataDto = await _repo.GetByIdAsync(id);
            if (dataDto == null)
            {
                return NotFound(new { message = "Data industri tidak ditemukan." });
            }
            return Ok(dataDto);
        }

        [HttpPost("CreateMasterIndustri")]
        [RequiresPermission("master_industri.create")]
        public async Task<IActionResult> CreateMasterIndustri([FromBody] CreateMasterIndustriRequest dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var username = User.FindFirstValue("namaakun");
            if (string.IsNullOrEmpty(username))
                return Unauthorized();

            var sanitizedDto = SanitizerHelper.EncodeObject(dto);

            if (string.IsNullOrEmpty(username))
                return Unauthorized();

            if (sanitizedDto == null)
                return NotFound();

            var newId = await _repo.CreateAsync(sanitizedDto, username);

            return Ok(new { message = "SUCCESS", id = newId });
        }

        [HttpPut("EditMasterIndustri")]
        [RequiresPermission("master_industri.edit")]
        public async Task<IActionResult> UpdateMasterIndustri([FromBody] UpdateMasterIndustriRequest dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var username = User.FindFirstValue("namaakun");
            if (string.IsNullOrEmpty(username))
                return Unauthorized();

            var sanitizedDto = SanitizerHelper.EncodeObject(dto);
            if (sanitizedDto == null)
                return NotFound();

            var success = await _repo.UpdateAsync(sanitizedDto, username);
            if (!success)
                return NotFound(new { message = "Data industri tidak ditemukan." });

            return Ok(new { message = "SUCCESS" });
        }

        [HttpPost("SetStatus/{id}")]
        [RequiresPermission("master_industri.edit")]
        public async Task<IActionResult> SetStatusMasterIndustri(int id)
        {
            var username = User.FindFirstValue("namaakun");
            if (string.IsNullOrEmpty(username))
                return Unauthorized();

            var success = await _repo.SetStatusAsync(id, username);
            if (!success)
                return NotFound(new { message = "Data industri tidak ditemukan." });

            return Ok(new { message = "SUCCESS" });
        }
    }
}
