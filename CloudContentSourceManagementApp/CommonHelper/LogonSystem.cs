namespace CloudContentSourceManagementApp.CommonHelper
{
    public static class LogonSystem
    {
        // Lưu thông tin đăng nhập cho toàn ứng dụng
        public static string TenantId { get; set; }
        public static string Username { get; set; }
        // Có thể thêm các trường khác nếu muốn (Email, Role, ...)

        // Hàm reset khi logout
        public static void Logout()
        {
            TenantId = null;
            Username = null;
        }
    }
}
