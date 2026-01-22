using astratech_apps_backend.DTOs.JamMinusPlus;
using astratech_apps_backend.Models;
using astratech_apps_backend.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace astratech_apps_backend.Repositories.Implementations
{
    public class JamMinusPlusRepository : IJamMinusPlusRepository
    {
        private readonly string _conn;

        public JamMinusPlusRepository(IConfiguration config)
        {
            _conn = PolmanAstraLibrary.PolmanAstraLibrary.Decrypt(
                config.GetConnectionString("DefaultConnection")!,
                Environment.GetEnvironmentVariable("DECRYPT_KEY_CONNECTION_STRING")
            );
        }

        public async Task<(IEnumerable<JamMinusPlus> Data, int Total)> GetAllAsync(GetAllJamMinusPlusRequest dto)
        {
            var list = new List<JamMinusPlus>();
            int totalData = 0;

            using var conn = new SqlConnection(_conn);
            using var cmd = new SqlCommand("sia_getDataJamMinusPlus", conn)
            {
                CommandType = CommandType.StoredProcedure,
                CommandTimeout = 300 
            };

            
            cmd.Parameters.AddWithValue("@Username", dto.Username ?? "");
            cmd.Parameters.AddWithValue("@Keyword", dto.Keyword ?? "");
            cmd.Parameters.AddWithValue("@SortOrder", dto.Sort ?? "mhs_id asc");
            cmd.Parameters.AddWithValue("@KonId", dto.KonId ?? "");
            cmd.Parameters.AddWithValue("@TahunAjaran", dto.TahunAkademik ?? "");
            cmd.Parameters.AddWithValue("@Semester", dto.Semester ?? "");
            cmd.Parameters.AddWithValue("@KelasId", dto.KelasId ?? "");
            cmd.Parameters.AddWithValue("@RoleId", dto.RoleID ?? "");

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();


            while (await reader.ReadAsync())
            {
                list.Add(new JamMinusPlus
                {
                    MahasiswaId = reader.GetString(reader.GetOrdinal("mhs_id")),
                    Nama = reader.GetString(reader.GetOrdinal("mhs_nama")),
                    Prodi = reader.GetString(reader.GetOrdinal("kon_singkatan")),
                    Kelas = reader.GetString(reader.GetOrdinal("kel_id")),
                    TahunAkademik = reader.GetString(reader.GetOrdinal("tahun_ajaran")),
                    Semester = reader.GetString(reader.GetOrdinal("semester")),
                 
                    SisaKompensasi = reader.IsDBNull(reader.GetOrdinal("SisaKompensasi"))
                        ? 0
                        : Convert.ToDecimal(reader["SisaKompensasi"]),
                    SisaMurni = reader.IsDBNull(reader.GetOrdinal("SisaMurni"))
                        ? 0
                        : Convert.ToDecimal(reader["SisaMurni"])
                });
            }

            totalData = list.Count;

            var paged = list
                .Skip((dto.Page - 1) * dto.PageSize)
                .Take(dto.PageSize)
                .ToList();

            return (paged, totalData);
        }


        public async Task<List<JamMinusPlusPeriodeDto>> GetDetailPeriodeAsync(string mahasiswaId)
        {
            var history = new List<JamMinusPlusDetailDto>();

            using var conn = new SqlConnection(_conn);
            using var cmd = new SqlCommand("sia_detailJamMinusPlus", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@MahasiswaId", mahasiswaId);
            cmd.Parameters.AddWithValue("@KonId", "");
            cmd.Parameters.AddWithValue("@ProdiId", "");
            cmd.Parameters.AddWithValue("@SemesterId", "");
            cmd.Parameters.AddWithValue("@TahunajaranId", "");
            cmd.Parameters.AddWithValue("@Jamplus1", "");
            cmd.Parameters.AddWithValue("@Jamplus2", "");
            cmd.Parameters.AddWithValue("@Jamminus1", "");
            cmd.Parameters.AddWithValue("@Jamminus2", "");
            cmd.Parameters.AddWithValue("@StatusAktif", "");
            cmd.Parameters.AddWithValue("@Tanggal", "");
            cmd.Parameters.AddWithValue("@Tglpelaksanaan", "");
            cmd.Parameters.AddWithValue("@Deskripsi", "");
            cmd.Parameters.AddWithValue("@Jenis", "");
            cmd.Parameters.AddWithValue("@MhsNama", "");
            cmd.Parameters.AddWithValue("@ProNama", "");
            cmd.Parameters.AddWithValue("@KelId", "");
            cmd.Parameters.AddWithValue("@ImpAlasan", "");
            cmd.Parameters.AddWithValue("@ImpStatus", "");
            cmd.Parameters.AddWithValue("@ImpJenis", "");
            cmd.Parameters.AddWithValue("@AmaKeterangan", "");
            cmd.Parameters.AddWithValue("@AmaMenitTerlambat", "");
            cmd.Parameters.AddWithValue("@AmaStatus", "");
            cmd.Parameters.AddWithValue("@RpsId", "");
            cmd.Parameters.AddWithValue("@RdePertemuan", "");
            cmd.Parameters.AddWithValue("@RdeTanggalPlan", "");
            cmd.Parameters.AddWithValue("@MkuNama", "");
            cmd.Parameters.AddWithValue("@JdeWaktuAwal", "");
            cmd.Parameters.AddWithValue("@JdeWaktuAkhir", "");
            cmd.Parameters.AddWithValue("@JdeTanggal", "");
            cmd.Parameters.AddWithValue("@JadTahunAjaran", "");
            cmd.Parameters.AddWithValue("@JadSemester", "");
            cmd.Parameters.AddWithValue("@RdeId", "");
            cmd.Parameters.AddWithValue("@JplId", "");
            cmd.Parameters.AddWithValue("@JplJenis", "");
            cmd.Parameters.AddWithValue("@JplDeskripsi", "");
            cmd.Parameters.AddWithValue("@JplJumlah", "");
            cmd.Parameters.AddWithValue("@JplTglpelaksanaan", "");
            cmd.Parameters.AddWithValue("@JplTahunAjaran", "");
            cmd.Parameters.AddWithValue("@JplSemester", "");
            cmd.Parameters.AddWithValue("@JplStatus", "");

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var tahunAkademik = reader["tahunajaran"]?.ToString() ?? "";

                if (string.IsNullOrWhiteSpace(tahunAkademik))
                    continue;

                history.Add(new JamMinusPlusDetailDto
                {
                    TahunAkademik = tahunAkademik,
                    Tanggal = reader["tanggal"]?.ToString() ?? "",
                    Keterangan = reader["deskripsi"]?.ToString() ?? "",
                    KompensasiMinus = reader.IsDBNull(reader.GetOrdinal("jamminus1")) ? 0 : Convert.ToDecimal(reader["jamminus1"]),
                    KompensasiPlus = reader.IsDBNull(reader.GetOrdinal("jamplus1")) ? 0 : Convert.ToDecimal(reader["jamplus1"]),
                    MurniMinus = reader.IsDBNull(reader.GetOrdinal("jamminus2")) ? 0 : Convert.ToDecimal(reader["jamminus2"]),
                    MurniPlus = reader.IsDBNull(reader.GetOrdinal("jamplus2")) ? 0 : Convert.ToDecimal(reader["jamplus2"])
                });
            }

            return history
                .Where(detail => !string.IsNullOrWhiteSpace(detail.TahunAkademik))
                .GroupBy(detail => detail.TahunAkademik)
                .Select(group => new JamMinusPlusPeriodeDto
                {
                    TahunAkademik = group.Key,
                    History = group.ToList(),
                    Summary = HitungSummary(group.ToList())
                })
                .OrderByDescending(period => period.TahunAkademik)
                .ToList();
        }


        private JamMinusPlusSummaryDto HitungSummary(List<JamMinusPlusDetailDto> history)
        {
            var subTotalKompensasi = history.Sum(detail => detail.KompensasiPlus - detail.KompensasiMinus);
            var subTotalMurni = history.Sum(detail => detail.MurniPlus - detail.MurniMinus);

            return new JamMinusPlusSummaryDto
            {
                SubTotalKompensasi = subTotalKompensasi,
                SubTotalMurni = subTotalMurni,
                Total = subTotalKompensasi + subTotalMurni
            };
        }


        public async Task<List<KeyValuePair<string, string>>> GetListKonsentrasiAsync()
        {
            var list = new List<KeyValuePair<string, string>>();

            using var conn = new SqlConnection(_conn);
            using var cmd = new SqlCommand("sia_getListKonsentrasiForJamMinusPlus", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(new KeyValuePair<string, string>(
                    reader["kon_id"].ToString()!,
                    reader["kon_nama"].ToString()!
                ));
            }
            return list;
        }
    }
}
