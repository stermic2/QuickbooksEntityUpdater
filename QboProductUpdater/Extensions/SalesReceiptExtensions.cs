using QuickBooksSharp.Entities;

namespace QboProductUpdater.Extensions
{
    public static class SalesReceiptExtensions
    {

        public static SalesReceipt changeAccountReference(this SalesReceipt salesReceipt)
        {
            if (salesReceipt.DepositToAccountRef?.value == "319")
            {
                salesReceipt.DepositToAccountRef.value = "33";
                salesReceipt.DepositToAccountRef.name = null;
            }
            return salesReceipt;
        }
        
    }
}