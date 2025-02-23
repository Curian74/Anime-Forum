using Application.Common.Pagination;
using Domain.Entities;
using Infrastructure.Common.ApiResponse;
using WibuBlog.Interfaces.Api;
using WibuBlog.ViewModels.Ticket;

namespace WibuBlog.Services
{
    public class TicketServices(IApiServices apiService)
    {
        private readonly IApiServices _apiService = apiService;

        public async Task<PagedResult<Ticket>> GetPagedTicketAsync(int? page, int? pageSize)
        {
            var response = await _apiService.GetAsync<ApiResponse<PagedResult<Ticket>>>(
                $"Ticket/GetPaged?page={page}&size={pageSize}");
            return response.Value!;
        }

        public async Task<Ticket> GetTicketByIdAsync<T>(T id)
        {
            var response = await _apiService.GetAsync<ApiResponse<Ticket>>($"Ticket/Get/{id}");
            return response.Value!;
        }

        public async Task<bool> AddNewTicketAsync(AddTicketVM data)
        {
            var response = await _apiService.PostAsync<ApiResponse<Ticket>>("User/CreateTicket", data);
            Console.WriteLine(response.Value);
            return response != null;
        }

        public async Task<Ticket> UpdateTicketAsync<T>(T id, Ticket data)
        {
            var response = await _apiService.PutAsync<ApiResponse<Ticket>>($"Ticket/Update/{id}", data);
            return response.Value!;
        }

        public async Task<bool> DeleteTicketAsync<T>(T id)
        {
            var response = await _apiService.DeleteAsync($"Ticket/Delete/{id}");
            return response;
        }
    }
}