namespace ClothBackend.DAL
{
    public  class BaseDAL
    {
        public static IConfiguration _configuration;

        public BaseDAL(IConfiguration configuration)
        {
            _configuration = configuration;
        }
    }
}
