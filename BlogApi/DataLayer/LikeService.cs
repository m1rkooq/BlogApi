using BlogApi.Models.LikesModels;
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
    public class LikeService : ILikeService
    {
        private string _config;

        public LikeService(IConfiguration configuration) =>
            _config = configuration.GetConnectionString("DefaultConnection");

        public async Task<int> GetCountLikeByPostId(Likes model)
        {
            int Result = 0;
            using (SqlConnection conn = new SqlConnection(_config))
            {
                string QueryString = "Select Count(*) From Likes WHERE PostId = @PostId";
                using (SqlCommand cmd = new SqlCommand(QueryString, conn))
                {
                    cmd.Parameters.AddWithValue("@PostId", model.PostId);

                    try
                    {
                        if (conn.State == ConnectionState.Closed)
                            await conn.OpenAsync();
                        var Count = await cmd.ExecuteScalarAsync();                        
                        Result = Convert.ToInt32(Count.ToString());
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            return Result;
        }

        public async Task<bool> LikeCheck(int UserId, Likes model)
        {
            bool Result = false;
            using (SqlConnection conn = new SqlConnection(_config))
            {
                string QueryString = "Select Count(*) From Likes WHERE UserId = @UserId AND PostId = @PostId";
                using (SqlCommand cmd = new SqlCommand(QueryString, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", UserId);
                    cmd.Parameters.AddWithValue("@PostId", model.PostId);

                    cmd.Parameters.Add("@ReturnId", SqlDbType.Int, 4);
                    cmd.Parameters["@ReturnId"].Direction = ParameterDirection.Output;
                    try
                    {
                        if (conn.State == ConnectionState.Closed)
                            await conn.OpenAsync();
                        var Count = await cmd.ExecuteScalarAsync();                        
                        if (Convert.ToInt32(Count.ToString()) == 1)
                        {
                            //model.LikeId = Int32.Parse(cmd.Parameters["@ReturnId"].Value.ToString());
                            await UnLikePost(UserId, model);
                            Result = true;                            
                        }

                        await LikePost(UserId, model);
                    }
                    catch (Exception ex)
                    {

                    }
                }


            }
            
            return Result;
        }


        public async Task LikePost(int UserId, Likes model)
        {
            using (SqlConnection conn = new SqlConnection(_config))
            {
                string CommandString = "INSERT INTO Likes (PostId, UserId) " +
                    "VALUES(@PostId, @UserId) " +
                    "Set @ReturnId = SCOPE_IDENTITY()";
                using (SqlCommand cmd = new SqlCommand(CommandString, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", UserId);
                    cmd.Parameters.AddWithValue("@PostId", model.PostId);

                    cmd.Parameters.Add("@ReturnId", SqlDbType.Int, 4);
                    cmd.Parameters["@ReturnId"].Direction = ParameterDirection.Output;

                    try
                    {
                        if (conn.State == ConnectionState.Closed)
                            await conn.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();

                        model.LikeId = Int32.Parse(cmd.Parameters["@ReturnId"].Value.ToString());
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
        }

        public async Task UnLikePost(int UserId, Likes model)
        {            
            using (SqlConnection conn = new SqlConnection(_config))
            {
                string CommandString = "DELETE FROM Likes " +
                    "WHERE UserId = @UserId AND PostId = @PostId";
                using (SqlCommand cmd = new SqlCommand(CommandString, conn))
                {                    
                    cmd.Parameters.AddWithValue("@UserId", UserId);
                    cmd.Parameters.AddWithValue("@PostId", model.PostId);

                    /*cmd.Parameters.Add("@ReturnId", SqlDbType.Int, 4);
                    cmd.Parameters["@ReturnId"].Direction = ParameterDirection.Output;*/

                    try
                    {
                        if (conn.State == ConnectionState.Closed)
                            await conn.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();                        
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
        }
    }
}
