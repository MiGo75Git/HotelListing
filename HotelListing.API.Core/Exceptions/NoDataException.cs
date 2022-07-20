namespace HotelListing.API.Core.Exceptions
{
    public class NoDataException : ApplicationException
    {
        public NoDataException(string name) : base($"No data present in {name}.")
        {

        }
    }
}
