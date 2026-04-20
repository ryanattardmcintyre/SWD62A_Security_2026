using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    public class BlogsException: Exception
    {
        public BlogsException(): base("An error occured while processing Blog data")
        {
        }
        public BlogsException(string message) : base(message)
        {
        }
         public BlogsException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
