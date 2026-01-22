using astratech_apps_backend.DTOs.MasterIndustri;
using astratech_apps_backend.Models;
using astratech_apps_backend.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace astratech_apps_backend.Repositories.Implementations
{
    public class MasterIndustriRepository(IConfiguration config) : IMasterIndustriRepository
    {
        private readonly string _conn = PolmanAstraLibrary.PolmanAstraLibrary.Decrypt(config.GetConnectionString("DefaultConnection")!, Environment.GetEnvironmentVariable("DECRYPT_KEY_CONNECTION_STRING"));

        public async Task<(IEnumerable<MasterIndustri>, int totalData)> GetAllAsync(GetAllMasterIndustriRequest dto)
        {
            List<MasterIndustri> list = new();
            int totalData = 0;

            using var conn = new SqlConnection(_conn);
            using var cmd = new SqlCommand("sia_getDataIndustriPrakerin", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Keyword", dto.SearchKeyword ?? "");
            cmd.Parameters.AddWithValue("@Grup", dto.Grup ?? "");
            cmd.Parameters.AddWithValue("@Status", string.IsNullOrWhiteSpace(dto.Status) ? "All" : dto.Status);
            cmd.Parameters.AddWithValue("@Urut", string.IsNullOrWhiteSpace(dto.Urut) ? "ipr_nama DESC" : dto.Urut);
            cmd.Parameters.AddWithValue("@Halaman", dto.PageNumber <= 0 ? 1 : dto.PageNumber);
            cmd.Parameters.AddWithValue("@Limit", dto.PageSize <= 0 ? 10 : dto.PageSize);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                if (reader["Count"] != DBNull.Value)
                    totalData = Convert.ToInt32(reader["Count"]);

                list.Add(new MasterIndustri
                {
                    Id = reader["ipr_id"] != DBNull.Value ? Convert.ToInt32(reader["ipr_id"]) : 0,
                    RowNumber = reader["rownum"] != DBNull.Value ? Convert.ToInt64(reader["rownum"]) : 0,
                    NamaIndustri = reader["NamaIndustri"]?.ToString() ?? "",
                    Cabang = reader["Cabang"]?.ToString() ?? "",
                    Grup = reader["Grup"]?.ToString() ?? "",
                    Status = reader["Status"]?.ToString() ?? ""
                });
            }
            return (list, totalData);
        }

        public async Task<MasterIndustri?> GetByIdAsync(int id)
        {
            using var conn = new SqlConnection(_conn);
            using var cmd = new SqlCommand("sia_detailIndustriPrakerin", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@IndustriId", id);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return null;

            return new MasterIndustri
            {
                Id = id,
                NamaIndustri = reader["ipr_nama"]?.ToString() ?? "",
                Cabang = reader["ipr_cabang"]?.ToString() ?? "",
                Grup = reader["ipr_grup"]?.ToString() ?? "",
                Alamat = reader["ipr_alamat"]?.ToString() ?? "",
                Telepon = reader["ipr_telepon"]?.ToString() ?? "",
                Fax = reader["ipr_fax"]?.ToString() ?? "",
                PIC = reader["ipr_namaPIC"]?.ToString() ?? "",
                TeleponPIC = reader["ipr_teleponPIC"]?.ToString() ?? "",
                EmailPIC = reader["ipr_emailPIC"]?.ToString() ?? "",
                NamaPICAtasan = reader["ipr_namaPICAtasan"]?.ToString() ?? "",
                TeleponPICAtasan = reader["ipr_teleponPICAtasan"]?.ToString() ?? "",
                EmailPICAtasan = reader["ipr_emailPICAtasan"]?.ToString() ?? "",
                Status = reader["ipr_status"]?.ToString() ?? ""
            };
        }

        public async Task<int> CreateAsync(CreateMasterIndustriRequest dto, string createdBy)
        {
            using var conn = new SqlConnection(_conn);
            using var cmd = new SqlCommand("sia_createIndustriPrakerin", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@NamaIndustri", dto.NamaIndustri);
            cmd.Parameters.AddWithValue("@Cabang", (object?)dto.Cabang ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Grup", dto.Grup);
            cmd.Parameters.AddWithValue("@Alamat", dto.Alamat);
            cmd.Parameters.AddWithValue("@Telepon", dto.Telepon);
            cmd.Parameters.AddWithValue("@Fax", (object?)dto.Fax ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PIC", dto.PIC);
            cmd.Parameters.AddWithValue("@TeleponPIC", (object?)dto.TeleponPIC ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@EmailPIC", (object?)dto.EmailPIC ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PICAtasan", (object?)dto.NamaPICAtasan ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@TeleponPICAtasan", (object?)dto.TeleponPICAtasan ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@EmailPICAtasan", (object?)dto.EmailPICAtasan ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CreateBy", createdBy);



            await conn.OpenAsync();
            var result = await cmd.ExecuteScalarAsync();
            return result == null ? 0 : Convert.ToInt32(result);
        }

        public async Task<bool> UpdateAsync(UpdateMasterIndustriRequest dto, string updatedBy)
        {
            using var conn = new SqlConnection(_conn);
            using var cmd = new SqlCommand("sia_editIndustriPrakerin", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Id", dto.Id);
            cmd.Parameters.AddWithValue("@NamaIndustri", dto.NamaIndustri);
            cmd.Parameters.AddWithValue("@Cabang", (object?)dto.Cabang ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Grup", dto.Grup);
            cmd.Parameters.AddWithValue("@Alamat", dto.Alamat);
            cmd.Parameters.AddWithValue("@Telepon", (object?)dto.Telepon ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Fax", (object?)dto.Fax ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PIC", dto.PIC);
            cmd.Parameters.AddWithValue("@TeleponPIC", (object?)dto.TeleponPIC ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@EmailPIC", (object?)dto.EmailPIC ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PICAtasan", (object?)dto.NamaPICAtasan ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@TeleponPICAtasan", (object?)dto.TeleponPICAtasan ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@EmailPICAtasan", (object?)dto.EmailPICAtasan ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@UpdatedBy", updatedBy);


            await conn.OpenAsync();
            return await cmd.ExecuteNonQueryAsync() > 0;
        }


        public async Task<bool> SetStatusAsync(int id, string updatedBy)
        {
            using var conn = new SqlConnection(_conn);
            using var cmd = new SqlCommand("sia_setStatusIndustriPrakerin", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@IndustriId", id);
            cmd.Parameters.AddWithValue("@UpdatedBy", updatedBy);

            await conn.OpenAsync();
            var rowsAffected = await cmd.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

    }
}

