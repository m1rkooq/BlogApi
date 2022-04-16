﻿using BlogApi.Models;
using BlogApi.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Microsoft.IdentityModel.Tokens;
using BlogApi.Helper;

namespace BlogApi.DataLayer
{
   
    public class UserService : IUserService
    {
        private IConfiguration _configuration;
       
        public UserService(IConfiguration configuration)
        {
            _configuration = configuration;           
        }

        public User Autheticate(UserRequest userRequest)
        {
            User user = null;
            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                
                using (SqlCommand cmd = new SqlCommand("Select * From Users Where Email = @Email And UserPassword = @Password;", conn))
                {
                    cmd.Parameters.AddWithValue("@Email", userRequest.Email);
                    cmd.Parameters.AddWithValue("@Password", userRequest.Password);

                    try
                    {
                        if (conn.State == ConnectionState.Closed)
                            conn.Open();
                        var reader = cmd.ExecuteReader();
                        if (reader.HasRows)
                        {
                            DataTable table = new DataTable();
                            table.Load(reader);
                            reader.Close();
                            var userRow = table.Rows[0];
                            user = new User()
                            {
                                Id = (int)userRow["Id"],
                                FirstName = (string)userRow["FirstName"],
                                Email = (string)userRow["Email"],
                                Role = (string)userRow["RoleName"]
                            };
                            
                        }
                    }
                    catch (Exception ex)
                    {

                    }

                }
            }
            return user;
        }
        

        public async Task<int> Register(UserRegister userRegister)
        {
            int Id = 0;
            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            { 
                using (SqlCommand cmd = new SqlCommand("INSERT INTO Users(FirstName, SecondName, Email, UserPassword) " +
                    "VALUES (@FirstName, @SecondName, @Email, @Password) " +
                    "Set @ReturnId = SCOPE_IDENTITY()", conn))
                {
                    cmd.Parameters.AddWithValue("@FirstName", userRegister.FirstName);
                    cmd.Parameters.AddWithValue("@SecondName", userRegister.SecondName);
                    cmd.Parameters.AddWithValue("@Email", userRegister.Email);
                    cmd.Parameters.AddWithValue("@Password", userRegister.Password);

                    cmd.Parameters.Add("@ReturnId", SqlDbType.Int, 4);
                    cmd.Parameters["@ReturnId"].Direction = ParameterDirection.Output;
                    try
                    {
                        if(conn.State == ConnectionState.Closed)
                            await conn.OpenAsync();
                        cmd.ExecuteNonQuery();
                        Id = Int32.Parse(cmd.Parameters["@ReturnId"].Value.ToString());                        
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            return Id;
        }
        
    }
}
