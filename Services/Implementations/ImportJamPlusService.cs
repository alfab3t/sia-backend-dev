using astratech_apps_backend.DTOs.JamPlus;
using astratech_apps_backend.Repositories.Interfaces;
using OfficeOpenXml;
using System.Globalization;

namespace astratech_apps_backend.Services.Implementations
{
    public class ImportJamPlusService
    {
        private readonly IJamPlusRepository _repo;

        public ImportJamPlusService(IJamPlusRepository repo)
        {
            _repo = repo;
        }

        
        public async Task<(int Success, int Failed, string Message, List<string> ErrorDetails)> ProcessImportAsync(ImportJamPlusRequest dto, string createdBy)
        {
            int success = 0;
            int failed = 0;
            List<string> errorDetails = new List<string>(); 

            if (dto.File == null || dto.File.Length == 0)
            {
                return (0, 0, "File tidak boleh kosong", errorDetails);
            }

            if (!dto.File.FileName.EndsWith(".xlsx"))
            {
                return (0, 0, "Format file harus .xlsx", errorDetails);
            }

            try
            {
                using (var stream = new MemoryStream())
                {
                    await dto.File.CopyToAsync(stream);
                    using (var package = new ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets[0];
                        if (worksheet == null) return (0, 0, "Worksheet tidak ditemukan", errorDetails);

                        int rowCount = worksheet.Dimension.Rows;

                        for (int row = 2; row <= rowCount; row++)
                        {
                            try
                            {
                            
                                var nim = worksheet.Cells[row, 2].Value?.ToString()?.Trim();
                                if (string.IsNullOrEmpty(nim))
                                {
                                    failed++;
                                    errorDetails.Add($"Baris {row}: NIM Kosong");
                                    continue;
                                }

                              
                                var jenis = worksheet.Cells[row, 4].Value?.ToString()?.Trim();
                                var deskripsi = worksheet.Cells[row, 6].Value?.ToString()?.Trim();

                                decimal jumlahJam = 0;
                                var cellJumlah = worksheet.Cells[row, 5].Value;

                                if (cellJumlah is double || cellJumlah is decimal || cellJumlah is int)
                                {
                                
                                    jumlahJam = Convert.ToDecimal(cellJumlah);
                                }
                                else if (cellJumlah is string strJumlah)
                                {
                                   
                                    if (!decimal.TryParse(strJumlah, NumberStyles.Any, CultureInfo.GetCultureInfo("id-ID"), out jumlahJam) &&
                                        !decimal.TryParse(strJumlah, NumberStyles.Any, CultureInfo.InvariantCulture, out jumlahJam))
                                    {
                                        failed++;
                                        errorDetails.Add($"Baris {row}: Format Jumlah salah ({strJumlah})");
                                        continue;
                                    }
                                }
                                else
                                {
                                    failed++;
                                    errorDetails.Add($"Baris {row}: Kolom Jumlah kosong/invalid");
                                    continue;
                                }

                               
                                DateTime tanggal;
                                var cellTanggal = worksheet.Cells[row, 7].Value;

                                if (cellTanggal is DateTime dt)
                                {
                                   
                                    tanggal = dt;
                                }
                                else if (cellTanggal is double dblDate)
                                {
                                    
                                    tanggal = DateTime.FromOADate(dblDate);
                                }
                                else
                                {
                                    
                                    var tanggalStr = cellTanggal?.ToString()?.Trim();
                                    string[] formats = { "yyyy-MM-dd", "dd/MM/yyyy", "dd-MM-yyyy", "yyyy/MM/dd" };

                                    if (!DateTime.TryParseExact(tanggalStr, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out tanggal))
                                    {
                                        failed++;
                                        errorDetails.Add($"Baris {row}: Format Tanggal salah ({tanggalStr})");
                                        continue;
                                    }
                                }

                                
                                var dataToSave = new CreateJamplus
                                {
                                    MahasiswaId = nim,
                                    Jumlah = jumlahJam,
                                    Jenis = string.IsNullOrEmpty(jenis) ? "Lainnya" : jenis,
                                    Deskripsi = string.IsNullOrEmpty(deskripsi) ? "Import via Excel" : deskripsi,
                                    Tanggal = tanggal,
                                    TahunAkademik = dto.TahunAkademik,
                                    Semester = dto.Semester
                                };

                                
                                await _repo.CreateAsync(dataToSave, createdBy);
                                success++;
                            }
                            catch (Exception ex)
                            {
                                failed++;
                                
                                errorDetails.Add($"Baris {row}: DB Error - {ex.Message}");
                            }
                        }
                    }
                }

                
                string finalMessage = "Proses import selesai";
                if (failed > 0)
                {
                    finalMessage += ". Cek console/response untuk detail error.";
                }

                return (success, failed, finalMessage, errorDetails);
            }
            catch (Exception ex)
            {
                return (0, 0, $"System Error: {ex.Message}", errorDetails);
            }
        }
    }
}