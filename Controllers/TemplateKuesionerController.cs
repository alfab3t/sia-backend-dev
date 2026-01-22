using astratech_apps_backend.DTOs.SkalaPenilaian;
using astratech_apps_backend.DTOs.TemplateKuesioner;
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
    public class TemplateKuesionerController(ITemplateKuesionerRepository repo) : ControllerBase
    {
        private readonly ITemplateKuesionerRepository _repo = repo;

        private const string namaAkun = "namaAkun";
        private const string berhasil = "SUCCESS";
        

        [HttpGet("GetAllTemplateKuesioner")]
        [RequiresPermission("template_kuesioner.view")]
        public async Task<IActionResult> GetAll([FromQuery] GetAllTemplateRequest dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var sanitizedDto = SanitizerHelper.EncodeObject(dto);
            if (sanitizedDto == null) return NotFound();

            var (list, totalData) = await _repo.GetAllAsync(sanitizedDto);
            var dataDto = list.Select(t => new TemplateDto
            {
                Id = t.Id,
                RowNumber = t.RowNumber,
                NamaTemplate = t.NamaTemplate,
                JenisKuesioner = t.JenisKuesioner,
                Skala = t.Skala,
                TanggalFinal = t.TanggalFinal?.ToString("dd-MM-yyyy") ?? "-",
                Status = t.Status
            }).ToList();

            return Ok(new GetAllTemplateResponse
            {
                Data = dataDto,
                TotalData = totalData,
                TotalHalaman = ((totalData - 1) / dto.PageSize) + 1
            });
        }

        [HttpGet("GetByIdTemplateKuesioner/{id}")]
        [RequiresPermission("template_kuesioner.view")]
        public async Task<IActionResult> GetById(int id)
        {
            var data = await _repo.GetByIdAsync(id);
            if (data == null) return NotFound();

            return Ok(new { data });
        }

        [HttpGet("GetPertanyaan/{templateId}")]
        [RequiresPermission("template_kuesioner.view")]
        public async Task<IActionResult> GetPertanyaan(int templateId)
        {
            var result = await _repo.GetPertanyaanByTemplateIdAsync(templateId);
            var dataDto = result.Select(p => new PertanyaanDto
            {
                Id = p.Id,
                PertanyaanText = p.PertanyaanText,
                IsHeader = p.IsHeader ? "Ya" : "Tidak",
                Jenis = p.Jenis
            }).ToList();

            return Ok(dataDto);
        }

        [HttpPost("CreateTemplateKuesioner")]
        [RequiresPermission("template_kuesioner.create")]
        public async Task<IActionResult> CreateTemplate([FromBody] CreateTemplateRequest dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var username = User.FindFirstValue(namaAkun);
            if (string.IsNullOrEmpty(username)) return Unauthorized();

            var sanitizedDto = SanitizerHelper.EncodeObject(dto);
            if (sanitizedDto == null) return BadRequest();

            var newId = await _repo.CreateTemplateAsync(sanitizedDto, username);
            return Ok(new { message = berhasil, id = newId });
        }

        [HttpPut("UpdateTemplateKuesioner")]
        [RequiresPermission("template_kuesioner.edit")]
        public async Task<IActionResult> UpdateTemplate([FromBody] UpdateTemplateRequest dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var username = User.FindFirstValue(namaAkun);
            if (string.IsNullOrEmpty(username)) return Unauthorized();

            var sanitizedDto = SanitizerHelper.EncodeObject(dto);
            if (sanitizedDto == null) return BadRequest(new { message = "Invalid input." });

            var success = await _repo.UpdateTemplateAsync(sanitizedDto, username);
            if (!success) return NotFound(new { message = "Template not found." });
            return Ok(new { message = berhasil });
        }

        [HttpPost("CreatePertanyaan")]
        [RequiresPermission("template_kuesioner.create")]
        public async Task<IActionResult> CreatePertanyaan([FromBody] CreateUpdatePertanyaanRequest dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var username = User.FindFirstValue(namaAkun);
            if (string.IsNullOrEmpty(username)) return Unauthorized();

            var sanitizedDto = SanitizerHelper.EncodeObject(dto);
            if (sanitizedDto == null) return NotFound();

                var newId = await _repo.CreatePertanyaanAsync(sanitizedDto, username);
            return Ok(new { message = berhasil, id = newId });
        }

        [HttpPut("UpdatePertanyaan")]
        [RequiresPermission("template_kuesioner.edit")]
        public async Task<IActionResult> UpdatePertanyaan([FromBody] CreateUpdatePertanyaanRequest dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var sanitizedDto = SanitizerHelper.EncodeObject(dto);
            if (sanitizedDto == null) return NotFound();

            if (dto.Id <= 0)
            {
                return BadRequest();
            }
                
            var username = User.FindFirstValue("namaakun");
            if (string.IsNullOrEmpty(username)) return Unauthorized();

            var success = await _repo.UpdatePertanyaanAsync(dto, username);

            if (!success)
            {
                return NotFound();
            }

            return Ok(new { message = berhasil });
        }



        [HttpPost("SetStatusPertanyaan")]
        [RequiresPermission("template_kuesioner.edit")]
        public async Task<IActionResult> SetStatusPertanyaan([FromBody] SetStatusPertanyaanRequest dto)
        {
            var username = User.FindFirstValue(namaAkun);
            if (string.IsNullOrEmpty(username)) return Unauthorized();

            var success = await _repo.SetStatusPertanyaanAsync(dto.Id, dto.Status, username);
            if (!success) return NotFound(new { message = "Data tidak ditemukan." });

            return Ok(new { message = berhasil });
        }

        [HttpPost("SetStatusTemplateKuesioner")]
        [RequiresPermission("template_kuesioner.edit")]
        public async Task<IActionResult> SetStatus([FromBody] SetStatusTemplateRequest dto)
        {
            var username = User.FindFirstValue(namaAkun);
            if (string.IsNullOrEmpty(username)) return Unauthorized();

            var success = await _repo.SetStatusTemplateAsync(dto.Id, dto.Status, username);
            if (!success) return NotFound(new { message = "Data tidak ditemukan." });

            return Ok(new { message = berhasil });
        }

        [HttpPost("FinalizeTemplateKuesioner/{id}")]
        [RequiresPermission("template_kuesioner.create")]
        public async Task<IActionResult> FinalizeTemplate(int id)
        {
            if (await _repo.IsTemplateEmptyAsync(id))
            {
                return BadRequest();
            }

            var username = User.FindFirstValue(namaAkun);
            if (string.IsNullOrEmpty(username)) return Unauthorized();

            var success = await _repo.SetFinalAsync(id, username);
            if (!success) return NotFound();

            return Ok(new { message = berhasil });
        }

        [HttpGet("GetPreviewTemplateKuesioner/{id}")]
        [RequiresPermission("template_kuesioner.view")]
        public async Task<IActionResult> GetPreview(int id)
        {
            var data = await _repo.GetPreviewAsync(id);

            if (data == null)
            {
                return NotFound(new { message = "Data preview tidak ditemukan." });
            }

            return Ok(data);
        }


        [HttpPost("ImportPertanyaan")]
        [RequiresPermission("template_kuesioner.edit")]
        public async Task<IActionResult> ImportPertanyaan([FromForm] ImportPertanyaanRequest dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var username = User.FindFirstValue("namaakun");
            if (string.IsNullOrEmpty(username)) return Unauthorized();
            var (success, message) = await _repo.ImportPertanyaanAsync(dto, username);

            if (!success)
            {
                return BadRequest(new { message });
            }
            return Ok(new { message });
        }
    }
}