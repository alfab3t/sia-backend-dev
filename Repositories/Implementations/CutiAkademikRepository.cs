using astratech_apps_backend.DTOs.CutiAkademik;
using astratech_apps_backend.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace astratech_apps_backend.Repositories.Implementations
{
    public class CutiAkademikRepository : ICutiAkademikRepository
    {
        private readonly string _conn;

        public CutiAkademikRepository(IConfiguration config)
        {
            _conn = PolmanAstraLibrary.PolmanAstraLibrary.Decrypt(
                config.GetConnectionString("DefaultConnection")!,
                Environment.GetEnvironmentVariable("DECRYPT_KEY_CONNECTION_STRING")
            );
        }

        public async Task<string?> CreateDraftAsync(CreateDraftCutiAkademikRequest dto)
        {
            using var conn = new SqlConnection(_conn);
            await conn.OpenAsync();

            var fileSP = SaveFile(dto.LampiranSuratPengajuan);
            var fileLampiran = SaveFile(dto.Lampiran);

            await using var cmd = new SqlCommand("sia_createCutiAkademik", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Step", "STEP1");
            cmd.Parameters.AddWithValue("@TahunAjaran", dto.TahunAjaran ?? "");
            cmd.Parameters.AddWithValue("@Semester", dto.Semester ?? "");
            cmd.Parameters.AddWithValue("@LampiranSuratPengajuan", fileSP ?? "");
            cmd.Parameters.AddWithValue("@Lampiran", fileLampiran ?? "");
            cmd.Parameters.AddWithValue("@MahasiswaId", dto.MhsId);
            cmd.Parameters.AddWithValue("@DraftId", "");
            cmd.Parameters.AddWithValue("@ModifiedBy", "");

            var result = await cmd.ExecuteScalarAsync();
            return result?.ToString();
        }




        public async Task<string?> GenerateIdAsync(GenerateIdFinalCutiAkademikRequest dto)
        {
            await using var conn = new SqlConnection(_conn);
            await conn.OpenAsync();
            
            await using var cmd = new SqlCommand("sia_createCutiAkademik", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Step", "STEP2");
            cmd.Parameters.AddWithValue("@TahunAjaran", "");
            cmd.Parameters.AddWithValue("@Semester", "");
            cmd.Parameters.AddWithValue("@LampiranSuratPengajuan", "");
            cmd.Parameters.AddWithValue("@Lampiran", "");
            cmd.Parameters.AddWithValue("@MahasiswaId", "");
            cmd.Parameters.AddWithValue("@DraftId", dto.DraftId);
            cmd.Parameters.AddWithValue("@ModifiedBy", dto.ModifiedBy);

            var result = await cmd.ExecuteScalarAsync();
            return result?.ToString();
        }

      

        public async Task<IEnumerable<CutiAkademikListResponse>> GetAllAsync(
            string mhsId, string status, string userId, string role, string search = "")
        {
            var result = new List<CutiAkademikListResponse>();

            await using var conn = new SqlConnection(_conn);
            await conn.OpenAsync();
            
            await using var cmd = new SqlCommand("sia_getDataCutiAkademik", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@MahasiswaId", mhsId ?? "");
            cmd.Parameters.AddWithValue("@Status", status ?? "");
            cmd.Parameters.AddWithValue("@UserId", userId ?? "");
            cmd.Parameters.AddWithValue("@Search", search ?? "");

            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                result.Add(new CutiAkademikListResponse
                {
                    Id = reader["cak_id"]?.ToString() ?? "",
                    IdDisplay = reader["id"]?.ToString() ?? "",
                    MhsId = reader["mhs_id"]?.ToString() ?? "",
                    NamaMahasiswa = reader["mhs_nama"]?.ToString() ?? "",
                    Prodi = reader["kon_nama"]?.ToString() ?? "",
                    TahunAjaran = reader["cak_tahunajaran"]?.ToString() ?? "",
                    Semester = reader["cak_semester"]?.ToString() ?? "",
                    ApproveProdi = reader["approve_prodi"]?.ToString() ?? "",
                    ApproveDir1 = reader["approve_dir1"]?.ToString() ?? "",
                    Tanggal = reader["tanggal"]?.ToString() ?? "",
                    SuratNo = reader["srt_no"]?.ToString() ?? "",
                    Status = reader["status"]?.ToString() ?? "",
                });
            }

            return result;
        }

        public async Task<CutiAkademikDetailResponse?> GetDetailAsync(string id)
        {
            await using var conn = new SqlConnection(_conn);
            await conn.OpenAsync();

            await using var cmd = new SqlCommand("sia_detailCutiAkademik", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@CutiAkademikId", id);

            await using var reader = await cmd.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
                return null;

            return new CutiAkademikDetailResponse
            {
                Id = reader["cak_id"].ToString(),
                MhsId = reader["mhs_id"].ToString(),
                Mahasiswa = reader["mhs_nama"]?.ToString() ?? "",
                Konsentrasi = reader["kon_nama"]?.ToString() ?? "",
                Angkatan = reader["mhs_angkatan"]?.ToString() ?? "",
                KonsentrasiSingkatan = reader["kon_singkatan"]?.ToString() ?? "",
                TahunAjaran = reader["cak_tahunajaran"]?.ToString() ?? "",
                Semester = reader["cak_semester"]?.ToString() ?? "",
                LampiranSP = reader["cak_lampiran_suratpengajuan"]?.ToString() ?? "",
                Lampiran = reader["cak_lampiran"]?.ToString() ?? "",
                Status = reader["cak_status"]?.ToString() ?? "",
                CreatedBy = reader["cak_created_by"]?.ToString() ?? "",
                TglPengajuan = reader["tgl"]?.ToString() ?? "",
                Sk = reader["cak_sk"]?.ToString() ?? "",
                SrtNo = reader["srt_no"]?.ToString() ?? "",
                ProdiNama = reader["pro_nama"]?.ToString() ?? "",
                Kaprodi = reader["kaprod"]?.ToString() ?? "",
                AppProdiDate = reader["cak_app_prodi_date"]?.ToString() ?? "",
                ApprovalProdi = reader["cak_approval_prodi"]?.ToString() ?? "",
                AppDir1Date = reader["cak_app_dir1_date"]?.ToString() ?? "",
                ApprovalDir1 = reader["cak_approval_dir1"]?.ToString() ?? "",
                Alamat = reader["mhs_alamat"]?.ToString() ?? "",
                Menimbang = reader["cak_menimbang"]?.ToString() ?? "",
                BulanCuti = reader["BulanCuti"]?.ToString() ?? "",
                Direktur = reader["direktur"]?.ToString() ?? "",
                Wadir1 = reader["wadir1"]?.ToString() ?? "",
                Wadir2 = reader["wadir2"]?.ToString() ?? "",
                Wadir3 = reader["wadir3"]?.ToString() ?? "",
                KodePos = reader["mhs_kodepos"]?.ToString() ?? "",
            };
        }


        public async Task<bool> UpdateAsync(string id, UpdateCutiAkademikRequest dto)
        {
            await using var conn = new SqlConnection(_conn);
            await conn.OpenAsync();

            var fileSP = SaveFile(dto.LampiranSuratPengajuan);
            var fileLampiran = SaveFile(dto.Lampiran);

            var cmd = new SqlCommand("sia_editCutiAkademik", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@CutiAkademikId", id);
            cmd.Parameters.AddWithValue("@TahunAjaran", dto.TahunAjaran ?? "");
            cmd.Parameters.AddWithValue("@Semester", dto.Semester ?? "");
            cmd.Parameters.AddWithValue("@LampiranSuratPengajuan", fileSP ?? "");
            cmd.Parameters.AddWithValue("@Lampiran", fileLampiran ?? "");
            cmd.Parameters.AddWithValue("@ModifiedBy", dto.ModifiedBy ?? "");

            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result ?? 0) > 0;
        }

        public async Task<bool> DeleteAsync(string id, string modifiedBy)
        {
            try
            {
                await using var conn = new SqlConnection(_conn);
                await using var cmd = new SqlCommand("sia_deleteCutiAkademik", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@CutiAkademikId", id);
                cmd.Parameters.AddWithValue("@ModifiedBy", modifiedBy);

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();

                return true;   
            }
            catch
            {
                return false;  
            }
        }


        public async Task<string?> CreateDraftByProdiAsync(CreateDraftCutiAkademikByProdiRequest dto)
        {
            try
            {
                await using var conn = new SqlConnection(_conn);
                await conn.OpenAsync();
                
                var fileSP = SaveFile(dto.LampiranSuratPengajuan);
                var fileLampiran = SaveFile(dto.Lampiran);

                var cmd = new SqlCommand("sia_createCutiAkademikByProdi", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@Step", "STEP1");
                cmd.Parameters.AddWithValue("@TahunAjaran", dto.TahunAjaran ?? "");
                cmd.Parameters.AddWithValue("@Semester", dto.Semester ?? "");
                cmd.Parameters.AddWithValue("@LampiranSuratPengajuan", fileSP ?? "");
                cmd.Parameters.AddWithValue("@Lampiran", fileLampiran ?? "");
                cmd.Parameters.AddWithValue("@MahasiswaId", dto.MhsId ?? "");
                cmd.Parameters.AddWithValue("@Menimbang", dto.Menimbang ?? "");
                cmd.Parameters.AddWithValue("@ApprovalProdi", dto.ApprovalProdi ?? "");
                cmd.Parameters.AddWithValue("@DraftId", ""); 
                cmd.Parameters.AddWithValue("@ModifiedBy", ""); 

                // SP sekarang return DraftId dengan SET NOCOUNT OFF
                using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return reader["DraftId"]?.ToString();
                }
                
                return null;
            }
            catch (Exception)
            {
                throw new InvalidOperationException($"Terjadi kesalahan saat membuat draft cuti akademik.");
            }
        }

        public async Task<string?> GenerateIdByProdiAsync(GenerateIdFinalCutiAkademikByProdiRequest dto)
        {
            await using var conn = new SqlConnection(_conn);
            await conn.OpenAsync();

            try
            {
                var cmd = new SqlCommand("sia_createCutiAkademikByProdi", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@Step", "STEP2");
                cmd.Parameters.AddWithValue("@TahunAjaran", ""); 
                cmd.Parameters.AddWithValue("@Semester", ""); 
                cmd.Parameters.AddWithValue("@LampiranSuratPengajuan", ""); 
                cmd.Parameters.AddWithValue("@Lampiran", ""); 
                cmd.Parameters.AddWithValue("@MahasiswaId", ""); 
                cmd.Parameters.AddWithValue("@Menimbang", ""); 
                cmd.Parameters.AddWithValue("@ApprovalProdi", ""); 
                cmd.Parameters.AddWithValue("@DraftId", dto.DraftId ?? "");
                cmd.Parameters.AddWithValue("@ModifiedBy", dto.ModifiedBy ?? "");

                // SP sekarang return OfficialId dengan SET NOCOUNT OFF
                using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return reader["OfficialId"]?.ToString();
                }
                
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<CutiAkademikListResponse>> GetRiwayatAsync(
    string userId, string status, string search)
        {
            var result = new List<CutiAkademikListResponse>();

            await using var conn = new SqlConnection(_conn);
            await conn.OpenAsync();
            
            await using var cmd = new SqlCommand("sia_getDataRiwayatCutiAkademik", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@UserId", userId ?? "");
            cmd.Parameters.AddWithValue("@Status", status ?? "");
            cmd.Parameters.AddWithValue("@Search", search ?? "");

            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var statusValue = reader["status"]?.ToString() ?? "";
                
                // Filter out Draft status from riwayat
                if (statusValue.Equals("Draft", StringComparison.OrdinalIgnoreCase))
                    continue;

                result.Add(new CutiAkademikListResponse
                {
                    Id = reader["cak_id"]?.ToString() ?? "",
                    IdDisplay = reader["id"]?.ToString() ?? "",
                    MhsId = reader["mhs_id"]?.ToString() ?? "",
                    NamaMahasiswa = reader["mhs_nama"]?.ToString() ?? "",
                    Prodi = reader["kon_nama"]?.ToString() ?? "",
                    TahunAjaran = reader["cak_tahunajaran"]?.ToString() ?? "",
                    Semester = reader["cak_semester"]?.ToString() ?? "",
                    ApproveProdi = reader["approve_prodi"]?.ToString() ?? "",
                    ApproveDir1 = reader["approve_dir1"]?.ToString() ?? "",
                    Tanggal = reader["tanggal"]?.ToString() ?? "",
                    SuratNo = reader["srt_no"]?.ToString() ?? "",
                    Status = statusValue
                });
            }

            return result;
        }

        public async Task<IEnumerable<CutiAkademikRiwayatExcelResponse>> GetRiwayatExcelAsync(string userId)
        {
            var result = new List<CutiAkademikRiwayatExcelResponse>();

            await using var conn = new SqlConnection(_conn);
            await conn.OpenAsync();
            
            await using var cmd = new SqlCommand("sia_getDataRiwayatCutiAkademikExcel", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@UserId", userId ?? "");

            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                result.Add(new CutiAkademikRiwayatExcelResponse
                {
                    NIM = reader["NIM"]?.ToString() ?? "",
                    NamaMahasiswa = reader["Nama Mahasiswa"]?.ToString() ?? "",
                    Konsentrasi = reader["Konsentrasi"]?.ToString() ?? "",
                    TanggalPengajuan = reader["Tanggal Pengajuan"]?.ToString() ?? "",
                    NoSK = reader["No SK"]?.ToString() ?? "",
                    NoPengajuan = reader["No Pengajuan"]?.ToString() ?? ""
                });
            }

            return result;
        }

        private string? SaveFile(IFormFile? file)
        {
            if (file == null || file.Length == 0)
                return null;

            var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/cuti");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(folder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            return fileName;
        }

       
        public async Task<bool> ApproveCutiAsync(ApproveCutiAkademikRequest dto)
        {
            try
            {
                await using var conn = new SqlConnection(_conn);
                await conn.OpenAsync();

                var cmd = new SqlCommand("sia_setujuiCutiAkademik", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@CutiAkademikId", dto.Id);
                cmd.Parameters.AddWithValue("@Role", dto.Role);
                cmd.Parameters.AddWithValue("@ApprovedBy", dto.ApprovedBy);

                using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    var success = Convert.ToBoolean(reader["Success"]);
                    var message = reader["Message"]?.ToString() ?? "";
                    var newStatus = reader["NewStatus"]?.ToString() ?? "";
                    
                    if (!success)
                    {
                        throw new InvalidOperationException($"{message}");
                    }
                    
                    return success;
                }

                throw new InvalidOperationException("SP tidak mengembalikan result set");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"{ex.Message}");
            }
        }

        public async Task<bool> ApproveProdiCutiAsync(ApproveCutiAkademikByProdiRequest dto)
        {
            await using var conn = new SqlConnection(_conn);
            await conn.OpenAsync();

            var cmd = new SqlCommand("sia_setujuiCutiAkademikProdi", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@CutiAkademikId", dto.Id);
            cmd.Parameters.AddWithValue("@Menimbang", dto.Menimbang ?? "");
            cmd.Parameters.AddWithValue("@ApprovedBy", dto.ApprovedBy);

            var rowsAffected = await cmd.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

       
        public async Task<bool> RejectCutiAsync(RejectCutiAkademikRequest dto)
        {
            try
            {
                await using var conn = new SqlConnection(_conn);
                await conn.OpenAsync();

                // Detect user role first
                var userRole = await DetectUserRoleAsync(dto.Username);
                
                var cmd = new SqlCommand("sia_tolakCutiAkademik", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@CutiAkademikId", dto.Id);
                cmd.Parameters.AddWithValue("@Role", userRole); // Role yang sudah di-detect
                cmd.Parameters.AddWithValue("@Keterangan", ""); // Kosong untuk sementara

                await cmd.ExecuteNonQueryAsync();
                
                return true;
            }
            catch (Exception)
            {
                throw new InvalidOperationException("Terjadi kesalahan saat menolak cuti akademik.");
            }
        }

        public async Task<bool> UploadSKAsync(UploadSKCutiAkademikRequest dto)
        {
            try
            {
                await using var conn = new SqlConnection(_conn);
                await conn.OpenAsync();

                var fileName = SaveFile(dto.FileSK);
                if (string.IsNullOrEmpty(fileName))
                    throw new InvalidOperationException("Gagal menyimpan file SK");

                var cmd = new SqlCommand("sia_createSKCutiAkademik", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@CutiAkademikId", dto.Id);
                cmd.Parameters.AddWithValue("@NomorSK", fileName);
                cmd.Parameters.AddWithValue("@ModifiedBy", dto.UploadBy);

                var rowsAffected = await cmd.ExecuteNonQueryAsync();
                
                if (rowsAffected == 0)
                    throw new InvalidOperationException($"Stored procedure tidak mengupdate record. ID: {dto.Id}");
                
                return true;
            }
            catch (Exception ex)
            {
                // Log the actual error for debugging
                throw new InvalidOperationException($"Error in UploadSKAsync: {ex.Message}", ex);
            }
        }

        public async Task<string> DetectUserRoleAsync(string username)  
        {
            try
            {
                await using var conn = new SqlConnection(_conn);
                await conn.OpenAsync();
                
                await using var cmd = new SqlCommand("all_getIdentityByUser", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@UsernameToFind", username);
                await using var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    var rolId = reader["rol_id"]?.ToString() ?? "";
                    var jabMainId = reader["jab_main_id"]?.ToString() ?? "";
                    
                    // Mapping rol_id ke role name untuk SP approval
                    return rolId switch
                    {
                        "ROL999" => "wadir1",
                        "ROL71" => "prodi", 
                        _ when username.Contains("finance", StringComparison.OrdinalIgnoreCase) => "finance",
                        _ when jabMainId == "4" => "wadir1",  // fallback jika rol_id kosong
                        _ when jabMainId == "6" => "prodi",   // fallback jika rol_id kosong
                        _ => ""
                    };
                }
                
                return "";
            }
            catch (Exception)
            {
                return "";
            }
        }

        public async Task<MahasiswaDetailDto?> GetDetailMahasiswaAsync(string mahasiswaId)
        {
            try
            {
                await using var conn = new SqlConnection(_conn);
                await conn.OpenAsync();
                
                await using var cmd = new SqlCommand("sia_detailMahasiswa", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@MahasiswaId", mahasiswaId);
                await using var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new MahasiswaDetailDto
                    {
                        MhsId = reader["mhs_id"]?.ToString() ?? "",
                        DulNoPendaftaran = reader["dul_no_pendaftaran"]?.ToString() ?? "",
                        MhsNama = reader["mhs_nama"]?.ToString() ?? "",
                        KonNama = reader["kon_nama"]?.ToString() ?? "",
                        MhsTempatLahir = reader["mhs_tempat_lahir"]?.ToString() ?? "",
                        MhsTglLahir = reader["mhs_tgl_lahir"]?.ToString() ?? "",
                        MhsJenisKelamin = reader["mhs_jenis_kelamin"]?.ToString() ?? "",
                        MhsAlamat = reader["mhs_alamat"]?.ToString() ?? "",
                        MhsKodepos = reader["mhs_kodepos"]?.ToString() ?? "",
                        MhsHp = reader["mhs_hp"]?.ToString() ?? "",
                        MhsEmail = reader["mhs_email"]?.ToString() ?? "",
                        MhsTglMasuk = reader["mhs_tgl_masuk"]?.ToString() ?? "",
                        MhsTglLulus = reader["mhs_tgl_lulus"]?.ToString() ?? "",
                        MhsAngkatan = reader["mhs_angkatan"]?.ToString() ?? "",
                        MhsStatusKuliah = reader["mhs_status_kuliah"]?.ToString() ?? "",
                        DulNamaAyah = reader["dul_nama_ayah"]?.ToString() ?? "",
                        DulHpAyah = reader["dul_hp_ayah"]?.ToString() ?? "",
                        DulStatusAyah = reader["dul_status_ayah"]?.ToString() ?? "",
                        DulAlamatAyah = reader["dul_alamat_ayah"]?.ToString() ?? "",
                        DulKodeposAyah = reader["dul_kodepos_ayah"]?.ToString() ?? "",
                        DulNamaIbu = reader["dul_nama_ibu"]?.ToString() ?? "",
                        DulHpIbu = reader["dul_hp_ibu"]?.ToString() ?? "",
                        DulStatusIbu = reader["dul_status_ibu"]?.ToString() ?? "",
                        DulAlamatIbu = reader["dul_alamat_ibu"]?.ToString() ?? "",
                        DulKodeposIbu = reader["dul_kodepos_ibu"]?.ToString() ?? "",
                        DulNamaWali = reader["dul_nama_wali"]?.ToString() ?? "",
                        DulHpWali = reader["dul_hp_wali"]?.ToString() ?? "",
                        DulStatusWali = reader["dul_status_wali"]?.ToString() ?? "",
                        DulAlamatWali = reader["dul_alamat_wali"]?.ToString() ?? "",
                        DulKodeposWali = reader["dul_kodepos_wali"]?.ToString() ?? "",
                        KonId = SafeConvertToInt(reader["kon_id"]),
                        MhsJenis = reader["mhs_jenis"]?.ToString() ?? "",
                        DulJalur = reader["dul_jalur"]?.ToString() ?? "",
                        AtasNama = reader["atasnama"]?.ToString() ?? "",
                        NoRek = reader["norek"]?.ToString() ?? "",
                        NamaBank = reader["namabank"]?.ToString() ?? "",
                        DulNisn = reader["dul_nisn"]?.ToString() ?? "",
                        KelId = reader["kel_id"]?.ToString() ?? "",
                        DulNik = reader["dul_nik"]?.ToString() ?? "",
                        RfidAktif = reader["rfid_aktif"]?.ToString() ?? ""
                    };
                }
                
                return null;
            }
            catch (Exception ex)
            {
                // Log error untuk debugging
                throw new InvalidOperationException($"Error in GetDetailMahasiswaAsync: {ex.Message}");
            }
        }

        public async Task<BebasTanggunganDto?> CheckBebasTanggunganAsync(string userId)
        {
            try
            {
                await using var conn = new SqlConnection(_conn);
                await conn.OpenAsync();
                
                await using var cmd = new SqlCommand("sia_checkBebasTanggungan", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@UserId", userId);
                await using var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new BebasTanggunganDto
                    {
                        Status = reader["status"]?.ToString() ?? ""
                    };
                }
                
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<ProfilMahasiswaDto?> GetProfilMahasiswaAsync(string nim)
        {
            try
            {
                await using var conn = new SqlConnection(_conn);
                await conn.OpenAsync();
                
                await using var cmd = new SqlCommand("sia_getProfilMahasiswa", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@Nim", nim);
                await using var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new ProfilMahasiswaDto
                    {
                        MhsNama = reader["mhs_nama"]?.ToString() ?? "",
                        MhsJenisKelamin = reader["mhs_jenis_kelamin"]?.ToString() ?? "",
                        Ttl = reader["ttl"]?.ToString() ?? "",
                        Prodi = reader["prodi"]?.ToString() ?? "",
                        MhsId = reader["mhs_id"]?.ToString() ?? "",
                        Awal = reader["awal"]?.ToString() ?? "",
                        DulJalur = reader["dul_jalur"]?.ToString() ?? "",
                        MhsStatusKuliah = reader["mhs_status_kuliah"]?.ToString() ?? "",
                        StatusBeasiswa = reader["statusBeasiswa"]?.ToString() ?? "",
                        MhsVaWisuda = reader["mhs_va_wisuda"]?.ToString() ?? "",
                        MhsVaCuti = reader["mhs_va_cuti"]?.ToString() ?? "",
                        MhsVaIdcard = reader["mhs_va_idcard"]?.ToString() ?? "",
                        MhsVaLainnya = reader["mhs_va_lainnya"]?.ToString() ?? "",
                        DulNik = reader["dul_nik"]?.ToString() ?? "",
                        DulAgama = reader["dul_agama"]?.ToString() ?? "",
                        DulKewarganegaraan = reader["dul_kewarganegaraan"]?.ToString() ?? "",
                        DulGolonganDarah = reader["dul_golongan_darah"]?.ToString() ?? "",
                        DulAlamat = reader["dul_alamat"]?.ToString() ?? "",
                        DulKodepos = reader["dul_kodepos"]?.ToString() ?? "",
                        DulHp = reader["dul_hp"]?.ToString() ?? "",
                        DulSd = reader["dul_sd"]?.ToString() ?? "",
                        DulSdTahunLulus = reader["dul_sd_tahun_lulus"]?.ToString() ?? "",
                        DulSmp = reader["dul_smp"]?.ToString() ?? "",
                        DulSmpTahunLulus = reader["dul_smp_tahun_lulus"]?.ToString() ?? "",
                        DulSma = reader["dul_sma"]?.ToString() ?? "",
                        DulSmaTahunLulus = reader["dul_sma_tahun_lulus"]?.ToString() ?? "",
                        DulPt = reader["dul_pt"]?.ToString() ?? "",
                        DulPtTahunLulus = reader["dul_pt_tahun_lulus"]?.ToString() ?? "",
                        DulKursus = reader["dul_kursus"]?.ToString() ?? "",
                        DulHobby = reader["dul_hobby"]?.ToString() ?? "",
                        DulPengalamanKerja = reader["dul_pengalaman_kerja"]?.ToString() ?? "",
                        DulOrganisasi = reader["dul_organisasi"]?.ToString() ?? "",
                        DulStatusKawin = reader["dul_status_kawin"]?.ToString() ?? "",
                        DulUkuranSepatu = reader["dul_ukuran_sepatu"]?.ToString() ?? "",
                        DulUkuranKemeja = reader["dul_ukuran_kemeja"]?.ToString() ?? "",
                        DulTinggiBadan = reader["dul_tinggi_badan"]?.ToString() ?? "",
                        DulBeratBadan = reader["dul_berat_badan"]?.ToString() ?? "",
                        DulNamaAyah = reader["dul_nama_ayah"]?.ToString() ?? "",
                        DulNikAyah = reader["dul_nik_ayah"]?.ToString() ?? "",
                        DulStatusAyah = reader["dul_status_ayah"]?.ToString() ?? "",
                        DulKewarganegaraanAyah = reader["dul_kewarganegaraan_ayah"]?.ToString() ?? "",
                        DulAgamaAyah = reader["dul_agama_ayah"]?.ToString() ?? "",
                        DulAlamatAyah = reader["dul_alamat_ayah"]?.ToString() ?? "",
                        DulKodeposAyah = reader["dul_kodepos_ayah"]?.ToString() ?? "",
                        DulHpAyah = reader["dul_hp_ayah"]?.ToString() ?? "",
                        DulPendidikanAyah = reader["dul_pendidikan_ayah"]?.ToString() ?? "",
                        DulPekerjaanAyah = reader["dul_pekerjaan_ayah"]?.ToString() ?? "",
                        DulPerusahaanAyah = reader["dul_perusahaan_ayah"]?.ToString() ?? "",
                        DulAlamatPerusahaanAyah = reader["dul_alamat_perusahaan_ayah"]?.ToString() ?? "",
                        DulPenghasilanAyah = reader["dul_penghasilan_ayah"]?.ToString() ?? "",
                        DulNamaIbu = reader["dul_nama_ibu"]?.ToString() ?? "",
                        DulNikIbu = reader["dul_nik_ibu"]?.ToString() ?? "",
                        DulStatusIbu = reader["dul_status_ibu"]?.ToString() ?? "",
                        DulKewarganegaraanIbu = reader["dul_kewarganegaraan_ibu"]?.ToString() ?? "",
                        DulAgamaIbu = reader["dul_agama_ibu"]?.ToString() ?? "",
                        DulAlamatIbu = reader["dul_alamat_ibu"]?.ToString() ?? "",
                        DulKodeposIbu = reader["dul_kodepos_ibu"]?.ToString() ?? "",
                        DulHpIbu = reader["dul_hp_ibu"]?.ToString() ?? "",
                        DulPendidikanIbu = reader["dul_pendidikan_ibu"]?.ToString() ?? "",
                        DulPekerjaanIbu = reader["dul_pekerjaan_ibu"]?.ToString() ?? "",
                        DulPerusahaanIbu = reader["dul_perusahaan_ibu"]?.ToString() ?? "",
                        DulAlamatPerusahaanIbu = reader["dul_alamat_perusahaan_ibu"]?.ToString() ?? "",
                        DulPenghasilanIbu = reader["dul_penghasilan_ibu"]?.ToString() ?? "",
                        DulNamaWali = reader["dul_nama_wali"]?.ToString() ?? "",
                        DulNikWali = reader["dul_nik_wali"]?.ToString() ?? "",
                        DulStatusWali = reader["dul_status_wali"]?.ToString() ?? "",
                        DulKewarganegaraanWali = reader["dul_kewarganegaraan_wali"]?.ToString() ?? "",
                        DulAgamaWali = reader["dul_agama_wali"]?.ToString() ?? "",
                        DulAlamatWali = reader["dul_alamat_wali"]?.ToString() ?? "",
                        DulKodeposWali = reader["dul_kodepos_wali"]?.ToString() ?? "",
                        DulHpWali = reader["dul_hp_wali"]?.ToString() ?? "",
                        DulPendidikanWali = reader["dul_pendidikan_wali"]?.ToString() ?? "",
                        DulPekerjaanWali = reader["dul_pekerjaan_wali"]?.ToString() ?? "",
                        DulPerusahaanWali = reader["dul_perusahaan_wali"]?.ToString() ?? "",
                        DulAlamatPerusahaanWali = reader["dul_alamat_perusahaan_wali"]?.ToString() ?? "",
                        DulPenghasilanWali = reader["dul_penghasilan_wali"]?.ToString() ?? "",
                        DulJumlahSaudara = reader["dul_jumlah_saudara"]?.ToString() ?? "",
                        DulJumlahKakak = reader["dul_jumlah_kakak"]?.ToString() ?? "",
                        DulJumlahAdik = reader["dul_jumlah_adik"]?.ToString() ?? "",
                        DulSaudaraSekolah = reader["dul_saudara_sekolah"]?.ToString() ?? "",
                        DulSaudaraBekerja = reader["dul_saudara_bekerja"]?.ToString() ?? "",
                        DulAstraGrup = reader["dul_astra_grup"]?.ToString() ?? "",
                        DulAstraHubungan = reader["dul_astra_hubungan"]?.ToString() ?? "",
                        DulAstraPerusahaan = reader["dul_astra_perusahaan"]?.ToString() ?? "",
                        DulPasFoto = reader["dul_pas_foto"]?.ToString() ?? "",
                        DulKtpSim = reader["dul_ktp_sim"]?.ToString() ?? "",
                        DulAktaKelahiran = reader["dul_akta_kelahiran"]?.ToString() ?? "",
                        DulKartuKeluarga = reader["dul_kartu_keluarga"]?.ToString() ?? "",
                        DulIjazah = reader["dul_ijazah"]?.ToString() ?? "",
                        DulSkhun = reader["dul_skhun"]?.ToString() ?? "",
                        DulBebasNarkoba = reader["dul_bebas_narkoba"]?.ToString() ?? "",
                        DulSanggupBayar = reader["dul_sanggup_bayar"]?.ToString() ?? "",
                        DulBuktiBayar = reader["dul_bukti_bayar"]?.ToString() ?? "",
                        DulEmail = reader["dul_email"]?.ToString() ?? "",
                        DulVaSumbangan = reader["dul_va_sumbangan"]?.ToString() ?? "",
                        DulVaSpp = reader["dul_va_spp"]?.ToString() ?? "",
                        DulStatus = reader["dul_status"]?.ToString() ?? "",
                        AtasNama = reader["atasnama"]?.ToString() ?? "",
                        NoRek = reader["norek"]?.ToString() ?? "",
                        NamaBank = reader["namabank"]?.ToString() ?? "",
                        MhsDosenAkademik = reader["mhs_dosen_akademik"]?.ToString() ?? "",
                        DulNisn = reader["dul_nisn"]?.ToString() ?? ""
                    };
                }
                
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<IEnumerable<KonsentrasiDropdownDto>> GetKonsentrasiBySekprodAsync(string username)
        {
            var result = new List<KonsentrasiDropdownDto>();
            
            try
            {
                await using var conn = new SqlConnection(_conn);
                await conn.OpenAsync();
                
                await using var cmd = new SqlCommand("sia_getListKonsentrasiBySekprod", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                // SP membutuhkan 50 parameter, kita isi parameter pertama dengan username
                cmd.Parameters.AddWithValue("@p1", username);
                for (int i = 2; i <= 50; i++)
                {
                    cmd.Parameters.AddWithValue($"@p{i}", "");
                }

                await using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    result.Add(new KonsentrasiDropdownDto
                    {
                        Id = SafeConvertToInt(reader["id"]),
                        Nama = reader["nama"]?.ToString() ?? ""
                    });
                }
            }
            catch (Exception)
            {
                // Return empty list on error
            }
            
            return result;
        }

        public async Task<MahasiswaByNimDto?> GetMahasiswaByNimAsync(string nim)
        {
            try
            {
                await using var conn = new SqlConnection(_conn);
                await conn.OpenAsync();
                
                await using var cmd = new SqlCommand("sia_getMahasiswaByNIM", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@Id", nim);
                await using var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new MahasiswaByNimDto
                    {
                        MhsNama = reader["mhs_nama"]?.ToString() ?? "",
                        KonNama = reader["kon_nama"]?.ToString() ?? "",
                        MhsAngkatan = reader["mhs_angkatan"]?.ToString() ?? "",
                        Kelas = reader["kelas"]?.ToString() ?? ""
                    };
                }
                
                return null;
            }
            catch (Exception ex)
            {
                // Log error untuk debugging
                throw new InvalidOperationException($"Error in GetMahasiswaByNimAsync: {ex.Message}");
            }
        }

        private static int SafeConvertToInt(object value)
        {
            if (value == null || value == DBNull.Value)
                return 0;
                
            var stringValue = value.ToString();
            if (string.IsNullOrEmpty(stringValue))
                return 0;
                
            if (int.TryParse(stringValue, out int result))
                return result;
                
            return 0; // Return 0 if conversion fails
        }

        public async Task<IEnumerable<MahasiswaByKonsentrasiDto>> GetMahasiswaByKonsentrasiAsync(string username)
        {
            var result = new List<MahasiswaByKonsentrasiDto>();
            
            try
            {
                await using var conn = new SqlConnection(_conn);
                await conn.OpenAsync();
                
                await using var cmd = new SqlCommand("sia_getListMahasiswaByKonsentrasi", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@Id", username);
                await using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var fullName = reader["mhs_nama"]?.ToString() ?? "";
                    
                    // Extract nama saja dari format "NIM - NAMA"
                    var namaSaja = fullName;
                    if (fullName.Contains(" - "))
                    {
                        var parts = fullName.Split(" - ", 2);
                        if (parts.Length > 1)
                        {
                            namaSaja = parts[1].Trim();
                        }
                    }
                    
                    result.Add(new MahasiswaByKonsentrasiDto
                    {
                        MhsId = reader["mhs_id"]?.ToString() ?? "",
                        MhsNama = namaSaja
                    });
                }
            }
            catch (Exception)
            {
                // Return empty list on error
            }
            
            return result;
        }

            
    }
}
