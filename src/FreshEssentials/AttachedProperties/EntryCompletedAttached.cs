using System;
using Xamarin.Forms;
using System.Windows.Input;

namespace FreshEssentials
{
    public class EntryCompletedAttached
    {
        public static readonly BindableProperty CommandProperty =
            BindableProperty.CreateAttached(
                propertyName: "Command",
                returnType: typeof(ICommand),
                declaringType: typeof(Entry),
                defaultValue: null,
                defaultBindingMode: BindingMode.OneWay,
                validateValue: null,
                propertyChanged: OnChanged);

        public static readonly BindableProperty CommandParameterProperty =
            BindableProperty.CreateAttached(
                propertyName: "CommandParameter",
                returnType: typeof(object),
                declaringType: typeof(Entry),
                defaultValue: null,
                defaultBindingMode: BindingMode.OneWay,
                validateValue: null);

        public static object GetCommandParameter(BindableObject bindable)
        {
            return (object)bindable.GetValue(CommandProperty);
        }

        public static void SetCommandParameter(BindableObject bindable, object value)
        {
            bindable.SetValue(CommandProperty, value);
        }

        public static ICommand GetCommand(BindableObject bindable)
        {
            return (ICommand)bindable.GetValue(CommandProperty);
        }

        public static void SetCommand(BindableObject bindable, ICommand value)
        {
            bindable.SetValue(CommandProperty, value);
        }

        public static void OnChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = bindable as Entry;
            if (control != null)
                control.Completed += EntryCompleted;
        }

        private static void EntryCompleted(object sender, EventArgs e)
        {
            var control = sender as Entry;

            var command = GetCommand(control);

            if (command != null && command.CanExecute(control.GetValue(CommandParameterProperty)))
                command.Execute(control.GetValue(CommandParameterProperty));
        }
    }
}

