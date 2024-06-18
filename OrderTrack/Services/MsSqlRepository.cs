using OrderTrack.Models;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;

namespace OrderTrack.Services
{
    public class MsSqlRepository
    {
        private readonly string _connectionString =  Global.MsSqlConnectionString;
        string SqlGetUserQuery = @"SELECT e.CodeUser, NameUser, BarCode, Login, PassWord, CodeProfile AS TypeUser 
FROM dbo.V1C_employee e
WHERE e.IsWork= 1 and  e.CodeUser NOT IN 
(SELECT e.CodeUser FROM  dbo.V1C_employee e where e.IsWork= 1 GROUP by CodeUser HAVING count(*)>1)
";
        public IEnumerable<User> SqlGetUser()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                return connection.Query<User>(SqlGetUserQuery).ToList();
            }
        }

    }
}
