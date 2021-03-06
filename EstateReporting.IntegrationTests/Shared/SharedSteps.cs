﻿namespace EstateReporting.IntegrationTests.Shared
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Common;
    using EstateManagement.DataTransferObjects;
    using EstateManagement.DataTransferObjects.Requests;
    using EstateManagement.DataTransferObjects.Responses;
    using EstateReporting.DataTransferObjects;
    using Newtonsoft.Json;
    using SecurityService.DataTransferObjects;
    using SecurityService.DataTransferObjects.Requests;
    using SecurityService.DataTransferObjects.Responses;
    using Shouldly;
    using TechTalk.SpecFlow;
    using TransactionProcessor.DataTransferObjects;
    using ClientDetails = Common.ClientDetails;
    using SortDirection = DataTransferObjects.SortDirection;

    [Binding]
    [Scope(Tag = "shared")]
    public class SharedSteps
    {
        private readonly ScenarioContext ScenarioContext;

        private readonly TestingContext TestingContext;

        public SharedSteps(ScenarioContext scenarioContext,
                         TestingContext testingContext)
        {
            this.ScenarioContext = scenarioContext;
            this.TestingContext = testingContext;
        }

        [Given(@"I have created the following estates")]
        [When(@"I create the following estates")]
        public async Task WhenICreateTheFollowingEstates(Table table)
        {
            foreach (TableRow tableRow in table.Rows)
            {
                String estateName = SpecflowTableHelper.GetStringRowValue(tableRow, "EstateName");

                CreateEstateRequest createEstateRequest = new CreateEstateRequest
                                                          {
                                                              EstateId = Guid.NewGuid(),
                                                              EstateName = estateName
                                                          };

                CreateEstateResponse response = await this.TestingContext.DockerHelper.EstateClient.CreateEstate(this.TestingContext.AccessToken, createEstateRequest, CancellationToken.None).ConfigureAwait(false);

                response.ShouldNotBeNull();
                response.EstateId.ShouldNotBe(Guid.Empty);

                // Cache the estate id
                this.TestingContext.AddEstateDetails(response.EstateId, estateName);

                this.TestingContext.Logger.LogInformation($"Estate {estateName} created with Id {response.EstateId}");
            }

            foreach (TableRow tableRow in table.Rows)
            {
                EstateDetails estateDetails = this.TestingContext.GetEstateDetails(tableRow);

                EstateResponse estate = null;
                await Retry.For(async () =>
                                {
                                    estate = await this.TestingContext.DockerHelper.EstateClient
                                                       .GetEstate(this.TestingContext.AccessToken, estateDetails.EstateId, CancellationToken.None).ConfigureAwait(false);
                                    estate.ShouldNotBeNull();
                                }, retryFor:TimeSpan.FromSeconds(90)).ConfigureAwait(false);

                estate.EstateName.ShouldBe(estateDetails.EstateName);
            }
        }

        [Given(@"I have assigned the following  operator to the merchants")]
        [When(@"I assign the following  operator to the merchants")]
        public async Task WhenIAssignTheFollowingOperatorToTheMerchants(Table table)
        {
            foreach (TableRow tableRow in table.Rows)
            {
                EstateDetails estateDetails = this.TestingContext.GetEstateDetails(tableRow);

                String token = this.TestingContext.AccessToken;
                if (String.IsNullOrEmpty(estateDetails.AccessToken) == false)
                {
                    token = estateDetails.AccessToken;
                }

                // Lookup the merchant id
                String merchantName = SpecflowTableHelper.GetStringRowValue(tableRow, "MerchantName");
                Guid merchantId = estateDetails.GetMerchantId(merchantName);

                // Lookup the operator id
                String operatorName = SpecflowTableHelper.GetStringRowValue(tableRow, "OperatorName");
                Guid operatorId = estateDetails.GetOperatorId(operatorName);

                AssignOperatorRequest assignOperatorRequest = new AssignOperatorRequest
                {
                    OperatorId = operatorId,
                    MerchantNumber = SpecflowTableHelper.GetStringRowValue(tableRow, "MerchantNumber"),
                    TerminalNumber = SpecflowTableHelper.GetStringRowValue(tableRow, "TerminalNumber"),
                };

                AssignOperatorResponse assignOperatorResponse = await this.TestingContext.DockerHelper.EstateClient.AssignOperatorToMerchant(token, estateDetails.EstateId, merchantId, assignOperatorRequest, CancellationToken.None).ConfigureAwait(false);

                assignOperatorResponse.EstateId.ShouldBe(estateDetails.EstateId);
                assignOperatorResponse.MerchantId.ShouldBe(merchantId);
                assignOperatorResponse.OperatorId.ShouldBe(operatorId);

                this.TestingContext.Logger.LogInformation($"Operator {operatorName} assigned to Estate {estateDetails.EstateName}");
            }
        }


        [Given(@"I have created the following operators")]
        [When(@"I create the following operators")]
        public async Task WhenICreateTheFollowingOperators(Table table)
        {
            foreach (TableRow tableRow in table.Rows)
            {
                String operatorName = SpecflowTableHelper.GetStringRowValue(tableRow, "OperatorName");
                Boolean requireCustomMerchantNumber = SpecflowTableHelper.GetBooleanValue(tableRow, "RequireCustomMerchantNumber");
                Boolean requireCustomTerminalNumber = SpecflowTableHelper.GetBooleanValue(tableRow, "RequireCustomTerminalNumber");

                CreateOperatorRequest createOperatorRequest = new CreateOperatorRequest
                {
                    Name = operatorName,
                    RequireCustomMerchantNumber = requireCustomMerchantNumber,
                    RequireCustomTerminalNumber = requireCustomTerminalNumber
                };

                // lookup the estate id based on the name in the table
                EstateDetails estateDetails = this.TestingContext.GetEstateDetails(tableRow);

                CreateOperatorResponse response = await this.TestingContext.DockerHelper.EstateClient.CreateOperator(this.TestingContext.AccessToken, estateDetails.EstateId, createOperatorRequest, CancellationToken.None).ConfigureAwait(false);

                response.ShouldNotBeNull();
                response.EstateId.ShouldNotBe(Guid.Empty);
                response.OperatorId.ShouldNotBe(Guid.Empty);

                // Cache the estate id
                estateDetails.AddOperator(response.OperatorId, operatorName);

                this.TestingContext.Logger.LogInformation($"Operator {operatorName} created with Id {response.OperatorId} for Estate {estateDetails.EstateName}");
            }
        }

        [Given("I create the following merchants")]
        [When(@"I create the following merchants")]
        public async Task WhenICreateTheFollowingMerchants(Table table)
        {
            foreach (TableRow tableRow in table.Rows)
            {
                // lookup the estate id based on the name in the table
                EstateDetails estateDetails = this.TestingContext.GetEstateDetails(tableRow);
                String token = this.TestingContext.AccessToken;
                if (String.IsNullOrEmpty(estateDetails.AccessToken) == false)
                {
                    token = estateDetails.AccessToken;
                }

                String merchantName = SpecflowTableHelper.GetStringRowValue(tableRow, "MerchantName");
                CreateMerchantRequest createMerchantRequest = new CreateMerchantRequest
                {
                    Name = merchantName,
                    Contact = new Contact
                    {
                        ContactName = SpecflowTableHelper.GetStringRowValue(tableRow, "ContactName"),
                        EmailAddress = SpecflowTableHelper.GetStringRowValue(tableRow, "EmailAddress")
                    },
                    Address = new Address
                    {
                        AddressLine1 = SpecflowTableHelper.GetStringRowValue(tableRow, "AddressLine1"),
                        Town = SpecflowTableHelper.GetStringRowValue(tableRow, "Town"),
                        Region = SpecflowTableHelper.GetStringRowValue(tableRow, "Region"),
                        Country = SpecflowTableHelper.GetStringRowValue(tableRow, "Country")
                    }
                };

                CreateMerchantResponse response = await this.TestingContext.DockerHelper.EstateClient
                                                            .CreateMerchant(token, estateDetails.EstateId, createMerchantRequest, CancellationToken.None).ConfigureAwait(false);

                response.ShouldNotBeNull();
                response.EstateId.ShouldBe(estateDetails.EstateId);
                response.MerchantId.ShouldNotBe(Guid.Empty);

                // Cache the merchant id
                estateDetails.AddMerchant(response.MerchantId, merchantName);

                this.TestingContext.Logger.LogInformation($"Merchant {merchantName} created with Id {response.MerchantId} for Estate {estateDetails.EstateName}");
            }

            foreach (TableRow tableRow in table.Rows)
            {
                EstateDetails estateDetails = this.TestingContext.GetEstateDetails(tableRow);

                String merchantName = SpecflowTableHelper.GetStringRowValue(tableRow, "MerchantName");

                Guid merchantId = estateDetails.GetMerchantId(merchantName);

                String token = this.TestingContext.AccessToken;
                if (String.IsNullOrEmpty(estateDetails.AccessToken) == false)
                {
                    token = estateDetails.AccessToken;
                }

                await Retry.For(async () =>
                                {
                                    MerchantResponse merchant = await this.TestingContext.DockerHelper.EstateClient
                                                                          .GetMerchant(token, estateDetails.EstateId, merchantId, CancellationToken.None)
                                                                          .ConfigureAwait(false);

                                    merchant.MerchantName.ShouldBe(merchantName);
                                });
            }
        }
        
        [When(@"I perform the following transactions")]
        public async Task WhenIPerformTheFollowingTransactions(Table table)
        {
            foreach (TableRow tableRow in table.Rows)
            {
                String merchantName = SpecflowTableHelper.GetStringRowValue(tableRow, "MerchantName");
                String dateString = SpecflowTableHelper.GetStringRowValue(tableRow, "DateTime");
                DateTime transactionDateTime = SpecflowTableHelper.GetDateForDateString(dateString, this.TestingContext.DateToUseForToday);
                // hack for UTC :|
                transactionDateTime = transactionDateTime.AddHours(1);
                String transactionNumber = SpecflowTableHelper.GetStringRowValue(tableRow, "TransactionNumber");
                String transactionType = SpecflowTableHelper.GetStringRowValue(tableRow, "TransactionType");
                String deviceIdentifier = SpecflowTableHelper.GetStringRowValue(tableRow, "DeviceIdentifier");
                
                EstateDetails estateDetails = this.TestingContext.GetEstateDetails(tableRow);
                
                // Lookup the merchant id
                Guid merchantId = estateDetails.GetMerchantId(merchantName);
                SerialisedMessage transactionResponse = null;
                switch (transactionType)
                {
                    case "Logon":
                        transactionResponse = await this.PerformLogonTransaction(estateDetails.EstateId,
                                                           merchantId,
                                                           transactionDateTime,
                                                           transactionType,
                                                           transactionNumber,
                                                           deviceIdentifier,
                                                           CancellationToken.None);
                        break;
                    case "Sale":

                        // Get specific sale fields
                        String operatorName = SpecflowTableHelper.GetStringRowValue(tableRow, "OperatorName");
                        Decimal transactionAmount = SpecflowTableHelper.GetDecimalValue(tableRow, "TransactionAmount");
                        String customerAccountNumber = SpecflowTableHelper.GetStringRowValue(tableRow, "CustomerAccountNumber");
                        String customerEmailAddress = SpecflowTableHelper.GetStringRowValue(tableRow, "CustomerEmailAddress");
                        String contractDescription = SpecflowTableHelper.GetStringRowValue(tableRow, "ContractDescription");
                        String productName = SpecflowTableHelper.GetStringRowValue(tableRow, "ProductName");
                        String recipientMobile = SpecflowTableHelper.GetStringRowValue(tableRow, "RecipientMobile");

                        Guid contractId = Guid.Empty;
                        Guid productId = Guid.Empty;
                        var contract = estateDetails.GetContract(contractDescription);
                        if (contract != null)
                        {
                            contractId = contract.ContractId;
                            var product = contract.GetProduct(productName);
                            productId = product.ProductId;
                        }

                        transactionResponse = await this.PerformSaleTransaction(estateDetails.EstateId,
                                                                                merchantId,
                                                                                transactionDateTime,
                                                                                transactionType,
                                                                                transactionNumber,
                                                                                deviceIdentifier,
                                                                                operatorName,
                                                                                transactionAmount,
                                                                                customerAccountNumber,
                                                                                customerEmailAddress,
                                                                                contractId,
                                                                                productId,
                                                                                recipientMobile,
                                                                                CancellationToken.None);
                        break;
                        
                }

                estateDetails.AddTransactionResponse(merchantId, transactionNumber, transactionResponse);
            }
        }

        [Given(@"I create a contract with the following values")]
        public async Task GivenICreateAContractWithTheFollowingValues(Table table)
        {
            foreach (TableRow tableRow in table.Rows)
            {
                EstateDetails estateDetails = this.TestingContext.GetEstateDetails(tableRow);

                String token = this.TestingContext.AccessToken;
                if (String.IsNullOrEmpty(estateDetails.AccessToken) == false)
                {
                    token = estateDetails.AccessToken;
                }

                String operatorName = SpecflowTableHelper.GetStringRowValue(tableRow, "OperatorName");
                Guid operatorId = estateDetails.GetOperatorId(operatorName);

                CreateContractRequest createContractRequest = new CreateContractRequest
                                                              {
                                                                  OperatorId = operatorId,
                                                                  Description = SpecflowTableHelper.GetStringRowValue(tableRow, "ContractDescription")
                                                              };

                CreateContractResponse contractResponse =
                    await this.TestingContext.DockerHelper.EstateClient.CreateContract(token, estateDetails.EstateId, createContractRequest, CancellationToken.None);

                estateDetails.AddContract(contractResponse.ContractId, createContractRequest.Description, operatorId);
            }
        }

        [When(@"I create the following Products")]
        public async Task WhenICreateTheFollowingProducts(Table table)
        {
            foreach (TableRow tableRow in table.Rows)
            {
                EstateDetails estateDetails = this.TestingContext.GetEstateDetails(tableRow);

                String token = this.TestingContext.AccessToken;
                if (String.IsNullOrEmpty(estateDetails.AccessToken) == false)
                {
                    token = estateDetails.AccessToken;
                }

                String contractName = SpecflowTableHelper.GetStringRowValue(tableRow, "ContractDescription");
                Contract contract = estateDetails.GetContract(contractName);
                String productValue = SpecflowTableHelper.GetStringRowValue(tableRow, "Value");

                AddProductToContractRequest addProductToContractRequest = new AddProductToContractRequest
                {
                    ProductName = SpecflowTableHelper.GetStringRowValue(tableRow, "ProductName"),
                    DisplayText = SpecflowTableHelper.GetStringRowValue(tableRow, "DisplayText"),
                    Value = null
                };
                if (String.IsNullOrEmpty(productValue) == false)
                {
                    addProductToContractRequest.Value = Decimal.Parse(productValue);
                }

                AddProductToContractResponse addProductToContractResponse = await this.TestingContext.DockerHelper.EstateClient.AddProductToContract(token,
                                                                                                                                                     estateDetails.EstateId,
                                                                                                                                                     contract.ContractId,
                                                                                                                                                     addProductToContractRequest,
                                                                                                                                                     CancellationToken.None);

                contract.AddProduct(addProductToContractResponse.ProductId, addProductToContractRequest.ProductName, addProductToContractRequest.DisplayText,
                                    addProductToContractRequest.Value);
            }
        }

        [When(@"I add the following Transaction Fees")]
        public async Task WhenIAddTheFollowingTransactionFees(Table table)
        {
            foreach (TableRow tableRow in table.Rows)
            {
                EstateDetails estateDetails = this.TestingContext.GetEstateDetails(tableRow);

                String token = this.TestingContext.AccessToken;
                if (String.IsNullOrEmpty(estateDetails.AccessToken) == false)
                {
                    token = estateDetails.AccessToken;
                }

                String contractName = SpecflowTableHelper.GetStringRowValue(tableRow, "ContractDescription");
                String productName = SpecflowTableHelper.GetStringRowValue(tableRow, "ProductName");
                Contract contract = estateDetails.GetContract(contractName);

                Product product = contract.GetProduct(productName);

                AddTransactionFeeForProductToContractRequest addTransactionFeeForProductToContractRequest = new AddTransactionFeeForProductToContractRequest
                {
                    Value =
                                                                                                                    SpecflowTableHelper
                                                                                                                        .GetDecimalValue(tableRow, "Value"),
                    Description =
                                                                                                                    SpecflowTableHelper.GetStringRowValue(tableRow,
                                                                                                                                                          "FeeDescription"),
                    CalculationType =
                                                                                                                    SpecflowTableHelper
                                                                                                                        .GetEnumValue<CalculationType>(tableRow,
                                                                                                                                                       "CalculationType")
                };

                AddTransactionFeeForProductToContractResponse addTransactionFeeForProductToContractResponse =
                    await this.TestingContext.DockerHelper.EstateClient.AddTransactionFeeForProductToContract(token,
                                                                                                              estateDetails.EstateId,
                                                                                                              contract.ContractId,
                                                                                                              product.ProductId,
                                                                                                              addTransactionFeeForProductToContractRequest,
                                                                                                              CancellationToken.None);

                product.AddTransactionFee(addTransactionFeeForProductToContractResponse.TransactionFeeId,
                                          addTransactionFeeForProductToContractRequest.CalculationType,
                                          addTransactionFeeForProductToContractRequest.Description,
                                          addTransactionFeeForProductToContractRequest.Value);
            }
        }

        /// <summary>
        /// Performs the logon transaction.
        /// </summary>
        /// <param name="estateId">The estate identifier.</param>
        /// <param name="merchantId">The merchant identifier.</param>
        /// <param name="transactionDateTime">The transaction date time.</param>
        /// <param name="transactionType">Type of the transaction.</param>
        /// <param name="transactionNumber">The transaction number.</param>
        /// <param name="deviceIdentifier">The device identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        private async Task<SerialisedMessage> PerformLogonTransaction(Guid estateId, Guid merchantId, DateTime transactionDateTime, String transactionType, String transactionNumber, String deviceIdentifier, CancellationToken cancellationToken)
        {
            LogonTransactionRequest logonTransactionRequest = new LogonTransactionRequest
                                                              {
                                                                  MerchantId = merchantId,
                                                                  EstateId = estateId,
                                                                  TransactionDateTime = transactionDateTime,
                                                                  TransactionNumber = transactionNumber,
                                                                  DeviceIdentifier = deviceIdentifier,
                                                                  TransactionType = transactionType
                                                              };

            SerialisedMessage serialisedMessage = new SerialisedMessage();
            serialisedMessage.Metadata.Add(MetadataContants.KeyNameEstateId, estateId.ToString());
            serialisedMessage.Metadata.Add(MetadataContants.KeyNameMerchantId, merchantId.ToString());
            serialisedMessage.SerialisedData = JsonConvert.SerializeObject(logonTransactionRequest, new JsonSerializerSettings
                                                                                                    {
                                                                                                        TypeNameHandling = TypeNameHandling.All
                                                                                                    });

            SerialisedMessage responseSerialisedMessage =
                await this.TestingContext.DockerHelper.TransactionProcessorClient.PerformTransaction(this.TestingContext.AccessToken,
                                                                                                     serialisedMessage,
                                                                                                     cancellationToken);

            return responseSerialisedMessage;
        }

        private async Task<SerialisedMessage> PerformSaleTransaction(Guid estateId, Guid merchantId, DateTime transactionDateTime, 
                                                                     String transactionType, String transactionNumber, String deviceIdentifier, 
                                                                     String operatorIdentifier, Decimal transactionAmount, String customerAccountNumber, 
                                                                     String customerEmailAddress,
                                                                     Guid contractId,
                                                                     Guid productId,
                                                                     String recipientMobile,
                                                                     CancellationToken cancellationToken)
        {
            var additionalTransactionMetadata = new Dictionary<String, String>();

            if (operatorIdentifier == "Voucher")
            {
                additionalTransactionMetadata = new Dictionary<String, String>
                                                {
                                                    {"Amount", transactionAmount.ToString()},
                                                    {"RecipientMobile", recipientMobile}
                                                };
            }
            else
            {
                additionalTransactionMetadata = new Dictionary<String, String>
                                                {
                                                    {"Amount", transactionAmount.ToString()},
                                                    {"CustomerAccountNumber", customerAccountNumber}
                                                };
            }

            SaleTransactionRequest saleTransactionRequest = new SaleTransactionRequest
                                                            {
                                                                MerchantId = merchantId,
                                                                EstateId = estateId,
                                                                TransactionDateTime = transactionDateTime,
                                                                TransactionNumber = transactionNumber,
                                                                DeviceIdentifier = deviceIdentifier,
                                                                TransactionType = transactionType,
                                                                OperatorIdentifier = operatorIdentifier,
                                                                AdditionalTransactionMetadata = additionalTransactionMetadata,
                                                                CustomerEmailAddress = customerEmailAddress,
                                                                ProductId = productId,
                                                                ContractId = contractId
            };

            SerialisedMessage serialisedMessage = new SerialisedMessage();
            serialisedMessage.Metadata.Add(MetadataContants.KeyNameEstateId, estateId.ToString());
            serialisedMessage.Metadata.Add(MetadataContants.KeyNameMerchantId, merchantId.ToString());
            serialisedMessage.SerialisedData = JsonConvert.SerializeObject(saleTransactionRequest, new JsonSerializerSettings
                                                                                                   {
                                                                                                       TypeNameHandling = TypeNameHandling.All
                                                                                                   });

            SerialisedMessage responseSerialisedMessage =
                await this.TestingContext.DockerHelper.TransactionProcessorClient.PerformTransaction(this.TestingContext.AccessToken,
                                                                                                     serialisedMessage,
                                                                                                     cancellationToken);

            return responseSerialisedMessage;
        }

        [Then(@"transaction response should contain the following information")]
        public void ThenTransactionResponseShouldContainTheFollowingInformation(Table table)
        {
            foreach (TableRow tableRow in table.Rows)
            {
                // Get the merchant name
                EstateDetails estateDetails = this.TestingContext.GetEstateDetails(tableRow);

                String merchantName = SpecflowTableHelper.GetStringRowValue(tableRow, "MerchantName");
                Guid merchantId = estateDetails.GetMerchantId(merchantName);
                
                String transactionNumber = SpecflowTableHelper.GetStringRowValue(tableRow, "TransactionNumber");
                SerialisedMessage serialisedMessage = estateDetails.GetTransactionResponse(merchantId, transactionNumber);
                Object transactionResponse = JsonConvert.DeserializeObject(serialisedMessage.SerialisedData,
                                                      new JsonSerializerSettings
                                                      {
                                                          TypeNameHandling = TypeNameHandling.All
                                                      });
                this.ValidateTransactionResponse(transactionNumber, (dynamic)transactionResponse, tableRow);
            }
        }

        private void ValidateTransactionResponse(String transactionNumber, LogonTransactionResponse logonTransactionResponse,
                                           TableRow tableRow)
        {
            String expectedResponseCode = SpecflowTableHelper.GetStringRowValue(tableRow, "ResponseCode");
            String expectedResponseMessage = SpecflowTableHelper.GetStringRowValue(tableRow, "ResponseMessage");

            logonTransactionResponse.ResponseCode.ShouldBe(expectedResponseCode, $"Transaction Number [{transactionNumber}]");
            logonTransactionResponse.ResponseMessage.ShouldBe(expectedResponseMessage, $"Transaction Number [{transactionNumber}]");
        }

        private void ValidateTransactionResponse(String transactionNumber, SaleTransactionResponse saleTransactionResponse,
                                                 TableRow tableRow)
        {
            String expectedResponseCode = SpecflowTableHelper.GetStringRowValue(tableRow, "ResponseCode");
            String expectedResponseMessage = SpecflowTableHelper.GetStringRowValue(tableRow, "ResponseMessage");

            saleTransactionResponse.ResponseCode.ShouldBe(expectedResponseCode, $"Transaction Number [{transactionNumber}]");
            saleTransactionResponse.ResponseMessage.ShouldBe(expectedResponseMessage, $"Transaction Number [{transactionNumber}]");
        }

        [Given(@"I create the following api scopes")]
        public async Task GivenICreateTheFollowingApiScopes(Table table)
        {
            foreach (TableRow tableRow in table.Rows)
            {
                CreateApiScopeRequest createApiScopeRequest = new CreateApiScopeRequest
                                                              {
                                                                  Name = SpecflowTableHelper.GetStringRowValue(tableRow, "Name"),
                                                                  Description = SpecflowTableHelper.GetStringRowValue(tableRow, "Description"),
                                                                  DisplayName = SpecflowTableHelper.GetStringRowValue(tableRow, "DisplayName")
                                                              };
                var createApiScopeResponse =
                    await this.CreateApiScope(createApiScopeRequest, CancellationToken.None).ConfigureAwait(false);

                createApiScopeResponse.ShouldNotBeNull();
                createApiScopeResponse.ApiScopeName.ShouldNotBeNullOrEmpty();
            }
        }

        private async Task<CreateApiScopeResponse> CreateApiScope(CreateApiScopeRequest createApiScopeRequest,
                                                                  CancellationToken cancellationToken)
        {
            CreateApiScopeResponse createApiScopeResponse = await this.TestingContext.DockerHelper.SecurityServiceClient.CreateApiScope(createApiScopeRequest, cancellationToken).ConfigureAwait(false);
            return createApiScopeResponse;
        }

        [Given(@"the following api resources exist")]
        public async Task GivenTheFollowingApiResourcesExist(Table table)
        {
            foreach (TableRow tableRow in table.Rows)
            {
                String resourceName = SpecflowTableHelper.GetStringRowValue(tableRow, "ResourceName");
                String displayName = SpecflowTableHelper.GetStringRowValue(tableRow, "DisplayName");
                String secret = SpecflowTableHelper.GetStringRowValue(tableRow, "Secret");
                String scopes = SpecflowTableHelper.GetStringRowValue(tableRow, "Scopes");
                String userClaims = SpecflowTableHelper.GetStringRowValue(tableRow, "UserClaims");

                List<String> splitScopes = scopes.Split(",").ToList();
                List<String> splitUserClaims = userClaims.Split(",").ToList();

                CreateApiResourceRequest createApiResourceRequest = new CreateApiResourceRequest
                {
                    Description = String.Empty,
                    DisplayName = displayName,
                    Name = resourceName,
                    Scopes = new List<String>(),
                    Secret = secret,
                    UserClaims = new List<String>()
                };
                splitScopes.ForEach(a =>
                {
                    createApiResourceRequest.Scopes.Add(a.Trim());
                });
                splitUserClaims.ForEach(a =>
                {
                    createApiResourceRequest.UserClaims.Add(a.Trim());
                });

                CreateApiResourceResponse createApiResourceResponse = await this.TestingContext.DockerHelper.SecurityServiceClient.CreateApiResource(createApiResourceRequest, CancellationToken.None).ConfigureAwait(false);

                createApiResourceResponse.ApiResourceName.ShouldBe(resourceName);
            }
        }

        [Given(@"the following clients exist")]
        public async Task GivenTheFollowingClientsExist(Table table)
        {
            foreach (TableRow tableRow in table.Rows)
            {
                String clientId = SpecflowTableHelper.GetStringRowValue(tableRow, "ClientId");
                String clientName = SpecflowTableHelper.GetStringRowValue(tableRow, "ClientName");
                String secret = SpecflowTableHelper.GetStringRowValue(tableRow, "Secret");
                String allowedScopes = SpecflowTableHelper.GetStringRowValue(tableRow, "AllowedScopes");
                String allowedGrantTypes = SpecflowTableHelper.GetStringRowValue(tableRow, "AllowedGrantTypes");

                List<String> splitAllowedScopes = allowedScopes.Split(",").ToList();
                List<String> splitAllowedGrantTypes = allowedGrantTypes.Split(",").ToList();

                CreateClientRequest createClientRequest = new CreateClientRequest
                {
                    Secret = secret,
                    AllowedGrantTypes = new List<String>(),
                    AllowedScopes = new List<String>(),
                    ClientDescription = String.Empty,
                    ClientId = clientId,
                    ClientName = clientName
                };

                splitAllowedScopes.ForEach(a =>
                {
                    createClientRequest.AllowedScopes.Add(a.Trim());
                });
                splitAllowedGrantTypes.ForEach(a =>
                {
                    createClientRequest.AllowedGrantTypes.Add(a.Trim());
                });

                CreateClientResponse createClientResponse = await this.TestingContext.DockerHelper.SecurityServiceClient.CreateClient(createClientRequest, CancellationToken.None).ConfigureAwait(false);

                createClientResponse.ClientId.ShouldBe(clientId);

                this.TestingContext.AddClientDetails(clientId, secret, allowedGrantTypes);
            }
        }

        [Given(@"I have a token to access the estate management and transaction processor resources")]
        public async Task GivenIHaveATokenToAccessTheEstateManagementAndTransactionProcessorResources(Table table)
        {
            foreach (TableRow tableRow in table.Rows)
            {
                String clientId = SpecflowTableHelper.GetStringRowValue(tableRow, "ClientId");

                ClientDetails clientDetails = this.TestingContext.GetClientDetails(clientId);

                if (clientDetails.GrantType == "client_credentials")
                {
                    TokenResponse tokenResponse = await this.TestingContext.DockerHelper.SecurityServiceClient.GetToken(clientId, clientDetails.ClientSecret, CancellationToken.None).ConfigureAwait(false);

                    this.TestingContext.AccessToken = tokenResponse.AccessToken;
                }
            }
        }

        [Given(@"I have assigned the following devices to the merchants")]
        public async Task GivenIHaveAssignedTheFollowingDevicesToTheMerchants(Table table)
        {
            foreach (TableRow tableRow in table.Rows)
            {
                EstateDetails estateDetails = this.TestingContext.GetEstateDetails(tableRow);

                String token = this.TestingContext.AccessToken;
                if (String.IsNullOrEmpty(estateDetails.AccessToken) == false)
                {
                    token = estateDetails.AccessToken;
                }

                // Lookup the merchant id
                String merchantName = SpecflowTableHelper.GetStringRowValue(tableRow, "MerchantName");
                Guid merchantId = estateDetails.GetMerchantId(merchantName);

                // Lookup the operator id
                String deviceIdentifier = SpecflowTableHelper.GetStringRowValue(tableRow, "DeviceIdentifier");

                AddMerchantDeviceRequest addMerchantDeviceRequest = new AddMerchantDeviceRequest
                                                                    {
                                                                        DeviceIdentifier = deviceIdentifier
                                                                    };

                AddMerchantDeviceResponse addMerchantDeviceResponse = await this.TestingContext.DockerHelper.EstateClient.AddDeviceToMerchant(token, estateDetails.EstateId, merchantId, addMerchantDeviceRequest, CancellationToken.None).ConfigureAwait(false);

                addMerchantDeviceResponse.EstateId.ShouldBe(estateDetails.EstateId);
                addMerchantDeviceResponse.MerchantId.ShouldBe(merchantId);
                addMerchantDeviceResponse.DeviceId.ShouldNotBe(Guid.Empty);

                this.TestingContext.Logger.LogInformation($"Device {deviceIdentifier} assigned to Merchant {merchantName} Estate {estateDetails.EstateName}");
            }
        }

        [Given(@"I make the following manual merchant deposits")]
        public async Task GivenIMakeTheFollowingManualMerchantDeposits(Table table)
        {
            foreach (TableRow tableRow in table.Rows)
            {
                EstateDetails estateDetails = this.TestingContext.GetEstateDetails(tableRow);

                String token = this.TestingContext.AccessToken;
                if (String.IsNullOrEmpty(estateDetails.AccessToken) == false)
                {
                    token = estateDetails.AccessToken;
                }

                // Lookup the merchant id
                String merchantName = SpecflowTableHelper.GetStringRowValue(tableRow, "MerchantName");
                Guid merchantId = estateDetails.GetMerchantId(merchantName);

                // Get current balance
                MerchantBalanceResponse previousMerchantBalance = await this.TestingContext.DockerHelper.EstateClient.GetMerchantBalance(token, estateDetails.EstateId, merchantId, CancellationToken.None);
                
                MakeMerchantDepositRequest makeMerchantDepositRequest = new MakeMerchantDepositRequest
                                                                        {
                                                                            DepositDateTime = SpecflowTableHelper.GetDateForDateString(SpecflowTableHelper.GetStringRowValue(tableRow, "DateTime"), this.TestingContext.DateToUseForToday),
                                                                            Source = MerchantDepositSource.Manual,
                                                                            Reference = SpecflowTableHelper.GetStringRowValue(tableRow, "Reference"),
                                                                            Amount = SpecflowTableHelper.GetDecimalValue(tableRow, "Amount")
                                                                        };

                MakeMerchantDepositResponse makeMerchantDepositResponse = await this.TestingContext.DockerHelper.EstateClient.MakeMerchantDeposit(token, estateDetails.EstateId, merchantId, makeMerchantDepositRequest, CancellationToken.None).ConfigureAwait(false);

                makeMerchantDepositResponse.EstateId.ShouldBe(estateDetails.EstateId);
                makeMerchantDepositResponse.MerchantId.ShouldBe(merchantId);
                makeMerchantDepositResponse.DepositId.ShouldNotBe(Guid.Empty);

                this.TestingContext.Logger.LogInformation($"Deposit Reference {makeMerchantDepositRequest.Reference} made for Merchant {merchantName}");

                // Check the merchant balance
                MerchantBalanceResponse currentMerchantBalance = await this.TestingContext.DockerHelper.EstateClient.GetMerchantBalance(token, estateDetails.EstateId, merchantId, CancellationToken.None);

                currentMerchantBalance.AvailableBalance.ShouldBe(previousMerchantBalance.AvailableBalance + makeMerchantDepositRequest.Amount);

            }
        }

        [When(@"I get the Estate Transactions By Date Report for Estate '(.*)' with the Start Date '(.*)' and the End Date '(.*)' the following data is returned")]
        public async Task WhenIGetTheEstateTransactionsByDateReportForEstateWithTheStartDateAndTheEndDateTheFollowingDataIsReturned(String estateName, String startDateString, String endDateString, Table table)
        {
            EstateDetails estateDetails = this.TestingContext.GetEstateDetails(estateName);

            String token = this.TestingContext.AccessToken;
            if (String.IsNullOrEmpty(estateDetails.AccessToken) == false)
            {
                token = estateDetails.AccessToken;
            }

            String startDate = SpecflowTableHelper.GetDateForDateString(startDateString, this.TestingContext.DateToUseForToday).ToString("yyyyMMdd");
            String endDate = SpecflowTableHelper.GetDateForDateString(endDateString, this.TestingContext.DateToUseForToday).ToString("yyyyMMdd");

            await Retry.For(async () =>
                            {
                                TransactionsByDayResponse response = await this.TestingContext.DockerHelper.EstateReportingClient
                                                                               .GetTransactionsForEstateByDate(token,
                                                                                                      estateDetails.EstateId,
                                                                                                      startDate,
                                                                                                      endDate,
                                                                                                      CancellationToken.None).ConfigureAwait(false);

                                response.ShouldNotBeNull();
                                response.TransactionDayResponses.ShouldNotBeNull();
                                response.TransactionDayResponses.ShouldNotBeEmpty();

                                foreach (TableRow tableRow in table.Rows)
                                {
                                    DateTime date = SpecflowTableHelper.GetDateForDateString(tableRow["Date"], this.TestingContext.DateToUseForToday);
                                    Int32 numberOfTransactions = SpecflowTableHelper.GetIntValue(tableRow, "NumberOfTransactions");
                                    Decimal valueOfTransactions = SpecflowTableHelper.GetDecimalValue(tableRow, "ValueOfTransactions");

                                    TransactionDayResponse transactionDayResponse = response.TransactionDayResponses.SingleOrDefault(t => t.Date == date);

                                    transactionDayResponse.ShouldNotBeNull();
                                    transactionDayResponse.NumberOfTransactions.ShouldBe(numberOfTransactions);
                                    transactionDayResponse.ValueOfTransactions.ShouldBe(valueOfTransactions);
                                }
                            });
        }

        [When(@"I get the Merchant Transactions By Date Report for Estate '(.*)' and Merchant '(.*)' with the Start Date '(.*)' and the End Date '(.*)' the following data is returned")]
        public async Task WhenIGetTheMerchantTransactionsByDateReportForEstateAndMerchantWithTheStartDateAndTheEndDateTheFollowingDataIsReturned(String estateName, String merchantName, String startDateString, String endDateString, Table table)
        {
            EstateDetails estateDetails = this.TestingContext.GetEstateDetails(estateName);
            Guid merchantId = estateDetails.GetMerchantId(merchantName);
            String token = this.TestingContext.AccessToken;
            if (String.IsNullOrEmpty(estateDetails.AccessToken) == false)
            {
                token = estateDetails.AccessToken;
            }

            String startDate = SpecflowTableHelper.GetDateForDateString(startDateString, this.TestingContext.DateToUseForToday).ToString("yyyyMMdd");
            String endDate = SpecflowTableHelper.GetDateForDateString(endDateString, this.TestingContext.DateToUseForToday).ToString("yyyyMMdd");

            await Retry.For(async () =>
            {
                TransactionsByDayResponse response = await this.TestingContext.DockerHelper.EstateReportingClient
                                                               .GetTransactionsForMerchantByDate(token,
                                                                                      estateDetails.EstateId,
                                                                                      merchantId,
                                                                                      startDate,
                                                                                      endDate,
                                                                                      CancellationToken.None).ConfigureAwait(false);

                response.ShouldNotBeNull();
                response.TransactionDayResponses.ShouldNotBeNull();
                response.TransactionDayResponses.ShouldNotBeEmpty();

                foreach (TableRow tableRow in table.Rows)
                {
                    DateTime date = SpecflowTableHelper.GetDateForDateString(tableRow["Date"], this.TestingContext.DateToUseForToday);
                    Int32 numberOfTransactions = SpecflowTableHelper.GetIntValue(tableRow, "NumberOfTransactions");
                    Decimal valueOfTransactions = SpecflowTableHelper.GetDecimalValue(tableRow, "ValueOfTransactions");

                    TransactionDayResponse transactionDayResponse = response.TransactionDayResponses.SingleOrDefault(t => t.Date == date);

                    transactionDayResponse.ShouldNotBeNull();
                    transactionDayResponse.NumberOfTransactions.ShouldBe(numberOfTransactions);
                    transactionDayResponse.ValueOfTransactions.ShouldBe(valueOfTransactions);
                }
            });
        }

        [When(@"I get the Estate Transactions By Operator Report for Estate '(.*)' with the Start Date '(.*)' and the End Date '(.*)' the following data is returned")]
        public async Task WhenIGetTheEstateTransactionsByOperatorReportForEstateWithTheStartDateAndTheEndDateTheFollowingDataIsReturned(String estateName, String startDateString, String endDateString, Table table)
        {
            EstateDetails estateDetails = this.TestingContext.GetEstateDetails(estateName);

            String token = this.TestingContext.AccessToken;
            if (String.IsNullOrEmpty(estateDetails.AccessToken) == false)
            {
                token = estateDetails.AccessToken;
            }

            String startDate = SpecflowTableHelper.GetDateForDateString(startDateString, this.TestingContext.DateToUseForToday).ToString("yyyyMMdd");
            String endDate = SpecflowTableHelper.GetDateForDateString(endDateString, this.TestingContext.DateToUseForToday).ToString("yyyyMMdd");

            await Retry.For(async () =>
            {
                TransactionsByOperatorResponse response = await this.TestingContext.DockerHelper.EstateReportingClient
                                                                    .GetTransactionsForEstateByOperator(token,
                                                                                                        estateDetails.EstateId,
                                                                                                        startDate,
                                                                                                        endDate,
                                                                                                        5,
                                                                                                        SortDirection.Ascending,
                                                                                                        SortField.Value,
                                                                                                        CancellationToken.None).ConfigureAwait(false);

                response.ShouldNotBeNull();
                response.TransactionOperatorResponses.ShouldNotBeNull();
                response.TransactionOperatorResponses.ShouldNotBeEmpty();

                foreach (TableRow tableRow in table.Rows)
                {
                    String operatorName = SpecflowTableHelper.GetStringRowValue(tableRow, "OperatorName");
                    Int32 numberOfTransactions = SpecflowTableHelper.GetIntValue(tableRow, "NumberOfTransactions");
                    Decimal valueOfTransactions = SpecflowTableHelper.GetDecimalValue(tableRow, "ValueOfTransactions");

                    TransactionOperatorResponse transactionOperatorResponse = response.TransactionOperatorResponses.SingleOrDefault(t => t.OperatorName == operatorName);

                    transactionOperatorResponse.ShouldNotBeNull();
                    transactionOperatorResponse.NumberOfTransactions.ShouldBe(numberOfTransactions);
                    transactionOperatorResponse.ValueOfTransactions.ShouldBe(valueOfTransactions);
                }
            });
        }


        [When(@"I get the Estate Transactions By Merchant Report for Estate '(.*)' with the Start Date '(.*)' and the End Date '(.*)' the following data is returned")]
        public async Task WhenIGetTheEstateTransactionsByMerchantReportForEstateWithTheStartDateAndTheEndDateTheFollowingDataIsReturned(String estateName, String startDateString, String endDateString, Table table)
        {
            EstateDetails estateDetails = this.TestingContext.GetEstateDetails(estateName);

            String token = this.TestingContext.AccessToken;
            if (String.IsNullOrEmpty(estateDetails.AccessToken) == false)
            {
                token = estateDetails.AccessToken;
            }

            String startDate = SpecflowTableHelper.GetDateForDateString(startDateString, this.TestingContext.DateToUseForToday).ToString("yyyyMMdd");
            String endDate = SpecflowTableHelper.GetDateForDateString(endDateString, this.TestingContext.DateToUseForToday).ToString("yyyyMMdd");

            await Retry.For(async () =>
            {
                TransactionsByMerchantResponse response = await this.TestingContext.DockerHelper.EstateReportingClient
                                                                    .GetTransactionsForEstateByMerchant(token,
                                                                                                        estateDetails.EstateId,
                                                                                                        startDate,
                                                                                                        endDate,
                                                                                                        5,
                                                                                                        SortDirection.Ascending,
                                                                                                        SortField.Value,
                                                                                                        CancellationToken.None).ConfigureAwait(false);

                response.ShouldNotBeNull();
                response.TransactionMerchantResponses.ShouldNotBeNull();
                response.TransactionMerchantResponses.ShouldNotBeEmpty();

                foreach (TableRow tableRow in table.Rows)
                {
                    String merchantName = SpecflowTableHelper.GetStringRowValue(tableRow, "MerchantName");
                    Int32 numberOfTransactions = SpecflowTableHelper.GetIntValue(tableRow, "NumberOfTransactions");
                    Decimal valueOfTransactions = SpecflowTableHelper.GetDecimalValue(tableRow, "ValueOfTransactions");

                    TransactionMerchantResponse transactionMerchantResponse = response.TransactionMerchantResponses.SingleOrDefault(t => t.MerchantName == merchantName);

                    transactionMerchantResponse.ShouldNotBeNull();
                    transactionMerchantResponse.NumberOfTransactions.ShouldBe(numberOfTransactions);
                    transactionMerchantResponse.ValueOfTransactions.ShouldBe(valueOfTransactions);
                }
            });
        }


        [Given(@"I set the date for today to ""(.*)""")]
        public void GivenISetTheDateForTodayTo(String dateForToday)
        {
            if (dateForToday.ToUpper() == "TODAY")
            {
                this.TestingContext.DateToUseForToday = DateTime.UtcNow.Date;

            }
            else
            {
                // Just use parse so that an exception is thrown if an incorrect date format us used.
                this.TestingContext.DateToUseForToday = DateTime.ParseExact(dateForToday,"dd/MM/yyyy", null);
            }
        }

        [When(@"I get the Estate Transactions By Week Report for Estate '(.*)' with the Start Date '(.*)' and the End Date '(.*)' the following data is returned")]
        public async Task WhenIGetTheEstateTransactionsByWeekReportForEstateWithTheStartDateAndTheEndDateTheFollowingDataIsReturned(String estateName, String startDateString, String endDateString, Table table)
        {
            EstateDetails estateDetails = this.TestingContext.GetEstateDetails(estateName);
            String token = this.TestingContext.AccessToken;
            if (String.IsNullOrEmpty(estateDetails.AccessToken) == false)
            {
                token = estateDetails.AccessToken;
            }

            String startDate = SpecflowTableHelper.GetDateForDateString(startDateString, this.TestingContext.DateToUseForToday).ToString("yyyyMMdd");
            String endDate = SpecflowTableHelper.GetDateForDateString(endDateString, this.TestingContext.DateToUseForToday).ToString("yyyyMMdd");

            await Retry.For(async () =>
            {
                TransactionsByWeekResponse response = await this.TestingContext.DockerHelper.EstateReportingClient
                                                               .GetTransactionsForEstateByWeek(token,
                                                                                      estateDetails.EstateId,
                                                                                      startDate,
                                                                                      endDate,
                                                                                      CancellationToken.None).ConfigureAwait(false);

                response.ShouldNotBeNull();
                response.TransactionWeekResponses.ShouldNotBeNull();
                response.TransactionWeekResponses.ShouldNotBeEmpty();

                foreach (TableRow tableRow in table.Rows)
                {
                    Int32 weekNumber = SpecflowTableHelper.GetIntValue(tableRow, "WeekNumber");
                    Int32 year = SpecflowTableHelper.GetIntValue(tableRow, "Year");
                    Int32 numberOfTransactions = SpecflowTableHelper.GetIntValue(tableRow, "NumberOfTransactions");
                    Decimal valueOfTransactions = SpecflowTableHelper.GetDecimalValue(tableRow, "ValueOfTransactions");

                    TransactionWeekResponse transactionWeekResponse = response.TransactionWeekResponses.SingleOrDefault(t => t.WeekNumber == weekNumber && t.Year == year);

                    transactionWeekResponse.ShouldNotBeNull();
                    transactionWeekResponse.NumberOfTransactions.ShouldBe(numberOfTransactions);
                    transactionWeekResponse.ValueOfTransactions.ShouldBe(valueOfTransactions);
                }
            });
        }

        [When(@"I get the Merchant Transactions By Week Report for Estate '(.*)' and Merchant '(.*)' with the Start Date '(.*)' and the End Date '(.*)' the following data is returned")]
        public async Task WhenIGetTheMerchantTransactionsByWeekReportForEstateAndMerchantWithTheStartDateAndTheEndDateTheFollowingDataIsReturned(string estateName, string merchantName, string startDateString, string endDateString, Table table)
        {
            EstateDetails estateDetails = this.TestingContext.GetEstateDetails(estateName);
            Guid merchantId = estateDetails.GetMerchantId(merchantName);
            String token = this.TestingContext.AccessToken;
            if (String.IsNullOrEmpty(estateDetails.AccessToken) == false)
            {
                token = estateDetails.AccessToken;
            }

            String startDate = SpecflowTableHelper.GetDateForDateString(startDateString, this.TestingContext.DateToUseForToday).ToString("yyyyMMdd");
            String endDate = SpecflowTableHelper.GetDateForDateString(endDateString, this.TestingContext.DateToUseForToday).ToString("yyyyMMdd");

            await Retry.For(async () =>
            {
                TransactionsByWeekResponse response = await this.TestingContext.DockerHelper.EstateReportingClient
                                                                .GetTransactionsForMerchantByWeek(token,
                                                                                                estateDetails.EstateId,
                                                                                                merchantId,
                                                                                                startDate,
                                                                                                endDate,
                                                                                                CancellationToken.None).ConfigureAwait(false);

                response.ShouldNotBeNull();
                response.TransactionWeekResponses.ShouldNotBeNull();
                response.TransactionWeekResponses.ShouldNotBeEmpty();

                foreach (TableRow tableRow in table.Rows)
                {
                    Int32 weekNumber = SpecflowTableHelper.GetIntValue(tableRow, "WeekNumber");
                    Int32 year = SpecflowTableHelper.GetIntValue(tableRow, "Year");
                    Int32 numberOfTransactions = SpecflowTableHelper.GetIntValue(tableRow, "NumberOfTransactions");
                    Decimal valueOfTransactions = SpecflowTableHelper.GetDecimalValue(tableRow, "ValueOfTransactions");

                    TransactionWeekResponse transactionWeekResponse = response.TransactionWeekResponses.SingleOrDefault(t => t.WeekNumber == weekNumber && t.Year == year);

                    transactionWeekResponse.ShouldNotBeNull();
                    transactionWeekResponse.NumberOfTransactions.ShouldBe(numberOfTransactions);
                    transactionWeekResponse.ValueOfTransactions.ShouldBe(valueOfTransactions);
                }
            });
        }

        [When(@"I get the Estate Transactions By Month Report for Estate '(.*)' with the Start Date '(.*)' and the End Date '(.*)' the following data is returned")]
        public async Task WhenIGetTheEstateTransactionsByMonthReportForEstateWithTheStartDateAndTheEndDateTheFollowingDataIsReturned(string estateName, string startDateString, string endDateString, Table table)
        {
            EstateDetails estateDetails = this.TestingContext.GetEstateDetails(estateName);
            String token = this.TestingContext.AccessToken;
            if (String.IsNullOrEmpty(estateDetails.AccessToken) == false)
            {
                token = estateDetails.AccessToken;
            }

            String startDate = SpecflowTableHelper.GetDateForDateString(startDateString, this.TestingContext.DateToUseForToday).ToString("yyyyMMdd");
            String endDate = SpecflowTableHelper.GetDateForDateString(endDateString, this.TestingContext.DateToUseForToday).ToString("yyyyMMdd");

            await Retry.For(async () =>
            {
                TransactionsByMonthResponse response = await this.TestingContext.DockerHelper.EstateReportingClient
                                                                .GetTransactionsForEstateByMonth(token,
                                                                                                estateDetails.EstateId,
                                                                                                startDate,
                                                                                                endDate,
                                                                                                CancellationToken.None).ConfigureAwait(false);

                response.ShouldNotBeNull();
                response.TransactionMonthResponses.ShouldNotBeNull();
                response.TransactionMonthResponses.ShouldNotBeEmpty();

                foreach (TableRow tableRow in table.Rows)
                {
                    Int32 monthNumber = SpecflowTableHelper.GetIntValue(tableRow, "MonthNumber");
                    Int32 year = SpecflowTableHelper.GetIntValue(tableRow, "Year");
                    Int32 numberOfTransactions = SpecflowTableHelper.GetIntValue(tableRow, "NumberOfTransactions");
                    Decimal valueOfTransactions = SpecflowTableHelper.GetDecimalValue(tableRow, "ValueOfTransactions");

                    TransactionMonthResponse transactionMonthResponse = response.TransactionMonthResponses.SingleOrDefault(t => t.MonthNumber== monthNumber && t.Year == year);

                    transactionMonthResponse.ShouldNotBeNull();
                    transactionMonthResponse.NumberOfTransactions.ShouldBe(numberOfTransactions);
                    transactionMonthResponse.ValueOfTransactions.ShouldBe(valueOfTransactions);
                }
            });
        }

        [When(@"I get the Merchant Transactions By Month Report for Estate '(.*)' and Merchant '(.*)' with the Start Date '(.*)' and the End Date '(.*)' the following data is returned")]
        public async Task WhenIGetTheMerchantTransactionsByMonthReportForEstateAndMerchantWithTheStartDateAndTheEndDateTheFollowingDataIsReturned(string estateName, string merchantName, string startDateString, string endDateString, Table table)
        {
            EstateDetails estateDetails = this.TestingContext.GetEstateDetails(estateName);
            Guid merchantId = estateDetails.GetMerchantId(merchantName);
            String token = this.TestingContext.AccessToken;
            if (String.IsNullOrEmpty(estateDetails.AccessToken) == false)
            {
                token = estateDetails.AccessToken;
            }

            String startDate = SpecflowTableHelper.GetDateForDateString(startDateString, this.TestingContext.DateToUseForToday).ToString("yyyyMMdd");
            String endDate = SpecflowTableHelper.GetDateForDateString(endDateString, this.TestingContext.DateToUseForToday).ToString("yyyyMMdd");

            await Retry.For(async () =>
            {
                TransactionsByMonthResponse response = await this.TestingContext.DockerHelper.EstateReportingClient
                                                                .GetTransactionsForMerchantByMonth(token,
                                                                                                estateDetails.EstateId,
                                                                                                merchantId,
                                                                                                startDate,
                                                                                                endDate,
                                                                                                CancellationToken.None).ConfigureAwait(false);

                response.ShouldNotBeNull();
                response.TransactionMonthResponses.ShouldNotBeNull();
                response.TransactionMonthResponses.ShouldNotBeEmpty();

                foreach (TableRow tableRow in table.Rows)
                {
                    Int32 monthNumber = SpecflowTableHelper.GetIntValue(tableRow, "MonthNumber");
                    Int32 year = SpecflowTableHelper.GetIntValue(tableRow, "Year");
                    Int32 numberOfTransactions = SpecflowTableHelper.GetIntValue(tableRow, "NumberOfTransactions");
                    Decimal valueOfTransactions = SpecflowTableHelper.GetDecimalValue(tableRow, "ValueOfTransactions");

                    TransactionMonthResponse transactionMonthResponse = response.TransactionMonthResponses.SingleOrDefault(t => t.MonthNumber == monthNumber && t.Year == year);

                    transactionMonthResponse.ShouldNotBeNull();
                    transactionMonthResponse.NumberOfTransactions.ShouldBe(numberOfTransactions);
                    transactionMonthResponse.ValueOfTransactions.ShouldBe(valueOfTransactions);
                }
            });
        }



    }
}
