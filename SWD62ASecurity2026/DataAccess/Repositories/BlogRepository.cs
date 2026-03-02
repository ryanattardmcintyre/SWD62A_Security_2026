using DataAccess.Context;
using Domain.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Context;

namespace DataAccess.Repositories
{
    public class BlogRepository
    {
        private readonly BlogDbContext _context;
        public BlogRepository(BlogDbContext context)
        {            
            _context = context;
        }


        public IQueryable<Blog> GetBlogs()
        {
            //LINQ to Entities
            //good practice:
            return _context.Blogs;
        }

        public IQueryable<Blog> GetBlogs(string keyword)
        {
            //bad practice:
            List<Blog> myREtrievedBlogs = new List<Blog>();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            _context.Database.OpenConnection();
            cmd.Parameters.AddWithValue("@keyword", keyword);

            SqlDataReader myREader = cmd.ExecuteReader();
            while(myREader.Read())
            {
                Blog b = new Blog();
                b.Id = (int)myREader["Id"];
                b.Title = (string)myREader["Title"];
                b.Content = (string)myREader["Content"];
                b.CreatedAt = (DateTime)myREader["CreatedAt"];
                b.AuthorEmail = (string)myREader["AuthorEmail"];
                b.Public = (bool)myREader["Public"];
                b.FilePath = (string)myREader["FilePath"];
                myREtrievedBlogs.Add(b);
            }
            _context.Database.CloseConnection();

            return myREtrievedBlogs.AsQueryable();
        }

        /*public bool DoesUserExist(string email, string password)
        {
            SqlCommand cmd =
               new SqlCommand("SELECT Count(*) FROM Users Where Email =@email and password =@password", new SqlConnection("YourConnectionString"));
            _context.Database.OpenConnection();
            
            cmd.Parameters.AddWithValue("@email", email);
            cmd.Parameters.AddWithValue("@password", password);
            
            int count = (int)cmd.ExecuteScalar();
            _context.Database.CloseConnection();

            return count == 1 ? true : false;


            //"SELECT Count(*) FROM Users Where Email ='admin' and password ='' OR 1=1;--'"
            //email: admin
            //password: ' OR 1=1;--
        }*/

        public void AddBlog(Blog b)
        {
            _context.Blogs.Add(b);
            _context.SaveChanges();
        }
    }
}
