namespace MarketPlaceUser.Bussiness.Dto.Views
{
    public class ErrorMessageView
    {
        public string ErrorMessage { get; set; }

        public ErrorMessageView(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }
    }
}
