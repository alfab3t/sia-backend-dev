using astratech_apps_backend.DTOs.TagihanCalonMahasiswa;
using astratech_apps_backend.Helpers;
using astratech_apps_backend.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace astratech_apps_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TagihanCalonMahasiswaController(ITagihanCalonMahasiswaRepository repo) : ControllerBase
    {
        private const string CurrencyFormat = "#,##0;(#,##0)";
        private readonly ITagihanCalonMahasiswaRepository _repo = repo;

        [HttpGet("GetAllTagihanCalonMahasiswa")]
        [RequiresPermission("tagihan_calon_mahasiswa.view")]
        public async Task<IActionResult> GetAllTagihanCalonMahasiswa([FromQuery] GetAllTagihanCalonMahasiswaRequest dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var sanitizedDto = SanitizerHelper.EncodeObject(dto);
            if (sanitizedDto == null) return NotFound();

            var result = await _repo.GetAllAsync(sanitizedDto);

            return Ok(result);
        }

        [HttpGet("ExportTagihanCalonMahasiswa")]
        [RequiresPermission("tagihan_calon_mahasiswa.export")]
        public async Task<IActionResult> ExportTagihanCalonMahasiswa([FromQuery] GetAllTagihanCalonMahasiswaRequest dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var sanitizedDto = SanitizerHelper.EncodeObject(dto);
                if (sanitizedDto == null) return NotFound();

                var data = await _repo.GetForExportAsync(sanitizedDto);

                using var package = new ExcelPackage();
                var ws = package.Workbook.Worksheets.Add("Tagihan Calon Mahasiswa");

                ws.View.ShowGridLines = false;

                int currentRow = 1;
                ws.Cells[currentRow, 1].Value = "TAGIHAN CALON MAHASISWA";
                ws.Cells[currentRow, 1, currentRow, 9].Merge = true;
                ws.Cells[currentRow, 1].Style.Font.Size = 16;
                ws.Cells[currentRow, 1].Style.Font.Bold = true;
                ws.Cells[currentRow, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[currentRow, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                currentRow += 2;

                var headers = new string[]
                {
                    "No", "NIM", "Nama", "Prodi", "Angkatan",
                    "Jalur Daftar", "Total Tagihan (Rp)", "Total Pembayaran (Rp)", "Sisa Tagihan (Rp)"
                };

                for (int col = 1; col <= headers.Length; col++)
                {
                    ws.Cells[currentRow, col].Value = headers[col - 1];
                    ws.Cells[currentRow, col].Style.Font.Bold = true;
                    ws.Cells[currentRow, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[currentRow, col].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                    ws.Cells[currentRow, col].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    ws.Cells[currentRow, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[currentRow, col].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                }

                currentRow++;

                foreach (var item in data)
                {
                    ws.Cells[currentRow, 1].Value = item.RowNumber;
                    ws.Cells[currentRow, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[currentRow, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    ws.Cells[currentRow, 2].Value = item.NIM;
                    ws.Cells[currentRow, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[currentRow, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    ws.Cells[currentRow, 3].Value = item.Nama;
                    ws.Cells[currentRow, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    ws.Cells[currentRow, 3].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    ws.Cells[currentRow, 4].Value = item.Prodi;
                    ws.Cells[currentRow, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[currentRow, 4].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    ws.Cells[currentRow, 5].Value = item.Angkatan;
                    ws.Cells[currentRow, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[currentRow, 5].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    ws.Cells[currentRow, 6].Value = item.JalurMasuk;
                    ws.Cells[currentRow, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[currentRow, 6].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    ws.Cells[currentRow, 7].Value = item.TotalTagihan;
                    ws.Cells[currentRow, 7].Style.Numberformat.Format = CurrencyFormat;
                    ws.Cells[currentRow, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    ws.Cells[currentRow, 7].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    ws.Cells[currentRow, 8].Value = item.TotalPembayaran;
                    ws.Cells[currentRow, 8].Style.Numberformat.Format = CurrencyFormat;
                    ws.Cells[currentRow, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    ws.Cells[currentRow, 8].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    ws.Cells[currentRow, 9].Value = item.SisaTagihan;
                    ws.Cells[currentRow, 9].Style.Numberformat.Format = CurrencyFormat;
                    ws.Cells[currentRow, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    ws.Cells[currentRow, 9].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    for (int col = 1; col <= 9; col++)
                    {
                        ws.Cells[currentRow, col].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    }

                    currentRow++;
                }

                ws.Cells[1, 1, currentRow - 1, 9].AutoFitColumns();

                var tableRange = ws.Cells[3, 1, currentRow - 1, 9];
                tableRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                tableRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                tableRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                tableRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                ws.Row(3).Height = 25;

                var fileBytes = await package.GetAsByteArrayAsync();

                return File(
                    fileBytes,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    $"TagihanCalonMahasiswa_{DateTime.Now:yyyyMMddHHmmss}.xlsx"
                );
            }
            catch
            {
                return StatusCode(500, new
                {
                    Message = "Gagal export Excel"
                });
            }
        }

        [HttpGet("GetListProdi")]
        [RequiresPermission("tagihan_calon_mahasiswa.view")]
        public async Task<IActionResult> GetListProdi()
        {
            try
            {
                var data = await _repo.GetListProdiAsync();

                var result = data.Select(x => new
                {
                    Value = x.Id,
                    Text = x.Nama
                }).ToList();

                var jsonOptions = new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNamingPolicy = null
                };

                return new JsonResult(new
                {
                    success = true,
                    data = result
                }, jsonOptions);
            }
            catch
            {
                var jsonOptions = new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNamingPolicy = null
                };

                return new JsonResult(new
                {
                    success = false,
                    message = "Gagal mengambil daftar program studi"
                }, jsonOptions)
                { StatusCode = 500 };
            }
        }

        [HttpGet("GetListAngkatan")]
        [RequiresPermission("tagihan_calon_mahasiswa.view")]
        public async Task<IActionResult> GetListAngkatan()
        {
            try
            {
                var data = await _repo.GetListAngkatanAsync();

                var sortedData = data.OrderByDescending(x => x).ToList();

                var result = sortedData.Select(x => new
                {
                    Value = x,
                    Text = x
                }).ToList();

                var jsonOptions = new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNamingPolicy = null
                };

                return new JsonResult(new
                {
                    success = true,
                    data = result
                }, jsonOptions);
            }
            catch
            {
                var jsonOptions = new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNamingPolicy = null
                };

                return new JsonResult(new
                {
                    success = false,
                    message = "Gagal mengambil daftar angkatan"
                }, jsonOptions)
                { StatusCode = 500 };
            }
        }
    }
}

