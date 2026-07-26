using FirstTests.Core;
using FirstTests.Pages;

namespace FirstTests.Tests;

public class LoginTests : BaseTest      // <- không còn [SetUp]/[TearDown]/new ChromeDriver()
{
    [Test]
	[Category("Login")]
    public void DangNhap_DungTaiKhoan_ThanhCong()
    {
        var trangSP = new LoginPage(driver).DangNhap("standard_user", "secret_sauce");

        Assert.That(trangSP.DaHienThi(), Is.True);
    }

    [Test]
	[Category("Smoke")]
    public void DangNhap_TaiKhoanBiKhoa_HienLoi()
    {
        var loginPage = new LoginPage(driver);
        loginPage.DangNhap("locked_out_user", "secret_sauce");

        Assert.That(loginPage.LayThongBaoLoi(), Does.Contain("locked out"));
    }

    // 1 hàm -> 4 ca test. KHÔNG thêm [Test] ở đây (sẽ sinh ca thừa bị lỗi).
    [TestCase("standard_user",   "secret_sauce", true)]
    [TestCase("locked_out_user", "secret_sauce", false)]
    [TestCase("problem_user",    "secret_sauce", true)]
    [TestCase("khong_ton_tai",   "sai_mat_khau", false)]
    
    // demo conflict
    // demo tao nhanh moi truoc khi thuc hien task moi
    public void DangNhap_NhieuTaiKhoan(string tk, string mk, bool mongDoiThanhCong)
    {
        var trangSP = new LoginPage(driver).DangNhap(tk, mk);

        Assert.That(trangSP.DaHienThi(), Is.EqualTo(mongDoiThanhCong));   // kỳ vọng cũng là DỮ LIỆU
    }
}