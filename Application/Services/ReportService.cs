using Application.DTO;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ReportService (IUnitOfWork unitOfWork, IMapper mapper)
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IGenericRepository<Report> _reportRepository = unitOfWork.GetRepository<Report>();
        private readonly IMapper _mapper = mapper;
        public async Task CreateReportAsync(CreateReportDto addReportDTO)
        {
            var report = _mapper.Map<Report>(addReportDTO);
            await _reportRepository.AddAsync(report);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
