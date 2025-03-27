using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class NotificationService(IUnitOfWork unitOfWork)
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IGenericRepository<Notification> _notificationGenericRepository = unitOfWork.GetRepository<Notification>();
        public async Task<Notification> Add(Notification noti)
        {
            await _notificationGenericRepository.AddAsync(noti);
            await _unitOfWork.SaveChangesAsync();
            return noti;
        }
    }
}
