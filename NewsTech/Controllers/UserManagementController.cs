using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NewsTech.Data;
using NewsTech.Models;

namespace NewsTech.Controllers
{
    public class UserManagementController : Controller
    {
		private readonly NewsTechDbContext _context;
		private readonly UserManager<NewsTechUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		public UserManagementController(NewsTechDbContext context, UserManager<NewsTechUser> userManager, RoleManager<IdentityRole> roleManager) {
			_context = context;
			_roleManager = roleManager;
			_userManager = userManager;
		}
		public async Task<ActionResult> Index() {
			var userList = _context.Users.ToList();
			List<NewsTechUserModel> userModelList = new List<NewsTechUserModel>();
			foreach (var item in userList) {
				bool isadmin = await _userManager.IsInRoleAsync(item, "admin");
				var user = new NewsTechUserModel {
					Id = item.Id,
					FullName = item.FirstName + " " + item.LastName,
					UserName = item.UserName,
					IsAdmin = isadmin
				};
				userModelList.Add(user);
			}
			return View(userModelList);
		}
		public async Task<ActionResult> MakeAdmin(string id) {
			if (!(await _roleManager.RoleExistsAsync("admin"))) {
				await _roleManager.CreateAsync(new IdentityRole { Name = "admin" });

			}
			var user = await _userManager.FindByIdAsync(id);
			await _userManager.AddToRoleAsync(user, "admin");
			return RedirectToAction("index");

		}
		public async Task<ActionResult> RemoveAdmin(string id) {
			var user = await _userManager.FindByIdAsync(id);
			await _userManager.RemoveFromRoleAsync(user, "admin");
			return RedirectToAction("index");
		}
	}
}