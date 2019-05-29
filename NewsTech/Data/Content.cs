using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace NewsTech.Data
{
	public class Content
	{
		
		public int Id { get; set; }
		public string Title { get; set; }

		[Required]
		public string ContentTextPart1 { get; set; }

		public string ContentTextPart2 { get; set; }
		//[NotMapped]
		//public virtual string ImageUrl { get; set; }

		public string SelectedImageUrl { get; set; }

		//[NotMapped]
		//public virtual ICollection<string> ThumbnailImageUrl { get; set; }

		public string SelectedThumbnailImageUrl { get; set; }

		public string VideoUrl { get; set; }
		public DateTime CreatedDate { get; set; }

		public int CategoryId { get; set; }
		public virtual Category Category { get; set; }

		[NotMapped]
		public virtual ICollection<Comment> Comments { get; set; }

		public string CreatedUserId { get; set; }
		public virtual Employee CreatedUser { get; set; }

		public int isPublished { get; set; }
		public bool isDeleted { get; set; }
		public int isReviewVideo { get; set; }

		public int VideoPosition { get; set; }
		public int ImagePosition { get; set; }
		public Content() {

			//ImageUrl = "aa";
			//ThumbnailImageUrl = new List<string>();
			Comments = new List<Comment>();
		}
	}
}
