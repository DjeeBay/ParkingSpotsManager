using ParkingSpotsManager.Controls;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace ParkingSpotsManager.Behaviors
{
    public class IsAdminToggleButtonBehavior : BehaviorBase<ToggleButton>
    {
        public static readonly BindableProperty IsAdminProperty = BindableProperty.Create(nameof(IsAdmin), typeof(int?), typeof(IsAdminToggleButtonBehavior), null);
        public int? IsAdmin
        {
            get => (int?)GetValue(IsAdminProperty);
            set { SetValue(IsAdminProperty, value); }
        }

        protected override void OnAttachedTo(ToggleButton bindable)
        {
            base.OnAttachedTo(bindable);
            bindable.BindingContextChanged += Bindable_BindingContextChanged;
        }

        protected override void OnDetachingFrom(ToggleButton bindable)
        {
            base.OnDetachingFrom(bindable);

            bindable.BindingContextChanged -= Bindable_BindingContextChanged;
        }

        private void Bindable_BindingContextChanged(object sender, EventArgs e)
        {
            var toggleButton = sender as ToggleButton;
            if (IsAdmin != null && IsAdmin == 1) {
                toggleButton.BackgroundColor = Color.FromHex("429e42");
            } else {
                toggleButton.BackgroundColor = Color.FromHex("C3C3C3");
            }
        }
    }
}
