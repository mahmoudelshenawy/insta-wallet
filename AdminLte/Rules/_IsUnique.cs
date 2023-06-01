using AdminLte.Data;
using AdminLte.Data.Entities;
using Dapper;
using DocumentFormat.OpenXml.ExtendedProperties;
using DocumentFormat.OpenXml.InkML;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Stripe;
using System.ComponentModel.DataAnnotations;

namespace AdminLte.Rules
{
    public class _IsUnique : ValidationAttribute
    {
        private readonly string _tableName;

        public _IsUnique(string tableName)
        {
            _tableName = tableName;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            try
            {
                var test = Startup.ConnectionString;
                var attribute = validationContext.MemberName;

                var tableName = new SqlParameter(_tableName, System.Data.SqlDbType.VarChar);
                var ColumnName = new SqlParameter(attribute, System.Data.SqlDbType.VarChar);
                var ColumnValue = new SqlParameter((string)value, System.Data.SqlDbType.VarChar);

                using (var scope = Startup._service.CreateScope())
                {

                    var context = scope.ServiceProvider.GetRequiredService<DapperDbContext>();
                    var query = @$"IF OBJECT_ID (N'{tableName}', N'U') IS NOT NULL 
                                     IF COL_LENGTH('{tableName}', 'Name') IS NOT NULL
                                     BEGIN
                                        SELECT COUNT(*) AS res FROM {tableName} WHERE {ColumnName} = '{ColumnValue}';
                                     END;
                                     ELSE
                                        SELECT 0 AS res;  
                                     ELSE 
                                        SELECT 0 AS res; ---Table Or Column Not Exists";
                    using (var connection = context.CreateConnection())
                    {
                        var exists = connection.Query<int>(query).First();
                    }
                }


                using (var scope = Startup._service.CreateScope())
                {
                    using var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    Type Test = validationContext.ObjectType;
                    var result = context.Database.ExecuteSqlRaw(@$"IF NOT EXISTS (SELECT * FROM 
                                               {tableName} WHERE {ColumnName} LIKE '{ColumnValue}')
                                                      BEGIN
                                                      PRINT 'pass validation';
                                                      END
                                                      ELSE
                                                      BEGIN
                                                      THROW 51000, 'The record exists.', 1;  
                                                      END");
                    if (result < 0)
                    {
                        return ValidationResult.Success;
                    }

                }
                return ValidationResult.Success;

            }
            catch (Exception)
            {
                return new ValidationResult("the name should be unique");
            }

        }


    }
}
