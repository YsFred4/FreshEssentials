using System;
using Xamarin.Forms;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FreshEssentials
{
    public class SegmentedButton
    {
        public string Title { get; set; }
    }

    public class SegmentedButtonGroup : Grid
    {
        public static readonly BindableProperty OnColorProperty = BindableProperty.Create<SegmentedButtonGroup, Color>(p => p.OnColor, Color.Blue);

        public Color OnColor
        {
            get { return (Color)GetValue(OnColorProperty); }
            set { SetValue(OnColorProperty, value); }
        }

        public static readonly BindableProperty OffColorProperty = BindableProperty.Create<SegmentedButtonGroup, Color>(p => p.OffColor, Color.White);

        public Color OffColor
        {
            get { return (Color)GetValue(OffColorProperty); }
            set { SetValue(OffColorProperty, value); }
        }

        public static readonly BindableProperty CommandProperty = BindableProperty.Create<SegmentedButtonGroup, Command>(p => p.Command, default(Command));

        public Command Command
        {
            get { return (Command)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public IList<SegmentedButton> SegmentedButtons
        {
            get;
            internal set;
        }

        public static readonly BindableProperty SelectedIndexProperty = BindableProperty.Create<SegmentedButtonGroup, int>(p => p.SelectedIndex, default(int));

        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }

        public SegmentedButtonGroup()
        {
            var segmentedButtons = new ObservableCollection<SegmentedButton>();
            segmentedButtons.CollectionChanged += SegmentedButtons_CollectionChanged;  
            SegmentedButtons = segmentedButtons;
            this.ColumnSpacing = 0;
            this.RowSpacing = 0;
        }

        void SegmentedButtons_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RebuildButtons();
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            RebuildButtons();
        }

        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == "SelectedIndex")
            {
                SetSelectedIndex();
            }
        }

        void RebuildButtons()
        {
            this.ColumnDefinitions.Clear();
            this.Children.Clear();

            for (int i = 0; i < SegmentedButtons.Count; i++)
            {
                var buttonSeg = SegmentedButtons[i];

                var label = new Label
                { 
                    Text = buttonSeg.Title, 
                    FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)), 
                    HorizontalTextAlignment = TextAlignment.Center, 
                    VerticalTextAlignment = TextAlignment.Center 
                };

                this.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                var frame = new AdvancedFrame();
                if (i == 0)
                    frame.Corners = RoundedCorners.left;
                else if ((i + 1) == SegmentedButtons.Count)
                    frame.Corners = RoundedCorners.right;
                else
                    frame.Corners = RoundedCorners.none;
                frame.OutlineColor = OnColor;
                frame.Content = label;
                frame.HorizontalOptions = LayoutOptions.FillAndExpand;

                if (i == SelectedIndex)
                {
                    frame.InnerBackground = OnColor;
                    label.TextColor = OffColor;  
                }
                else
                {
                    frame.InnerBackground = OffColor;
                    label.TextColor = OnColor;  
                }

                var tapGesture = new TapGestureRecognizer();
                tapGesture.Command = ItemTapped;
                tapGesture.CommandParameter = i;
                frame.GestureRecognizers.Add(tapGesture);

                this.Children.Add(frame, i, 0);
            }
        }

        public Command ItemTapped
        {
            get
            {
                return new Command((obj) =>
                    {

                        int index = (int)obj;

                        SelectedIndex = index;

                        if (Command != null)
                        {
                            Command.Execute(this.SegmentedButtons[index].Title);
                        }
                    });
            }
        }

        void SetSelectedIndex()
        {
            for (int i = 0; i < Children.Count; i++)
            {
                var frame = Children[i] as AdvancedFrame;
                var label = frame.Content as Label;
                if (i == SelectedIndex)
                {
                    frame.InnerBackground = OnColor;
                    label.TextColor = OffColor;  
                }
                else
                {
                    frame.InnerBackground = OffColor;
                    label.TextColor = OnColor;  
                }
            }               
        }
    }
}
