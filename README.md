# PaymentGateWayDemo
A Demo to showcase Payment Gate Way

## Assumptions
* Password used across ``` ch@ck0utA ```
* My Mock Bank has a defined set of valid cards. Any other cards used would return an invalid card error

```
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
new BankAccountCards{ CardNumber = "4111 1111 1111 1124", Email = "", Expiry= "11/19", Balance = 1000.35M }
```

## Usage

* A merchant has to be created/registered using the endpoint ``` /PaymentGateway/Merchant/Registration ```
* The registered merchant would have to be authenticated using the endpoint ``` /PaymentGateway/Merchant/MerchantLogin ```. A token would be returned which the merchant would use to access resources.
* The registered merchant can use either the "synchronous" endpoint of ``` /PaymentGateway/Merchant/Process ``` to process a transaction or ``` /PaymentGateway/Merchant/Process/Queued ``` which kind of simulates a webhook. It returns the response to the provided return url.

* All Endpoint are listing in each projects swagger page
![picture alt](ReadMeAssets/PaymentGateway.png "Mock Bank Swagger")
![picture alt](ReadMeAssets/MockBank.png "Mock Bank Swagger")

## Commands
* build
```bash 
dotnet build 
```
* test
```bash
dotnet test 
```
* add migration 
```bash
dotnet ef migrations add [YourDescription] -c PaymentGatewayDbContext.PaymentGatewayDbContext -p PaymentGatewayDbContext 
```
* update database 
```bash
dotnet ef database update --project PaymentGatewayDbContext 
```

## Requirement
**Node JS.**

### Check NodeJS Existence
```bash
node -v
```
Run the above command on terminal. If no version number is outputted, you probably dont have node installed.


### Run Demo
```bash
npm run demo
```
### Frontend Test
See [Qunit Test](js/afrinic-test.js).

### NodeJS Test
See [Mocha Test](test/server-test.js).

### Sample Code
See [List View](js/afrinic-list.js).

# Technologies Used
* WebApi.
* EntityFramework.
* Microsoft EF Identity.
* Sql Server Local Db.
* InMemoryDatabase.
* Docker.
* Hangfire.
* FluentValidation.
* Log4Net.
* RestSharp.
* NUnit.
* Swagger API.
* StackExchange.Profiling
* JwtBearer Token Authentication
* AutoFixture