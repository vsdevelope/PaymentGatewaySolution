using Microsoft.EntityFrameworkCore;
using PaymentGateway.Application.Utilities;
using PaymentGateway.Domain;
using PaymentGateway.Domain.Entities;
using System;
using System.Security.Cryptography;
using System.Text;

namespace PaymentGateway.Persistence
{
    public class PaymentGatewayDbContext:DbContext
    {
        private IEncryptionService _encryptionService;
        public PaymentGatewayDbContext():base()
        {

        }
        public PaymentGatewayDbContext(DbContextOptions<PaymentGatewayDbContext> options, IEncryptionService encryptionService)
             : base(options)
        {
            _encryptionService = encryptionService;    
        }

        public DbSet<MerchantKeyMapping> MerchantKeyMappings { get; set; }
        public DbSet<Merchant> Merchants { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<PaymentTransactionStatus> TransactionStatuses { get; set; }

        public DbSet<PaymentTransactionType> TransactionTypes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                "Data Source= (localdb)\\MSSQLLocalDB; Initial Catalog=PaymentGatewayData");
            optionsBuilder.EnableSensitiveDataLogging();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(PaymentGatewayDbContext).Assembly);

            //seed data, added through migrations

            SeedPaymentTransactionStatusData(modelBuilder);
            SeedPaymentTransactionTypeData(modelBuilder);

            var merchantId = "M0001";
            var terminalId = "T0001";
            var merchantKey = _encryptionService.Encrypt(merchantId);

            SeedMerchantKeyMapping(modelBuilder, 1, merchantId, merchantKey);
            SeedMerchantData(modelBuilder, 1, terminalId);
            modelBuilder.Entity<Transaction>().HasData(new Transaction
            {
                MerchantId = merchantId,
                TerminalId = terminalId,
                TransactionId = 10000,
                Amount = 120.18M,
                Currency = "GBP",
                CardNumber = _encryptionService.Encrypt("4444333322221111"),
                CVV = _encryptionService.Encrypt("123"),
                ExpiryDate = "12/21",
                CustomerName="Mr Customer1",
                CustomerAddressLine1="Customer 1 AL1",
                PostCode="AB1 2CD",
                TransactionTypeId = (int)TransactionTypeEnum.CreditCardPayment,
                TransactionStatusId = (int)TransactionStatusEnum.Succeeded,
                DateTransactionCreated = DateTimeOffset.Now,
                DateTransactionUpdated = DateTimeOffset.Now.AddMinutes(2),
                BankReference = "HSBC0001",
                StatusReason="Succeeded"
            });
            modelBuilder.Entity<Transaction>().HasData(new Transaction
            {
                MerchantId = merchantId,
                TerminalId = terminalId,
                TransactionId = 10001,
                Amount = 99.99M,
                Currency = "GBP",
                CardNumber = _encryptionService.Encrypt("5555000098761234"),
                CVV = _encryptionService.Encrypt("321"),
                ExpiryDate = "12/22",
                CustomerName = "Mr Customer2",
                CustomerAddressLine1 = "Customer 2 AL1",
                PostCode = "AB2 1CD",
                TransactionTypeId = (int)TransactionTypeEnum.DebitCardPayment,
                TransactionStatusId = (int)TransactionStatusEnum.Failed,
                DateTransactionCreated = DateTimeOffset.Now,
                DateTransactionUpdated = DateTimeOffset.Now.AddSeconds(90),
                BankReference = "HSBC0002",
                StatusReason = "InvalidCardDetails"
            });

            terminalId = "T0002";
            SeedMerchantData(modelBuilder, 1, terminalId);
            modelBuilder.Entity<Transaction>().HasData(new Transaction
            {
                MerchantId = merchantId,
                TerminalId = terminalId,
                TransactionId = 10002,
                Amount = 99.99M,
                Currency = "GBP",
                CardNumber = _encryptionService.Encrypt("5555000098761234"),
                CVV = _encryptionService.Encrypt("321"),
                ExpiryDate = "12/22",
                CustomerName = "Mr Customer3",
                CustomerAddressLine1 = "Customer 3 AL1",
                PostCode = "AB3 2CD",
                TransactionTypeId = (int)TransactionTypeEnum.PreAuth,
                TransactionStatusId = (int)TransactionStatusEnum.InProgress,
                DateTransactionCreated = DateTimeOffset.Now,
                DateTransactionUpdated = DateTimeOffset.Now.AddSeconds(68),
                BankReference = "BLAC0001",
                StatusReason = "LinkFailure"
            });

            merchantId = "M0002";
            terminalId = "TM0001";
            merchantKey = _encryptionService.Encrypt(merchantId);

            SeedMerchantKeyMapping(modelBuilder, 2, merchantId, merchantKey);
            SeedMerchantData(modelBuilder, 2, terminalId);


