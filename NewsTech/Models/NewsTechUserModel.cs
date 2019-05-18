using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NewsTech.Models
{
	public class NewsTechUserModel
	{
		public string Id { get; set; }
		[Display(Name = "Kullanıcı Adı")]
		public string UserName { get; set; }

		[Display(Name = "Ad Soyad")]
		public string FullName { get; set; }

		[Display(Name = "Admin mi? ")]
		public bool IsAdmin { get; set; }
	}
}
