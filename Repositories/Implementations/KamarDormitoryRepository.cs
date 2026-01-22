using astratech_apps_backend.DTOs.Kamar_Dormitory;
using astratech_apps_backend.Repositories.Interfaces;
using System.Globalization;
using Microsoft.Data.SqlClient;
using System.Data;

namespace astratech_apps_backend.Repositories.Implementations
{
    public class KamarDormitoryRepository(IConfiguration config) : IKamarDormitoryRepository
    {
        private readonly string _conn = PolmanAstraLibrary.PolmanAstraLibrary
            .Decrypt(config.GetConnectionString("DefaultConnection")!,
            Environment.GetEnvironmentVariable("DECRYPT_KEY_CONNECTION_STRING"));

        private const string SearchKeyword = "@SearchKeyword";
        private const string Status = "@Status";
        private const string Urut = "@Urut";
        private const string JenisKamar = "@JenisKamar";
        private const string Halaman = "@Halaman";
        private const string Limit = "@Limit";
        private const string DormitoryId = "@DormitoryId";
        private const string Mode = "@Mode";
        private const string Lantai = "@Lantai";
        private const string KodeKamar = "@KodeKamar";
        private const string Foto = "@Foto";
        private const string Keterangan = "@Keterangan";
        private const string CreatedBy = "@CreatedBy";
        private const string AtributId = "@AtributId";
        private const string Jumlah = "@Jumlah";
        private const string DetailDormitory = "sia_detailDormitory";
        private const string DormitoryAtribut = "sia_getDataDormitoryAtribut";
        private const string DormitoryPenghuni = "sia_getDataDormitoryPenghuni";
        private const string GetDormitoryAtribut = "sia_getDataDormitoryAtribut";
        private const string GetDormitory = "sia_getDataDormitory";
        private const string CreateDormitory = "sia_createDormitory";
        private const string CreateDormitoryAtribut = "sia_createDormitoryAtribut";
        private const string Id = "dja_id";
        private const string Nama = "dja_nama";
        private const string Jumlah1 = "dat_jumlah";
        private const string Status1 = "dat_status";
        private const string Keterangan1 = "dat_keterangan";
        private const string TotalData = "TotalData";
        private const string RowNum = "RowNum";
        private const string Id1 = "dor_id";
        private const string Kamar = "dor_kamar";
        private const string JenisKamar1 = "dor_jenis_kamar";
        private const string Penghuni = "dor_penghuni";
        private const string Kondisi = "dor_kondisi";
        private const string Status2 = "dor_status";
        private const string StatusAktif = "Aktif";
        private const string UrutKamar = "dor_kamar";
        private const string ModeDetail = "Detail";
        private const string ModeAktif = "Aktif";
        private const string KondisiBaik = "Baik";
        private const string Baik = "Baik";
        private const string TidakBaik = "Tidak Baik";
        private const string Empty = "";
        private const string StatusRiwayat = "Riwayat";
        private const string FullDateTime = "yyyy-MM-dd HH:mm:ss";
        private const string EndOfDay = "yyyy-MM-dd 23:59:59";

