using Application.Common.Pagination;
using Application.DTO;
using Application.Interfaces.Pagination;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using System.Linq.Expressions;

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

        public async Task<int> ApproveReportAsync(Guid reportId, bool approval, string? note = null)
        {
            try
            {
                Console.WriteLine($"ApproveReportAsync called with: ReportId={reportId}, Approval={approval}, Note={note}");

                var report = await _reportRepository.GetByIdAsync(reportId);
                if (report == null)
                {
                    Console.WriteLine($"Report not found: {reportId}");
                    return 0; 
                }

                report.IsApproved = approval;
                report.Note = note;

                await _reportRepository.UpdateAsync(report);
                int result = await _unitOfWork.SaveChangesAsync();

                if (approval)
                {
                    await CheckAndDeactivatePostsWithMultipleReports();
                }

                Console.WriteLine($"Report updated. SaveChanges result: {result}");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ApproveReportAsync: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                throw;
            }
        }


        public async Task<(IEnumerable<Report> Items, int TotalCount)> GetAllAsync(
           Expression<Func<Report, bool>>? filter = null,
           Func<IQueryable<Report>, IOrderedQueryable<Report>>? orderBy = null)
        {
            return await _reportRepository.GetAllAsync(filter, orderBy);
        }

        public async Task<(IEnumerable<Report>, int totalCount)> FindAsync(
        Expression<Func<Report, bool>>? filter = null,
        Func<IQueryable<Report>, IOrderedQueryable<Report>>? orderBy = null)
        {
            return await _reportRepository.GetPagedAsync(1, 10, filter, orderBy);
        }

        public async Task<IPagedResult<Report>> GetPagedAsync(
            int page = 1,
            int size = 10,
            Expression<Func<Report, bool>>? filter = null,
            Func<IQueryable<Report>, IOrderedQueryable<Report>>? orderBy = null)
        {
            var (items, totalCount) = await _reportRepository.GetPagedAsync(page, size, filter, orderBy);
            return new PagedResult<Report>(items, totalCount, page, size);
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

                var user = await _userRepository.GetByIdAsync(report.UserId);
                if (user != null)
                {
                    reportDto.Username = user.UserName;
                }

                var post = await _postRepository.GetByIdAsync(report.PostId);
                if (post != null)
                {
                    reportDto.PostTitle = post.Title;
                    reportDto.PostCategoryId = post.PostCategoryId;

                    var category = await _unitOfWork.GetRepository<PostCategory>().GetByIdAsync(post.PostCategoryId);
                    reportDto.CategoryName = category?.Name ?? "Unknown";
                }

                reportDtos.Add(reportDto);
            }

            return reportDtos;
        }

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

                var user = await _userRepository.GetByIdAsync(report.UserId);
                if (user != null)
                {
                    reportDto.Username = user.UserName;
                }

                var post = await _postRepository.GetByIdAsync(report.PostId);
                if (post != null)
                {
                    reportDto.PostTitle = post.Title;
                    reportDto.PostCategoryId = post.PostCategoryId;

                    var category = await _unitOfWork.GetRepository<PostCategory>().GetByIdAsync(post.PostCategoryId);
                    reportDto.CategoryName = category?.Name ?? "Unknown";
                }

                reportDtos.Add(reportDto);
            }

            return new PagedResult<ReportDto>(reportDtos, totalCount, page, size);
        }
        public async Task CheckAndDeactivatePostsWithMultipleReports(int acceptedReportThreshold = 3)
        {
            var approvedReports = await _reportRepository.GetAllAsync(r => r.IsApproved == true);

            var postReportCounts = approvedReports.Items
                .GroupBy(r => r.PostId)
                .Where(g => g.Count() >= acceptedReportThreshold)
                .ToList();

            foreach (var postReportGroup in postReportCounts)
            {
                var post = await _postRepository.GetByIdAsync(postReportGroup.Key);
                if (post != null)
                {
                    post.IsHidden = true;
                    await _postRepository.UpdateAsync(post);
                }
            }

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
