using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace NewsTech.Data
{
	public class Comment
	{
		public int Id { get; set; }

		[Required]
		public int CreatedUserId { get; set; }
		public virtual NewsTechUser CreatedUser { get; set; }
		[Required]
		[StringLength(1000)]
		public string Content { get; set; }

		public DateTime CreatedDate { get; set; }
		public int NewsId { get; set; }
		public virtual News News { get; set; }

		public int? ParentCommentId { get; set; }
		[ForeignKey("ParentCommentId")]
		public Comment ParentComment { get; set; }

		public bool isDeleted { get; set; }


	}
}
