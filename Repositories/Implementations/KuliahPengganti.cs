using astratech_apps_backend.DTOs.KuliahPengganti;
using astratech_apps_backend.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace astratech_apps_backend.Repositories.Implementations
{
    public class KuliahPenggantiRepository : IKuliahPenggantiRepository
    {
        private readonly string _conn;
        
        
        private const string ParamIdRuang = "@IdRuang";
        private const string ParamTanggal = "@Tanggal";
        private const string ParamSemester = "@Semester";
        private const string ParamIdKuliahPengganti = "@IdKuliahPengganti";
        private const string ParamModifBy = "@ModifBy";
        private const string ParamTahunAjaran = "@TahunAjaran";
        private const string ParamWaktuMulai = "@WaktuAwal";
        private const string ParamWaktuAkhir = "@WaktuAkhir";
        private const string ParamIdMataKuliah = "@IdMataKuliah";
        private const string ParamIdGrup = "@IdGrup";
        private const string ParamUsername = "@Username";
        
        
        private const string ColKpeId = "kpe_id";
        private const string ColKonSingkatan = "kon_singkatan";
        private const string ColJadTahunAjaran = "jad_tahun_ajaran";
        private const string ColJadSemester = "jad_semester";
        private const string ColMkuNama = "mku_nama";
        private const string ColKodeDosen = "kode_dosen";
        private const string ColRdePertemuan = "rde_pertemuan";
        private const string ColJadwalRencana = "jadwal_rencana";
        private const string ColJadwalPengganti = "jadwal_pengganti";
        private const string ColRuaNama = "rua_nama";
        private const string ColKpeStatus = "kpe_status";
        private const string ColKpeTanggal = "kpe_tanggal";
        


        private const string COL_KELAS_ID = "kel_id";



        private const string COL_MK_ID = "mku_id";
        private const string COL_MK_NAMA = "mku_nama";

        private const string PARAM_KONSENTRASI = "@IdKonsentrasi";
        private const string PARAM_KELAS = "@IdKelas";
        private const string PARAM_NIM = "@NimMahasiswa";
        private const string PARAM_USERNAME_DOSEN = "@UsernameDosen";
        private const string PARAM_SECTION = "@IdSection";


        private const string COL_KON_ID = "kon_id";
        private const string COL_KON_NAMA = "kon_nama";

        public KuliahPenggantiRepository(IConfiguration config)
        {
            _conn = PolmanAstraLibrary.PolmanAstraLibrary.Decrypt(config.GetConnectionString("DefaultConnection")!,Environment.GetEnvironmentVariable("DECRYPT_KEY_CONNECTION_STRING"));
        }

        public async Task<bool> CreatePengajuanKPAsync(CreatePengajuanKPDto dto)
        {
          
                using (SqlConnection conn = new SqlConnection(_conn))
                {
                    await conn.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("sia_createPengajuanKP", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@IdPertemuan", dto.PertemuanId);
                        cmd.Parameters.AddWithValue("@IdDosen", "");        
                        cmd.Parameters.AddWithValue("@IdMahasiswa", "");        
                        cmd.Parameters.AddWithValue(ParamIdRuang, dto.RuanganId);
                        cmd.Parameters.AddWithValue(ParamTanggal, dto.TanggalPengajuan);
                        cmd.Parameters.AddWithValue("@WaktuAwal", dto.JamAwal);
                        cmd.Parameters.AddWithValue(ParamUsername, dto.UsernameKaryawan);
                        cmd.Parameters.AddWithValue(ParamWaktuAkhir, dto.JamAkhir);
                        cmd.Parameters.AddWithValue("@Alasan", dto.AlasanPengajuan);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
        }


        public async Task<bool> EditPengajuanKPAsync(EditPengajuanKPDto dto)
        {
    
                using (SqlConnection conn = new SqlConnection(_conn))
                {
                    await conn.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("sia_editPengajuanKP", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue(ParamIdKuliahPengganti, dto.KuliahPenggantiId);
                        cmd.Parameters.AddWithValue("@IdPertemuan", dto.PertemuanId);
                        cmd.Parameters.AddWithValue(ParamIdRuang, dto.RuanganId);
                        cmd.Parameters.AddWithValue(ParamTanggal, dto.TanggalPengajuan);
                        cmd.Parameters.AddWithValue(ParamWaktuMulai, dto.JamAwal);
                        cmd.Parameters.AddWithValue(ParamWaktuAkhir, dto.JamAkhir);
                        cmd.Parameters.AddWithValue("@Alasan", dto.AlasanPengajuan ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue(ParamModifBy, dto.ModifiedBy);  

                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;

        }


        public async Task<bool> ApprovePengajuanKPAsync(ApprovePengajuanKPDto dto)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_conn))
                {
                    await conn.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("sia_approvePengajuanKP", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue(ParamIdKuliahPengganti, dto.KuliahPenggantiId ?? "");
                        cmd.Parameters.AddWithValue("@Approver", dto.Approver ?? "");
                        cmd.Parameters.AddWithValue("@Role", dto.Role ?? "");
                        cmd.Parameters.AddWithValue("@Status", dto.Status ?? "");
                        cmd.Parameters.AddWithValue(ParamIdRuang, dto.RuanganId ?? "");
                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                return true;
            }
            catch 
            {
               
                return false;
            }
        }

        public async Task<bool> SentPengajuanKPAsync(SentPengajuanKPDto dto)
        {
            try
            {
                using var conn = new SqlConnection(_conn);
                await conn.OpenAsync();

                using var cmd = new SqlCommand("sia_sentPengajuanKP", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue(ParamIdKuliahPengganti, dto.KuliahPenggantiId);
                cmd.Parameters.AddWithValue(ParamModifBy, dto.ModifiedBy);

                await cmd.ExecuteNonQueryAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task DeletePengajuanKPAsync(string KuliahPenggantiId, string ModifiedBy)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_deletePengajuanKP", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue(ParamIdKuliahPengganti, KuliahPenggantiId);
            cmd.Parameters.AddWithValue(ParamModifBy, ModifiedBy);

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<bool> RejectPengajuanKPAsync(RejectPengajuanKPDto dto)
        {
            try
            {
                using var conn = new SqlConnection(_conn);
                await conn.OpenAsync();

                using var cmd = new SqlCommand("sia_rejectPengajuanKP", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue(ParamIdKuliahPengganti, dto.KuliahPenggantiId);
                cmd.Parameters.AddWithValue("@Alasan", dto.AlasanTolak);
                cmd.Parameters.AddWithValue(ParamModifBy, dto.ModifiedBy);

                await cmd.ExecuteNonQueryAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> CancelPengajuanKPAsync(CancelPengajuanKPDto dto)
        {
            try
            {
                using var conn = new SqlConnection(_conn);
                await conn.OpenAsync();

                using var cmd = new SqlCommand("sia_cancelKuliahPengganti", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue(ParamIdKuliahPengganti, dto.KuliahPenggantiId);
                cmd.Parameters.AddWithValue(ParamModifBy, dto.ModifiedBy);

                await cmd.ExecuteNonQueryAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<IEnumerable<GetDataKP>> GetDataKPAsync(GetKPRequest request)
        {
            var results = new List<GetDataKP>();

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getDataKP", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            
            cmd.Parameters.AddWithValue("@Pencarian", request.SearchText ?? "");
            cmd.Parameters.AddWithValue(PARAM_KONSENTRASI, request.KonsentrasiId ?? "");
            cmd.Parameters.AddWithValue(ParamTahunAjaran, request.TahunAjaran ?? "");
            cmd.Parameters.AddWithValue(ParamSemester, request.Semester ?? "");
            cmd.Parameters.AddWithValue(ParamIdMataKuliah, request.MataKuliahId ?? "");
            cmd.Parameters.AddWithValue("@FilterStatusProdi", request.Status ?? "");
            cmd.Parameters.AddWithValue("@KodeProdi", request.ProdiCode ?? "");
            cmd.Parameters.AddWithValue("@SortBy", request.SortBy ?? "kpe_tanggal desc");

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                results.Add(new GetDataKP
                {
                    KuliahPenggantiId = reader[ColKpeId]?.ToString() ?? "",
                    KonsentrasiSingkatan = reader[ColKonSingkatan]?.ToString() ?? "",
                    TahunAjaran = reader[ColJadTahunAjaran]?.ToString() ?? "",
                    Semester = reader[ColJadSemester]?.ToString() ?? "",
                    NamaMataKuliah = reader[ColMkuNama]?.ToString() ?? "",
                    DosenKode = reader[ColKodeDosen]?.ToString() ?? "",
                    JadwalPertemuan = reader[ColRdePertemuan]?.ToString() ?? "",
                    JadwalRencana = reader[ColJadwalRencana]?.ToString() ?? "",
                    JadwalPengganti = reader[ColJadwalPengganti]?.ToString() ?? "",
                    NamaRuangan = reader[ColRuaNama]?.ToString() ?? "",
                    Status = reader[ColKpeStatus]?.ToString() ?? "",
                    KonsentrasiId = reader[COL_KON_ID ]?.ToString() ?? "",
                });
            }
            return results;
        }

        public async Task<IEnumerable<GetDataKPRiwayat>> GetDataKPRiwayatAsync(GetKPRiwayatRequest request)
        {
            var results = new List<GetDataKPRiwayat>();


            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getDataKPRiwayat", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            
            cmd.Parameters.AddWithValue("@SearchKeyword", request.SearchText ?? "");
            cmd.Parameters.AddWithValue("@SortColumn", request.SortBy ?? "kpe_tanggal DESC");
            cmd.Parameters.AddWithValue("@KonsentrasiId", request.KonsentrasiId ?? "");
            cmd.Parameters.AddWithValue(ParamTahunAjaran, request.TahunAjaran ?? "");
            cmd.Parameters.AddWithValue(ParamSemester, request.Semester ?? "");
            cmd.Parameters.AddWithValue("@KodeProdi", request.ProdiCode ?? "");
            cmd.Parameters.AddWithValue("@RoleId", request.RoleCode ?? "");

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                results.Add(new GetDataKPRiwayat
                {
                    KuliahPenggantiId = reader[ColKpeId]?.ToString() ?? "",
                    KonsentrasiSingkatan = reader[ColKonSingkatan]?.ToString() ?? "",
                    TahunAjaran = reader[ColJadTahunAjaran]?.ToString() ?? "",
                    Semester = reader[ColJadSemester]?.ToString() ?? "",
                    NamaMataKuliah = reader[ColMkuNama]?.ToString() ?? "",
                    DosenKode = reader[ColKodeDosen]?.ToString() ?? "",
                    JadwalPertemuan = reader[ColRdePertemuan]?.ToString() ?? "",
                    JadwalRencana = reader[ColJadwalRencana]?.ToString() ?? "",
                    JadwalPengganti = reader[ColJadwalPengganti]?.ToString() ?? "",
                    NamaRuangan = reader[ColRuaNama]?.ToString() ?? "",
                    Status = reader[ColKpeStatus]?.ToString() ?? "",
                    TanggalPengajuan = reader[ColKpeTanggal]?.ToString() ?? "",
                    KonsentrasiId = reader[COL_KON_ID ]?.ToString() ?? ""
                });
            }

            return results;
        }

        public async Task<IEnumerable<GetKPByDosenResponseDto>> GetDataKPByDosenAsync(string KaryawanUsername, GetKPByDosenRequestDto request)
        {
            var results = new List<GetKPByDosenResponseDto>();

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getDataKPByDosen", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            
            cmd.Parameters.AddWithValue(ParamUsername, KaryawanUsername ?? "");
            cmd.Parameters.AddWithValue("@Search", request.SearchText ?? "");
            cmd.Parameters.AddWithValue("@OrderBy", request.OrderBy ?? "kpe_tanggal DESC");
            cmd.Parameters.AddWithValue(PARAM_KONSENTRASI, request.KonsentrasiId ?? "");
            cmd.Parameters.AddWithValue(ParamTahunAjaran, request.TahunAjaran ?? "");
            cmd.Parameters.AddWithValue(ParamSemester, request.Semester ?? "");
            cmd.Parameters.AddWithValue(ParamIdMataKuliah, request.MataKuliahId ?? "");

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                results.Add(new GetKPByDosenResponseDto
                {
                    KuliahPenggantiId = reader[ColKpeId]?.ToString() ?? "",
                    KonsentrasiSingkatan = reader[ColKonSingkatan]?.ToString() ?? "",
                    TahunAjaran = reader[ColJadTahunAjaran]?.ToString() ?? "",
                    Semester = reader[ColJadSemester]?.ToString() ?? "",
                    NamaMataKuliah = reader[ColMkuNama]?.ToString() ?? "",
                    DosenKode = reader[ColKodeDosen]?.ToString() ?? "",
                    JadwalPertemuan = reader[ColRdePertemuan]?.ToString() ?? "",
                    JadwalRencana = reader[ColJadwalRencana]?.ToString() ?? "",
                    JadwalPengganti = reader[ColJadwalPengganti]?.ToString() ?? "",
                    NamaRuangan = reader[ColRuaNama]?.ToString() ?? "",
                    status = reader[ColKpeStatus]?.ToString() ?? "",
                    TanggalPengajuan = reader[ColKpeTanggal]?.ToString() ?? ""
                });
            }

            return results;
        }

        public async Task<IEnumerable<GetKPByNimResponseDto>> GetDataKPByNIMAsync(GetKPByNimRequestDto request)
        {
            var results = new List<GetKPByNimResponseDto>();

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getDataKPByNIM", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            
            cmd.Parameters.AddWithValue("@IdMahasiswa", request.NIM ?? "");
            cmd.Parameters.AddWithValue("@SearchKeyword", request.SearchText ?? "");
            cmd.Parameters.AddWithValue("@OrderBy", request.OrderBy ?? "kpe_tanggal DESC");
            cmd.Parameters.AddWithValue(PARAM_KONSENTRASI, request.KonsentrasiId ?? "");
            cmd.Parameters.AddWithValue(ParamTahunAjaran, request.TahunAjaran ?? "");
            cmd.Parameters.AddWithValue(ParamSemester, request.Semester ?? "");
            cmd.Parameters.AddWithValue(ParamIdMataKuliah, request.MataKuliahId ?? "");


            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                results.Add(new GetKPByNimResponseDto
                {
                    KuliahPenggantiId = reader[ColKpeId]?.ToString() ?? "",
                    KonsentrasiSingkatan = reader[ColKonSingkatan]?.ToString() ?? "",
                    TahunAjaran = reader[ColJadTahunAjaran]?.ToString() ?? "",
                    Semester = reader[ColJadSemester]?.ToString() ?? "",
                    NamaMataKuliah = reader[ColMkuNama]?.ToString() ?? "",
                    DosenKode = reader[ColKodeDosen]?.ToString() ?? "",
                    JadwalPertemuan = reader[ColRdePertemuan]?.ToString() ?? "",
                    JadwalRencana = reader[ColJadwalRencana]?.ToString() ?? "",
                    JadwalPengganti = reader[ColJadwalPengganti]?.ToString() ?? "",
                    NamaRuangan = reader[ColRuaNama]?.ToString() ?? "",
                    Status = reader[ColKpeStatus]?.ToString() ?? "",
                    TanggalPengajuan = reader[ColKpeTanggal]?.ToString() ?? ""
                });
            }

            return results;
        }

        public async Task<DetailKPDto> GetDetailKPAsync(string Id)
        {
            try
            {
                using var conn = new SqlConnection(_conn);
                await conn.OpenAsync();

                using var cmd = new SqlCommand("sia_detailKP", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Id", Id);

                using var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new DetailKPDto
                    {
                        KuliahPenggantiId = reader["kpe_id"].ToString()!,
                        KonsentrasiId = reader[COL_KON_ID ].ToString()!,
                        TahunAjaran = reader[ColJadTahunAjaran].ToString()!,
                        Semester = reader[ColJadSemester].ToString()!,
                        MataKuliahId = reader["mku_id"].ToString()!,
                        NamaMataKuliah = reader[ColMkuNama].ToString()!,
                        PertemuanId = reader["rde_id"].ToString()!,
                        NamaDosen = reader["dosen1"]?.ToString()!,
                        NamaDosen2 = reader["dosen2"]?.ToString()!,
                        NamaDosen3 = reader["dosen3"]?.ToString()!,
                        JadwalRencana = reader[ColJadwalRencana]?.ToString()!,
                        TanggalPengajuan = reader[ColKpeTanggal].ToString()!,
                        JamAwal = reader["kpe_waktu_awal"].ToString()!,
                        JamAkhir = reader["kpe_waktu_akhir"].ToString()!,
                        RuanganId = reader["rua_id"].ToString()!,
                        Status = reader[ColKpeStatus].ToString()!,
                        JadwalPengganti = reader[ColJadwalPengganti].ToString()!,
                        NamaRuangan = reader[ColRuaNama].ToString()!,
                        GrupNama = reader["grup"].ToString()!,
                        SectionNama = reader["section"].ToString()!,
                        GrupId = reader["gru_id"].ToString()!,
                        SectionId = reader["sec_id"].ToString()!,
                        NamaKonsentrasi = reader["kon_nama"].ToString()!,
                        KelasId = reader["kel_id"].ToString()!,
                        AlasanPengajuan = reader["alasan"].ToString()!,
                        AlasanTolak = reader["tolak"].ToString()!
                    };
                }

                return null!;
            }
            catch 
            {
            
                return null!;
            }
        }

        public async Task<DetailPertemuanKPDto> GetDetailPertemuanKPAsync(string Id)
        {
            try
            {
                using var conn = new SqlConnection(_conn);
                await conn.OpenAsync();

                using var cmd = new SqlCommand("sia_detailPertemuanKP", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Id", Id);

                using var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new DetailPertemuanKPDto
                    {
                        PertemuanId = reader["rde_id"].ToString()!,
                        JadwalPertemuan = reader["jadwal"].ToString()!,
                        MateriPertemuan = reader["rde_materi"].ToString()!,
                        JamAwal = Convert.ToInt32(reader["jamawal"]),
                        JamAkhir = Convert.ToInt32(reader["jamakhir"]),
                        MenitAwal = Convert.ToInt32(reader["menitawal"]),
                        MenitAkhir = Convert.ToInt32(reader["menitakhir"])
                    };
                }

                return null!;
            }
            catch 
            {
            
                return null!;
            }
        }

        public async Task<string?> CheckJadwalBentrokKPAsync(CheckJadwalBentrokKPRequestDto dto)
        {
     
                using var conn = new SqlConnection(_conn);
                await conn.OpenAsync();

                using var cmd = new SqlCommand("sia_checkJadwalBentrokKP", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Mode", dto.ModeBentrok);
                cmd.Parameters.AddWithValue(ParamTanggal, dto.TanggalPengajuan);
                cmd.Parameters.AddWithValue("@WaktuAwal", dto.JamAwal);
                cmd.Parameters.AddWithValue(ParamWaktuAkhir, dto.JamAkhir);
                cmd.Parameters.AddWithValue("@IdDosen", dto.DosenId);
                cmd.Parameters.AddWithValue(ParamIdRuang, dto.RuanganId);
                cmd.Parameters.AddWithValue(ParamIdGrup, dto.GrupId);
                cmd.Parameters.AddWithValue(PARAM_SECTION, dto.SectionId);

                using var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {       
                    return reader[0]?.ToString();
                }
                return null;

        }

        public async Task<IEnumerable<CheckJadwalBentrokResponseDto>>CheckJadwalBentrokAsync(CheckJadwalBentrokRequestDto dto)
        {
            var list = new List<CheckJadwalBentrokResponseDto>();

            using var conn = new SqlConnection(_conn);
            await conn.OpenAsync();

            using var cmd = new SqlCommand("sia_checkJadwalBentrok", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Mode", dto.ModeBentrok);
            cmd.Parameters.AddWithValue(ParamTanggal, dto.TanggalPengajuan);
            cmd.Parameters.AddWithValue(ParamWaktuMulai, dto.JamAwal);
            cmd.Parameters.AddWithValue(ParamWaktuAkhir, dto.JamAkhir);
            cmd.Parameters.AddWithValue("@IdDosen", dto.DosenId);
            cmd.Parameters.AddWithValue(ParamIdRuang, dto.RuanganId);
            cmd.Parameters.AddWithValue(ParamIdGrup, dto.GrupId);
            cmd.Parameters.AddWithValue(PARAM_SECTION, dto.SectionId);
            cmd.Parameters.AddWithValue("@DetailJadwal", (object?)dto.JadwalDetailId ?? DBNull.Value);

            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                
                if (reader.FieldCount == 1)
                {
                    list.Add(new CheckJadwalBentrokResponseDto
                    {
                        IsBentrok = false
                    });
                    break;
                }

                list.Add(new CheckJadwalBentrokResponseDto
                {
                    IsBentrok = true,
                    Subjek = reader["subjek"]?.ToString() ?? "",
                    NamaMataKuliah = reader[ColMkuNama]?.ToString(),
                    TanggalPengajuan = reader[ColKpeTanggal]?.ToString(),
                    JamAwal = reader["waktu_awal"]?.ToString(),
                    JamAkhir = reader["waktu_akhir"]?.ToString(),
                    NamaKonsentrasi = reader["kon_nama"]?.ToString()
                });
            }

            return list;
        }



        public async Task<TahunAjaranActiveDto?> GetTahunAjaranAktifAsync()
        {
            TahunAjaranActiveDto? result = null;

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getActiveTahunAkademikByTanggal", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                result = new TahunAjaranActiveDto
                {
                    TahunAjaran = reader["kak_tahun_ajaran"].ToString()!,
                    GanjilGenap = reader["kak_ganjil_genap"].ToString()!,
                    TanggalMulai = Convert.ToDateTime(reader["kak_tgl_from"])
                };
            }

            return result;
        }

        public async Task<IEnumerable<GetTahunSemesterActive>> GetTahunSemesterActiveAsync()
        {
            var results = new List<GetTahunSemesterActive>();

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getActiveTahunAkademikByTanggal", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            await conn.OpenAsync();

            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                results.Add(new GetTahunSemesterActive
                {
                    TahunAjaran = reader["kak_tahun_ajaran"].ToString()!,
                    Semester = reader["kak_ganjil_genap"].ToString()!,
                    TanggalMulai = Convert.ToDateTime(reader["kak_tgl_from"])
                });
            }

            return results;
        }


        public async Task<IEnumerable<GetAllListDosen>> GetAllListDosenAsync()
        {
            var results = new List<GetAllListDosen>();

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getListDosen", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            await conn.OpenAsync();

            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                results.Add(new GetAllListDosen
                {
                    DosenId = reader["dos_id"].ToString()!,
                    NamaDosen = reader["dos_nama"].ToString()!
                });
            }

            return results;
        }
        public async Task<IEnumerable<GetListGrup>> GetListGrupAsync(string KonsentrasiId, string KelasId, string GrupTahunAjaran, string GrupSemester)
        {
            var results = new List<GetListGrup>();

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getListGrupByKonsentrasiKelas", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue(PARAM_KONSENTRASI, KonsentrasiId);
            cmd.Parameters.AddWithValue("@IdKelas", KelasId);
            cmd.Parameters.AddWithValue("@GrupTahunAjaran", GrupTahunAjaran);
            cmd.Parameters.AddWithValue("@GrupSemester", GrupSemester);

            await conn.OpenAsync();

            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                results.Add(new GetListGrup
                {
                    GrupId = reader["gru_id"].ToString()!,
                    GrupNama = reader["gru_nama"].ToString()!,

                });
            }

            return results;
        }
           public async Task<IEnumerable<GetListKelas>> GetListKelasAsync(string KonsentrasiId,string TahunAjaran,string Jenis)
        {
            var results = new List<GetListKelas>();

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getListKelas", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue(PARAM_KONSENTRASI, KonsentrasiId);
            cmd.Parameters.AddWithValue("@TahunAkademik", TahunAjaran);
            cmd.Parameters.AddWithValue("@JenisKelas", Jenis);

            await conn.OpenAsync();

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                results.Add(new GetListKelas
                {
                    KelasId = reader[COL_KELAS_ID]?.ToString() ?? string.Empty
                });
            }

            return results;
        }

        public async Task<IEnumerable<GetKelasByNimDto>> GetKelasByNIMAsync(string KonsentrasiId,string TahunAjaran,string Nim)
        {
            var results = new List<GetKelasByNimDto>();

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getListKelasByNIM", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue(PARAM_KONSENTRASI, KonsentrasiId ?? string.Empty);
            cmd.Parameters.AddWithValue(ParamTahunAjaran, TahunAjaran ?? string.Empty);
            cmd.Parameters.AddWithValue("@Nim", Nim ?? string.Empty);

            await conn.OpenAsync();

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                results.Add(new GetKelasByNimDto
                {
                    KelasId = reader[COL_KELAS_ID]?.ToString() ?? string.Empty,
                    KelasNama = reader["kel_nama"]?.ToString() ?? string.Empty
                });
            }

            return results;
        }

        public async Task<IEnumerable<GetKelasByDosenDto>> GetKelasByDosenAsync(
            string KonsentrasiId,
            string TahunAjaran,
            string Username)
        {
            var results = new List<GetKelasByDosenDto>();

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getListKelasByDosen", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue(PARAM_KONSENTRASI, KonsentrasiId ?? string.Empty);
            cmd.Parameters.AddWithValue(ParamTahunAjaran, TahunAjaran ?? string.Empty);
            cmd.Parameters.AddWithValue(ParamUsername, Username ?? string.Empty);

            await conn.OpenAsync();

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                results.Add(new GetKelasByDosenDto
                {
                    KelasId = reader[COL_KELAS_ID]?.ToString() ?? string.Empty
                });
            }

            return results;
        }
    
    
    
        public async Task<List<GetMataKuliahKPDto>> GetMataKuliahKPAsync(
            string KonsentrasiId,
            string KelasId)
        {
            var list = new List<GetMataKuliahKPDto>();

            await using var con = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getListMataKuliahKP", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue(PARAM_KONSENTRASI, KonsentrasiId);
            cmd.Parameters.AddWithValue(PARAM_KELAS, KelasId);

            await con.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(new GetMataKuliahKPDto
                {
                    MataKuliahId = reader[COL_MK_ID].ToString() ?? "",
                    NamaMataKuliah = reader[COL_MK_NAMA].ToString() ?? ""
                });
            }

            return list;
        }

        public async Task<GetMataKuliahDetailDto> GetMataKuliahDetailAsync(
            string MataKuliahId,
            string KonsentrasiId,
            string TahunAjaran,
            string Semester,
            string GrupId,
            string SectionId)
        {
            var result = new GetMataKuliahDetailDto();

            await using var con = new SqlConnection(_conn);
            await con.OpenAsync();

            await using var cmd = new SqlCommand("sia_detailMatkulKP", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue(ParamIdMataKuliah, MataKuliahId);
            cmd.Parameters.AddWithValue(PARAM_KONSENTRASI, KonsentrasiId);
            cmd.Parameters.AddWithValue(ParamTahunAjaran, TahunAjaran);
            cmd.Parameters.AddWithValue(ParamSemester, Semester);
            cmd.Parameters.AddWithValue(ParamIdGrup, GrupId);
            cmd.Parameters.AddWithValue(PARAM_SECTION, SectionId);

            await using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                result = new GetMataKuliahDetailDto
                {
                    JadwalId   = reader.GetValue(reader.GetOrdinal("jde_id"))?.ToString() ?? "",
                    NamaDosen = reader.GetValue(reader.GetOrdinal("nama"))?.ToString() ?? "",
                    NamaDosen2= reader.GetValue(reader.GetOrdinal("nama2"))?.ToString() ?? "",
                    NamaDosen3= reader.GetValue(reader.GetOrdinal("nama3"))?.ToString() ?? "",

                    DosenId   = reader.GetValue(reader.GetOrdinal("dos_id_1"))?.ToString() ?? "",
                    DosenId2  = reader.GetValue(reader.GetOrdinal("dos_id_2"))?.ToString() ?? "",
                    DosenId3  = reader.GetValue(reader.GetOrdinal("dos_id_3"))?.ToString() ?? "",
                };
            }



            return result;
        }

        public async Task<List<GetMataKuliahByNimDto>> GetMataKuliahByNIMAsync(
            string NimMahasiswa,
            string TahunAjaran,
            string Semester)
        {
            var list = new List<GetMataKuliahByNimDto>();

            await using var con = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getListMataKuliahByNIM", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue(PARAM_NIM, NimMahasiswa);
            cmd.Parameters.AddWithValue(ParamTahunAjaran, TahunAjaran);
            cmd.Parameters.AddWithValue(ParamSemester, Semester);

            await con.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(new GetMataKuliahByNimDto
                {
                    MataKuliahId = reader[COL_MK_ID].ToString() ?? "",
                    NamaMataKuliah = reader[COL_MK_NAMA].ToString() ?? ""
                });
            }

            return list;
        }

        public async Task<List<GetMataKuliahByDosenDto>> GetMataKuliahByDosenAsync(
            string UsernameDosen,
            string KelasId,
            string TahunAjaran,
            string Semester,
            string SectionId)
        {
            var list = new List<GetMataKuliahByDosenDto>();

            await using var con = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getListMataKuliahByDosen", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue(PARAM_USERNAME_DOSEN, UsernameDosen);
            cmd.Parameters.AddWithValue(PARAM_KELAS, KelasId);
            cmd.Parameters.AddWithValue(ParamTahunAjaran, TahunAjaran);
            cmd.Parameters.AddWithValue(ParamSemester, Semester);
            cmd.Parameters.AddWithValue(PARAM_SECTION, SectionId ?? "%%");

            await con.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(new GetMataKuliahByDosenDto
                {
                    MataKuliahId = reader[COL_MK_ID].ToString() ?? "",
                    NamaMataKuliah = reader[COL_MK_NAMA].ToString() ?? ""
                });
            }

            return list;
        }
   

        public async Task<IEnumerable<PertemuanListDto>> GetListPertemuanAsync(
            string KonsentrasiId,
            string TahunAjaran,
            string Semester,
            string MataKuliahId,
            string GrupId,
            string SectionId)
        {
            var result = new List<PertemuanListDto>();

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getListPertemuan", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue(PARAM_KONSENTRASI, KonsentrasiId);
            cmd.Parameters.AddWithValue(ParamTahunAjaran, TahunAjaran);
            cmd.Parameters.AddWithValue(ParamSemester, Semester);
            cmd.Parameters.AddWithValue(ParamIdMataKuliah, MataKuliahId);
            cmd.Parameters.AddWithValue(ParamIdGrup, GrupId);
            cmd.Parameters.AddWithValue(PARAM_SECTION, SectionId);

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                result.Add(new PertemuanListDto
                {
                    PertemuanId = reader["rde_id"]?.ToString() ?? "",
                    JadwalPertemuan = reader["rde_pertemuan"]?.ToString() ?? "",
                });
            }

            return result;
        }

        public async Task<IEnumerable<GetAllListProdi>> GetAllListProdiAsync()
        {
            var results = new List<GetAllListProdi>();

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getListKonsentrasi", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                results.Add(new GetAllListProdi
                {
                    KonsentrasiId   = reader[COL_KON_ID]?.ToString() ?? "",
                    NamaKonsentrasi = reader[COL_KON_NAMA]?.ToString() ?? ""
                });
            }

            return results;
        }

        public async Task<IEnumerable<GetKonsentrasiByNimDto>> GetKonsentrasiByNIMAsync(string Nim)
        {
            var results = new List<GetKonsentrasiByNimDto>();

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getListKonsentrasiByNIM", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@NIM", Nim ?? "");

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                results.Add(new GetKonsentrasiByNimDto
                {
                    KonsentrasiId   = reader[COL_KON_ID]?.ToString() ?? "",
                    NamaKonsentrasi = reader[COL_KON_NAMA]?.ToString() ?? ""
                });
            }

            return results;
        }
        public async Task<IEnumerable<GetKonsentrasiByDosenDto>> GetKonsentrasiByDosenAsync(string Username)
        {
            var results = new List<GetKonsentrasiByDosenDto>();

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getListKonsentrasiByDosen", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue(ParamUsername, Username ?? "");

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                results.Add(new GetKonsentrasiByDosenDto
                {
                    KonsentrasiId   = reader[COL_KON_ID]?.ToString() ?? "",
                    NamaKonsentrasi = reader[COL_KON_NAMA]?.ToString() ?? ""
                });
            }

            return results;
        }
    
            public async Task<IEnumerable<GetAllListRuangan>> GetAllListRuanganAsync()
        {
            var results = new List<GetAllListRuangan>();

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getListRuanganMahasiswa", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            await conn.OpenAsync();

            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                results.Add(new GetAllListRuangan
                {
                    RuanganId = reader["rua_id"].ToString()!,
                    NamaRuangan = reader["rua_nama"].ToString()!
                });
            }

            return results;
        }
   
        public async Task<IEnumerable<SectionDto>> GetSectionByGrupAsync(string KonsentrasiId, string KelasId, string GrupId, string TahunAjaran, string Semester)
        {
            var result = new List<SectionDto>();

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getListSectionByGrup2", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue(PARAM_KONSENTRASI, KonsentrasiId);
            cmd.Parameters.AddWithValue("@IdKelas", KelasId);
            cmd.Parameters.AddWithValue(ParamIdGrup, GrupId);
            cmd.Parameters.AddWithValue(ParamTahunAjaran, TahunAjaran);
            cmd.Parameters.AddWithValue(ParamSemester, Semester);

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                result.Add(new SectionDto
                {
                    SectionId = reader["sec_id"].ToString() ?? "",
                    SectionNama = reader["sec_nama"].ToString() ?? ""
                });
            }

            return result;
        }



    }
}