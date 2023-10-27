using Microsoft.AspNetCore.Identity;
using WebUI.Models;

namespace WebUI.Areas.Admin.ViewModels
{
    public class UserRoleVM
    {
        public User User { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }
}
