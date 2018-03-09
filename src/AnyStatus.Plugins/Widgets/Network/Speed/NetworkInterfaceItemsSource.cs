using System.Linq;
using System.Net.NetworkInformation;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace AnyStatus
{
    public class NetworkInterfaceItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var items = new ItemCollection();

            var interfaces = from @interface in NetworkInterface.GetAllNetworkInterfaces()
                             select new Xceed.Wpf.Toolkit.PropertyGrid.Attributes.Item()
                             {
                                 DisplayName = @interface.Name,
                                 Value = @interface.Id
                             };

            items.AddRange(interfaces);

            return items;
        }
    }
}