using astratech_apps_backend.DTOs.JamMinusPlus;
using astratech_apps_backend.Helpers;
using astratech_apps_backend.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace astratech_apps_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class JamMinusPlusController : ControllerBase
    {
        private readonly IJamMinusPlusRepository _repo;

        public JamMinusPlusController(IJamMinusPlusRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("GetAll")]
        [RequiresPermission("jam_plus.view")]
        public async Task<IActionResult> GetAll([FromQuery] GetAllJamMinusPlusRequest dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var username = User.FindFirstValue("namaakun");
            if (string.IsNullOrEmpty(username))
                return Unauthorized();

            dto.Username = username;

            var sanitized = SanitizerHelper.EncodeObject(dto);
            if (sanitized == null)
                return NotFound();

            var (list, totalData) = await _repo.GetAllAsync(sanitized);

            var response = new GetAllJamMinusPlusResponse
            {
                ListData = list.Select(jamminusplus => new JamMinusPlusDto
                {
                    MahasiswaId = jamminusplus.MahasiswaId,
                    Nama = jamminusplus.Nama,
                    Prodi = jamminusplus.Prodi,
                    TahunAkademik = jamminusplus.TahunAkademik,
                    Semester = jamminusplus.Semester,
                    Kelas = jamminusplus.Kelas,
                    SisaKompensasi = jamminusplus.SisaKompensasi,
                    SisaMurni = jamminusplus.SisaMurni
                }).ToList(),

                TotalData = totalData,
                TotalHalaman = dto.PageSize > 0
                    ? ((totalData - 1) / dto.PageSize) + 1
                    : 1
            };

            return Ok(response);
        }

        [HttpGet("Detail/{nim}")]
        [RequiresPermission("jam_plus.view")]
        public async Task<IActionResult> Detail(string nim)
        {
            var result = await _repo.GetDetailPeriodeAsync(nim);
            return Ok(result);
        }

        [HttpGet("Konsentrasi")]
        [RequiresPermission("jam_plus.view")]
        public async Task<IActionResult> Konsentrasi()
        {
            return Ok(await _repo.GetListKonsentrasiAsync());
        }

        [HttpGet("ExportExcel")]
        [RequiresPermission("jam_plus.view")]
        public async Task<IActionResult> ExportExcel([FromQuery] GetAllJamMinusPlusRequest dto)
        {
            var username = User.FindFirstValue("namaakun");
            if (string.IsNullOrEmpty(username)) return Unauthorized();

            dto.Username = username;
            dto.RoleID = User.FindFirstValue("rolid")
                         ?? User.FindFirstValue("role")
                         ?? string.Empty;

            var sanitized = SanitizerHelper.EncodeObject(dto);
            if (sanitized == null) return BadRequest();

            sanitized.PageSize = 999999;

            var (list, _) = await _repo.GetAllAsync(sanitized);

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Jam Minus Plus");

                var dataExport = list.Select(jamminusplus => new
                {
                    NIM = jamminusplus.MahasiswaId,
                    Nama_Mahasiswa = jamminusplus.Nama,
                    Program_Studi = jamminusplus.Prodi,
                    Kelas = jamminusplus.Kelas,
                    Tahun_Akademik = jamminusplus.TahunAkademik,
                    Semester = jamminusplus.Semester,
                    Sisa_Kompensasi = jamminusplus.SisaKompensasi,
                    Sisa_Murni = jamminusplus.SisaMurni
                }).ToList();

                worksheet.Cells["A1"].LoadFromCollection(dataExport, true);

                using (var range = worksheet.Cells["A1:H1"])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }

                worksheet.Cells.AutoFitColumns();

                var fileContent = package.GetAsByteArray();
                var fileName = $"Data_Jam_Minus_Plus_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";

                return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }
    }
}
