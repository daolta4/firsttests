using System.Text.Json;
using FirstTests.Core;
using FirstTests.Data;
using FirstTests.Pages;
using OpenQA.Selenium;

namespace FirstTests.Tests;

public class DataDrivenCheckoutTests : BaseTest
{
    private static IEnumerable<TestCaseData> TuJson()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Data", "checkout_cases.json");
        var ds   = JsonSerializer.Deserialize<List<CheckoutCase>>(File.ReadAllText(path))
                   ?? throw new InvalidOperationException("checkout_cases.json rỗng hoặc sai định dạng");

        var i = 0;
        foreach (var c in ds)
        {
            i++;
            yield return new TestCaseData(c)
                .SetName($"ThanhToan_{i:00}_{c.TenCa}");
        }
    }

    [TestCaseSource(nameof(TuJson))]
    public void ThanhToan_TuFileJson(CheckoutCase caTest)
    {
        // 1. Đăng nhập với tài khoản standard_user
        var trangSanPham = new LoginPage(driver).DangNhap("standard_user", "secret_sauce");

        // 2+3. Thêm sản phẩm vào giỏ rồi mở giỏ hàng.
        // KHÔNG dùng driver.FindElement(...).Click() thẳng trong file test: không chờ,
        // không kiểm tra cú click có ăn hay không -> đây chính là lỗi đỏ trên CI.
        var checkoutPage = trangSanPham.ThemVaoGio("sauce-labs-backpack")
                                       .MoGioHang();

        // 4. Bắt đầu checkout và nhập thông tin từ file JSON
        checkoutPage.BatDau()
                    .NhapThongTin(caTest.Ten, caTest.Ho, caTest.MaBuuChinh);

        // 5. Kiểm tra kết quả
        if (caTest.MongDoiThanhCong)
        {
            var tieuDe = checkoutPage.HoanTatVaLayTieuDe();
            
            // Nếu giao diện hiển thị thông điệp "Thank you for your order!" ở trang hoàn tất
            Assert.That(tieuDe, Is.EqualTo("Checkout: Complete!").Or.EqualTo("Thank you for your order!"));
        }
        else
        {
            Assert.That(checkoutPage.LayThongBaoLoi(), Is.EqualTo(caTest.ThongBaoLoi));
        }
    }
}