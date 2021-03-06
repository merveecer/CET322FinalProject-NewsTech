﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewsTech.Data
{
	public class Category
	{
		public int Id { get; set; }
		public string Name { get; set; }

		public bool isDeleted { get; set; }
		public bool isActive { get; set; }
		public bool isPublished { get; set; }

		public DateTime CreatedDate { get; set; }
		public int DisplayOrder { get; set; }
		public string CreatorUserId { get; set; }

		public virtual NewsTechUser CreatorUser { get; set; }


	}
}
