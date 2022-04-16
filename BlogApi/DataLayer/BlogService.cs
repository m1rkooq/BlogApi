using BlogApi.Models.BlogsModels;
using BlogApi.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApi.DataLayer
{
    public class BlogService : IBlogServise
    {
        private string _config;

        public BlogService(IConfiguration configuration) =>
            _config = configuration.GetConnectionString("DefaultConnection");

        public async Task<List<Blog>> GetUserBlog(int UserId)
        {
            List<Blog> result = new List<Blog>();
            using (SqlConnection conn = new SqlConnection(_config))
            {
                string query = "select Users.FirstName, Users.SecondName, (SELECT Count(*) From Posts WHERE Posts.UserId = Users.Id) as postsCount from Users Where Users.Id = @UserId";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", UserId);               

                    try
                    {
                        if (conn.State == ConnectionState.Closed)
                           await conn.OpenAsync();                        

                        IDataReader reader = await cmd.ExecuteReaderAsync();
                        while (reader.Read())
                        {
                            result.Add(new Blog()
                            {
                                Id = UserId,
                                FirstName = reader["FirstName"].ToString(),
                                SecondName = reader["SecondName"].ToString(),
                                CountPosts = int.Parse(reader["postsCount"].ToString())
                            });
                        }
                    }
                    catch(Exception ex)
                    {
                        
                    }  
                }
            }
            return result;
        }
    }
}
