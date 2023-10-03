namespace MarketPlaceUser.Bussiness.Dto.Views
{
    public class SuccessMessageView
    {
        public string Message { get; set; }

        public SuccessMessageView(string message)
        {
            Message = message;
        }
    }
}
