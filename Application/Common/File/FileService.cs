using Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.File
{
	public class FileService
	{
		private readonly IWebHostEnvironment _webHostEnvironment;
		public FileService(IWebHostEnvironment webHostEnvironment)
		{
			_webHostEnvironment = webHostEnvironment;
		}
		public async Task<Media> UploadImage(IFormFile file)
		{
			string uploadFolder = GetUploadPath();
			if (!Directory.Exists(uploadFolder))
			{
				Directory.CreateDirectory(uploadFolder);
			}
			Guid mediaId = Guid.NewGuid();
			string encryptedFileName = EncryptFileName(file,mediaId);
			string fileSavePath = Path.Combine(uploadFolder, encryptedFileName);
			using (FileStream stream = new FileStream(fileSavePath, FileMode.Create))
			{
				await file.CopyToAsync(stream);
			}
			
			Media media = new Media
			{
				Id = mediaId,
				Url = fileSavePath
			};
			return media;
		}

		private string GetUploadPath()
		{
			return Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
		}

		private string EncryptFileName(IFormFile file, Guid fileName)
		{
			string fileExtension = Path.GetExtension(file.FileName);
			string newFileName = $"{fileName}{fileExtension}";
			return newFileName;
		}
	}
}
