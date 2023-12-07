using API.Helpers;
using Microsoft.AspNetCore.Http;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace API.Extensions
{
    public static class HttpExtensions
    {
        public static void AddPaginationHeader(this HttpResponse respose, PaginationHeader header) {
            var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            respose.Headers.Add("Pagination", JsonSerializer.Serialize(header, jsonOptions));
            respose.Headers.Add("Access-Control-Expose-Headers", "Pagination");
        }
    }
}
