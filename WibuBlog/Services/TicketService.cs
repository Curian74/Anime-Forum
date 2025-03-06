using Application.Common.Pagination;
using Application.DTO;
using Domain.Entities;
using WibuBlog.Common.ApiResponse;
using WibuBlog.Interfaces.Api;
using WibuBlog.ViewModels.Ticket;

namespace WibuBlog.Services
{
    public class TicketService(IApiService apiService)
    {
        private readonly IApiService _apiService = apiService;

        public async Task<PagedResult<Ticket>> GetPagedTicketAsync(int? page, int? pageSize)
        {
            var response = await _apiService.GetAsync<ApiResponse<PagedResult<Ticket>>>(
                $"Ticket/GetPaged?page={page}&size={pageSize}");
            return response.Value!;
        }

        public async Task<Ticket> GetTicketByIdAsync<T>(T id)
        {
            try
            {
                var guidId = Guid.Parse(id.ToString());
                var response = await _apiService.GetAsync<ApiResponse<Ticket>>($"Ticket/GetTicketDetail/{guidId}");
                if (response != null)
                {
                    return response.Value;
                }
                return null;
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }


        public async Task<bool> AddNewTicketAsync(AddTicketVM data)
        {
            var response = await _apiService.PostAsync<ApiResponse<Ticket>>("Ticket/CreateTicket", data);
            Console.WriteLine(response.Value);
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
                    IsApproved = model.IsApproved
                };

                var response = await _apiService.PutAsync<ApiResponse<bool>>("Ticket/UpdateTicket", updateDto);
                Console.WriteLine($"API response: {response != null}");
                return response != null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating ticket: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteTicketAsync<T>(T id)
        {
            var response = await _apiService.DeleteAsync($"Ticket/Delete/{id}");
            return response;
        }
    }
}