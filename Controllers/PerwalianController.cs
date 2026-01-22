using astratech_apps_backend.DTOs.Perwalian.Pelaksanaan;
using astratech_apps_backend.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using astratech_apps_backend.Helpers;
using DTOs.Perwalian.Pelaksanaan;

namespace astratech_apps_backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PerwalianController : ControllerBase
{
    private readonly IPerwalianRepository _repo;

    public PerwalianController(IPerwalianRepository repo)
    {
        _repo = repo;
    }

    [HttpGet("GetAll")]
    [RequiresPermission("pelaksanaan_perwalian.view")]
    public async Task<IActionResult> GetAll(
        string? search = "",
        string urut = "pelaksanaan_perwalian desc")
    {
        var role = User.FindFirst("role")?.Value ?? "";
        var username = User.Identity?.Name ?? "";

        var req = new GetAllPelaksanaanRequestDto
        {
            SearchKeyword = search,
            Urut = urut,
            Role = role,
            Username = username
        };

        var sanitizedReq = SanitizerHelper.EncodeObject(req);
        if (sanitizedReq == null) return NotFound();

        var (data, total) = await _repo.GetAll(sanitizedReq);
        return Ok(new { data, total });
    }

    [HttpGet("Detail/{id}")]
    [RequiresPermission("pelaksanaan_perwalian.view")]
    public async Task<IActionResult> GetDetail(int id)
    {
        var data = await _repo.GetById(id);
        return data == null ? NotFound() : Ok(data);
    }

    [HttpPost("Create")]
    [RequiresPermission("pelaksanaan_perwalian.create")]
    public async Task<IActionResult> Create([FromBody] CreatePelaksanaanRequestDto dto)
    {
        var sanitizedDto = SanitizerHelper.EncodeObject(dto);
        if (sanitizedDto == null) return NotFound();

        var entity = new Models.Perwalian.Perwalian
        {
            IdMahasiswa = sanitizedDto.IdMahasiswa,
            IdDosen = sanitizedDto.IdDosen,
            Subjek = sanitizedDto.Subjek,
            Status = "ACTIVE"
        };

        var created = await _repo.Create(entity);
        return Ok(new { message = "Berhasil dibuat", data = created });
    }

    [HttpPut("Edit/{id}")]
    [RequiresPermission("pelaksanaan_perwalian.edit")]
    public async Task<IActionResult> Edit(int id, [FromBody] UpdatePelaksanaanRequestDto dto)
    {
        var sanitizedDto = SanitizerHelper.EncodeObject(dto);
        if (sanitizedDto == null) return NotFound();

        var ok = await _repo.Update(id, sanitizedDto);
        return ok ? Ok(new { message = "Berhasil diperbarui" }) : NotFound();
    }

    [HttpDelete("Delete/{id}")]
    [RequiresPermission("pelaksanaan_perwalian.delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var role =
            User.FindFirst("roleId")?.Value ??
            User.FindFirst("idrole")?.Value ??
            User.FindFirst("role")?.Value ??
            "";

        role = role.ToUpper();

        if (role != "ROL22" && role != "ROL25")
            return BadRequest("role / permission tidak sesuai");

        var ok = await _repo.Delete(id);
        return ok ? Ok("Berhasil soft delete") : NotFound();
    }

    [HttpPost("AddDetail")]
    [RequiresPermission("pelaksanaan_perwalian.edit")]
    public async Task<IActionResult> AddDetail([FromForm] CreatePelaksanaanDetailRequestDto dto)
    {
        string? fileName = null;

        if (dto.File != null)
        {
            var folder = Path.Combine("Uploads", "Perwalian");
            Directory.CreateDirectory(folder);

            fileName = $"{Guid.NewGuid()}_{dto.File.FileName}";
            var fullPath = Path.Combine(folder, fileName);

            using var stream = new FileStream(fullPath, FileMode.Create);
            await dto.File.CopyToAsync(stream);
        }

        var detail = new Models.Perwalian.PerwalianDetail
        {
            IdPerwalian = dto.IdPerwalian,
            Pesan = dto.Pesan,
            Tipe = dto.Tipe,
            Berkas = fileName,
            Status = "ACTIVE"
        };

        var result = await _repo.AddDetail(detail);
        return Ok(result);
    }

    [HttpGet("Export")]
    [RequiresPermission("pelaksanaan_perwalian.export")]
    public async Task<IActionResult> Export([FromQuery] string search = "")
    {
        var role = User.FindFirst("role")?.Value ?? "";
        var username = User.Identity?.Name ?? "";

        var req = new GetAllPelaksanaanRequestDto
        {
            SearchKeyword = search,
            Role = role,
            Username = username,
            Urut = "pelaksanaan_perwalian desc"
        };

        var sanitizedReq = SanitizerHelper.EncodeObject(req);
        if (sanitizedReq == null) return NotFound();

        var (data, _) = await _repo.GetAll(sanitizedReq);

        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Perwalian");

        ws.Cell(1, 1).Value = "NIM";
        ws.Cell(1, 2).Value = "Dosen";
        ws.Cell(1, 3).Value = "Subjek";
        ws.Cell(1, 4).Value = "Status";

        int row = 2;
        foreach (var x in data)
        {
            ws.Cell(row, 1).Value = x.IdMahasiswa;
            ws.Cell(row, 2).Value = x.IdDosen;
            ws.Cell(row, 3).Value = x.Subjek;
            ws.Cell(row, 4).Value = x.Status;
            row++;
        }

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        stream.Position = 0;

        return File(
            stream.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "Perwalian.xlsx"
        );
    }

    [HttpGet("Dropdown/Mahasiswa")]
    public async Task<IActionResult> DropdownMahasiswa()
    {
        return Ok(await _repo.GetDropdownMahasiswa(""));
    }

    [HttpGet("Dropdown/Dosen")]
    public async Task<IActionResult> DropdownDosen()
    {
        return Ok(await _repo.GetDropdownDosen(""));
    }

    [HttpGet("Dropdown/Prodi")]
    public async Task<IActionResult> DropdownProdi()
    {
        return Ok(await _repo.GetListProdiAsync());
    }

    [HttpGet("Dropdown/MahasiswaByProdi")]
    public async Task<IActionResult> GetMahasiswaByProdi([FromQuery] int idkonsentrasi)
    {
        return Ok(await _repo.GetMahasiswaByProdi(idkonsentrasi));
    }

    [HttpGet("Dropdown/DosenByProdi")]
    public async Task<IActionResult> DropdownDosenByProdi([FromQuery] string prodi)
    {
        prodi = SanitizerHelper.EncodeString(prodi);
        return Ok(await _repo.GetDropdownDosenByProdi(prodi));
    }

    [HttpGet("Dropdown/AngkatanByProdi")]
    public async Task<IActionResult> DropdownAngkatanByProdi([FromQuery] string prodi)
    {
        prodi = SanitizerHelper.EncodeString(prodi);
        return Ok(await _repo.GetDropdownAngkatanByProdi(prodi));
    }
}
