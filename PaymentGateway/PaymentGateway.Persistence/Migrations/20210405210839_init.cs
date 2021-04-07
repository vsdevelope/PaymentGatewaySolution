using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PaymentGateway.Persistence.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MerchantKeyMappings",
                columns: table => new
                {
                    MerchantId = table.Column<string>(maxLength: 10, nullable: false),
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MerchantKey = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MerchantKeyMapping_MerchantId", x => x.MerchantId);
                    table.UniqueConstraint("PK_MerchantKeyMapping_Key", x => x.MerchantKey);
                });

            migrationBuilder.CreateTable(
                name: "Merchants",
                columns: table => new
                {
                    TerminalId = table.Column<string>(maxLength: 10, nullable: false),
                    MerchantKeyMappingId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Merchant_Terminal", x => x.TerminalId);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    TransactionId = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "10000, 1"),
                    MerchantId = table.Column<string>(maxLength: 10, nullable: false),
                    TerminalId = table.Column<string>(maxLength: 10, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Currency = table.Column<string>(nullable: true),
                    CardNumber = table.Column<string>(nullable: false),
                    CVV = table.Column<string>(nullable: false),
                    ExpiryDate = table.Column<string>(nullable: false),
                    CustomerName = table.Column<string>(nullable: true),
                    CustomerAddressLine1 = table.Column<string>(nullable: true),
                    PostCode = table.Column<string>(nullable: true),
                    TransactionTypeId = table.Column<int>(nullable: false),
                    DateTransactionCreated = table.Column<DateTimeOffset>(nullable: false),
                    DateTransactionUpdated = table.Column<DateTimeOffset>(nullable: false),
                    TransactionStatusId = table.Column<int>(nullable: false),
                    StatusReason = table.Column<string>(nullable: true),
                    BankReference = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.TransactionId);
                });

            migrationBuilder.CreateTable(
                name: "TransactionStatuses",
                columns: table => new
                {
                    PaymentTransactionStatusId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(maxLength: 25, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payment_Transaction_Status", x => x.PaymentTransactionStatusId);
                });

            migrationBuilder.CreateTable(
                name: "TransactionTypes",
                columns: table => new
                {
                    PaymentTransactionTypeId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(maxLength: 25, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payment_Transaction_Type", x => x.PaymentTransactionTypeId);
                });

            migrationBuilder.InsertData(
                table: "MerchantKeyMappings",
                columns: new[] { "MerchantId", "Id", "MerchantKey" },
                values: new object[,]
                {
                    { "M0001", 1, "hCQcGPPOf0DKCLSTWDlveQ==" },
                    { "M0002", 2, "7UWuP2WPizfr4O7GmepI3A==" },
                    { "M0003", 3, "eDhl3PO6DZ/lX1Ype9tbFQ==" }
                });

            migrationBuilder.InsertData(
                table: "Merchants",
                columns: new[] { "TerminalId", "MerchantKeyMappingId" },
                values: new object[,]
                {
                    { "T0002", 1 },
                    { "T0001", 1 },
                    { "TM0001", 2 },
                    { "TMX001", 3 }
                });

            migrationBuilder.InsertData(
                table: "TransactionStatuses",
                columns: new[] { "PaymentTransactionStatusId", "Description" },
                values: new object[,]
                {
                    { 5, "Cancelled" },
                    { 6, "Error From Acquirer" },
                    { 1, "Succeeded" },
                    { 2, "Failed" },
                    { 3, "InProgress" },
                    { 4, "Exception" }
                });

            migrationBuilder.InsertData(
                table: "TransactionTypes",
                columns: new[] { "PaymentTransactionTypeId", "Description" },
                values: new object[,]
                {
                    { 6, "Cancel" },
                    { 3, "PreAuth" },
                    { 4, "Auth" },
                    { 2, "DebitCardPayment" },
                    { 1, "CreditCardPayment" },
                    { 5, "Refund" }
                });

            migrationBuilder.InsertData(
                table: "Transactions",
                columns: new[] { "TransactionId", "Amount", "BankReference", "CVV", "CardNumber", "Currency", "CustomerAddressLine1", "CustomerName", "DateTransactionCreated", "DateTransactionUpdated", "ExpiryDate", "MerchantId", "PostCode", "StatusReason", "TerminalId", "TransactionStatusId", "TransactionTypeId" },
                values: new object[,]
                {
                    { 10001L, 99.99m, "HSBC0002", "uBeKFM03ScPlfo3f/iEcUA==", "QLKHKBxBaca4O9Aac0FO3wZ4SgmkmvyWo0AUN7rqM3d2RGQPqOyjAOt4g1G8eY2x", "GBP", "Customer 2 AL1", "Mr Customer2", new DateTimeOffset(new DateTime(2021, 4, 5, 22, 8, 38, 875, DateTimeKind.Unspecified).AddTicks(1368), new TimeSpan(0, 1, 0, 0, 0)), new DateTimeOffset(new DateTime(2021, 4, 5, 22, 10, 8, 875, DateTimeKind.Unspecified).AddTicks(1403), new TimeSpan(0, 1, 0, 0, 0)), "12/22", "M0001", "AB2 1CD", "InvalidCardDetails", "T0001", 2, 2 },
                    { 10002L, 99.99m, "BLAC0001", "uBeKFM03ScPlfo3f/iEcUA==", "QLKHKBxBaca4O9Aac0FO3wZ4SgmkmvyWo0AUN7rqM3d2RGQPqOyjAOt4g1G8eY2x", "GBP", "Customer 3 AL1", "Mr Customer3", new DateTimeOffset(new DateTime(2021, 4, 5, 22, 8, 38, 877, DateTimeKind.Unspecified).AddTicks(2513), new TimeSpan(0, 1, 0, 0, 0)), new DateTimeOffset(new DateTime(2021, 4, 5, 22, 9, 46, 877, DateTimeKind.Unspecified).AddTicks(2538), new TimeSpan(0, 1, 0, 0, 0)), "12/22", "M0001", "AB3 2CD", "LinkFailure", "T0002", 3, 3 },
                    { 10003L, 99.99m, "LLy0001", "uBeKFM03ScPlfo3f/iEcUA==", "AXqFl9U3iEHMjBq6KjWs08YU0rQE+/N4oaZf0Kg6gvo=", "GBP", "Customer 4 AL1", "Mr Customer4", new DateTimeOffset(new DateTime(2021, 4, 5, 22, 8, 38, 880, DateTimeKind.Unspecified).AddTicks(3586), new TimeSpan(0, 1, 0, 0, 0)), new DateTimeOffset(new DateTime(2021, 4, 5, 22, 9, 56, 880, DateTimeKind.Unspecified).AddTicks(3604), new TimeSpan(0, 1, 0, 0, 0)), "10/22", "M0002", "AB4 5CD", "Succeeded", "TM0001", 1, 6 },
                    { 10004L, 99.99m, "LLy0001", "V0P1bOxKJEudFjsUjN35Cw==", "bI+sGVxXLxwKwcMbuFLIpwDp44IS/ISKgxU2aZ/6gRMkuccVG2/W7ZbgqcHkCDaS", "GBP", "Customer 5 AL1", "Mr Customer5", new DateTimeOffset(new DateTime(2021, 4, 5, 22, 8, 38, 883, DateTimeKind.Unspecified).AddTicks(6522), new TimeSpan(0, 1, 0, 0, 0)), new DateTimeOffset(new DateTime(2021, 4, 5, 22, 10, 27, 883, DateTimeKind.Unspecified).AddTicks(6543), new TimeSpan(0, 1, 0, 0, 0)), "12/22", "M0003", "AB4 5CD", "Succeeded", "TMX001", 1, 6 },
                    { 10000L, 120.18m, "HSBC0001", "svkLRj9nYEgZo7gWDJD5IQ==", "n7Na77vcGsJiVmOYhxSHFeNia8tb2y8trC42595bSBZBgrziJqf0ZLYkIiKXmLYb", "GBP", "Customer 1 AL1", "Mr Customer1", new DateTimeOffset(new DateTime(2021, 4, 5, 22, 8, 38, 869, DateTimeKind.Unspecified).AddTicks(8775), new TimeSpan(0, 1, 0, 0, 0)), new DateTimeOffset(new DateTime(2021, 4, 5, 22, 10, 38, 872, DateTimeKind.Unspecified).AddTicks(8034), new TimeSpan(0, 1, 0, 0, 0)), "12/21", "M0001", "AB1 2CD", "Succeeded", "T0001", 1, 1 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MerchantKeyMappings");

            migrationBuilder.DropTable(
                name: "Merchants");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "TransactionStatuses");

            migrationBuilder.DropTable(
                name: "TransactionTypes");
        }
    }
}
