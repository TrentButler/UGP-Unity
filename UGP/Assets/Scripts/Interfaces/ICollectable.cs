using System.Collections.Generic;
namespace UGP
{
    public interface ICollectable
    {
        Item ItemGivenOnPickup(ICollector c);
        List<Item> ItemsGivenOnPickup(ICollector c);
    }
}
