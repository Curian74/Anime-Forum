using Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.File
{
	public class FileService
	{
		private readonly IWebHostEnvironment _webHostEnvironment;
		private readonly IHttpContextAccessor _httpContextAccessor;
		public FileService(IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContextAccessor)
		{
			_webHostEnvironment = webHostEnvironment;
			_httpContextAccessor = httpContextAccessor;
		}
		public async Task<Media> UploadImage(IFormFile file)
		{
			string currentUserId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
									?? _httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value;
			Guid userId = Guid.Parse(currentUserId);
			string webRootPath = _webHostEnvironment.WebRootPath; 
			string relativeFolderPath = Path.Combine("uploads", "images", "user"); 
			string uploadFolder = Path.Combine(webRootPath, relativeFolderPath); 

			if (!Directory.Exists(uploadFolder))
			{
				Directory.CreateDirectory(uploadFolder);
			}

			Guid mediaId = Guid.NewGuid();
			string encryptedFileName = EncryptFileName(file, mediaId);
			string fileSavePath = Path.Combine(uploadFolder, encryptedFileName); 

			using (FileStream stream = new FileStream(fileSavePath, FileMode.Create))
			{
				await file.CopyToAsync(stream);
			}
			string relativeFilePath = $"/uploads/images/user/{encryptedFileName}";

			Media media = new Media
			{
				Id = mediaId,
				Url = relativeFilePath, 
				CreatedBy = userId
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
