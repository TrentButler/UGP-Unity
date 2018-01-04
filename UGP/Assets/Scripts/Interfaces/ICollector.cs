using System.Collections.Generic;
namespace Trent
{
    public interface ICollector
    {
        void TakeItem(Item item);
        void TakeItems(List<Item> items);
    }
}