using Microsoft.EntityFrameworkCore;

namespace Application.Core
{
    //used for any kind of list in our program
    public class PagedList<T> : List<T>
    {
        public PagedList(IEnumerable<T> items, int count, int pageNumber, int pageSize)
        {
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            PageSize = pageSize;
            TotalCount = count;
            AddRange(items);
        }

        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        //Static method to create a page list and return it
        //Retunt a Task of a paged list and the pagedList is going to be of type T for whatever we're paging here
        //Method called CreatesAsync with queryable variable because we are going to recieve our list of items before have been executed to a list in the DB
        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pageNumber,
            int pageSize)
        {
            var count = await source.CountAsync(); //number of items in list, this does query to DB
            //When implement pagin we are going to be making 2 queries to DB
            var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedList<T>(items, count, pageNumber, pageSize);
        }
    }
}