namespace SuggestBoxLib
{
    using Interfaces;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    /// <summary>
    /// Implements a text based control that updates a list of suggestions
    /// when user updates a given text based path -> TextChangedEvent is raised.
    ///
    /// This control uses <see cref="ISuggestSource"/> and HierarchyHelper
    /// to suggest entries in a separate popup as the user types.
    /// </summary>
    public class SuggestBox : SuggestBoxBase
    {
        #region fields
        public static readonly RoutedEvent QueryChangedEvent = EventManager.RegisterRoutedEvent(nameof(QueryChanged), RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<string>), typeof(SuggestBox));
        public static readonly DependencyProperty TextChangedCommandProperty = DependencyProperty.Register(nameof(TextChangedCommand), typeof(ICommand), typeof(SuggestBox), new PropertyMetadata(null));

        public event RoutedPropertyChangedEventHandler<string> QueryChanged
        {
            add => AddHandler(QueryChangedEvent, value);
            remove => RemoveHandler(QueryChangedEvent, value);
        }

        #endregion fields

        #region Constructor
        static SuggestBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SuggestBox), new FrameworkPropertyMetadata(typeof(SuggestBox)));
        }

        public SuggestBox()
        {
            IsVisibleChanged += SuggestBox_IsVisibleChanged;
        }

        #endregion Constructor

        #region Public Properties

        /// <summary>
        /// Gets/sets a command that should be executed whenever the text in the textbox
        /// portion of this control has changed.
        /// </summary>
        public ICommand TextChangedCommand
        {
            get { return (ICommand)GetValue(TextChangedCommandProperty); }
            set { SetValue(TextChangedCommandProperty, value); }
        }

        #endregion Public Properties

        #region Methods
        
        /// <summary>
        /// Method executes when the <see cref="SuggestBoxBase.EnableSuggestions"/> dependency property has changed its value.
        /// </summary>
        protected override void OnEnableSuggestionChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnEnableSuggestionChanged(e);

            if ((bool)e.NewValue == true)
                QueryForSuggestions();
        }

        /// <summary>
        /// Method executes when the visibility of the control is changed to query for
        /// suggestions if this was enabled...
        /// </summary>
        private void SuggestBox_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (((bool)e.NewValue) == true)
                QueryForSuggestions();
        }

        /// <summary>
        /// Method executes when new text is entered in the textbox portion of the control.
        /// </summary>
        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);

            IsHintVisible = string.IsNullOrEmpty(this.Text);

            QueryForSuggestions();
        }

        private void QueryForSuggestions()
        {
            // A change during disabled state is likely to be caused by a bound property
            // in a viewmodel (a machine based edit rather than user input)
            // -> Lets break the message loop here to avoid unnecessary CPU processings...
            if (this.IsEnabled == false || this.IsLoaded == false)
                return;

            // Text change is likely to be from property change so we ignore it
            // if control is invisible or suggestions are currently not requested
            if (Visibility != Visibility.Visible || EnableSuggestions == false)
                return;

            if (ParentWindowIsClosing)
                return;

            this.RaiseEvent(new RoutedPropertyChangedEventArgs<string>(string.Empty, Text, QueryChangedEvent));

            // Check whether this attached behaviour is bound to a RoutedCommand
            if (this.TextChangedCommand is RoutedCommand command)
            {
                // Execute the routed command
                command.Execute(this.Text, this);
            }
            else
            {
                // Execute the Command as bound delegate if anything bound
                TextChangedCommand?.Execute(this.Text);
            }
        }
        #endregion Methods
    }
}