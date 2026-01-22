using Microsoft.Data.SqlClient;
using astratech_apps_backend.DTOs.Perwalian.Pelaksanaan;
using astratech_apps_backend.Models.Perwalian;
using astratech_apps_backend.Repositories.Interfaces;
using System.Data;
using DTOs.Perwalian.Pelaksanaan;

namespace astratech_apps_backend.Repositories.Implementations;

public class PerwalianRepository : IPerwalianRepository
{
    private const string COL_MHS_ID = "mhs_id";
    private const string COL_DOS_ID = "dos_id";
    private const string PARAM_PWA_ID = "@pwaid";

    private readonly string _conn;

    public PerwalianRepository(IConfiguration configuration)
    {
        _conn = PolmanAstraLibrary.PolmanAstraLibrary.Decrypt(
            configuration.GetConnectionString("DefaultConnection")!,
            Environment.GetEnvironmentVariable("DECRYPT_KEY_CONNECTION_STRING")
        );
    }

    private static string MapUrut(string? urut)
    {
        if (string.IsNullOrWhiteSpace(urut))
            return "pwa_created_date desc";

        if (urut.Contains("pwa_"))
            return urut;

        return urut.ToLower() switch
        {
            "pelaksanaan_perwalian_desc" => "pwa_created_date desc",
            "pelaksanaan_perwalian_asc"  => "pwa_created_date asc",
            _ => "pwa_created_date desc"
        };
    }

    public async Task<(IEnumerable<Perwalian> data, int total)> GetAll(GetAllPelaksanaanRequestDto req)
    {
        var orderBy = MapUrut(req.Urut);
        var list = new List<Perwalian>();

        using var conn = new SqlConnection(_conn);
        using var cmd = new SqlCommand("sia_getDataPerwalian", conn)
        {
            CommandType = CommandType.StoredProcedure
        };

        cmd.Parameters.AddWithValue("@searchKeyword", req.SearchKeyword ?? "");
        cmd.Parameters.AddWithValue("@orderBy", orderBy);
        cmd.Parameters.AddWithValue("@role", req.Role ?? "");
        cmd.Parameters.AddWithValue("@username", req.Username ?? "");

        await conn.OpenAsync();
        using var da = new SqlDataAdapter(cmd);
        var dt = new DataTable();
        da.Fill(dt);

        foreach (DataRow r in dt.Rows)
        {
            var pwaId = r.Field<int>("IdPerwalian");
            var details = await GetDetails(pwaId);

            list.Add(new Perwalian
            {
                IdPerwalian = pwaId,
                IdMahasiswa = r.Field<string>("IdMahasiswa"),
                IdDosen     = r.Field<int?>("IdDosen"),
                Subjek      = r.Field<string>("Subjek"),
                Status      = r.Field<string>("Status"),
                Details     = details.ToList()
            });
        }

        return (list, list.Count);
    }