            modelBuilder.Entity<Transaction>().HasData(new Transaction
            {
                MerchantId = merchantId,
                TerminalId = terminalId,
                TransactionId = 10003,
                Amount = 99.99M,
                Currency = "GBP",
                CardNumber = _encryptionService.Encrypt("367811118761234"),
                CVV = _encryptionService.Encrypt("321"),
                ExpiryDate = "10/22",
                CustomerName = "Mr Customer4",
                CustomerAddressLine1 = "Customer 4 AL1",
                PostCode = "AB4 5CD",
                TransactionTypeId = (int)TransactionTypeEnum.Cancel,
                TransactionStatusId = (int)TransactionStatusEnum.Succeeded,
                DateTransactionCreated = DateTimeOffset.Now,
                DateTransactionUpdated = DateTimeOffset.Now.AddSeconds(78),
                BankReference = "LLy0001",
                StatusReason = "Succeeded"
            });

            merchantId = "M0003";
            terminalId = "TMX001";
            merchantKey = _encryptionService.Encrypt(merchantId);

            SeedMerchantKeyMapping(modelBuilder, 3, merchantId, merchantKey);
            SeedMerchantData(modelBuilder, 3, terminalId);


            modelBuilder.Entity<Transaction>().HasData(new Transaction
            {
                MerchantId = merchantId,
                TerminalId = terminalId,
                TransactionId = 10004,
                Amount = 99.99M,
                Currency = "GBP",
                CardNumber = _encryptionService.Encrypt("1234000098765555"),
                CVV = _encryptionService.Encrypt("111"),
                ExpiryDate = "12/22",
                CustomerName = "Mr Customer5",
                CustomerAddressLine1 = "Customer 5 AL1",
                PostCode = "AB4 5CD",
                TransactionTypeId = (int)TransactionTypeEnum.Cancel,
                TransactionStatusId = (int)TransactionStatusEnum.Succeeded,
                DateTransactionCreated = DateTimeOffset.Now,
                DateTransactionUpdated = DateTimeOffset.Now.AddSeconds(109),
                BankReference = "LLy0001",
                StatusReason = "Succeeded"
            });
        }

        private void SeedTransactionData(ModelBuilder modelBuilder, Transaction transaction)
        {
            modelBuilder.Entity<Transaction>().HasData(transaction);
        }
        private void SeedPaymentTransactionStatusData(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PaymentTransactionStatus>().HasData(new PaymentTransactionStatus
            {
                PaymentTransactionStatusId = 1,
                Description= "Succeeded"
            });

            modelBuilder.Entity<PaymentTransactionStatus>().HasData(new PaymentTransactionStatus
            {
                PaymentTransactionStatusId = 2,
                Description = "Failed"
            });

            modelBuilder.Entity<PaymentTransactionStatus>().HasData(new PaymentTransactionStatus
            {
                PaymentTransactionStatusId = 3,
                Description = "InProgress"
            });

            modelBuilder.Entity<PaymentTransactionStatus>().HasData(new PaymentTransactionStatus
            {
                PaymentTransactionStatusId = 4,
                Description = "Exception"
            });

            modelBuilder.Entity<PaymentTransactionStatus>().HasData(new PaymentTransactionStatus
            {
                PaymentTransactionStatusId = 5,
                Description = "Cancelled"
            });

            modelBuilder.Entity<PaymentTransactionStatus>().HasData(new PaymentTransactionStatus
            {
                PaymentTransactionStatusId = 6,
                Description = "Error From Acquirer"
            });
        }

        private void SeedPaymentTransactionTypeData(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PaymentTransactionType>().HasData(new PaymentTransactionType
            {
                PaymentTransactionTypeId = 1,
                Description = "CreditCardPayment"
            });

            modelBuilder.Entity<PaymentTransactionType>().HasData(new PaymentTransactionType
            {
                PaymentTransactionTypeId = 2,
                Description = "DebitCardPayment"
            });

            modelBuilder.Entity<PaymentTransactionType>().HasData(new PaymentTransactionType
            {
                PaymentTransactionTypeId = 3,
                Description = "PreAuth"
            });

            modelBuilder.Entity<PaymentTransactionType>().HasData(new PaymentTransactionType
            {
                PaymentTransactionTypeId = 4,
                Description = "Auth"
            });

            modelBuilder.Entity<PaymentTransactionType>().HasData(new PaymentTransactionType
            {
                PaymentTransactionTypeId = 5,
                Description = "Refund"
            });

            modelBuilder.Entity<PaymentTransactionType>().HasData(new PaymentTransactionType
            {
                PaymentTransactionTypeId = 6,
                Description = "Cancel"
            });
        }

        private void SeedMerchantKeyMapping(ModelBuilder modelBuilder, int merchantKeyMappingId,string merchantId,string merchantKey)
        {
            modelBuilder.Entity<MerchantKeyMapping>().HasData(new MerchantKeyMapping
            {
                Id = merchantKeyMappingId,
                MerchantId = merchantId,
                MerchantKey = merchantKey
            });
        }
        private void SeedMerchantData(ModelBuilder modelBuilder,int merchantKeyMappingId,string terminalId)
        {
            modelBuilder.Entity<Merchant>().HasData(new Merchant
            {
                MerchantKeyMappingId = merchantKeyMappingId,
                TerminalId = terminalId,
            });
        }

        private static string GetHash(HashAlgorithm hashAlgorithm, string input)
        {

            // Convert the input string to a byte array and compute the hash.
            byte[] data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            var sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }
    }
}
