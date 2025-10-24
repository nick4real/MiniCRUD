using System.Text;
using System.Text.Json;

namespace MiniCRUD.WebApp.Services;

public class Response<D>
{
    public D? Data { get; set; }
    public string? Message { get; set; }
    public bool IsSuccess => Message == null;
}

public class HttpClientService
{
    private readonly HttpClient _httpClient;
    public HttpClientService(HttpClient httpClient)
    {
        _httpClient = httpClient;

        _httpClient.BaseAddress = new Uri(Environment.GetEnvironmentVariable("services__minicrud-api__http__0") 
            ?? Environment.GetEnvironmentVariable("services__minicrud_api__http__0")
            ?? throw new Exception("Empty URL"));
    }
    public async Task<Response<TResult>> GetAsync<TResult>(string relativeRoute)
    {
        return await RunAsync<TResult>(HttpMethod.Get, relativeRoute);
    }
    public async Task<Response<TResult>> PostAsync<TResult, TArgument>(string relativeRoute, TArgument arg)
    {
        return await RunAsync<TResult, TArgument>(HttpMethod.Post, relativeRoute, arg);
    }
    public async Task<Response<TResult>> PutAsync<TResult>(string relativeRoute)
    {
        return await RunAsync<TResult>(HttpMethod.Put, relativeRoute);
    }
    public async Task<Response<TResult>> PutAsync<TResult, TArgument>(string relativeRoute, TArgument arg)
    {
        return await RunAsync<TResult, TArgument>(HttpMethod.Put, relativeRoute, arg);
    }
    public async Task<Response<TResult>> DeleteAsync<TResult>(string relativeRoute)
    {
        return await RunAsync<TResult>(HttpMethod.Delete, relativeRoute);
    }

    private async Task<Response<TResult>> RunAsync<TResult>(HttpMethod httpMethod, string relativeRoute)
    {
        try
        {
            HttpResponseMessage response = new();

            switch (httpMethod.Method)
            {
                case "GET":
                    response = await _httpClient.GetAsync(relativeRoute);
                    break;
                case "POST":
                    response = await _httpClient.PostAsync(relativeRoute, null);
                    break;
                case "PUT":
                    response = await _httpClient.PutAsync(relativeRoute, null);
                    break;
                case "DELETE":
                    response = await _httpClient.DeleteAsync(relativeRoute);
                    break;
            }

            if (response.IsSuccessStatusCode)
            {
                TResult answer = JsonSerializer.Deserialize<TResult>(await response.Content.ReadAsStreamAsync(), new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                })!;
                return new Response<TResult> { Data = answer };
            }
            return new Response<TResult>() { Message = await response.Content.ReadAsStringAsync() };
        }
        catch (Exception ex)
        {
            return new Response<TResult>() { Message = ex.Message };
        }
    }

    private async Task<Response<TResult>> RunAsync<TResult, TArgument>(HttpMethod httpMethod, string relativeRoute, TArgument arg)
    {
        try
        {
            var json = JsonSerializer.Serialize(arg);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = new();

            switch (httpMethod.Method)
            {
                case "GET":
                    response = await _httpClient.GetAsync(relativeRoute);
                    break;
                case "POST":
                    response = await _httpClient.PostAsync(relativeRoute, content);
                    break;
                case "PUT":
                    response = await _httpClient.PutAsync(relativeRoute, content);
                    break;
                case "DELETE":
                    response = await _httpClient.DeleteAsync(relativeRoute);
                    break;
            }

            if (response.IsSuccessStatusCode)
            {
                TResult answer = JsonSerializer.Deserialize<TResult>(await response.Content.ReadAsStreamAsync(), new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                })!;
                return new Response<TResult> { Data = answer };
            }
            return new Response<TResult>() { Message = await response.Content.ReadAsStringAsync() };
        }
        catch (Exception ex)
        {
            return new Response<TResult>() { Message = ex.Message };
        }
    }
}
