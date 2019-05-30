using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsTech.Data;
using NewsTech.Models;

namespace NewsTech.Controllers
{
	[Authorize(Roles = "admin,editor")]
	public class CategoryController : Controller
	{
		private readonly UserManager<NewsTechUser> _userManager;
		private readonly NewsTechDbContext _context;
		public CategoryController(NewsTechDbContext context,
			UserManager<NewsTechUser> userManager
			) {
			_context = context;
			_userManager = userManager;
		}
		#region Delete
		public IActionResult Delete(int id) {
			var category = _context.Categories.Where(x => x.Id == id).FirstOrDefault();
			if (!(User.IsInRole("admin") || User.IsInRole("editor"))) {
				return Unauthorized();
			}
			if (category != null) {
				category.isDeleted = true;
				_context.Categories.Update(category);
				_context.SaveChanges();

				return RedirectToAction("NewsList");
			}
			return NotFound();
		}

		#endregion
		#region List
		public IActionResult List() {
			if (!(User.IsInRole("admin") || User.IsInRole("editor"))) {
				return Unauthorized();
			}
			var categories = _context.Categories.Include(x => x.CreatorUser).ToList();
			var categoriesmodel = new List<CategoryModel>();
			foreach (var category in categories) {
				var model = new CategoryModel {
					Id = category.Id,
					Name = category.Name,
					CreatedDate = category.CreatedDate,
					CreatorUser = category.CreatorUser,
					CreatorUserId = category.CreatorUserId,
					DisplayOrder = category.DisplayOrder,
					isActive = category.isActive,
					isPublished=category.isPublished
					
				};
				categoriesmodel.Add(model);
			}
			return View(categoriesmodel);
		}
		#endregion
		#region Create
		public IActionResult Create() {
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Create(CategoryModel model) {
			var loginUserId = _userManager.GetUserId(User);
			if (!(User.IsInRole("admin") || User.IsInRole("editor"))) {
				return Unauthorized();
			}

			var existingCategory = _context.Categories.Where(x => x.Name == model.Name && !x.isDeleted).FirstOrDefault();
			if (ModelState.IsValid && existingCategory == null && loginUserId != null) {
				Category newCategory = new Category {
					Name = model.Name,
					CreatedDate = DateTime.Now,
					CreatorUser = _context.Users.Where(x => x.Id == loginUserId).FirstOrDefault(),
					CreatorUserId = loginUserId,
					isActive = true,
					isDeleted = false,
					isPublished = model.isPublished,
					DisplayOrder = model.DisplayOrder
				};
				_context.Categories.Add(newCategory);
				_context.SaveChanges();
			}

			return RedirectToAction("List");
		}
		#endregion

		#region Edit
		public IActionResult Edit(int id) {
			if (!(User.IsInRole("admin") || User.IsInRole("editor"))) {
				return Unauthorized();
			}
			var category = _context.Categories.Where(x => x.Id == id).FirstOrDefault();
			if (id == 0)
				return BadRequest();

			if (category == null)
				return NotFound();
			CategoryModel categoryModel = new CategoryModel {
				DisplayOrder = category.DisplayOrder,
				isPublished = category.isPublished,
				Name = category.Name
			};
			return View(categoryModel);
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Edit(int? id,CategoryModel model) {
			if (!(User.IsInRole("admin") || User.IsInRole("editor"))) {
				return Unauthorized();
			}
			var existingCategory = _context.Categories.Where(x => x.Id == id).FirstOrDefault();
			if (id == 0)
				return BadRequest();

			if (existingCategory == null)
				return NotFound();
			if (ModelState.IsValid) {
				existingCategory.Name = model.Name;
				existingCategory.DisplayOrder = model.DisplayOrder;
				existingCategory.isPublished = model.isPublished;
				_context.Categories.Update(existingCategory);
				_context.SaveChanges();
				return RedirectToAction("Edit");
			}
			return View(model);
		}


		#endregion
	}
}