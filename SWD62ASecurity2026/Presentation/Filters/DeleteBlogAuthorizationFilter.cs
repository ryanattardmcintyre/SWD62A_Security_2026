using DataAccess.Repositories;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Presentation.Filters
{
    public class DeleteBlogAuthorizationFilter : IAuthorizationFilter
    {

        private string _permission;
        public DeleteBlogAuthorizationFilter(string permission)
        {
            _permission= permission;
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var id = context.RouteData.Values["id"];
            if (id == null)
            {
                context.Result = new Microsoft.AspNetCore.Mvc.BadRequestResult();
                return;
            }

            foreach (var role in _permission.Split(','))
            {
                if (context.HttpContext.User.IsInRole(role))
                {
                    return;
                }
            }



            var user = context.HttpContext.User;
            if (user == null)
            {
                context.Result = new Microsoft.AspNetCore.Mvc.UnauthorizedResult();
                return;
            }

            if (context.HttpContext.User.Identity.IsAuthenticated == false)
            {
                context.Result = new Microsoft.AspNetCore.Mvc.UnauthorizedResult();
                return;
            }

            var myEvent = context.HttpContext.RequestServices.GetService<BlogRepository>().GetBlogs()
                .SingleOrDefault(x => x.Id == Convert.ToInt32(id));

            if (myEvent != null)
            {
                if (myEvent.AuthorEmail == user.Identity.Name)
                {
                    return;
                }
            }
            context.Result = new Microsoft.AspNetCore.Mvc.ForbidResult();
        }
    }
}
