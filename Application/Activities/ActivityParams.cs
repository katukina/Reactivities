using System;
using Application.Core;

namespace Application.Activities
{
    //Class used for filtering, inherit from PagingParams as that can be used for any kind of list to paginated the result
    public class ActivityParams : PagingParams
    {
        public bool IsGoing { get; set; }
        public bool IsHost { get; set; }
        public DateTime StartDate { get; set; } = DateTime.UtcNow;
    }
}