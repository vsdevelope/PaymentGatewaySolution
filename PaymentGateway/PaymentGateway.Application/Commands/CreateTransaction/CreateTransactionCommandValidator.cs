using FluentValidation;
using PaymentGateway.Application.Contracts.Persistence;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PaymentGateway.Application.Commands.CreateTransaction
{
    public class CreateTransactionCommandValidator: AbstractValidator<CreateTransactionCommand>
    {
        private ITransactionTypeRepository _transactionTypeRespository;
        public CreateTransactionCommandValidator(ITransactionTypeRepository transactionTypeRespository)
        {
            _transactionTypeRespository = transactionTypeRespository;
            RuleFor(p => p.MerchantId)
                .NotEmpty().WithMessage("{PropertyName} is required.");

            RuleFor(p => p.TerminalId)
                .NotEmpty().WithMessage("{PropertyName} is required.");

            RuleFor(p => p.Amount)
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .GreaterThan(0).WithMessage("{PropertyName} should be greater than 0");

            RuleFor(p => p.Currency)
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .Must(CurrencyCheck).WithMessage("only GBP currency is supported.");

            RuleFor(p => p.CardNumber)
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .Matches(@"^\d{16}$").WithMessage("{PropertyName} should contain only 16 digts");
             
            RuleFor(p => p.CVV)
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .Matches(@"\d{3,4}$").WithMessage("{PropertyName} length should contain only 3 or 4 digits");

            RuleFor(p => p.ExpiryDate)
               .NotEmpty().WithMessage("{PropertyName} is required.")
               .Must(ValidExpiryDate).WithMessage("Card already expired or invalid expiry date. Expiry date should be mm/yy format"); ;

            RuleFor(p => p.TransactionTypeId)
              .NotEmpty().WithMessage("{PropertyName} is required.")
              .GreaterThanOrEqualTo(1).WithMessage("{PropertyName} should be greater than or equal to 1")
              .MustAsync(ValidTransactionType).WithMessage("'{PropertyValue}' not supported for {PropertyName}");

            RuleFor(p => p.TransactionDate)
               .NotEmpty().WithMessage("{PropertyName} is required.")
               .LessThanOrEqualTo(DateTimeOffset.Now).WithMessage("{PropertyName} cannot be future dated");

            RuleFor(p => p.CustomerName)
                .NotEmpty().WithMessage("{PropertyName} is required.");

            RuleFor(p => p.CustomerAddressLine1)
                .NotEmpty().WithMessage("{PropertyName} is required.");

            RuleFor(p => p.PostCode)
                .NotEmpty().WithMessage("{PropertyName} is required.");
        }

        private bool CurrencyCheck(string currency)
        {
            return currency?.ToLower()?.Equals("gbp")??false;
        }

        private async Task<bool> ValidTransactionType(int transactionTypeId, CancellationToken arg2)
        {
            var result = await _transactionTypeRespository.GetByIdAsync(transactionTypeId);

            return await Task.FromResult(result != null);
        }

        private bool ValidExpiryDate(string expiryDate)
        {
            // Based on googled information, cards expire on the last day of the month listed as the expiry date.
            // The assumption is made here that that happens at the beginning of the last day.
            var expiryDateArr = expiryDate.Split("/");
            if(expiryDateArr.Length!=2)
            {
                return false;
            }
            var isSuccessful = int.TryParse(expiryDateArr[1],out int cardExpiryYear);
            if(!isSuccessful)
            {
                return false;
            }
            if (cardExpiryYear >= 20)
            {
                cardExpiryYear = cardExpiryYear + 2000;
            }
            else
            {
                cardExpiryYear = cardExpiryYear + 2100;
            }

           isSuccessful = int.TryParse(expiryDateArr[0], out int cardExpiryMonth);
            if(!isSuccessful || cardExpiryMonth >12)
            {
                return false;
            }
            var cardExpiryTime = new DateTime(cardExpiryYear, cardExpiryMonth, 1).AddMonths(1).AddDays(-1);

            return cardExpiryTime > DateTime.Now;
        }
    }
}
