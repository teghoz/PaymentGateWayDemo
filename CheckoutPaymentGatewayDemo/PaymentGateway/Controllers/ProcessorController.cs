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

namespace PaymentGateway.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProcessorController : ControllerBase
    {
        private UnitOfWork unitOfWork = new UnitOfWork();
        // GET: api/Processor
        [HttpGet]
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

        // POST: api/Processor
        [HttpPost]
        public async Task<IActionResult> Process([FromBody] PaymentInfo model)
        {
            //validate the model
            var paymentResult = new PaymentResult();
            var validator = new PaymentValidator();
            var results = validator.Validate(model);

            if (results.IsValid)
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
