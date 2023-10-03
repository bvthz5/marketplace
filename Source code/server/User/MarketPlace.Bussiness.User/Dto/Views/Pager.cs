namespace MarketPlaceUser.Bussiness.Dto.Views
{
    public class Pager<T>
    {
        public int TotalItems { get; private set; }

        public int TotalPages { get; private set; }

        public int CurrentPage { get; private set; }

        public int PageSize { get; private set; }

        public bool HasNext { get; private set; }

        public bool HasPrevious { get; private set; }

        public List<T> Result { get; private set; }

        public Pager(int pageNumber, int pageSize, List<T> resultSet)
        {
            CurrentPage = pageNumber < 0 ? 1 : pageNumber;

            PageSize = pageSize < 1 ? 10 : pageSize;

            TotalItems = resultSet.Count;

            TotalPages = (int)Math.Ceiling(TotalItems / (decimal)PageSize);

            HasNext = CurrentPage < TotalPages;

            HasPrevious = 1 < CurrentPage;

            Result = resultSet.Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize).ToList();
        }
    }
}
