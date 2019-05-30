using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsTech.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewsTech.Views.Shared.Components.CategoryMenu
{
	public class CategoryMenuViewComponent: ViewComponent
	{
		private readonly NewsTechDbContext _context;
		public CategoryMenuViewComponent(NewsTechDbContext context ) {
			_context = context;
		}
		public async Task<IViewComponentResult> InvokeAsync() {
			var categories = await _context.Categories.ToListAsync();
			return View(categories);
		}

	}
}
