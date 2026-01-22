using astratech_apps_backend.DTOs.Kuesioner;
using astratech_apps_backend.Models;
using astratech_apps_backend.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace astratech_apps_backend.Repositories.Implementations
{
    public class KuesionerRepository(IConfiguration config) : IKuesionerRepository
    {
        private readonly string _conn = PolmanAstraLibrary.PolmanAstraLibrary.Decrypt(config.GetConnectionString("DefaultConnection")!, Environment.GetEnvironmentVariable("DECRYPT_KEY_CONNECTION_STRING"));
        private const string JenisDosen = "Dosen";
        private const string JenisFasilitas = "Fasilitas";
        private const string JenisResponden = "Responden";
        private const string TahunAkademik = "TahunAkademik";
        private const string Semester = "Semester";
        private const string Prodi = "Prodi";
        private const string Username = "Username";
        private const string Mahasiswa = "MhsId";
        private const string Kuesioner = "KueId";

        public async Task<(IEnumerable<KuesionerDto>, int totalData)> GetAllAsync(GetAllKuesionerRequest dto, string username, string roleId)
        {
            var list = new List<KuesionerDto>();
            int totalData = 0;

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getDataKuesioner", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@SearchKeyword", dto.SearchKeyword ?? "");
            cmd.Parameters.AddWithValue("@Urut", dto.Urut);
            cmd.Parameters.AddWithValue(TahunAkademik, dto.TahunAkademik ?? "");
            cmd.Parameters.AddWithValue(Semester, dto.Semester ?? "");
            cmd.Parameters.AddWithValue(Prodi, dto.Prodi ?? "");
            cmd.Parameters.AddWithValue("@Id", roleId);
            cmd.Parameters.AddWithValue(Username, username);
            cmd.Parameters.AddWithValue("@Halaman", dto.PageNumber);
            cmd.Parameters.AddWithValue("@Limit", dto.PageSize);

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                totalData = reader.GetInt32(reader.GetOrdinal("Count"));
                do
                {
                    list.Add(new KuesionerDto
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("kue_id")),
                        RowNumber = reader.GetInt64(reader.GetOrdinal("rownum")),
                        NamaKuesioner = reader.GetString(reader.GetOrdinal("kue_nama")),
                        TahunAkademik = reader.GetString(reader.GetOrdinal("kue_tahun_akademik")),
                        Semester = reader.GetInt32(reader.GetOrdinal("kue_semester")),
                        Prodi = reader.GetString(reader.GetOrdinal("kon_singkatan")),
                        Kelas = reader.GetString(reader.GetOrdinal("kel_id")),
                        StatusPengisian = reader.GetString(reader.GetOrdinal("status"))
                    });
                } while (await reader.ReadAsync());
            }
            return (list, totalData);
        }


        public async Task<(bool Success, string Message)> GenerateKuesionerAsync(GenerateKuesionerRequest dto, string createdBy)
        {
            string namaJenis = "";

            await using var conn = new SqlConnection(_conn);
            await conn.OpenAsync();

            using (var cmdJenis = new SqlCommand("sia_detailJenisKuesioner", conn) { CommandType = CommandType.StoredProcedure })
            {
                cmdJenis.Parameters.AddWithValue("@Id", dto.IdJenisKuesioner);

                using var reader = await cmdJenis.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    namaJenis = reader.GetString(reader.GetOrdinal("jku_nama"));
                }
                else
                {
                    return (false, "Jenis Kuesioner tidak ditemukan.");
                }
            }

            using (var cmdCheck = new SqlCommand("sia_checkKuesioner", conn) { CommandType = CommandType.StoredProcedure })
            {
                cmdCheck.Parameters.AddWithValue(TahunAkademik, dto.TahunAkademik);
                cmdCheck.Parameters.AddWithValue(Semester, dto.Semester);
                cmdCheck.Parameters.AddWithValue("@JenisKue", dto.IdJenisKuesioner);
                using var reader = await cmdCheck.ExecuteReaderAsync();
                if (reader.HasRows)
                {
                    return (false, $"Kuesioner {namaJenis} untuk {dto.TahunAkademik} Semester {dto.Semester} sudah ada.");
                }
            }

            DataTable dtEntities = new();
            string spSource = namaJenis.Equals(JenisDosen, StringComparison.OrdinalIgnoreCase)
                ? "sia_getKuesionerDosen"
                : "sia_getKuesionerFasilitas";

            if (namaJenis.Equals(JenisDosen, StringComparison.OrdinalIgnoreCase) && dto.PersentaseKehadiran.HasValue)
            {
                spSource = "sia_getKuesionerDosenNotNull";
            }

            using (var cmdSource = new SqlCommand(spSource, conn) { CommandType = CommandType.StoredProcedure })
            {
                cmdSource.Parameters.AddWithValue(TahunAkademik, dto.TahunAkademik);
                cmdSource.Parameters.AddWithValue(Semester, dto.Semester);

                if (dto.PersentaseKehadiran.HasValue && spSource == "sia_getKuesionerDosenNotNull")
                {
                    cmdSource.Parameters.AddWithValue("@percent", dto.PersentaseKehadiran.Value);
                }
                else if (spSource == "sia_getKuesionerFasilitas")
                {
                    cmdSource.Parameters.AddWithValue("@nama", namaJenis);
                }

                using var reader = await cmdSource.ExecuteReaderAsync();
                dtEntities.Load(reader);
            }

            if (dtEntities.Rows.Count == 0)
            {
                return (false, $"Tidak ada data jadwal/entitas untuk digenerate pada periode {dto.TahunAkademik} {dto.Semester}.");
            }

            using var transaction = await conn.BeginTransactionAsync();
            try
            {
                foreach (DataRow row in dtEntities.Rows)
                {
                    using var cmdInsert = new SqlCommand("sia_createKuesioner", conn, (SqlTransaction) transaction) { CommandType = CommandType.StoredProcedure };

                    if (namaJenis.Equals(JenisDosen, StringComparison.OrdinalIgnoreCase))
                    {
                        cmdInsert.Parameters.AddWithValue("@NamaKue", row[0].ToString());
                        cmdInsert.Parameters.AddWithValue("@TemplateKue", dto.IdTemplateKuesioner);
                        cmdInsert.Parameters.AddWithValue(TahunAkademik, row[1].ToString());
                        cmdInsert.Parameters.AddWithValue(Semester, row[2].ToString());
                        cmdInsert.Parameters.AddWithValue("@Konsentrasi", row[3].ToString());
                        cmdInsert.Parameters.AddWithValue("@MataKuliah", row[4].ToString());
                        cmdInsert.Parameters.AddWithValue("@Dosen", row[6].ToString());
                        cmdInsert.Parameters.AddWithValue("@Kel", row[7].ToString());
                        cmdInsert.Parameters.AddWithValue("@Sec", row[5].ToString());
                        cmdInsert.Parameters.AddWithValue("@Gru", row["gru_id"].ToString());
                        cmdInsert.Parameters.AddWithValue(Username, createdBy);
                    }
                    else
                    {
                        cmdInsert.Parameters.AddWithValue("@NamaKue", row[0].ToString());
                        cmdInsert.Parameters.AddWithValue("@TemplateKue", dto.IdTemplateKuesioner);
                        cmdInsert.Parameters.AddWithValue(TahunAkademik, row[1].ToString());
                        cmdInsert.Parameters.AddWithValue(Semester, row[2].ToString());
                        cmdInsert.Parameters.AddWithValue("@Konsentrasi", row[4].ToString());
                        cmdInsert.Parameters.AddWithValue("@MataKuliah", DBNull.Value);
                        cmdInsert.Parameters.AddWithValue("@Dosen", DBNull.Value);
                        cmdInsert.Parameters.AddWithValue("@Kel", DBNull.Value);
                        cmdInsert.Parameters.AddWithValue("@Sec", row[3].ToString());
                        cmdInsert.Parameters.AddWithValue("@Gru", DBNull.Value);
                        cmdInsert.Parameters.AddWithValue(Username, createdBy);
                    }

                    await cmdInsert.ExecuteNonQueryAsync();
                }
                await transaction.CommitAsync();
                return (true, "Kuesioner berhasil digenerate.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return (false, "Gagal generate: " + ex.Message);
            }
        }

        public async Task<(bool Success, string Message)> SubmitJawabanAsync(SubmitKuesionerRequest dto, string username)
        {
            await using var conn = new SqlConnection(_conn);
            await conn.OpenAsync();

            using (var cmdCheck = new SqlCommand("sia_checkSubmitKuesioner", conn) { CommandType = CommandType.StoredProcedure })
            {
                cmdCheck.Parameters.AddWithValue(Mahasiswa, username);
                cmdCheck.Parameters.AddWithValue(Kuesioner, dto.KuesionerId);
                using var reader = await cmdCheck.ExecuteReaderAsync();
                if (reader.HasRows) return (false, "Anda sudah mengisi kuesioner ini sebelumnya.");
            }

            using var transaction = await conn.BeginTransactionAsync();
            try
            {
                int headerId = 0;
                using (var cmdHead = new SqlCommand("sia_createKuesionerMahasiswa", conn, (SqlTransaction)transaction) { CommandType = CommandType.StoredProcedure })
                {
                    cmdHead.Parameters.AddWithValue(Kuesioner, dto.KuesionerId);
                    cmdHead.Parameters.AddWithValue(Mahasiswa, username);
                    cmdHead.Parameters.AddWithValue("@Comment", dto.KritikSaran ?? "");
                    cmdHead.Parameters.AddWithValue("@Username", username);

                    var res = await cmdHead.ExecuteScalarAsync();
                    headerId = Convert.ToInt32(res);
                }

                foreach (var jwb in dto.JawabanList)
                {
                    using var cmdDet = new SqlCommand("sia_createKuesionerMahasiswaDetail", conn, (SqlTransaction)transaction) { CommandType = CommandType.StoredProcedure };
                    cmdDet.Parameters.AddWithValue("@KmaId", headerId);
                    cmdDet.Parameters.AddWithValue("@TkdId", jwb.PertanyaanId);
                    cmdDet.Parameters.AddWithValue("@KmdJwb", jwb.Jawaban); 
                    cmdDet.Parameters.AddWithValue("@Username", username);

                    string jenisJwb = int.TryParse(jwb.Jawaban, out _) ? "PG" : "JS";
                    cmdDet.Parameters.AddWithValue("@JenisJwb", jenisJwb);

                    await cmdDet.ExecuteNonQueryAsync();
                }

                await transaction.CommitAsync();
                return (true, "Berhasil");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return (false, "Gagal submit: " + ex.Message);
            }
        }

        public async Task<KuesionerResultDto?> GetResultForAdminAsync(int kuesionerId)
        {
            await using var conn = new SqlConnection(_conn);
            await conn.OpenAsync();

            var result = await FetchKuesionerHeaderAsync(conn, kuesionerId);
            if (result == null) return null;

            result.Summary = await FetchKuesionerSummaryAsync(conn, kuesionerId, result.SkalaMax);

            result.KritikSaran = await FetchKritikSaranAsync(conn, kuesionerId);

            return result;
        }

        private static async Task<KuesionerResultDto?> FetchKuesionerHeaderAsync(SqlConnection conn, int id)
        {
            using var cmd = new SqlCommand("sia_detailKuesioner", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Id", id);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new KuesionerResultDto
                {
                    NamaKuesioner = reader.GetString(0),
                    SkalaMax = reader.GetInt32(2)
                };
            }
            return null;
        }

        private static async Task<List<KuesionerSummaryDto>> FetchKuesionerSummaryAsync(SqlConnection conn, int id, int skalaMax)
        {
            var list = new List<KuesionerSummaryDto>();

            using var cmd = new SqlCommand("sia_summaryKuesioner", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Id", id);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var item = new KuesionerSummaryDto
                {
                    PertanyaanId = reader.GetInt32(reader.GetOrdinal("tkd_id")),
                    PertanyaanText = reader.GetString(reader.GetOrdinal("tkd_pertanyaan")),
                    IsHeader = Convert.ToBoolean(reader.GetValue(reader.GetOrdinal("tkd_isheader"))),
                    JenisPertanyaan = reader.GetString(reader.GetOrdinal("tkd_jenis")),
                };

                if (!item.IsHeader && item.JenisPertanyaan == "Pilihan Ganda")
                {
                    await CalculateStatisticsAsync(reader, item, skalaMax);
                }

                list.Add(item);
            }
            return list;
        }

        private static async Task CalculateStatisticsAsync(SqlDataReader reader, KuesionerSummaryDto item, int skalaMax)
        {
            int totalCount = 0;
            decimal weightedSum = 0;

            for (int i = 1; i <= skalaMax; i++)
            {
                string colName = i.ToString();
                int count = 0;

                int colIndex;
                try { colIndex = reader.GetOrdinal(colName); } catch { colIndex = -1; }

                if (colIndex != -1 && !await reader.IsDBNullAsync(colIndex))
                {
                    count = Convert.ToInt32(reader.GetValue(colIndex));
                }

                item.PilihanJawabanCount.Add(colName, count);
                totalCount += count;
                weightedSum += (count * i);
            }

            item.TotalResponden = totalCount;
            item.RataRata = totalCount > 0 ? Math.Round(weightedSum / totalCount, 2) : 0;
        }

        private static async Task<List<KritikSaranDto>> FetchKritikSaranAsync(SqlConnection conn, int id)
        {
            var list = new List<KritikSaranDto>();

            using var cmd = new SqlCommand("sia_getDataKritikSaran", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Id", id);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                string komentar;
                try
                {
                    komentar = reader.GetString(reader.GetOrdinal("kma_comment"));
                }
                catch
                {
                    komentar = reader.GetString(0);
                }

                if (string.IsNullOrWhiteSpace(komentar)) komentar = "Tidak ada komentar.";

                list.Add(new KritikSaranDto { Komentar = komentar });
            }
            return list;
        }


        public async Task<KuesionerDetailDto?> GetDetailForUserAsync(int kuesionerId, string username)
        {
            await using var conn = new SqlConnection(_conn);
            await conn.OpenAsync();

            bool sudahDiisi = await CheckIfSubmittedAsync(conn, kuesionerId, username);

            var detail = await FetchKuesionerDetailHeaderAsync(conn, kuesionerId, sudahDiisi);
            if (detail == null) return null;

            detail.PertanyaanList = await FetchPertanyaanListAsync(conn, kuesionerId, username, sudahDiisi);

            if (sudahDiisi)
            {
                detail.KritikSaran = await FetchKritikSaranSingleAsync(conn, kuesionerId, username);
            }

            return detail;
        }

        private static async Task<bool> CheckIfSubmittedAsync(SqlConnection conn, int kuesionerId, string username)
        {
            using var cmd = new SqlCommand("sia_checkSubmitKuesioner", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue(Mahasiswa, username);
            cmd.Parameters.AddWithValue(Kuesioner, kuesionerId);

            using var reader = await cmd.ExecuteReaderAsync();
            return reader.HasRows;
        }

        private static async Task<KuesionerDetailDto?> FetchKuesionerDetailHeaderAsync(SqlConnection conn, int kuesionerId, bool sudahDiisi)
        {
            using var cmd = new SqlCommand("sia_detailKuesioner", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Id", kuesionerId);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new KuesionerDetailDto
                {
                    Id = kuesionerId,
                    NamaKuesioner = reader.GetString(0),
                    SkalaMax = reader.GetInt32(2),
                    SkalaDefinisi = reader.GetString(3),
                    SudahDiisi = sudahDiisi
                };
            }
            return null;
        }

        private static async Task<List<PertanyaanJawabanDto>> FetchPertanyaanListAsync(SqlConnection conn, int kuesionerId, string username, bool sudahDiisi)
        {
            var list = new List<PertanyaanJawabanDto>();
            string spName = sudahDiisi ? "sia_lihatKuesionerDetail" : "sia_getDataPertanyaanIsiKue";

            using var cmd = new SqlCommand(spName, conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue(Kuesioner, kuesionerId);
            if (sudahDiisi)
            {
                cmd.Parameters.AddWithValue(Mahasiswa, username);
            }

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var isHeaderVal = reader.GetValue(reader.GetOrdinal("tkd_isheader"));
                bool isHeader = isHeaderVal switch
                {
                    bool b => b,
                    int i => i == 1,
                    string s => s == "1" || s.Equals("ya", StringComparison.OrdinalIgnoreCase),
                    _ => false
                };

                string? jawaban = null;
                if (sudahDiisi)
                {
                    jawaban = await ReadJawabanAsync(reader);
                }

                list.Add(new PertanyaanJawabanDto
                {
                    Id = reader.GetInt32(reader.GetOrdinal("tkd_id")),
                    Pertanyaan = reader.GetString(reader.GetOrdinal("tkd_pertanyaan")),
                    IsHeader = isHeader,
                    Jenis = reader.GetString(reader.GetOrdinal("tkd_jenis")),
                    Jawaban = jawaban
                });
            }
            return list;
        }

        private static async Task<string?> ReadJawabanAsync(SqlDataReader reader)
        {
            int jawabanIdx;
            try { jawabanIdx = reader.GetOrdinal("kmd_skala"); } catch { jawabanIdx = -1; }

            if (jawabanIdx != -1 && !await reader.IsDBNullAsync(jawabanIdx))
            {
                return reader.GetValue(jawabanIdx).ToString();
            }
            return null;
        }

        private static async Task<string?> FetchKritikSaranSingleAsync(SqlConnection conn, int kuesionerId, string username)
        {
            using var cmd = new SqlCommand("sia_lihatKuesioner", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue(Kuesioner, kuesionerId);
            cmd.Parameters.AddWithValue(Mahasiswa, username);

            var res = await cmd.ExecuteScalarAsync();
            return res?.ToString();
        }

        public async Task<List<Dictionary<string, object>>> GetExportDataAsync(GetExportDataRequest request)
        {
            var result = new List<Dictionary<string, object>>();

            using (var conn = new SqlConnection(_conn))
            {
                await conn.OpenAsync();

                using var cmd = new SqlCommand();
                cmd.Connection = conn;
                bool isConfigured = await ConfigureExportCommandAsync(cmd, conn, request);
                if (!isConfigured) return result;
                if (conn.State != ConnectionState.Open) await conn.OpenAsync();

                using var reader = await cmd.ExecuteReaderAsync();
                result = await ReadExportDataAsync(reader);
            }
            return result;
        }

        private static async Task<bool> ConfigureExportCommandAsync(SqlCommand cmd, SqlConnection conn, GetExportDataRequest request)
        {
            cmd.CommandType = CommandType.StoredProcedure;

            if (request.JenisExport.Equals(JenisDosen, StringComparison.OrdinalIgnoreCase))
            {
                cmd.CommandText = "sia_exportKuesionerDosen";
                cmd.Parameters.AddWithValue("@TahunAkademik", request.TahunAkademik);
                cmd.Parameters.AddWithValue("@Semester", request.Semester);
                cmd.Parameters.AddWithValue("@Role", request.RoleId);
                cmd.Parameters.AddWithValue("@Username", request.Username);
                return true;
            }

            if (request.JenisExport.Contains(JenisFasilitas, StringComparison.OrdinalIgnoreCase) || request.JenisExport.Equals("2"))
            {
                string idJenis = await ResolveFasilitasIdAsync(conn, request.JenisExport);

                cmd.CommandText = "sia_exportKuesionerFasilitas";
                cmd.Parameters.AddWithValue("@TahunAkademik", request.TahunAkademik);
                cmd.Parameters.AddWithValue("@Semester", request.Semester);
                cmd.Parameters.AddWithValue("@JenisKue", idJenis);
                return true;
            }

            if (request.JenisExport.Equals(JenisResponden, StringComparison.OrdinalIgnoreCase))
            {
                cmd.CommandText = "sia_exportKuesionerResponden";
                cmd.Parameters.AddWithValue("@TahunAkademik", request.TahunAkademik);
                cmd.Parameters.AddWithValue("@Semester", request.Semester);
                return true;
            }

            return false;
        }

        private static async Task<string> ResolveFasilitasIdAsync(SqlConnection conn, string jenisExport)
        {
            if (int.TryParse(jenisExport, out _))
            {
                return jenisExport;
            }

            if (conn.State != ConnectionState.Open) await conn.OpenAsync();

            using var cmdCari = new SqlCommand("sia_getJenisKuesionerIdByNama", conn);
            cmdCari.CommandType = CommandType.StoredProcedure;

            cmdCari.Parameters.AddWithValue("@NamaJenis", jenisExport);

            var res = await cmdCari.ExecuteScalarAsync();

            return res?.ToString() ?? "2";
        }

        private static async Task<List<Dictionary<string, object>>> ReadExportDataAsync(SqlDataReader reader)
        {
            var list = new List<Dictionary<string, object>>();
            while (await reader.ReadAsync())
            {
                var row = new Dictionary<string, object>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    row[reader.GetName(i)] = reader.GetValue(i) == DBNull.Value ? null! : reader.GetValue(i);
                }
                list.Add(row);
            }
            return list;
        }

    }
}