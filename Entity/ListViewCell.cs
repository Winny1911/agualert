using System;
using Xamarin.Forms;

namespace TaVazando
{

	public class ListViewCell : ViewCell
	{
		public ListViewCell ()
		{
			var description = new Label();
			description.SetBinding(Label.TextProperty, "Descricao");
			description.Style = (Style)Application.Current.Resources ["section_title"];
			description.VerticalOptions = LayoutOptions.Center;
			description.HorizontalOptions = LayoutOptions.StartAndExpand;
			description.FontAttributes = FontAttributes.Bold;
			description.BindingContextChanged += (sender, e) =>{};

			var s = new StackLayout();
			s.Orientation = StackOrientation.Horizontal;
			s.Padding = 10;
			s.Children.Add(description);
			this.View = s;

		}
	} 

}

