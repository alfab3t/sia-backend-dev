using astratech_apps_backend.DTOs.PengubahanNilai;
using astratech_apps_backend.Models;
using astratech_apps_backend.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using System.ComponentModel;
using System.Data;

namespace astratech_apps_backend.Repositories.Implementations;

public class PengubahanNilaiRepository(IConfiguration config) : IPengubahanNilaiRepository
{
    private readonly string _conn = PolmanAstraLibrary.PolmanAstraLibrary.Decrypt(
        config.GetConnectionString("DefaultConnection")!,
        Environment.GetEnvironmentVariable("DECRYPT_KEY_CONNECTION_STRING")!);

    private const string PARAM_TAHUN_AJARAN = "@TahunAjaran";
    private const string PARAM_SEMESTER = "@Semester";
    private const string PARAM_ROLE_ID = "@RoleId";
    private const string PARAM_USERNAME = "@Username";
    private const string ROLE_DOSEN = "ROL73";
    private const string ROLE_PRODI = "ROL71";
    private const string ROLE_KAPROD = "ROL72";
    private const string ROLE_DAAK = "ROL74";
    private const string STATUS_DRAFT = "Draft";
    private const string STATUS_MENUNGGU_PRODI = "Menunggu Approval Prodi";
    private const string STATUS_MENUNGGU_DAAK = "Menunggu Approval DAAK";
    private const string COLUMN_PPN_CREATED_BY = "ppn_created_by";
    private const string ERROR_DATA_NOT_FOUND = "Data tidak ditemukan";


