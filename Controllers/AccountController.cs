using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PracticeAPIs.Model;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;

namespace PracticeAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IConfiguration configuration;
        public AccountController(IConfiguration configuration) 
        {
            this.configuration = configuration;
        }

        [HttpPost]
        [Route("Create")]
        public IActionResult createAccount([FromBody] AccountModel account) 
        {
            Dictionary<string, dynamic> result;
            try
            {
                SqlConnection conn = new SqlConnection(configuration.GetConnectionString("DbConnection").ToString());
                string query = $"INSERT INTO tblAccount(firstName, middleName, lastName, userName, email, password) VALUES('{account.FirstName}', '{account.MiddleName}', '{account.LastName}', '{account.UserName}', '{account.Email}', '{account.Password}')";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();

                result = new Dictionary<string, dynamic>
                {
                    {"Result", true},
                    {"Message", "Account created successfully."},
                };


                return Ok(result);

            }
            catch (Exception e) 
            {
                result = new Dictionary<string, dynamic>
                        {
                            {"Result", false},
                            {"Message", $"Error creating account {e}" }
                        };
                return BadRequest(result);
            }
        }

        [HttpGet]
        [Route("Accounts")]
        public IActionResult GetAllAccount() 
        {
            Dictionary<string, dynamic> result;
            try 
            {
                SqlConnection conn = new SqlConnection(configuration.GetConnectionString("DbConnection").ToString());
                string query = "SELECT * FROM tblAccount";
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                List<AccountModel> accountList = new List<AccountModel>();

                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        AccountModel account = new AccountModel
                        {
                            FirstName = Convert.ToString(dt.Rows[i]["firstName"]),
                            MiddleName = Convert.ToString(dt.Rows[i]["middleName"]),
                            LastName = Convert.ToString(dt.Rows[i]["lastName"]),
                            UserName = Convert.ToString(dt.Rows[i]["userName"]),
                            Email = Convert.ToString(dt.Rows[i]["email"]),
                            Password = Convert.ToString(dt.Rows[i]["password"]),
                        };
                        accountList.Add(account);
                    }
                }
                if (accountList.Count > 0)
                {
                    result = new Dictionary<string, dynamic>
                        {
                            {"Result", true},
                            {"Message", accountList}
                        };
                    return Ok(result);
                }
                else 
                {
                    result = new Dictionary<string, dynamic>
                        {
                            {"Result", false},
                            {"Message", "No accounts found" }
                        };
                    return Ok(result);
                }
            }
            catch (Exception e) 
            {
                result = new Dictionary<string, dynamic>
                        {
                            {"Result", false},
                            {"Message", $"Error fetching accounts: {e}" }
                        };

                return BadRequest(result);
            }
        }
    }
}
