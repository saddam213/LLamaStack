using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;

namespace LLamaStack.WPF.Services
{

    public interface IDialogService
    {
        T GetDialog<T>() where T : Window;
        T GetDialog<T>(Window owner) where T : Window;
    }

    public class DialogService : IDialogService
    {

        public T GetDialog<T>() where T : Window
        {
            return Resolve<T>(Application.Current.MainWindow);
        }

        public T GetDialog<T>(Window owner) where T : Window
        {
            return Resolve<T>(owner);
        }

        private T Resolve<T>(Window owner) where T : Window
        {
            var container = ((App)Application.Current).ServiceProvider;
            var dialog = container.GetService<T>();
            dialog.Owner = owner;
            return dialog;
        }
    }

}
