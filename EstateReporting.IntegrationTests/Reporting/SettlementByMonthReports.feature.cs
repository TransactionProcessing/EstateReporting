﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (https://www.specflow.org/).
//      SpecFlow Version:3.5.0.0
//      SpecFlow Generator Version:3.5.0.0
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace EstateReporting.IntegrationTests.Reporting
{
    using TechTalk.SpecFlow;
    using System;
    using System.Linq;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "3.5.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [Xunit.TraitAttribute("Category", "base")]
    [Xunit.TraitAttribute("Category", "shared")]
    public partial class SettlementByMonthReportsFeature : object, Xunit.IClassFixture<SettlementByMonthReportsFeature.FixtureData>, System.IDisposable
    {
        
        private static TechTalk.SpecFlow.ITestRunner testRunner;
        
        private string[] _featureTags = new string[] {
                "base",
                "shared"};
        
        private Xunit.Abstractions.ITestOutputHelper _testOutputHelper;
        
#line 1 "SettlementByMonthReports.feature"
#line hidden
        
        public SettlementByMonthReportsFeature(SettlementByMonthReportsFeature.FixtureData fixtureData, EstateReporting_IntegrationTests_XUnitAssemblyFixture assemblyFixture, Xunit.Abstractions.ITestOutputHelper testOutputHelper)
        {
            this._testOutputHelper = testOutputHelper;
            this.TestInitialize();
        }
        
        public static void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Reporting", "SettlementByMonthReports", null, ProgrammingLanguage.CSharp, new string[] {
                        "base",
                        "shared"});
            testRunner.OnFeatureStart(featureInfo);
        }
        
        public static void FeatureTearDown()
        {
            testRunner.OnFeatureEnd();
            testRunner = null;
        }
        
        public virtual void TestInitialize()
        {
        }
        
        public virtual void TestTearDown()
        {
            testRunner.OnScenarioEnd();
        }
        
        public virtual void ScenarioInitialize(TechTalk.SpecFlow.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioInitialize(scenarioInfo);
            testRunner.ScenarioContext.ScenarioContainer.RegisterInstanceAs<Xunit.Abstractions.ITestOutputHelper>(_testOutputHelper);
        }
        
        public virtual void ScenarioStart()
        {
            testRunner.OnScenarioStart();
        }
        
        public virtual void ScenarioCleanup()
        {
            testRunner.CollectScenarioErrors();
        }
        
        public virtual void FeatureBackground()
        {
#line 4
#line hidden
            TechTalk.SpecFlow.Table table64 = new TechTalk.SpecFlow.Table(new string[] {
                        "Name",
                        "DisplayName",
                        "Description"});
            table64.AddRow(new string[] {
                        "estateManagement",
                        "Estate Managememt REST Scope",
                        "A scope for Estate Managememt REST"});
            table64.AddRow(new string[] {
                        "transactionProcessor",
                        "Transaction Processor REST  Scope",
                        "A scope for Transaction Processor REST"});
#line 6
 testRunner.Given("I create the following api scopes", ((string)(null)), table64, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table65 = new TechTalk.SpecFlow.Table(new string[] {
                        "ResourceName",
                        "DisplayName",
                        "Secret",
                        "Scopes",
                        "UserClaims"});
            table65.AddRow(new string[] {
                        "estateManagement",
                        "Estate Managememt REST",
                        "Secret1",
                        "estateManagement",
                        "MerchantId, EstateId, role"});
            table65.AddRow(new string[] {
                        "transactionProcessor",
                        "Transaction Processor REST",
                        "Secret1",
                        "transactionProcessor",
                        ""});
#line 11
 testRunner.Given("the following api resources exist", ((string)(null)), table65, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table66 = new TechTalk.SpecFlow.Table(new string[] {
                        "ClientId",
                        "ClientName",
                        "Secret",
                        "AllowedScopes",
                        "AllowedGrantTypes"});
            table66.AddRow(new string[] {
                        "serviceClient",
                        "Service Client",
                        "Secret1",
                        "estateManagement,transactionProcessor",
                        "client_credentials"});
#line 16
 testRunner.Given("the following clients exist", ((string)(null)), table66, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table67 = new TechTalk.SpecFlow.Table(new string[] {
                        "ClientId"});
            table67.AddRow(new string[] {
                        "serviceClient"});
#line 20
 testRunner.Given("I have a token to access the estate management and transaction processor resource" +
                    "s", ((string)(null)), table67, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table68 = new TechTalk.SpecFlow.Table(new string[] {
                        "EstateName"});
            table68.AddRow(new string[] {
                        "Test Estate1"});
#line 24
 testRunner.Given("I have created the following estates", ((string)(null)), table68, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table69 = new TechTalk.SpecFlow.Table(new string[] {
                        "EstateName",
                        "OperatorName",
                        "RequireCustomMerchantNumber",
                        "RequireCustomTerminalNumber"});
            table69.AddRow(new string[] {
                        "Test Estate1",
                        "Safaricom",
                        "True",
                        "True"});
#line 28
 testRunner.Given("I have created the following operators", ((string)(null)), table69, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table70 = new TechTalk.SpecFlow.Table(new string[] {
                        "EstateName",
                        "OperatorName",
                        "ContractDescription"});
            table70.AddRow(new string[] {
                        "Test Estate1",
                        "Safaricom",
                        "Safaricom Contract"});
#line 32
 testRunner.Given("I create a contract with the following values", ((string)(null)), table70, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table71 = new TechTalk.SpecFlow.Table(new string[] {
                        "EstateName",
                        "OperatorName",
                        "ContractDescription",
                        "ProductName",
                        "DisplayText",
                        "Value"});
            table71.AddRow(new string[] {
                        "Test Estate1",
                        "Safaricom",
                        "Safaricom Contract",
                        "Variable Topup",
                        "Custom",
                        ""});
#line 36
 testRunner.When("I create the following Products", ((string)(null)), table71, "When ");
#line hidden
            TechTalk.SpecFlow.Table table72 = new TechTalk.SpecFlow.Table(new string[] {
                        "EstateName",
                        "OperatorName",
                        "ContractDescription",
                        "ProductName",
                        "CalculationType",
                        "FeeDescription",
                        "Value"});
            table72.AddRow(new string[] {
                        "Test Estate1",
                        "Safaricom",
                        "Safaricom Contract",
                        "Variable Topup",
                        "Percentage",
                        "Merchant Commission",
                        "0.50"});
#line 40
 testRunner.When("I add the following Transaction Fees", ((string)(null)), table72, "When ");
#line hidden
            TechTalk.SpecFlow.Table table73 = new TechTalk.SpecFlow.Table(new string[] {
                        "MerchantName",
                        "AddressLine1",
                        "Town",
                        "Region",
                        "Country",
                        "ContactName",
                        "EmailAddress",
                        "EstateName",
                        "SettlementSchedule"});
            table73.AddRow(new string[] {
                        "Test Merchant 1",
                        "Address Line 1",
                        "TestTown",
                        "Test Region",
                        "United Kingdom",
                        "Test Contact 1",
                        "testcontact1@merchant1.co.uk",
                        "Test Estate1",
                        "Weekly"});
            table73.AddRow(new string[] {
                        "Test Merchant 2",
                        "Address Line 1",
                        "TestTown",
                        "Test Region",
                        "United Kingdom",
                        "Test Contact 2",
                        "testcontact2@merchant2.co.uk",
                        "Test Estate1",
                        "Monthly"});
#line 44
 testRunner.Given("I create the following merchants", ((string)(null)), table73, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table74 = new TechTalk.SpecFlow.Table(new string[] {
                        "OperatorName",
                        "MerchantName",
                        "MerchantNumber",
                        "TerminalNumber",
                        "EstateName"});
            table74.AddRow(new string[] {
                        "Safaricom",
                        "Test Merchant 1",
                        "00000001",
                        "10000001",
                        "Test Estate1"});
            table74.AddRow(new string[] {
                        "Safaricom",
                        "Test Merchant 2",
                        "00000002",
                        "10000002",
                        "Test Estate1"});
#line 50
 testRunner.Given("I have assigned the following  operator to the merchants", ((string)(null)), table74, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table75 = new TechTalk.SpecFlow.Table(new string[] {
                        "DeviceIdentifier",
                        "MerchantName",
                        "EstateName"});
            table75.AddRow(new string[] {
                        "123456780",
                        "Test Merchant 1",
                        "Test Estate1"});
            table75.AddRow(new string[] {
                        "123456781",
                        "Test Merchant 2",
                        "Test Estate1"});
#line 55
 testRunner.Given("I have assigned the following devices to the merchants", ((string)(null)), table75, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table76 = new TechTalk.SpecFlow.Table(new string[] {
                        "Reference",
                        "Amount",
                        "DateTime",
                        "MerchantName",
                        "EstateName"});
            table76.AddRow(new string[] {
                        "Deposit1",
                        "50000.00",
                        "Today",
                        "Test Merchant 1",
                        "Test Estate1"});
            table76.AddRow(new string[] {
                        "Deposit1",
                        "50000.00",
                        "Today",
                        "Test Merchant 2",
                        "Test Estate1"});
#line 60
 testRunner.Given("I make the following manual merchant deposits", ((string)(null)), table76, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table77 = new TechTalk.SpecFlow.Table(new string[] {
                        "DateTime",
                        "TransactionNumber",
                        "TransactionType",
                        "MerchantName",
                        "DeviceIdentifier",
                        "EstateName",
                        "OperatorName",
                        "TransactionAmount",
                        "CustomerAccountNumber",
                        "CustomerEmailAddress",
                        "ContractDescription",
                        "ProductName"});
            table77.AddRow(new string[] {
                        "2022-01-06",
                        "1",
                        "Sale",
                        "Test Merchant 1",
                        "123456780",
                        "Test Estate1",
                        "Safaricom",
                        "100.00",
                        "123456789",
                        "",
                        "Safaricom Contract",
                        "Variable Topup"});
            table77.AddRow(new string[] {
                        "2022-01-06",
                        "2",
                        "Sale",
                        "Test Merchant 1",
                        "123456780",
                        "Test Estate1",
                        "Safaricom",
                        "50.00",
                        "123456789",
                        "",
                        "Safaricom Contract",
                        "Variable Topup"});
            table77.AddRow(new string[] {
                        "2022-01-06",
                        "3",
                        "Sale",
                        "Test Merchant 1",
                        "123456780",
                        "Test Estate1",
                        "Safaricom",
                        "25.00",
                        "123456789",
                        "",
                        "Safaricom Contract",
                        "Variable Topup"});
            table77.AddRow(new string[] {
                        "2022-01-06",
                        "1",
                        "Sale",
                        "Test Merchant 2",
                        "123456781",
                        "Test Estate1",
                        "Safaricom",
                        "101.00",
                        "123456789",
                        "",
                        "Safaricom Contract",
                        "Variable Topup"});
            table77.AddRow(new string[] {
                        "2022-01-06",
                        "2",
                        "Sale",
                        "Test Merchant 2",
                        "123456781",
                        "Test Estate1",
                        "Safaricom",
                        "55.00",
                        "123456789",
                        "",
                        "Safaricom Contract",
                        "Variable Topup"});
            table77.AddRow(new string[] {
                        "2022-01-06",
                        "3",
                        "Sale",
                        "Test Merchant 2",
                        "123456781",
                        "Test Estate1",
                        "Safaricom",
                        "27.00",
                        "123456789",
                        "",
                        "Safaricom Contract",
                        "Variable Topup"});
#line 65
 testRunner.When("I perform the following transactions", ((string)(null)), table77, "When ");
#line hidden
            TechTalk.SpecFlow.Table table78 = new TechTalk.SpecFlow.Table(new string[] {
                        "EstateName",
                        "MerchantName",
                        "TransactionNumber",
                        "ResponseCode",
                        "ResponseMessage"});
            table78.AddRow(new string[] {
                        "Test Estate1",
                        "Test Merchant 1",
                        "1",
                        "0000",
                        "SUCCESS"});
            table78.AddRow(new string[] {
                        "Test Estate1",
                        "Test Merchant 1",
                        "2",
                        "0000",
                        "SUCCESS"});
            table78.AddRow(new string[] {
                        "Test Estate1",
                        "Test Merchant 1",
                        "3",
                        "0000",
                        "SUCCESS"});
            table78.AddRow(new string[] {
                        "Test Estate1",
                        "Test Merchant 2",
                        "1",
                        "0000",
                        "SUCCESS"});
            table78.AddRow(new string[] {
                        "Test Estate1",
                        "Test Merchant 2",
                        "2",
                        "0000",
                        "SUCCESS"});
            table78.AddRow(new string[] {
                        "Test Estate1",
                        "Test Merchant 2",
                        "3",
                        "0000",
                        "SUCCESS"});
#line 75
 testRunner.Then("transaction response should contain the following information", ((string)(null)), table78, "Then ");
#line hidden
            TechTalk.SpecFlow.Table table79 = new TechTalk.SpecFlow.Table(new string[] {
                        "SettlementDate",
                        "EstateName",
                        "NumberOfFees"});
            table79.AddRow(new string[] {
                        "2022-01-13",
                        "Test Estate1",
                        "3"});
            table79.AddRow(new string[] {
                        "2022-02-06",
                        "Test Estate1",
                        "3"});
#line 85
 testRunner.When("I get the pending settlements the following information should be returned", ((string)(null)), table79, "When ");
#line hidden
#line 90
 testRunner.When("I process the settlement for \'2022-01-13\' on Estate \'Test Estate1\' then 3 fees ar" +
                    "e marked as settled and the settlement is completed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 91
 testRunner.When("I process the settlement for \'2022-02-06\' on Estate \'Test Estate1\' then 3 fees ar" +
                    "e marked as settled and the settlement is completed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
        }
        
        void System.IDisposable.Dispose()
        {
            this.TestTearDown();
        }
        
        [Xunit.SkippableFactAttribute(DisplayName="Settlement By Month")]
        [Xunit.TraitAttribute("FeatureTitle", "SettlementByMonthReports")]
        [Xunit.TraitAttribute("Description", "Settlement By Month")]
        public virtual void SettlementByMonth()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Settlement By Month", null, tagsOfScenario, argumentsOfScenario);
#line 93
this.ScenarioInitialize(scenarioInfo);
#line hidden
            bool isScenarioIgnored = default(bool);
            bool isFeatureIgnored = default(bool);
            if ((tagsOfScenario != null))
            {
                isScenarioIgnored = tagsOfScenario.Where(__entry => __entry != null).Where(__entry => String.Equals(__entry, "ignore", StringComparison.CurrentCultureIgnoreCase)).Any();
            }
            if ((this._featureTags != null))
            {
                isFeatureIgnored = this._featureTags.Where(__entry => __entry != null).Where(__entry => String.Equals(__entry, "ignore", StringComparison.CurrentCultureIgnoreCase)).Any();
            }
            if ((isScenarioIgnored || isFeatureIgnored))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 4
this.FeatureBackground();
#line hidden
                TechTalk.SpecFlow.Table table80 = new TechTalk.SpecFlow.Table(new string[] {
                            "SettlementMonth",
                            "SettlementYear",
                            "NumberOfFeesSettled",
                            "ValueOfFeesSettled"});
                table80.AddRow(new string[] {
                            "1",
                            "2022",
                            "3",
                            "0.88"});
                table80.AddRow(new string[] {
                            "2",
                            "2022",
                            "3",
                            "0.93"});
#line 94
 testRunner.When("I get the Estate Settlement By Month Report for Estate \'Test Estate1\' with the St" +
                        "art Date \'2022-01-13\' and the End Date \'2022-02-06\' the following data is return" +
                        "ed", ((string)(null)), table80, "When ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [Xunit.SkippableFactAttribute(DisplayName="Settlement By Month for Merchant")]
        [Xunit.TraitAttribute("FeatureTitle", "SettlementByMonthReports")]
        [Xunit.TraitAttribute("Description", "Settlement By Month for Merchant")]
        public virtual void SettlementByMonthForMerchant()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Settlement By Month for Merchant", null, tagsOfScenario, argumentsOfScenario);
#line 99
this.ScenarioInitialize(scenarioInfo);
#line hidden
            bool isScenarioIgnored = default(bool);
            bool isFeatureIgnored = default(bool);
            if ((tagsOfScenario != null))
            {
                isScenarioIgnored = tagsOfScenario.Where(__entry => __entry != null).Where(__entry => String.Equals(__entry, "ignore", StringComparison.CurrentCultureIgnoreCase)).Any();
            }
            if ((this._featureTags != null))
            {
                isFeatureIgnored = this._featureTags.Where(__entry => __entry != null).Where(__entry => String.Equals(__entry, "ignore", StringComparison.CurrentCultureIgnoreCase)).Any();
            }
            if ((isScenarioIgnored || isFeatureIgnored))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 4
this.FeatureBackground();
#line hidden
                TechTalk.SpecFlow.Table table81 = new TechTalk.SpecFlow.Table(new string[] {
                            "SettlementMonth",
                            "SettlementYear",
                            "NumberOfFeesSettled",
                            "ValueOfFeesSettled"});
                table81.AddRow(new string[] {
                            "1",
                            "2022",
                            "3",
                            "0.88"});
#line 100
 testRunner.When("I get the Estate Settlement By Month Report for Estate \'Test Estate1\' and Merchan" +
                        "t \'Test Merchant 1\' with the Start Date \'2022-01-13\' and the End Date \'2022-02-0" +
                        "6\' the following data is returned", ((string)(null)), table81, "When ");
#line hidden
                TechTalk.SpecFlow.Table table82 = new TechTalk.SpecFlow.Table(new string[] {
                            "SettlementMonth",
                            "SettlementYear",
                            "NumberOfFeesSettled",
                            "ValueOfFeesSettled"});
                table82.AddRow(new string[] {
                            "2",
                            "2022",
                            "3",
                            "0.93"});
#line 103
testRunner.When("I get the Estate Settlement By Month Report for Estate \'Test Estate1\' and Merchan" +
                        "t \'Test Merchant 2\' with the Start Date \'2022-01-13\' and the End Date \'2022-02-0" +
                        "6\' the following data is returned", ((string)(null)), table82, "When ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "3.5.0.0")]
        [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
        public class FixtureData : System.IDisposable
        {
            
            public FixtureData()
            {
                SettlementByMonthReportsFeature.FeatureSetup();
            }
            
            void System.IDisposable.Dispose()
            {
                SettlementByMonthReportsFeature.FeatureTearDown();
            }
        }
    }
}
#pragma warning restore
#endregion
