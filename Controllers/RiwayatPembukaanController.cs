using astratech_apps_backend.DTOs.RiwayatPembukuan;
using astratech_apps_backend.Helpers;
using astratech_apps_backend.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

namespace astratech_apps_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RiwayatPembukuanController(IRiwayatPembukuanRepository repo) : ControllerBase
    {
        private readonly IRiwayatPembukuanRepository _repo = repo;

        [HttpGet("GetAllRiwayatPembukuan")]
        [RequiresPermission("riwayat_pembukuan.view")]
        public async Task<IActionResult> GetAllRiwayatPembukuan([FromQuery] GetAllRiwayatPembukuanRequest dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var sanitizedDto = SanitizerHelper.EncodeObject(dto);
            if (sanitizedDto == null) return NotFound();

            var (list, totalData) = await _repo.GetAllAsync(sanitizedDto);

            var dataDTO = list.Select(m => new RiwayatPembukuanDto
            {
                RowNumber = m.RowNumber ?? 0,
                Id = m.Id,
                ProgramStudi = m.Program,
                VirtualAccount = m.VirtualAccount,
                TahunAkademik = m.AcademicYear,
                NIM = m.StudentId,
                NamaMahasiswa = m.StudentName,
                Tanggal = m.TransactionDate,
                Jumlah = m.Amount,
                Keterangan = m.Description
            }).ToList();

            var response = new GetAllRiwayatPembukuanResponse
            {
                Data = dataDTO,
                TotalData = totalData,
                TotalHalaman = ((totalData - 1) / dto.PageSize) + 1
            };

            return Ok(response);
        }

        [HttpGet("ExportRiwayatPembukuan")]
        [RequiresPermission("riwayat_pembukuan.export")]
        public async Task<IActionResult> ExportRiwayatPembukuan([FromQuery] GetAllRiwayatPembukuanRequest dto)
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
                var ws = package.Workbook.Worksheets.Add("Riwayat Pembukuan");

                ws.View.ShowGridLines = false;

                int currentRow = 1;
                ws.Cells[currentRow, 1].Value = "RIWAYAT PEMBUKUAN";
                ws.Cells[currentRow, 1, currentRow, 9].Merge = true;
                ws.Cells[currentRow, 1].Style.Font.Size = 16;
                ws.Cells[currentRow, 1].Style.Font.Bold = true;
                ws.Cells[currentRow, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                ws.Cells[currentRow, 1].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                currentRow += 2;

                var headers = new string[]
                {
                    "No", "Virtual Account", "NIM/No daftar", "Prodi", "Nama",
                    "Tahun ajaran", "Waktu/Tanggal pembukuan", "Jumlah", "Keterangan"
                };

                for (int col = 1; col <= headers.Length; col++)
                {
                    ws.Cells[currentRow, col].Value = headers[col - 1];
                    ws.Cells[currentRow, col].Style.Font.Bold = true;
                    ws.Cells[currentRow, col].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    ws.Cells[currentRow, col].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                    ws.Cells[currentRow, col].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    ws.Cells[currentRow, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    ws.Cells[currentRow, col].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                }

                currentRow++;

                foreach (var item in data)
                {
                    ws.Cells[currentRow, 1].Value = item.No;
                    ws.Cells[currentRow, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    ws.Cells[currentRow, 1].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                    ws.Cells[currentRow, 2].Value = item.VirtualAccount;
                    ws.Cells[currentRow, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    ws.Cells[currentRow, 2].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                    ws.Cells[currentRow, 3].Value = item.NIMOrNoDaftar;
                    ws.Cells[currentRow, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    ws.Cells[currentRow, 3].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                    ws.Cells[currentRow, 4].Value = item.Prodi;
                    ws.Cells[currentRow, 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    ws.Cells[currentRow, 4].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                    ws.Cells[currentRow, 5].Value = item.Nama;
                    ws.Cells[currentRow, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    ws.Cells[currentRow, 5].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                    ws.Cells[currentRow, 6].Value = item.TahunAjaran;
                    ws.Cells[currentRow, 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    ws.Cells[currentRow, 6].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                    ws.Cells[currentRow, 7].Value = item.WaktuPembukuan.ToString("yyyy-MM-dd HH:mm");
                    ws.Cells[currentRow, 7].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    ws.Cells[currentRow, 7].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                    ws.Cells[currentRow, 8].Value = item.Jumlah;
                    ws.Cells[currentRow, 8].Style.Numberformat.Format = "#,##0"; 
                    ws.Cells[currentRow, 8].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                    ws.Cells[currentRow, 8].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                    ws.Cells[currentRow, 9].Value = item.Keterangan;
                    ws.Cells[currentRow, 9].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    ws.Cells[currentRow, 9].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                    for (int col = 1; col <= 9; col++)
                    {
                        ws.Cells[currentRow, col].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    }

                    currentRow++;
                }

                ws.Cells[1, 1, currentRow - 1, 9].AutoFitColumns();

                var tableRange = ws.Cells[3, 1, currentRow - 1, 9];
                tableRange.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                tableRange.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                tableRange.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                tableRange.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;

                ws.Row(3).Height = 25;

                var fileBytes = await package.GetAsByteArrayAsync();

                return File(
                    fileBytes,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    $"RiwayatPembukuan_{DateTime.Now:yyyyMMddHHmmss}.xlsx"
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

        [HttpGet("GetListKonsentrasi")]
        [RequiresPermission("riwayat_pembukuan.view")]
        public async Task<IActionResult> GetListKonsentrasi()
        {
            try
            {
                var data = await _repo.GetListKonsentrasiAsync();

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

        [HttpGet("GetListTahunAjaran")]
        [RequiresPermission("riwayat_pembukuan.view")]
        public async Task<IActionResult> GetListTahunAjaran()
        {
            try
            {
                var data = await _repo.GetListTahunAjaranAsync();

                var filteredData = FilterTahunAjaran(data);

                var sortedData = filteredData.OrderByDescending(x => x).ToList();

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
                    message = "Gagal mengambil daftar tahun akademik",
                }, jsonOptions)
                { StatusCode = 500 };
            }
        }

        [HttpGet("GetActiveTahunAjaran")]
        [RequiresPermission("riwayat_pembukuan.view")]
        public async Task<IActionResult> GetActiveTahunAjaran()
        {
            try
            {
                var tahunAjaran = await _repo.GetActiveTahunAjaranByTanggalAsync();

                var jsonOptions = new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNamingPolicy = null
                };

                return new JsonResult(new
                {
                    success = true,
                    tahunAjaran = tahunAjaran ?? ""
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
                    message = "Gagal mengambil tahun akademik aktif",
                }, jsonOptions)
                { StatusCode = 500 };
            }
        }

        private static List<string> FilterTahunAjaran(IEnumerable<string> tahunAjaranList)
        {
            var currentYear = DateTime.Now.Year;

            return [.. tahunAjaranList
                .Where(tahun => !string.IsNullOrEmpty(tahun) && tahun.Contains('/'))
                .Where(tahun =>
                {
                    var parts = tahun.Split('/');
                    return parts.Length == 2 && int.TryParse(parts[0], out int startYear) && startYear <= currentYear;
                })];
        }
    }
}