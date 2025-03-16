using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{

	public class MediaService(IUnitOfWork unitOfWork)
	{
		private readonly IGenericRepository<Media> _mediaGenericRepository = unitOfWork.GetRepository<Media>();
		private readonly IUnitOfWork _unitOfWork = unitOfWork;


		public async Task<Media> AddAsync(Media media)
		{
			await _mediaGenericRepository.AddAsync(media);
			await _unitOfWork.SaveChangesAsync();
			return media;
		}
	}
}
