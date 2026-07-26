using System.Text.Json;
using FirstTests.Core;
using FirstTests.Data;
using FirstTests.Pages;

namespace FirstTests.Tests;

public class DataDrivenLoginTests : BaseTest
{
    // BẮT BUỘC static: NUnit gọi hàm này lúc DÒ test, trước khi tạo đối tượng của class.
    private static IEnumerable<TestCaseData> TuJson()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Data", "users.json");
        var ds   = JsonSerializer.Deserialize<List<UserData>>(File.ReadAllText(path))
                   ?? throw new InvalidOperationException("users.json rỗng hoặc sai định dạng");

        var i = 0;
        foreach (var u in ds)
        {
            i++;
            // Chỉ số i đảm bảo tên DUY NHẤT — 2 dòng cùng Username rỗng sẽ trùng tên,
            // mà NUnit yêu cầu tên ca test không được trùng.
            var ten = string.IsNullOrWhiteSpace(u.Username) ? "trong" : u.Username.Trim();
            yield return new TestCaseData(u.Username, u.Password, u.ThanhCong)
                .SetName($"DangNhap_{i:00}_{ten}");
        }
    }

    [TestCaseSource(nameof(TuJson))]     // nameof: gõ sai là trình biên dịch báo ngay
    public void DangNhap_TuFileJson(string tk, string mk, bool mongDoiThanhCong)
    {
        var trangSP = new LoginPage(driver).DangNhap(tk, mk);

        Assert.That(trangSP.DaHienThi(), Is.EqualTo(mongDoiThanhCong));
    }
}