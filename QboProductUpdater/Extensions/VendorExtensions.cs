using QuickBooksSharp.Entities;

namespace QboProductUpdater.Extensions
{
    public static class VendorExtensions
    {
        public static Vendor makeCompanyNameEqualVendorNameIfNullOrWhitespace(this Vendor vendor)
        {
            if (string.IsNullOrWhiteSpace(vendor.CompanyName))
                vendor.CompanyName = vendor.DisplayName;
            return vendor;
        }
    }
}