        public async Task<IEnumerable<AtributKamarDto>> GetDefaultAttributesAsync()
        {
            var list = new List<AtributKamarDto>();

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand(GetDormitoryAtribut, conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue(DormitoryId, string.Empty);

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            var ordinalId = reader.GetOrdinal(Id);
            var ordinalNama = reader.GetOrdinal(Nama);
            var ordinalJumlah = reader.GetOrdinal(Jumlah1);
            var ordinalStatus = reader.GetOrdinal(Status1);
            var ordinalKet = reader.GetOrdinal(Keterangan1);

            while (await reader.ReadAsync())
            {
                list.Add(new AtributKamarDto
                {
                    Id = reader.GetInt32(ordinalId),
                    NamaAtribut = reader.GetString(ordinalNama),
                    Jumlah = reader.GetInt32(ordinalJumlah),
                    KondisiBaik = reader.GetString(ordinalStatus) == bool.TrueString,
                    Keterangan = reader.GetString(ordinalKet)
                });
            }

            return list;
        }
        public async Task<(IEnumerable<KamarDormitoryDto>, int totalData)> GetAllAsync(GetAllKamarDormitoryRequest dto)
        {
            var list = new List<KamarDormitoryDto>();

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand(GetDormitory, conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue(SearchKeyword, dto.SearchKeyword ?? string.Empty);
            cmd.Parameters.AddWithValue(Status, dto.Status ?? StatusAktif);
            cmd.Parameters.AddWithValue(Urut, dto.Urut ?? UrutKamar);
            cmd.Parameters.AddWithValue(JenisKamar, dto.JenisKamar ?? string.Empty);
            cmd.Parameters.AddWithValue(Halaman, dto.PageNumber);
            cmd.Parameters.AddWithValue(Limit, dto.PageSize);

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            int totalData = 0;

            var ordinalTotal = reader.GetOrdinal(TotalData);
            var ordinalRow = reader.GetOrdinal(RowNum);
            var ordinalId = reader.GetOrdinal(Id1);
            var ordinalKamar = reader.GetOrdinal(Kamar);
            var ordinalJenis = reader.GetOrdinal(JenisKamar1);
            var ordinalPenghuni = reader.GetOrdinal(Penghuni);
            var ordinalKondisi = reader.GetOrdinal(Kondisi);
            var ordinalStatus = reader.GetOrdinal(Status2);

            while (await reader.ReadAsync())
            {
                if (totalData == 0)
                    totalData = reader.GetInt32(ordinalTotal);

                list.Add(new KamarDormitoryDto
                {
                    RowNumber = reader.GetInt64(ordinalRow),
                    Id = reader.GetInt32(ordinalId),
                    KodeKamar = reader.GetString(ordinalKamar),
                    JenisKamar = reader.GetString(ordinalJenis),
                    Penghuni = reader.GetInt32(ordinalPenghuni),
                    Kondisi = reader.GetString(ordinalKondisi),
                    Status = reader.GetString(ordinalStatus)
                });
            }

            return (list, totalData);
        }
        public async Task<KamarDormitoryDto?> GetByIdAsync(int id)
        {
            await using var conn = new SqlConnection(_conn);
            await conn.OpenAsync();

            KamarDormitoryDto dto;

            await using (var cmd = new SqlCommand(DetailDormitory, conn)
            {
                CommandType = CommandType.StoredProcedure
            })
            {
                cmd.Parameters.AddWithValue(DormitoryId, id.ToString());

                await using var reader = await cmd.ExecuteReaderAsync();
                if (!await reader.ReadAsync())
                    return null;

                dto = new KamarDormitoryDto
                {
                    Id = id,
                    RowNumber = 0,
                    Lantai = await reader.IsDBNullAsync(0) ? (short)0 : reader.GetInt16(0),
                    KodeKamar = await reader.IsDBNullAsync(1) ? string.Empty : reader.GetString(1),
                    JenisKamar = await reader.IsDBNullAsync(2) ? string.Empty : reader.GetString(2),
                    Foto = await reader.IsDBNullAsync(3) ? string.Empty : reader.GetString(3),
                    Penghuni = await reader.IsDBNullAsync(4) ? 0 : reader.GetInt32(4),
                    Kondisi = await reader.IsDBNullAsync(5) ? KondisiBaik : reader.GetString(5),
                    Keterangan = await reader.IsDBNullAsync(6) ? string.Empty : reader.GetString(6),
                    Status = await reader.IsDBNullAsync(7) ? StatusAktif : reader.GetString(7),
                    Atribut = new List<AtributKamarDto>(),
                    JumlahPenghuniAktif = 0
                };
            }

            await using (var cmdAttr = new SqlCommand(DormitoryAtribut, conn)
            {
                CommandType = CommandType.StoredProcedure
            })
            {
                cmdAttr.Parameters.AddWithValue(DormitoryId, id.ToString());
                cmdAttr.Parameters.AddWithValue(Mode, ModeDetail);

                await using var attrReader = await cmdAttr.ExecuteReaderAsync();
                while (await attrReader.ReadAsync())
                {
                    dto.Atribut.Add(new AtributKamarDto
                    {
                        Id = await attrReader.IsDBNullAsync(1) ? 0 : Convert.ToInt32(attrReader.GetValue(1)),
                        NamaAtribut = await attrReader.IsDBNullAsync(2) ? string.Empty : attrReader.GetString(2),
                        Jumlah = await attrReader.IsDBNullAsync(3) ? 0 : Convert.ToInt32(attrReader.GetValue(3)),
                        KondisiBaik = (await attrReader.IsDBNullAsync(4) ? bool.FalseString : attrReader.GetString(4))
                            .Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase),
                        Keterangan = await attrReader.IsDBNullAsync(5) ? string.Empty : attrReader.GetString(5)
                    });
                }
            }

            var penghuniAktifList = new List<PenghuniKamarDto>();
            await using (var cmdAktif = new SqlCommand(DormitoryPenghuni, conn)
            {
                CommandType = CommandType.StoredProcedure
            })
            {
                cmdAktif.Parameters.AddWithValue(DormitoryId, id.ToString());
                cmdAktif.Parameters.AddWithValue(Mode, ModeAktif);

                await using var readerAktif = await cmdAktif.ExecuteReaderAsync();
                while (await readerAktif.ReadAsync())
                {
                    penghuniAktifList.Add(new PenghuniKamarDto
                    {
                        RowNumber = readerAktif.GetInt64(0),
                        Id = readerAktif.GetInt32(1),
                        Nama = readerAktif.GetString(2),
                        Prodi = readerAktif.GetString(3),
                        HP = readerAktif.GetString(4),
                        TanggalMasuk = readerAktif.GetString(5),
                        TerakhirDitagih = readerAktif.GetString(6),
                        TanggalKeluar = null
                    });
                }
            }

            dto.JumlahPenghuniAktif = penghuniAktifList.Count;
            return dto;
        }


        public async Task<int> CreateAsync(CreateKamarDormitoryRequest dto, string createdBy)
        {
            await using var conn = new SqlConnection(_conn);
            await conn.OpenAsync();

            int newId;
            await using (var cmd = new SqlCommand(CreateDormitory, conn)
            {
                CommandType = CommandType.StoredProcedure
            })
            {
                cmd.Parameters.AddWithValue(Lantai, dto.Lantai);
                cmd.Parameters.AddWithValue(KodeKamar, dto.KodeKamar);
                cmd.Parameters.AddWithValue(JenisKamar, dto.JenisKamar);
                cmd.Parameters.AddWithValue(Foto, dto.Foto ?? Empty);
                cmd.Parameters.AddWithValue(Keterangan, dto.Keterangan ?? Empty);
                cmd.Parameters.AddWithValue(CreatedBy, createdBy);

                newId = Convert.ToInt32(await cmd.ExecuteScalarAsync());
            }

            var defaultAttrs = await GetDefaultAttributesAsync();

            foreach (var attr in dto.Atribut)
            {
                var defaultAttr = defaultAttrs.FirstOrDefault(a =>
                    a.NamaAtribut.Equals(attr.NamaAtribut, StringComparison.OrdinalIgnoreCase));

                if (defaultAttr != null)
                {
                    await using var cmdAttr = new SqlCommand(CreateDormitoryAtribut, conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmdAttr.Parameters.AddWithValue(DormitoryId, newId);
                    cmdAttr.Parameters.AddWithValue(AtributId, defaultAttr.Id);
                    cmdAttr.Parameters.AddWithValue(Jumlah, attr.Jumlah.ToString());
                    cmdAttr.Parameters.AddWithValue(Status, attr.KondisiBaik ? Baik : TidakBaik);
                    cmdAttr.Parameters.AddWithValue(Keterangan, attr.Keterangan ?? Empty);
                    cmdAttr.Parameters.AddWithValue(CreatedBy, createdBy);

                    await cmdAttr.ExecuteNonQueryAsync();
                }
            }

            return newId;
        }

        public async Task<bool> UpdateAsync(UpdateKamarDormitoryRequest dto, string updatedBy)
        {
            await using var conn = new SqlConnection(_conn);
            await conn.OpenAsync();

            await using (var cmd = new SqlCommand("sia_editDormitory", conn)
            {
                CommandType = CommandType.StoredProcedure
            })
            {
                cmd.Parameters.AddWithValue("@Id", dto.Id);
                cmd.Parameters.AddWithValue("@Lantai", dto.Lantai);
                cmd.Parameters.AddWithValue("@KodeKamar", dto.KodeKamar);
                cmd.Parameters.AddWithValue("@JenisKamar", dto.JenisKamar);
                cmd.Parameters.AddWithValue("@Foto", dto.Foto ?? "");
                cmd.Parameters.AddWithValue("@Keterangan", dto.Keterangan ?? "");
                cmd.Parameters.AddWithValue("@UpdatedBy", updatedBy);

                await cmd.ExecuteNonQueryAsync();
            }

            var defaultAttrs = await GetDefaultAttributesAsync();

            foreach (var attr in dto.Atribut)
            {
                var defaultAttr = defaultAttrs.FirstOrDefault(a =>
                    a.NamaAtribut.Equals(attr.NamaAtribut, StringComparison.OrdinalIgnoreCase));

                if (defaultAttr != null)
                {
                    await using var cmdAttr = new SqlCommand("sia_createDormitoryAtribut", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmdAttr.Parameters.AddWithValue("@DormitoryId", dto.Id);
                    cmdAttr.Parameters.AddWithValue("@AtributId", defaultAttr.Id);
                    cmdAttr.Parameters.AddWithValue("@Jumlah", attr.Jumlah.ToString());
                    cmdAttr.Parameters.AddWithValue("@Status", attr.KondisiBaik ? "Baik" : "Tidak Baik");
                    cmdAttr.Parameters.AddWithValue("@Keterangan", attr.Keterangan ?? "");
                    cmdAttr.Parameters.AddWithValue("@CreatedBy", updatedBy);

                    await cmdAttr.ExecuteNonQueryAsync();
                }
            }

            return true;
        }
        public async Task<bool> SetStatusAsync(int id)
        {
            await using var conn = new SqlConnection(_conn);
            await conn.OpenAsync();

            await using var cmd = new SqlCommand("sia_setStatusDormitory", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Id", id.ToString());

            var result = await cmd.ExecuteScalarAsync();
            return result?.ToString() == "OK";
        }

        public async Task<(IEnumerable<PenghuniKamarDto> list, int totalData, int jumlahAktif)> GetPenghuniKamarAsync(GetPenghuniKamarRequest dto)
        {
            var list = new List<PenghuniKamarDto>();
            int jumlahAktif = 0;

            var mode = dto.Status ?? StatusAktif;

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getDataDormitoryPenghuni", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@DormitoryId", dto.IdKamar.ToString());
            cmd.Parameters.AddWithValue("@Mode", mode);

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var penghuni = new PenghuniKamarDto
                {
                    RowNumber = await reader.IsDBNullAsync(0) ? 0 : Convert.ToInt64(reader.GetValue(0)),
                    Id = await reader.IsDBNullAsync(1) ? 0 : Convert.ToInt32(reader.GetValue(1)),
                    Nama = await reader.IsDBNullAsync(2) ? string.Empty : reader.GetString(2),
                    Prodi = await reader.IsDBNullAsync(3) ? string.Empty : reader.GetString(3),
                    HP = await reader.IsDBNullAsync(4) ? string.Empty : reader.GetString(4),
                    TanggalMasuk = await reader.IsDBNullAsync(5) ? string.Empty : reader.GetString(5),
                    TerakhirDitagih = await reader.IsDBNullAsync(6) ? string.Empty : reader.GetString(6),
                    TanggalKeluar = null
                };

                if (mode == StatusRiwayat)
                {
                    penghuni.TanggalKeluar = await reader.IsDBNullAsync(7) ? string.Empty : reader.GetString(7);
                }

                list.Add(penghuni);
            }

            if (mode == StatusAktif)
            {
                jumlahAktif = list.Count;
            }

            return (list, list.Count, jumlahAktif);
        }
        public async Task<int> GetJumlahPenghuniAktifAsync(int idKamar)
        {
            var request = new GetPenghuniKamarRequest
            {
                IdKamar = idKamar,
                Status = "Aktif"
            };

            var (_, _, jumlahAktif) = await GetPenghuniKamarAsync(request);
            return jumlahAktif;
        }
        public async Task<DetailPenghuniKamarDto?> GetDetailPenghuniKamarAsync(int idPenghuni)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_detailDormitoryPenghuni", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@PenghuniId", idPenghuni.ToString());

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new DetailPenghuniKamarDto
                {
                    NIK = await reader.IsDBNullAsync(0) ? "" : reader.GetString(0),
                    AlamatKTP = await reader.IsDBNullAsync(1) ? "" : reader.GetString(1),
                    ScanKTP = await reader.IsDBNullAsync(2) ? "" : reader.GetString(2)
                };
            }

