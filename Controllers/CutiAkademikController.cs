using astratech_apps_backend.DTOs.CutiAkademik;
using astratech_apps_backend.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")] 
    public class CutiAkademikController : ControllerBase
    {
        private readonly ICutiAkademikRepository _repository;

        public CutiAkademikController(ICutiAkademikRepository repository)
        {
            _repository = repository;
        }

        [HttpPost("CreateDraftCutiAkademik")]
        public async Task<IActionResult> CreateDraftCutiAkademik([FromBody] CreateDraftCutiAkademikRequest dto)
        {
            var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png" };
            const int maxFileSize = 10 * 1024 * 1024; // 10MB

            if (dto.LampiranSuratPengajuan != null)
            {
                var fileExtension = Path.GetExtension(dto.LampiranSuratPengajuan.FileName).ToLowerInvariant();
                
                if (!allowedExtensions.Contains(fileExtension))
                {
                    return BadRequest(new { message = $"Tipe file lampiran surat pengajuan tidak diizinkan. Gunakan: {string.Join(", ", allowedExtensions)}" });
                }

                if (dto.LampiranSuratPengajuan.Length > maxFileSize)
                {
                    return BadRequest(new { message = "Ukuran file lampiran surat pengajuan maksimal 10MB." });
                }
            }

            if (dto.Lampiran != null)
            {
                var fileExtension = Path.GetExtension(dto.Lampiran.FileName).ToLowerInvariant();
                
                if (!allowedExtensions.Contains(fileExtension))
                {
                    return BadRequest(new { message = $"Tipe file lampiran tidak diizinkan. Gunakan: {string.Join(", ", allowedExtensions)}" });
                }

                if (dto.Lampiran.Length > maxFileSize)
                {
                    return BadRequest(new { message = "Ukuran file lampiran maksimal 10MB." });
                }
            }

            var id = await _repository.CreateDraftAsync(dto);
            return Ok(new { draftId = id });
        }

        [HttpPut("GenerateIdFinalCutiAkademik")]
        public async Task<IActionResult> GenerateIdFinalCutiAkademik([FromBody] GenerateIdFinalCutiAkademikRequest dto)
        {
            var id = await _repository.GenerateIdAsync(dto);

            if (id == null)
                return BadRequest(new { message = "Gagal generate ID final." });

            return Ok(new { finalId = id });
        }

        [HttpPost("CreateDraftCutiAkademikByProdi")]
        public async Task<IActionResult> CreateDraftCutiAkademikByProdi([FromBody] CreateDraftCutiAkademikByProdiRequest dto)
        {
            try
            {
                var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png" };
                const int maxFileSize = 10 * 1024 * 1024; 

                if (dto.LampiranSuratPengajuan != null)
                {
                    var fileExtension = Path.GetExtension(dto.LampiranSuratPengajuan.FileName).ToLowerInvariant();
                    
                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        return BadRequest(new { message = $"Tipe file lampiran surat pengajuan tidak diizinkan. Gunakan: {string.Join(", ", allowedExtensions)}" });
                    }

                    if (dto.LampiranSuratPengajuan.Length > maxFileSize)
                    {
                        return BadRequest(new { message = "Ukuran file lampiran surat pengajuan maksimal 10MB." });
                    }
                }

                if (dto.Lampiran != null)
                {
                    var fileExtension = Path.GetExtension(dto.Lampiran.FileName).ToLowerInvariant();
                    
                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        return BadRequest(new { message = $"Tipe file lampiran tidak diizinkan. Gunakan: {string.Join(", ", allowedExtensions)}" });
                    }

                    if (dto.Lampiran.Length > maxFileSize)
                    {
                        return BadRequest(new { message = "Ukuran file lampiran maksimal 10MB." });
                    }
                }

                var id = await _repository.CreateDraftByProdiAsync(dto);
                
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest(new { message = "Gagal membuat draft cuti akademik." });
                }
                
                return Ok(new { draftId = id });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { 
                    message = "Terjadi kesalahan saat membuat draft.", 
                    error = ex.Message 
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { 
                    message = "Operasi tidak valid saat membuat draft.", 
                    error = ex.Message 
                });
            }
        }

        [HttpPut("GenerateIdFinalCutiAkademikByProdi")]
        public async Task<IActionResult> GenerateIdFinalCutiAkademikByProdi([FromBody] GenerateIdFinalCutiAkademikByProdiRequest dto)
        {
            try
            {
                var id = await _repository.GenerateIdByProdiAsync(dto);
                
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest(new { message = "Gagal generate id final (prodi)." });
                }
                
                return Ok(new { finalId = id });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { 
                    message = "Terjadi kesalahan saat generate final ID.", 
                    error = ex.Message 
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { 
                    message = "Operasi tidak valid saat generate final ID.", 
                    error = ex.Message 
                });
            }
        }

        [HttpGet("GetAllCutiAkademik")]
        public async Task<IActionResult> GetAllCutiAkademik(
            [FromQuery] string mhsId = "%", 
            [FromQuery] string status = "",
            [FromQuery] string userId = "", 
            [FromQuery] string role = "", 
            [FromQuery] string search = "")
        {
            var result = await _repository.GetAllAsync(mhsId, status, userId, role, search);
            return Ok(result);
        }

        [HttpGet("GetDetailCutiAkademik")]
        public async Task<IActionResult> GetDetailCutiAkademik([FromQuery] string id)
        {
            var data = await _repository.GetDetailAsync(id);

            if (data == null)
                return NotFound(new { message = "Detail Cuti Akademik tidak ditemukan." });

            return Ok(data);
        }

        [HttpPut("UpdateCutiAkademik/{id}")]
        public async Task<IActionResult> UpdateCutiAkademik(string id, [FromBody] UpdateCutiAkademikRequest dto)
        {
            try
            {
                var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png" };
                const int maxFileSize = 10 * 1024 * 1024; // 10MB

                if (dto.LampiranSuratPengajuan != null)
                {
                    var fileExtension = Path.GetExtension(dto.LampiranSuratPengajuan.FileName).ToLowerInvariant();
                    
                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        return BadRequest(new { message = $"Tipe file lampiran surat pengajuan tidak diizinkan. Gunakan: {string.Join(", ", allowedExtensions)}" });
                    }

                    if (dto.LampiranSuratPengajuan.Length > maxFileSize)
                    {
                        return BadRequest(new { message = "Ukuran file lampiran surat pengajuan maksimal 10MB." });
                    }
                }

                if (dto.Lampiran != null)
                {
                    var fileExtension = Path.GetExtension(dto.Lampiran.FileName).ToLowerInvariant();
                    
                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        return BadRequest(new { message = $"Tipe file lampiran tidak diizinkan. Gunakan: {string.Join(", ", allowedExtensions)}" });
                    }

                    if (dto.Lampiran.Length > maxFileSize)
                    {
                        return BadRequest(new { message = "Ukuran file lampiran maksimal 10MB." });
                    }
                }

                if (string.IsNullOrEmpty(dto.ModifiedBy))
                {
                    dto.ModifiedBy = HttpContext.Items["UserId"]?.ToString() ?? "system";
                }

                var success = await _repository.UpdateAsync(id, dto);

                if (success)
                {
                    return Ok(new { message = "Cuti Akademik berhasil diupdate." });
                }
                
                return BadRequest(new { message = "Gagal mengupdate Cuti Akademik. Data mungkin tidak ditemukan." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { 
                    message = "Terjadi kesalahan saat mengupdate data.", 
                    error = ex.Message,
                    details = ex.InnerException?.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { 
                    message = "Operasi tidak valid saat mengupdate data.", 
                    error = ex.Message,
                    details = ex.InnerException?.Message
                });
            }
        }

        [HttpDelete("DeleteCutiAkademik/{id}")]
        public async Task<IActionResult> DeleteCutiAkademik(string id)
        {
            var modifiedBy = HttpContext.Items["UserId"]?.ToString() ?? "system";

            var success = await _repository.DeleteAsync(id, modifiedBy);

            if (!success)
                return BadRequest(new { message = "Gagal menghapus Cuti Akademik." });

            return Ok(new { message = "Cuti Akademik berhasil dihapus." });
        }

        [HttpGet("GetRiwayatCutiAkademik")]
        public async Task<IActionResult> GetRiwayatCutiAkademik(
            [FromQuery] string userId = "", 
            [FromQuery] string status = "", 
            [FromQuery] string search = "")
        {
            var result = await _repository.GetRiwayatAsync(userId, status, search);
            return Ok(result);
        }

        [HttpGet("ExportRiwayatCutiAkademikToExcel")]
        public async Task<IActionResult> ExportRiwayatCutiAkademikToExcel([FromQuery] string userId = "")
        {
            try
            {
                var data = await _repository.GetRiwayatExcelAsync(userId);
                
                using var workbook = new ClosedXML.Excel.XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Riwayat Cuti Akademik");

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

                int row = 2;
                foreach (var item in data)
                {
                    worksheet.Cell(row, 1).Value = item.NIM;
                    worksheet.Cell(row, 2).Value = item.NamaMahasiswa;
                    worksheet.Cell(row, 3).Value = item.Konsentrasi;
                    worksheet.Cell(row, 4).Value = item.TanggalPengajuan;
                    worksheet.Cell(row, 5).Value = item.NoSK;
                    worksheet.Cell(row, 6).Value = item.NoPengajuan;
                    row++;
                }

                worksheet.Columns().AdjustToContents();

                if (row > 2)
                {
                    var dataRange = worksheet.Range(2, 1, row - 1, 6);
                    dataRange.Style.Border.OutsideBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
                    dataRange.Style.Border.InsideBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
                }

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                stream.Position = 0;

                var fileName = $"RiwayatCutiAkademik_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                
                return File(stream.ToArray(),
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { 
                    message = "Terjadi kesalahan saat membuat file Excel.", 
                    error = ex.Message 
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { 
                    message = "Operasi tidak valid saat membuat file Excel.", 
                    error = ex.Message 
                });
            }
        }

        [HttpGet("DownloadFileCutiAkademik/{filename}")]
        public IActionResult DownloadFileCutiAkademik(string filename)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/cuti", filename);

            if (!System.IO.File.Exists(path))
                return NotFound();  

            var fileBytes = System.IO.File.ReadAllBytes(path);
            return File(fileBytes, "application/octet-stream", filename);
        }

        [HttpPut("ApproveCutiAkademik")]
        public async Task<IActionResult> ApproveCutiAkademik([FromBody] ApproveCutiAkademikRequest dto)
        {
            try
            {
                var detectedRole = await _repository.DetectUserRoleAsync(dto.ApprovedBy);
                if (string.IsNullOrEmpty(detectedRole))
                {
                    return BadRequest(new { 
                        message = "Tidak dapat mendeteksi role pengguna. Pastikan username valid.",
                        username = dto.ApprovedBy
                    });
                }
                
                dto.Role = detectedRole;
                
                var success = await _repository.ApproveCutiAsync(dto);
                
                if (success)
                {
                    return Ok(new { 
                        approved = true,
                        id = dto.Id,
                        approvedBy = dto.ApprovedBy,
                        role = detectedRole,
                        message = $"Cuti akademik berhasil disetujui oleh {detectedRole}"
                    });
                }
                
                return BadRequest(new { 
                    message = "Gagal menyetujui cuti akademik. Data mungkin tidak ditemukan atau sudah diproses.",
                    id = dto.Id,
                    detectedRole = detectedRole,
                    username = dto.ApprovedBy
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { 
                    message = "Terjadi kesalahan saat menyetujui cuti akademik.", 
                    error = ex.Message,
                    id = dto.Id
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { 
                    message = "Operasi tidak valid saat menyetujui cuti akademik.", 
                    error = ex.Message,
                    id = dto.Id
                });
            }
        }

        [HttpPut("ApproveCutiAkademikByProdi")]
        public async Task<IActionResult> ApproveCutiAkademikByProdi([FromBody] ApproveCutiAkademikByProdiRequest dto)
        {
            try
            {
                if (string.IsNullOrEmpty(dto.Id))
                {
                    return BadRequest(new { message = "ID cuti akademik harus diisi." });
                }
                
                if (string.IsNullOrEmpty(dto.ApprovedBy))
                {
                    return BadRequest(new { message = "ApprovedBy harus diisi." });
                }
                
                if (string.IsNullOrWhiteSpace(dto.Menimbang))
                {
                    return BadRequest(new { message = "Menimbang/pertimbangan harus diisi dan tidak boleh kosong." });
                }
                
                var success = await _repository.ApproveProdiCutiAsync(dto);
                
                if (success)
                {
                    return Ok(new { message = "Cuti akademik berhasil disetujui oleh prodi." });
                }
                
                return BadRequest(new { message = "Gagal menyetujui cuti akademik. Periksa apakah ID valid dan data dapat diupdate." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { 
                    message = "Terjadi kesalahan saat menyetujui cuti akademik.", 
                    error = ex.Message,
                    details = ex.InnerException?.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { 
                    message = "Operasi tidak valid saat menyetujui cuti akademik.", 
                    error = ex.Message,
                    details = ex.InnerException?.Message
                });
            }
        }

        [HttpPut("RejectCutiAkademik")]
        public async Task<IActionResult> RejectCutiAkademik([FromBody] RejectCutiAkademikRequest dto)
        {
            try
            {
                if (string.IsNullOrEmpty(dto.Id))
                {
                    return BadRequest(new { message = "ID cuti akademik harus diisi." });
                }
                
                if (string.IsNullOrEmpty(dto.Username))
                {
                    return BadRequest(new { message = "Username harus diisi." });
                }
                
                var detectedRole = await _repository.DetectUserRoleAsync(dto.Username);
                if (string.IsNullOrEmpty(detectedRole))
                {
                    return BadRequest(new { 
                        message = "Tidak dapat mendeteksi role pengguna. Pastikan username valid.",
                        username = dto.Username
                    });
                }
                
                dto.Role = detectedRole;
                
                var success = await _repository.RejectCutiAsync(dto);
                
                if (success)    
                {
                    return Ok(new { 
                        rejected = true,
                        id = dto.Id,
                        rejectedBy = dto.Username,
                        role = detectedRole,
                        message = $"Cuti akademik berhasil ditolak oleh {detectedRole}"
                    });
                }
                
                return BadRequest(new { 
                    message = "Gagal menolak cuti akademik. Data mungkin tidak ditemukan atau sudah diproses.",
                    id = dto.Id,
                    detectedRole = detectedRole,
                    username = dto.Username
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { 
                    message = "Terjadi kesalahan saat menolak cuti akademik.", 
                    error = ex.Message,
                    id = dto.Id
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { 
                    message = "Operasi tidak valid saat menolak cuti akademik.", 
                    error = ex.Message,
                    id = dto.Id
                });
            }
        }

        [HttpPut("UploadSKCutiAkademik")]
        public async Task<IActionResult> UploadSKCutiAkademik([FromBody] UploadSKCutiAkademikRequest dto)
        {
            try
            {
                var allowedFileExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png" };
                const int maxFileSize = 10 * 1024 * 1024; 
                
                if (string.IsNullOrEmpty(dto.Id))
                {
                    return BadRequest(new { message = "ID cuti akademik harus diisi." });
                }
                
                if (dto.FileSK == null || dto.FileSK.Length == 0)
                {
                    return BadRequest(new { message = "File SK harus diupload." });
                }
                
                if (string.IsNullOrEmpty(dto.UploadBy))
                {
                    return BadRequest(new { message = "UploadBy harus diisi." });
                }

                var fileExtension = Path.GetExtension(dto.FileSK.FileName).ToLowerInvariant();
                
                if (!allowedFileExtensions.Contains(fileExtension))
                {
                    return BadRequest(new { message = $"Tipe file tidak diizinkan. Gunakan: {string.Join(", ", allowedFileExtensions)}" });
                }

                if (dto.FileSK.Length > maxFileSize)
                {
                    return BadRequest(new { message = "Ukuran file maksimal 10MB." });
                }
                
                var success = await _repository.UploadSKAsync(dto);
                
                if (success)
                {
                    return Ok(new { 
                        message = "SK berhasil diupload. Status cuti akademik telah diubah menjadi 'Disetujui'. Nomor SK akan ditampilkan otomatis di daftar.",
                        success = true,
                        id = dto.Id
                    });
                }
                
                return BadRequest(new { message = "Gagal mengupload SK. Periksa apakah ID valid dan status cuti adalah 'Menunggu Upload SK'." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { 
                    message = "Terjadi kesalahan saat mengupload SK.", 
                    error = ex.Message,
                    details = ex.InnerException?.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { 
                    message = "Operasi tidak valid saat mengupload SK.", 
                    error = ex.Message,
                    details = ex.InnerException?.Message
                });
            }
        }
    }
}