    public async Task<Perwalian?> GetById(int id)
    {
        using var conn = new SqlConnection(_conn);
        using var cmd = new SqlCommand(@"
            SELECT pwa_id, mhs_id, dos_id, pwa_subjek, pwa_status
            FROM sia_msperwalian
            WHERE pwa_id = @pwaid", conn);

        cmd.Parameters.AddWithValue(PARAM_PWA_ID, id);

        await conn.OpenAsync();
        using var da = new SqlDataAdapter(cmd);
        var headerDt = new DataTable();
        da.Fill(headerDt);

        if (headerDt.Rows.Count == 0)
            return null;

        var h = headerDt.Rows[0];
        var details = await GetDetails(id);

        return new Perwalian
        {
            IdPerwalian = id,
            IdMahasiswa = h[COL_MHS_ID]?.ToString(),
            IdDosen = Convert.ToInt32(h[COL_DOS_ID]),
            Subjek = h["pwa_subjek"]?.ToString(),
            Status = h["pwa_status"]?.ToString(),
            Details = details.ToList()
        };
    }

    public async Task<Perwalian> Create(Perwalian entity)
    {
        using var conn = new SqlConnection(_conn);
        using var cmd = new SqlCommand("sia_createPerwalian", conn)
        {
            CommandType = CommandType.StoredProcedure
        };

        cmd.Parameters.AddWithValue("@IdMahasiswa", entity.IdMahasiswa);
        cmd.Parameters.AddWithValue("@IdDosen", entity.IdDosen);
        cmd.Parameters.AddWithValue("@Subjek", entity.Subjek);

        await conn.OpenAsync();
        using var da = new SqlDataAdapter(cmd);
        var dt = new DataTable();
        da.Fill(dt);

        if (dt.Rows.Count > 0 && dt.Columns.Contains("IdPerwalian"))
        {
            entity.IdPerwalian = Convert.ToInt32(dt.Rows[0]["IdPerwalian"]);
        }

        return entity;
    }

    public async Task<bool> Update(int id, UpdatePelaksanaanRequestDto dto)
    {
        using var conn = new SqlConnection(_conn);
        using var cmd = new SqlCommand("sia_editPerwalian", conn)
        {
            CommandType = CommandType.StoredProcedure
        };

        cmd.Parameters.AddWithValue("@IdPerwalian", id);
        cmd.Parameters.AddWithValue("@IdMahasiswa", dto.IdMahasiswa);
        cmd.Parameters.AddWithValue("@IdDosen", dto.IdDosen);
        cmd.Parameters.AddWithValue("@Subjek", dto.Subjek);
        cmd.Parameters.AddWithValue("@Status", dto.Status);

        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
        return true;
    }

    public async Task<bool> Delete(int id)
    {
        using var conn = new SqlConnection(_conn);
        using var cmd = new SqlCommand("sia_deletePerwalian", conn)
        {
            CommandType = CommandType.StoredProcedure
        };

        cmd.Parameters.AddWithValue(PARAM_PWA_ID, id);

        await conn.OpenAsync();
        return await cmd.ExecuteNonQueryAsync() > 0;
    }

    public async Task<IEnumerable<PerwalianDetail>> GetDetails(int pwaId)
    {
        using var conn = new SqlConnection(_conn);
        using var cmd = new SqlCommand("sia_getDataPerwalianDetail", conn)
        {
            CommandType = CommandType.StoredProcedure
        };

        cmd.Parameters.AddWithValue("@IdPerwalian", pwaId);

        await conn.OpenAsync();
        using var da = new SqlDataAdapter(cmd);
        var dt = new DataTable();
        da.Fill(dt);

        return dt.AsEnumerable().Select(perwaliandetail => new PerwalianDetail
        {
            IdPerwalianDetail = Convert.ToInt32(perwaliandetail["IdDetail"]),
            IdPerwalian = pwaId,
            Pesan = perwaliandetail["Pesan"]?.ToString(),
            Tipe = perwaliandetail["Tipe"]?.ToString(),
            Berkas = perwaliandetail["Berkas"]?.ToString(),
            Status = perwaliandetail["Status"]?.ToString()
        });
    }

    public async Task<PerwalianDetail> AddDetail(PerwalianDetail detail)
    {
        using var conn = new SqlConnection(_conn);
        using var cmd = new SqlCommand("sia_createBalasanPerwalian", conn)
        {
            CommandType = CommandType.StoredProcedure
        };

        cmd.Parameters.AddWithValue(PARAM_PWA_ID, detail.IdPerwalian);
        cmd.Parameters.AddWithValue("@pesan", detail.Pesan);
        cmd.Parameters.AddWithValue("@tipe", detail.Tipe);
        cmd.Parameters.AddWithValue("@berkas", detail.Berkas ?? "");

        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
        return detail;
    }

    public async Task<List<Dictionary<string, object>>> GetDropdownMahasiswa(string username)
    {
        using var conn = new SqlConnection(_conn);
        using var cmd = new SqlCommand("dbo.sia_getListMahasiswa", conn)
        {
            CommandType = CommandType.StoredProcedure
        };

        await conn.OpenAsync();
        using var da = new SqlDataAdapter(cmd);
        var dt = new DataTable();
        da.Fill(dt);

        var list = new List<Dictionary<string, object>>();
        foreach (DataRow row in dt.Rows)
        {
            list.Add(new Dictionary<string, object>
            {
                ["id"] = row[COL_MHS_ID]?.ToString() ?? "",
                ["nama"] = row["mhs_nama"]?.ToString() ?? ""
            });
        }

        return list;
    }

    public async Task<List<Dictionary<string, object>>> GetDropdownDosen(string username)
    {
        using var conn = new SqlConnection(_conn);
        using var cmd = new SqlCommand("sia_getListDosenByKonsentrasi", conn)
        {
            CommandType = CommandType.StoredProcedure
        };

        await conn.OpenAsync();
        using var da = new SqlDataAdapter(cmd);
        var dt = new DataTable();
        da.Fill(dt);

        var list = new List<Dictionary<string, object>>();
        foreach (DataRow row in dt.Rows)
        {
            list.Add(new Dictionary<string, object>
            {
                ["id"] = row[COL_DOS_ID]?.ToString() ?? "",
                ["nama"] = row["dos_nama"]?.ToString() ?? ""
            });
        }

        return list;
    }

    public async Task<List<Dictionary<string, object>>> GetListProdiAsync()
    {
        using var conn = new SqlConnection(_conn);
        using var cmd = new SqlCommand("sia_getListKonsentrasi", conn)
        {
            CommandType = CommandType.StoredProcedure
        };

        await conn.OpenAsync();
        using var da = new SqlDataAdapter(cmd);
        var dt = new DataTable();
        da.Fill(dt);

        var list = new List<Dictionary<string, object>>();
        foreach (DataRow row in dt.Rows)
        {
            list.Add(new Dictionary<string, object>
            {
                ["id"] = row["kon_id"]?.ToString() ?? "",
                ["nama"] = row["kon_nama"]?.ToString() ?? ""
            });
        }

        return list;
    }

    public async Task<List<Dictionary<string, object>>> GetMahasiswaByProdi(int idkonsentrasi)
    {
        using var conn = new SqlConnection(_conn);
        using var cmd = new SqlCommand("sia_getListMahasiswaByProdi", conn)
        {
            CommandType = CommandType.StoredProcedure
        };

        cmd.Parameters.AddWithValue("@IdKonsentrasi", idkonsentrasi);

        await conn.OpenAsync();
        using var da = new SqlDataAdapter(cmd);
        var dt = new DataTable();
        da.Fill(dt);

        var list = new List<Dictionary<string, object>>();
        foreach (DataRow row in dt.Rows)
        {
            list.Add(new Dictionary<string, object>
            {
                ["id"] = row["IdMahasiswa"]?.ToString() ?? "",
                ["nama"] = row["NamaMahasiswa"]?.ToString() ?? "",
                ["angkatan"] = row["Angkatan"]?.ToString() ?? ""
            });
        }

        return list;
    }

    public async Task<List<Dictionary<string, object>>> GetDropdownDosenByProdi(string prodi)
    {
        using var conn = new SqlConnection(_conn);
        using var cmd = new SqlCommand("dbo.sia_getListDosenByKonsentrasi", conn)
        {
            CommandType = CommandType.StoredProcedure
        };

        await conn.OpenAsync();
        using var da = new SqlDataAdapter(cmd);
        var dt = new DataTable();
        da.Fill(dt);

        var list = new List<Dictionary<string, object>>();
        foreach (DataRow row in dt.Rows)
        {
            list.Add(new Dictionary<string, object>
            {
                ["id"] = row[COL_DOS_ID]?.ToString() ?? "",
                ["nama"] = row["dos_nama"]?.ToString() ?? ""
            });
        }

        return list;
    }

    public async Task<List<string>> GetDropdownAngkatanByProdi(string prodi)
    {
        using var conn = new SqlConnection(_conn);
        using var cmd = new SqlCommand(@"
            SELECT DISTINCT Angkatan
            FROM sia_msmahasiswa
            WHERE kon_id = @kon_id
            ORDER BY Angkatan DESC", conn);

        cmd.Parameters.AddWithValue("@kon_id", prodi);

        await conn.OpenAsync();
        using var da = new SqlDataAdapter(cmd);
        var dt = new DataTable();
        da.Fill(dt);

        return dt.AsEnumerable()
                 .Select(r => r["Angkatan"]?.ToString() ?? "")
                 .ToList();
    }
}