            return null;
        }
        public async Task<bool> CreatePenghuniAsync(
    CreatePenghuniKamarRequest dto,
    string createdBy)
        {
            await using var conn = new SqlConnection(_conn);
            await conn.OpenAsync();

            await using var cmd = new SqlCommand("sia_createDormitoryPenghuni", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@DorId", dto.IdKamar);
            cmd.Parameters.AddWithValue("@MhsId", dto.IdMahasiswa.Trim());
            cmd.Parameters.AddWithValue("@NamaPenghuni", dto.NamaPenghuni ?? "");
            cmd.Parameters.AddWithValue("@TanggalMasuk", dto.TanggalMasuk);

            cmd.Parameters.AddWithValue("@AlamatKTP", dto.AlamatKtp ?? "");
            cmd.Parameters.AddWithValue("@CreatedBy", createdBy);

            var lastBilled = dto.LastBilled ?? dto.TanggalMasuk;
            cmd.Parameters.AddWithValue("@LastBilled", lastBilled);

            cmd.Parameters.AddWithValue("@NIK", dto.Nik ?? "");
            cmd.Parameters.AddWithValue("@ScanKTP", dto.ScanKtpFileName ?? "");

            await cmd.ExecuteNonQueryAsync();
            return true;
        }

        public async Task<bool> PindahPenghuniAsync(PindahPenghuniRequest dto, string updatedBy)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_setPindahDormitory", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@PenghuniId", dto.IdPenghuni.ToString());
            cmd.Parameters.AddWithValue("@KamarBaruId", dto.IdKamarBaru.ToString());
            cmd.Parameters.AddWithValue("@UpdatedBy", updatedBy);

