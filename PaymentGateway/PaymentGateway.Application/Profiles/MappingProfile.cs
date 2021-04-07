using AutoMapper;
using PaymentGateway.Application.Commands.CreateTransaction;
using PaymentGateway.Application.Models.Acquirer;
using PaymentGateway.Application.Queries.GetTransactionDetail;
using PaymentGateway.Application.Utilities;
using PaymentGateway.Domain.Entities;
using System;

namespace PaymentGateway.Application.Profiles
{
    public class MappingProfile:Profile
    {
          public MappingProfile(IEncryptionService encryptionService)
        {
            CreateMap<CreateTransactionCommand, Transaction>()
                .ForMember(dest => dest.CardNumber,
                opt => opt.MapFrom(src => encryptionService.Encrypt(src.CardNumber)))
                 .ForMember(dest => dest.CVV,
                opt => opt.MapFrom(src => encryptionService.Encrypt(src.CVV)))
                 .ForMember(dest=>dest.DateTransactionCreated,
                 opt=>opt.MapFrom(src=>src.TransactionDate));
            CreateMap<Transaction, CreateTransactionDto>();
            CreateMap<Transaction, TransactionDetailVm>()
                .ForMember(dest => dest.CardNumber,
                opt => opt.MapFrom(src => encryptionService.Decrypt(src.CardNumber).MaskCreditCardNumber()))
                 .ForMember(dest => dest.CVV,
                opt => opt.MapFrom(src => encryptionService.Decrypt(src.CVV).MaskCVV()))
                 .ForMember(dest => dest.TransactionType,
                 opt => opt.MapFrom(src => Enum.GetName(typeof(TransactionTypeEnum),src.TransactionTypeId)))
                 .ForMember(dest => dest.TransactionStatus,
                 opt => opt.MapFrom(src => Enum.GetName(typeof(TransactionStatusEnum),src.TransactionStatusId)));
            CreateMap<CreateTransactionCommand, AcquirerTransactionRequest>();
        }
    }
}
