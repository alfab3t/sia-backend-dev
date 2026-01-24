using astratech_apps_backend.DTOs.CutiAkademik;
using astratech_apps_backend.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using astratech_apps_backend.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace astratech_apps_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CutiAkademikController : ControllerBase
    {
        private readonly ICutiAkademikRepository _repository;

        public CutiAkademikController(ICutiAkademikRepository repository)
        {
            _repository = repository;
        }

        [HttpPost("CreateDraftCutiAkademik")]
        [RequiresPermission("cuti_akademik.create")]
        public async Task<IActionResult> CreateDraftCutiAkademik([FromForm] CreateDraftCutiAkademikRequest dto)
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

            var id = await _repository.CreateDraftAsync(dto);
            return Ok(new { draftId = id });
        }

        [HttpPut("GenerateIdFinalCutiAkademik")]
        [RequiresPermission("cuti_akademik.create")]
        public async Task<IActionResult> GenerateIdFinalCutiAkademik([FromBody] GenerateIdFinalCutiAkademikRequest dto)
        {
            var id = await _repository.GenerateIdAsync(dto);

            if (id == null)
                return BadRequest(new { message = "Gagal generate ID final." });

            return Ok(new { finalId = id });
        }

        [HttpPost("CreateDraftCutiAkademikByProdi")]
        [RequiresPermission("cuti_akademik.create")]
        public async Task<IActionResult> CreateDraftCutiAkademikByProdi([FromForm] CreateDraftCutiAkademikByProdiRequest dto)
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
                        return BadRequest(new { message = "Tipe file lampiran tidak diizinkan." });
                    }

                    if (dto.Lampiran.Length > maxFileSize)
                    {
                        return BadRequest(new { message = "Ukuran file lampiran maksimal 10MB." });
                    }
                }

                var id = await _repository.CreateDraftByProdiAsync(dto);
                
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest(new { message = "Gagal Membuat Data Draft Pengajuan Cuti Akademik." });
                }
                
                return Ok(new { draftId = id });
            }
            catch (Exception)
            {
                return BadRequest(new { 
                    message = "Terjadi kesalahan saat membuat draft."
                });
            }
        }

        [HttpPut("GenerateIdFinalCutiAkademikByProdi")]
        [RequiresPermission("cuti_akademik.create")]
        public async Task<IActionResult> GenerateIdFinalCutiAkademikByProdi([FromBody] GenerateIdFinalCutiAkademikByProdiRequest dto)
        {
            try
            {
                var id = await _repository.GenerateIdByProdiAsync(dto);
                
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest(new { message = "Gagal Pengajuan Cuti Akademik" });
                }
                
                return Ok(new { finalId = id });
            }
            catch (Exception)
            {
                return BadRequest(new { 
                    message = "Terjadi kesalahan saat generate final ID."
                });
            }
        }

        [HttpGet("GetAllCutiAkademik")]
        [RequiresPermission("cuti_akademik.view")]
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
        [RequiresPermission("cuti_akademik.view")]
        public async Task<IActionResult> GetDetailCutiAkademik([FromQuery] string id)
        {
            var data = await _repository.GetDetailAsync(id);

            if (data == null)
                return NotFound(new { message = "Detail Cuti Akademik tidak ditemukan." });

            return Ok(data);
        }

        [HttpPut("UpdateCutiAkademik/{id}")]
        [RequiresPermission("cuti_akademik.edit")]
        public async Task<IActionResult> UpdateCutiAkademik(string id, [FromForm] UpdateCutiAkademikRequest dto)
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
                        return BadRequest(new { message = "Tipe file lampiran surat pengajuan tidak diizinkan." });
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
                        return BadRequest(new { message = "Tipe file lampiran tidak diizinkan." });
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
                
                return BadRequest(new { message = "Gagal Perbarui Data Pengajuan Cuti Akademik." });
            }
            catch (Exception)
            {
                return BadRequest(new { 
                    message = "Terjadi kesalahan saat mengupdate data."
                });
            }
        }

        [HttpDelete("DeleteCutiAkademik/{id}")]
        [RequiresPermission("cuti_akademik.delete")]
        public async Task<IActionResult> DeleteCutiAkademik(string id)
        {
            var modifiedBy = HttpContext.Items["UserId"]?.ToString() ?? "system";

            var success = await _repository.DeleteAsync(id, modifiedBy);

            if (!success)
                return BadRequest(new { message = "Gagal Menghapus Data Pengajuan Cuti Akademik." });

            return Ok(new { message = "Data Pengajuaun Cuti Akademik berhasil dihapus." });
        }

        [HttpGet("GetRiwayatCutiAkademik")]
        [RequiresPermission("cuti_akademik.view")]
        public async Task<IActionResult> GetRiwayatCutiAkademik(
            [FromQuery] string userId = "", 
            [FromQuery] string status = "", 
            [FromQuery] string search = "")
        {
            var result = await _repository.GetRiwayatAsync(userId, status, search);
            return Ok(result);
        }

        [HttpGet("ExportRiwayatCutiAkademikToExcel")]
        [RequiresPermission("cuti_akademik.export")]
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
            catch (Exception)
            {
                return BadRequest(new { 
                    message = "Terjadi kesalahan saat membuat file Excel."
                });
            }
        }

        [HttpGet("DownloadFileCutiAkademik/{filename}")]
        [RequiresPermission("cuti_akademik.print")]
        public IActionResult DownloadFileCutiAkademik(string filename)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/cuti", filename);

            if (!System.IO.File.Exists(path))
                return NotFound();  

            var fileBytes = System.IO.File.ReadAllBytes(path);
            return File(fileBytes, "application/octet-stream", filename);
        }

        [HttpPut("ApproveCutiAkademik")]
        [RequiresPermission("cuti_akademik.approve_reject")]
        public async Task<IActionResult> ApproveCutiAkademik([FromBody] ApproveCutiAkademikRequest dto)
        {
            try
            {
                var detectedRole = await _repository.DetectUserRoleAsync(dto.ApprovedBy);
                if (string.IsNullOrEmpty(detectedRole))
                {
                    return BadRequest(new { 
                        message = "Role tidak valid untuk user ini."
                    });
                }
                
                dto.Role = detectedRole;
                
                var success = await _repository.ApproveCutiAsync(dto);
                
                if (success)
                {
                    return Ok(new { 
                        message = "Cuti Akademik berhasil disetujui."
                    });
                }
                
                return BadRequest(new { 
                    message = "Gagal menyetujui cuti akademik."
                });
            }
            catch (Exception)
            {
                return BadRequest(new { 
                    message = "Terjadi kesalahan saat menyetujui cuti akademik."
                });
            }
        }

        [HttpPut("ApproveCutiAkademikByProdi")]
        [RequiresPermission("cuti_akademik.approve_reject")]
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
                    return Ok(new { message = "Cuti akademik berhasil disetujui." });
                }
                
                return BadRequest(new { message = "Gagal Menyetujui Cuti Akademik." });
            }
            catch (Exception)
            {
                return BadRequest(new { 
                    message = "Terjadi kesalahan saat menyetujui cuti akademik."
                });
            }
        }

        [HttpPut("RejectCutiAkademik")]
        [RequiresPermission("cuti_akademik.approve_reject")]
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
                
                var success = await _repository.RejectCutiAsync(dto);
                
                if (success)    
                {
                    return Ok(new { 
                        message = "Cuti Akademik berhasil ditolak"
                    });
                }
                
                return BadRequest(new { 
                    message = "Gagal menolak cuti akademik."
                });
            }
            catch (Exception)
            {
                return BadRequest(new { 
                    message = "Terjadi kesalahan saat menolak cuti akademik."
                });
            }
        }

        [HttpPut("UploadSKCutiAkademik")]
        [RequiresPermission("cuti_akademik.edit")]
        public async Task<IActionResult> UploadSKCutiAkademik([FromForm] UploadSKCutiAkademikRequest dto)
        {
            try
            {
                var allowedFileExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png" };
                const int maxFileSize = 10 * 1024 * 1024; 

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
                
                return Ok(new { 
                    message = "SK berhasil diupload."
                });
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Gagal upload SK." });
            }
        }

        [HttpGet("GetDetailMahasiswa")]
        [RequiresPermission("cuti_akademik.view")]
        public async Task<IActionResult> GetDetailMahasiswa([FromQuery] string mahasiswaId)
        {
            try
            {
                if (string.IsNullOrEmpty(mahasiswaId))
                {
                    return BadRequest(new { message = "Mahasiswa ID harus diisi." });
                }

                var data = await _repository.GetDetailMahasiswaAsync(mahasiswaId);

                if (data == null)
                    return NotFound(new { message = "Detail mahasiswa tidak ditemukan." });

                return Ok(data);
            }
            catch (Exception)
            {
                return BadRequest(new { 
                    message = "Terjadi kesalahan saat mengambil detail mahasiswa."
                });
            }
        }

        [HttpGet("CheckBebasTanggungan")]
        [RequiresPermission("cuti_akademik.view")]
        public async Task<IActionResult> CheckBebasTanggungan([FromQuery] string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest(new { message = "User ID harus diisi." });
                }

                var data = await _repository.CheckBebasTanggunganAsync(userId);

                if (data == null)
                    return NotFound(new { message = "Data bebas tanggungan tidak ditemukan." });

                return Ok(data);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Terjadi kesalahan saat mengecek bebas tanggungan." });
            }
        }

        [HttpGet("GetProfilMahasiswa")]
        [RequiresPermission("cuti_akademik.view")]
        public async Task<IActionResult> GetProfilMahasiswa([FromQuery] string nim)
        {
            try
            {
                if (string.IsNullOrEmpty(nim))
                {
                    return BadRequest(new { message = "NIM harus diisi." });
                }

                var data = await _repository.GetProfilMahasiswaAsync(nim);

                if (data == null)
                    return NotFound(new { message = "Profil mahasiswa tidak ditemukan." });

                return Ok(data);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Terjadi kesalahan saat mengambil profil mahasiswa." });
            }
        }

        [HttpGet("GetKonsentrasiBySekprod")]
        [RequiresPermission("cuti_akademik.view")]
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

        [HttpGet("GetMahasiswaByNim")]
        [RequiresPermission("cuti_akademik.view")]
        public async Task<IActionResult> GetMahasiswaByNim([FromQuery] string nim)
        {
            try
            {
                if (string.IsNullOrEmpty(nim))
                {
                    return BadRequest(new { message = "NIM harus diisi." });
                }

                var data = await _repository.GetMahasiswaByNimAsync(nim);

                if (data == null)
                    return NotFound(new { message = "Mahasiswa tidak ditemukan." });

                return Ok(data);
            }
            catch (Exception)
            {
                return BadRequest(new { 
                    message = "Terjadi kesalahan saat mengambil data mahasiswa."
                });
            }
        }

        [HttpGet("GetMahasiswaByKonsentrasi")]
        [RequiresPermission("cuti_akademik.view")]
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