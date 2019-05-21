using Microsoft.AspNetCore.Mvc.Rendering;
using NewsTech.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NewsTech.Models
{
	public class ContentModel
	{
		public ContentModel() {
			PublishingStatus = new List<SelectListItem>();
			Position = new List<SelectListItem>();
			ImageUrl = new List<string>();
			ThumbnailImageUrl = new List<string>();
			IsReviewVideo = new List<SelectListItem>();
		}
		public int Id { get; set; }

		[Display(Name = "Başlık")]
		public string Title { get; set; }

		[Display(Name = "İçerik Metni 1.Bölüm")]
		public string ContentTextPart1 { get; set; }

		[Display(Name = "İçerik Metni 2.Bölüm")]
		public string ContentTextPart2 { get; set; }


		[Display(Name = "Haber Resimleri")]
		public virtual IList<string> ImageUrl { get; set; }

		[Display(Name = "Haber Küçük Resimleri")]
		public virtual IList<string> ThumbnailImageUrl { get; set; }

		public  string SelectedImageUrl { get; set; }
		public  string SelectedThumbnailImageUrl { get; set; }

		[Display(Name = "Video Linki")]
		public string VideoUrl { get; set; }

		[Display(Name = "Oluşturma Tarihi")]
		public DateTime CreatedDate { get; set; }

		[Display(Name = "Kategori")]
		public int SelectedCategoryId { get; set; }
		public virtual IEnumerable<SelectListItem> AvailableCategories { get; set; }
		public virtual Category Category { get; set; }

		[Display(Name = "Oluşturan Kişi")]
		public int CreatedUserId { get; set; }
		public virtual Employee CreatedUser { get; set; }

		[Display(Name = "Yayınlanma Durumu")]
		public int isPublished { get; set; }
		public bool isDeleted { get; set; }

		[Display(Name = "İçerik Türü")]
		public int isReviewVideo { get; set; }
		public IEnumerable<SelectListItem> PublishingStatus { get; set; }

		public IEnumerable<SelectListItem> IsReviewVideo { get; set; }

		[Display(Name = "Video Anasayfa Pozisyonu")]
		public int VideoPosition { get; set; }

		[Display(Name = "Resim Anasayfa Pozisyonu")]
		public int ImagePosition { get; set; }
		public IEnumerable<SelectListItem> Position { get; set; }


	}
	public enum PublishStatus
	{
		Yayınla = 1,
		Taslak = 2
	}
	public enum ContentType
	{
		Haber = 1,
		Video = 2
	}
	public enum Position
	{
		Yukarı = 1,
		Orta = 2,
		Aşağı=3

	}
}
