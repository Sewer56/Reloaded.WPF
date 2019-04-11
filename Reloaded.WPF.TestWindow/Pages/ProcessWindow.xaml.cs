using System;
using System.Windows;
using Reloaded.WPF.TestWindow.Models.ViewModel;
using Reloaded.WPF.Theme.Default;

namespace Reloaded.WPF.TestWindow.Pages
{
    /// <summary>
    /// The main page of the application.
    /// </summary>
    public partial class ProcessWindow : ReloadedPage
    {
        public ProcessWindow()
        {
            InitializeComponent();
            this.DataContext = IoC.Get<Models.ViewModel.ProcessWindow>();
        }

        private async void AnimateInClick(object sender, RoutedEventArgs e)
        {
            await this.AnimateIn();
        }

        private async void AnimateOutClick(object sender, RoutedEventArgs e)
        {
            await this.AnimateOut();
            await this.AnimateIn();
        }

        private void ChangeStateClick(object sender, RoutedEventArgs e)
        {
            var viewModel = (Models.ViewModel.ProcessWindow) this.DataContext;
            viewModel.WindowViewModel.WindowState = NextEnum(viewModel.WindowViewModel.WindowState);
        }

        /// <summary>
        /// Retrieves the next enumerable value, or first if the current value is last.
        /// </summary>
        private T NextEnum<T>(T enumValue) where T : struct
        {
            T[] possibleValues = (T[]) Enum.GetValues(enumValue.GetType());
            int nextEnumIndex = Array.IndexOf(possibleValues, enumValue) + 1;
            return (possibleValues.Length == nextEnumIndex) ? possibleValues[0] 
                                                            : possibleValues[nextEnumIndex];
        }
    }
}
