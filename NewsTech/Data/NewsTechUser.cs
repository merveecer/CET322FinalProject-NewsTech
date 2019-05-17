using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NewsTech.Data
{
	public class NewsTechUser:IdentityUser
	{
		[Required]
		[StringLength(100)]
		public string FirstName { get; set; }

		[Required]
		[StringLength(100)]
		public string LastName { get; set; }

		[Required]
		public DateTime BirthDate { get; set; }

		public DateTime CreatedDateTime { get; set; }

		public bool isDeleted { get; set; }
		public bool isActive { get; set; }
		public bool isEmployee { get; set; }

	}
}
