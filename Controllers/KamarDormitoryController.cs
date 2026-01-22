using astratech_apps_backend.DTOs.Kamar_Dormitory;
using astratech_apps_backend.Helpers;
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
    public class KamarDormitoryController(IKamarDormitoryRepository repo) : ControllerBase
    {
        private readonly IKamarDormitoryRepository _repo = repo;

        private const string NamaAkun = "namaakun";
        private const string Success = "SUCCESS";

        [HttpGet("GetDefaultAtributKamar")]
        [RequiresPermission("kamar_dormitory.view")]
        public async Task<IActionResult> GetDefaultAttributes()
        {
            var attrs = await _repo.GetDefaultAttributesAsync();
            return Ok(attrs);
        }

        [HttpGet("GetAllKamarDormitory")]
        [RequiresPermission("kamar_dormitory.view")]
        public async Task<IActionResult> GetAllKamarDormitory([FromQuery] GetAllKamarDormitoryRequest dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var sanitizedDto = SanitizerHelper.EncodeObject(dto);
            if (sanitizedDto == null)
                return NotFound();

            var (list, totalData) = await _repo.GetAllAsync(sanitizedDto);

            var dataDTO = list.Select(m => new KamarDormitoryDto
            {
                Id = m.Id,
                RowNumber = m.RowNumber,
                KodeKamar = m.KodeKamar,
                Penghuni = m.Penghuni,
                JenisKamar = m.JenisKamar,
                Kondisi = m.Kondisi,
                Status = m.Status
            }).ToList();

            var response = new GetAllKamarDormitoryResponse
            {
                Data = dataDTO,
                TotalData = totalData,
                TotalHalaman = ((totalData - 1) / dto.PageSize) + 1
            };

            return Ok(response);
        }

        [HttpGet("DetailKamarDormitory/{id}")]
        [RequiresPermission("kamar_dormitory.view")]
        public async Task<IActionResult> GetKamarDormitoryById(int id)
        {
            var dataDto = await _repo.GetByIdAsync(id);
            if (dataDto == null)
            {
                return NotFound(new { message = "Data kamar dormitory tidak ditemukan." });
            }
            return Ok(dataDto);
        }

[HttpPost("CreateKamarDormitory")]
[RequiresPermission("kamar_dormitory.create")]
public async Task<IActionResult> CreateKamarDormitory([FromBody] CreateKamarDormitoryRequest dto)
{
    if (!ModelState.IsValid)
        return BadRequest(ModelState);

    var username = User.FindFirstValue(NamaAkun);
    if (string.IsNullOrEmpty(username))
        return Unauthorized();

    var sanitizedDto = SanitizerHelper.EncodeObject(dto);
    if (sanitizedDto == null)
        return NotFound();

    var newId = await _repo.CreateAsync(sanitizedDto, username);

    return Ok(new { message = Success, id = newId });
}

        [HttpPut("EditKamarDormitory")]
        [RequiresPermission("kamar_dormitory.edit")]
        public async Task<IActionResult> UpdateKamarDormitory([FromBody] UpdateKamarDormitoryRequest dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var username = User.FindFirstValue(NamaAkun);
            if (string.IsNullOrEmpty(username))
                return Unauthorized();

            var sanitizedDto = SanitizerHelper.EncodeObject(dto);
            if (sanitizedDto == null)
                return NotFound();

            var success = await _repo.UpdateAsync(sanitizedDto, username);
            if (!success)
            {
                return NotFound(new { message = "Data kamar dormitory tidak ditemukan." });
            }

            return Ok(new { message = Success });
        }

        [HttpPost("SetStatusKamarDormitory/{id}")]
        [RequiresPermission("kamar_dormitory.edit")]
        public async Task<IActionResult> SetStatusKamarDormitory(int id)
        {
            var username = User.FindFirstValue(NamaAkun);
            if (string.IsNullOrEmpty(username))
                return Unauthorized();

            var success = await _repo.SetStatusAsync(id);
            if (!success)
            {
                return NotFound(new { message = "Data kamar dormitory tidak ditemukan." });
            }

            return Ok(new { message = Success });
        }
        [HttpGet("GetPenghuniKamar")]
        [RequiresPermission("kamar_dormitory.view")]
        public async Task<IActionResult> GetPenghuniKamar([FromQuery] GetPenghuniKamarRequest dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (dto.Status != "Aktif" && dto.Status != "Riwayat")
                return BadRequest(new { message = "Status harus 'Aktif' atau 'Riwayat'" });

            var sanitizedDto = SanitizerHelper.EncodeObject(dto);
            if (sanitizedDto == null)
                return NotFound();

            var (list, totalData, jumlahAktif) = await _repo.GetPenghuniKamarAsync(sanitizedDto);

            var response = new GetAllPenghuniKamarResponse
            {
                Data = list.ToList(),
                TotalData = totalData,
                TotalHalaman = ((totalData - 1) / dto.PageSize) + 1,
                JumlahAktif = jumlahAktif
            };

            return Ok(response);
        }

        [HttpGet("DetailPenghuniKamar/{idPenghuni}")]
        [RequiresPermission("kamar_dormitory.view")]
        public async Task<IActionResult> GetDetailPenghuniKamar(int idPenghuni)
        {
            var dataDto = await _repo.GetDetailPenghuniKamarAsync(idPenghuni);
            if (dataDto == null)
            {
                return NotFound(new { message = "Data penghuni kamar tidak ditemukan." });
            }
            return Ok(dataDto);
        }

        [HttpPost("TambahPenghuni")]
        [RequiresPermission("kamar_dormitory.create")]
        public async Task<IActionResult> TambahPenghuni([FromForm] CreatePenghuniKamarRequest dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .Select(x => new
                    {
                        Field = x.Key,
                        Errors = x.Value.Errors.Select(e => e.ErrorMessage)
                    });

                return BadRequest(new
                {
                    message = "ModelState Invalid",
                    errors
                });
            }

            var username = User.FindFirstValue("namaakun");
            if (string.IsNullOrEmpty(username))
                return Unauthorized();

            if (dto.ScanKtp != null)
            {
                var folder = Path.Combine("Uploads", "KTP");
                Directory.CreateDirectory(folder);

                var original = Path.GetFileName(dto.ScanKtp.FileName);
                dto.ScanKtpFileName = $"{Guid.NewGuid()}_{original}";
                var path = Path.Combine(folder, dto.ScanKtpFileName);

                using var stream = System.IO.File.Create(path);
                await dto.ScanKtp.CopyToAsync(stream);
            }

            var sanitizedDto = SanitizerHelper.EncodeObject(dto);
            if (sanitizedDto == null)
                return BadRequest("Invalid data");

            var success = await _repo.CreatePenghuniAsync(sanitizedDto, username);

            if (!success)
                return BadRequest(new { message = "Gagal menambah penghuni" });

            return Ok(new { message = "SUCCESS" });
        }

        [HttpPost("PindahPenghuni")]
        [RequiresPermission("kamar_dormitory.edit")]
        public async Task<IActionResult> PindahPenghuni([FromBody] PindahPenghuniRequest dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var username = User.FindFirstValue("namaakun");
            if (string.IsNullOrEmpty(username))
                return Unauthorized();

            var sanitizedDto = SanitizerHelper.EncodeObject(dto);
            if (sanitizedDto == null)
                return NotFound();

            var success = await _repo.PindahPenghuniAsync(sanitizedDto, username);
            if (!success)
            {
                return BadRequest(new { message = "Gagal memindahkan penghuni." });
            }

            return Ok(new { message = "SUCCESS" });
        }

        [HttpPost("CheckoutPenghuni")]
        [RequiresPermission("kamar_dormitory.edit")]
        public async Task<IActionResult> CheckoutPenghuni([FromBody] CheckoutPenghuniRequest dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var username = User.FindFirstValue("namaakun");
            if (string.IsNullOrEmpty(username))
                return Unauthorized();

            var sanitizedDto = SanitizerHelper.EncodeObject(dto);
            if (sanitizedDto == null)
                return NotFound();

            var success = await _repo.CheckoutPenghuniAsync(sanitizedDto, username);
            if (!success)
            {
                return BadRequest(new { message = "Gagal checkout penghuni." });
            }

            return Ok(new { message = "SUCCESS" });
        }

        [HttpGet("GetAllMahasiswa")]
        [RequiresPermission("kamar_dormitory.view")]
        public async Task<IActionResult> GetAllMahasiswa([FromQuery] GetAllMahasiswaRequest dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var sanitizedDto = SanitizerHelper.EncodeObject(dto);
            if (sanitizedDto == null)
                return NotFound();

            var list = await _repo.GetAllMahasiswaAsync(sanitizedDto);
            return Ok(list);
        }

        [HttpGet("GetKamarDropdown")]
        [RequiresPermission("kamar_dormitory.view")]
        public async Task<IActionResult> GetKamarDropdown()
        {
            try
            {
                var list = await _repo.GetAllKamarDropdownAsync();
                return Ok(list);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Gagal memuat data kamar" });
            }
        }

        [HttpGet("ExportKamarDormitory")]
        [RequiresPermission("kamar_dormitory.export")]
        public async Task<IActionResult> ExportKamarDormitory([FromQuery] ExportKamarDormitoryRequest request)
        {
            try
            {
                var data = await _repo.GetDataForExportAsync(request);

                if (data == null)
                {
                    return StatusCode(500, new { message = "Repository returned null data" });
                }

                var filteredData = data;

                if (!string.IsNullOrWhiteSpace(request.Prodi))
                {
                    filteredData = filteredData
                        .Where(x => !string.IsNullOrEmpty(x.Prodi) && x.Prodi.IndexOf(request.Prodi, StringComparison.OrdinalIgnoreCase) >= 0)
                        .ToList();
                }

                if (!string.IsNullOrWhiteSpace(request.Angkatan))
                {
                    filteredData = filteredData
                        .Where(x => !string.IsNullOrEmpty(x.Angkatan) && x.Angkatan.IndexOf(request.Angkatan, StringComparison.OrdinalIgnoreCase) >= 0)
                        .ToList();
                }

                var excelBytes = ExcelExportService.ExportKamarDormitory(
                    filteredData,
                    request.StartDate,
                    request.EndDate,
                    request.Prodi,
                    request.Angkatan
                );

                var fileName = $"Laporan_Penghuni_Dormitory_{request.StartDate:yyyyMMdd}_sd_{request.EndDate:yyyyMMdd}.xlsx";

                return File(
                    excelBytes,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Terjadi kesalahan internal",
                    error = ex.Message
                });
            }
        }
    }
}