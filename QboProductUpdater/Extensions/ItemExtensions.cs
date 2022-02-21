using QuickBooksSharp.Entities;

namespace QboProductUpdater.Extensions
{
    public static class ItemExtensions
    {
        public static Item makeDescriptionSameAsName(this Item item)
        {
            item.PurchaseDesc = item.Name;
            return item;
        }
        
        public static Item renameTo(this Item item, string newName)
        {
            item.Name = newName;
            return item;
        }
    }
}