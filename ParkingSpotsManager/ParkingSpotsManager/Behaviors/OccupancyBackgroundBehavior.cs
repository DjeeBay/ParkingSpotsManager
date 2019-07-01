using ParkingSpotsManager.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace ParkingSpotsManager.Behaviors
{
    public class OccupancyBackgroundBehavior : BehaviorBase<StackLayout>
    {
        public static readonly BindableProperty OccupiedByProperty = BindableProperty.Create(nameof(OccupiedBy), typeof(int?), typeof(OccupancyBackgroundBehavior), null);
        public int? OccupiedBy
        {
            get => (int?)GetValue(OccupiedByProperty);
            set { SetValue(OccupiedByProperty, value); }
        }

        protected override void OnAttachedTo(StackLayout bindable)
        {
            base.OnAttachedTo(bindable);
            bindable.BindingContextChanged += Bindable_BindingContextChanged;
        }

        protected override void OnDetachingFrom(StackLayout bindable)
        {
            base.OnDetachingFrom(bindable);

            bindable.BindingContextChanged -= Bindable_BindingContextChanged;
        }

        private void Bindable_BindingContextChanged(object sender, EventArgs e)
        {
            var stackLayout = sender as StackLayout;
            if (OccupiedBy != null) {
                stackLayout.BackgroundColor = Color.FromHex("f8d7da");
            } else {
                stackLayout.BackgroundColor = Color.FromHex("d4edda");
            }
        }
    }
}
