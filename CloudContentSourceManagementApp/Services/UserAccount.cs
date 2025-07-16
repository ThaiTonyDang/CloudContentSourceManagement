using CloudContentSourceManagementApp.Models;
using System;
using System.Linq;

namespace CloudContentSourceManagementApp.Services
{
    public static class UserService
    {
        // Kiểm tra tên đăng nhập đã tồn tại chưa
        public static bool UsernameExists(string username)
        {
            using (var db = new SqlDbContext.DriveDbContext())
            {
                return db.UserAccounts.Any(x => x.Username.Trim().ToLower() == username.Trim().ToLower());
            }
        }

        // Thêm user mới vào DB
        public static void AddUser(UserAccount acc)
        {
            using (var db = new SqlDbContext.DriveDbContext())
            {
                db.UserAccounts.Add(acc);
                db.SaveChanges();
            }
        }

        // Lấy user theo username
        public static UserAccount GetUserByUsername(string username)
        {
            using (var db = new SqlDbContext.DriveDbContext())
            {
                return db.UserAccounts.FirstOrDefault(x => x.Username.Trim().ToLower() == username.Trim().ToLower());
            }
        }
    }
}
