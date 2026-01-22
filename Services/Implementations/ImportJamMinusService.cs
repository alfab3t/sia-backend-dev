using astratech_apps_backend.DTOs.Jam_Minus; 
using astratech_apps_backend.DTOs.JamMinus; 
using astratech_apps_backend.Repositories.Interfaces;
using OfficeOpenXml;
using System.Globalization;

namespace astratech_apps_backend.Services.Implementations
{
    public class ImportJamMinusService
    {
        private readonly IJamMinusRepository _repo;

        public ImportJamMinusService(IJamMinusRepository repo)
        {
            _repo = repo;
        }

        public async Task<(int Success, int Failed, string Message)> ProcessImportAsync(ImportJamMinusRequest dto, string createdBy)
        {
            int success = 0;
            int failed = 0;

            if (dto.File == null || dto.File.Length == 0)
            {
                return (0, 0, "File tidak boleh kosong");
            }

            try
            {
                using (var stream = new MemoryStream())
                {
                    await dto.File.CopyToAsync(stream);

                    using (var package = new ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets[0];
                        if (worksheet == null) return (0, 0, "Worksheet tidak ditemukan");

                        int rowCount = worksheet.Dimension.Rows;
                        for (int row = 2; row <= rowCount; row++)
                        {
                            try
                            {
                                var nim = worksheet.Cells[row, 1].Value?.ToString()?.Trim();
                                var jumlahStr = worksheet.Cells[row, 2].Value?.ToString()?.Trim();
                                var jenis = worksheet.Cells[row, 3].Value?.ToString()?.Trim();
                                var deskripsi = worksheet.Cells[row, 4].Value?.ToString()?.Trim();
                                var tanggalValue = worksheet.Cells[row, 5].Value;

                                if (string.IsNullOrEmpty(nim)) continue;

                                if (!decimal.TryParse(jumlahStr, NumberStyles.Any, CultureInfo.GetCultureInfo("id-ID"), out decimal jumlahJam))
                                {
                                    failed++;
                                    continue;
                                }

                                DateTime tanggal;
                                if (tanggalValue is DateTime dt)
                                {
                                    tanggal = dt;
                                }
                                else
                                {
                                    if (!DateTime.TryParse(tanggalValue?.ToString(), CultureInfo.InvariantCulture, DateTimeStyles.None, out tanggal))
                                    {
                                        failed++;
                                        continue;
                                    }
                                }

                                var dataToSave = new CreateJamMinus
                                {
                                    Nim = nim,
                                    Jumlah = jumlahJam,
                                    Jenis = string.IsNullOrEmpty(jenis) ? "Lainnya" : jenis,
                                    Deskripsi = deskripsi,
                                    Tanggal = tanggal,
                                    TahunAkademik = dto.TahunAkademik,
                                    Semester = dto.Semester
                                };

                                await _repo.CreateAsync(dataToSave, createdBy);
                                success++;
                            }
                            catch (Exception)
                            {
                                failed++;
                            }
                        }
                    }
                }
                return (success, failed, "Proses import selesai");
            }
            catch (Exception ex)
            {
                return (0, 0, $"System Error: {ex.Message}");
            }
        }
    }
}