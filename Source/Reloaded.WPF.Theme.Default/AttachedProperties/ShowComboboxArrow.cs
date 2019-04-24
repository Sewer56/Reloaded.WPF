using System.Windows.Controls;
using Reloaded.WPF.MVVM;

namespace Reloaded.WPF.Theme.Default.AttachedProperties
{
    /// <summary>
    /// Dependency property which if set to false, does not show the <see cref="ComboBox"/> arrow.
    /// </summary>
    /// <remarks>This property has no logic. All logic is done in binding.</remarks>
    public class ShowComboboxArrow : AttachedPropertyBase<ShowComboboxArrow, bool>
    {

    }
}