    public async Task<PengubahanNilaiResponse?> GetDataPengubahanNilaiAsync(
    PengubahanNilaiRequestDto dto, string username, string roleId)
    {
        try
        {
            var result = new PengubahanNilaiResponse();
            var listModel = new List<PengubahanNilai>();

            string semesterParam = dto.Semester ?? "";
            string searchTermParam = (dto.SearchTerm ?? "").Trim();

            if (semesterParam.Equals("Ganjil", StringComparison.OrdinalIgnoreCase))
                semesterParam = "1,3,5,7";
            else if (semesterParam.Equals("Genap", StringComparison.OrdinalIgnoreCase))
                semesterParam = "2,4,6,8";

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getDataPengubahanNilai", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@SearchTerm", searchTermParam);
            cmd.Parameters.AddWithValue("@KonsentrasiId", dto.KonsentrasiId ?? "");
            cmd.Parameters.AddWithValue(PARAM_TAHUN_AJARAN, dto.TahunAjaran ?? "");
            cmd.Parameters.AddWithValue(PARAM_SEMESTER, semesterParam);
            cmd.Parameters.AddWithValue(PARAM_ROLE_ID, roleId ?? "");
            cmd.Parameters.AddWithValue("@Status", dto.Status ?? "");
            cmd.Parameters.AddWithValue(PARAM_USERNAME, username ?? "");
            cmd.Parameters.AddWithValue("@Page", dto.Page);
            cmd.Parameters.AddWithValue("@PageSize", dto.PageSize);


            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            int totalRecords = 0;
            while (await reader.ReadAsync())
            {
                if (totalRecords == 0) totalRecords = GetInt(reader, "TotalRecords");
                var model = new PengubahanNilai
                {
                    PermintaanPengubahanNilaiId = GetInt(reader, "ppn_id"),
                    RowNumber = GetInt(reader, "rownum"),
                    KonsentrasiSingkatan = SafeGetString(reader, "kon_singkatan"),
                    KonsentrasiId = SafeGetString(reader, "kon_id"),
                    TahunAjaran = SafeGetString(reader, "kni_tahun_ajaran"),
                    Semester = SafeGetString(reader, "mku_semester"),
                    MataKuliahNama = SafeGetString(reader, "mku_nama"),
                    DosenNama = SafeGetString(reader, "dos_nama"),
                    Status = SafeGetString(reader, "ppn_status").Trim(),
                    KelasId = SafeGetString(reader, "kel_id"),
                    CreatedBy = SafeGetString(reader, "ppn_created_by").Trim(),
                };
                model.OpenEdit = GetBoolean(reader, "ppn_open_edit");

                listModel.Add(model);
            }
           
            result.Data = listModel.Select(item => new PengubahanNilaiDto
            {
                RowNum = (int)item.RowNumber,
                PermintaanPengubahanNilaiId = item.PermintaanPengubahanNilaiId,
                Konsentrasi = item.KonsentrasiSingkatan,
                TahunAjaran = item.TahunAjaran,
                Semester = item.Semester,
                MataKuliah = item.MataKuliahNama,
                Dosen = item.DosenNama,
                Status = item.Status,
                KonsentrasiId = item.KonsentrasiId,
                KelasId = item.KelasId,
                OpenEdit = item.OpenEdit,
                CreatedBy = item.CreatedBy,
                Aksi = DetermineAllowedActions(roleId ?? "", item.Status, item.CreatedBy, username ?? "")
            }).ToList();

            result.TotalRecords = totalRecords;
            result.CurrentPage = dto.Page;
            result.PageSize = dto.PageSize;
            result.TotalPages = totalRecords == 0 ? 0 : (int)Math.Ceiling((double)totalRecords / dto.PageSize);

            return result; 
        }
        catch (Exception ex)
        {
            return new PengubahanNilaiResponse
            {
                ErrorMessage = $"Gagal mendapatkan data: {ex.Message}"
            };
        }
    }
    private static List<string> DetermineAllowedActions(string roleId, string status, string createdBy, string username)
    {
        var actions = new List<string> { "Detail" };

        roleId = (roleId ?? "").Trim();
        status = (status ?? "").Trim();
        createdBy = (createdBy ?? "").Trim();
        username = (username ?? "").Trim();

        bool isOwner = string.Equals(createdBy, username, StringComparison.OrdinalIgnoreCase);

        if (string.Equals(roleId, ROLE_DOSEN, StringComparison.OrdinalIgnoreCase))
        {
            if (isOwner && status.Equals(STATUS_DRAFT, StringComparison.OrdinalIgnoreCase))
            {
                actions.Add("Delete");
                actions.Add("Sent");
            }
        }
        else if (string.Equals(roleId, ROLE_PRODI, StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(roleId, ROLE_KAPROD, StringComparison.OrdinalIgnoreCase))
        {
            if (status.Equals(STATUS_DRAFT, StringComparison.OrdinalIgnoreCase))
            {
                actions.Add("Delete");
                actions.Add("Sent");
            }
            else if (status.Equals(STATUS_MENUNGGU_PRODI, StringComparison.OrdinalIgnoreCase))
            {
                actions.Add("Approve");
                actions.Add("Reject");
            }
        }

        else if (string.Equals(roleId, ROLE_DAAK, StringComparison.OrdinalIgnoreCase)
  && status.Equals(STATUS_MENUNGGU_DAAK, StringComparison.OrdinalIgnoreCase))
        {
            actions.Add("Approve");
            actions.Add("Reject");
        }

        return actions;
    }

    public async Task<BaseResponse?> SendPengubahanNilaiAsync(int id, SendPengubahanNilaiDto dto, string username, string roleId)
    {
        try
        {
            await using var conn = new SqlConnection(_conn);
            await conn.OpenAsync();

            var details = await GetStatusAndOwnerAsync(conn, id);
            if (!details.Found) return new BaseResponse { IsSuccess = false, ErrorMessage = ERROR_DATA_NOT_FOUND };

            string currentStatus = details.Status.Trim();
            string targetType = dto.Tipe;

            if (targetType == "Prodi" && !currentStatus.Equals("Draft", StringComparison.OrdinalIgnoreCase))
            {
                return new BaseResponse
                {
                    IsSuccess = false,
                    ErrorMessage = $"Gagal: Status saat ini '{currentStatus}'. " +
                                  "Pengiriman ke Prodi hanya diperbolehkan untuk status Draft."
                };
            }

            if (targetType == "DAAK" &&
                !currentStatus.Equals("Disetujui", StringComparison.OrdinalIgnoreCase) &&
                !currentStatus.Equals(STATUS_MENUNGGU_PRODI, StringComparison.OrdinalIgnoreCase))
            {
                return new BaseResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "Gagal: Dokumen belum disetujui Prodi."
                };
            }


            await using var cmdSend = new SqlCommand("sia_sentPengubahanNilai", conn) { CommandType = CommandType.StoredProcedure };
            cmdSend.Parameters.AddWithValue("@TargetType", targetType);
            cmdSend.Parameters.AddWithValue("@SentId", id.ToString());
            cmdSend.Parameters.AddWithValue(PARAM_USERNAME, username);
           

            await cmdSend.ExecuteNonQueryAsync();

            return new BaseResponse { IsSuccess = true, Message = $"Berhasil dikirim ke {targetType}." };
        }
        catch (Exception ex) { return new BaseResponse { IsSuccess = false, ErrorMessage = $"Error: {ex.Message}" }; }
    }

    public async Task<BaseResponse?> ApprovePengubahanNilaiAsync(int id, string username, string roleId)
    {
        try
        {
            await using var conn = new SqlConnection(_conn);
            await conn.OpenAsync();
            var details = await GetStatusAndOwnerAsync(conn, id);
            if (!details.Found) return new BaseResponse { IsSuccess = false, ErrorMessage = ERROR_DATA_NOT_FOUND };

            string currentStatus = details.Status.Trim();
            string tipeApprove = "";

            if (roleId == ROLE_PRODI || roleId == ROLE_KAPROD)
            {
                tipeApprove = "Prodi";
                if (!currentStatus.Equals(STATUS_MENUNGGU_PRODI, StringComparison.OrdinalIgnoreCase))
                    return new BaseResponse { IsSuccess = false, ErrorMessage = "Gagal: Status bukan Menunggu Approval Prodi." };
            }
            else if (roleId == ROLE_DAAK)
            {
                tipeApprove = "DAAK";
                if (!currentStatus.Equals(STATUS_MENUNGGU_DAAK, StringComparison.OrdinalIgnoreCase))
                    return new BaseResponse { IsSuccess = false, ErrorMessage = "Gagal: Status bukan Menunggu Approval DAAK." };
            }
            

            await using var cmd = new SqlCommand("sia_approvePengubahanNilai", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@TipeApprove", tipeApprove);
            cmd.Parameters.AddWithValue("@ApproveId", id.ToString());
            cmd.Parameters.AddWithValue(PARAM_USERNAME, username);
            
            await cmd.ExecuteNonQueryAsync();

            return new BaseResponse { IsSuccess = true, Message = "Berhasil Disetujui." };
        }
        catch (Exception ex) { return new BaseResponse { IsSuccess = false, ErrorMessage = ex.Message }; }
    }

    public async Task<BaseResponse?> RejectPengubahanNilaiAsync(int id, RejectPengubahanNilaiDto dto, string username, string roleId)
    {
        try
        {
            await using var conn = new SqlConnection(_conn);
            await conn.OpenAsync();
            var details = await GetStatusAndOwnerAsync(conn, id);
            if (!details.Found) return new BaseResponse { IsSuccess = false, ErrorMessage = ERROR_DATA_NOT_FOUND };

            string currentStatus = details.Status.Trim();

            if ((roleId == ROLE_PRODI || roleId == ROLE_KAPROD) && !currentStatus.Equals(STATUS_MENUNGGU_PRODI, StringComparison.OrdinalIgnoreCase) ||
                roleId == ROLE_DAAK && !currentStatus.Equals(STATUS_MENUNGGU_DAAK, StringComparison.OrdinalIgnoreCase))
            {
                return new BaseResponse { IsSuccess = false, ErrorMessage = "Gagal tolak: Status tidak sesuai." };
            }

            await using var cmd = new SqlCommand("sia_rejectPengubahanNilai", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@RejectId", id.ToString());
            cmd.Parameters.AddWithValue(PARAM_USERNAME, username);
            cmd.Parameters.AddWithValue("@AlasanTolak", dto.AlasanTolak);
            await cmd.ExecuteNonQueryAsync();

            return new BaseResponse { IsSuccess = true, Message = "Pengajuan Ditolak." };
        }
        catch (Exception ex) { return new BaseResponse { IsSuccess = false, ErrorMessage = ex.Message }; }
    }

    public async Task<BaseResponse?> DeletePengubahanNilaiAsync(int id, string username, string roleId)
    {
        try
        {
            await using var conn = new SqlConnection(_conn);
            await conn.OpenAsync();
            var details = await GetStatusAndOwnerAsync(conn, id);
            if (!details.Found) return new BaseResponse { IsSuccess = false, ErrorMessage = ERROR_DATA_NOT_FOUND };

            string currentStatus = details.Status.Trim();

            if (!currentStatus.Equals(STATUS_DRAFT, StringComparison.OrdinalIgnoreCase))
                return new BaseResponse { IsSuccess = false, ErrorMessage = "Hanya status Draft/Revisi yang bisa dihapus." };

            if (roleId == ROLE_DOSEN && !string.Equals(details.CreatedBy.Trim(), username.Trim(), StringComparison.OrdinalIgnoreCase))
                return new BaseResponse { IsSuccess = false, ErrorMessage = "Anda tidak berhak menghapus data ini." };

            await using var cmd = new SqlCommand("sia_deletePengubahanNilai", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@RejectId", id.ToString());
            await cmd.ExecuteNonQueryAsync();

            return new BaseResponse { IsSuccess = true, Message = "Data Berhasil Dihapus." };
        }
        catch (Exception ex) { return new BaseResponse { IsSuccess = false, ErrorMessage = ex.Message }; }
    }

    private static async Task<(bool Found, string Status, string CreatedBy)> GetStatusAndOwnerAsync(SqlConnection conn, int id)
    {
        await using var cmd = new SqlCommand("sia_detailPengubahanNilai", conn) { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.AddWithValue("@DetailId", id.ToString());

        using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return (
                true,
                SafeGetString(reader, "ppn_status"),
                SafeGetString(reader, COLUMN_PPN_CREATED_BY)
            );
        }
        return (false, "", "");
    }

    private static string ConvertSemesterToText(string semester)
    {
        if (int.TryParse(semester, out int sem)) return sem % 2 != 0 ? "Ganjil" : "Genap";
        return semester;
    }

    private static string SafeGetString(SqlDataReader reader, string colName)
    {
        try { return reader[colName]?.ToString() ?? ""; } catch { return ""; }
    }

    private static int GetInt(SqlDataReader reader, string colName)
    {
        try { return Convert.ToInt32(reader[colName]); } catch { return 0; }
    }

    public async Task<BaseResponse?> CreatePengubahanNilaiAsync(CreatePengubahanNilaiDto dto, string username)
    {
        try
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_createPengubahanNilai", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue(PARAM_TAHUN_AJARAN, dto.TahunAjaran);
            cmd.Parameters.AddWithValue(PARAM_SEMESTER, dto.Semester);
            cmd.Parameters.AddWithValue("@MatakuliahId", dto.MataKuliahId);
            cmd.Parameters.AddWithValue("@KelasId", dto.KelasId);
            cmd.Parameters.AddWithValue(PARAM_USERNAME, username);
            cmd.Parameters.AddWithValue("@AlasanPengajuan", dto.AlasanPengajuan);
            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
            return new BaseResponse { IsSuccess = true, Message = "Berhasil dibuat" };
        }
        catch (Exception ex) { return new BaseResponse { IsSuccess = false, ErrorMessage = ex.Message }; }
    }

    public async Task<DetailPengubahanNilaiResponse?> GetDetailPengubahanNilaiAsync(int id)
    {
        try
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_detailPengubahanNilai", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@DetailId", id.ToString());
            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                
                var model = new PengubahanNilai
                {
                    TahunAjaran = SafeGetString(reader, "kni_tahun_ajaran"),
                    Semester = SafeGetString(reader, "kni_semester"),
                    MataKuliahNama = SafeGetString(reader, "mku_nama"),
                    KelasId = SafeGetString(reader, "kel_id"),
                    DosenNama = SafeGetString(reader, "dos_nama"),
                    Status = SafeGetString(reader, "ppn_status"),
                    AlasanTolak = SafeGetString(reader, "ppn_alasan_tolak"),
                    MataKuliahId = SafeGetString(reader, "mku_id"),
                    AlasanPengajuan = SafeGetString(reader, "ppn_alasan_pengajuan"),
                    CreatedBy = SafeGetString(reader, "ppn_created_by"),
                    CreatedDate = reader.GetDateTime(reader.GetOrdinal("ppn_created_date"))
                };

                return new DetailPengubahanNilaiResponse
                {
                    TahunAjaran = model.TahunAjaran,
                    Semester = model.Semester,
                    MataKuliah = model.MataKuliahNama,
                    KelasId = model.KelasId,
                    Dosen = model.DosenNama,
                    Status = model.Status,
                    AlasanTolak = model.AlasanTolak,
                    MataKuliahId = model.MataKuliahId,
                    AlasanPengajuan = model.AlasanPengajuan,
                    DibuatOleh = model.CreatedBy,
                    TanggalDibuat = model.CreatedDate.ToString("dd MMMM yyyy HH:mm")
                };
            }
            return new DetailPengubahanNilaiResponse { ErrorMessage = ERROR_DATA_NOT_FOUND };
        }
        catch (Exception ex) { return new DetailPengubahanNilaiResponse { ErrorMessage = ex.Message }; }
    }

    public async Task<DosenByPengubahanResponse?> GetDosenByPengubahanNilaiAsync(int ppnId)
    {
        var result = new DosenByPengubahanResponse(); var data = new List<DosenByPengubahanDto>();
        try
        {
            await using var conn = new SqlConnection(_conn); await conn.OpenAsync();
            await using var cmd = new SqlCommand("sia_getDosenByPengubahanNilai", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@PermintaanPengubahanNilaiId", ppnId.ToString());
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync()) data.Add(new DosenByPengubahanDto { Username = SafeGetString(reader, COLUMN_PPN_CREATED_BY), Nama = SafeGetString(reader, "dos_nama") });
            result.Data = data; return result;
        }
        catch (Exception ex) { return new DosenByPengubahanResponse { ErrorMessage = ex.Message }; }
    }

    public async Task<ProdiByMatkulResponse?> GetProdiByMataKuliahAsync(string mkuId)
    {
        var result = new ProdiByMatkulResponse(); var data = new List<ProdiByMatkulDto>();
        try
        {
            await using var conn = new SqlConnection(_conn); await conn.OpenAsync();
            await using var cmd = new SqlCommand("sia_getProdiByMatkul", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@MataKuliahId", mkuId);
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync()) data.Add(new ProdiByMatkulDto { CoordinatorUsername = SafeGetString(reader, "kry_username") });
            result.Data = data; return result;
        }
        catch (Exception ex) { return new ProdiByMatkulResponse { ErrorMessage = ex.Message }; }
    }

    public async Task<KonsentrasiResponse?> GetListKonsentrasiAsync(string username, string roleId, bool filterByUser = false)
    {
        var result = new KonsentrasiResponse(); var data = new List<KonsentrasiDto>();
        try
        {
            await using var conn = new SqlConnection(_conn); await conn.OpenAsync();
            var cmd = new SqlCommand(filterByUser ? "sia_getKonsentrasiByUser" : "sia_getListKonsentrasi", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue(PARAM_USERNAME, username ?? ""); if (!filterByUser) cmd.Parameters.AddWithValue("@RoleId", roleId ?? "");
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync()) data.Add(new KonsentrasiDto { KonsentrasiId = SafeGetString(reader, "Kon_Id"), KonNama = SafeGetString(reader, "kon_nama") });
            result.Data = data; return result;
        }
        catch (Exception ex) { return new KonsentrasiResponse { ErrorMessage = ex.Message }; }
    }

    public async Task<TahunAjaranResponse?> GetListTahunAjaranAsync()
    {
        try
        {
            var activeDto = await GetActiveTahunAkademikAsync(); string activeYear = activeDto?.Data?.FirstOrDefault()?.TahunAjaran ?? "";
            var result = new TahunAjaranResponse(); var data = new List<string>();
            await using var conn = new SqlConnection(_conn); await conn.OpenAsync();
            await using var cmd = new SqlCommand("sia_getListTahunAjaran", conn) { CommandType = CommandType.StoredProcedure };
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                string year = SafeGetString(reader, "kak_tahun_ajaran");
                if (string.IsNullOrEmpty(activeYear) || string.Compare(year, activeYear) <= 0) data.Add(year);
            }
            result.Data = data; return result;
        }
        catch (Exception ex) { return new TahunAjaranResponse { ErrorMessage = ex.Message }; }
    }

    public async Task<TahunAkademikActiveResponse?> GetActiveTahunAkademikAsync()
    {
        var result = new TahunAkademikActiveResponse(); var data = new List<TahunAkademikActiveDto>();
        try
        {
            await using var conn = new SqlConnection(_conn); await conn.OpenAsync();
            await using var cmd = new SqlCommand("sia_getActiveTahunAkademikByTanggal", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@DateToCheck", DBNull.Value);
            await using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync()) data.Add(new TahunAkademikActiveDto { TahunAjaran = SafeGetString(reader, "kak_tahun_ajaran"), Semester = ConvertSemesterToText(SafeGetString(reader, "kak_ganjil_genap")), TanggalMulai = SafeGetString(reader, "kak_tgl_from") });
            else data.Add(new TahunAkademikActiveDto { TahunAjaran = $"{DateTime.Now.Year}/{DateTime.Now.Year + 1}", Semester = "Ganjil", TanggalMulai = DateTime.Now.ToString("yyyy-MM-dd") });
            result.Data = data; return result;
        }
        catch (Exception ex) { return new TahunAkademikActiveResponse { ErrorMessage = ex.Message }; }
    }

    public async Task<MataKuliahResponse?> GetListMataKuliahAsync(string tahunAjaran, string semester, string roleId, string username)
    {
        var result = new MataKuliahResponse(); var data = new List<MataKuliahDto>();
        try
        {
            await using var conn = new SqlConnection(_conn); await conn.OpenAsync();
            await using var cmd = new SqlCommand("sia_getListMataKuliahByPenilaian", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue(PARAM_USERNAME, username ?? "");
            cmd.Parameters.AddWithValue(PARAM_TAHUN_AJARAN, tahunAjaran ?? "");
            cmd.Parameters.AddWithValue(PARAM_SEMESTER, semester ?? "");
            cmd.Parameters.AddWithValue(PARAM_ROLE_ID, roleId ?? "");
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync()) data.Add(new MataKuliahDto { MataKuliahId = SafeGetString(reader, "mku_id"), MataKuliahNama = SafeGetString(reader, "mku_nama") });
            result.Data = data; return result;
        }
        catch (Exception ex) { return new MataKuliahResponse { ErrorMessage = ex.Message }; }
    }

    public async Task<KelasResponse?> GetListKelasAsync(string tahunAjaran, string semester, string mataKuliahId, string roleId, string username)
    {
        var result = new KelasResponse(); var data = new List<string>();
        try
        {
            await using var conn = new SqlConnection(_conn); await conn.OpenAsync();
            await using var cmd = new SqlCommand("sia_getListKelasByPenilaian", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue(PARAM_USERNAME, username);
            cmd.Parameters.AddWithValue(PARAM_TAHUN_AJARAN, tahunAjaran);
            cmd.Parameters.AddWithValue(PARAM_SEMESTER, semester);
            cmd.Parameters.AddWithValue("@MatakuliahId", mataKuliahId);
            cmd.Parameters.AddWithValue(PARAM_ROLE_ID, roleId);
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync()) data.Add(SafeGetString(reader, "kel_id"));
            result.Data = data; return result;
        }
        catch (Exception ex) { return new KelasResponse { ErrorMessage = ex.Message }; }
    }

    private static bool GetBoolean(SqlDataReader reader, string colName)
    {
        try
        {
            int ord = reader.GetOrdinal(colName);
            return !reader.IsDBNull(ord) && Convert.ToBoolean(reader[ord]);
        }
        catch (Exception)
        {
            return false;
        }
    }
}

