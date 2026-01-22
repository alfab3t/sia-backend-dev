using astratech_apps_backend.DTOs.Mahasiswa;
using astratech_apps_backend.Models;
using astratech_apps_backend.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace astratech_apps_backend.Repositories.Implementations
{
    public class MahasiswaARepository(IConfiguration config) : IMahasiswaRepository
    {
        private const string ParamIdMahasiswa = "@MahasiswaId";
        private const string ParamIdMahasiswaCamelCase = "@IdMahasiswa";
        private const string ColumnIdMahasiswa = "mhs_id";
        private const string ColumnMhsNama = "mhs_nama";
        private const string ColumnKonId = "kon_id";
        private const string ColumnLkePoint = "lke_point";
        private const string ColumnKegId = "keg_id";
        private const string ColumnRownum = "rownum";
        private const string ColumnRmaCreatedDate = "rma_created_date";
        private const string ColumnRmaModifDate = "rma_modif_date";
        private const string ColumnSemester = "semester";
        private const string ParamHalaman = "@Halaman";
        private const string ParamLimit = "@Limit";

        private readonly string _conn = PolmanAstraLibrary.PolmanAstraLibrary.Decrypt(
            config.GetConnectionString("DefaultConnection")!,
            Environment.GetEnvironmentVariable("DECRYPT_KEY_CONNECTION_STRING"));

        public async Task<bool> UpdateAsync(UpdateDataMahasiswaRequest dto, string updatedBy)
        {
            if (dto == null || string.IsNullOrEmpty(dto.idMahasiswa))
                return false;

            if (string.IsNullOrEmpty(updatedBy))
                return false;

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_editMahasiswa", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue(ParamIdMahasiswa, dto.idMahasiswa);
            cmd.Parameters.AddWithValue("@NoPendaftaran", dto.NoPendaftaran ?? string.Empty);
            cmd.Parameters.AddWithValue("@Nama", dto.Nama ?? string.Empty);
            cmd.Parameters.AddWithValue("@JenisKelamin", dto.JenisKelamin ?? string.Empty);
            cmd.Parameters.AddWithValue("@TempatLahir", dto.TempatLahir ?? string.Empty);
            cmd.Parameters.Add("@TanggalLahir", SqlDbType.DateTime).Value = dto.TanggalLahir.GetValueOrDefault();
            cmd.Parameters.AddWithValue("@Alamat", dto.Alamat ?? string.Empty);
            cmd.Parameters.AddWithValue("@KodePos", dto.KodePos ?? string.Empty);
            cmd.Parameters.AddWithValue("@Hp", dto.Hp ?? string.Empty);
            cmd.Parameters.AddWithValue("@Email", dto.Email ?? string.Empty);
            cmd.Parameters.AddWithValue("@NamaAyah", dto.NamaAyah ?? string.Empty);
            cmd.Parameters.AddWithValue("@HpAyah", dto.HpAyah ?? string.Empty);
            cmd.Parameters.AddWithValue("@StatusAyah", dto.StatusAyah ?? string.Empty);
            cmd.Parameters.AddWithValue("@AlamatAyah", dto.AlamatAyah ?? string.Empty);
            cmd.Parameters.AddWithValue("@KodeposAyah", dto.KodePosAyah ?? string.Empty);
            cmd.Parameters.AddWithValue("@NamaIbu", dto.NamaIbu ?? string.Empty);
            cmd.Parameters.AddWithValue("@HpIbu", dto.HpIbu ?? string.Empty);
            cmd.Parameters.AddWithValue("@StatusIbu", dto.StatusIbu ?? string.Empty);
            cmd.Parameters.AddWithValue("@AlamatIbu", dto.AlamatIbu ?? string.Empty);
            cmd.Parameters.AddWithValue("@KodeposIbu", dto.KodePosIbu ?? string.Empty);
            cmd.Parameters.AddWithValue("@NamaWali", dto.NamaWali ?? string.Empty);
            cmd.Parameters.AddWithValue("@HpWali", dto.HpWali ?? string.Empty);
            cmd.Parameters.AddWithValue("@StatusWali", dto.StatusWali ?? string.Empty);
            cmd.Parameters.AddWithValue("@AlamatWali", dto.AlamatWali ?? string.Empty);
            cmd.Parameters.AddWithValue("@KodeposWali", dto.KodePosWali ?? string.Empty);


            cmd.Parameters.AddWithValue("@ModifBy", updatedBy);
            cmd.Parameters.AddWithValue("@Nisn", dto.Nisn ?? string.Empty);

            await conn.OpenAsync();

            await cmd.ExecuteNonQueryAsync();
            return true;
        }

        public async Task<string?> GetNamaFileFotoAsync(string mahasiswaId)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("dbo.sia_getProfilMahasiswa", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Nim", mahasiswaId);

            await conn.OpenAsync();

            await using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return reader["dul_pas_foto"]?.ToString();
            }

            return null;
        }


        public async Task<Mahasiswa?> GetByIdAsync(string mahasiswaId)
        {
            await using var conn = new SqlConnection(_conn);
            await conn.OpenAsync();

            var mahasiswa = await GetBasicMahasiswaDataAsync(conn, mahasiswaId);
            if (mahasiswa == null) return null;

            ParseAlamatDataAsync(mahasiswa);
            await GetProfilDataAsync(conn, mahasiswa, mahasiswaId);
            await GetLaporanKegiatanDataAsync(conn, mahasiswa, mahasiswaId);
            await GetRfidDetailDataAsync(conn, mahasiswa, mahasiswaId);

            return mahasiswa;
        }

        private static async Task<Mahasiswa?> GetBasicMahasiswaDataAsync(SqlConnection conn, string mahasiswaId)
        {
            await using var cmdDetail = new SqlCommand("sia_detailMahasiswa", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmdDetail.Parameters.AddWithValue(ParamIdMahasiswa, mahasiswaId);
            await using var reader = await cmdDetail.ExecuteReaderAsync();

            if (!await reader.ReadAsync()) return null;

            var mahasiswa = new Mahasiswa
            {
                IdMahasiswa = reader[ColumnIdMahasiswa]?.ToString(),
                NoPendaftaran = reader["dul_no_pendaftaran"]?.ToString(),
                Nama = reader[ColumnMhsNama]?.ToString(),
                Konsentrasi = reader["kon_nama"]?.ToString(),
                TempatLahir = reader["mhs_tempat_lahir"]?.ToString(),
                TanggalLahir = reader["mhs_tgl_lahir"]?.ToString(),
                JenisKelamin = reader["mhs_jenis_kelamin"]?.ToString(),
                Alamat = reader["mhs_alamat"]?.ToString()?.Replace('#', ','),
                KodePos = reader["mhs_kodepos"]?.ToString(),
                HP = reader["mhs_hp"]?.ToString(),
                Email = reader["mhs_email"]?.ToString(),
                TanggalMasuk = reader["mhs_tgl_masuk"]?.ToString(),
                TanggalLulus = reader["mhs_tgl_lulus"]?.ToString(),
                Angkatan = reader["mhs_angkatan"]?.ToString(),
                StatusKuliah = reader["mhs_status_kuliah"]?.ToString(),
                NamaAyah = reader["dul_nama_ayah"]?.ToString(),
                HPAyah = reader["dul_hp_ayah"]?.ToString(),
                StatusAyah = reader["dul_status_ayah"]?.ToString(),
                AlamatAyah = reader["dul_alamat_ayah"]?.ToString()?.Replace('#', ','),
                KodePosAyah = reader["dul_kodepos_ayah"]?.ToString(),
                NamaIbu = reader["dul_nama_ibu"]?.ToString(),
                HPIbu = reader["dul_hp_ibu"]?.ToString(),
                StatusIbu = reader["dul_status_ibu"]?.ToString(),
                AlamatIbu = reader["dul_alamat_ibu"]?.ToString()?.Replace('#', ','),
                KodePosIbu = reader["dul_kodepos_ibu"]?.ToString(),
                NamaWali = reader["dul_nama_wali"]?.ToString(),
                HPWali = reader["dul_hp_wali"]?.ToString(),
                StatusWali = reader["dul_status_wali"]?.ToString(),
                AlamatWali = reader["dul_alamat_wali"]?.ToString()?.Replace('#', ','),
                KodePosWali = reader["dul_kodepos_wali"]?.ToString(),
                JenisMahasiswa = reader["mhs_jenis"]?.ToString(),
                JalurDaftarUlang = reader["dul_jalur"]?.ToString(),
                AtasNamaRekening = reader["atasnama"]?.ToString(),
                Norek = reader["norek"]?.ToString(),
                NamaBank = reader["namabank"]?.ToString(),
                Nisn = reader["dul_nisn"]?.ToString(),
                KelurahanId = reader["kel_id"]?.ToString(),
                Nik = reader["dul_nik"]?.ToString(),
                RFID = reader["rfid_aktif"]?.ToString(),
                KonsentrasiId = SafeGetInt16Nullable(reader, ColumnKonId),
            };
            return mahasiswa;
        }

        private static void ParseAlamatDataAsync(Mahasiswa mahasiswa)
        {
            ParseSingleAlamatAsync(mahasiswa.Alamat, (jalan, kelurahan, kecamatan, kabupaten, provinsi) =>
            {
                mahasiswa.AlamatJalan = jalan;
                mahasiswa.Kelurahan = kelurahan;
                mahasiswa.Kecamatan = kecamatan;
                mahasiswa.Kabupaten = kabupaten;
                mahasiswa.Provinsi = provinsi;
                mahasiswa.AlamatAyahJalan = jalan;
                mahasiswa.KelurahanAyah = kelurahan;
                mahasiswa.KecamatanAyah = kecamatan;
                mahasiswa.KabupatenAyah = kabupaten;
                mahasiswa.ProvinsiAyah = provinsi;
                mahasiswa.AlamatIbuJalan = jalan;
                mahasiswa.KelurahanIbu = kelurahan;
                mahasiswa.KecamatanIbu = kecamatan;
                mahasiswa.KabupatenIbu = kabupaten;
                mahasiswa.ProvinsiIbu = provinsi;
                mahasiswa.AlamatWaliJalan = jalan;
                mahasiswa.KelurahanWali = kelurahan;
                mahasiswa.KecamatanWali = kecamatan;
                mahasiswa.KabupatenWali = kabupaten;
                mahasiswa.ProvinsiWali = provinsi;
            });
        }

        private static void ParseSingleAlamatAsync(string? alamat, Action<string?, string?, string?, string?, string?> setProperties)
        {
            if (!string.IsNullOrEmpty(alamat))
            {
                alamat = alamat.Replace('#', ',');

                if (alamat.Contains(','))
                {
                    var parts = alamat.Split(',');
                    if (parts.Length >= 5)
                    {
                        setProperties(
                            parts[0].Trim(),
                            parts[1].Trim(),
                            parts[2].Trim(),
                            parts[3].Trim(),
                            parts[4].Trim()
                        );
                        return;
                    }
                }
            }

            setProperties(alamat, null, null, null, null);
        }


        private static async Task GetProfilDataAsync(SqlConnection conn, Mahasiswa mahasiswa, string mahasiswaId)
        {
            await using var cmdProfil = new SqlCommand("sia_getProfilMahasiswa", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmdProfil.Parameters.AddWithValue("@Nim", mahasiswaId);
            await using var reader = await cmdProfil.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                MapProfilData(reader, mahasiswa);
            }
        }

        private static void MapProfilData(SqlDataReader reader, Mahasiswa mahasiswa)
        {
            mahasiswa.Ttl = reader["ttl"]?.ToString();
            mahasiswa.Agama = reader["dul_agama"]?.ToString();
            mahasiswa.Kewarganegaraan = reader["dul_kewarganegaraan"]?.ToString();
            mahasiswa.GolonganDarah = reader["dul_golongan_darah"]?.ToString();
            mahasiswa.StatusKawin = reader["dul_status_kawin"]?.ToString();
            mahasiswa.UkuranSepatu = int.TryParse(reader["dul_ukuran_sepatu"]?.ToString(), out var ukuranSepatu) ? ukuranSepatu : (int?)null;
            mahasiswa.UkuranKemeja = reader["dul_ukuran_kemeja"]?.ToString();
            mahasiswa.TinggiBadan = int.TryParse(reader["dul_tinggi_badan"]?.ToString(), out var tinggiBadan) ? tinggiBadan : (int?)null;
            mahasiswa.BeratBadan = int.TryParse(reader["dul_berat_badan"]?.ToString(), out var beratBadan) ? beratBadan : (int?)null;
            mahasiswa.Sd = reader["dul_sd"]?.ToString();
            mahasiswa.SdTahunLulus = reader["dul_sd_tahun_lulus"]?.ToString();
            mahasiswa.Smp = reader["dul_smp"]?.ToString();
            mahasiswa.SmpTahunLulus = reader["dul_smp_tahun_lulus"]?.ToString();
            mahasiswa.Sma = reader["dul_sma"]?.ToString();
            mahasiswa.SmaTahunLulus = reader["dul_sma_tahun_lulus"]?.ToString();
            mahasiswa.Pt = reader["dul_pt"]?.ToString();
            mahasiswa.PtTahunLulus = reader["dul_pt_tahun_lulus"]?.ToString();
            mahasiswa.Kursus = reader["dul_kursus"]?.ToString();
            mahasiswa.Hobby = reader["dul_hobby"]?.ToString();
            mahasiswa.PengalamanKerja = reader["dul_pengalaman_kerja"]?.ToString();
            mahasiswa.Organisasi = reader["dul_organisasi"]?.ToString();
            mahasiswa.NikAyah = reader["dul_nik_ayah"]?.ToString();
            mahasiswa.KewarganegaraanAyah = reader["dul_kewarganegaraan_ayah"]?.ToString();
            mahasiswa.AgamaAyah = reader["dul_agama_ayah"]?.ToString();
            mahasiswa.PendidikanAyah = reader["dul_pendidikan_ayah"]?.ToString();
            mahasiswa.PekerjaanAyah = reader["dul_pekerjaan_ayah"]?.ToString();
            mahasiswa.PerusahaanAyah = reader["dul_perusahaan_ayah"]?.ToString();
            mahasiswa.AlamatPerusahaanAyah = reader["dul_alamat_perusahaan_ayah"]?.ToString();
            mahasiswa.PenghasilanAyah = reader["dul_penghasilan_ayah"]?.ToString();
            mahasiswa.NikIbu = reader["dul_nik_ibu"]?.ToString();
            mahasiswa.KewarganegaraanIbu = reader["dul_kewarganegaraan_ibu"]?.ToString();
            mahasiswa.AgamaIbu = reader["dul_agama_ibu"]?.ToString();
            mahasiswa.PendidikanIbu = reader["dul_pendidikan_ibu"]?.ToString();
            mahasiswa.PekerjaanIbu = reader["dul_pekerjaan_ibu"]?.ToString();
            mahasiswa.PerusahaanIbu = reader["dul_perusahaan_ibu"]?.ToString();
            mahasiswa.AlamatPerusahaanIbu = reader["dul_alamat_perusahaan_ibu"]?.ToString();
            mahasiswa.PenghasilanIbu = reader["dul_penghasilan_ibu"]?.ToString();
            mahasiswa.NikWali = reader["dul_nik_wali"]?.ToString();
            mahasiswa.KewarganegaraanWali = reader["dul_kewarganegaraan_wali"]?.ToString();
            mahasiswa.AgamaWali = reader["dul_agama_wali"]?.ToString();
            mahasiswa.PendidikanWali = reader["dul_pendidikan_wali"]?.ToString();
            mahasiswa.PekerjaanWali = reader["dul_pekerjaan_wali"]?.ToString();
            mahasiswa.PerusahaanWali = reader["dul_perusahaan_wali"]?.ToString();
            mahasiswa.AlamatPerusahaanWali = reader["dul_alamat_perusahaan_wali"]?.ToString();
            mahasiswa.PenghasilanWali = reader["dul_penghasilan_wali"]?.ToString();
            mahasiswa.JumlahSaudara = int.TryParse(reader["dul_jumlah_saudara"]?.ToString(), out var jumlahSaudara) ? jumlahSaudara : (int?)null;
            mahasiswa.JumlahKakak = int.TryParse(reader["dul_jumlah_kakak"]?.ToString(), out var jumlahKakak) ? jumlahKakak : (int?)null;
            mahasiswa.JumlahAdik = int.TryParse(reader["dul_jumlah_adik"]?.ToString(), out var jumlahAdik) ? jumlahAdik : (int?)null;
            mahasiswa.SaudaraSekolah = int.TryParse(reader["dul_saudara_sekolah"]?.ToString(), out var saudaraSekolah) ? saudaraSekolah : (int?)null;
            mahasiswa.SaudaraBekerja = int.TryParse(reader["dul_saudara_bekerja"]?.ToString(), out var saudaraBekerja) ? saudaraBekerja : (int?)null;
            mahasiswa.AstraGrup = reader["dul_astra_grup"]?.ToString();
            mahasiswa.AstraHubungan = reader["dul_astra_hubungan"]?.ToString();
            mahasiswa.AstraPerusahaan = reader["dul_astra_perusahaan"]?.ToString();
            mahasiswa.PasFoto = reader["dul_pas_foto"]?.ToString();
            mahasiswa.KtpSim = reader["dul_ktp_sim"]?.ToString();
            mahasiswa.AktaKelahiran = reader["dul_akta_kelahiran"]?.ToString();
            mahasiswa.KartuKeluarga = reader["dul_kartu_keluarga"]?.ToString();
            mahasiswa.Ijazah = reader["dul_ijazah"]?.ToString();
            mahasiswa.Skhun = reader["dul_skhun"]?.ToString();
            mahasiswa.BebasNarkoba = reader["dul_bebas_narkoba"]?.ToString();
            mahasiswa.SanggupBayar = reader["dul_sanggup_bayar"]?.ToString();
            mahasiswa.BuktiBayar = reader["dul_bukti_bayar"]?.ToString();
            mahasiswa.VaWisuda = reader["mhs_va_wisuda"]?.ToString();
            mahasiswa.VaCuti = reader["mhs_va_cuti"]?.ToString();
            mahasiswa.VaIdcard = reader["mhs_va_idcard"]?.ToString();
            mahasiswa.VaLainnya = reader["mhs_va_lainnya"]?.ToString();
            mahasiswa.VaSumbangan = reader["dul_va_sumbangan"]?.ToString();
            mahasiswa.VaSpp = reader["dul_va_spp"]?.ToString();
            mahasiswa.StatusBeasiswa = reader["statusBeasiswa"]?.ToString();
            mahasiswa.DosenAkademik = reader["mhs_dosen_akademik"]?.ToString();
            mahasiswa.Status = reader["dul_status"]?.ToString();
        }

        private static async Task GetLaporanKegiatanDataAsync(SqlConnection conn, Mahasiswa mahasiswa, string mahasiswaId)
        {
            await using var cmdLaporanKegiatan = new SqlCommand("sia_getDataLaporanKegiatan", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmdLaporanKegiatan.Parameters.AddWithValue("@Keyword", string.Empty);
            cmdLaporanKegiatan.Parameters.AddWithValue("@Status", string.Empty);
            cmdLaporanKegiatan.Parameters.AddWithValue("@OrderBy", "lke_created_date");
            cmdLaporanKegiatan.Parameters.AddWithValue(ParamHalaman, 1);
            cmdLaporanKegiatan.Parameters.AddWithValue(ParamLimit, 10);
            cmdLaporanKegiatan.Parameters.AddWithValue("@Tahun", string.Empty);
            cmdLaporanKegiatan.Parameters.AddWithValue("@IdKegiatan", string.Empty);
            cmdLaporanKegiatan.Parameters.AddWithValue("@IdMahasiswa", mahasiswaId);
            cmdLaporanKegiatan.Parameters.AddWithValue("@Role", string.Empty);

            await using var reader = await cmdLaporanKegiatan.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                mahasiswa.LaporanId = reader["lke_id"]?.ToString();
                mahasiswa.LaporanNama = reader["lke_nama"]?.ToString();
                mahasiswa.LaporanTanggalKegiatan = reader["lke_tanggal_kegiatan"]?.ToString();
                mahasiswa.LaporanPoint = SafeGetInt32(reader, ColumnLkePoint);
                mahasiswa.LaporanStatus = reader["lke_status"]?.ToString();
                mahasiswa.LaporanTanggalKegiatan = reader["lke_tahun_ajaran"]?.ToString();
                mahasiswa.KegiatanId = SafeGetInt16Nullable(reader, ColumnKegId);
                mahasiswa.LaporanTingkat = reader["lke_tingkat"]?.ToString();
            }
            await reader.CloseAsync();
        }

        private static async Task GetRfidDetailDataAsync(SqlConnection conn, Mahasiswa mahasiswa, string mahasiswaId)
        {
            await using var cmdRfidMahasiswa = new SqlCommand("sia_getDataRFID", conn);
            cmdRfidMahasiswa.CommandType = CommandType.StoredProcedure;
            cmdRfidMahasiswa.Parameters.AddWithValue(ParamIdMahasiswa, mahasiswaId);

            await using var readerRfidMahasiswa = await cmdRfidMahasiswa.ExecuteReaderAsync();

            while (await readerRfidMahasiswa.ReadAsync())
            {
                var status = readerRfidMahasiswa["rma_status"]?.ToString();

                if (!string.Equals(status, "Aktif", StringComparison.OrdinalIgnoreCase))
                    continue;

                mahasiswa.RfidMahasiswaId = SafeGetInt32(readerRfidMahasiswa, "rma_id");
                mahasiswa.IdMahasiswa = readerRfidMahasiswa["mhs_id"]?.ToString() ?? mahasiswaId;
                mahasiswa.RfidMahasiswa = readerRfidMahasiswa["rma_rfid"]?.ToString();
                mahasiswa.RfidMahasiswaStatus = status;
                mahasiswa.RfidMahasiswaCreatedBy = readerRfidMahasiswa["rma_created_by"]?.ToString();
                mahasiswa.RfidMahasiswaCreatedDate =SafeGetDateTimeNullable(readerRfidMahasiswa, "rma_created_date");
                mahasiswa.RfidMahasiswaModifBy = readerRfidMahasiswa["rma_modif_by"]?.ToString();
                mahasiswa.RfidMahasiswaModifDate = SafeGetDateTimeNullable(readerRfidMahasiswa, "rma_modif_date");

                break;
            }

            if (string.IsNullOrEmpty(mahasiswa.RfidMahasiswa))
            {
                mahasiswa.IdMahasiswa = mahasiswaId;
            }
        }


        public async Task<(IEnumerable<Mahasiswa>, int totalData)> GetAllAsync(GetAllMahasiswaRequest dto)
    {
        await using var conn = new SqlConnection(_conn);

        await using var cmd = new SqlCommand("sia_getDataMahasiswa", conn)
        {
            CommandType = CommandType.StoredProcedure
        };

        cmd.Parameters.AddWithValue("@Search", dto.SearchKeyword ?? "");
        cmd.Parameters.AddWithValue("@StatusKuliah", dto.Status ?? "");
        cmd.Parameters.AddWithValue("@OrderBy", dto.Urut ?? "mhs_id");
        cmd.Parameters.AddWithValue("@ProdiId", dto.ProdiId ?? "");
        cmd.Parameters.AddWithValue("@Angkatan", dto.Angkatan ?? "");
        cmd.Parameters.AddWithValue("@RoleId", dto.RoleId ?? "");
        cmd.Parameters.AddWithValue("@Username", dto.Username ?? "");
        cmd.Parameters.AddWithValue("@JenisMahasiswa", dto.JenisMahasiswaId ?? "");
        cmd.Parameters.AddWithValue(ParamHalaman, dto.PageNumber ?? 1);
        cmd.Parameters.AddWithValue(ParamLimit, dto.PageSize ?? 10);

        await conn.OpenAsync();

        var allData = new List<Mahasiswa>();
        int totalData = 0;

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            allData.Add(new Mahasiswa
            {
                IdMahasiswa   = reader[ColumnIdMahasiswa]?.ToString() ?? "",
                Nama          = reader[ColumnMhsNama]?.ToString() ?? "",
                Konsentrasi   = reader["kon_singkatan"]?.ToString() ?? "",
                Angkatan      = reader["mhs_angkatan"]?.ToString() ?? "",
                StatusKuliah  = reader["mhs_status_kuliah"]?.ToString() ?? "",
                JenisMahasiswa= reader["mhs_jenis"]?.ToString() ?? "",
                RFID          = reader["rfid"]?.ToString() ?? "",
                RowNumber     = SafeGetInt64(reader, ColumnRownum)
            });


            if (totalData == 0)
            {
                totalData = SafeGetInt32(reader, "Count");
            }
        }

        return (allData, totalData);
    }


        public async Task<List<Mahasiswa>> GetLaporanKegiatanMahasiswaAsync(GetLaporanKegiatanRequest request)
        {
            var result = new List<Mahasiswa>();

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getDataLaporanKegiatan", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Keyword", request.Search ?? string.Empty);
            cmd.Parameters.AddWithValue("@Status", request.Status ?? "Disetujui");
            cmd.Parameters.AddWithValue("@OrderBy", request.OrderBy ?? "lke_tanggal_kegiatan asc");
            cmd.Parameters.AddWithValue("@Tahun", request.Tahun ?? string.Empty);
            cmd.Parameters.AddWithValue("@IdKegiatan", request.KegiatanId ?? string.Empty);
            cmd.Parameters.AddWithValue("@IdMahasiswa", request.IdMahasiswa ?? string.Empty);
            cmd.Parameters.AddWithValue("@Role", request.Role ?? string.Empty);
            cmd.Parameters.AddWithValue(ParamHalaman, request.Halaman);
            cmd.Parameters.AddWithValue(ParamLimit, request.Limit);

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var item = new Mahasiswa
                {
                    RowNumber = SafeGetInt64(reader, ColumnRownum),
                    LaporanId = SafeGetString(reader, "lke_id"),
                    LaporanNama = SafeGetString(reader, "lke_nama"),
                    LaporanTanggalKegiatan = SafeGetString(reader, "lke_tanggal_kegiatan"),
                    LaporanPoint = SafeGetInt32(reader, ColumnLkePoint),
                    LaporanStatus = SafeGetString(reader, "lke_status"),
                    LaporanTahunAjaran = SafeGetString(reader, "lke_tahun_ajaran"),
                    KegiatanId = SafeGetInt16Nullable(reader, ColumnKegId) ?? 0,
                    LaporanTingkat = SafeGetString(reader, "lke_tingkat"),
                    Nama = SafeGetString(reader, ColumnMhsNama),
                    KonsentrasiId = SafeGetInt16Nullable(reader, ColumnKonId) ?? 0
                };

                result.Add(item);
            }

            return result;
        }



        public async Task<IEnumerable<string>> GetListAngkatanAktifAsync()
        {
            var list = new List<string>();

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getListAngkatanAktif", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(reader.GetString(reader.GetOrdinal("mhs_angkatan")));
            }

            return list;
        }

        public async Task<IEnumerable<(string KonsentrasiId, string KonsentrasiNama)>> GetListKonsentrasiAsync(string username, string roleId)
        {
            var list = new List<(string KonsentrasiId, string KonsentrasiNama)>();

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getListKonsentrasi", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var konId = reader[ColumnKonId]?.ToString() ?? "";
                var konNama = reader["kon_nama"]?.ToString() ?? "";
                list.Add((konId, konNama));
            }

            return list;
        }

        public async Task<IEnumerable<string>> GetListProvinsiAsync()
        {
            var list = new List<string>();

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getListProvinsi", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(reader.GetString(reader.GetOrdinal("kpo_provinsi")));
            }

            return list;
        }

        public async Task<IEnumerable<string>> GetListKabupatenAsync(string provinsi)
        {
            var list = new List<string>();

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getListKabupaten", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@Provinsi", provinsi ?? string.Empty);

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(reader.GetString(reader.GetOrdinal("kpo_kabupaten_kota")));
            }

            return list;
        }

        public async Task<IEnumerable<string>> GetListKecamatanAsync(string provinsi, string kabupaten)
        {
            var list = new List<string>();

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getListKecamatan", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@Provinsi", provinsi ?? string.Empty);
            cmd.Parameters.AddWithValue("@Kabupaten", kabupaten ?? string.Empty);

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(reader.GetString(reader.GetOrdinal("kpo_kecamatan")));
            }

            return list;
        }

        public async Task<IEnumerable<string>> GetListKelurahanAsync(string provinsi, string kabupaten, string kecamatan)
        {
            var list = new List<string>();

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getListKelurahan", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@Provinsi", provinsi ?? string.Empty);
            cmd.Parameters.AddWithValue("@Kabupaten", kabupaten ?? string.Empty);
            cmd.Parameters.AddWithValue("@Kecamatan", kecamatan ?? string.Empty);

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(reader.GetString(reader.GetOrdinal("kpo_desa_kelurahan")));
            }

            return list;
        }

        public async Task<bool> CreateRekeningAsync(string mahasiswaId, string atasNama, string norek, string namaBank, string createdBy)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_createRekening", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue(ParamIdMahasiswa, mahasiswaId ?? string.Empty);
            cmd.Parameters.AddWithValue("@AtasNama", atasNama ?? string.Empty);
            cmd.Parameters.AddWithValue("@NomorRekening", norek ?? string.Empty);
            cmd.Parameters.AddWithValue("@NamaBank", namaBank ?? string.Empty);
            cmd.Parameters.AddWithValue("@CreatedBy", createdBy ?? string.Empty);

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();

            return true;
        }

        public async Task<IEnumerable<string>> CreateNotifikasiAsync(string action, string refId, string appId, string title, string createdBy, string description)
        {
            var list = new List<string>();

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("all_createNotifikasi", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Type", action ?? string.Empty);
            cmd.Parameters.AddWithValue("@RefId", refId ?? string.Empty);
            cmd.Parameters.AddWithValue("@AppId", appId ?? string.Empty);
            cmd.Parameters.AddWithValue("@Title", title ?? string.Empty);
            cmd.Parameters.AddWithValue("@Sender", createdBy ?? string.Empty);
            cmd.Parameters.AddWithValue("@Message", description ?? string.Empty);

            cmd.Parameters.AddWithValue("@Keyword", string.Empty);
            cmd.Parameters.AddWithValue(ParamHalaman, 1);
            cmd.Parameters.AddWithValue(ParamLimit, 10);

            await conn.OpenAsync();

            await using var reader = await cmd.ExecuteReaderAsync();
            var usernameOrdinal = reader.GetOrdinal("username");

            while (await reader.ReadAsync())
            {
                if (!await reader.IsDBNullAsync(usernameOrdinal))
                {
                    list.Add(reader.GetString(usernameOrdinal));
                }
            }

            return list;
        }



        public async Task<byte[]> ExportMahasiswaExcelAsync(ExportMahasiswaRequest request, string roleId, string username)
        {
            ExcelPackage.License.SetNonCommercialOrganization("Politeknik Astra");

            using var pck = new ExcelPackage();
            var idCulture = new System.Globalization.CultureInfo("id-ID");
            string tanggalUpdate = DateTime.Now.ToString("dddd, dd MMMM yyyy | HH:mm", idCulture);

            await using var conn = new SqlConnection(_conn);
            await conn.OpenAsync();

            await using var cmd = new SqlCommand("sia_getDataMahasiswaExport", conn)
            {
                CommandType = CommandType.StoredProcedure,
                CommandTimeout = 300

            };

            cmd.Parameters.AddWithValue("@KonsentrasiId", request.ProgramStudi ?? string.Empty);
            cmd.Parameters.AddWithValue("@Angkatan", request.Angkatan ?? string.Empty);
            cmd.Parameters.AddWithValue("@JenisBeasiswa", request.JenisMahasiswa ?? string.Empty);
            cmd.Parameters.AddWithValue("@StatusKuliah", request.Status ?? string.Empty);
            cmd.Parameters.AddWithValue("@Tingkat", request.Tingkat ?? string.Empty);
            cmd.Parameters.AddWithValue("@RoleId", roleId ?? string.Empty);
            cmd.Parameters.AddWithValue("@Username", username ?? string.Empty);

            await using var reader = await cmd.ExecuteReaderAsync();

            if (!reader.HasRows)
            {
                return Array.Empty<byte>();
            }

            var ws = pck.Workbook.Worksheets.Add("Data Mahasiswa");

            ws.Cells[1, 1, 1, reader.FieldCount].Merge = true;
            ws.Cells[1, 1].Value = "Data Mahasiswa";
            ws.Cells[1, 1].Style.Font.Bold = true;
            ws.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            ws.Cells[2, 1, 2, reader.FieldCount].Merge = true;
            ws.Cells[2, 1].Value = $"(Terakhir diperbarui pada {tanggalUpdate})";
            ws.Cells[2, 1].Style.Font.Bold = true;
            ws.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            for (int i = 0; i < reader.FieldCount; i++)
            {
                var cell = ws.Cells[4, i + 1];
                cell.Value = reader.GetName(i);
                cell.Style.Font.Bold = true;
                cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                cell.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            }

            int currentRow = 5;
            while (await reader.ReadAsync())
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    var cell = ws.Cells[currentRow, i + 1];
                    cell.Value = reader[i]?.ToString() ?? string.Empty;
                    cell.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                }
                currentRow++;
            }

            if (ws.Dimension != null)
                ws.Cells[ws.Dimension.Address].AutoFitColumns();

            return await pck.GetAsByteArrayAsync();
        }

        public async Task<string> CheckLastStatusBeasiswaDOResignAsync(string mahasiswaId)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_checkLastStatusBeasiswaDOResign", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue(ParamIdMahasiswa, mahasiswaId ?? string.Empty);

            await conn.OpenAsync();
            var result = await cmd.ExecuteScalarAsync();

            return result?.ToString() ?? "Reguler";
        }

        public async Task<Mahasiswa?> GetProfilAsync(string mahasiswaId)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getProfilMahasiswa", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@Nim", mahasiswaId);

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return null;

            var mahasiswa = MapProfilMahasiswaData(reader);
            await reader.CloseAsync();
            await using var cmdRfid = new SqlCommand("sia_getDataRFID", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmdRfid.Parameters.AddWithValue(ParamIdMahasiswa, mahasiswaId);

            await using var readerRfid = await cmdRfid.ExecuteReaderAsync();

            while (await readerRfid.ReadAsync())
            {
                if (readerRfid["rma_status"]?.ToString() == "Aktif")
                {
                    mahasiswa.RFID = readerRfid["rma_rfid"]?.ToString();
                    break;
                }
            }

            return mahasiswa;
        }


        private static Mahasiswa MapProfilMahasiswaData(SqlDataReader reader)
        {
            return new Mahasiswa
            {
                Nama = reader[ColumnMhsNama]?.ToString(),
                JenisKelamin = reader["mhs_jenis_kelamin"]?.ToString(),
                TempatLahir = reader["ttl"]?.ToString()?.Split(',')[0]?.Trim(),
                Ttl = reader["ttl"]?.ToString(),
                Konsentrasi = reader["prodi"]?.ToString(),
                IdMahasiswa = reader[ColumnIdMahasiswa]?.ToString(),
                Angkatan = reader["awal"]?.ToString()?.Replace(" Ganjil", ""),
                JalurDaftarUlang = reader["dul_jalur"]?.ToString(),
                StatusKuliah = reader["mhs_status_kuliah"]?.ToString(),
                StatusBeasiswa = reader["statusBeasiswa"]?.ToString(),
                VaWisuda = reader["mhs_va_wisuda"]?.ToString(),
                VaCuti = reader["mhs_va_cuti"]?.ToString(),
                VaIdcard = reader["mhs_va_idcard"]?.ToString(),
                VaLainnya = reader["mhs_va_lainnya"]?.ToString(),
                Nik = reader["dul_nik"]?.ToString(),
                Agama = reader["dul_agama"]?.ToString(),
                Kewarganegaraan = reader["dul_kewarganegaraan"]?.ToString(),
                GolonganDarah = reader["dul_golongan_darah"]?.ToString(),
                Alamat = reader["dul_alamat"]?.ToString(),
                KodePos = reader["dul_kodepos"]?.ToString(),
                HP = reader["dul_hp"]?.ToString(),
                Email = reader["dul_email"]?.ToString(),
                StatusKawin = reader["dul_status_kawin"]?.ToString(),
                UkuranSepatu = int.TryParse(reader["dul_ukuran_sepatu"]?.ToString(), out var ukuranSepatu) ? ukuranSepatu : (int?)null,
                UkuranKemeja = reader["dul_ukuran_kemeja"]?.ToString(),
                TinggiBadan = int.TryParse(reader["dul_tinggi_badan"]?.ToString(), out var tinggiBadan) ? tinggiBadan : (int?)null,
                BeratBadan = int.TryParse(reader["dul_berat_badan"]?.ToString(), out var beratBadan) ? beratBadan : (int?)null,
                Sd = reader["dul_sd"]?.ToString(),
                SdTahunLulus = reader["dul_sd_tahun_lulus"]?.ToString(),
                Smp = reader["dul_smp"]?.ToString(),
                SmpTahunLulus = reader["dul_smp_tahun_lulus"]?.ToString(),
                Sma = reader["dul_sma"]?.ToString(),
                SmaTahunLulus = reader["dul_sma_tahun_lulus"]?.ToString(),
                Pt = reader["dul_pt"]?.ToString(),
                PtTahunLulus = reader["dul_pt_tahun_lulus"]?.ToString(),
                Kursus = reader["dul_kursus"]?.ToString(),
                Hobby = reader["dul_hobby"]?.ToString(),
                PengalamanKerja = reader["dul_pengalaman_kerja"]?.ToString(),
                Organisasi = reader["dul_organisasi"]?.ToString(),
                NamaAyah = reader["dul_nama_ayah"]?.ToString(),
                NikAyah = reader["dul_nik_ayah"]?.ToString(),
                StatusAyah = reader["dul_status_ayah"]?.ToString(),
                KewarganegaraanAyah = reader["dul_kewarganegaraan_ayah"]?.ToString(),
                AgamaAyah = reader["dul_agama_ayah"]?.ToString(),
                AlamatAyah = reader["dul_alamat_ayah"]?.ToString(),
                KodePosAyah = reader["dul_kodepos_ayah"]?.ToString(),
                HPAyah = reader["dul_hp_ayah"]?.ToString(),
                PendidikanAyah = reader["dul_pendidikan_ayah"]?.ToString(),
                PekerjaanAyah = reader["dul_pekerjaan_ayah"]?.ToString(),
                PerusahaanAyah = reader["dul_perusahaan_ayah"]?.ToString(),
                AlamatPerusahaanAyah = reader["dul_alamat_perusahaan_ayah"]?.ToString(),
                PenghasilanAyah = reader["dul_penghasilan_ayah"]?.ToString(),
                NamaIbu = reader["dul_nama_ibu"]?.ToString(),
                NikIbu = reader["dul_nik_ibu"]?.ToString(),
                StatusIbu = reader["dul_status_ibu"]?.ToString(),
                KewarganegaraanIbu = reader["dul_kewarganegaraan_ibu"]?.ToString(),
                AgamaIbu = reader["dul_agama_ibu"]?.ToString(),
                AlamatIbu = reader["dul_alamat_ibu"]?.ToString(),
                KodePosIbu = reader["dul_kodepos_ibu"]?.ToString(),
                HPIbu = reader["dul_hp_ibu"]?.ToString(),
                PendidikanIbu = reader["dul_pendidikan_ibu"]?.ToString(),
                PekerjaanIbu = reader["dul_pekerjaan_ibu"]?.ToString(),
                PerusahaanIbu = reader["dul_perusahaan_ibu"]?.ToString(),
                AlamatPerusahaanIbu = reader["dul_alamat_perusahaan_ibu"]?.ToString(),
                PenghasilanIbu = reader["dul_penghasilan_ibu"]?.ToString(),
                NamaWali = reader["dul_nama_wali"]?.ToString(),
                NikWali = reader["dul_nik_wali"]?.ToString(),
                StatusWali = reader["dul_status_wali"]?.ToString(),
                KewarganegaraanWali = reader["dul_kewarganegaraan_wali"]?.ToString(),
                AgamaWali = reader["dul_agama_wali"]?.ToString(),
                AlamatWali = reader["dul_alamat_wali"]?.ToString(),
                KodePosWali = reader["dul_kodepos_wali"]?.ToString(),
                HPWali = reader["dul_hp_wali"]?.ToString(),
                PendidikanWali = reader["dul_pendidikan_wali"]?.ToString(),
                PekerjaanWali = reader["dul_pekerjaan_wali"]?.ToString(),
                PerusahaanWali = reader["dul_perusahaan_wali"]?.ToString(),
                AlamatPerusahaanWali = reader["dul_alamat_perusahaan_wali"]?.ToString(),
                PenghasilanWali = reader["dul_penghasilan_wali"]?.ToString(),
                JumlahSaudara = int.TryParse(reader["dul_jumlah_saudara"]?.ToString(), out var jumlahSaudara) ? jumlahSaudara : (int?)null,
                JumlahKakak = int.TryParse(reader["dul_jumlah_kakak"]?.ToString(), out var jumlahKakak) ? jumlahKakak : (int?)null,
                JumlahAdik = int.TryParse(reader["dul_jumlah_adik"]?.ToString(), out var jumlahAdik) ? jumlahAdik : (int?)null,
                SaudaraSekolah = int.TryParse(reader["dul_saudara_sekolah"]?.ToString(), out var saudaraSekolah) ? saudaraSekolah : (int?)null,
                SaudaraBekerja = int.TryParse(reader["dul_saudara_bekerja"]?.ToString(), out var saudaraBekerja) ? saudaraBekerja : (int?)null,
                AstraGrup = reader["dul_astra_grup"]?.ToString(),
                AstraHubungan = reader["dul_astra_hubungan"]?.ToString(),
                AstraPerusahaan = reader["dul_astra_perusahaan"]?.ToString(),
                PasFoto = reader["dul_pas_foto"]?.ToString(),
                KtpSim = reader["dul_ktp_sim"]?.ToString(),
                AktaKelahiran = reader["dul_akta_kelahiran"]?.ToString(),
                KartuKeluarga = reader["dul_kartu_keluarga"]?.ToString(),
                Ijazah = reader["dul_ijazah"]?.ToString(),
                Skhun = reader["dul_skhun"]?.ToString(),
                BebasNarkoba = reader["dul_bebas_narkoba"]?.ToString(),
                SanggupBayar = reader["dul_sanggup_bayar"]?.ToString(),
                BuktiBayar = reader["dul_bukti_bayar"]?.ToString(),
                VaSumbangan = reader["dul_va_sumbangan"]?.ToString(),
                VaSpp = reader["dul_va_spp"]?.ToString(),
                Status = reader["dul_status"]?.ToString(),
                AtasNamaRekening = reader["atasnama"]?.ToString(),
                Norek = reader["norek"]?.ToString(),
                NamaBank = reader["namabank"]?.ToString(),
                DosenAkademik = reader["mhs_dosen_akademik"]?.ToString(),
                Nisn = reader["dul_nisn"]?.ToString()
            };
        }

        public async Task<IEnumerable<(string Semester, string Status, int Sks)>> GetDataRiwayatKuliahAsync(string mahasiswaId)
        {
            var list = new List<(string Semester, string Status, int Sks)>();

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getDataRiwayatKuliah", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue(ParamIdMahasiswaCamelCase, mahasiswaId ?? string.Empty);

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add((
                    Semester: reader.GetString(reader.GetOrdinal(ColumnSemester)),
                    Status: reader.GetString(reader.GetOrdinal("status")),
                    Sks: reader.GetInt32(reader.GetOrdinal("sks"))
                ));
            }

            return list;
        }

        public async Task<IEnumerable<(string Semester, string MataKuliahKode, string MataKuliahNama, int MataKuliahSks, string Nilai, string MataKuliahTipe, int Rownum, int JumlahSks)>> GetDataRiwayatStudiAsync(string mahasiswaId)
        {
            var list = new List<(string Semester, string MataKuliahKode, string MataKuliahNama, int MataKuliahSks, string Nilai, string MataKuliahTipe, int Rownum, int JumlahSks)>();

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getDataRiwayatStudi", conn)
            {
                CommandType = CommandType.StoredProcedure,
                CommandTimeout = 300
            };

            cmd.Parameters.AddWithValue(ParamIdMahasiswaCamelCase, mahasiswaId ?? string.Empty);

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add((
                    Semester: reader.GetString(reader.GetOrdinal(ColumnSemester)),
                    MataKuliahKode: reader.GetString(reader.GetOrdinal("mku_kode")),
                    MataKuliahNama: reader.GetString(reader.GetOrdinal("mku_nama")),
                    MataKuliahSks: reader.GetInt32(reader.GetOrdinal("mku_sks")),
                    Nilai: reader.GetString(reader.GetOrdinal("nilai")),
                    MataKuliahTipe: reader.GetString(reader.GetOrdinal("mku_tipe")),
                    Rownum: reader.GetInt32(reader.GetOrdinal(ColumnRownum)),
                    JumlahSks: reader.GetInt32(reader.GetOrdinal("jmlSKS"))
                ));
            }

            return list;
        }

        public async Task<bool> CheckCompletedKuesionerAsync(string mahasiswaId)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_checkCompletedKuesioner", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@UserId", mahasiswaId ?? string.Empty);

            await conn.OpenAsync();
            var result = await cmd.ExecuteScalarAsync();

            return result?.ToString() == "Y";
        }


        public async Task<string> CheckBebasTanggunganAsync(string mahasiswaId)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_checkBebasTanggungan", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add("@UserId", SqlDbType.VarChar, 50).Value = mahasiswaId ?? string.Empty;

            await conn.OpenAsync();

            var result = await cmd.ExecuteScalarAsync() as string;
            return string.IsNullOrWhiteSpace(result) ? "OK" : result;
        }



        public async Task<IEnumerable<(string Semester, decimal? Ip, decimal? Ipk, string Publish)>> GetDataPerformaIpAsync(string mahasiswaId)
        {
            var list = new List<(string Semester, decimal? Ip, decimal? Ipk, string Publish)>();

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getDataPerformaIP", conn)
            {
                CommandType = CommandType.StoredProcedure,
                CommandTimeout = 300
            };

            cmd.Parameters.AddWithValue(ParamIdMahasiswaCamelCase, mahasiswaId ?? string.Empty);

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add((
                    Semester: reader.GetString(reader.GetOrdinal(ColumnSemester)),
                    Ip: SafeGetDecimalNullable(reader, "ip"),
                    Ipk: SafeGetDecimalNullable(reader, "ipk"),
                    Publish: reader.GetString(reader.GetOrdinal("publish"))
                ));
            }

            return list;
        }

        public async Task<IEnumerable<(string Semester, int Persentase)>> GetDataPerformaKehadiranAsync(string mahasiswaId)
        {
            var list = new List<(string Semester, int Persentase)>();

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getDataPerformaKehadiran", conn)
            {
                CommandType = CommandType.StoredProcedure,
                CommandTimeout = 300
            };

            cmd.Parameters.AddWithValue(ParamIdMahasiswaCamelCase, mahasiswaId ?? string.Empty);

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add((
                    Semester: reader.GetString(reader.GetOrdinal(ColumnSemester)),
                    Persentase: reader.GetInt32(reader.GetOrdinal("persentase"))
                ));
            }

            return list;
        }

        public async Task<IEnumerable<(string Semester, decimal Minus)>> GetDataPerformaJamMinusAsync(string mahasiswaId)
        {
            var list = new List<(string Semester, decimal Minus)>();

            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getDataPerformaJamMinus", conn)
            {
                CommandType = CommandType.StoredProcedure,
                CommandTimeout = 300
            };

            cmd.Parameters.AddWithValue(ParamIdMahasiswa, mahasiswaId ?? string.Empty);

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add((
                    Semester: reader[ColumnSemester]?.ToString() ?? string.Empty,
                    Minus: SafeGetDecimalNullable(reader, "minus") ?? 0m
                ));
            }

            return list;
        }

        public async Task<IEnumerable<(string Semester, int Total, int Teguran, int Sp1, int Sp2, int Sp3)>> GetDataPerformaPelanggaranAsync(string mahasiswaId)
        {
            var list = new List<(string Semester, int Total, int Teguran, int Sp1, int Sp2, int Sp3)>();
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getDataPerformaPelanggaran", conn)
            {
                CommandType = CommandType.StoredProcedure,
                CommandTimeout = 300
            };

            cmd.Parameters.AddWithValue(ParamIdMahasiswa, mahasiswaId ?? string.Empty);

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add((
                    Semester: reader.GetString(reader.GetOrdinal(ColumnSemester)),
                    Total: reader.GetInt32(reader.GetOrdinal("total")),
                    Teguran: reader.GetInt32(reader.GetOrdinal("teguran")),
                    Sp1: reader.GetInt32(reader.GetOrdinal("sp1")),
                    Sp2: reader.GetInt32(reader.GetOrdinal("sp2")),
                    Sp3: reader.GetInt32(reader.GetOrdinal("sp3"))
                ));
            }

            return list;
        }

        public async Task<IEnumerable<(int Tahun, int Bulan, int TotalAbsen)>> GetDashboardAbsensiSatgasAsync(string createdBy)
        {
            var list = new List<(int Tahun, int Bulan, int TotalAbsen)>();
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getDashboardAbsensiSatgas", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@CreatedBy", createdBy ?? string.Empty);

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add((
                    Tahun: reader.GetInt32(reader.GetOrdinal("tahun")),
                    Bulan: reader.GetInt32(reader.GetOrdinal("bulan")),
                    TotalAbsen: reader.GetInt32(reader.GetOrdinal("totalabsen"))
                ));
            }

            return list;
        }

        public async Task<string?> CreateRfidMahasiswaAsync(string mahasiswaId, string rfid, string createdBy)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_createRFIDMahasiswa", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue(ParamIdMahasiswa, mahasiswaId ?? string.Empty);
            cmd.Parameters.AddWithValue("@Rfid", rfid ?? string.Empty);
            cmd.Parameters.AddWithValue("@CreatedBy", createdBy ?? string.Empty);

            await conn.OpenAsync();
            var result = await cmd.ExecuteScalarAsync();

            return result?.ToString();
        }

        public async Task<string?> SetStatusRfidAsync(string status, string rmaId)
        {
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_setStatusRFID", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Status", status);
            cmd.Parameters.AddWithValue("@RfidMahasiswaId", rmaId);

            await conn.OpenAsync();
            var result = await cmd.ExecuteScalarAsync();

            return result?.ToString();
        }


        public async Task<IEnumerable<Mahasiswa>> GetDataRfidAsync(string mahasiswaId)
        {
            var list = new List<Mahasiswa>();
            await using var conn = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("sia_getDataRFID", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue(ParamIdMahasiswa, mahasiswaId ?? string.Empty);

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(new Mahasiswa
                {
                    RfidMahasiswaId = reader.GetInt32(reader.GetOrdinal("rma_id")),
                    IdMahasiswa = reader[ColumnIdMahasiswa]?.ToString() ?? string.Empty,
                    RfidMahasiswa = reader["rma_rfid"]?.ToString() ?? string.Empty,
                    RfidMahasiswaStatus = reader["rma_status"]?.ToString() ?? string.Empty,
                    RfidMahasiswaCreatedBy = reader["rma_created_by"]?.ToString() ?? string.Empty,
                    RfidMahasiswaCreatedDate = SafeGetDateTimeNullable(reader, ColumnRmaCreatedDate),
                    RfidMahasiswaModifBy = SafeGetString(reader, "rma_modif_by"),
                    RfidMahasiswaModifDate = SafeGetDateTimeNullable(reader, ColumnRmaModifDate)
                });
            }

            return list;
        }

        public async Task<byte[]> ExportMahasiswaRFIDExcelAsync(string konsentrasiId, string urut)
        {
            ExcelPackage.License.SetNonCommercialOrganization("Politeknik Astra");

            using var pck = new ExcelPackage();
            var idCulture = new System.Globalization.CultureInfo("id-ID");
            string tanggalUpdate = DateTime.Now.ToString("dddd, dd MMMM yyyy | HH:mm", idCulture);

            await using var conn = new SqlConnection(_conn);
            await conn.OpenAsync();

            await using var cmd = new SqlCommand("sia_getDataMahasiswaExportRFID", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@KonsentrasiId", konsentrasiId ?? string.Empty);
            cmd.Parameters.AddWithValue("@Urut", urut ?? "NIM asc");

            await using var reader = await cmd.ExecuteReaderAsync();

            if (!reader.HasRows)
            {
                return Array.Empty<byte>();
            }

            var ws = pck.Workbook.Worksheets.Add("Data RFID Mahasiswa");

            ws.Cells[1, 1, 1, reader.FieldCount].Merge = true;
            ws.Cells[1, 1].Value = "Data RFID Mahasiswa";
            ws.Cells[1, 1].Style.Font.Bold = true;
            ws.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            ws.Cells[2, 1, 2, reader.FieldCount].Merge = true;
            ws.Cells[2, 1].Value = $"(Terakhir diperbarui pada {tanggalUpdate})";
            ws.Cells[2, 1].Style.Font.Bold = true;
            ws.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            for (int i = 0; i < reader.FieldCount; i++)
            {
                var cell = ws.Cells[4, i + 1];
                cell.Value = reader.GetName(i);
                cell.Style.Font.Bold = true;
                cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                cell.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            }

            int currentRow = 5;
            while (await reader.ReadAsync())
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    var cell = ws.Cells[currentRow, i + 1];
                    cell.Value = reader[i]?.ToString() ?? string.Empty;
                    cell.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                }

                var rfidValue = reader[reader.FieldCount - 1]?.ToString() ?? string.Empty;
                if (string.IsNullOrWhiteSpace(rfidValue))
                {
                    var rowRange = ws.Cells[currentRow, 1, currentRow, reader.FieldCount];
                    rowRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    rowRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow);
                }

                currentRow++;
            }

            if (ws.Dimension != null)
                ws.Cells[ws.Dimension.Address].AutoFitColumns();

            return await pck.GetAsByteArrayAsync();
        }

        private static string SafeGetString(SqlDataReader reader, string column)
        {
            var ordinal = reader.GetOrdinal(column);
            if (reader.IsDBNull(ordinal))
                return string.Empty;

            return Convert.ToString(reader.GetValue(ordinal)) ?? string.Empty;
        }

        private static short? SafeGetInt16Nullable(SqlDataReader reader, string column)
        {
            var ordinal = reader.GetOrdinal(column);
            if (reader.IsDBNull(ordinal))
                return null;

            return Convert.ToInt16(reader.GetValue(ordinal));
        }

        private static long SafeGetInt64(SqlDataReader reader, string column)
        {
            var ordinal = reader.GetOrdinal(column);
            return reader.IsDBNull(ordinal) ? 0L : reader.GetInt64(ordinal);
        }

        private static int SafeGetInt32(SqlDataReader reader, string column)
        {
            var ordinal = reader.GetOrdinal(column);
            return reader.IsDBNull(ordinal) ? 0 : reader.GetInt32(ordinal);
        }

        private static decimal? SafeGetDecimalNullable(SqlDataReader reader, string column)
        {
            var ordinal = reader.GetOrdinal(column);

            if (reader.IsDBNull(ordinal))
                return null;

            var value = reader.GetValue(ordinal);

            if (value is decimal decimall)
                return decimall;

            if (decimal.TryParse(value.ToString(), out var result))
                return result;

            return null;
        }

        private static DateTime? SafeGetDateTimeNullable(SqlDataReader reader, string column)
        {
            var ordinal = reader.GetOrdinal(column);
            return reader.IsDBNull(ordinal) ? null : reader.GetDateTime(ordinal);
        }
    }
}