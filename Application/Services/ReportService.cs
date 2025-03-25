using Application.Common.Pagination;
using Application.DTO;
using Application.Interfaces.Pagination;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static Domain.ValueObjects.Enums.TicketStatusEnum;

namespace Application.Services
{
    public class ReportService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IGenericRepository<Report> _reportRepository = unitOfWork.GetRepository<Report>();
        private readonly IGenericRepository<User> _userRepository = unitOfWork.GetRepository<User>();
        private readonly IGenericRepository<Post> _postRepository = unitOfWork.GetRepository<Post>();
        private readonly IMapper _mapper = mapper;
        public async Task CreateReportAsync(CreateReportDto addReportDTO)
        {
            var report = _mapper.Map<Report>(addReportDTO);
            await _reportRepository.AddAsync(report);
            await _unitOfWork.SaveChangesAsync();
        }
    
            public async Task<int> ApproveTicketAsync(Guid reportId, bool approval, string? note = null)
        {
            var report = await _reportRepository.GetByIdAsync(reportId);

            if (report != null)
            {
                report.IsApproved = approval;
                report.Note = note;
                await _reportRepository.UpdateAsync(report);
            }

            return await _unitOfWork.SaveChangesAsync();
        }
        public async Task<IEnumerable<ReportDto>> GetReportsWithDetailsAsync(
            Expression<Func<Report, bool>>? filter = null,
            Func<IQueryable<Report>, IOrderedQueryable<Report>>? orderBy = null)
        {
            var reports = await _reportRepository.GetAllAsync(filter, orderBy);
            var reportDtos = new List<ReportDto>();

            foreach (var report in reports.Items)
            {
                var reportDto = _mapper.Map<ReportDto>(report);

                // Get user info
                var user = await _userRepository.GetByIdAsync(report.UserId);
                if (user != null)
                {
                    reportDto.Username = user.UserName; // Adjust property name based on your User entity
                }

                // Get post info
                var post = await _postRepository.GetByIdAsync(report.PostId);
                if (post != null)
                {
                    reportDto.PostTitle = post.Title; // Adjust property name based on your Post entity
                }

                reportDtos.Add(reportDto);
            }

            return reportDtos;
        }

        // New method for paged reports with details
        public async Task<IPagedResult<ReportDto>> GetPagedReportsWithDetailsAsync(
            int page = 1,
            int size = 10,
            Expression<Func<Report, bool>>? filter = null,
            Func<IQueryable<Report>, IOrderedQueryable<Report>>? orderBy = null)
        {
            var (items, totalCount) = await _reportRepository.GetPagedAsync(page, size, filter, orderBy);
            var reportDtos = new List<ReportDto>();

            foreach (var report in items)
            {
                var reportDto = _mapper.Map<ReportDto>(report);

                // Get user info
                var user = await _userRepository.GetByIdAsync(report.UserId);
                if (user != null)
                {
                    reportDto.Username = user.UserName; // Adjust property name based on your User entity
                }

                // Get post info
                var post = await _postRepository.GetByIdAsync(report.PostId);
                if (post != null)
                {
                    reportDto.PostTitle = post.Title; // Adjust property name based on your Post entity
                }

                reportDtos.Add(reportDto);
            }

            return new PagedResult<ReportDto>(reportDtos, totalCount, page, size);
        }
    }
}
