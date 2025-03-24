using Application.Common.Interfaces;

namespace Api.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int? UserId
        {
            get
            {
                var claim = _httpContextAccessor.HttpContext?.User?.Claims.FirstOrDefault(c => c.Type == "id");

                if (claim == null)
                    return null;

                int id;

                if (int.TryParse(claim.Value, out id))
                {
                    return id;
                }
                return null;
            }
        }
    }
}
