using astratech_apps_backend.DTOs.MeninggalDunia;
using astratech_apps_backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace astratech_apps_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MeninggalDuniaController : ControllerBase
    {
        private readonly IMeninggalDuniaService _service;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public MeninggalDuniaController(IMeninggalDuniaService service, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _service = service;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }


        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] GetAllMeninggalDuniaRequest req)
        {
            try
            {
                if (req.PageSize <= 0)
                {
                    req.PageSize = 50; 
                }
                
                ModelState.Clear();
                var result = await _service.GetAllAsync(req);
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Terjadi kesalahan saat mengambil data.", error = ex.Message });
            }
        }



        [HttpGet("mahasiswa")]
        public async Task<IActionResult> GetMahasiswa([FromQuery] string? search = null)
        {
            var data = await _service.GetMahasiswaListAsync(search);
            return Ok(data);
        }

        [HttpGet("mahasiswa-dropdown")]
        public async Task<IActionResult> GetMahasiswaDropdown()
        {
            var data = await _service.GetMahasiswaDropdownAsync();
            return Ok(data);
        }

        [HttpGet("mahasiswa/{mhsId}")]
        public async Task<IActionResult> GetMahasiswaDetail(string mhsId)
        {
            var data = await _service.GetMahasiswaDetailAsync(mhsId);
            if (data == null)
                return NotFound(new { message = "Data mahasiswa tidak ditemukan" });
            
            return Ok(data);
        }

        [HttpGet("mahasiswa/{mhsId}/prodi")]
        public async Task<IActionResult> GetMahasiswaProdi(string mhsId)
        {
            var data = await _service.GetMahasiswaProdiAsync(mhsId);
            if (data == null)
                return NotFound(new { message = "Data prodi mahasiswa tidak ditemukan" });
            
            return Ok(data);
        }

        [HttpGet("program-studi")]
        public async Task<IActionResult> GetProgramStudi()
        {
            var data = await _service.GetProgramStudiListAsync();
            return Ok(data);
        }




        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetail(string id)
        {
            try
            {
                id = Uri.UnescapeDataString(id);
                
                var data = await _service.GetDetailAsync(id);

                if (data == null)
                    return NotFound(new { message = $"Data dengan ID '{id}' tidak ditemukan" });

                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Terjadi kesalahan saat mengambil detail data", error = ex.Message });
            }
        }



       
        [HttpGet("file/{filename}")]
        public IActionResult DownloadFile(string filename)
        {
            const string uploadsFolder = "uploads";
            const string meninggalFolder = "meninggal";
            const string lampiranFolder = "lampiran";
            const string wwwrootFolder = "wwwroot";
            
            var possiblePaths = new[]
            {
                Path.Combine(Directory.GetCurrentDirectory(), wwwrootFolder, uploadsFolder, meninggalFolder, filename),
                Path.Combine(Directory.GetCurrentDirectory(), uploadsFolder, meninggalFolder, filename),
                Path.Combine(Directory.GetCurrentDirectory(), uploadsFolder, meninggalFolder, lampiranFolder, filename),
                Path.Combine(Directory.GetCurrentDirectory(), wwwrootFolder, uploadsFolder, meninggalFolder, lampiranFolder, filename)
            };

            var foundPath = possiblePaths.FirstOrDefault(System.IO.File.Exists);

            if (foundPath == null)
                return NotFound(new { 
                    message = "File tidak ditemukan.", 
                    filename = filename,
                    searchedPaths = possiblePaths.Select(p => p.Replace(Directory.GetCurrentDirectory(), "")).ToArray()
                });

            var fileBytes = System.IO.File.ReadAllBytes(foundPath);
            var contentType = GetContentType(filename);
            
            return File(fileBytes, contentType, filename);
        }

        private static string GetContentType(string filename)
        {
            const string pdfExt = ".pdf";
            const string jpgExt = ".jpg";
            const string jpegExt = ".jpeg";
            const string pngExt = ".png";
            const string txtExt = ".txt";
            
            var extension = Path.GetExtension(filename).ToLowerInvariant();
            return extension switch
            {
                pdfExt => "application/pdf",
                jpgExt or jpegExt => "image/jpeg",
                pngExt => "image/png",
                txtExt => "text/plain",
                _ => "application/octet-stream"
            };
        }





        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateMeninggalDuniaRequest dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png" };
            const int maxFileSize = 10 * 1024 * 1024; 
            const string userIdKey = "UserId";
            const string systemUser = "system";

            if (dto.LampiranFile != null)
            {
                var fileExtension = Path.GetExtension(dto.LampiranFile.FileName).ToLowerInvariant();
                
                if (!allowedExtensions.Contains(fileExtension))
                {
                    return BadRequest(new { message = $"Tipe file lampiran tidak diizinkan. Gunakan: {string.Join(", ", allowedExtensions)}" });
                }

                if (dto.LampiranFile.Length > maxFileSize)
                {
                    return BadRequest(new { message = "Ukuran file lampiran maksimal 10MB." });
                }
            }

            var createdBy = HttpContext.Items[userIdKey]?.ToString() ?? systemUser;
            var id = await _service.CreateAsync(dto, createdBy);
            return Ok(new { id });
        }

        [HttpPost("finalize/{draftId}")]
        public async Task<IActionResult> Finalize(string draftId)
        {
            try
            {
                const string userIdKey = "UserId";
                const string systemUser = "system";
                
                var updatedBy = HttpContext.Items[userIdKey]?.ToString() ?? systemUser;
                var officialId = await _service.FinalizeAsync(draftId, updatedBy);
                
                if (string.IsNullOrEmpty(officialId))
                {
                    return BadRequest(new { 
                        message = "Gagal memfinalisasi draft. Draft mungkin tidak ditemukan, sudah diproses, atau terjadi kesalahan dalam generate ID resmi.",
                        draftId = draftId
                    });
                }

                return Ok(new { 
                    message = "Draft berhasil difinalisasi menjadi pengajuan resmi.",
                    draftId = draftId,
                    officialId = officialId,
                    updatedBy = updatedBy
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { 
                    message = "Terjadi kesalahan saat memfinalisasi draft.",
                    draftId = draftId,
                    error = ex.Message
                });
            }
        }

        

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromForm] UpdateMeninggalDuniaRequest dto)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest(new { message = "ID tidak boleh kosong." });
                }

                var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png" };
                const int maxFileSize = 10 * 1024 * 1024; 
                const string userIdKey = "UserId";
                const string systemUser = "system";

                var updatedBy = HttpContext.Items[userIdKey]?.ToString() ?? systemUser;

                if (dto.LampiranFile != null)
                {
                    var fileExtension = Path.GetExtension(dto.LampiranFile.FileName).ToLowerInvariant();
                    
                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        return BadRequest(new { 
                            message = $"Tipe file tidak diizinkan. Gunakan: {string.Join(", ", allowedExtensions)}" 
                        });
                    }

                    if (dto.LampiranFile.Length > maxFileSize)
                    {
                        return BadRequest(new { message = "Ukuran file maksimal 10MB." });
                    }
                }

                var success = await _service.UpdateAsync(id, dto, updatedBy);

                if (!success)
                {
                    return BadRequest(new { 
                        message = "Gagal memperbarui data. Data mungkin tidak ditemukan.",
                        id = id
                    });
                }

                return Ok(new { 
                    message = "Data berhasil diperbarui.",
                    id = id,
                    updatedBy = updatedBy,
                    hasFile = dto.LampiranFile != null,
                    mhsId = dto.MhsId
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { 
                    message = "Terjadi kesalahan saat memperbarui data.",
                    error = ex.Message,
                    id = id
                });
            }
        }

        [HttpPut("upload-sk")]
        public async Task<IActionResult> UploadSK([FromForm] UploadSKMeninggalRequest request)
        {
            try
            {
                var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png" };
                const int maxFileSize = 10 * 1024 * 1024; 
                const string userIdKey = "UserId";
                const string systemUser = "system";

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
                    request.ModifiedBy = HttpContext.Items[userIdKey]?.ToString() ?? systemUser;
                }

                var skFileExtension = Path.GetExtension(request.SK.FileName).ToLowerInvariant();
                var spkbFileExtension = Path.GetExtension(request.SKPB.FileName).ToLowerInvariant();
                
                if (!allowedExtensions.Contains(skFileExtension))
                {
                    return BadRequest(new { message = $"Tipe file SK tidak diizinkan. Gunakan: {string.Join(", ", allowedExtensions)}" });
                }

                if (!allowedExtensions.Contains(spkbFileExtension))
                {
                    return BadRequest(new { message = $"Tipe file SPKB tidak diizinkan. Gunakan: {string.Join(", ", allowedExtensions)}" });
                }

                if (request.SK.Length > maxFileSize)
                {
                    return BadRequest(new { message = "Ukuran file SK maksimal 10MB." });
                }

                if (request.SKPB.Length > maxFileSize)
                {
                    return BadRequest(new { message = "Ukuran file SPKB maksimal 10MB." });
                }

                var result = await _service.UploadSKAsync(request.MduId, request.SK, request.SKPB, request.ModifiedBy);

                if (!result)
                {
                    return BadRequest(new { message = "Gagal upload SK Meninggal Dunia. Periksa apakah MduId valid dan status adalah 'Menunggu Upload SK'." });
                }

                return Ok(new { 
                    message = "Upload SK berhasil. Status meninggal dunia telah diubah menjadi 'Disetujui'. Nomor SK akan ditampilkan otomatis dengan format tahun 2026.",
                    success = true,
                    mduId = request.MduId,
                    skFileName = request.SK.FileName,
                    spkbFileName = request.SKPB.FileName,
                    modifiedBy = request.ModifiedBy
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { 
                    message = "Terjadi kesalahan saat mengupload SK.", 
                    error = ex.Message,
                    details = ex.InnerException?.Message
                });
            }
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDelete(string id)
        {
            const string userIdKey = "UserId";
            const string systemUser = "system";
            
            var updatedBy = HttpContext.Items[userIdKey]?.ToString() ?? systemUser;

            var result = await _service.SoftDeleteAsync(id, updatedBy);

            if (!result)
                return BadRequest(new { message = "Gagal menghapus data." });

            return Ok(new { message = "Data meninggal dunia berhasil dihapus (soft delete)." });
        }




        [HttpPut("approve/{id}")]
        public async Task<IActionResult> Approve(string id, [FromBody] ApproveMeninggalDuniaRequest dto)
        {
            try
            {
                id = Uri.UnescapeDataString(id);
                
                
                var detectedRole = await _service.DetectUserRoleAsync(dto.Username);
                if (string.IsNullOrEmpty(detectedRole))
                {
                    return BadRequest(new { 
                        message = "Tidak dapat mendeteksi role pengguna. Pastikan username valid.",
                        username = dto.Username
                    });
                }
                
                
                dto.Role = detectedRole;
                
                var result = await _service.ApproveAsync(id, dto);

                if (!result)
                {
                    return BadRequest(new { 
                        message = "Gagal menyetujui pengajuan. Data mungkin tidak ditemukan atau sudah diproses.",
                        id = id,
                        detectedRole = detectedRole,
                        username = dto.Username
                    });
                }

                return Ok(new { 
                    approved = true,
                    id = id,
                    approvedBy = dto.Username,
                    role = detectedRole,
                    message = $"Pengajuan berhasil disetujui oleh {detectedRole}"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { 
                    message = "Terjadi kesalahan saat menyetujui pengajuan.",
                    error = ex.Message,
                    id = id
                });
            }
        }

        [HttpPut("reject/{id}")]
        public async Task<IActionResult> Reject(string id, [FromBody] RejectMeninggalDuniaRequest dto)
        {
            try
            {
                id = Uri.UnescapeDataString(id);
                
                var detectedRole = await _service.DetectUserRoleAsync(dto.Username);
                if (string.IsNullOrEmpty(detectedRole))
                {
                    return BadRequest(new { 
                        message = "Tidak dapat mendeteksi role pengguna. Pastikan username valid.",
                        username = dto.Username
                    });
                }
                
                
                dto.Role = detectedRole;
                
                var success = await _service.RejectAsync(id, dto);

                if (!success)
                {
                    return BadRequest(new { 
                        message = "Gagal menolak pengajuan. Data mungkin tidak ditemukan atau sudah diproses.",
                        id = id,
                        detectedRole = detectedRole,
                        username = dto.Username
                    });
                }

                return Ok(new
                {
                    rejected = true,
                    id = id,
                    rejectedBy = dto.Username,
                    role = detectedRole,
                    message = $"Pengajuan berhasil ditolak oleh {detectedRole}"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { 
                    message = "Terjadi kesalahan saat menolak pengajuan.",
                    error = ex.Message,
                    id = id
                });
            }
        }


        [HttpGet("Riwayat")]
        public async Task<IActionResult> GetRiwayat([FromQuery] GetRiwayatMeninggalDuniaRequest req)
        {
            return Ok(await _service.GetRiwayatAsync(req));
        }
            

        [HttpGet("riwayat/excel")]
        [ProducesResponseType(typeof(FileResult), 200)]
        public async Task<IActionResult> GetRiwayatExcel(
        [FromQuery] string sort = "",
        [FromQuery] string konsentrasi = ""
        )
        {
            try
            {
                var data = await _service.GetRiwayatExcelAsync(sort, konsentrasi);
                
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

                data.Select((item, index) => new { item, rowIndex = index + 2 })
                    .ToList()
                    .ForEach(x =>
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
                
                return File(stream.ToArray(),
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName);
            }
            catch (Exception ex)
            {
                return BadRequest(new { 
                    message = "Terjadi kesalahan saat membuat file Excel.", 
                    error = ex.Message 
                });
            }
        }

        
        [HttpPost("DownloadPdf/{id}")]
        [ProducesResponseType(typeof(FileResult), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> DownloadPdf(string id, [FromQuery] string username, [FromQuery] string role)
        {
            try
            {
                id = Uri.UnescapeDataString(id);

                var validationResult = ValidateDownloadPdfParameters(username, role);
                if (validationResult != null) return validationResult;

                var meninggalDetail = await _service.GetDetailAsync(id);
                if (meninggalDetail == null)
                {
                    return NotFound(new { 
                        message = "Data meninggal dunia tidak ditemukan",
                        id = id,
                        username = username,
                        role = role
                    });
                }

                var roleValidationResult = ValidateRoleAndStatus(role, meninggalDetail.Status);
                if (roleValidationResult != null) return roleValidationResult;

                return await CallReportService(id, username, role, "Report_SK_Meninggal_Dunia", "SK_Meninggal_Dunia");
            }
            catch (HttpRequestException ex)
            {
                return BadRequest(new { 
                    message = "Terjadi kesalahan saat download PDF SK Meninggal Dunia.",
                    error = ex.Message,
                    id = id,
                    username = username,
                    role = role
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { 
                    message = "Terjadi kesalahan saat download PDF SK Meninggal Dunia.",
                    error = ex.Message,
                    id = id,
                    username = username,
                    role = role
                });
            }
        }

        private IActionResult? ValidateDownloadPdfParameters(string username, string role)
        {
            if (string.IsNullOrEmpty(username))
            {
                return BadRequest(new { 
                    message = "Parameter username harus diisi",
                    username = username
                });
            }

            if (string.IsNullOrEmpty(role))
            {
                return BadRequest(new { 
                    message = "Parameter role harus diisi",
                    username = username,
                    role = role
                });
            }

            return null;
        }

        private IActionResult? ValidateRoleAndStatus(string role, string? status)
        {
            if (string.IsNullOrEmpty(status))
            {
                return BadRequest(new { 
                    message = "Status tidak ditemukan",
                    role = role
                });
            }

            return role switch
            {
                "ROL23" when status != "Disetujui" => StatusCode(403, new { 
                    message = "Mahasiswa hanya dapat cetak SK saat status 'Disetujui'",
                    currentStatus = status,
                    requiredStatus = "Disetujui",
                    role = role
                }),
                "ROL21" when status != "Menunggu Upload SK" => StatusCode(403, new { 
                    message = "Admin Akademik hanya dapat cetak SK saat status 'Menunggu Upload SK'",
                    currentStatus = status,
                    requiredStatus = "Menunggu Upload SK",
                    role = role
                }),
                "ROL23" or "ROL21" => null,
                _ => StatusCode(403, new { 
                    message = "Role tidak memiliki akses untuk cetak SK",
                    role = role,
                    allowedRoles = new[] { "ROL23", "ROL21" }
                })
            };
        }

        private async Task<IActionResult> CallReportService(string id, string username, string role, string reportName, string filePrefix)
        {
            var client = _httpClientFactory.CreateClient();
            var url = _configuration["Key:reportServiceUrl"];

            if (string.IsNullOrEmpty(url))
            {
                return BadRequest(new { 
                    message = "URL service report tidak dikonfigurasi",
                    id = id,
                    username = username,
                    role = role
                });
            }

            var requestBody = new
            {
                reportName = reportName,
                parameters = new { id }
            };

            var content = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json"
            );

            try
            {
                var response = await client.PostAsync(url, content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    
                    if (errorContent.Contains("database logon failed") || 
                        errorContent.Contains("error crystal report"))
                    {
                        return BadRequest(new { 
                            message = "Service report berhasil terhubung namun terjadi error database/crystal report",
                            error = errorContent,
                            connectionStatus = "Connected - Database/Crystal Report Error",
                            id = id,
                            username = username,
                            role = role
                        });
                    }
                    
                    return BadRequest(new { 
                        message = "Gagal mengambil file PDF dari service report",
                        error = errorContent,
                        statusCode = (int)response.StatusCode,
                        id = id,
                        username = username,
                        role = role
                    });
                }

                var pdfBytes = await response.Content.ReadAsByteArrayAsync();
                return File(pdfBytes, "application/pdf", $"{filePrefix}_{id.Replace("/", "_")}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");
            }
            catch (HttpRequestException ex)
            {
                return BadRequest(new { 
                    message = "Terjadi kesalahan saat download PDF SK Meninggal Dunia.",
                    error = ex.Message,
                    id = id,
                    username = username,
                    role = role
                });
            }
        }

    }
}
