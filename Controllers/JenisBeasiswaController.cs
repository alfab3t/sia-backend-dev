using astratech_apps_backend.DTOs.JenisBeasiswa;
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
    public class JenisBeasiswaController : ControllerBase
    {
        private readonly IJenisBeasiswaRepository _repo;

        public JenisBeasiswaController(IJenisBeasiswaRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("GetAllJenisBeasiswa")]
        [RequiresPermission("jenis_beasiswa.view")]
        public async Task<IActionResult> GetAllJenis([FromQuery] GetAllJenisBeasiswaRequest dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var sanitizedDto = SanitizerHelper.EncodeObject(dto);
            if (sanitizedDto == null) return NotFound();

            var (list, totalData) = await _repo.GetAllAsync(sanitizedDto);

            var dataDTO = list.Select(m => new JenisBeasiswaDto
            {
                Id = m.Id,
                NamaInstitusi = m.NamaInstitusi,
                NamaJenisBeasiswa = m.NamaJenisBeasiswa,
                masaSemester = m.masaSemester,
                Status = m.Status
            }).ToList();

            var response = new GetAllJenisBeasiswaResponse
            {
                Data = dataDTO,
                TotalData = totalData,
                TotalHalaman = ((totalData - 1) / dto.PageSize) + 1
            };

            return Ok(response);
        }

        [HttpGet("GetAllJenisBeasiswaDropdown")]
        [RequiresPermission("jenis_beasiswa.view")]
        public async Task<IActionResult> GetAllJenisDropdown([FromQuery] GetAllJenisBeasiswaDropdownRequest dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var sanitizedDto = SanitizerHelper.EncodeObject(dto);
            if (sanitizedDto == null) return NotFound();

            var (list, totalData) = await _repo.GetAllDropdownAsync(sanitizedDto);

            var dataDTO = list.Select(m => new JenisBeasiswaDto
            {
                Id = m.Id,
                NamaInstitusi = m.NamaInstitusi,
                NamaJenisBeasiswa = m.NamaJenisBeasiswa,
                masaSemester = m.masaSemester,
                Status = m.Status
            }).ToList();
            int totalHalaman = 1;

            if (dto.PageSize > 0)
            {
                totalHalaman = (int)Math.Ceiling((double)totalData / dto.PageSize);
            }

            var response = new GetAllJenisBeasiswaDropdownResponse
            {
                Data = dataDTO,
                TotalData = totalData,
                TotalHalaman = totalHalaman
            };


            return Ok(response);
        }


        [HttpGet("DetailJenisBeasiswa/{id}")]
        [RequiresPermission("jenis_beasiswa.view")]
        public async Task<IActionResult> GetJenisBeasiswaById(short id)
        {
            var dataDto = await _repo.GetByIdAsync(id);

            if (dataDto == null)
            {
                return NotFound(new { message = "Data jenis beasiswa tidak ditemukan." });
            }

            return Ok(dataDto);
        }


        [HttpPost("CreateJenisBeasiswa")]
        [RequiresPermission("jenis_beasiswa.create")]
        public async Task<IActionResult> CreateJenisBeasiswa([FromBody] CreateJenisBeasiswaRequest dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var username = User.FindFirstValue("namaakun");
            if (string.IsNullOrEmpty(username)) return Unauthorized();

            var sanitizedDto = SanitizerHelper.EncodeObject(dto);
            if (sanitizedDto == null) return BadRequest();

            var newId = await _repo.CreateAsync(dto, username);

            return Ok(new { message = "SUCCESS", id = newId });
        }

        [HttpPut("EditJenisBeasiswa")]
        [RequiresPermission("jenis_beasiswa.edit")]
        public async Task<IActionResult> UpdateJenisBeasiswa([FromBody] UpdateJenisBeasiswaRequest dto)
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
                return NotFound(new { message = "Data Jenis Beasiswa tidak ditemukan." });

            return Ok(new { message = "SUCCESS" });
        }



        [HttpPost("SetStatusJenisBeasiswa/{id}")]
        [RequiresPermission("jenis_beasiswa.edit")]
        public async Task<IActionResult> SetStatusJenisBeasiswa(short id)
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

            return Ok(new { message = "SUCCESS" });
        }


    }
}
