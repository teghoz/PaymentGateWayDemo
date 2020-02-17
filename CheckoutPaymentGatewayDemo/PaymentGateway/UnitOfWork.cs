using PaymentGateway.Model;
using PaymentGatewayDbContext;
using PaymentGateWayModels;
using PaymentGatewayRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentGateway
{
    public class UnitOfWork: IDisposable
    {
        private PaymentGatewayDbContext.PaymentGatewayDbContext context = ContextManager.PaymentGatewayContext();
        private CheckOutRepository<Transactions> transactionRepository;
        private CheckOutRepository<CardDetails> carDetailsRepository;

        public CheckOutRepository<Transactions> TransactionRepository
        {
            get
            {

                if (this.transactionRepository == null)
                {
                    this.transactionRepository = new CheckOutRepository<Transactions>(context);
                }
                return transactionRepository;
            }
        }

        public CheckOutRepository<CardDetails> CarDetailsRepository
        {
            get
            {

                if (this.carDetailsRepository == null)
                {
                    this.carDetailsRepository = new CheckOutRepository<CardDetails>(context);
                }
                return carDetailsRepository;
            }
        }

        public void Save()
        {
            context.SaveChanges();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
