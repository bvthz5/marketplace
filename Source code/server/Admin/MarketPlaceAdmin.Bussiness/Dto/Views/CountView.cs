namespace MarketPlaceAdmin.Bussiness.Dto.Views
{
    public class CountView
    {
        public string Property { get; set; }

        public int Count { get; set; }

        public CountView(string property, int count)
        {
            Property = property;
            Count = count;
        }
    }
}
