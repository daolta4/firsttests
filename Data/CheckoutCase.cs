namespace FirstTests.Data;

public class CheckoutCase
{
    public string TenCa { get; set; } = string.Empty;
    public string Ten { get; set; } = string.Empty;
    public string Ho { get; set; } = string.Empty;
    public string MaBuuChinh { get; set; } = string.Empty;
    public bool MongDoiThanhCong { get; set; }
    public string ThongBaoLoi { get; set; } = string.Empty;

    public override string ToString() => TenCa;
}