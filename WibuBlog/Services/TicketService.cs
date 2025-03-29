using Application.Common.Pagination;
using Application.DTO;
using Application.Interfaces.Pagination;
using Domain.Entities;
using WibuBlog.Common.ApiResponse;
using WibuBlog.Interfaces.Api;
using WibuBlog.ViewModels.Ticket;

namespace WibuBlog.Services
{
    public class TicketService(IApiService apiService)
    {
        private readonly IApiService _apiService = apiService;

        public async Task<List<Ticket>> GetUserTicketsAsync(Guid userId)
        {
            try
            {
                var response = await _apiService.GetAsync<ApiResponse<List<Ticket>>>("Ticket/GetUserTickets");
                // Check if we got a response
                if (response?.Value == null)
                    return new List<Ticket>();

                return response.Value;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetUserTicketsAsync: {ex.Message}");
                return new List<Ticket>();
            }
        }

        public async Task<List<Ticket>> GetAllTicketsAsync()
        {
            try
            {
                var response = await _apiService.GetAsync<ApiResponse<List<Ticket>>>("Ticket/GetAllTickets");
                // Check if we got a response
                if (response?.Value == null)
                    return new List<Ticket>();

                return response.Value;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllTicketsAsync: {ex.Message}");
                return new List<Ticket>();
            }
        }

        public async Task<Ticket> GetTicketByIdAsync(Guid id)
        {
            try
            {
                var response = await _apiService.GetAsync<ApiResponse<Ticket>>($"Ticket/GetTicketDetail/{id}");
                return response?.Value;
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }

        public async Task<IPagedResult<Ticket>> GetPagedAsync(int page = 1, int pageSize = 10, bool descending = false)
        {
            try
            {
                var response = await _apiService.GetAsync<ApiResponse<PagedResult<Ticket>>>($"Ticket/GetPagedTickets?page={page}&size={pageSize}&descending={descending}");
                return response.Value;
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }

        public async Task<bool> AddNewTicketAsync(AddTicketVM data)
        {
            var response = await _apiService.PostAsync<ApiResponse<Ticket>>("Ticket/CreateTicket", data);
            return response != null;
        }

        public async Task<bool> UpdateTicketAsync(Guid id, TicketDetailVM model)
        {
            try
            {
                var updateDto = new UpdateTicketDto
                {
                    Id = id,
                    Content = model.Content,
                    Email = model.Email,
                    Tag = model.Tag,
                    Status = model.Status
                };
                var response = await _apiService.PutAsync<ApiResponse<bool>>($"Ticket/UpdateTicket/{id}", updateDto);
                return response != null && response.Value;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> CloseTicketAsync(Guid id)
        {
            var response = await _apiService.PutAsync<ApiResponse<bool>>($"Ticket/CloseTicket/{id}", null);
            return response != null && response.Value;
        }

        public async Task<bool> DeleteTicketAsync<T>(T id)
        {
            var response = await _apiService.DeleteAsync($"Ticket/Delete/{id}");
            return response;
        }

        public async Task<bool> ApproveTicketAsync(Guid reportId, bool approval, string? note = null)
        {
            var response = await _apiService.PutAsync<ApiResponse<Ticket>>(
                $"Ticket/ApproveTicket/reports/{reportId}",
                new ApproveReportDto
                {
                    Approval = approval,
                    Note = note
                }
            );
            return response != null;
        }
    }
}