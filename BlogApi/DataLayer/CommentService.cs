using BlogApi.Models;
using BlogApi.Models.CommentsModels;
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
    public class CommentService : ICommentService
    {
        private string _config;

        public CommentService(IConfiguration configuration) =>
            _config = configuration.GetConnectionString("DefaultConnection");

        public async Task<List<CommentResponse>> CommentCreate(int UserId, CommentCreate commentCreate)
        {
            List<CommentResponse> commentResponseList = new List<CommentResponse>();
            using (SqlConnection conn = new SqlConnection(_config))
            {
                string CommandString = "INSERT INTO Comments CommText, CreateTime, PostId, UserId " +
                    "VALUES @CommText, @CreateTime, @PostId, UserId" +
                    "Set @ReturnId = SCOPE_IDENTITY()";
                using (SqlCommand cmd = new SqlCommand(CommandString, conn))
                {
                    cmd.Parameters.AddWithValue("@CommText", commentCreate.CommentText);
                    cmd.Parameters.AddWithValue("@CreateTime", commentCreate.CreateTime);
                    cmd.Parameters.AddWithValue("@PostId", commentCreate.PostId);
                    cmd.Parameters.AddWithValue("@UserId", UserId);

                    cmd.Parameters.Add("@ReturnId", SqlDbType.Int, 4);
                    cmd.Parameters["ReturnId"].Direction = ParameterDirection.Output;

                    try
                    {
                        if(conn.State == ConnectionState.Closed)
                            await conn.OpenAsync();
                        
                        IDataReader reader = await cmd.ExecuteReaderAsync();
                        var user = GetUserInfo(UserId);
                        
                        while (reader.Read())
                        {
                            commentResponseList.Add(new CommentResponse()
                            {
                                Users = user,
                                CommentId = Convert.ToInt32(cmd.Parameters["@ReturnId"].Value.ToString()),
                                CommentText = commentCreate.CommentText,
                                CreateTime = commentCreate.CreateTime,
                                PostId = commentCreate.PostId
                            });
                        }
                    }
                    catch(Exception ex)
                    {

                    }


                }
            }
            return commentResponseList;
        }

        public async Task<int> CommentDelete(int UserId, CommentDelete commentDelete)
        {
            int Result = 0;
            using (SqlConnection conn = new SqlConnection(_config))
            {
                string CommandString = "DELETE Comments Where @UserId = UserId AND " +
                    "@PostId = PostId AND @Id = Id";
                using (SqlCommand cmd = new SqlCommand(CommandString, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", commentDelete.Id);
                    cmd.Parameters.AddWithValue("@PostId", commentDelete.PostId);
                    cmd.Parameters.AddWithValue("@UserId", UserId);

                    try
                    {
                        if (conn.State == ConnectionState.Closed)
                            await conn.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();                        
                        Result = 1;
                    }
                    catch(Exception ex)
                    {

                    }
                }

            }
            return Result;

        }

        public async Task<bool> isTrue(int UserId, CommentDelete commentDelete)
        {
            bool Result = false;
            using (SqlConnection conn = new SqlConnection(_config))
            {
                using (SqlCommand cmd = new SqlCommand("Select Count(*) from Comments Where Id = @Id and UserId = @UserId And PostId = @PostId", conn))
                {
                    cmd.Parameters.AddWithValue("@Id", commentDelete.Id);
                    cmd.Parameters.AddWithValue("@Id", commentDelete.PostId);
                    cmd.Parameters.AddWithValue("@UserId", UserId);

                    try
                    {
                        if (conn.State == ConnectionState.Closed)
                            await conn.OpenAsync();
                        var Count = await cmd.ExecuteScalarAsync();

                        if (Convert.ToInt32(Count.ToString()) > 0)
                            Result = true;
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            return Result;
        }

        public IEnumerable<Comment> GetCommentsByPostId(Comment comment)
        {
            using (SqlConnection conn = new SqlConnection(_config))
            {
                string QueryString = "Select * From Comments Where PostId = @PostId";
                using (SqlCommand cmd = new SqlCommand(QueryString, conn))
                {
                    cmd.Parameters.AddWithValue("@PostId", comment.PostId);


                    if (conn.State == ConnectionState.Closed)
                        conn.Open();

                    IDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        var post = new Comment();
                        comment.Id = Convert.ToInt32(reader["Id"].ToString());
                        comment.CommentText = reader["Title"].ToString();
                        comment.CreateDate = Convert.ToDateTime(reader["CreateTime"].ToString()).ToString("f");
                        comment.PostId = Convert.ToInt32(reader["PostId"].ToString());
                        comment.UserId = Convert.ToInt32(reader["UserId"].ToString());

                        yield return post;
                    }
                }

            }
        }


        private List<UserResponce> GetUserInfo(int UserId)
        {
            List<UserResponce> users = new List<UserResponce>();
            using (SqlConnection conn = new SqlConnection(_config))
            {
                string CommandString = "Select FirstName, SecondName from Users Where Id = @UserId";
                using (SqlCommand cmd = new SqlCommand(CommandString, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", UserId);

                    try
                    {
                        if (conn.State == ConnectionState.Closed)
                            conn.Open();
                        IDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            users.Add(new UserResponce()
                            {
                                Id = UserId,
                                FirstName = reader["FirstName"].ToString(),
                                SecondName = reader["SecondName"].ToString()
                            });
                        }


                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            return users;
        }
    }
}
