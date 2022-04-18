using BlogApi.Models;
using BlogApi.Models.Admins;
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
    public class AdminService : IAdminServise
    {
        private string _config;

        public AdminService(IConfiguration configuration) =>
            _config = configuration.GetConnectionString("DefaultConnection");

        public IEnumerable<AdminResponsePerDate> GetPostPerDate(AdminsRequestsDate model)
        {
            using (SqlConnection conn = new SqlConnection(_config))
            {
                string query = "Select Count(*) as CountPosts from Posts where CreateTime = @Date";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Date", model.Date);

                    if (conn.State == ConnectionState.Closed)
                        conn.Open();

                    IDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        var AdminResponse = new AdminResponsePerDate();

                        AdminResponse.Date = model.Date;
                        AdminResponse.CountPostPerday = int.Parse(reader["CountPosts"].ToString());

                        yield return AdminResponse;
                    }
                }
            }
        }

        public List<AdminsResponsePerTags> GetPostPerTags(AdminsRequestTags model)
        {
            List<AdminsResponsePerTags> tagResponse = new List<AdminsResponsePerTags>();
            using (SqlConnection conn = new SqlConnection(_config))
            {
                foreach(string tag in model.Tags)
                {
                    string query = "Select Count(*) as CountPosts from Posts join PostsTags on PostsTags.PostId = Posts.Id join Tags on Tags.Id = PostsTags.TagsId and Tags.title = @Tags";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Tags", tag);

                        try
                        {
                            if (conn.State == ConnectionState.Closed)
                                conn.Open();

                            var reader = cmd.ExecuteReader();
                            while (reader.Read())
                            {
                                tagResponse.Add(new AdminsResponsePerTags()
                                {
                                    Tags = tag,
                                    CountPosts = Convert.ToInt32(reader["CountPosts"].ToString())
                                });
                            }
                            
                        }
                        catch(Exception ex)
                        {

                        }
                        
                    }
                }               

            }
            return tagResponse;
        }

        public IEnumerable<AdminResponsePerUsers> GetPostPerUser(int UserId)
        {            
            using (SqlConnection conn = new SqlConnection(_config))
            {
                string query = "select Id, FirstName, SecondName, (Select Count(*) from Posts where UserId = @UserId) " +
                    "as CountPost from Users";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", UserId);

                    if (conn.State == ConnectionState.Closed)
                        conn.Open();

                    IDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        var AdminResponse = new AdminResponsePerUsers();

                        AdminResponse.Users = new List<UserResponce>();
                        AdminResponse.Users.Add(new UserResponce()
                        {
                            Id = int.Parse(reader["Id"].ToString()),
                            FirstName = reader["FirstName"].ToString(),
                            SecondName = reader["SecondName"].ToString()
                        });
                        AdminResponse.CountPostPerUser = int.Parse(reader["CountPost"].ToString());

                        yield return AdminResponse;
                    }
                }
            }

        }
    }
}
