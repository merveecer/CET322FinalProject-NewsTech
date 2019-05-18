using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NewsTech.Data
{
	public class Employee
	{
		[Required]
		public string Id { get; set; }

		[Required]
		[StringLength(100)]
		public string FirstName { get; set; }

		[Required]
		[StringLength(100)]
		public string LastName { get; set; }

		[Required]
		public DateTime BirthDate { get; set; }

		[Required]
		public string  Email { get; set; }

		[Required]

		public string PhoneNumber { get; set; }

		public DateTime CreatedDateTime { get; set; }

		public bool isDeleted { get; set; }
		public bool isActive { get; set; }
		public int GenderId { get; set; }//if it is 1, personal gender is male. if it is 2, personal gender is female.
		public int MaritalStatusId { get; set; }//if it is 1, marital status is single. if it is 2,marital status is married.

		public string CreatorUserId { get; set; }
		public virtual NewsTechUser CreatorUser { get; set; }
	}
}
