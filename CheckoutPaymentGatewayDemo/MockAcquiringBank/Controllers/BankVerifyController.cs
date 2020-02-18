﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedResource;
using SharedResource.ViewModels;

namespace MockAcquiringBank.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BankVerifyController : ControllerBase
    {
        // GET: api/BankVerify
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpPost("Verify")]
        public async Task<IActionResult> VerifyCardOayment([FromBody] BankVerifcationBaggage model)
        {
            //check if the card exist
            BankRepository bankRepository = new BankRepository();
            if(bankRepository.CardBank().Any(c => c.CardNumber == model.CardNumber))
            {
                var expiryCheck = model.CardExpiry.GetCardExpiry();
                if (expiryCheck.month != 0 && expiryCheck.year != 0)
                {
                    //check if the card is not expired
                    DateTime today = DateTime.Now;
                    if (today.Year <= expiryCheck.year && today.Month <= expiryCheck.month)
                    {
                        //check if the card has sufficient balance
                        if(model.Amount <= bankRepository.CardBank().Where(c => c.CardNumber == model.CardNumber).FirstOrDefault().Balance)
                        {
                            return Ok(new BankVerificationResponse
                            {
                                Status = true,
                                TransactionCode = Utilities.GenerateToken(15),
                                Message = "Success"
                            }); ;
                        }
                    }
                    else
                    {
                        return BadRequest(new BankVerificationResponse { Status = false,  Message = "Expired Card" });
                    }
                }
            }
            else
            {
                return BadRequest(new BankVerificationResponse { Status = false, Message = "Invalid Card" });
            }
            

            return BadRequest(new BankVerificationResponse { Status = false, Message = "Something We" });
        }
    }
}
