using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.ServiceProcess;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;

namespace AnyStatus
{
    public class WindowsServiceNameEditor : ITypeEditor
    {
        [ExcludeFromCodeCoverage]
        public FrameworkElement ResolveEditor(PropertyItem propertyItem)
        {
            return CreateElement(propertyItem, propertyItem.IsReadOnly ? BindingMode.OneWay : BindingMode.TwoWay);
        }

        public static FrameworkElement CreateElement(object bindingSource, BindingMode bindingMode)
        {
            var services = ServiceController.GetServices()
                .OrderBy(s => s.DisplayName)
                .Select(service => new KeyValuePair<string, string>(service.DisplayName, service.ServiceName))
                .ToList();

            var comboBox = new ComboBox
            {
                ItemsSource = services,
                DisplayMemberPath = "Key",
                SelectedValuePath = "Value",
            };

            var binding = new Binding("Value")
            {
                Source = bindingSource,
                ValidatesOnExceptions = true,
                ValidatesOnDataErrors = true,
                Mode = bindingMode
            };

            BindingOperations.SetBinding(comboBox, Selector.SelectedValueProperty, binding);

            return comboBox;
        }
    }
}
