Feature: Transactions
	As a Merchant
	I want to Post and Get transactions
	So that I can accept payments from my Ecommerce customers


Scenario: Trying to post a transaction without valid MerchantKey
	Given I am not authenticated merchant
	When I post a transaction
	| MerchantId | TerminalId | Amount  | Currency | CardNumber       | CVV | ExpiryDate   | CustomerName   | CustomerAddressLine1 | Postcode | TransactionTypeId | TransactionDate |
	| M0003      | TMX001     | 9999.99 | GBP      | 4444123433331111 | 123 | <<futuredate>> | BDD Customer 1 | BDD AL1              | BD1 GH2  | 3                 | <<pastdate>>    |
	Then the response has an HTTP status code of Unauthorized
	
Scenario: Trying to post a transaction for non registered terminal
	Given I am authenticated as 'Merchant1'
	When I post a transaction
	| MerchantId | TerminalId | Amount  | Currency | CardNumber       | CVV | ExpiryDate   | CustomerName   | CustomerAddressLine1 | Postcode | TransactionTypeId | TransactionDate |
	| M0001      | TMX001     | 9999.99 | GBP      | 4444123433331111 | 123 | <<futuredate>> | BDD Customer 1 | BDD AL1              | BD1 GH2  | 3                 | <<pastdate>>    |
	Then the response has an HTTP status code of Forbidden

Scenario: Trying to post a transaction with invalid input
	Given I am authenticated as 'Merchant1'
	When I post a transaction
	| MerchantId | TerminalId | Amount | Currency | CardNumber       | CVV | ExpiryDate | CustomerName | CustomerAddressLine1 | Postcode | TransactionTypeId | TransactionDate |
	|            |            |        |          | 444412343d331111 | ddd | 44/44      |              |                      |          |                   |                 |
	Then the response has an HTTP status code of BadRequest
	And I should see the following validation errors
	| Error                                                                           |
	| Merchant Id is required.                                                        |
	| Terminal Id is required.                                                        |
	| Amount is required.                                                             |
	| Amount should be greater than 0                                                 |
	| Currency is required.                                                           |
	| only GBP currency is supported.                                                 |
	| Card Number should contain only 16 digts                                        |
	| CVV length should contain only 3 or 4 digits                                    |
	| Card already expired or invalid expiry date. Expiry date should be mm/yy format |
	| Transaction Type Id is required.                                                |
	| Transaction Type Id should be greater than or equal to 1                        |
	| '0' not supported for Transaction Type Id                                       |
	| Transaction Date is required.                                                   |
	| Customer Name is required.                                                      |
	| Customer Address Line1 is required.                                             |
	| Post Code is required.                                                          |

	Scenario: Trying to post a transaction for a registered terminal with valid request
	Given I am authenticated as 'Merchant1'
	When I post a transaction
	| MerchantId | TerminalId | Amount  | Currency | CardNumber       | CVV | ExpiryDate   | CustomerName   | CustomerAddressLine1 | Postcode | TransactionTypeId | TransactionDate |
	| M0001      | T0002     | 9999.99 | GBP      | 4444123433331111 | 123 | <<futuredate>> | BDD Customer 1 | BDD AL1              | BD1 GH2  | 3                 | <<pastdate>>    |
	Then the response has an HTTP status code of Created
	When I try to get details of previous transaction
	Then the response has an HTTP status code of Ok
	And I should see transaction detail
	| TerminalId | TransactionId     | Amount  | Currency | CardNumber          | CVV | ExpiryDate     | CustomerName   | CustomerAddressLine1 | PostCode | TransactionType | DateTransactionCreated | dateTransactionUpdated     | transactionStatus | bankReference | statusReason |
	| T0002      | <<transactionId>> | 9999.99 | GBP      | ****-****-****-1111 | *** | <<expiryDate>> | BDD Customer 1 | BDD AL1              | BD1 GH2  | 3               | <<transactionDate>>    | <<transactionUpdatedDate>> | 1                 | 1111          | Succeeded    |
