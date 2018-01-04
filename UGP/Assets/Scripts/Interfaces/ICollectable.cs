using System.Collections.Generic;
namespace Trent
{
    public interface ICollectable
    {
        Item ItemGivenOnPickup(ICollector c);
        List<Item> ItemsGivenOnPickup(ICollector c);
    }
}
