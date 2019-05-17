using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

		private IList<SelectListItem> GetAvailableRoles(IList<IdentityRole> roles) {
			var availableRoles = new List<SelectListItem>();
			foreach (var role in roles) {
				availableRoles.Add(new SelectListItem {
					Value = role.Id.ToString(),
					Text = role.Name
				});

			}
			availableRoles.Insert(0, new SelectListItem {
				Value = "0",
				Text = "Please select a role"
			});
			return availableRoles;
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

		public IActionResult Create() {
			EmployeeModel employeeModel = new EmployeeModel();
			var roles = _context.Roles.OrderBy(x => x.Name).ToList();
			employeeModel.AvailableRoles = GetAvailableRoles(roles);
			//It is taken from Stackoverflow https://stackoverflow.com/questions/20789968/mvc4-dropdown-menu-for-gender
			IEnumerable<GenderType> GenderType = Enum.GetValues(typeof(GenderType)) .Cast<GenderType>();
			IEnumerable<MaritalStatus> MaritalStatus = Enum.GetValues(typeof(MaritalStatus)).Cast<MaritalStatus>();

			employeeModel.GenderList= from gender in GenderType
									  select new SelectListItem {
										  Text = gender.ToString(),
										  Value = ((int)gender).ToString()
									  };
			employeeModel.MaritalStatus = from maritalStatus in MaritalStatus
										  select new SelectListItem {
										   Text = maritalStatus.ToString(),
										   Value = ((int)maritalStatus).ToString()
									   };
			return View(employeeModel);
			
		}

	}
}