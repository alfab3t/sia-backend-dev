using astratech_apps_backend.DTOs.MeninggalDunia;
using astratech_apps_backend.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using astratech_apps_backend.Helpers;
using Microsoft.AspNetCore.Authorization;


namespace astratech_apps_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MeninggalDuniaController : ControllerBase
    {
        private readonly IMeninggalDuniaRepository _repository;

        public MeninggalDuniaController(IMeninggalDuniaRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("GetAllMeninggalDunia")]
        [RequiresPermission("meninggal_dunia.view")]
        public async Task<IActionResult> GetAllMeninggalDunia([FromQuery] GetAllMeninggalDuniaRequest req)
        {
            try
            {
                if (req.PageSize <= 0)
                {
                    req.PageSize = 50; 
                }

                ModelState.Clear();
                var result = await _repository.GetAllAsync(req);

                return Ok(new MeninggalDuniaResponse
                {
                    Data = result.Data.Select(x => new MeninggalDuniaListDto
                    {
                        Id = x.Id,
                        NoPengajuan = x.NoPengajuan,
                        TanggalPengajuan = x.TanggalPengajuan,
                        NamaMahasiswa = x.NamaMahasiswa,
                        Nim = x.Nim,
                        Prodi = x.Prodi,
                        NomorSK = x.NomorSK,
                        Status = x.Status
                    }).ToList(),
                    TotalData = result.TotalData,
                    TotalHalaman = (int)Math.Ceiling((double)result.TotalData / req.PageSize)
                });
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Terjadi kesalahan saat mengambil data." });
            }
        }

        [HttpGet("GetMahasiswaListForMeninggalDunia")]
        [RequiresPermission("meninggal_dunia.view")]
        public async Task<IActionResult> GetMahasiswaListForMeninggalDunia([FromQuery] string? search = null)
        {
            var data = await _repository.GetMahasiswaListAsync(search);
            return Ok(data);
        }

        [HttpGet("GetMahasiswaDropdownForMeninggalDunia")]
        [RequiresPermission("meninggal_dunia.view")]
        public async Task<IActionResult> GetMahasiswaDropdownForMeninggalDunia()
        {
            var data = await _repository.GetMahasiswaDropdownSPAsync();
            return Ok(data);
        }

        [HttpGet("GetMahasiswaDetailForMeninggalDunia/{mhsId}")]
        [RequiresPermission("meninggal_dunia.view")]
        public async Task<IActionResult> GetMahasiswaDetailForMeninggalDunia(string mhsId)
        {
            var data = await _repository.GetMahasiswaDetailAsync(mhsId);
            if (data == null)
                return NotFound(new { message = "Data mahasiswa tidak ditemukan" });

            return Ok(data);
        }

        [HttpGet("GetMahasiswaProdiForMeninggalDunia/{mhsId}")]
        [RequiresPermission("meninggal_dunia.view")]
        public async Task<IActionResult> GetMahasiswaProdiForMeninggalDunia(string mhsId)
        {
            var data = await _repository.GetMahasiswaProdiSPAsync(mhsId);
            if (data == null)
                return NotFound(new { message = "Data prodi mahasiswa tidak ditemukan" });

            return Ok(data);
        }

        [HttpGet("GetProgramStudiListForMeninggalDunia")]
        [RequiresPermission("meninggal_dunia.view")]
        public async Task<IActionResult> GetProgramStudiListForMeninggalDunia()
        {
            var data = await _repository.GetProgramStudiListAsync();
            return Ok(data);
        }

        [HttpGet("GetDetailMeninggalDunia/{id}")]
        [RequiresPermission("meninggal_dunia.view")]
        public async Task<IActionResult> GetDetailMeninggalDunia(string id)
        {
            try
            {
                id = Uri.UnescapeDataString(id);
                var data = await _repository.GetDetailAsync(id);
                if (data == null)
                    return NotFound(new { message = "Data tidak ditemukan" });

                return Ok(data);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Terjadi kesalahan saat mengambil detail data" });
            }
        }

        [HttpGet("DownloadFileMeninggalDunia/{filename}")]
        [RequiresPermission("meninggal_dunia.print")]
        public IActionResult DownloadFileMeninggalDunia(string filename)
        {
            var possiblePaths = new[]
            {
                Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/meninggal", filename),
                Path.Combine(Directory.GetCurrentDirectory(), "uploads/meninggal", filename),
                Path.Combine(Directory.GetCurrentDirectory(), "uploads/meninggal/lampiran", filename),
                Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/meninggal/lampiran", filename)
            };

            var foundPath = possiblePaths.FirstOrDefault(System.IO.File.Exists);
            if (foundPath == null)
                return NotFound(new { message = "File tidak ditemukan." });

            var fileBytes = System.IO.File.ReadAllBytes(foundPath);
            var contentType = GetContentType(filename);
            return File(fileBytes, contentType, filename);
        }

        private static string GetContentType(string filename)
        {
            var extension = Path.GetExtension(filename).ToLowerInvariant();
            return extension switch
            {
                ".pdf" => "application/pdf",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".txt" => "text/plain",
                _ => "application/octet-stream"
            };
        }

        [HttpPost("CreateMeninggalDunia")]
        [RequiresPermission("meninggal_dunia.create")]
        public async Task<IActionResult> CreateMeninggalDunia([FromForm] CreateMeninggalDuniaRequest dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Data yang dikirim tidak valid", errors = ModelState });

            var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png" };
            const int maxFileSize = 10 * 1024 * 1024; 

            if (dto.LampiranFile != null)
            {
                var fileExtension = Path.GetExtension(dto.LampiranFile.FileName).ToLowerInvariant();
                
                if (!allowedExtensions.Contains(fileExtension))
                {
                    return BadRequest(new { message = "Tipe file lampiran tidak diizinkan." });
                }

                if (dto.LampiranFile.Length > maxFileSize)
                {
                    return BadRequest(new { message = "Ukuran file lampiran maksimal 10MB." });
                }
            }

            var createdBy = HttpContext.Items["UserId"]?.ToString() ?? "system";
            var id = await _repository.CreateAsync(dto, createdBy);
            return Ok(new { id });
        }

        [HttpPost("FinalizeDraftMeninggalDunia/{draftId}")]
        [RequiresPermission("meninggal_dunia.create")]
        public async Task<IActionResult> FinalizeDraftMeninggalDunia(string draftId)
        {
            var updatedBy = HttpContext.Items["UserId"]?.ToString() ?? "system";
            var officialId = await _repository.FinalizeAsync(draftId, updatedBy);

            if (string.IsNullOrEmpty(officialId))
            {
                return BadRequest(new { message = "Gagal mengajukan Meninggal Dunia." });
            }

            return Ok(new { message = "Pengajuan Meninggal Dunia Berhasil Dikirim." });
        }

        [HttpPut("UpdateMeninggalDunia/{id}")]
        [RequiresPermission("meninggal_dunia.edit")]
        public async Task<IActionResult> UpdateMeninggalDunia(string id, [FromForm] UpdateMeninggalDuniaRequest dto)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest(new { message = "Data pengajuan Meninggal Dunia Tidak Boleh Kosong." });
                }

                var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png" };
                const int maxFileSize = 10 * 1024 * 1024; 

                var updatedBy = HttpContext.Items["UserId"]?.ToString() ?? "system";

                var fileExtension = Path.GetExtension(dto.LampiranFile.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    return BadRequest(new { message = "Tipe file tidak diizinkan." });
                }

                if (dto.LampiranFile.Length > maxFileSize)
                {
                    return BadRequest(new { message = "Ukuran file maksimal 10MB." });
                }

                var success = await _repository.UpdateAsync(id, dto, updatedBy);
                if (!success)
                {
                    return BadRequest(new { message = "Gagal Perbarui Data Pengajuan Meninggal Dunia." });
                }

                return Ok(new { message = "Data Pengajuan Meninggal Dunia Berhasil Di Perbarui." });
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Terjadi kesalahan saat memperbarui data." });
            }
        }

        [HttpPut("UploadSKMeninggalDunia")]
        [RequiresPermission("meninggal_dunia.edit")]
        public async Task<IActionResult> UploadSKMeninggalDunia([FromForm] UploadSKMeninggalDuniaRequest request)
        {
            try
            {
                var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png" };
                const int maxFileSize = 10 * 1024 * 1024; 

                if (string.IsNullOrEmpty(request.MduId))
                {
                    return BadRequest(new { message = "MduId harus diisi." });
                }

                if (request.SK == null || request.SK.Length == 0)
                {
                    return BadRequest(new { message = "File SK harus diupload." });
                }

                if (request.SKPB == null || request.SKPB.Length == 0)
                {
                    return BadRequest(new { message = "File SPKB harus diupload." });
                }

                if (string.IsNullOrEmpty(request.ModifiedBy))
                {
                    request.ModifiedBy = HttpContext.Items["UserId"]?.ToString() ?? "system";
                }

                var skFileExtension = Path.GetExtension(request.SK.FileName).ToLowerInvariant();
                var spkbFileExtension = Path.GetExtension(request.SKPB.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(skFileExtension))
                {
                    return BadRequest(new { message = "Tipe file SK tidak diizinkan." });
                }

                if (!allowedExtensions.Contains(spkbFileExtension))
                {
                    return BadRequest(new { message = "Tipe file SPKB tidak diizinkan." });
                }

                if (request.SK.Length > maxFileSize)
                {
                    return BadRequest(new { message = "Ukuran file SK maksimal 10MB." });
                }

                if (request.SKPB.Length > maxFileSize)
                {
                    return BadRequest(new { message = "Ukuran file SPKB maksimal 10MB." });
                }

                string skFilePath = "";
                string spkbFilePath = "";

                if (request.SK != null)
                {
                    var skFileName = $"SK_{request.MduId}_{DateTime.Now:yyyyMMddHHmmss}_{request.SK.FileName}";
                    var skPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/meninggal", skFileName);
                    Directory.CreateDirectory(Path.GetDirectoryName(skPath)!);
                    
                    using (var stream = new FileStream(skPath, FileMode.Create))
                    {
                        await request.SK.CopyToAsync(stream);
                    }
                    skFilePath = skFileName;
                }

                if (request.SKPB != null)
                {
                    var spkbFileName = $"SPKB_{request.MduId}_{DateTime.Now:yyyyMMddHHmmss}_{request.SKPB.FileName}";
                    var spkbPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/meninggal", spkbFileName);
                    Directory.CreateDirectory(Path.GetDirectoryName(spkbPath)!);
                    
                    using (var stream = new FileStream(spkbPath, FileMode.Create))
                    {
                        await request.SKPB.CopyToAsync(stream);
                    }
                    spkbFilePath = spkbFileName;
                }

                var result = await _repository.UploadSKAsync(request.MduId, skFilePath, spkbFilePath, request.ModifiedBy);
                if (!result)
                {
                    return BadRequest(new { message = "Gagal upload SK Meninggal Dunia." });
                }

                return Ok(new { message = "Upload SK berhasil." });
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Terjadi kesalahan saat mengupload SK." });
            }
        }

        [HttpDelete("DeleteMeninggalDunia/{id}")]
        [RequiresPermission("meninggal_dunia.delete")]
        public async Task<IActionResult> DeleteMeninggalDunia(string id)
        {
            var updatedBy = HttpContext.Items["UserId"]?.ToString() ?? "system";
            var result = await _repository.SoftDeleteAsync(id, updatedBy);

            if (!result)
                return BadRequest(new { message = "Gagal Menghapus Data Pengajuan Meninggal Dunia." });

            return Ok(new { message = "Data Pengajuan Meninggal Dunia berhasil dihapus." });
        }

        [HttpPut("ApproveMeninggalDunia/{id}")]
        [RequiresPermission("meninggal_dunia.approve_reject")]
        public async Task<IActionResult> ApproveMeninggalDunia(string id, [FromBody] ApproveMeninggalDuniaRequest dto)
        {
            try
            {
                id = Uri.UnescapeDataString(id);
                var detectedRole = await _repository.DetectUserRoleAsync(dto.Username);
                if (string.IsNullOrEmpty(detectedRole))
                {
                    return BadRequest(new { message = "ROL Tidak Valid." });
                }

                dto.Role = detectedRole;
                var result = await _repository.ApproveAsync(id, dto);
                if (!result)
                {
                    return BadRequest(new { message = "Gagal Menyetujui Data Pengajuan Meninggal Dunia." });
                }

                return Ok(new { message = "Data Pengajuan Meninggal Dunia berhasil disetujui" });
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Terjadi Kesalahan Saat Menyetujui Data Pengajuan Meninggal Dunia." });
            }
        }

        [HttpPut("RejectMeninggalDunia/{id}")]
        [RequiresPermission("meninggal_dunia.approve_reject")]
        public async Task<IActionResult> RejectMeninggalDunia(string id, [FromBody] RejectMeninggalDuniaRequest dto)
        {
            try
            {
                id = Uri.UnescapeDataString(id);
                var detectedRole = await _repository.DetectUserRoleAsync(dto.Username);
                if (string.IsNullOrEmpty(detectedRole))
                {
                    return BadRequest(new { message = "ROL Tidak Valid" });
                }

                dto.Role = detectedRole;
                var success = await _repository.RejectAsync(id, dto);
                if (!success)
                {
                    return BadRequest(new { message = "Gagal Menolak Data pengajuan Meninggal Dunia." });
                }

                return Ok(new { message = "Data pengajuan Meninggal Dunia Berhasil Ditolak" });
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Terjadi Kesalahan Saat Menolak Pengajuan." });
            }
        }

        [HttpGet("GetRiwayatMeninggalDunia")]
        [RequiresPermission("meninggal_dunia.view")]
        public async Task<IActionResult> GetRiwayatMeninggalDunia([FromQuery] GetRiwayatMeninggalDuniaRequest req)
        {
            var result = await _repository.GetRiwayatAsync(req);
            return Ok(new GetRiwayatMeninggalDuniaResponse
            {
                Data = result.Data.ToList(),
                TotalData = result.TotalData,
                TotalHalaman = (int)Math.Ceiling((double)result.TotalData / (req.PageSize > 0 ? req.PageSize : 50))
            });
        }

        [HttpGet("ExportRiwayatMeninggalDuniaToExcel")]
        [RequiresPermission("meninggal_dunia.export")]
        public async Task<IActionResult> ExportRiwayatMeninggalDuniaToExcel([FromQuery] string sort = "", [FromQuery] string konsentrasi = "")
        {
            try
            {
                var data = await _repository.GetRiwayatExcelAsync(sort, konsentrasi);
                using var workbook = new ClosedXML.Excel.XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Riwayat Meninggal Dunia");

                worksheet.Cell(1, 1).Value = "NIM";
                worksheet.Cell(1, 2).Value = "Nama Mahasiswa";
                worksheet.Cell(1, 3).Value = "Konsentrasi";
                worksheet.Cell(1, 4).Value = "Tanggal Pengajuan";
                worksheet.Cell(1, 5).Value = "No SK";
                worksheet.Cell(1, 6).Value = "No Pengajuan";

                var headerRange = worksheet.Range(1, 1, 1, 6);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.LightGray;
                headerRange.Style.Border.OutsideBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
                headerRange.Style.Border.InsideBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;

                data.Select((item, index) => new { item, rowIndex = index + 2 }).ToList().ForEach(x =>
                {
                    worksheet.Cell(x.rowIndex, 1).Value = x.item.NIM;
                    worksheet.Cell(x.rowIndex, 2).Value = x.item.NamaMahasiswa;
                    worksheet.Cell(x.rowIndex, 3).Value = x.item.Konsentrasi;
                    worksheet.Cell(x.rowIndex, 4).Value = x.item.TanggalPengajuan;
                    worksheet.Cell(x.rowIndex, 5).Value = x.item.NoSK;
                    worksheet.Cell(x.rowIndex, 6).Value = x.item.NoPengajuan;
                });

                var totalRows = data.Count() + 1;
                worksheet.Columns().AdjustToContents();

                if (totalRows > 2)
                {
                    var dataRange = worksheet.Range(2, 1, totalRows, 6);
                    dataRange.Style.Border.OutsideBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
                    dataRange.Style.Border.InsideBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
                }

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                stream.Position = 0;

                var fileName = $"RiwayatMeninggalDunia_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Terjadi kesalahan saat membuat file Excel." });
            }
        }

        [HttpGet("GetKonsentrasiBySekprod")]
        [RequiresPermission("meninggal_dunia.view")]
        public async Task<IActionResult> GetKonsentrasiBySekprod([FromQuery] string username)
        {
            try
            {
                if (string.IsNullOrEmpty(username))
                {
                    return BadRequest(new { message = "Username harus diisi." });
                }

                var data = await _repository.GetKonsentrasiBySekprodAsync(username);
                return Ok(data);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Terjadi kesalahan saat mengambil data konsentrasi." });
            }
        }

        [HttpGet("GetMahasiswaByKonsentrasi")]
        [RequiresPermission("meninggal_dunia.view")]
        public async Task<IActionResult> GetMahasiswaByKonsentrasi([FromQuery] string username)
        {
            try
            {
                if (string.IsNullOrEmpty(username))
                {
                    return BadRequest(new { message = "Username harus diisi." });
                }

                var data = await _repository.GetMahasiswaByKonsentrasiAsync(username);
                return Ok(data);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Terjadi kesalahan saat mengambil data mahasiswa." });
            }
        }
    }
}