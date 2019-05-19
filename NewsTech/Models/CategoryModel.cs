using NewsTech.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewsTech.Models
{
	public class CategoryModel
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public DateTime CreatedDate { get; set; }
		public int DisplayOrder { get; set; }
		public bool isActive { get; set; }
		public string CreatorUserId { get; set; }
		public bool isPublished { get; set; }
		public virtual NewsTechUser CreatorUser { get; set; }
	}
}
