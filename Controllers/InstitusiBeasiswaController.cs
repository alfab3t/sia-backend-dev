using astratech_apps_backend.DTOs.InstitusiBeasiswa;
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
    public class InstitusiBeasiswaController(IInstitusiBeasiswaRepository repo) : ControllerBase
    {
        private readonly IInstitusiBeasiswaRepository _repo = repo;

        [HttpGet("GetAllInstitusiBeasiswa")]
        [RequiresPermission("institusi_beasiswa.view")]
        public async Task<IActionResult> GetAllInstitusi([FromQuery] GetAllInstitusiBeasiswaRequest dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var sanitizedDto = SanitizerHelper.EncodeObject(dto);
            if (sanitizedDto == null) return NotFound();

            var (list, totalData) = await _repo.GetAllAsync(sanitizedDto);

            var dataDTO = list.Select(m => new InstitusiBeasiswaDto
            {
                Id = m.Id,
                NamaInstitusiBeasiswa = m.NamaInstitusiBeasiswa,
                Alamat = m.Alamat,
                Telepon = m.Telepon,
                Email = m.Email,
                Status = m.Status
            }).ToList();

            var response = new GetAllInstitusiBeasiswaResponse
            {
                Data = dataDTO,
                TotalData = totalData,
                TotalHalaman = ((totalData - 1) / dto.PageSize) + 1
            };

            return Ok(response);
        }

        [HttpGet("GetAllInstitusiBeasiswaDropdown")]
        [RequiresPermission("institusi_beasiswa.view")]
        public async Task<IActionResult> GetAllInstitusiDropdown([FromQuery] GetAllInstitusiBeasiswaDropdownRequest dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var sanitizedDto = SanitizerHelper.EncodeObject(dto);
            if (sanitizedDto == null) return NotFound();

            var (list, totalData) = await _repo.GetAllDropDownAsync(sanitizedDto);

            var dataDTO = list.Select(m => new InstitusiBeasiswaDto
            {
                Id = m.Id,
                NamaInstitusiBeasiswa = m.NamaInstitusiBeasiswa,
                Alamat = m.Alamat,
                Telepon = m.Telepon,
                Email = m.Email,
                Status = m.Status
            }).ToList();
            int totalHalaman = 1;

            if (dto.PageSize > 0)
            {
                totalHalaman = (int)Math.Ceiling((double)totalData / dto.PageSize);
            }

            var response = new GetAllInstitusiBeasiswaDropdownResponse
            {
                Data = dataDTO,
                TotalData = totalData,
                TotalHalaman = totalHalaman
            };


            return Ok(response);
        }

        [HttpGet("DetailInstitusiBeasiswa/{id}")]
        [RequiresPermission("institusi_beasiswa.view")]
        public async Task<IActionResult> GetInstitusiBeasiswaById(short id)
        {
            var dataDto = await _repo.GetByIdAsync(id);

            if (dataDto == null)
            {
                return NotFound(new { message = "Data institusi beasiswa tidak ditemukan." });
            }

            return Ok(dataDto);
        }

        [HttpPost("CreateInstitusiBeasiswa")]
        [RequiresPermission("institusi_beasiswa.create")]
        public async Task<IActionResult> CreateInstitusiBeasiswa([FromBody] CreateInstitusiBeasiswaRequest dto)
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

            var newId = await _repo.CreateAsync(sanitizedDto, username);

            return Ok(new { message = "SUCCESS", id = newId });
        }


        [HttpPut("EditInstitusiBeasiswa")]
        [RequiresPermission("institusi_beasiswa.edit")]
        public async Task<IActionResult> UpdateInstitusiBeasiswa([FromBody] UpdateInstitusiBeasiswaRequest dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var username = User.FindFirstValue("namaakun");
            if (string.IsNullOrEmpty(username))
                return Unauthorized();

            var sanitizedDto = SanitizerHelper.EncodeObject(dto);
            if (sanitizedDto == null)
                return BadRequest(new { message = "Data tidak valid." });

            var success = await _repo.UpdateAsync(dto, username);

            if (!success)
                return NotFound(new { message = "Data institusi Beasiswa tidak ditemukan." });

            return Ok(new { message = "SUCCESS" });
        }

        [HttpPost("SetStatusInstitusiBeasiswa/{id}")]
        [RequiresPermission("institusi_beasiswa.edit")]
        public async Task<IActionResult> SetStatusInstitusiBeasiwa(short id)
        {
            var username = User.FindFirstValue("namaakun");

            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized();
            }

            var success = await _repo.SetStatusAsync(id, username);
            if (!success)
            {
                return NotFound(new { message = "Data institusiBeasiswa tidak ditemukan." });
            }
            if (!success)
                return NotFound(new { message = "Data institusiBeasiswa tidak ditemukan." });

            return Ok(new { message = "SUCCESS" });
        }
    }
}
