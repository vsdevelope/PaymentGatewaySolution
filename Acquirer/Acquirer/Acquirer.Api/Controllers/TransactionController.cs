using Acquirer.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Acquirer.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private static readonly Dictionary<string,TransactionResponse> mockedResponses= new Dictionary<string, TransactionResponse>
        {
            {"1111",new TransactionResponse {Status = "Succeeded",
                        BankReference = "1111",
                        StatusReason = "Succeeded" } 
            },
            {"2222",new TransactionResponse
                    {
                        Status = "Failed",
                        BankReference = "2222",
                        StatusReason = "InvalidCardDetails"
                    }
            },
            {"3333",new TransactionResponse
                    {
                        Status = "Failed",
                        BankReference = "3333",
                        StatusReason = "LinkFailure"
                    }
            },

            {"4444",new TransactionResponse
                    {
                        Status = "Failed",
                        BankReference = "4444",
                        StatusReason = "SomethingWentWrong.TryAgain"
                    }
            },
            {"5555",new TransactionResponse
                    {
                        Status = "Failed",
                        BankReference = "5555",
                        StatusReason = "CancelledTryAgain"
                    }
            },
            {"6666",null},
        };

        private readonly ILogger<TransactionController> _logger;

        public TransactionController(ILogger<TransactionController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<TransactionResponse>> Post([FromBody] TransactionRequest request,[FromHeader] string apiKey)
        {
            if(string.IsNullOrWhiteSpace(apiKey) 
                || !apiKey.Equals("OPtmhC7QiueYeIqy1OkEPQsvuZQdMWlyDE2fjs"))
            {
                return Unauthorized("invalid api key to access Acquirer Gateway");
            }

            var isFound = mockedResponses.TryGetValue(request.CardNumber.Substring(request.CardNumber.Length - 4, 4),out TransactionResponse response);

            if(!isFound)
            {
                response = mockedResponses["1111"];
            }

            return Ok(response);
        }
    }
}
