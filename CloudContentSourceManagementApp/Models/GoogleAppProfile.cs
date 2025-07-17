using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudContentSourceManagementApp.Models
{
    public class GoogleAppProfile
    {
        public int Id { get; set; }
        public string ProfileName { get; set; }
        public string UserEmail { get; set; }
        public string ClientId { get; set; }
        public string ClientEmail { get; set; }
        public string PrivateKey { get; set; }
        public long CreatedTime { get; set; }
        public string TenantId { get; set; }
    }
}
