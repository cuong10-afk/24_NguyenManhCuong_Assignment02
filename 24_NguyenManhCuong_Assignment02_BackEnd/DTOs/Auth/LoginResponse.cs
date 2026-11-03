namespace _24_NguyenManhCuong_Assignment02_BackEnd.DTOs.Auth
{
    public class LoginResponse
    {
        public string Token { get; set; } = null!;
        public short? AccountID { get; set; }
        public string? AccountName { get; set; }
        public string? AccountEmail { get; set; }
        public int? AccountRole { get; set; }
    }
}