            await conn.OpenAsync();
            var result = await cmd.ExecuteScalarAsync();

            return result?.ToString() == "OK";
        }
        public async Task<bool> CheckoutPenghuniAsync(CheckoutPenghuniRequest dto, string updatedBy)
        {
            await using var conn = new SqlConnection(_conn);
            await conn.OpenAsync();

            await using var cmd = new SqlCommand("sia_setKeluarDormitory", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@PenghuniId", dto.IdPenghuni.ToString());
            cmd.Parameters.AddWithValue("@TanggalKeluar", dto.TanggalKeluar.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@UpdatedBy", updatedBy);

            await cmd.ExecuteNonQueryAsync();

            return true;
        }
        public async Task<IEnumerable<MahasiswaDto>> GetAllMahasiswaAsync(GetAllMahasiswaRequest request)
        {
            var list = new List<MahasiswaDto>();

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getListMahasiswaDormitory", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@IdKamar", request.IdKamar ?? "");
            cmd.Parameters.AddWithValue("@SearchKeyword", request.SearchKeyword ?? "");

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(new MahasiswaDto
                {
                    Id = reader["mhs_id"]?.ToString() ?? "",
                    Nama = reader["mhs_nama"]?.ToString() ?? ""
                });
            }

            return list;
        }
        public async Task<IEnumerable<KamarDormitoryDto>> GetAllKamarDropdownAsync()
        {
            var list = new List<KamarDormitoryDto>();

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getAllDataDormitory", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@SearchKeyword", "");
            cmd.Parameters.AddWithValue("@Status", "Aktif");
            cmd.Parameters.AddWithValue("@Urut", "dor_kamar");
            cmd.Parameters.AddWithValue("@JenisKamar", "");

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(new KamarDormitoryDto
                {
                    Id = reader.GetInt32(reader.GetOrdinal("dor_id")),
                    KodeKamar = reader.GetString(reader.GetOrdinal("dor_kamar")),
                    JenisKamar = reader.GetString(reader.GetOrdinal("dor_jenis_kamar")),
                    Penghuni = reader.GetInt32(reader.GetOrdinal("dor_penghuni")),
                    Status = reader.GetString(reader.GetOrdinal("dor_status"))
                });
            }

            return list;
        }
        public async Task<List<KamarDormitoryExportDto>> GetDataForExportAsync(ExportKamarDormitoryRequest request)
        {
            var list = new List<KamarDormitoryExportDto>();

            await using var conn = new SqlConnection(_conn);
            await conn.OpenAsync();

            await using var cmd = new SqlCommand("sia_reportDormitory", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add("@StartDate", SqlDbType.VarChar, 50)
           .Value = request.StartDate.ToString(FullDateTime);

            cmd.Parameters.Add("@EndDate", SqlDbType.VarChar, 50)
           .Value = request.EndDate.ToString(EndOfDay);

            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var dto = new KamarDormitoryExportDto();

                dto.NIM = await reader.IsDBNullAsync(0) ? "" : await reader.GetFieldValueAsync<string>(0);
                dto.Nama = await reader.IsDBNullAsync(1) ? "" : await reader.GetFieldValueAsync<string>(1);
                dto.Prodi = await reader.IsDBNullAsync(2) ? "" : await reader.GetFieldValueAsync<string>(2);
                dto.Angkatan = await reader.IsDBNullAsync(3) ? "" : await reader.GetFieldValueAsync<string>(3);
                dto.NoKamar = await reader.IsDBNullAsync(4) ? "" : await reader.GetFieldValueAsync<string>(4);

                if (!await reader.IsDBNullAsync(5))
                {
                    var tglMasukStr = await reader.GetFieldValueAsync<string>(5);
                    if (DateTime.TryParseExact(
                            tglMasukStr,
                            "yyyy-MM-dd HH:mm:ss",
                            CultureInfo.InvariantCulture,
                            DateTimeStyles.None,
                            out var dtMasuk))
                    {
                        dto.TanggalMasuk = dtMasuk;
                    }
                }

                if (!await reader.IsDBNullAsync(6))
                {
                    var tglKeluarStr = await reader.GetFieldValueAsync<string>(6);
                    if (!string.IsNullOrEmpty(tglKeluarStr) &&
                        DateTime.TryParseExact(
                            tglKeluarStr,
                            "yyyy-MM-dd HH:mm:ss",
                            CultureInfo.InvariantCulture,
                            DateTimeStyles.None,
                            out var dtKeluar))
                    {
                        dto.TanggalKeluar = dtKeluar;
                    }
                }

                if (!await reader.IsDBNullAsync(7))
                {
                    var tglBilledStr = await reader.GetFieldValueAsync<string>(7);
                    if (DateTime.TryParseExact(
                            tglBilledStr,
                            "yyyy-MM-dd HH:mm:ss",
                            CultureInfo.InvariantCulture,
                            DateTimeStyles.None,
                            out var dtBilled))
                    {
                        dto.TerakhirDitagih = dtBilled;
                    }
                }

                dto.NIK = await reader.IsDBNullAsync(8) ? "" : await reader.GetFieldValueAsync<string>(8);
                if (dto.NIK == "-") dto.NIK = "";

                dto.Tagihan = await reader.IsDBNullAsync(9) ? 0 : await reader.GetFieldValueAsync<decimal>(9);
                dto.Pembayaran = await reader.IsDBNullAsync(10) ? 0 : await reader.GetFieldValueAsync<decimal>(10);
                dto.SisaTagihan = dto.Tagihan - dto.Pembayaran;

                list.Add(dto);
            }

            return list;
        }

    }
}