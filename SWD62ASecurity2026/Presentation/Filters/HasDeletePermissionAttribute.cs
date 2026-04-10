using Microsoft.AspNetCore.Mvc;

namespace Presentation.Filters
{

    //Note: i called it differently from the original filter class because
    //      when you apply an attribute you don't write down the "Attribute" part.
    // DeleteBlogAuthorizationFilterAttribute => DeleteBlogAuthorizationFilter which clashes with the original filter
    public class HasDeletePermissionAttribute : TypeFilterAttribute
    {


        public HasDeletePermissionAttribute() 
            : base(typeof(DeleteBlogAuthorizationFilter))
        {
        }
    }
}
