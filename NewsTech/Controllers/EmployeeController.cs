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
	public class EmployeeController : Controller
	{
		private readonly NewsTechDbContext _context;
		private readonly UserManager<NewsTechUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		public EmployeeController(NewsTechDbContext context, UserManager<NewsTechUser> userManager, RoleManager<IdentityRole> roleManager) {
			_context = context;
			_roleManager = roleManager;
			_userManager = userManager;
		}
		public IActionResult List() {
			var employees = _context.Users.Where(x => x.isEmployee).ToList();
			var employeesmodel = new List<EmployeeModel>();
			foreach (var employee in employees) {
				var model = new EmployeeModel {
					Id=employee.Id,
					Name = employee.FirstName,
					Surname = employee.LastName,
					Birthdate = employee.BirthDate,
					Email = employee.Email
				};
				employeesmodel.Add(model);
			}

			return View(employeesmodel);
		}
	}
}