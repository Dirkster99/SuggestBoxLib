using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;

namespace CachedPathSuggestBox.Demo.ValidationRules
{

   [ContentProperty("ComparisonValue")]
   public class IsEmptyValidationRule : ValidationRule
   {
      public ComparisonValue ComparisonValue { get; set; }

      public override ValidationResult Validate(object value, CultureInfo cultureInfo)
      {
         // prevents invalidating initial values
         //var str = value?.ToString().ToUpperInvariant();
         //if (string.IsNullOrWhiteSpace(str) || (str.Length == 1 && str[0] >= 'A' && str[0] <= 'Z'))
         //{
         //   return ValidationResult.ValidResult;
         //}

         return ComparisonValue.Value <= 0 ?
            new ValidationResult(false, $"Not a valid path {value}") :
            ValidationResult.ValidResult;
      }
   }

   public class ComparisonValue : DependencyObject
   {
      public int Value
      {
         get => (int)GetValue(ValueProperty);
         set => SetValue(ValueProperty, value);
      }
      public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
         nameof(Value),
         typeof(int),
         typeof(ComparisonValue),
         new PropertyMetadata(default(int), OnValueChanged));

      private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
      {
         ComparisonValue comparisonValue = (ComparisonValue)d;
         BindingExpressionBase bindingExpressionBase = BindingOperations.GetBindingExpressionBase(comparisonValue, BindingToTriggerProperty);
         bindingExpressionBase?.UpdateSource();
      }

      public object BindingToTrigger
      {
         get => GetValue(BindingToTriggerProperty);
         set => SetValue(BindingToTriggerProperty, value);
      }
      public static readonly DependencyProperty BindingToTriggerProperty = DependencyProperty.Register(
         nameof(BindingToTrigger),
         typeof(object),
         typeof(ComparisonValue),
         new FrameworkPropertyMetadata(default(object), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
   }


   public class BindingProxy : Freezable
   {
      protected override Freezable CreateInstanceCore()
      {
         return new BindingProxy();
      }

      public object Data
      {
         get => GetValue(DataProperty);
         set => SetValue(DataProperty, value);
      }
      public static readonly DependencyProperty DataProperty = DependencyProperty.Register(nameof(Data), typeof(object), typeof(BindingProxy), new UIPropertyMetadata(null));
   }
}
