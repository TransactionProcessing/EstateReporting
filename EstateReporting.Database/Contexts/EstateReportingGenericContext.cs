﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstateReporting.Database
{
    using System.IO;
    using System.Reflection;
    using System.Threading;
    using Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Microsoft.Extensions.Configuration;
    using Shared.Logger;
    using ViewEntities;
    using File = Entities.File;

    public abstract class EstateReportingGenericContext : DbContext
    {
        protected readonly String DatabaseEngine;

        protected readonly String ConnectionString;
        
        #region Properties

        /// <summary>
        /// Gets or sets the contract products.
        /// </summary>
        /// <value>
        /// The contract products.
        /// </value>
        public DbSet<ContractProduct> ContractProducts { get; set; }

        public DbSet<Settlement> Settlements { get; set; }

        /// <summary>
        /// Gets or sets the contract product transaction fees.
        /// </summary>
        /// <value>
        /// The contract product transaction fees.
        /// </value>
        public DbSet<ContractProductTransactionFee> ContractProductTransactionFees { get; set; }

        /// <summary>
        /// Gets or sets the contracts.
        /// </summary>
        /// <value>
        /// The contracts.
        /// </value>
        public DbSet<Contract> Contracts { get; set; }

        /// <summary>
        /// Gets or sets the estate operators.
        /// </summary>
        /// <value>
        /// The estate operators.
        /// </value>
        public DbSet<EstateOperator> EstateOperators { get; set; }

        /// <summary>
        /// Gets or sets the estates.
        /// </summary>
        /// <value>
        /// The estates.
        /// </value>
        public DbSet<Estate> Estates { get; set; }

        /// <summary>
        /// Gets or sets the estate security users.
        /// </summary>
        /// <value>
        /// The estate security users.
        /// </value>
        public DbSet<EstateSecurityUser> EstateSecurityUsers { get; set; }

        /// <summary>
        /// Gets or sets the file import log files.
        /// </summary>
        /// <value>
        /// The file import log files.
        /// </value>
        public virtual DbSet<FileImportLogFile> FileImportLogFiles { get; set; }

        /// <summary>
        /// Gets or sets the file import logs.
        /// </summary>
        /// <value>
        /// The file import logs.
        /// </value>
        public virtual DbSet<FileImportLog> FileImportLogs { get; set; }

        /// <summary>
        /// Gets or sets the file import log view.
        /// </summary>
        /// <value>
        /// The file import log view.
        /// </value>
        public virtual DbSet<FileImportLogView> FileImportLogView { get; set; }

        /// <summary>
        /// Gets or sets the file lines.
        /// </summary>
        /// <value>
        /// The file lines.
        /// </value>
        public virtual DbSet<FileLine> FileLines { get; set; }

        /// <summary>
        /// Gets or sets the files.
        /// </summary>
        /// <value>
        /// The files.
        /// </value>
        public virtual DbSet<File> Files { get; set; }

        /// <summary>
        /// Gets or sets the file view.
        /// </summary>
        /// <value>
        /// The file view.
        /// </value>
        public virtual DbSet<FileView> FileView { get; set; }

        /// <summary>
        /// Gets or sets the merchant addresses.
        /// </summary>
        /// <value>
        /// The merchant addresses.
        /// </value>
        public DbSet<MerchantAddress> MerchantAddresses { get; set; }

        /// <summary>
        /// Gets or sets the merchant balance histories.
        /// </summary>
        /// <value>
        /// The merchant balance histories.
        /// </value>
        public virtual DbSet<MerchantBalanceHistory> MerchantBalanceHistories { get; set; }

        /// <summary>
        /// Gets or sets the merchant balance view.
        /// </summary>
        /// <value>
        /// The merchant balance view.
        /// </value>
        public virtual DbSet<MerchantBalanceView> MerchantBalanceView { get; set; }

        /// <summary>
        /// Gets or sets the merchant contacts.
        /// </summary>
        /// <value>
        /// The merchant contacts.
        /// </value>
        public DbSet<MerchantContact> MerchantContacts { get; set; }

        /// <summary>
        /// Gets or sets the merchant devices.
        /// </summary>
        /// <value>
        /// The merchant devices.
        /// </value>
        public DbSet<MerchantDevice> MerchantDevices { get; set; }

        /// <summary>
        /// Gets or sets the merchant operators.
        /// </summary>
        /// <value>
        /// The merchant operators.
        /// </value>
        public DbSet<MerchantOperator> MerchantOperators { get; set; }

        /// <summary>
        /// Gets or sets the estate security users.
        /// </summary>
        /// <value>
        /// The estate security users.
        /// </value>
        public DbSet<Merchant> Merchants { get; set; }

        /// <summary>
        /// Gets or sets the merchant security users.
        /// </summary>
        /// <value>
        /// The merchant security users.
        /// </value>
        public DbSet<MerchantSecurityUser> MerchantSecurityUsers { get; set; }

        /// <summary>
        /// Gets or sets the reconciliations.
        /// </summary>
        /// <value>
        /// The reconciliations.
        /// </value>
        public DbSet<Reconciliation> Reconciliations { get; set; }

        /// <summary>
        /// Gets or sets the transaction fees.
        /// </summary>
        /// <value>
        /// The transaction fees.
        /// </value>
        public DbSet<TransactionFee> TransactionFees { get; set; }

        /// <summary>
        /// Gets or sets the transactions.
        /// </summary>
        /// <value>
        /// The transactions.
        /// </value>
        public DbSet<Transaction> Transactions { get; set; }

        /// <summary>
        /// Gets or sets the transaction additional request data.
        /// </summary>
        /// <value>
        /// The transaction additional request data.
        /// </value>
        public DbSet<TransactionAdditionalRequestData> TransactionsAdditionalRequestData { get; set; }

        /// <summary>
        /// Gets or sets the transaction additional response data.
        /// </summary>
        /// <value>
        /// The transaction additional response data.
        /// </value>
        public DbSet<TransactionAdditionalResponseData> TransactionsAdditionalResponseData { get; set; }

        /// <summary>
        /// Gets or sets the transactions view.
        /// </summary>
        /// <value>
        /// The transactions view.
        /// </value>
        public virtual DbSet<TransactionsView> TransactionsView { get; set; }

        public virtual DbSet<SettlementView> SettlementsView { get; set; }

        /// <summary>
        /// Gets or sets the vouchers.
        /// </summary>
        /// <value>
        /// The vouchers.
        /// </value>
        public DbSet<Voucher> Vouchers { get; set; }

        public DbSet<MerchantSettlementFee> MerchantSettlementFees { get; set; }
        #endregion

        protected EstateReportingGenericContext(String databaseEngine, String connectionString)
        {
            this.DatabaseEngine = databaseEngine;
            this.ConnectionString = connectionString;
        }

        public EstateReportingGenericContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Estate>().HasKey(t => new
            {
                t.EstateId
            });

            modelBuilder.Entity<EstateSecurityUser>().HasKey(t => new
            {
                t.SecurityUserId,
                t.EstateId
            });

            modelBuilder.Entity<Merchant>().HasKey(t => new
            {
                t.EstateId,
                t.MerchantId
            });

            modelBuilder.Entity<MerchantAddress>().HasKey(t => new
            {
                t.EstateId,
                t.MerchantId,
                t.AddressId
            });

            modelBuilder.Entity<MerchantContact>().HasKey(t => new
            {
                t.EstateId,
                t.MerchantId,
                t.ContactId
            });

            modelBuilder.Entity<MerchantDevice>().HasKey(t => new
            {
                t.EstateId,
                t.MerchantId,
                t.DeviceId
            });

            modelBuilder.Entity<MerchantSecurityUser>().HasKey(t => new
            {
                t.EstateId,
                t.MerchantId,
                t.SecurityUserId
            });

            modelBuilder.Entity<EstateOperator>().HasKey(t => new
            {
                t.EstateId,
                t.OperatorId
            });

            modelBuilder.Entity<MerchantOperator>().HasKey(t => new
            {
                t.EstateId,
                t.MerchantId,
                t.OperatorId
            });

            modelBuilder.Entity<Transaction>().HasKey(t => new
            {
                t.EstateId,
                t.MerchantId,
                t.TransactionId
            });

            modelBuilder.Entity<TransactionFee>().HasKey(t => new
            {
                t.TransactionId,
                t.FeeId
            });

            modelBuilder.Entity<Contract>().HasKey(c => new
            {
                c.EstateId,
                c.OperatorId,
                c.ContractId
            });

            modelBuilder.Entity<ContractProduct>().HasKey(c => new
            {
                c.EstateId,
                c.ContractId,
                c.ProductId
            });

            modelBuilder.Entity<ContractProductTransactionFee>().HasKey(c => new
            {
                c.EstateId,
                c.ContractId,
                c.ProductId,
                c.TransactionFeeId
            });
            modelBuilder.Entity<ContractProductTransactionFee>().Property(p => p.Value).DecimalPrecision(18, 4);

            modelBuilder.Entity<MerchantBalanceHistory>().Property(p => p.AvailableBalance).DecimalPrecision(18, 4);
            modelBuilder.Entity<MerchantBalanceHistory>().Property(p => p.Balance).DecimalPrecision(18, 4);
            modelBuilder.Entity<MerchantBalanceHistory>().Property(p => p.ChangeAmount).DecimalPrecision(18, 4);

            modelBuilder.Entity<TransactionAdditionalRequestData>().HasKey(t => new
            {
                t.EstateId,
                t.MerchantId,
                t.TransactionId
            });

            modelBuilder.Entity<TransactionAdditionalRequestData>().HasKey(t => new
            {
                t.EstateId,
                t.MerchantId,
                t.TransactionId
            });

            modelBuilder.Entity<FileImportLog>().HasKey(f => new
            {
                f.EstateId,
                f.FileImportLogId
            });

            modelBuilder.Entity<FileImportLogFile>().HasKey(f => new
            {
                f.EstateId,
                f.FileImportLogId,
                f.FileId
            });

            modelBuilder.Entity<File>().HasKey(f => new
            {
                f.EstateId,
                f.FileImportLogId,
                f.FileId
            });

            modelBuilder.Entity<FileLine>().HasKey(f => new
            {
                f.EstateId,
                f.FileId,
                f.LineNumber
            });

            modelBuilder.Entity<Settlement>().HasKey(s => new
            {
                s.EstateId,
                s.SettlementId
            });

            modelBuilder.Entity<MerchantSettlementFee>().HasKey(s => new
            {
                s.EstateId,
                s.SettlementId,
                s.TransactionId,
                s.FeeId
            });

            modelBuilder.Entity<TransactionsView>().HasNoKey().ToView("uvwTransactions");
            modelBuilder.Entity<MerchantBalanceView>().HasNoKey().ToView("uvwMerchantBalance");
            modelBuilder.Entity<FileImportLogView>().HasNoKey().ToView("uvwFileImportLog");
            modelBuilder.Entity<FileView>().HasNoKey().ToView("uvwFile");
            modelBuilder.Entity<SettlementView>().HasNoKey().ToView("uvwSettlements");

            base.OnModelCreating(modelBuilder);
        }

        public virtual async Task MigrateAsync(CancellationToken cancellationToken)
        {
            if (this.Database.IsSqlServer() || this.Database.IsMySql())
            {
                await this.Database.MigrateAsync(cancellationToken);
                await this.SetIgnoreDuplicates(cancellationToken);
                await this.CreateViews(cancellationToken);
            }
        }

        /// <summary>
        /// Creates the views.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task CreateViews(CancellationToken cancellationToken)
        {


            String executingAssemblyLocation = Assembly.GetExecutingAssembly().Location;
            String executingAssemblyFolder = Path.GetDirectoryName(executingAssemblyLocation);

            String scriptsFolder = $@"{executingAssemblyFolder}/Views/{this.DatabaseEngine}";
            
            String[] directiories = Directory.GetDirectories(scriptsFolder);
            directiories = directiories.OrderBy(d => d).ToArray();

            foreach (String directiory in directiories)
            {
                String[] sqlFiles = Directory.GetFiles(directiory, "*View.sql");
                foreach (String sqlFile in sqlFiles.OrderBy(x => x))
                {
                    Logger.LogDebug($"About to create View [{sqlFile}]");
                    String sql = System.IO.File.ReadAllText(sqlFile);

                    // Check here is we need to replace a Database Name
                    if (sql.Contains("{DatabaseName}"))
                    {
                        sql = sql.Replace("{DatabaseName}", this.Database.GetDbConnection().Database);
                    }

                    // Create the new view using the original sql from file
                    await this.Database.ExecuteSqlRawAsync(sql, cancellationToken);

                    Logger.LogDebug($"Created View [{sqlFile}] successfully.");
                }
            }
        }

        public static Boolean IsDuplicateInsertsIgnored(String tableName) => TablesToIgnoreDuplicates.Contains(tableName.Trim(), StringComparer.InvariantCultureIgnoreCase);
        

        protected static List<String> TablesToIgnoreDuplicates = new List<String>();

        /// <summary>
        /// Sets the ignore duplicates.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        protected virtual async Task SetIgnoreDuplicates(CancellationToken cancellationToken)
        {
            TablesToIgnoreDuplicates = new List<String>
                                       {
                                           nameof(Estate),
                                           nameof(EstateSecurityUser),
                                           nameof(EstateOperator),
                                           nameof(Merchant),
                                           nameof(MerchantAddress),
                                           nameof(MerchantContact),
                                           nameof(MerchantDevice),
                                           nameof(MerchantSecurityUser),
                                           nameof(MerchantOperator),
                                           nameof(Transaction),
                                           nameof(TransactionFee),
                                           nameof(TransactionAdditionalRequestData),
                                           nameof(TransactionAdditionalResponseData),
                                           nameof(Contract),
                                           nameof(ContractProduct),
                                           nameof(ContractProductTransactionFee),
                                           nameof(Reconciliation),
                                           nameof(Voucher),
                                           nameof(FileImportLog),
                                           nameof(FileImportLogFile),
                                           nameof(File),
                                           nameof(FileLine),
                                           nameof(Settlement),
                                           nameof(MerchantSettlementFees)
                                       };
        }
    }

    public static class Extensions
    {
        #region Methods

        /// <summary>
        /// Decimals the precision.
        /// </summary>
        /// <param name="propertyBuilder">The property builder.</param>
        /// <param name="precision">The precision.</param>
        /// <param name="scale">The scale.</param>
        /// <returns></returns>
        public static PropertyBuilder DecimalPrecision(this PropertyBuilder propertyBuilder,
                                                       Int32 precision,
                                                       Int32 scale)
        {
            return propertyBuilder.HasColumnType($"decimal({precision},{scale})");
        }

        #endregion
    }
}
