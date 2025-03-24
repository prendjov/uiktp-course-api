namespace DTO.Pagination
{
    public class PaginationFilter
    {
        private int _pageSize;
        private int _pageNumber;

        public int PageSize
        {
            get
            {
                return _pageSize;
            }

            set
            {
                _pageSize = value > 0 && value <= 100 ? value : 100;
            }
        }

        public int PageNumber
        {
            get
            {
                return _pageNumber;
            }

            set
            {
                _pageNumber = value >= 1 ? value : 1;
            }
        }
    }
}
