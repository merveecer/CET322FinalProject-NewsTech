using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NewsTech.Models
{
	public class EmployeeModel
	{
		public EmployeeModel() {
			GenderList = new List<SelectListItem>();
			AvailableRoles = new List<SelectListItem>();
			MaritalStatus = new List<SelectListItem>();
		}


		public string Id { get; set; }

		[Display(Name = "Ad")]
		public string Name { get; set; }
		[Display(Name = "Soyad")]
		public string Surname { get; set; }
		[Display(Name = "E-posta")]
		public string Email { get; set; }
		[Display(Name = "Doğum Tarihi")]
		public DateTime Birthdate { get; set; }
		[Display(Name = "Telefon Numarası")]
		public string PhoneNumber { get; set; }
		[Display(Name = "Rol")]
		public virtual IEnumerable<SelectListItem> AvailableRoles { get; set; }
		public int SelectedRoleId { get; set; }

		[Display(Name = "Cinsiyet")]
		public int SelectedGenderId { get; set; }
		public IEnumerable<SelectListItem> GenderList { get; set; }

		[Display(Name = "Medeni Durum")]
		public int SelectedMaritalStatusId { get; set; }
		public IEnumerable<SelectListItem> MaritalStatus { get; set; }


	}
	public enum GenderType
	{
		Erkek = 1,
		Kadın = 2
	}
	public enum MaritalStatus
	{
		Evli = 1,
		Bekar = 2
	}
}
