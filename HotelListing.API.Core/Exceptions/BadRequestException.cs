namespace HotelListing.API.Core.Exceptions
{
    public class BadRequestException : ApplicationException
    {
        public BadRequestException(string info) : base($"Bad request ! {info}")
        {

        }
    }
}
