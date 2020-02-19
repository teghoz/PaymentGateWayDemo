using MockAcquiringBank.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MockAcquiringBank
{
    public class BankRepository
    {
        public List<BankAccountCards> CardBank()
        {
            return new List<BankAccountCards>()
            {
                new BankAccountCards{ CardNumber = "4111 1111 1111 1111", Email = "", Expiry= "10/24", Balance = 10.34M  },
                new BankAccountCards{ CardNumber = "4111 1111 1111 1112", Email = "", Expiry= "3/20", Balance = 125.34M },
                new BankAccountCards{ CardNumber = "4111 1111 1111 1113", Email = "", Expiry= "8/25", Balance = 150.34M },
                new BankAccountCards{ CardNumber = "4111 1111 1111 1114", Email = "", Expiry= "5/23", Balance = 1299.34M },
                new BankAccountCards{ CardNumber = "4111 1111 1111 1115", Email = "", Expiry= "3/19", Balance = 150000.34M },
                new BankAccountCards{ CardNumber = "4111 1111 1111 1116", Email = "", Expiry= "4/21", Balance = 0.34M },
                new BankAccountCards{ CardNumber = "4111 1111 1111 1117", Email = "", Expiry= "7/22", Balance = 1356.34M },
                new BankAccountCards{ CardNumber = "4111 1111 1111 1118", Email = "", Expiry= "11/26", Balance = 15.34M },
                new BankAccountCards{ CardNumber = "4111 1111 1111 1119", Email = "", Expiry= "12/24", Balance = 239.00M },
                new BankAccountCards{ CardNumber = "4111 1111 1111 1120", Email = "", Expiry= "10/21", Balance = 3000.00M },
                new BankAccountCards{ CardNumber = "4111 1111 1111 1121", Email = "", Expiry= "9/22", Balance = 100.00M},
                new BankAccountCards{ CardNumber = "4111 1111 1111 1122", Email = "", Expiry= "11/24", Balance = 5000.00M },
                new BankAccountCards{ CardNumber = "4111 1111 1111 1123", Email = "", Expiry= "7/25", Balance = 200 },
                new BankAccountCards{ CardNumber = "4111 1111 1111 1124", Email = "", Expiry= "11/19", Balance = 1000.35M },
            };
        }
    }
}
