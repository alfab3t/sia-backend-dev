using astratech_apps_backend.DTOs.TemplateKuesioner;
using astratech_apps_backend.Models;
using astratech_apps_backend.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Globalization;
using OfficeOpenXml;

namespace astratech_apps_backend.Repositories.Implementations
{
    public class TemplateKuesionerRepository(IConfiguration config) : ITemplateKuesionerRepository
    {
        private readonly string _conn = PolmanAstraLibrary.PolmanAstraLibrary.Decrypt(config.GetConnectionString("DefaultConnection")!, Environment.GetEnvironmentVariable("DECRYPT_KEY_CONNECTION_STRING"));

        public async Task<int> CreateTemplateAsync(CreateTemplateRequest dto, string createdBy)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_createTemplateKuesioner", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@NamaTemplate", dto.NamaTemplate);
            cmd.Parameters.AddWithValue("@IdJenisKuesioner", dto.IdJenisKuesioner);
            cmd.Parameters.AddWithValue("@IdSkala", dto.IdSkalaPenilaian);
            cmd.Parameters.AddWithValue("@CreatedBy", createdBy);

            await conn.OpenAsync();
            var newId = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(newId);
        }

        public async Task<(IEnumerable<TemplateKuesioner>, int totalData)> GetAllAsync(GetAllTemplateRequest dto)
        {
            var list = new List<TemplateKuesioner>();
            int totalData = 0;

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getDataTemplateKuesioner", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Keyword", dto.SearchKeyword);
            cmd.Parameters.AddWithValue("@Status", dto.Status);
            cmd.Parameters.AddWithValue("@Urut", dto.Urut);
            cmd.Parameters.AddWithValue("@Halaman", dto.PageNumber);
            cmd.Parameters.AddWithValue("@Limit", dto.PageSize);

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                totalData = reader.GetInt32(reader.GetOrdinal("Count"));

                do
                {
                    var tglFinalOrdinal = reader.GetOrdinal("tglFinal");
                    DateTime? tanggalFinalValue = null;

                    if (!await reader.IsDBNullAsync(tglFinalOrdinal))
                    {
                        string tglFinalString = reader.GetString(tglFinalOrdinal);
                        if (tglFinalString != "-" && DateTime.TryParse(tglFinalString, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
                        {
                            tanggalFinalValue = parsedDate;
                        }
                    }

                    list.Add(new TemplateKuesioner
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("tku_id")),
                        RowNumber = reader.GetInt64(reader.GetOrdinal("rownum")),
                        NamaTemplate = reader.GetString(reader.GetOrdinal("tku_nama")),
                        JenisKuesioner = reader.GetString(reader.GetOrdinal("jku_nama")),
                        Skala = reader.GetString(reader.GetOrdinal("skala")),
                        TanggalFinal = tanggalFinalValue,
                        Status = reader.GetString(reader.GetOrdinal("tku_status")),
                    });

                } while (await reader.ReadAsync());
            }

            return (list, totalData);
        }

        public async Task<TemplateKuesioner?> GetByIdAsync(int id)
        {   
            TemplateKuesioner? item = null;
            await using var conn = new SqlConnection(_conn);

            await using var cmd = new SqlCommand("sia_detailTemplateKuesioner", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Id", id);

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                string tglString = reader.GetString(reader.GetOrdinal("tku_modif_date"));
                DateTime? tanggalFinalValue = null;

                if (tglString != "-" && DateTime.TryParse(tglString, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
                {
                    tanggalFinalValue = parsedDate;
                }

                item = new TemplateKuesioner
                {
                    Id = id,
                    NamaTemplate = reader.GetString(reader.GetOrdinal("tku_nama")),
                    JenisKuesioner = reader.GetString(reader.GetOrdinal("jku_nama")),
                    Skala = reader.GetString(reader.GetOrdinal("skala")),
                    TanggalFinal = tanggalFinalValue,
                    Status = reader.GetString(reader.GetOrdinal("tku_status")),
                    IdJenisKuesioner = reader.GetInt32(reader.GetOrdinal("jku_id")),
                    IdSkala = reader.GetInt32(reader.GetOrdinal("spe_id"))
                };
            }

            return item;
        }

        public async Task<IEnumerable<Pertanyaan>> GetPertanyaanByTemplateIdAsync(int templateId)
        {
            var list = new List<Pertanyaan>();
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getDataPertanyaanKuesioner", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@TemplateId", templateId);

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new Pertanyaan
                {
                    Id = reader.GetInt32(reader.GetOrdinal("tkd_id")),
                    IsHeader = reader.GetString(reader.GetOrdinal("tkd_isheader")) == "Ya",
                    PertanyaanText = reader.GetString(reader.GetOrdinal("tkd_pertanyaan")),
                    Jenis = reader.GetString(reader.GetOrdinal("tkd_jenis")),
                });
            }
            return list;
        }

        public async Task<int> CreatePertanyaanAsync(CreateUpdatePertanyaanRequest dto, string createdBy)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_createPertanyaanKuesioner", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@Id", dto.Id);
            cmd.Parameters.AddWithValue("@Pertanyaan", dto.PertanyaanText);
            cmd.Parameters.AddWithValue("@IsHeader", dto.IsHeader ? "1" : "0");
            cmd.Parameters.AddWithValue("@Username", createdBy);
            cmd.Parameters.AddWithValue("@Jenis", dto.Jenis);

            await conn.OpenAsync();
            var newId = await cmd.ExecuteNonQueryAsync();
            return Convert.ToInt32(newId);
        }

        public async Task<bool> UpdatePertanyaanAsync(CreateUpdatePertanyaanRequest dto, string updatedBy)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_editPertanyaanKuesioner", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@Id", dto.Id);
            cmd.Parameters.AddWithValue("@Pertanyaan", dto.PertanyaanText);
            cmd.Parameters.AddWithValue("@IsHeader", dto.IsHeader ? "1" : "0");
            cmd.Parameters.AddWithValue("@Jenis", dto.Jenis);
            cmd.Parameters.AddWithValue("@Username", updatedBy);

            await conn.OpenAsync();
            try
            {
                await cmd.ExecuteNonQueryAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> SetStatusPertanyaanAsync(int id, string newStatus, string updatedBy)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_deletePertanyaan", conn)
            {   
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@Status", newStatus);
            cmd.Parameters.AddWithValue("@Id", id);
            await conn.OpenAsync();
            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> SetStatusTemplateAsync(int id, string newStatus, string updatedBy)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_deleteTemplateKuesioner", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@Status", newStatus);
            cmd.Parameters.AddWithValue("@Id", id);

            await conn.OpenAsync();
            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> SetFinalAsync(int id, string updatedBy)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_setTemplateKuesioerFinal", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@UpdatedBy", updatedBy);

            await conn.OpenAsync();
            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> IsTemplateEmptyAsync(int id)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_checkTemplateKuesioner", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@Id", id);

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return (await reader.ReadAsync());
            }
            return true;
        }

        public async Task<bool> UpdateTemplateAsync(UpdateTemplateRequest dto, string updatedBy)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_editTemplateKuesioner", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@Id", dto.Id);
            cmd.Parameters.AddWithValue("@NamaTemplate", dto.NamaTemplate);
            cmd.Parameters.AddWithValue("@IdJenis", dto.IdJenisKuesioner);
            cmd.Parameters.AddWithValue("@IdSkala", dto.IdSkalaPenilaian);
            cmd.Parameters.AddWithValue("@UpdatedBy", updatedBy);

            await conn.OpenAsync();
            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        public async Task<PreviewTemplateDto?> GetPreviewAsync(int id)
        {
            var result = new PreviewTemplateDto { TemplateId = id };

            await using var conn = new SqlConnection(_conn);
            await conn.OpenAsync();

    
            using (var cmdHead = new SqlCommand("sia_detailTemplateKuesioner", conn) { CommandType = CommandType.StoredProcedure })
            {
                cmdHead.Parameters.AddWithValue("@Id", id); 

                using var readerHead = await cmdHead.ExecuteReaderAsync();
                if (await readerHead.ReadAsync())
                {
                    string skalaStr = readerHead.GetString(readerHead.GetOrdinal("skala"));
                  
                    int skalaMax = 5;
                    if (!string.IsNullOrEmpty(skalaStr))
                    {
                        var parts = skalaStr.Split('(');
                        if (parts.Length > 0 && int.TryParse(parts[0].Trim(), out int val))
                        {
                            skalaMax = val;
                        }
                    }
                    result.SkalaMax = skalaMax;
                }
                else
                {
                    return null;
                }
            }

            using (var cmdQ = new SqlCommand("sia_getDataPreviewTemplateKuesioner", conn) { CommandType = CommandType.StoredProcedure })
            {
                cmdQ.Parameters.AddWithValue("@Id", id);

                using var reader = await cmdQ.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    var isHeaderVal = reader.GetValue(reader.GetOrdinal("tkd_isheader"));
                    bool isHeader = false;
                    if (isHeaderVal is string s) isHeader = s == "1" || s.Equals("Ya", StringComparison.OrdinalIgnoreCase);
                    else if (isHeaderVal is int i) isHeader = i == 1;
                    else if (isHeaderVal is bool b) isHeader = b;

                    result.PertanyaanList.Add(new PreviewPertanyaanDto
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("tkd_id")),
                        Pertanyaan = reader.GetString(reader.GetOrdinal("tkd_pertanyaan")),
                        IsHeader = isHeader,
                        Jenis = reader.GetString(reader.GetOrdinal("tkd_jenis"))
                    });
                }
            }

            return result;
        }

        public async Task<(bool Success, string Message)> ImportPertanyaanAsync(ImportPertanyaanRequest dto, string createdBy)
        {
            var (IsValid, Message) = ValidateImportFile(dto.File);
            if (!IsValid)
            {
                return (false, Message);
            }

            List<(string Pertanyaan, bool IsHeader, string Jenis)> listPertanyaan;
            try
            {
                listPertanyaan = await ParseExcelDataAsync(dto.File);

                if (listPertanyaan.Count == 0)
                    return (false, "Tidak ada data pertanyaan yang valid untuk diimport.");
            }
            catch (Exception ex)
            {
                return (false, "Gagal memproses file Excel: " + ex.Message);
            }

            try
            {
                await SaveImportToDatabaseAsync(dto.IdTemplate, listPertanyaan, createdBy);
                return (true, "Import berhasil. Pertanyaan lama telah diganti dengan data baru.");
            }
            catch (Exception ex)
            {
                return (false, "Gagal menyimpan ke database: " + ex.Message);
            }
        }

        private static (bool IsValid, string Message) ValidateImportFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return (false, "File tidak boleh kosong.");

            var extension = Path.GetExtension(file.FileName).ToLower();
            if (extension != ".xlsx")
                return (false, "Format file harus .xlsx");

            if (file.Length > 2 * 1024 * 1024)
                return (false, "Ukuran file maksimal 2 MB.");

            return (true, string.Empty);
        }

        private static async Task<List<(string Pertanyaan, bool IsHeader, string Jenis)>> ParseExcelDataAsync(IFormFile file)
        {
            var result = new List<(string, bool, string)>();

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);

            using var package = new ExcelPackage(stream);
            var worksheet = package.Workbook.Worksheets[0] ?? throw new InvalidDataException("Sheet Excel tidak ditemukan.");
            int rowCount = worksheet.Dimension?.Rows ?? 0;

            if (rowCount < 2)
                throw new InvalidDataException("File Excel kosong atau tidak ada data (hanya header).");

            for (int row = 2; row <= rowCount; row++)
            {
                var colHeader = worksheet.Cells[row, 1].Value?.ToString()?.Trim();
                var colPertanyaan = worksheet.Cells[row, 2].Value?.ToString()?.Trim();
                var colJenis = worksheet.Cells[row, 3].Value?.ToString()?.Trim();

                if (string.IsNullOrEmpty(colHeader) && string.IsNullOrEmpty(colPertanyaan) && string.IsNullOrEmpty(colJenis))
                    continue;
                if (string.IsNullOrEmpty(colHeader))
                    throw new InvalidDataException($"Baris {row}: Kolom 'Header?' harus diisi (Ya/Tidak).");
                if (string.IsNullOrEmpty(colPertanyaan))
                    throw new InvalidDataException($"Baris {row}: Kolom 'Pertanyaan' harus diisi.");

                bool isHeader = colHeader.Equals("Ya", StringComparison.OrdinalIgnoreCase) || 
                                colHeader.Equals("Yes", StringComparison.OrdinalIgnoreCase) ||
                                colHeader.Equals("1");

                if (!isHeader && string.IsNullOrEmpty(colJenis))
                    throw new InvalidDataException($"Baris {row}: Kolom 'Jenis' harus diisi jika pertanyaan ini bukan header.");

                string jenisFinal = colJenis ?? "-";

                result.Add((colPertanyaan, isHeader, jenisFinal));
            }

            return result;
        }
        private async Task SaveImportToDatabaseAsync(int Id, List<(string Pertanyaan, bool IsHeader, string Jenis)> data, string createdBy)
        {
            await using var conn = new SqlConnection(_conn);
            await conn.OpenAsync();

            using var transaction = (SqlTransaction)await conn.BeginTransactionAsync();

            try
            {
                foreach (var item in data)
                {
                    using var cmdInsert = new SqlCommand("sia_createPertanyaanKuesioner", conn, transaction)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmdInsert.Parameters.AddWithValue("@Id", Id);
                    cmdInsert.Parameters.AddWithValue("@Pertanyaan", item.Pertanyaan);
                    cmdInsert.Parameters.AddWithValue("@IsHeader", item.IsHeader ? "1" : "0");
                    cmdInsert.Parameters.AddWithValue("@Username", createdBy);
                    cmdInsert.Parameters.AddWithValue("@Jenis", item.Jenis);

                    await cmdInsert.ExecuteNonQueryAsync();
                }

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}