using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.V3.Pages.Account.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NewsTech.Data;
using NewsTech.Models;

namespace NewsTech.Controllers
{
	public class ContentController : Controller
	{
		private readonly NewsTechDbContext _context;
		private readonly UserManager<NewsTechUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly ILogger<RegisterModel> _logger;
		private readonly IHostingEnvironment _hostingEnvironment;
		private int viewCount;

		public ContentController(NewsTechDbContext context,
			UserManager<NewsTechUser> userManager,
			RoleManager<IdentityRole> roleManager,
			ILogger<RegisterModel> logger,
			IHostingEnvironment hostingEnvironment
			) {
			_context = context;
			_roleManager = roleManager;
			_logger = logger;
			_userManager = userManager;
			_hostingEnvironment = hostingEnvironment;

		}

		#region Methods
		private IList<SelectListItem> GetAvailableCategories(IList<Category> categories) {
			var availableCategories = new List<SelectListItem>();
			foreach (var category in categories) {
				availableCategories.Add(new SelectListItem {
					Value = category.Id.ToString(),
					Text = category.Name
				});

			}
			availableCategories.Insert(0, new SelectListItem {
				Value = "0",
				Text = "Bir Kategori Seçiniz..."
			});
			return availableCategories;
		}
		#endregion

		public IActionResult UserHomePage() {
			var News = _context.Contents.Where(x => !x.isDeleted & x.isPublished == 1).Include(x => x.Category).Include(x => x.CreatedUser).ToList();
			var newsModel = new List<ContentModel>();

			foreach (var news in News) {
				var creator = _context.Employees.Where(x => x.Id == news.CreatedUserId).FirstOrDefault();
				var model = new ContentModel {
					Id = news.Id,
					Title = news.Title,
					isPublished = news.isPublished,
					SelectedImageUrl = news.SelectedImageUrl,
					ContentTextPart1 = news.ContentTextPart1,
					ContentTextPart2 = news.ContentTextPart2,
					isReviewVideo = news.isReviewVideo,
					ImagePosition = news.ImagePosition,
					CategoryName = news.Category.Name,
					CreatedUser = news.CreatedUser,
					//ThumbnailImageUrl=news.ThumbnailImageUrl,
					SelectedThumbnailImageUrl = news.SelectedThumbnailImageUrl,
					CreatedUserFullName = creator.FirstName + " " + creator.LastName,
					CreatedDate = news.CreatedDate

				};
				newsModel.Add(model);
			}
			return View(newsModel);
		}
		public IActionResult ContentEdit(int id) {
			var content = _context.Contents.Where(x => x.Id == id).Include(x => x.Category).Include(x => x.CreatedUser).FirstOrDefault();
			var loginUserId = _userManager.GetUserId(User);


			if (id == 0) {
				return BadRequest();
			}
			if (content == null)
				return NotFound();
			ContentModel contentModel = new ContentModel {
				CategoryName = content.Category.Name,
				ContentTextPart1 = content.ContentTextPart1,
				ContentTextPart2 = content.ContentTextPart2,
				CreatedDate = content.CreatedDate,
				SelectedImageUrl = content.SelectedImageUrl,
				isReviewVideo = content.isReviewVideo,
				isPublished = content.isPublished,
				SelectedThumbnailImageUrl = content.SelectedThumbnailImageUrl,
				ImagePosition = content.ImagePosition,
				Title = content.Title,
				VideoPosition = content.VideoPosition,
				VideoUrl = content.VideoUrl,
				SelectedCategoryId = content.CategoryId,
				Id = content.Id
			};

			var categories = _context.Categories.Where(x => !x.isDeleted && x.isActive && x.isPublished).ToList();
			contentModel.AvailableCategories = GetAvailableCategories(categories);
			IEnumerable<PublishStatus> publishStatuses = Enum.GetValues(typeof(PublishStatus)).Cast<PublishStatus>();
			IEnumerable<ContentType> contentTypes = Enum.GetValues(typeof(ContentType)).Cast<ContentType>();
			IEnumerable<Position> positions = Enum.GetValues(typeof(Position)).Cast<Position>();
			contentModel.Position = from position in positions
									select new SelectListItem {
										Text = position.ToString(),
										Value = ((int)position).ToString()
									};
			contentModel.ContentType = from contenttype in contentTypes
									   select new SelectListItem {
										   Text = contenttype.ToString(),
										   Value = ((int)contenttype).ToString()
									   };
			contentModel.PublishingStatus = from status in publishStatuses
											select new SelectListItem {
												Text = status.ToString(),
												Value = ((int)status).ToString()
											};

			return View(contentModel);
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult ContentEdit(int id, ContentModel model, IFormFile thumnailImages, IFormFile newsImages) {
			var currenntContent = _context.Contents.Where(x => x.Id == id).Include(x => x.Category).FirstOrDefault();
			if (id == 0)
				return BadRequest();
			if (currenntContent == null)
				return NotFound();
			if (ModelState.IsValid) {
				if (!User.IsInRole("admin")) {
					return Unauthorized();
				}
				currenntContent.ContentTextPart1 = model.ContentTextPart1;
				currenntContent.ContentTextPart2 = model.ContentTextPart2;
				currenntContent.ImagePosition = model.ImagePosition;
				currenntContent.isPublished = model.isPublished;
				currenntContent.isReviewVideo = model.isReviewVideo;
				currenntContent.Title = model.Title;
				currenntContent.VideoPosition = model.VideoPosition;
				currenntContent.VideoUrl = model.VideoUrl;
				currenntContent.CategoryId = model.SelectedCategoryId;

				string dirPath = Path.Combine(_hostingEnvironment.WebRootPath, @"uploads\");
				if (thumnailImages != null) {
					var fileName = Guid.NewGuid().ToString().Replace("-", "") + "_" + thumnailImages.FileName;
					using (var fileStream = new FileStream(dirPath + fileName, FileMode.Create)) {
						thumnailImages.CopyTo(fileStream);
					}
					currenntContent.SelectedThumbnailImageUrl = fileName;

				}
				if (newsImages != null) {
					var fileName = Guid.NewGuid().ToString().Replace("-", "") + "_" + newsImages.FileName;
					using (var fileStream = new FileStream(dirPath + fileName, FileMode.Create)) {
						newsImages.CopyTo(fileStream);
					}
					currenntContent.SelectedImageUrl = fileName;

				}

				_context.Contents.Update(currenntContent);
				_context.SaveChanges();
				return RedirectToAction("ContentEdit");
			} else
				return View(model);
		}
		public IActionResult NewsDetail(int id) {
			viewCount++;
			var contentmodel = new ContentModel();
			var news = _context.Contents.Where(x => x.Id == id).Include(x => x.Category).Include(x => x.CreatedUser).FirstOrDefault();
			contentmodel.SelectedImageUrl = news.SelectedImageUrl;
			contentmodel.SelectedThumbnailImageUrl = news.SelectedThumbnailImageUrl;
			contentmodel.CategoryName = news.Category.Name;
			contentmodel.ContentTextPart1 = news.ContentTextPart1;
			contentmodel.ContentTextPart2 = news.ContentTextPart2;
			contentmodel.CreatedDate = news.CreatedDate;
			contentmodel.CreatedUserFullName = news.CreatedUser.FirstName + " " + news.CreatedUser.LastName;
			contentmodel.ImagePosition = news.ImagePosition;
			contentmodel.ViewCount = viewCount;
			contentmodel.Title = news.Title;
			return View(contentmodel);
		}
		public IActionResult NewsList() {
			var News = _context.Contents.Where(x => !x.isDeleted).ToList();
			var newsModel = new List<ContentModel>();

			foreach (var news in News) {
				var creator = _context.Employees.Where(x => x.Id == news.CreatedUserId).FirstOrDefault();
				var model = new ContentModel {
					Id = news.Id,
					Title = news.Title,
					isPublished = news.isPublished,
					//ThumbnailImageUrl=news.ThumbnailImageUrl,
					SelectedThumbnailImageUrl = news.SelectedThumbnailImageUrl,
					CreatedUserFullName = creator.FirstName + " " + creator.LastName,
					CreatedDate = news.CreatedDate

				};
				newsModel.Add(model);
			}
			return View(newsModel);
		}
		public IActionResult ContentCreate() {
			ContentModel contentModel = new ContentModel();
			var categories = _context.Categories.Where(x => !x.isDeleted && x.isActive && x.isPublished).ToList();
			contentModel.AvailableCategories = GetAvailableCategories(categories);
			IEnumerable<PublishStatus> publishStatuses = Enum.GetValues(typeof(PublishStatus)).Cast<PublishStatus>();
			IEnumerable<ContentType> contentTypes = Enum.GetValues(typeof(ContentType)).Cast<ContentType>();
			IEnumerable<Position> positions = Enum.GetValues(typeof(Position)).Cast<Position>();
			contentModel.Position = from position in positions
									select new SelectListItem {
										Text = position.ToString(),
										Value = ((int)position).ToString()
									};
			contentModel.ContentType = from contenttype in contentTypes
									   select new SelectListItem {
										   Text = contenttype.ToString(),
										   Value = ((int)contenttype).ToString()
									   };
			contentModel.PublishingStatus = from status in publishStatuses
											select new SelectListItem {
												Text = status.ToString(),
												Value = ((int)status).ToString()
											};
			return View(contentModel);


		}
		[ValidateAntiForgeryToken]
		[HttpPost]
		public IActionResult ContentCreate(ContentModel model, IFormFile thumnailImages, IFormFile newsImages) {
			var loginUserId = _userManager.GetUserId(User);
			if (!(User.IsInRole("admin") || User.IsInRole("editor"))) {
				return Unauthorized();
			}
			var existingContent = _context.Contents.Where(x => x.Title == model.Title && !x.isDeleted);
			if (ModelState.IsValid && existingContent != null && loginUserId != null) {
				Content newContent = new Content {
					Title = model.Title,
					ContentTextPart1 = model.ContentTextPart1,
					ContentTextPart2 = model.ContentTextPart2,
					CreatedDate = DateTime.Now,
					ImagePosition = model.ImagePosition,
					isDeleted = false,
					isPublished = model.isPublished,
					VideoPosition = model.VideoPosition,
					VideoUrl = model.VideoUrl,
					CreatedUserId = loginUserId,
					CreatedUser = _context.Employees.Where(x => x.Id == loginUserId).FirstOrDefault(),
					CategoryId = model.SelectedCategoryId,
					Category = model.Category,
					isReviewVideo = model.isReviewVideo
				};
				List<string> thumbUrls = new List<string>();

				string dirPath = Path.Combine(_hostingEnvironment.WebRootPath, @"uploads\");
				if (thumnailImages != null) {
					var fileName = Guid.NewGuid().ToString().Replace("-", "") + "_" + thumnailImages.FileName;
					using (var fileStream = new FileStream(dirPath + fileName, FileMode.Create)) {
						thumnailImages.CopyTo(fileStream);
					}
					newContent.SelectedThumbnailImageUrl = fileName;

				}
				if (newsImages != null) {
					var fileName = Guid.NewGuid().ToString().Replace("-", "") + "_" + newsImages.FileName;
					using (var fileStream = new FileStream(dirPath + fileName, FileMode.Create)) {
						newsImages.CopyTo(fileStream);
					}
					newContent.SelectedImageUrl = fileName;

				}
				//newContent.ImageUrl= thumbUrls;
				_context.Contents.Add(newContent);
				_context.SaveChanges();
			}
			return RedirectToAction("NewsList");
		}
	}
}