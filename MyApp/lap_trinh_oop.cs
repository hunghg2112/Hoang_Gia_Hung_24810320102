using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoSpeedLogistics
{
    // ==========================================
    // 1. CLASS TRỪU TƯỢNG (ABSTRACTION & ENCAPSULATION)
    // ==========================================
    public abstract class PhuongTien
    {
        // Private Fields
        private string _maPT;
        private string _tenHang;
        private int _namSanXuat;
        private decimal _giaGoc;

        // Properties (Validation)
        public string MaPT
        {
            get => _maPT;
            set => _maPT = string.IsNullOrWhiteSpace(value) ? "PT000" : value.Trim();
        }

        public string TenHang
        {
            get => _tenHang;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Tên hãng không được để trống!");
                _tenHang = value.Trim();
            }
        }

        public int NamSanXuat
        {
            get => _namSanXuat;
            set
            {
                if (value < 1900 || value > DateTime.Now.Year)
                    throw new ArgumentException("Năm sản xuất không hợp lệ!");
                _namSanXuat = value;
            }
        }

        public decimal GiaGoc
        {
            get => _giaGoc;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Giá gốc phải lớn hơn 0!");
                _giaGoc = value;
            }
        }

        // Constructor
        public PhuongTien(string maPT, string tenHang, int namSanXuat, decimal giaGoc)
        {
            MaPT = maPT;
            TenHang = tenHang;
            NamSanXuat = namSanXuat;
            GiaGoc = giaGoc;
        }

        // Abstract Method
        public abstract decimal TinhGiaLanBanh();

        // Virtual Method
        public virtual string GetInfo()
        {
            return $"Mã: {MaPT} | Hãng: {TenHang} | Năm SX: {NamSanXuat} | Giá gốc: {GiaGoc:N0} VNĐ";
        }
    }

    // ==========================================
    // 2. CLASS Ô TÔ (INHERITANCE & POLYMORPHISM)
    // ==========================================
    public class OTo : PhuongTien
    {
        private int _soChoNgoi;
        private double _dungTichDongCo;

        public int SoChoNgoi
        {
            get => _soChoNgoi;
            set => _soChoNgoi = value > 0 ? value : throw new ArgumentException("Số chỗ ngồi phải > 0");
        }

        public double DungTichDongCo
        {
            get => _dungTichDongCo;
            set => _dungTichDongCo = value > 0 ? value : throw new ArgumentException("Dung tích động cơ phải > 0");
        }

        public OTo(string maPT, string tenHang, int namSanXuat, decimal giaGoc, int soChoNgoi, double dungTichDongCo) 
            : base(maPT, tenHang, namSanXuat, giaGoc)
        {
            SoChoNgoi = soChoNgoi;
            DungTichDongCo = dungTichDongCo;
        }

        public override decimal TinhGiaLanBanh()
        {
            if (SoChoNgoi <= 9)
                return GiaGoc + (GiaGoc * 0.12m) + (GiaGoc * 0.30m); // Lệ phí 12%, TTĐB 30%
            else
                return GiaGoc + (GiaGoc * 0.10m); // Lệ phí 10%
        }

        public override string GetInfo()
        {
            return base.GetInfo() + $" | Số chỗ: {SoChoNgoi} | Động cơ: {DungTichDongCo}L";
        }
    }

    // ==========================================
    // 3. CLASS XE MÁY (INHERITANCE & POLYMORPHISM)
    // ==========================================
    public class XeMay : PhuongTien
    {
        private int _dungTichXylanh;

        public int DungTichXylanh
        {
            get => _dungTichXylanh;
            set => _dungTichXylanh = value > 0 ? value : throw new ArgumentException("Dung tích xi lanh phải > 0");
        }

        public XeMay(string maPT, string tenHang, int namSanXuat, decimal giaGoc, int dungTichXylanh) 
            : base(maPT, tenHang, namSanXuat, giaGoc)
        {
            DungTichXylanh = dungTichXylanh;
        }

        public override decimal TinhGiaLanBanh()
        {
            if (DungTichXylanh < 175)
                return GiaGoc + (GiaGoc * 0.02m); // Trước bạ 2%
            else
                return GiaGoc + (GiaGoc * 0.05m); // Trước bạ 5%
        }

        public override string GetInfo()
        {
            return base.GetInfo() + $" | Phân khối: {DungTichXylanh}cc";
        }
    }

    // ==========================================
    // 4. CLASS QUẢN LÝ
    // ==========================================
    public class QuanLyPhuongTien
    {
        private List<PhuongTien> _danhSach = new List<PhuongTien>();

        public void AddPhuongTien(PhuongTien pt)
        {
            _danhSach.Add(pt);
        }

        public void DisplayAll()
        {
            foreach (var pt in _danhSach)
            {
                Console.WriteLine(pt.GetInfo());
                Console.WriteLine($"   => Giá lăn bánh: {pt.TinhGiaLanBanh():N0} VNĐ\n");
            }
        }

        public PhuongTien FindMaxGiaLanBanh()
        {
            // LINQ để tìm phần tử có giá lăn bánh lớn nhất
            return _danhSach.OrderByDescending(pt => pt.TinhGiaLanBanh()).FirstOrDefault();
        }

        public List<PhuongTien> SearchByName(string keyword)
        {
            // LINQ: StringComparison.OrdinalIgnoreCase giúp tìm kiếm không phân biệt hoa/thường
            return _danhSach.Where(pt => pt.TenHang.Contains(keyword, StringComparison.OrdinalIgnoreCase)).ToList();
        }
    }

    // ==========================================
    // 5. CHƯƠNG TRÌNH CHÍNH (TEST CASES)
    // ==========================================
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            QuanLyPhuongTien qlpt = new QuanLyPhuongTien();

            Console.WriteLine("--- BẮT ĐẦU TEST CASES ---\n");

            // [TC01] - Validation Năm sản xuất (Ngoại lệ)
            Console.WriteLine("[TC01] KIỂM TRA VALIDATION NĂM SẢN XUẤT");
            try
            {
                OTo otoLoi = new OTo("PT001", "Ford", 1850, 1000000000m, 5, 2.0);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Kết quả TC01 (Thành công): Ném ra lỗi -> {ex.Message}\n");
            }

            // Tạo dữ liệu chuẩn cho các Test Case sau
            OTo oto5Cho = new OTo("PT002", "Toyota Camry", 2023, 1000000000m, 5, 2.5);
            XeMay xemay150 = new XeMay("PT003", "Honda Winner X", 2023, 50000000m, 150);

            // [TC02] - Tính giá lăn bánh Ô tô (1 tỷ -> 1.42 tỷ)
            Console.WriteLine("[TC02] KIỂM TRA TÍNH GIÁ LĂN BÁNH Ô TÔ");
            Console.WriteLine($"Kỳ vọng: 1,420,000,000 | Thực tế: {oto5Cho.TinhGiaLanBanh():N0} VNĐ\n");

            // [TC03] - Tính giá lăn bánh Xe máy (50 triệu -> 51 triệu)
            Console.WriteLine("[TC03] KIỂM TRA TÍNH GIÁ LĂN BÁNH XE MÁY");
            Console.WriteLine($"Kỳ vọng: 51,000,000 | Thực tế: {xemay150.TinhGiaLanBanh():N0} VNĐ\n");

            // [TC04] - Kiểm tra tính Đa hình
            Console.WriteLine("[TC04] KIỂM TRA ĐA HÌNH TRONG VÒNG LẶP");
            qlpt.AddPhuongTien(oto5Cho);
            qlpt.AddPhuongTien(xemay150);
            qlpt.DisplayAll(); 
            // C# sẽ tự kích hoạt đúng hàm TinhGiaLanBanh() và GetInfo() tùy thuộc vào loại xe

            // [TC05] - Tìm giá lăn bánh cao nhất
            Console.WriteLine("[TC05] KIỂM TRA TÌM GIÁ LĂN BÁNH MAX");
            var maxPt = qlpt.FindMaxGiaLanBanh();
            if (maxPt != null)
            {
                Console.WriteLine($"Xe có giá max là: {maxPt.TenHang}");
                Console.WriteLine($"Giá trị: {maxPt.TinhGiaLanBanh():N0} VNĐ\n");
            }

            Console.WriteLine("--- KẾT THÚC TEST CASES ---");
            Console.ReadLine();
        }
    }
}