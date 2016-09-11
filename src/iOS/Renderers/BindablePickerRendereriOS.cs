using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using CoreGraphics;
using FreshEssentials;
using UIKit;
using RectangleF = CoreGraphics.CGRect;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(BindablePicker), typeof(FreshEssentials.iOS.BindablePickerRendereriOS), UIUserInterfaceIdiom.Pad)]
namespace FreshEssentials.iOS
{
    public class BindablePickerRendereriOS : ViewRenderer<BindablePicker, UIButton>
    {
        class PickerSource : UIPickerViewModel
        {
            readonly BindablePickerRendereriOS _renderer;

            public PickerSource(BindablePickerRendereriOS model)
            {
                _renderer = model;
            }

            public int SelectedIndex { get; internal set; }

            public string SelectedItem { get; internal set; }

            public override nint GetComponentCount(UIPickerView picker)
            {
                return 1;
            }

            public override nint GetRowsInComponent(UIPickerView pickerView, nint component)
            {
                return _renderer.Element.Items != null ? _renderer.Element.Items.Count : 0;
            }

            public override string GetTitle(UIPickerView picker, nint row, nint component)
            {
                return _renderer.Element.Items[(int)row];
            }

            public override void Selected(UIPickerView picker, nint row, nint component)
            {
                if (_renderer.Element.Items.Count == 0)
                {
                    SelectedItem = null;
                    SelectedIndex = -1;
                }
                else
                {
                    SelectedItem = _renderer.Element.Items[(int)row];
                    SelectedIndex = (int)row;
                }
            }
        }

        /// <summary>
        /// The _picker
        /// </summary>
        private UIPickerView _picker;

        /// <summary>
        /// The _pop over
        /// </summary>
        private UIPopoverController _popOver;

        /// <summary>
        /// Called when [element changed].
        /// </summary>
        /// <param name="e">The e.</param>
        protected override void OnElementChanged(ElementChangedEventArgs<BindablePicker> e)
        {



            if (e.OldElement != null)
                ((ObservableCollection<string>)e.OldElement.Items).CollectionChanged -= RowsCollectionChanged;

            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    var button = new UIButton(UIButtonType.RoundedRect);
                    button.TouchUpInside += OnStarted;

                    _picker = new UIPickerView();

                    SetNativeControl(button);
                }

                Control.SetTitle(Element.Title, UIControlState.Normal);
                _picker.Model = new PickerSource(this);

                UpdatePicker();

                ((ObservableCollection<string>)e.NewElement.Items).CollectionChanged += RowsCollectionChanged;
            }

            base.OnElementChanged(e);
        }

        /// <summary>
        /// Handles the <see cref="E:ElementPropertyChanged" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == Picker.TitleProperty.PropertyName)
            {
                Control.SetTitle(Element.Title, UIControlState.Normal);
            }
            else if (e.PropertyName == Picker.SelectedIndexProperty.PropertyName)
            {
                UpdatePicker();
            }
        }

        /// <summary>
        /// Handles the <see cref="E:Started" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="eventArgs">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void OnStarted(object sender, EventArgs eventArgs)
        {
                    
            var button = UIButton.FromType(UIButtonType.System);
            button.SetTitle("OK", UIControlState.Normal);
            button.Frame = new CGRect(0, 250, 320, 50);
            button.TouchUpInside += (s, e) =>
            {
                UpdatePickerFromModel(_picker.Model as PickerSource);
            };

            _picker.Frame = new CGRect(0, 0, 320, 250);

            var vc = new UIViewController { _picker, button };
            vc.View.Frame = new CGRect(0, 0, 320, 300);
            vc.PreferredContentSize = new CGSize(320, 300);
            _popOver = new UIPopoverController(vc);
            _popOver.PresentFromRect(new CGRect(Control.Frame.Width / 2, Control.Frame.Height - 3, 0, 0), Control, UIPopoverArrowDirection.Any, true);
            _popOver.DidDismiss += (s, e) =>
            {
                _popOver = null;
                Control.ResignFirstResponder();
            };
        }

        void RowsCollectionChanged(object sender, EventArgs e)
        {
            UpdatePicker();
        }

        void UpdatePicker()
        {
            var selectedIndex = Element.SelectedIndex;
            var items = Element.Items;
            _picker.ReloadAllComponents();
            if (items == null || items.Count == 0)
                return;

            UpdatePickerSelectedIndex(selectedIndex);
        }

        void UpdatePickerFromModel(PickerSource s)
        {
            if (Element != null)
            {
                ((IElementController)Element).SetValueFromRenderer(Picker.SelectedIndexProperty, s.SelectedIndex);
                _popOver.Dismiss(true);
            }
        }

        void UpdatePickerSelectedIndex(int formsIndex)
        {
            var source = (PickerSource)_picker.Model;
            source.SelectedIndex = formsIndex;
            source.SelectedItem = formsIndex >= 0 ? Element.Items[formsIndex] : null;
            _picker.Select(Math.Max(formsIndex, 0), 0, true);
        }

    }
}
