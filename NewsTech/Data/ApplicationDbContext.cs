using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace NewsTech.Data
{
	public class NewsTechDbContext : IdentityDbContext<NewsTechUser>
	{
		public NewsTechDbContext(DbContextOptions<NewsTechDbContext> options)
			: base(options) {
		}

		public DbSet<Category> Categories { get; set; }
		public DbSet<Comment> Comments { get; set; }
		public DbSet<News> News { get; set; }
		public DbSet<Employee> Employees { get; set; }

	}
}
