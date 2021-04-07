using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using PaymentGateway.Application.Commands.CreateTransaction;
using PaymentGateway.Application.Queries.GetTransactionDetail;
using System.Threading.Tasks;

namespace PaymentGateway.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<TransactionController> _logger;

        public TransactionController(IMediator mediator, ILogger<TransactionController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Gets a specific transaction.
        /// </summary>
        /// <param name="id"></param>
        [HttpGet("{id}", Name = "GetTransactionById")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTransactionById(long id,[FromHeader] string merchantKey)
        {
            _logger.LogInformation($"Getting details for transaction: {id}");
            var getTransactionDetailQuery = new GetTransactionDetailQuery() { TransactionId = id,MerchantKey=merchantKey };

            var result = await _mediator.Send(getTransactionDetailQuery);

            if(result==null)
            {
                return NotFound($"Transaction ({id}) is not found");
            }

            return Ok(result);

        }

        /// <summary>
        /// Post a transaction.
        /// </summary>
        [HttpPost(Name = "PostTransaction")]
        [ProducesResponseType(typeof(CreateTransactionCommandResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(CreateTransactionCommandResponse), StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> PostTransaction([FromBody]CreateTransactionCommand request, [FromHeader] string merchantKey)
        {
            _logger.LogInformation($"Received post transaction request");
            request.MerchantKey = merchantKey;
            var result = await _mediator.Send(request);
            if(result.Success)
            {
               return Created($"/api/Transaction/{result.Transaction.TransactionId}", result);
            }
            else
            {
                return BadRequest(result);
            }
        }
    }
}
