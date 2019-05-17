using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewsTech.Models
{
	public class EmployeeModel
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public string Surname { get; set; }
		public string Email { get; set; }
		public DateTime Birthdate { get; set; }
	}
}
