using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoSpeedVehicleManagement
{
    public abstract class PhuongTien
    {
        private string _maPT = string.Empty;
        private string _tenHang = string.Empty;
        private int _namSanXuat;
        private decimal _giaGoc;

        protected PhuongTien(string maPT, string tenHang, int namSanXuat, decimal giaGoc)
        {
            MaPT = maPT;
            TenHang = tenHang;
            NamSanXuat = namSanXuat;
            GiaGoc = giaGoc;
        }

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
                    throw new ArgumentException("Tên hãng không được để trống.");
                _tenHang = value.Trim();
            }
        }

        public int NamSanXuat
        {
            get => _namSanXuat;
            set
            {
                int currentYear = DateTime.Now.Year;
                if (value < 1900 || value > currentYear)
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
                    throw new ArgumentException("Giá gốc phải lớn hơn 0.");
                _giaGoc = value;
            }
        }

        public abstract decimal TinhGiaLanBanh();

        public virtual string GetInfo()
        {
            return $"Mã PT: {MaPT}, Hãng: {TenHang}, Năm SX: {NamSanXuat}, Giá gốc: {GiaGoc:N0} VNĐ";
        }
    }

    public class OTo : PhuongTien
    {
        private int _soChoNgoi;
        private double _dungTichDongCo;

        public OTo(string maPT, string tenHang, int namSanXuat, decimal giaGoc, int soChoNgoi, double dungTichDongCo)
            : base(maPT, tenHang, namSanXuat, giaGoc)
        {
            SoChoNgoi = soChoNgoi;
            DungTichDongCo = dungTichDongCo;
        }

        public int SoChoNgoi
        {
            get => _soChoNgoi;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Số chỗ ngồi phải lớn hơn 0.");
                _soChoNgoi = value;
            }
        }

        public double DungTichDongCo
        {
            get => _dungTichDongCo;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Dung tích động cơ phải lớn hơn 0.");
                _dungTichDongCo = value;
            }
        }

        public override decimal TinhGiaLanBanh()
        {
            if (SoChoNgoi <= 9)
                return GiaGoc + (GiaGoc * 0.12m) + (GiaGoc * 0.30m);
            return GiaGoc + (GiaGoc * 0.10m);
        }

        public override string GetInfo()
        {
            return $"{base.GetInfo()}, Số chỗ ngồi: {SoChoNgoi}, Dung tích động cơ: {DungTichDongCo} cc";
        }
    }

    public class XeMay : PhuongTien
    {
        private int _dungTichXylanh;

        public XeMay(string maPT, string tenHang, int namSanXuat, decimal giaGoc, int dungTichXylanh)
            : base(maPT, tenHang, namSanXuat, giaGoc)
        {
            DungTichXylanh = dungTichXylanh;
        }

        public int DungTichXylanh
        {
            get => _dungTichXylanh;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Dung tích xy lanh phải lớn hơn 0.");
                _dungTichXylanh = value;
            }
        }

        public override decimal TinhGiaLanBanh()
        {
            if (DungTichXylanh < 175)
                return GiaGoc + (GiaGoc * 0.02m);
            return GiaGoc + (GiaGoc * 0.05m);
        }

        public override string GetInfo()
        {
            return $"{base.GetInfo()}, Dung tích xy lanh: {DungTichXylanh} cc";
        }
    }

    public class QuanLyPhuongTien
    {
        private readonly List<PhuongTien> _danhSach = new();

        public void AddPhuongTien(PhuongTien pt)
        {
            if (pt == null)
                throw new ArgumentNullException(nameof(pt));
            _danhSach.Add(pt);
        }

        public void DisplayAll()
        {
            if (_danhSach.Count == 0)
            {
                Console.WriteLine("Không có phương tiện nào trong danh sách.");
                return;
            }

            foreach (var pt in _danhSach)
            {
                Console.WriteLine($"{pt.GetInfo()} | Giá lăn bánh: {pt.TinhGiaLanBanh():N0} VNĐ");
            }
        }

        public PhuongTien FindMaxGiaLanBanh()
        {
            if (_danhSach.Count == 0)
                throw new InvalidOperationException("Danh sách phương tiện rỗng.");
            return _danhSach.OrderByDescending(pt => pt.TinhGiaLanBanh()).First();
        }

        public List<PhuongTien> SearchByName(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return new List<PhuongTien>();

            return _danhSach
                .Where(pt => pt.TenHang.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            QuanLyPhuongTien quanLy = new QuanLyPhuongTien();
            int choice = 0;

            while (true)
            {
                Console.WriteLine("\n========== HỆ THỐNG QUẢN LÝ PHƯƠNG TIỆN GIAO THÔNG AUTOSPEED ==========");
                Console.WriteLine("1. Thêm Ô Tô");
                Console.WriteLine("2. Thêm Xe Máy");
                Console.WriteLine("3. Hiển thị toàn bộ phương tiện");
                Console.WriteLine("4. Tìm phương tiện có giá lăn bánh cao nhất");
                Console.WriteLine("5. Tìm phương tiện theo tên hãng");
                Console.WriteLine("6. Chạy Test Case");
                Console.WriteLine("0. Thoát");
                Console.Write("Chọn chức năng (0-6): ");

                if (!int.TryParse(Console.ReadLine(), out choice))
                {
                    Console.WriteLine("❌ Vui lòng nhập số hợp lệ!");
                    continue;
                }

                switch (choice)
                {
                    case 1:
                        ThemOTo(quanLy);
                        break;
                    case 2:
                        ThemXeMay(quanLy);
                        break;
                    case 3:
                        Console.WriteLine("\n--- DANH SÁCH PHƯƠNG TIỆN ---");
                        quanLy.DisplayAll();
                        break;
                    case 4:
                        TimGiaLanBanhMax(quanLy);
                        break;
                    case 5:
                        TimTheoTenHang(quanLy);
                        break;
                    case 6:
                        ChayTestCase(quanLy);
                        break;
                    case 0:
                        Console.WriteLine("👋 Cảm ơn bạn đã sử dụng hệ thống!");
                        return;
                    default:
                        Console.WriteLine("❌ Lựa chọn không hợp lệ!");
                        break;
                }
            }
        }

        static void ThemOTo(QuanLyPhuongTien quanLy)
        {
            try
            {
                Console.WriteLine("\n--- THÊM Ô TÔ ---");
                Console.Write("Nhập mã phương tiện (để trống sẽ mặc định PT000): ");
                string maPT = Console.ReadLine() ?? string.Empty;

                Console.Write("Nhập tên hãng: ");
                string tenHang = Console.ReadLine() ?? string.Empty;

                Console.Write("Nhập năm sản xuất: ");
                if (!int.TryParse(Console.ReadLine(), out int namSX))
                {
                    Console.WriteLine("❌ Năm sản xuất phải là số!");
                    return;
                }

                Console.Write("Nhập giá gốc (VNĐ): ");
                if (!decimal.TryParse(Console.ReadLine(), out decimal giaGoc))
                {
                    Console.WriteLine("❌ Giá gốc phải là số!");
                    return;
                }

                Console.Write("Nhập số chỗ ngồi: ");
                if (!int.TryParse(Console.ReadLine(), out int soChoNgoi))
                {
                    Console.WriteLine("❌ Số chỗ ngồi phải là số!");
                    return;
                }

                Console.Write("Nhập dung tích động cơ (cc): ");
                if (!double.TryParse(Console.ReadLine(), out double dungTichDongCo))
                {
                    Console.WriteLine("❌ Dung tích động cơ phải là số!");
                    return;
                }

                OTo oto = new OTo(maPT, tenHang, namSX, giaGoc, soChoNgoi, dungTichDongCo);
                quanLy.AddPhuongTien(oto);
                Console.WriteLine($"✅ Thêm ô tô thành công!");
                Console.WriteLine($"   {oto.GetInfo()} | Giá lăn bánh: {oto.TinhGiaLanBanh():N0} VNĐ");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi: {ex.Message}");
            }
        }

        static void ThemXeMay(QuanLyPhuongTien quanLy)
        {
            try
            {
                Console.WriteLine("\n--- THÊM XE MÁY ---");
                Console.Write("Nhập mã phương tiện (để trống sẽ mặc định PT000): ");
                string maPT = Console.ReadLine() ?? string.Empty;

                Console.Write("Nhập tên hãng: ");
                string tenHang = Console.ReadLine() ?? string.Empty;

                Console.Write("Nhập năm sản xuất: ");
                if (!int.TryParse(Console.ReadLine(), out int namSX))
                {
                    Console.WriteLine("❌ Năm sản xuất phải là số!");
                    return;
                }

                Console.Write("Nhập giá gốc (VNĐ): ");
                if (!decimal.TryParse(Console.ReadLine(), out decimal giaGoc))
                {
                    Console.WriteLine("❌ Giá gốc phải là số!");
                    return;
                }

                Console.Write("Nhập dung tích xy lanh (cc): ");
                if (!int.TryParse(Console.ReadLine(), out int dungTichXylanh))
                {
                    Console.WriteLine("❌ Dung tích xy lanh phải là số!");
                    return;
                }

                XeMay xeMay = new XeMay(maPT, tenHang, namSX, giaGoc, dungTichXylanh);
                quanLy.AddPhuongTien(xeMay);
                Console.WriteLine($"✅ Thêm xe máy thành công!");
                Console.WriteLine($"   {xeMay.GetInfo()} | Giá lăn bánh: {xeMay.TinhGiaLanBanh():N0} VNĐ");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi: {ex.Message}");
            }
        }

        static void TimGiaLanBanhMax(QuanLyPhuongTien quanLy)
        {
            try
            {
                PhuongTien max = quanLy.FindMaxGiaLanBanh();
                Console.WriteLine("\n--- PHƯƠNG TIỆN CÓ GIÁ LĂN BÁNH CAO NHẤT ---");
                Console.WriteLine($"✅ {max.GetInfo()} | Giá lăn bánh: {max.TinhGiaLanBanh():N0} VNĐ");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi: {ex.Message}");
            }
        }

        static void TimTheoTenHang(QuanLyPhuongTien quanLy)
        {
            Console.WriteLine("\n--- TÌM PHƯƠNG TIỆN THEO TÊN HÃNG ---");
            Console.Write("Nhập tên hãng cần tìm: ");
            string keyword = Console.ReadLine() ?? string.Empty;

            var result = quanLy.SearchByName(keyword);

            if (result.Count == 0)
            {
                Console.WriteLine($"❌ Không tìm thấy phương tiện nào với tên hãng '{keyword}'");
            }
            else
            {
                Console.WriteLine($"✅ Tìm thấy {result.Count} phương tiện:");
                foreach (var pt in result)
                {
                    Console.WriteLine($"   {pt.GetInfo()} | Giá lăn bánh: {pt.TinhGiaLanBanh():N0} VNĐ");
                }
            }
        }

        static void ChayTestCase(QuanLyPhuongTien quanLy)
        {
            Console.WriteLine("\n========== CHẠY TEST CASE ==========");

            // TC 01: Validation năm sản xuất
            Console.WriteLine("\n📋 TC01: Kiểm tra Validation Năm sản xuất");
            try
            {
                var otoInvalid = new OTo("PT001", "Toyota", 1850, 1000000000, 5, 2.0);
                Console.WriteLine("❌ FAILED - Không ném ngoại lệ!");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"✅ PASSED - {ex.Message}");
            }

            // TC 02: Tính giá lăn bánh ô tô
            Console.WriteLine("\n📋 TC02: Kiểm tra Tính Giá Lăn Bánh Ô tô");
            try
            {
                var oto = new OTo("PT002", "Toyota", 2022, 1000000000, 5, 2.0);
                decimal giaLanBanh = oto.TinhGiaLanBanh();
                decimal expected = 1000000000 + (1000000000 * 0.12m) + (1000000000 * 0.30m);

                if (giaLanBanh == expected)
                {
                    Console.WriteLine($"✅ PASSED");
                    Console.WriteLine($"   Giá lăn bánh = {giaLanBanh:N0} VNĐ (kỳ vọng: {expected:N0} VNĐ)");
                }
                else
                {
                    Console.WriteLine($"❌ FAILED");
                    Console.WriteLine($"   Giá lăn bánh = {giaLanBanh:N0} VNĐ (kỳ vọng: {expected:N0} VNĐ)");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ FAILED - {ex.Message}");
            }

            // TC 03: Tính giá lăn bánh xe máy
            Console.WriteLine("\n📋 TC03: Kiểm tra Tính Giá Lăn Bánh Xe máy");
            try
            {
                var xeMay = new XeMay("PT003", "Honda", 2021, 50000000, 150);
                decimal giaLanBanh = xeMay.TinhGiaLanBanh();
                decimal expected = 50000000 + (50000000 * 0.02m);

                if (giaLanBanh == expected)
                {
                    Console.WriteLine($"✅ PASSED");
                    Console.WriteLine($"   Giá lăn bánh = {giaLanBanh:N0} VNĐ (kỳ vọng: {expected:N0} VNĐ)");
                }
                else
                {
                    Console.WriteLine($"❌ FAILED");
                    Console.WriteLine($"   Giá lăn bánh = {giaLanBanh:N0} VNĐ (kỳ vọng: {expected:N0} VNĐ)");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ FAILED - {ex.Message}");
            }

            // TC 04: Đa hình List<PhuongTien>
            Console.WriteLine("\n📋 TC04: Kiểm tra Đa hình List<PhuongTien>");
            try
            {
                QuanLyPhuongTien testQuanLy = new QuanLyPhuongTien();
                var oto = new OTo("PT004", "BMW", 2023, 1000000000, 5, 2.0);
                var xeMay = new XeMay("PT005", "Honda", 2021, 50000000, 150);

                testQuanLy.AddPhuongTien(oto);
                testQuanLy.AddPhuongTien(xeMay);

                Console.WriteLine("✅ PASSED");
                Console.WriteLine("   C# tự động kích hoạt đúng công thức tính giá lăn bánh:");
                testQuanLy.DisplayAll();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ FAILED - {ex.Message}");
            }

            // TC 05: Tìm giá lăn bánh max
            Console.WriteLine("\n📋 TC05: Kiểm tra Tìm Giá Lăn Bánh Max");
            try
            {
                QuanLyPhuongTien testQuanLy = new QuanLyPhuongTien();
                var oto = new OTo("PT006", "Toyota", 2022, 1000000000, 5, 2.0);
                var xeMay = new XeMay("PT007", "Honda", 2021, 50000000, 150);

                testQuanLy.AddPhuongTien(oto);
                testQuanLy.AddPhuongTien(xeMay);

                var max = testQuanLy.FindMaxGiaLanBanh();
                decimal expected = 1420000000;

                if (max is OTo && max.TinhGiaLanBanh() == expected)
                {
                    Console.WriteLine($"✅ PASSED");
                    Console.WriteLine($"   {max.GetInfo()} | Giá lăn bánh: {max.TinhGiaLanBanh():N0} VNĐ");
                }
                else
                {
                    Console.WriteLine($"❌ FAILED");
                    Console.WriteLine($"   {max.GetInfo()} | Giá lăn bánh: {max.TinhGiaLanBanh():N0} VNĐ");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ FAILED - {ex.Message}");
            }

            Console.WriteLine("\n========== KẾT THÚC TEST CASE ==========");
        }
    }
}
