using System;

namespace PaymentGateway.Application.Contracts.Exceptions
{
    public class UnAuthorizedException:ApplicationException
    {
        public UnAuthorizedException(string message):base(message)
        {

        }
    }
}
