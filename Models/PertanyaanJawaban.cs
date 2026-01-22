using astratech_apps_backend.Models;
using System;

namespace astratech_apps_backend.Models
{
    public class PertanyaanJawaban
    {
        public int Id { get; set; }
        public string PertanyaanText { get; set; } = string.Empty;
        public bool IsHeader { get; set; }
        public string Jenis { get; set; } = string.Empty;
        public string? Jawaban { get; set; }
    }
}   