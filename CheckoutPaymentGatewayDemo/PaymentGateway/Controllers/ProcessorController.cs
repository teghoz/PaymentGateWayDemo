using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SharedResource.ViewModels;
using PaymentGateWayModels;
using PaymentGateway.Model;
using SharedResource.ViewModels.ViewModels;
using PaymentGateway.Validators;
using Newtonsoft.Json;
using SharedResource;
using RestSharp;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;
using PaymentGatewayDbContext;
using Microsoft.IdentityModel.Tokens;
using PaymentGateway.Models;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using StackExchange.Profiling;
using Hangfire;

namespace PaymentGateway.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class ProcessorController : ControllerBase
    {
        private UnitOfWork unitOfWork = new UnitOfWork();
        private readonly SignInManager<PaymentGatewayDbContext.ApplicationUser> _paymentGatewaySignInManager;
        private readonly UserManager<PaymentGatewayDbContext.ApplicationUser> _paymentGatewayUserManager;
        private RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationSettings _applicationSettings;      

        public ProcessorController(SignInManager<PaymentGatewayDbContext.ApplicationUser> paymentGatewaySignInManager,
            UserManager<PaymentGatewayDbContext.ApplicationUser> paymentGatewayUserManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<ApplicationSettings> options)
        {
            _paymentGatewaySignInManager = paymentGatewaySignInManager;
            _paymentGatewayUserManager = paymentGatewayUserManager;
            _roleManager = roleManager;
            _applicationSettings = options.Value;
        }

        /// <summary>
        /// Merchant Login. Returns a token for authentication
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("Login")]     
        public async Task<IActionResult> MerchantLogin([FromForm] LoginBaggage login)
        {
            string userName = login.UserName;
            string password = login.Password;
            bool rememberMe = login.RememberMe;

            try
            {
                Microsoft.AspNetCore.Identity.SignInResult result = new Microsoft.AspNetCore.Identity.SignInResult();

                if (login.UseEmail)
                {
                    var userNameFromEmail = unitOfWork.MerchantRepository.Get(m => m.Email == login.Email).FirstOrDefault().Username;
                    if (userNameFromEmail != null)
                    {
                        result = await _paymentGatewaySignInManager.PasswordSignInAsync(userNameFromEmail, password, rememberMe, lockoutOnFailure: true);
                    }
                }
                else
                {
                    result = await _paymentGatewaySignInManager.PasswordSignInAsync(userName, password, rememberMe, lockoutOnFailure: true);
                }

                if (result.Succeeded)
                {
                    //_logger.LogInformation("User logged in.");
                    var currentMerchant = _paymentGatewayUserManager.FindByNameAsync(userName).Result;
                    var currentMerchantId = currentMerchant.MerchantId;

                    if (_paymentGatewayUserManager.IsInRoleAsync(currentMerchant, "Merchant").Result)
                    {
                        var merchantId = _paymentGatewayUserManager.FindByNameAsync(userName).Result.MerchantId;
                        if (merchantId != 0)
                        {
                            if (unitOfWork.MerchantRepository.Get(m => m.Id == merchantId).FirstOrDefault().IsActive == false)
                            {
                                return BadRequest(new { message = "AccountDeactivated" });
                            }
                            else
                            {
                                var tokenString = GetToken(currentMerchant);

                                // return basic user info (without password) and token to store client side
                                return Ok(new
                                {
                                    Token = tokenString
                                });
                            }
                        }
                    }
                    else
                    {
                        await _paymentGatewaySignInManager.SignOutAsync();
                        return BadRequest(new { message = "You do not have a member role" });
                    }
                }
                else
                {
                    return BadRequest(new { message = "Username or password is incorrect" });
                }

                return BadRequest(new { message = "Something Went Wrong" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.RecursiveMessages() });
            }

        }
        private string GetToken(ApplicationUser currentMerchant)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_applicationSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Email, currentMerchant.Email),
                    new Claim(ClaimTypes.GivenName, currentMerchant.FullName ?? $@"{currentMerchant.FirstName} {currentMerchant.LastName}"),
                    new Claim(ClaimTypes.Authentication, currentMerchant.Id),
                    new Claim(ClaimTypes.Role, "Merchant"),
                    new Claim(ClaimTypes.Name, currentMerchant.MerchantId.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = "CHECKOUT",
                IssuedAt = DateTime.Now
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return tokenString;
        }

        /// <summary>
        /// Register a Merchant
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]             
        [HttpPost("Merchant/Registration")]
        public async Task<IActionResult> RegisterMerchant([FromForm] MerchantRegistration model)
        {
            var validator = new MerchantRegistrationValidator();
            var results = validator.Validate(model);
            if (results.IsValid)
            {
                var merchant = new Merchant
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    FullName = $@"{model.FirstName} {model.LastName}",
                    Email = model.Email,
                    IsActive = true
                };
                unitOfWork.MerchantRepository.Insert(merchant);
                unitOfWork.Save();

                PaymentGatewayDbContext.ApplicationUser user = new PaymentGatewayDbContext.ApplicationUser
                {
                    MerchantId = merchant.Id,
                    FirstName = merchant.FirstName,
                    LastName = merchant.LastName,
                    Email = merchant.Email,
                    UserName = merchant.Email,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true
                };

                var result = await _paymentGatewayUserManager.CreateAsync(user, "ch@ck0utA");
                if (result.Succeeded)
                {
                    await _paymentGatewayUserManager.AddToRoleAsync(user, "Merchant");
                    return Ok(new { Message = "Merchant Registration Succesful", Status = 1, Merchant = merchant });
                }
                else
                {
                    return BadRequest(JsonConvert.SerializeObject(result.Errors.Select(e => e.Description).ToList()));                    
                }
            }
            else
            {
                return BadRequest(JsonConvert.SerializeObject(results.Errors.Select(e => e.ErrorMessage).ToList()));
            }
        }
        /// <summary>
        /// Process a mechant payment. Requires Authentication
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        // POST: api/Processor
        [HttpPost("Process")]
        public IActionResult Process([FromForm] PaymentInfo model)
        {
            Program.log.Info($@"Process endpoint invoked");
            Program.log.Info($@"Baggage {JsonConvert.SerializeObject(model)}");

            var profiler = MiniProfiler.StartNew("Payment Gateway");
            using (profiler.Step("Process Workflow"))
            {
                //validate the model
                var paymentResult = new PaymentResult();
                var validator = new PaymentValidator();
                var results = validator.Validate(model);
                Program.log.Info($@"Process baggage validation {JsonConvert.SerializeObject(results.Errors.Select(e => e.ErrorMessage).ToList())}");

                if (results.IsValid)
                {
                    Program.log.Info($@"Validation Succesful");
                    var merchantInfo = unitOfWork.MerchantRepository.GetByID(model.MerchantId);
                    if (merchantInfo != null)
                    {
                        Program.log.Info($@"Validation Succesful");
                        Program.log.Info($@"Saving Card Details");
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

                        Program.log.Info($@"Saving Card Details");

                        Program.log.Info($@"Contacting Mock Bank");
                        var client = new RestClient($@"{ConfigurationManager.AppSetting["MockBankUri"]}");
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
                        Program.log.Info($@"Mock Bank Response {JsonConvert.SerializeObject(bankVerificationResponse)}");

                        if (bankVerificationResponse.Status)
                        {
                            transaction.Status = eStatusTypes.Success;
                            transaction.BankTransactionCode = bankVerificationResponse.TransactionCode;
                            unitOfWork.TransactionRepository.Update(transaction);
                            var resp = new { Message = "Transaction Completed Succesfully", TransanctionCode = transaction.Code, Status = true };
                            Program.log.Info($@"Payment Gateway Response {JsonConvert.SerializeObject(resp)}");
                            Program.log.Info($@"Profiler data {profiler.RenderPlainText()}");
                            return Ok(resp);
                        }
                        else
                        {
                            transaction.Status = eStatusTypes.Failure;
                            unitOfWork.TransactionRepository.Update(transaction);
                            Program.log.Info($@"Profiler data {profiler.RenderPlainText()}");
                            return BadRequest(new { message = bankVerificationResponse.Message, TransanctionCode = transaction.Code, Status = false });
                        }
                    }
                    else
                    {
                        Program.log.Info($@"Profiler data {profiler.RenderPlainText()}");
                        return BadRequest(new { message = "Merchant Not Found" });
                    }
                }
                else
                {
                    paymentResult.Error = results.Errors.Select(e => e.ErrorMessage).ToList();
                    paymentResult.Message = "Model Invalid";
                    paymentResult.Status = SharedResource.eStatusTypes.Failure;
                    Program.log.Info($@"Profiler data {profiler.RenderPlainText()}");
                    return BadRequest(JsonConvert.SerializeObject(paymentResult));
                }
            }
        }
        /// <summary>
        /// Processes request in the queue and should post back response to provided endpoint
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("Process/Queued")]
        public string QueuedProcess([FromForm] PaymentInfo model)
        {
            Program.log.Info($@"Queued Process endpoint invoked");
            Program.log.Info($@"Baggage {JsonConvert.SerializeObject(model)}");

            var profiler = MiniProfiler.StartNew("Payment Gateway");
            using (profiler.Step("Queued Process Workflow"))
            {
                //validate the model
                var paymentResult = new PaymentResult();
                var validator = new PaymentValidatorQueued();
                var results = validator.Validate(model);
                Program.log.Info($@"Queued Process baggage validation {JsonConvert.SerializeObject(results.Errors.Select(e => e.ErrorMessage).ToList())}");

                var job = BackgroundJob.Enqueue(() => TaskManager.GatewayProcessor(results, model, profiler, paymentResult, unitOfWork, _applicationSettings, null));
                return job;
            }
        }
        /// <summary>
        /// Get All Merchant Transactions
        /// </summary>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        [HttpPost("Merchant/Transactions")]
        public IActionResult MerchantTransctions(int start, int length)
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    var merchantId = int.Parse(User.Claims.First(u => u.Type == ClaimTypes.Name).Value);
                    var listCount = unitOfWork.TransactionRepository.Get(t => t.MerchantId == merchantId).Count();
                    int value = (listCount - start < 0 ? 1 : (listCount - start));

                    return Ok(new
                    {
                        recordsFiltered = length,
                        recordsTotal = listCount,
                        Data = unitOfWork.TransactionRepository.Get(m => m.MerchantId == merchantId, null, "CardDetails").Skip(start).Take(Math.Min(length, value)).ToList()
                    });
                }
                else
                {
                    return BadRequest(new { message = $@"Wrong/Expired Token" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.RecursiveMessages() });
            }
        }
        /// <summary>
        /// Get a particular merchant transaction
        /// </summary>
        /// <param name="reference"></param>
        /// <returns></returns>
        [HttpGet("Merchant/Transaction")]
        public IActionResult MerchantTransction(string reference)
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    var merchantId = int.Parse(User.Claims.First(u => u.Type == ClaimTypes.Name).Value);

                    return Ok(new
                    {
                        Data = unitOfWork.TransactionRepository.Get(m => m.MerchantId == merchantId && m.Code == reference, null, "CardDetails").FirstOrDefault()
                    });
                }
                else
                {
                    return BadRequest(new { message = $@"Wrong/Expired Token" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.RecursiveMessages() });
            }
        }
        [AllowAnonymous]
        [HttpPost("Merchant/TestReciever")]
        public void TestReciever(object payload)
        {
            Program.log.Info($@"Queued Reciever invoked endpoint invoked {JsonConvert.SerializeObject(payload)}");
        }
    }
}
