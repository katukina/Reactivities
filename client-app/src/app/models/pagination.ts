//Properties that we get back
export interface Pagination {
    currentPage: number;
    itemsPerPage: number;
    totalItems: number;
    totalPages: number;
}
// Be able to use for anything then generic type parameter
export class PaginatedResult<T> {
    data: T; //in our case is going to be an activity array inside our paginated results. Axios provides us with a wat to interegote a response and do soemthing
    pagination: Pagination;

    constructor(data: T, pagination: Pagination) {
        this.data = data;
        this.pagination = pagination;
    }
}

//for pass to API those parameters
export class PagingParams {
    pageNumber;
    pageSize;

    constructor(pageNumber = 1, pageSize = 3) {
        this.pageNumber = pageNumber;
        this.pageSize = pageSize;
    }
}