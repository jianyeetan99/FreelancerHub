using System.Data;
using FreelancerHub.Domain.Models.AppSettings;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace FreelancerHub.Infrastructure.Common;

public class DapperContext(IOptionsMonitor<ConnectionStrings> connectionStrings)
{
    private readonly ConnectionStrings _connectionStrings = connectionStrings.CurrentValue;

    public IDbConnection Freelancer() => new SqlConnection(_connectionStrings.DefaultConnection);
}