using System.Text.Json;

namespace API.Extensions
{
    public static class HttpExtensions
    {
        // Add a pagination header to any response that we send back from our API
        //Extend Http response
        public static void AddPaginationHeader(this HttpResponse response, int currentPage,
            int itemsPerPage, int totalItems, int totalPages)
        {
            var paginationHeader = new
            {
                currentPage,
                itemsPerPage,
                totalItems,
                totalPages
            };
            response.Headers.Add("Pagination", JsonSerializer.Serialize(paginationHeader));
            //Spelt exactly as it is there otherwise problems to run it and of course "Pagination" spelt exactly as what we're calling the header we're returning
            response.Headers.Add("Access-Control-Expose-Headers", "Pagination");
        }
    }
}