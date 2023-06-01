using AdminLte.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminLte.Test.Faker
{
    internal class TransactionTypeFaker
    {
        public List<TransactionTypes> GenerateTransactionTypes()
        {
            var transactionTypes = new List<TransactionTypes>();
            transactionTypes.Add(new TransactionTypes { Id = 1, Feeable = true, Name = Data.Enums.TransactionTypeEnum.Deposit });
            transactionTypes.Add(new TransactionTypes { Id = 2, Feeable = true, Name = Data.Enums.TransactionTypeEnum.Withdrawal });
            transactionTypes.Add(new TransactionTypes { Id = 3, Feeable = true, Name = Data.Enums.TransactionTypeEnum.Investment_Deposit });
            transactionTypes.Add(new TransactionTypes { Id = 4, Feeable = true, Name = Data.Enums.TransactionTypeEnum.Investment_Revenue });
            transactionTypes.Add(new TransactionTypes { Id = 5, Feeable = true, Name = Data.Enums.TransactionTypeEnum.Payment_Received });
            transactionTypes.Add(new TransactionTypes { Id = 6, Feeable = true, Name = Data.Enums.TransactionTypeEnum.Exchange_From });
            transactionTypes.Add(new TransactionTypes { Id = 7, Feeable = true, Name = Data.Enums.TransactionTypeEnum.Exchange_To });
            return transactionTypes;
        }
    }
}
