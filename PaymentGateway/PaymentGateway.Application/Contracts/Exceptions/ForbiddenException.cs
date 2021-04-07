using System;

namespace PaymentGateway.Application.Contracts.Exceptions
{
    public class ForbiddenException : ApplicationException
    {
        public ForbiddenException(string message) : base(message)
        {

        }
    }
}
