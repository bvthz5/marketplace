namespace MarketPlaceUser.Bussiness.Dto.Views
{
    public class PagerOffset<T>
    {
        public int TotalItems { get; private set; }

        public int PageSize { get; private set; }

        public bool HasNext { get; private set; }

        public bool HasPrevious { get; private set; }

        public List<T> Result { get; private set; }

        public PagerOffset(int index, int limit, int count, List<T> resultSet)
        {
            PageSize = limit < 1 ? 10 : limit;

            TotalItems = count;

            HasNext = index + limit < count;

            HasPrevious = index > 0;

            Result = resultSet;
        }
    }
}
