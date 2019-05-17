﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NewsTech.Models
{
	public class NewsTechUserModel
	{
		public string Id { get; set; }
		public string UserName { get; set; }
		public string FullName { get; set; }
		public bool IsAdmin { get; set; }
	}
}
