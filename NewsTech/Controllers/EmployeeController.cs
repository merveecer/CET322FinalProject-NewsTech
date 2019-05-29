using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using NewsTech.Areas.Identity.Pages.Account;
using NewsTech.Data;
using NewsTech.Models;

namespace NewsTech.Controllers
{
	public class EmployeeController : Controller
	{
		private readonly NewsTechDbContext _context;
		private readonly UserManager<NewsTechUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly ILogger<RegisterModel> _logger;
		private readonly IEmailSender _emailSender;

		public EmployeeController(NewsTechDbContext context,
			UserManager<NewsTechUser> userManager,
			RoleManager<IdentityRole> roleManager,
			ILogger<RegisterModel> logger,
			   IEmailSender emailSender) {
			_context = context;
			_roleManager = roleManager; _logger = logger;
			_userManager = userManager;
		}
		#region Methods
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
				Text = "Bir Rol Seçiniz..."
			});
			return availableRoles;
		}

		private async void MakeAdmin(string id) {
			if (!(await _roleManager.RoleExistsAsync("admin"))) {
				await _roleManager.CreateAsync(new IdentityRole { Name = "admin" });

			}
			var user = await _userManager.FindByIdAsync(id);
			await _userManager.AddToRoleAsync(user, "admin");
		}
		private async void MakeEditor(string id) {
			if (!(await _roleManager.RoleExistsAsync("Editor"))) {
				await _roleManager.CreateAsync(new IdentityRole { Name = "Editor" });

			}
			var user = await _userManager.FindByIdAsync(id);
			await _userManager.AddToRoleAsync(user, "Editor");
		}

		private async Task<string> GetRoleOfUser(string id) {
			var user = await _userManager.FindByIdAsync(id);
			var roleName = await _userManager.GetRolesAsync(user);
			var role = await _roleManager.FindByNameAsync(roleName.FirstOrDefault());
			var roleid = await _roleManager.GetRoleIdAsync(role);

			return roleid;
		}
		#endregion

		public IActionResult List() {
			var employees = _context.Employees.ToList();
			var employeesmodel = new List<EmployeeModel>();
			foreach (var employee in employees) {
				var model = new EmployeeModel {
					Id = employee.Id,
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
			IEnumerable<GenderType> GenderType = Enum.GetValues(typeof(GenderType)).Cast<GenderType>();
			IEnumerable<MaritalStatus> MaritalStatus = Enum.GetValues(typeof(MaritalStatus)).Cast<MaritalStatus>();

			employeeModel.GenderList = from gender in GenderType
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
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(EmployeeModel model) {
			var loginUserId = _userManager.GetUserId(User);
			if (!(User.IsInRole("admin"))) {
				return Unauthorized();
			}
			var existingUser = _context.Users.Where(x => x.UserName == model.Email).FirstOrDefault();
			if (ModelState.IsValid && existingUser == null && loginUserId != null) {
				NewsTechUser newUser = new NewsTechUser {
					Email = model.Email,
					BirthDate = model.Birthdate,
					CreatedDateTime = DateTime.Now,
					FirstName = model.Name,
					LastName = model.Surname,
					isActive = true,
					isDeleted = false,
					PhoneNumber = model.PhoneNumber,
					UserName = model.Email,
					Gender = model.SelectedGenderId
				};

				var newPassword = "Xx123456.";
				var result = await _userManager.CreateAsync(newUser, newPassword);
				if (result.Succeeded) {
					_logger.LogInformation("User created a new account with password.");
					Employee employee = new Employee {
						Id = newUser.Id,
						Email = newUser.Email,
						BirthDate = newUser.BirthDate,
						CreatedDateTime = DateTime.Now,
						CreatorUserId = loginUserId,
						FirstName = newUser.FirstName,
						LastName = newUser.LastName,
						isActive = true,
						isDeleted = false,
						GenderId = newUser.Gender,
						MaritalStatusId = model.SelectedMaritalStatusId,
						PhoneNumber = newUser.PhoneNumber
					};
					_context.Employees.Add(employee);
					_context.SaveChanges();
					var idOfAdmin = await _roleManager.GetRoleIdAsync(await _roleManager.FindByNameAsync("admin"));
					//var idOfEditor = await _roleManager.GetRoleIdAsync(await _roleManager.FindByNameAsync("Editor"));
					if (model.SelectedRoleId == idOfAdmin)
						MakeAdmin(employee.Id);
					//if (model.SelectedRoleId == idOfEditor)
					//	MakeEditor(employee.Id);

				}
				foreach (var error in result.Errors) {
					ModelState.AddModelError(string.Empty, error.Description);
				}

			}
			return RedirectToAction("List");
		}
		public async Task<IActionResult> Edit(string id) {
			var employee = _context.Employees.Where(x => x.Id == id).FirstOrDefault();
			var user = _context.Users.Where(x => x.Id == id).FirstOrDefault();

			if (id == null || id == "") {
				return BadRequest();
			}
			if (employee == null)
				return NotFound();
			EmployeeModel employeeModel = new EmployeeModel {
				Name = employee.FirstName,
				Birthdate = employee.BirthDate,
				Email = employee.Email,
				SelectedMaritalStatusId = employee.MaritalStatusId,
				PhoneNumber = employee.PhoneNumber,
				SelectedGenderId = employee.GenderId,
				//SelectedRoleId = GetRoleOfUser(employee.Id).ToString(),
				Surname = employee.LastName,
				Id = employee.Id
			};

			var roleName = await _userManager.GetRolesAsync(user);
			var role = await _roleManager.FindByNameAsync(roleName.FirstOrDefault());
			var roleid = await _roleManager.GetRoleIdAsync(role);

			employeeModel.SelectedRoleId = roleid;

			var roles = _context.Roles.OrderBy(x => x.Name).ToList();
			employeeModel.AvailableRoles = GetAvailableRoles(roles);
			//It is taken from Stackoverflow https://stackoverflow.com/questions/20789968/mvc4-dropdown-menu-for-gender
			IEnumerable<GenderType> GenderType = Enum.GetValues(typeof(GenderType)).Cast<GenderType>();
			IEnumerable<MaritalStatus> MaritalStatus = Enum.GetValues(typeof(MaritalStatus)).Cast<MaritalStatus>();

			employeeModel.GenderList = from gender in GenderType
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

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Edit(string id, EmployeeModel model) {
			var currentEmployee = _context.Employees.Where(x => x.Id == id).FirstOrDefault();
			var user = _context.Users.Where(x => x.Id == id).FirstOrDefault();

			if (id == null || id == "") {
				return BadRequest();
			}
			if (currentEmployee == null)
				return NotFound();
			if (ModelState.IsValid) {
				if (!User.IsInRole("admin")) {
					return Unauthorized();
				}
				user.LastName = model.Surname;
				user.PhoneNumber = model.PhoneNumber;
				user.UserName = model.Email;
				user.Email = model.Email;
				user.FirstName = model.Name;
				user.Gender = model.SelectedGenderId;
				user.BirthDate = model.Birthdate;
				_context.Users.Update(user);

				currentEmployee.FirstName = model.Name;
				currentEmployee.BirthDate = model.Birthdate;
				currentEmployee.Email = model.Email;
				currentEmployee.GenderId = model.SelectedGenderId;
				currentEmployee.LastName = model.Surname;
				currentEmployee.MaritalStatusId = model.SelectedMaritalStatusId;
				currentEmployee.PhoneNumber = model.PhoneNumber;
				_context.Employees.Update(currentEmployee);
				_context.SaveChanges();
				return RedirectToAction("Edit");
			} else {
				return View(model);
			}
		
		}
	}
}