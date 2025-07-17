using CloudContentSourceManagementApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudContentSourceManagementApp.Services
{
    public static class GoogleAppProfileService
    {
        public static List<GoogleAppProfile> GetGoogleAppProfilesByTenantId(string tenantId)
        {
            using (var db = new SqlDbContext.DriveDbContext())
            {
                return db.GoogleAppProfiles.Where(x => x.TenantId.Trim().ToLower() == tenantId.Trim().ToLower()).ToList();
            }
        }

        public static void CreateGoogleAppProfile(GoogleAppProfile app)
        {
            using (var db = new SqlDbContext.DriveDbContext())
            {
                db.GoogleAppProfiles.Add(app);
                db.SaveChanges();
            }
        }
    }
}
