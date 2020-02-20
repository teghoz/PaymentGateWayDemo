using FluentValidation.Results;
using Hangfire.Console;
using Hangfire.Server;
using Newtonsoft.Json;
using PaymentGateway.Models;
using PaymentGateWayModels;
using RestSharp;
using SharedResource;
using SharedResource.ViewModels;
using SharedResource.ViewModels.ViewModels;
using StackExchange.Profiling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentGateway.Model
{
    public static class TaskManager
    {
        public static void GatewayProcessor(ValidationResult results,
            PaymentInfo model, MiniProfiler profiler, PaymentResult paymentResult,
            UnitOfWork unitOfWork, ApplicationSettings _applicationSettings, PerformContext context)
        {
            if (results.IsValid)
            {
                context.WriteLine($@"Validation Succesful");
                var merchantInfo = unitOfWork.MerchantRepository.GetByID(model.MerchantId);
                if (merchantInfo != null)
                {
                    context.WriteLine($@"Validation Succesful");
                    context.WriteLine($@"Saving Card Details");
                    //post the model to acquiring bank
                    var encryptedCardNumber = Utilities.EncryptString(model.CreditCardNumber, _applicationSettings.Secret);
                    var card = unitOfWork.CarDetailsRepository.Get(c => c.CreditCardNumber == encryptedCardNumber).FirstOrDefault();
                    if (card == null)
                    {
                        card = new CardDetails
                        {
                            CardType = model.CardType,
                            CreditCardNumber = model.CreditCardNumber.EncryptString(_applicationSettings.Secret),
                            cvv = model.cvv.EncryptString(_applicationSettings.Secret),
                            Expiry = model.Expiry,
                            MerchantId = model.MerchantId,
                            Token = Utilities.GenerateToken(15)
                        };
                        unitOfWork.CarDetailsRepository.Insert(card);
                        unitOfWork.Save();
                    }

                    var transaction = new Transactions
                    {
                        MerchantId = merchantInfo.Id,
                        Amount = model.Amount,
                        Currency = model.Currency,
                        Code = $@"PG-{Utilities.GenerateToken(15)}",
                        Status = eStatusTypes.Pending,
                        CardId = card.Id
                    };
                    unitOfWork.TransactionRepository.Insert(transaction);
                    unitOfWork.Save();

                    context.WriteLine($@"Saving Card Details");

                    context.WriteLine($@"Contacting Mock Bank");
                    var client = new RestClient(ConfigurationManager.AppSetting["MockBankUri"]);
                    var bagg = new BankVerifcationBaggage
                    {
                        Amount = model.Amount,
                        CardCVV = model.cvv,
                        CardExpiry = model.Expiry,
                        CardNumber = model.CreditCardNumber
                    };

                    var request = new RestRequest("api/BankVerify/Verify", DataFormat.Json);
                    request.RequestFormat = DataFormat.Json;
                    request.AddJsonBody(bagg);
                    var response = client.Post(request);
                    var bankVerificationResponse = JsonConvert.DeserializeObject<BankVerificationResponse>(response.Content);
                    context.WriteLine($@"Mock Bank Response {JsonConvert.SerializeObject(bankVerificationResponse)}");

                    if (bankVerificationResponse.Status)
                    {
                        transaction.Status = eStatusTypes.Success;
                        transaction.BankTransactionCode = bankVerificationResponse.TransactionCode;
                        unitOfWork.TransactionRepository.Update(transaction);
                        var resp = new { Message = "Transaction Completed Succesfully", TransanctionCode = transaction.Code, Status = true };
                        context.WriteLine($@"Payment Gateway Response {JsonConvert.SerializeObject(resp)}");
                        context.WriteLine($@"Profiler data {profiler.RenderPlainText()}");
                        CallbackSuppliedEndpoint(model, resp, context);
                    }
                    else
                    {
                        transaction.Status = eStatusTypes.Failure;
                        unitOfWork.TransactionRepository.Update(transaction);
                        context.WriteLine($@"Profiler data {profiler.RenderPlainText()}");
                        CallbackSuppliedEndpoint(model, new { message = bankVerificationResponse.Message, TransanctionCode = transaction.Code, Status = false }, context);
                    }
                }
                else
                {
                    context.WriteLine($@"Profiler data {profiler.RenderPlainText()}");
                    CallbackSuppliedEndpoint(model, new { message = "Merchant Not Found" }, context);
                }
            }
            else
            {
                paymentResult.Error = results.Errors.Select(e => e.ErrorMessage).ToList();
                paymentResult.Message = "Model Invalid";
                paymentResult.Status = SharedResource.eStatusTypes.Failure;
                context.WriteLine($@"Profiler data {profiler.RenderPlainText()}");
                CallbackSuppliedEndpoint(model, JsonConvert.SerializeObject(paymentResult), context);
            }
        }

        public static void CallbackSuppliedEndpoint(PaymentInfo model, object payload, PerformContext context)
        {
            var client = new RestClient(model.RedirectUrl);
            var request = new RestRequest("", DataFormat.Json);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(payload);
            var response = client.Post(request);
            context.WriteLine($@"webhook reference: {JsonConvert.SerializeObject(response.Content)}");
        }
    }
}
