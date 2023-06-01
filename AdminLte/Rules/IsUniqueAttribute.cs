using AdminLte.Data;
using AdminLte.ViewModels;
using Dapper;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;

namespace AdminLte.Rules
{
    public class IsUniqueAttribute : ValidationAttribute
    {
        private readonly string _tableName;
        private readonly string _columnName;
        private readonly string _exculdeSelf;

        public IsUniqueAttribute(string tableName , string columnName = null , string excludeSelf = null)
        {
            _tableName = tableName;
            _columnName = columnName;
            _exculdeSelf = excludeSelf;
        }
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            try
            {
                int exists;
                var attribute = string.IsNullOrEmpty(_columnName) ? validationContext.MemberName : _columnName;
                var tableName = new SqlParameter(_tableName, System.Data.SqlDbType.VarChar);
                var ColumnName = new SqlParameter(attribute, System.Data.SqlDbType.VarChar);
                var ColumnValue = new SqlParameter((string)value, System.Data.SqlDbType.VarChar);
               var test = validationContext.ObjectInstance;
                var exculder = validationContext.ObjectInstance?.GetType().GetProperty(_exculdeSelf).GetValue(validationContext.ObjectInstance, null);
                string filterQuery = @$"AND {_exculdeSelf} != {exculder}";
                using (var scope = Startup._service.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<DapperDbContext>();
                    var query = @$"IF OBJECT_ID (N'{tableName}', N'U') IS NOT NULL 
                                     IF COL_LENGTH('{tableName}', 'Name') IS NOT NULL
                                     BEGIN
                                        SELECT COUNT(*) AS res FROM {tableName} WHERE {ColumnName} = '{ColumnValue}' {filterQuery};
                                     END;
                                     ELSE
                                        SELECT -1 AS res;  
                                     ELSE 
                                        SELECT -1 AS res; ---Table Or Column Not Exists";
                    using (var connection = context.CreateConnection())
                    {
                        exists = connection.Query<int>(query).First();
                    }
                }



                if (exists > 0)
                {
                    return new ValidationResult("the name should be unique");
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
