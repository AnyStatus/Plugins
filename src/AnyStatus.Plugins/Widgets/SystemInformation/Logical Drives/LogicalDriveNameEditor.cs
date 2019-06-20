using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;

namespace AnyStatus
{
    public class LogicalDriveNameEditor : ITypeEditor
    {
        private const string Key = "Key";
        private const string Value = "Value";

        [ExcludeFromCodeCoverage]
        public FrameworkElement ResolveEditor(PropertyItem propertyItem)
        {
            return CreateElement(propertyItem, propertyItem.IsReadOnly ? BindingMode.OneWay : BindingMode.TwoWay);
        }

        public static FrameworkElement CreateElement(object bindingSource, BindingMode bindingMode)
        {
            var drives = Directory.GetLogicalDrives()
                .Select(drive => new KeyValuePair<string, string>(drive, drive))
                .ToList();

            var comboBox = new ComboBox
            {
                ItemsSource = drives,
                DisplayMemberPath = Key,
                SelectedValuePath = Value,
            };

            var binding = new Binding(Value)
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
