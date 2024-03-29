﻿namespace EstateReporting.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Database;
    using Database.ViewEntities;
    using Microsoft.EntityFrameworkCore;
    using Models;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="EstateReporting.Repository.IEstateReportingRepositoryForReports" />
    [ExcludeFromCodeCoverage]
    public class EstateReportingRepositoryForReports : IEstateReportingRepositoryForReports
    {
        #region Fields

        /// <summary>
        /// The database context factory
        /// </summary>
        private readonly Shared.EntityFramework.IDbContextFactory<EstateReportingGenericContext> DbContextFactory;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EstateReportingRepository" /> class.
        /// </summary>
        /// <param name="dbContextFactory">The database context factory.</param>
        public EstateReportingRepositoryForReports(Shared.EntityFramework.IDbContextFactory<EstateReportingGenericContext> dbContextFactory) {
            this.DbContextFactory = dbContextFactory;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the settlement.
        /// </summary>
        /// <param name="estateId">The estate identifier.</param>
        /// <param name="merchantId">The merchant identifier.</param>
        /// <param name="settlementId">The settlement identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<SettlementModel> GetSettlement(Guid estateId,
                                                         Guid? merchantId,
                                                         Guid settlementId,
                                                         CancellationToken cancellationToken) {
            EstateReportingGenericContext context = await this.DbContextFactory.GetContext(estateId, cancellationToken);

            IQueryable<SettlementView> query = context.SettlementsView.Where(t => t.EstateId == estateId && t.SettlementId == settlementId).AsQueryable();

            if (merchantId.HasValue) {
                query = query.Where(t => t.MerchantId == merchantId);
            }

            var result = query.AsEnumerable().GroupBy(t => new {
                                                                   t.SettlementId,
                                                                   t.SettlementDate,
                                                                   t.IsCompleted
                                                               }).SingleOrDefault();

            if (result == null)
                return null;

            SettlementModel model = new SettlementModel {
                                                            SettlementDate = result.Key.SettlementDate,
                                                            SettlementId = result.Key.SettlementId,
                                                            NumberOfFeesSettled = result.Count(),
                                                            ValueOfFeesSettled = result.Sum(x => x.CalculatedValue),
                                                            IsCompleted = result.Key.IsCompleted
                                                        };

            result.ToList().ForEach(f => model.SettlementFees.Add(new SettlementFeeModel {
                                                                                             SettlementDate = f.SettlementDate,
                                                                                             SettlementId = f.SettlementId,
                                                                                             CalculatedValue = f.CalculatedValue,
                                                                                             MerchantId = f.MerchantId,
                                                                                             MerchantName = f.MerchantName,
                                                                                             FeeDescription = f.FeeDescription,
                                                                                             IsSettled = f.IsSettled,
                                                                                             TransactionId = f.TransactionId,
                                                                                             OperatorIdentifier = f.OperatorIdentifier
                                                                                         }));

            return model;
        }

        /// <summary>
        /// Gets the settlements.
        /// </summary>
        /// <param name="estateId">The estate identifier.</param>
        /// <param name="merchantId">The merchant identifier.</param>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<List<SettlementModel>> GetSettlements(Guid estateId,
                                                                Guid? merchantId,
                                                                String startDate,
                                                                String endDate,
                                                                CancellationToken cancellationToken) {
            EstateReportingGenericContext context = await this.DbContextFactory.GetContext(estateId, cancellationToken);

            DateTime queryStartDate = DateTime.ParseExact(startDate, "yyyyMMdd", null);
            DateTime queryEndDate = DateTime.ParseExact(endDate, "yyyyMMdd", null);

            IQueryable<SettlementView> query = context.SettlementsView.Where(t => t.EstateId == estateId &&
                                                                                  t.SettlementDate >= queryStartDate.Date && t.SettlementDate <= queryEndDate.Date)
                                                      .AsQueryable();

            if (merchantId.HasValue) {
                query = query.Where(t => t.MerchantId == merchantId);
            }

            List<SettlementModel> result = await query.GroupBy(t => new {
                                                                            t.SettlementId,
                                                                            t.SettlementDate,
                                                                            t.IsCompleted
                                                                        }).Select(t => new SettlementModel {
                                                                                                               SettlementId = t.Key.SettlementId,
                                                                                                               SettlementDate = t.Key.SettlementDate,
                                                                                                               NumberOfFeesSettled = t.Count(),
                                                                                                               ValueOfFeesSettled = t.Sum(x => x.CalculatedValue),
                                                                                                               IsCompleted = t.Key.IsCompleted
                                                                                                           }).OrderByDescending(t => t.SettlementDate)
                                                      .ToListAsync(cancellationToken);

            return result;
        }

        #endregion
    }
}