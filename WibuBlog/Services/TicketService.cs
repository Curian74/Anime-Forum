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

        public async Task<List<Ticket>> GetUserTicketsAsync(Guid userId)
        {
            try
            {
                var response = await _apiService.GetAsync<ApiResponse<object>>("Ticket/GetUserTickets/UserTickets");
                // Check if we got a response
                if (response?.Value == null)
                    return new List<Ticket>();

                // Try to extract the tickets property using reflection
                var valueType = response.Value.GetType();
                var ticketsProp = valueType.GetProperty("tickets");

                if (ticketsProp != null)
                {
                    var ticketsObj = ticketsProp.GetValue(response.Value);
                    if (ticketsObj is List<Ticket> tickets)
                        return tickets;
                }

                // If we couldn't extract tickets property, log an error
                Console.WriteLine("Could not extract tickets from API response");
                return new List<Ticket>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetUserTicketsAsync: {ex.Message}");
                return new List<Ticket>();
            }
        }

        public async Task<Ticket> GetTicketByIdAsync<T>(T id)
        {
            try
            {
                var guidId = Guid.Parse(id.ToString());
                var response = await _apiService.GetAsync<ApiResponse<Ticket>>($"Ticket/GetTicketDetail/{guidId}");
                return response?.Value;
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
            var response = await _apiService.PutAsync<ApiResponse<bool>>($"Ticket/CloseTicket/Close/{id}", null);
            return response != null && response.Value;
        }

        public async Task<bool> DeleteTicketAsync<T>(T id)
        {
            var response = await _apiService.DeleteAsync($"Ticket/Delete/{id}");
            return response;
        }
    }
}