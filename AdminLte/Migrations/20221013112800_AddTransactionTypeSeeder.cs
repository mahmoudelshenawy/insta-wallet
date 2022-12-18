using AdminLte.Data.Enums;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdminLte.Migrations
{
    public partial class AddTransactionTypeSeeder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table : "TransactionTypes",
                columns : new[] {"Id" , "Name"},
                values : new object[] {1 , Enum.GetName(typeof(TransactionTypeEnum), TransactionTypeEnum.Withdrawal)}
                );
            migrationBuilder.InsertData(
                table : "TransactionTypes",
                columns : new[] {"Id" , "Name"},
                values : new object[] {2 , Enum.GetName(typeof(TransactionTypeEnum), TransactionTypeEnum.Deposit)}
                );
            migrationBuilder.InsertData(
                table : "TransactionTypes",
                columns : new[] {"Id" , "Name"},
                values : new object[] {3 , Enum.GetName(typeof(TransactionTypeEnum), TransactionTypeEnum.Transferred)}
                );
            migrationBuilder.InsertData(
                table : "TransactionTypes",
                columns : new[] {"Id" , "Name"},
                values : new object[] {4 , Enum.GetName(typeof(TransactionTypeEnum), TransactionTypeEnum.Received)}
                );
            migrationBuilder.InsertData(
                table : "TransactionTypes",
                columns : new[] {"Id" , "Name"},
                values : new object[] {5 , Enum.GetName(typeof(TransactionTypeEnum), TransactionTypeEnum.Investment_Deposit)}
                );
            migrationBuilder.InsertData(
                table : "TransactionTypes",
                columns : new[] {"Id" , "Name"},
                values : new object[] {6 , Enum.GetName(typeof(TransactionTypeEnum), TransactionTypeEnum.Investment_Revenue)}
                );
             migrationBuilder.InsertData(
                table : "TransactionTypes",
                columns : new[] {"Id" , "Name"},
                values : new object[] {7 , Enum.GetName(typeof(TransactionTypeEnum), TransactionTypeEnum.Deal_Deposit)}
                );
             migrationBuilder.InsertData(
                table : "TransactionTypes",
                columns : new[] {"Id" , "Name"},
                values : new object[] {8 , Enum.GetName(typeof(TransactionTypeEnum), TransactionTypeEnum.Deal_Revenue) }
                );
             migrationBuilder.InsertData(
                table : "TransactionTypes",
                columns : new[] {"Id" , "Name"},
                values : new object[] {9, Enum.GetName(typeof(TransactionTypeEnum), TransactionTypeEnum.Deal_Capital) }
                );

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM dbo.TransactionTypes");
        }
    }
}
