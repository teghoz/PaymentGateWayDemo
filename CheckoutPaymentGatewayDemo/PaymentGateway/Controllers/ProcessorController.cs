using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedResource.ViewModels;
using PaymentGatewayRepository;
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
        private PaymentGatewayDbContext.PaymentGatewayDbContext _paymentGatewayDbContext;
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
        // GET: api/Processor
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Processor/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

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
                                var tokenString = await GetToken(currentMerchant);

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

        private async Task<string> GetToken(ApplicationUser currentMerchant)
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

        // POST: api/Processor
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
                    return Ok(new { Message = "Merchant Registration Succesful", Status = 1 });
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

            return BadRequest(new { message = "Something Went Wrong" });
        }
        // POST: api/Processor
        [HttpPost("Process")]
        public async Task<IActionResult> Process([FromForm] PaymentInfo model)
        {
            //validate the model
            var paymentResult = new PaymentResult();
            var validator = new PaymentValidator();
            var results = validator.Validate(model);

            if (results.IsValid)
            {
                var merchantInfo = unitOfWork.MerchantRepository.GetByID(model.MerchantId);
                if(merchantInfo != null)
                {
                    //post the model to acquiring bank
                    var card = new CardDetails
                    {
                        CardType = model.CardType,
                        CreditCardNumber = model.CreditCardNumber.EncryptString(ConfigurationManager.AppSetting["EncryptionKey"]),
                        cvv = model.cvv.EncryptString(ConfigurationManager.AppSetting["EncryptionKey"]),
                        Expiry = model.Expiry,
                        MerchantId = model.MerchantId,
                        Token = Utilities.GenerateToken(15)
                    };
                    unitOfWork.CarDetailsRepository.Insert(card);

                    var transaction = new Transactions
                    {
                        Amount = model.Amount
                    };
                    unitOfWork.TransactionRepository.Insert(transaction);
                    unitOfWork.Save();

                    var client = new RestClient(ConfigurationManager.AppSetting["MockBankUri"]);
                    var request = new RestRequest("statuses/home_timeline.json", DataFormat.Json);
                    var response = client.Post(request);
                    //return result
                }
                else
                {
                    return BadRequest(new { message = "Merchant Not Found" });
                }
            }
            else
            {

                paymentResult.Error = results.Errors.Select(e => e.ErrorMessage).ToList();
                paymentResult.Message = "Model Invalid";
                paymentResult.Status = SharedResource.eStatusTypes.Failure;
                return BadRequest(JsonConvert.SerializeObject(paymentResult));
            }

            return BadRequest(new { message = "Something Went Wrong" });
        }

        // POST: api/Processor
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Processor/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
