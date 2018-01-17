using System.Collections.Generic;
namespace UGP
{
    public interface ICollector
    {
        void TakeItem(Item item);
        void TakeItems(List<Item> items);
    }
}