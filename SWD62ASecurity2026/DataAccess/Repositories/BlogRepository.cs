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
using Microsoft.Extensions.Configuration;
using Domain.Exceptions;

namespace DataAccess.Repositories
{
    public class BlogRepository
    {
        private readonly BlogDbContext _context;
        private readonly IConfiguration _config;
        public BlogRepository(BlogDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
            _context.Database.SetConnectionString(config.GetConnectionString("BlogUserConnection"));
        }


        public IQueryable<Blog> GetBlogs()
        {
            //LINQ to Entities
            //good practice:
            //var todeleteBlog = _context.Blogs.ElementAt(0); //testing the permission applied with BlogUser
            //_context.Blogs.Remove(todeleteBlog);
            //_context.SaveChanges();


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
            while (myREader.Read())
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


        public void DeleteBlog(int id)
        {
            _context.Database.SetConnectionString(_config.GetConnectionString("DefaultConnection"));


            var blogToDelete = _context.Blogs.SingleOrDefault(x => x.Id == id);
            if (blogToDelete != null)
            {
                _context.Blogs.Remove(blogToDelete);
                _context.SaveChanges();
            }
        }

        //updates one permission on a blog


        public void UpdatePermissionOnBlog(SharingPermission permission)
        {
            _context.SharingPermissions.Add(permission);
            _context.SaveChanges();
        }

        //updates multiple permissions on a/multiple blogs
        //start transaction
        //1, ryanattard@gmail.com, READ
        //1, joeborg@gmail.com, READ
        //1, joeborg1@gmail.com, READ
        //1, ryanattard@gmail.com, READ //<<<<< an error will be raised reason: you run out of space
        //1, joeborg2@gmail.com, READ
        //1, joeborg3@gmail.com, READ
        //1, joeborg4@gmail.com, READ
        //1, joeborg5@gmail.com, READ
        //commit transaction 
        public void UpdatePermissionsOnBlog(SharingPermission [] permissions )
        {
            _context.Database.SetConnectionString(_config.GetConnectionString("DefaultConnection"));
            var transaction = _context.Database.BeginTransaction();
            try
            {
                foreach (var permission in permissions)
                {
                    UpdatePermissionOnBlog(permission);
                   // throw new Exception("Simulated error to test transactions");
                }
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                //log the exception
                throw new BlogsException("Error happened while updating permissions on blog");

            }

        }


        
    }
}