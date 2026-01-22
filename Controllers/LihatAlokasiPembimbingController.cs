using astratech_apps_backend.DTOs.LihatAlokasiPembimbing;
using astratech_apps_backend.Helpers;
using astratech_apps_backend.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using OfficeOpenXml;
using OfficeOpenXml.Style; 
using System.Globalization;


namespace astratech_apps_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LihatAlokasiPembimbingController(ILihatAlokasiPembimbingRepository repo) : ControllerBase
    {
        private readonly ILihatAlokasiPembimbingRepository _repo = repo;

        [HttpGet("GetAllLihatAlokasiPembimbing")]
        [RequiresPermission("lihat_alokasi_pembimbing.view")]
        public async Task<IActionResult> GetAll([FromQuery] GetAllLihatAlokasiPembimbingRequest dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var sanitizedDto = SanitizerHelper.EncodeObject(dto);
            if (sanitizedDto == null) return NotFound();

            var (list, totalData) = await _repo.GetAllAsync(sanitizedDto);

            var dataDTO = list.Select(m => new LihatAlokasiPembimbingDto
            {
                Id = m.Id,
                RowNumber = m.RowNumber,
                Tanggal = m.Tanggal,
                Konsentrasi = m.Konsentrasi,
                Industri = m.Industri,
                NamaKelompok = m.NamaKelompok,
                AnggotaKelompok = m.AnggotaKelompok,
                NamaDosenPembimbing = m.NamaDosenPembimbing,
                DosenId = m.DosenId,
                JudulProposal = m.JudulProposal,
                JudulProposalAktual = m.JudulProposalAktual,
                Status = m.Status,
                TahunAkademik = m.TahunAkademik,
                KonsentrasiId = m.KonsentrasiId
            }).ToList();

            var response = new GetAllLihatAlokasiPembimbingResponse
            {
                Data = dataDTO, 
                TotalData = totalData,
                TotalHalaman = ((totalData - 1) / dto.PageSize) + 1
            };

            return Ok(response);
        }

        [HttpGet("ExportTA")]
        [RequiresPermission("lihat_alokasi_pembimbing.export")]
        public async Task<IActionResult> ExportToExcel([FromQuery] ExportLihatAlokasiPembimbingRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var dataTable = await _repo.ExportTA(
                    request.TahunAkademik,
                    request.KonsentrasiId
                );

                if (dataTable.Rows.Count == 0)
                {
                    return NotFound(new { message = "Tidak ada data yang ditemukan untuk filter ini." });
                }

                string namaProdi = dataTable.Rows[0]["kon_singkatan"]?.ToString() ?? "Prodi Tidak Diketahui";

                string safeNamaProdi = string.Join("_", namaProdi.Split(Path.GetInvalidFileNameChars()));

                using var package = new ExcelPackage();
                var worksheet = package.Workbook.Worksheets.Add("AlokasiPembimbing");

                string judulUtama = $"Data Pembimbing Tugas Akhir {namaProdi} Tahun Ajaran {request.TahunAkademik}";
                worksheet.Cells["A1"].Value = judulUtama;
                worksheet.Cells["A1:M1"].Merge = true;
                worksheet.Cells["A1"].Style.Font.Bold = true;
                worksheet.Cells["A1"].Style.Font.Size = 14;
                worksheet.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                var cultureIndo = new CultureInfo("id-ID");
                string tanggalUpdate = DateTime.Now.ToString("dddd, d MMMM yyyy | HH.mm", cultureIndo);
                worksheet.Cells["A2"].Value = $"(Terakhir diperbarui pada {tanggalUpdate})";
                worksheet.Cells["A2:M2"].Merge = true;
                worksheet.Cells["A2"].Style.Font.Bold = true;
                worksheet.Cells["A2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                string[] headers = {
                    "No",                           
                    "Program Studi",               
                    "Nama Industri",                
                    "Nama Kelompok",                
                    "Mahasiswa",                    
                    "Judul Tugas Akhir",            
                    "Pembimbing Akademik",          
                    "Nama Pembimbing Industri",     
                    "No HP Pembimbing Industri",    
                    "Email Pembimbing Industri",    
                    "Nama Pembimbing Industri 2",   
                    "No HP Pembimbing Industri 2",  
                    "Email Pembimbing Industri 2"   
                };

                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cells[3, i + 1].Value = headers[i];

                    var cell = worksheet.Cells[3, i + 1];
                    cell.Style.Font.Bold = true;
                    cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    cell.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }

                worksheet.Cells["A4"].LoadFromDataTable(dataTable, false);

                int startRow = 3; 
                int totalData = dataTable.Rows.Count;
                int endRow = startRow + totalData; 
                int endCol = headers.Length;

                using (var range = worksheet.Cells[startRow, 1, endRow, endCol])
                {
                    range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                }

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                var fileBytes = await package.GetAsByteArrayAsync();
                string fileName = $"Data_Industri_{safeNamaProdi.Replace(" ", "_")}.xlsx";
                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                return File(
                    fileBytes,
                    contentType,
                    fileName
                );
            }
            catch 
            {
                return StatusCode(500, new { message = "Terjadi kesalahan internal server saat memproses export data." });
            }
        }

        [HttpGet("GetListKonsentrasi")]
        [RequiresPermission("lihat_alokasi_pembimbing.view")]
        public async Task<IActionResult> GetAllKonsentrasi()
        {
            var username = User.FindFirstValue("namaakun") ?? string.Empty;
            var roleId = User.FindFirstValue("rolid") ?? User.FindFirstValue("role") ?? string.Empty;

            var results = await _repo.GetListKonsentrasiAsync(username, roleId);

            return Ok(results);
        }

        [HttpGet("GetListKonsentrasiByNPK")]
         [RequiresPermission("lihat_alokasi_pembimbing.view")]
        public async Task<IActionResult> GetListKonsentrasiByNPK([FromQuery] GetListKonsentrasiByNPKRequest dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var sanitizedDto = SanitizerHelper.EncodeObject(dto);
            if (sanitizedDto == null) return NotFound();

            var username = string.IsNullOrEmpty(sanitizedDto.Username)
                           ? (User.FindFirstValue("namaakun") ?? string.Empty)
                           : sanitizedDto.Username;

            var userRole = string.IsNullOrEmpty(sanitizedDto.Role)
                           ? (User.FindFirstValue("rolid") ?? User.FindFirstValue(ClaimTypes.Role) ?? string.Empty)
                           : sanitizedDto.Role;

            try
            {
                sanitizedDto.Username = username;
                sanitizedDto.Role = userRole;

                var results = await _repo.GetListKonsentrasiByNPKAsync(sanitizedDto);

                var dataDTO = results.Select(x => new KonsentrasiByNPKDto
                {
                    KonsentrasiId = x.KonsentrasiId,
                    NamaKonsentrasi = x.NamaKonsentrasi
                }).ToList();

                var response = new GetListKonsentrasiByNPKResponse
                {
                    Status = results.Any() ? 200 : 404,
                    Message = results.Any() ? "Sukses" : "Data tidak ditemukan",
                    Data = dataDTO
                };

                return Ok(response);
            }
            catch 
            {
                return StatusCode(500, new { message = "Terjadi kesalahan internal pada server." });
            }
        }

        [HttpGet("GetListKonsentrasiByDosen")]
        [RequiresPermission("lihat_alokasi_pembimbing.view")]
        public async Task<IActionResult> GetListKonsentrasiByDosen([FromQuery] GetListKonsentrasiByDosenRequest dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var sanitizedDto = SanitizerHelper.EncodeObject(dto);
            if (sanitizedDto == null) return NotFound();

            var username = string.IsNullOrEmpty(sanitizedDto.Username)
                           ? (User.FindFirstValue("namaakun") ?? string.Empty)
                           : sanitizedDto.Username;

            var response = new GetListKonsentrasiByDosenResponse();

            try
            {
                var results = await _repo.GetListKonsentrasiByDosenAsync(username);

                var dataDTO = results.Select(x => new KonsentrasiByDosenDto
                {
                    KonsentrasiId = x.KonsentrasiId,
                    NamaKonsentrasi = x.NamaKonsentrasi
                }).ToList();

                response.Status = dataDTO.Any() ? 200 : 404;
                response.Message = dataDTO.Any() ? "Sukses" : "Data tidak ditemukan";
                response.Data = dataDTO;

                return Ok(response);
            }
            catch 
            {
                return StatusCode(500, new { message = "Terjadi kesalahan internal pada server." });
            }
        }


        [HttpGet("GetTahunAkademikAktif")]
        [RequiresPermission("lihat_alokasi_pembimbing.view")]
        public async Task<IActionResult> GetTahunAkademikAktif()
        {
            try
            {
                var result = await _repo.GetActiveTahunAkademikAsync();

                if (result == null)
                    return NotFound(new { message = "Data tahun akademik tidak ditemukan." });

                var response = new TahunAkademikAktifDto
                {
                    TahunAjaran = result.TahunAjaran,
                    Semester = result.Semester,
                    TanggalMulai = result.TanggalMulai
                };

                return Ok(response);
            }
            catch
            {
                return StatusCode(500, new { message = "Terjadi kesalahan internal pada server." });
            }
        }
    }
}
