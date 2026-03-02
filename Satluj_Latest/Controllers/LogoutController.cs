using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;


namespace Satluj_Latest.Controllers
{
    public class LogoutController : Controller
    {
        //
        // GET: /Logout/

        private async Task ClearSessionAndLogout()
        {
            
            HttpContext.Session.Clear();

           
            await HttpContext.SignOutAsync();
        }
        public async Task<IActionResult> SchoolLogout()
        {
            await ClearSessionAndLogout();
            return RedirectToAction("SchoolLogin", "Account");
        }
        public async Task<IActionResult> ParentLogout()
        {
            await ClearSessionAndLogout();
            return RedirectToAction("ParentLogin", "Account");
        }

        public async Task<IActionResult> StaffLogout()
        {
            await ClearSessionAndLogout();
            return RedirectToAction("StaffLogin", "Account");
        }

        public async Task<IActionResult> AdminLogout()
        {
            await ClearSessionAndLogout();
            return RedirectToAction("SuperAdminLogin", "Account");
        }
    }
}
