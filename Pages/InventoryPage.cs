using System.Globalization;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace FirstTests.Pages;

public class InventoryPage : BasePage
{
    private readonly By tieuDe  = By.ClassName("title");
    private readonly By sanPham = By.ClassName("inventory_item");
    private readonly By gia     = By.ClassName("inventory_item_price");
    private readonly By combo   = By.ClassName("product_sort_container");

    public InventoryPage(IWebDriver d) : base(d) { }

    /// <summary>
    /// Hàm trả về bool thì PHẢI trả lời được cả KHÔNG (slide 28).
    /// Bản Bài 4 dùng Tim() -> ca nghịch chờ hết timeout rồi NÉM, Assert không chạy tới.
    /// </summary>
    public bool DaHienThi()
    {
        var w = new WebDriverWait(driver, TimeSpan.FromSeconds(3));  // ngắn: ca nghịch nào cũng chờ
        try
        {
            return w.Until(d => d.FindElements(tieuDe).Count > 0    // FindElements: KHÔNG ném
                                && d.FindElement(tieuDe).Text == "Products");
        }
        catch (WebDriverTimeoutException)
        {
            return false;   // không vào được = câu TRẢ LỜI, không phải sự cố
        }
    }

    public int SoSanPham() => driver.FindElements(sanPham).Count;

    public InventoryPage SapXep(string value)
    {
        new SelectElement(Tim(combo)).SelectByValue(value);
        return this;            // vẫn ở trang này -> trả về chính nó
    }

    /// <summary>InvariantCulture BẮT BUỘC: máy vi-VN đọc "29.99" thành 2999 mà KHÔNG báo lỗi.</summary>
    public List<decimal> LayDanhSachGia() =>
        driver.FindElements(gia)
              .Select(e => decimal.Parse(e.Text.Replace("$", ""), CultureInfo.InvariantCulture))
              .ToList();
}