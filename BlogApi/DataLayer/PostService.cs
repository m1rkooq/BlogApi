using BlogApi.Models.PostsModels;
using BlogApi.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using BlogApi.Models.TagsModels;
using BlogApi.Models;

namespace BlogApi.DataLayer
{
    
    public class PostService : IPostService
    {
        private string _config;

        public PostService(IConfiguration configuration)
        {
            _config = configuration.GetConnectionString("DefaultConnection");
        }


        public async Task<List<PostResponse>> CreatePost(int UserId, PostCreate postCreate)
        {           
            List<PostResponse> postList = new List<PostResponse>();
            using (SqlConnection conn = new SqlConnection(_config))
            {                
                string CommandString = "INSERT INTO Posts(Title, Content, CreateTime, UpdateTime, UserId)" +
                    "VALUES (@Title, @Content, @CreateTime, @UpdateTime, @UserId)" +
                    "Set @ReturnId = SCOPE_IDENTITY()";
                using (SqlCommand cmd = new SqlCommand(CommandString, conn))
                {
                    cmd.Parameters.AddWithValue("@Title", postCreate.Title);
                    cmd.Parameters.AddWithValue("@Content", postCreate.Content);
                    cmd.Parameters.AddWithValue("@CreateTime", postCreate.CreatedDate);
                    cmd.Parameters.AddWithValue("@UpdateTime", postCreate.UpdatedDate);
                    cmd.Parameters.AddWithValue("@UserId", UserId);

                    cmd.Parameters.Add("@ReturnId", SqlDbType.Int, 4);
                    cmd.Parameters["@ReturnId"].Direction = ParameterDirection.Output;

                    try
                    {
                        if (conn.State == ConnectionState.Closed)
                            await conn.OpenAsync();
                        await cmd.ExecuteScalarAsync();
                        var PostId = Int32.Parse(cmd.Parameters["@ReturnId"].Value.ToString());

                        if(PostId != 0)
                        {
                            var result = await CreateTags(PostId, postCreate.Tags);
                            var user = GetUserInfo(UserId);
                            postList.Add(new PostResponse
                            {
                                User = user,
                                Id = PostId,
                                Content = postCreate.Content,
                                CreatedDate = postCreate.CreatedDate.ToString(),
                                UpdateDate = postCreate.UpdatedDate.ToString(),
                                Tags = result
                            });
                        }
                        return postList;
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            return null;
        }

        

        public async Task<List<PostResponse>> UpdatePost(int UserId, PostUpdate postUpdate)
        {
            List<PostResponse> postList = new List<PostResponse>();
            using (SqlConnection conn = new SqlConnection(_config))
            {
                string CommandString = "UPDATE Posts SET " +
                    "Title = @Title, " +
                    "Content = @Content, " +
                    "UpdateTime = @UpdateTime " +
                    "WHERE Id = @Id And " +
                    "UserId = @UserId";

                using (SqlCommand cmd = new SqlCommand(CommandString, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", postUpdate.Id);
                    cmd.Parameters.AddWithValue("@Title", postUpdate.Title);
                    cmd.Parameters.AddWithValue("@Content", postUpdate.Content);
                    cmd.Parameters.AddWithValue("@UpdateTime", postUpdate.UpdateDate);
                    cmd.Parameters.AddWithValue("@UserId", UserId);
                    try
                    {
                        if (conn.State == ConnectionState.Closed)
                            await conn.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();
                        var user = GetUserInfo(UserId);
                        postList.Add(new PostResponse
                        {
                            User = user,
                            Id = postUpdate.Id,
                            Content = postUpdate.Content,                            
                            UpdateDate = postUpdate.UpdateDate.ToString("f"),
                            Tags = GetTagList(postUpdate.Id)
                        });
                    }
                    catch (Exception ex)
                    {
                        
                    }
                }
            }
            return postList;
        }

        public async Task<int> DeletePost(int UserId, PostDelete postDelete)
        {
            int Return = 0;
            using (SqlConnection conn = new SqlConnection(_config))
            {
                string CommandString = "Delete Posts Where Id = @Id and UserId = @UserId";
                using (SqlCommand cmd = new SqlCommand(CommandString, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", postDelete.Id);
                    cmd.Parameters.AddWithValue("@UserId", UserId);                  

                    try
                    {
                        if (conn.State == ConnectionState.Closed)
                            await conn.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();
                        Return = 1;

                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            return Return;
        }



        public IEnumerable<GetPostPesponse> GetPostByUserId(int UserId)
        {            
            using (SqlConnection conn = new SqlConnection(_config))
            {
                string QueryString = "Select u.Id as UserId, u.FirstName, u.SecondName, p.Id as PostId ,p.Title , p.Content ,p.CreateTime, p.UpdateTime, " +
                    "(Select Count(*) from Comments where Comments.PostId = @PostId) as CommentCount, (Select Count(*) from Likes where PostId = @PostId) as CountLike from Users as u, Posts as p" +
                    "Where Posts.UserId = @UserId";
                using (SqlCommand cmd = new SqlCommand(QueryString, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", UserId);

                    
                    if(conn.State == ConnectionState.Closed)
                        conn.Open();
                    
                    IDataReader reader = cmd.ExecuteReader();                    
                    while (reader.Read())
                    {                        
                        var post = new GetPostPesponse();

                        post.user = new List<UserResponce>();
                        post.user.Add(new UserResponce() {
                            Id = UserId,
                            FirstName = reader["FirstName"].ToString(),
                            SecondName = reader["SecondName"].ToString()
                        });
                        post.Id = Convert.ToInt32(reader["PostId"].ToString());
                        post.Title = reader["Title"].ToString();
                        post.Content = reader["Content"].ToString();
                        post.CreatedDate = Convert.ToDateTime(reader["CreateTime"].ToString()).ToString("f");
                        post.UpdateDate = Convert.ToDateTime(reader["UpdateTime"].ToString()).ToString("f");
                        post.CountComments = Convert.ToInt32(reader["CommentCount"].ToString());
                        post.CountLike = Convert.ToInt32(reader["CountLike"].ToString());
                        post.TagList = GetTagList(post.Id);

                       /* post.TagList = new List<Tags>();
                        post.TagList.Add(new Tags()
                        {
                            Id = Convert.ToInt32(reader["TagId"].ToString()),
                            TagName = reader["title"].ToString()
                        });*/
                        yield return post;
                    }
                }
            }
        }

        public IEnumerable<GetPostPesponse> GetPostByUserIdAndPostId(int UserId, int PostId)
        {
            using (SqlConnection conn = new SqlConnection(_config))
            {
                string QueryString = "Select u.Id as UserId, u.FirstName, u.SecondName, p.Id as PostId ,p.Title , p.Content ,p.CreateTime, p.UpdateTime, " +
                    "(Select Count(*) from Comments where Comments.PostId = @PostId) as CommentCount, (Select Count(*) from Likes where PostId = @PostId) as CountLike from Users as u, Posts as p" +
                    "Where Posts.UserId = @UserId and Posts.Id = @PostId";
                using (SqlCommand cmd = new SqlCommand(QueryString, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", UserId);
                    cmd.Parameters.AddWithValue("@PostId", PostId);

                    if (conn.State == ConnectionState.Closed)
                        conn.Open();                    
                    IDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        var post = new GetPostPesponse();
                        post.user = new List<UserResponce>();
                        post.user.Add(new UserResponce()
                        {
                            Id = UserId,
                            FirstName = reader["FirstName"].ToString(),
                            SecondName = reader["SecondName"].ToString()
                        });
                        post.Id = Convert.ToInt32(reader["Id"].ToString());
                        post.Title = reader["Title"].ToString();
                        post.Content = reader["Content"].ToString();
                        post.CreatedDate = Convert.ToDateTime(reader["CreateTime"].ToString()).ToString("f");
                        post.UpdateDate = Convert.ToDateTime(reader["UpdateTime"].ToString()).ToString("f");
                        post.CountComments = Convert.ToInt32(reader["CommentCount"].ToString());
                        post.CountLike = Convert.ToInt32(reader["CountLike"].ToString());
                        post.TagList = GetTagList(PostId);

                        yield return post;
                    }
                }
            }
        }

        public async Task<bool> isTrue(int PostId, int UserId)
        {
            bool res = false;
            using (SqlConnection conn = new SqlConnection(_config))
            {
                using (SqlCommand cmd = new SqlCommand("Select Count(*) from Posts Where Id = @Id and UserId = @UserId", conn))
                {
                    cmd.Parameters.AddWithValue("@Id", PostId);
                    cmd.Parameters.AddWithValue("@UserId", UserId);

                    try
                    {
                        if (conn.State == ConnectionState.Closed)
                            await conn.OpenAsync();
                        var Count = await cmd.ExecuteScalarAsync();
                        
                        if (Convert.ToInt32(Count.ToString()) > 0)
                            res = true;
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            return res;
        }


        private async Task<List<Tags>> CreateTags(int PostId, string tagsString)
        {
            List<Tags> tags = new List<Tags>();
            using (SqlConnection conn = new SqlConnection(_config))
            {
                foreach (string tag in tagsString.Split('#'))
                {
                    string CommandString = "INSERT INTO Tags(title) VALUES (@Title) " +
                        "Set @ReturnId = Scope_identity()";
                    using (SqlCommand cmd = new SqlCommand(CommandString, conn))
                    {
                        cmd.Parameters.AddWithValue("@Title", tag);

                        cmd.Parameters.Add("@ReturnId", SqlDbType.Int, 4);
                        cmd.Parameters["@ReturnId"].Direction = ParameterDirection.Output;

                        try
                        {
                            if (conn.State == ConnectionState.Closed)
                                await conn.OpenAsync();
                            await cmd.ExecuteNonQueryAsync();

                            var TagsId = Int32.Parse(cmd.Parameters["@ReturnId"].Value.ToString());

                            tags.Add(new Tags()
                            {
                                Id = TagsId,
                                TagName = tag,
                                //PostId = PostId
                            });
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
                await AddInPostTags(PostId, tags);

            }
            return tags;
        }


        private async Task AddInPostTags(int PostId, List<Tags> tagList)
        {
            using (SqlConnection conn = new SqlConnection(_config))
            {
                //for(int i = 0; i < tagList.Count; i++)
                foreach (Tags tag in tagList)
                {
                    string CommandString = "INSERT INTO PostsTags(TagsId, PostId) VALUES (@TagsId, @PostId) ";
                    using (SqlCommand cmd = new SqlCommand(CommandString, conn))
                    {
                        cmd.Parameters.AddWithValue("@TagsId", tag.Id);
                        cmd.Parameters.AddWithValue("@PostId", PostId);

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

        private List<Tags> GetTagList(int PostId)
        {
            List<Tags> tags = new List<Tags>();
            using (SqlConnection conn = new SqlConnection(_config))
            {
                string CommandString = " Select Tags.Id, Tags.title from PostsTags " +
                    "join Tags on PostsTags.TagsId = Tags.Id and PostsTags.PostId = @PostId";
                using (SqlCommand cmd = new SqlCommand(CommandString, conn))
                {
                    cmd.Parameters.AddWithValue("@PostId", PostId);

                    try
                    {
                        if (conn.State == ConnectionState.Closed)
                            conn.Open();
                        IDataReader reader = cmd.ExecuteReader();
                        while(reader.Read())
                        {
                            tags.Add(new Tags()
                            {
                                Id = Convert.ToInt32(reader["Id"].ToString()),
                                TagName = reader["title"].ToString()                               
                            });
                        }

                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            return tags;
        }
        private List<UserResponce> GetUserInfo(int UserId)
        {
            List<UserResponce> tags = new List<UserResponce>();
            using (SqlConnection conn = new SqlConnection(_config))
            {
                string CommandString = " Select Id, FirstName, SecondName from Users where Id = @UserId";
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
                            tags.Add(new UserResponce()
                            {
                                Id = Convert.ToInt32(reader["Id"].ToString()),
                                FirstName = reader["FirstName"].ToString(),
                                SecondName = reader["SecondName"].ToString(),
                            });
                        }

                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            return tags;
        }

    }
}
