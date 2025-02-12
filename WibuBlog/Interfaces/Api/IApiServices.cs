﻿namespace WibuBlog.Interfaces.Api
{
    public interface IApiServices
    {
        Task<T> GetAsync<T>(string endpoint);
        Task<T> PostAsync<T>(string endpoint, object data);
        Task<T> PutAsync<T>(string endpoint, object data);
        Task<bool> DeleteAsync(string endpoint);
    }
}
