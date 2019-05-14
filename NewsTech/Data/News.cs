using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NewsTech.Data
{
	public class News
	{
		public int Id { get; set; }
		public string Title { get; set; }

		[Required]
		public string Content { get; set; }

		public string ImageUrl { get; set; }
		public DateTime CreatedDate { get; set; }

		public int CategoryId { get; set; }
		public virtual Category Category { get; set; }

		public virtual IList<Comment> Comments { get; set; }

		public int CreatedUserId { get; set; }
		public virtual NewsTechUser CreatedUser { get; set; }

		public bool isPublished { get; set; }
		public bool isDeleted { get; set; }
	}
}
