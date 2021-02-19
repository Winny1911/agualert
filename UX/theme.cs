using System;
using Xamarin.Forms;

namespace TaVazando
{
	public class theme
	{
		public theme ()
		{
		}

		public const string COLOR_PRIMARY ="#00aff0"; //light blue
		public const string COLOR_ACCENT_ONE ="#007dcd"; //medium blue
		public const string COLOR_ACCENT_TWO = "#1b5fa8"; //"#2e7acc"; //dark blue
		public const string COLOR_GREY ="#707070";
		public const string COLOR_GREY_LIGHT ="#9e9aa0";
		public const string COLOR_GREY_DARK ="#564c5d";
		public const string COLOR_WHITE ="#ffffff";
		public const string COLOR_BLACK ="#000000";

		public static void LoadTheme(){
			//font 
			const double size_base = 8;
			const double FONT_SIZE_H1 = size_base; //SMALLEST
			const double FONT_SIZE_H2 = size_base + (size_base*40/100);
			const double FONT_SIZE_H3 = size_base + (size_base*80/100);
			const double FONT_SIZE_H4 = size_base + (size_base*140/100);
			const double FONT_SIZE_H5 = size_base + (size_base*220/100);
			const double FONT_SIZE_H6 = size_base + (size_base*260/100);

			Application.Current.Resources = new ResourceDictionary ();

			Application.Current.Resources.Add ("H1", 
				new Style (typeof(Label)) {
					Setters = { 
						new Setter { Property = Label.FontSizeProperty, Value = FONT_SIZE_H1 },
						new Setter { Property = Label.TextColorProperty, Value = Color.FromHex(COLOR_BLACK) },
					}
				}
			);

			Application.Current.Resources.Add ("H2", 
				new Style (typeof(Label)) {
					BaseResourceKey = "H1",
					Setters = { 
						new Setter { Property = Label.FontSizeProperty, Value = FONT_SIZE_H2 },
					}
				}
			);

			Application.Current.Resources.Add ("H3", 
				new Style (typeof(Label)) {
					BaseResourceKey = "H1",
					Setters = { 
						new Setter { Property = Label.FontSizeProperty, Value = FONT_SIZE_H3 },
					}
				}
			);

			Application.Current.Resources.Add ("H4", 
				new Style (typeof(Label)) {
					BaseResourceKey = "H1",
					Setters = { 
						new Setter { Property = Label.FontSizeProperty, Value = FONT_SIZE_H4 },
					}
				}
			);

			Application.Current.Resources.Add ("H5", 
				new Style (typeof(Label)) {
					BaseResourceKey = "H1",
					Setters = { 
						new Setter { Property = Label.FontSizeProperty, Value = FONT_SIZE_H5 },
					}
				}
			);

			Application.Current.Resources.Add ("H6", 
				new Style (typeof(Label)) {
					BaseResourceKey = "H1",
					Setters = { 
						new Setter { Property = Label.FontSizeProperty, Value = FONT_SIZE_H6 },
					}
				}
			);

			#region LABELS

			Application.Current.Resources.Add ("description_title", 
				new Style (typeof(Label)) {
					Setters = { 
						new Setter { Property = Label.FontSizeProperty, Value = FONT_SIZE_H4 },
						new Setter { Property = Label.TextColorProperty, Value = Color.FromHex (COLOR_BLACK)},

						new Setter { Property = Label.LineBreakModeProperty, Value="CharacterWrap"},
					}
				}
			);

			Application.Current.Resources.Add ("description_subtitle", 
				new Style (typeof(Label)) {
					BaseResourceKey="H2",
					Setters = { 
						new Setter { Property = Label.TextColorProperty, Value = Color.FromHex (COLOR_GREY_DARK)},
						new Setter { Property = Label.FontAttributesProperty, Value = "Bold"  },
					}
				}
			);

			Application.Current.Resources.Add ("section_title", 
				new Style (typeof(Label)) {
					Setters = { 
						new Setter { Property = Label.FontSizeProperty, Value = FONT_SIZE_H3 },
						new Setter { Property = Label.TextColorProperty, Value = Color.FromHex (COLOR_ACCENT_TWO)},
						new Setter { Property = Label.FontAttributesProperty, Value = "Bold"  },
					}
				}
			);


			Application.Current.Resources.Add ("section_title_dark", 
				new Style (typeof(Label)) {
					Setters = { 
						new Setter { Property = Label.FontSizeProperty, Value = FONT_SIZE_H3 },
						new Setter { Property = Label.TextColorProperty, Value = Color.FromHex (COLOR_GREY_DARK)},
						new Setter { Property = Label.FontAttributesProperty, Value = "Bold"  },
					}
				}
			);

			Application.Current.Resources.Add ("regular_text", 
				new Style (typeof(Label)) {
					Setters = { 
						new Setter { Property = Label.FontSizeProperty, Value = FONT_SIZE_H3 },
						new Setter { Property = Label.TextColorProperty, Value = Color.FromHex (COLOR_ACCENT_TWO)},
					}
				}
			);

			Application.Current.Resources.Add ("toolbar_text", 
				new Style (typeof(Label)) {
					Setters = { 
						new Setter { Property = Label.FontSizeProperty, Value = FONT_SIZE_H4 },
						new Setter { Property = Label.TextColorProperty, Value = Color.FromHex (COLOR_WHITE)},
						new Setter { Property = Label.HorizontalOptionsProperty, Value = "StartAndExpand"},
						new Setter { Property = Label.VerticalOptionsProperty, Value = "CenterAndExpand"},
					}
				}
			);


			Application.Current.Resources.Add ("message_text", 
				new Style (typeof(Label)) {
					Setters = { 
						new Setter { Property = Label.FontSizeProperty, Value = FONT_SIZE_H4 },
						new Setter { Property = Label.TextColorProperty, Value = Color.FromHex (COLOR_GREY_DARK)},
						new Setter { Property = Label.HorizontalOptionsProperty, Value = "StartAndExpand"},
						new Setter { Property = Label.VerticalOptionsProperty, Value = "CenterAndExpand"},
						new Setter { Property = Label.FontAttributesProperty, Value = "Bold"  },
					}
				}
			);

			#endregion

			#region BUTTONS
			Application.Current.Resources.Add ("button",
				new Style (typeof(Button)) {
					Setters = {
						new Setter { Property = Button.FontSizeProperty, Value = FONT_SIZE_H3 },
						new Setter { Property = Button.TextColorProperty, Value = Color.FromHex (COLOR_WHITE)  },
						new Setter { Property = Button.HorizontalOptionsProperty, Value = "FillAndExpand" },
						new Setter { Property = Button.BackgroundColorProperty, Value = Color.FromHex (COLOR_ACCENT_ONE) },
						new Setter { Property = Button.FontAttributesProperty, Value = "Bold" },
					}
				}
			);
			#endregion

			#region SEPARATORS
			Application.Current.Resources.Add ("horizontal_separator",
				new Style (typeof(BoxView)) {
					Setters = {
						new Setter { Property = View.HorizontalOptionsProperty, Value = "FillAndExpand" },
						new Setter { Property = VisualElement.BackgroundColorProperty, Value = Color.FromHex (COLOR_ACCENT_TWO) },
						new Setter { Property = VisualElement.HeightRequestProperty, Value = 2 },
					}
				}
			);

			Application.Current.Resources.Add ("vertical_margin",
				new Style (typeof(BoxView)) {
					Setters = {
						new Setter { Property = View.HorizontalOptionsProperty, Value = "FillAndExpand" },
						new Setter { Property = VisualElement.BackgroundColorProperty, Value = Color.Transparent},
						new Setter { Property = VisualElement.HeightRequestProperty, Value = 10 },
					}
				}
			);

			Application.Current.Resources.Add ("hr",
				new Style (typeof(BoxView)) {
					Setters = {
						new Setter { Property = View.HorizontalOptionsProperty, Value = "FillAndExpand" },
						new Setter { Property = VisualElement.BackgroundColorProperty, Value = Color.FromHex(COLOR_ACCENT_TWO)},
						new Setter { Property = VisualElement.HeightRequestProperty, Value = 1 },
					}
				}
			);

			#endregion


			#region CONTAINERS
			Application.Current.Resources.Add ("scrollview",
				new Style (typeof(ScrollView)) {
					BaseResourceKey="main_container_color",
					Setters = {
						new Setter { Property = View.HorizontalOptionsProperty, Value = "FillAndExpand" },
						new Setter { Property = View.VerticalOptionsProperty, Value = "FillAndExpand" },
					}
				}
			);

			Application.Current.Resources.Add ("main_container",
				new Style (typeof(StackLayout)) {
					BaseResourceKey="main_container_color",
					Setters = {
						new Setter { Property = View.HorizontalOptionsProperty, Value = "FillAndExpand" },
						new Setter { Property = View.VerticalOptionsProperty, Value = "FillAndExpand" },
						new Setter { Property = StackLayout.OrientationProperty, Value = "Vertical" },
						new Setter { Property = StackLayout.SpacingProperty, Value = 0 },
					}
				}
			);

			Application.Current.Resources.Add ("inner_container",
				new Style (typeof(StackLayout)) {
					BaseResourceKey="inner_container_color",
					Setters = {
						new Setter { Property = View.HorizontalOptionsProperty, Value = "FillAndExpand" },
						new Setter { Property = View.VerticalOptionsProperty, Value = "FillAndExpand" },
						new Setter { Property = StackLayout.OrientationProperty, Value = "Vertical" },
						new Setter { Property = Layout.PaddingProperty, Value = new Thickness(10) },
					}
				}
			);

			Application.Current.Resources.Add ("cell_container",
				new Style (typeof(StackLayout)) {
					Setters = {
						new Setter { Property = View.HorizontalOptionsProperty, Value = "FillAndExpand" },
						new Setter { Property = StackLayout.OrientationProperty, Value = "Horizontal" },
						new Setter { Property = Layout.PaddingProperty, Value = new Thickness(10,5,10,5) },
						new Setter { Property = StackLayout.SpacingProperty, Value = 0 },

					}
				}
			);

			Application.Current.Resources.Add ("cell_details_container",
				new Style (typeof(StackLayout)) {
					Setters = {
						new Setter { Property = View.HorizontalOptionsProperty, Value = "FillAndExpand" },
						new Setter { Property = Layout.PaddingProperty, Value = new Thickness(10,0,0,0) },
						new Setter { Property = StackLayout.SpacingProperty, Value = 0 },
					}
				}
			);

			Application.Current.Resources.Add ("dynamic_toolbar",
				new Style (typeof(StackLayout)) {
					Setters = {
						new Setter { Property = View.HorizontalOptionsProperty, Value = "FillAndExpand" },
						new Setter { Property = StackLayout.SpacingProperty, Value = 10 },
						new Setter { Property = StackLayout.OrientationProperty, Value = "Horizontal" },
						new Setter { Property = StackLayout.BackgroundColorProperty, Value = Color.FromHex(COLOR_ACCENT_TWO) },
					}
				}
			);


			#endregion

			#region IMAGES
			Application.Current.Resources.Add ("cell_image",
				new Style (typeof(Image)) {
					Setters = {
						new Setter { Property = VisualElement.WidthRequestProperty, Value = 50 },
						new Setter { Property = VisualElement.HeightRequestProperty, Value = 50 },
						new Setter { Property = View.HorizontalOptionsProperty, Value = "Center" },
						new Setter { Property = View.VerticalOptionsProperty, Value = "Center" },
					}
				}
			);

			Application.Current.Resources.Add ("cell_tap_image",
				new Style (typeof(Image)) {
					Setters = {
						new Setter { Property = Image.SourceProperty, Value = "tap.png" },
						new Setter { Property = VisualElement.HeightRequestProperty, Value = 12 },
						new Setter { Property = View.HorizontalOptionsProperty, Value = "End" },
					}
				}
			);

			Application.Current.Resources.Add ("cell_indicators_image_base",
				new Style (typeof(Image)) {
					Setters = {
						new Setter { Property = VisualElement.HeightRequestProperty, Value = 12 },
						new Setter { Property = VisualElement.WidthRequestProperty, Value = 12 },
						new Setter { Property = View.HorizontalOptionsProperty, Value = "End" },
					}
				}
			);

			Application.Current.Resources.Add ("cell_heart_image",
				new Style (typeof(Image)) {
					BaseResourceKey="cell_indicators_image_base",
					Setters = {
						new Setter { Property = Image.SourceProperty, Value = "heart.png" },
					}
				}
			);

			Application.Current.Resources.Add ("cell_star_image",
				new Style (typeof(Image)) {
					BaseResourceKey="cell_indicators_image_base",
					Setters = {
						new Setter { Property = Image.SourceProperty, Value = "star.png" },
					}
				}
			);

			Application.Current.Resources.Add ("dynamic_toolbar_icon_base",
				new Style (typeof(Image)) {
					Setters = {
						new Setter { Property = View.HorizontalOptionsProperty, Value = "Start" },
					}
				}
			);

			Application.Current.Resources.Add ("toolbar_icon",
				new Style (typeof(Image)) 
				{
					BaseResourceKey="dynamic_toolbar_icon_base",
					Setters = {
						new Setter { Property = Image.SourceProperty, Value = "ic_toolbar.png" },
					}
				}
			);

			Application.Current.Resources.Add ("icon_gps",
				new Style (typeof(Image)) 
				{
					BaseResourceKey="dynamic_toolbar_icon_base",
					Setters = {
						new Setter { Property = Image.SourceProperty, Value = "ic_gps.png" },
					}
				}
			);


			Application.Current.Resources.Add ("icon_seletor",
				new Style (typeof(Image)) 
				{
					BaseResourceKey="dynamic_toolbar_icon_base",
					Setters = {
						new Setter { Property = Image.SourceProperty, Value = "ic_seletor.png" },
					}
				}
			);


			Application.Current.Resources.Add ("icon_search",
				new Style (typeof(Image)) 
				{
					BaseResourceKey="dynamic_toolbar_icon_base",
					Setters = {
						new Setter { Property = Image.SourceProperty, Value = "ic_search.png" },
					}
				}
			);

			Application.Current.Resources.Add ("icon_wifi",
				new Style (typeof(Image)) 
				{
					Setters = {
						new Setter { Property = Image.SourceProperty, Value = "wifi.png" },
						new Setter { Property = View.HorizontalOptionsProperty, Value = "CenterAndExpand" },
						new Setter { Property = View.VerticalOptionsProperty, Value = "StartAndExpand" },
						new Setter { Property = View.WidthRequestProperty, Value = "64" },
						new Setter { Property = View.HeightRequestProperty, Value = "64" },

					}
				}
			);

			Application.Current.Resources.Add ("icon_location",
				new Style (typeof(Image)) 
				{
					BaseResourceKey="dynamic_toolbar_icon_base",
					Setters = {
						new Setter { Property = Image.SourceProperty, Value = "ic_local.png" },
						new Setter { Property = Image.WidthRequestProperty, Value = "28" },
						new Setter { Property = Image.HeightRequestProperty, Value = "28" },
					}
				}
			);

			Application.Current.Resources.Add ("icon_help",
				new Style (typeof(Image)) 
				{
					BaseResourceKey="dynamic_toolbar_icon_base",
					Setters = {
						new Setter { Property = Image.SourceProperty, Value = "ic_help.png" },
						new Setter { Property = Image.WidthRequestProperty, Value = "28" },
						new Setter { Property = Image.HeightRequestProperty, Value = "28" },
					}
				}
			);

			Application.Current.Resources.Add ("icon_description",
				new Style (typeof(Image)) 
				{
					Setters = {
						new Setter { Property = Image.SourceProperty, Value = "ic_descricao.png" },
						new Setter { Property = Image.WidthRequestProperty, Value = "28" },
						new Setter { Property = Image.HeightRequestProperty, Value = "28" },
					}
				}
			);

			Application.Current.Resources.Add ("icon_appointment",
				new Style (typeof(Image)) 
				{
					Setters = {
						new Setter { Property = Image.SourceProperty, Value = "ic_ocorrencia.png" },
						new Setter { Property = Image.WidthRequestProperty, Value = "28" },
						new Setter { Property = Image.HeightRequestProperty, Value = "28" },
					}
				}
			);

			#endregion


			#region EDITEXT | TEXTBOX
			Application.Current.Resources.Add ("entry_default",
				new Style (typeof(Entry)) {
					Setters = {
						new Setter { Property = Entry.BackgroundColorProperty, Value = Color.FromHex(COLOR_WHITE) },
						new Setter { Property = Entry.TextColorProperty, Value = Color.FromHex (COLOR_ACCENT_TWO)},
					}
				}
			);

			Application.Current.Resources.Add ("entry_color",
				new Style (typeof(Entry)) {
					Setters = {
						new Setter { Property = Entry.BackgroundColorProperty, Value = Color.FromHex(COLOR_WHITE) },
						new Setter { Property = Entry.TextColorProperty, Value = Color.FromHex (COLOR_ACCENT_TWO)},

					}
				}
			);

			Application.Current.Resources.Add ("editor",
				new Style (typeof(Editor)) {
					Setters = {
						new Setter { Property = Editor.BackgroundColorProperty, Value = Color.FromHex(COLOR_WHITE) },
						new Setter { Property = Editor.TextProperty, Value = Color.FromHex (COLOR_ACCENT_TWO) },
					}
				}
			);


			#endregion

			#region BACKGROUNDS
			Application.Current.Resources.Add ("main_container_color",
				new Style (typeof(StackLayout)) {
					Setters = {
						new Setter { Property = VisualElement.BackgroundColorProperty, Value = Color.FromHex(COLOR_WHITE) },
					}
				}
			);

			Application.Current.Resources.Add ("main_container_dashboard_color",
				new Style (typeof(StackLayout)) {
					Setters = {
						new Setter { Property = VisualElement.BackgroundColorProperty, Value = Color.FromHex(COLOR_WHITE) },
						new Setter { Property = StackLayout.SpacingProperty, Value = 15 },
						new Setter { Property = StackLayout.PaddingProperty, Value =   new Thickness(20)},
					}
				}
			);

			Application.Current.Resources.Add ("main_container_empty_color",
				new Style (typeof(StackLayout)) {
					Setters = {
						new Setter { Property = StackLayout.SpacingProperty, Value = 15 },
						new Setter { Property = StackLayout.PaddingProperty, Value =   new Thickness(20)},
					}
				}
			);


			Application.Current.Resources.Add ("inner_container_color",
				new Style (typeof(StackLayout)) {
					Setters = {
						new Setter { Property = VisualElement.BackgroundColorProperty, Value = Color.FromHex(COLOR_WHITE) },
					}
				}
			);

			#endregion


			#region LISTVIEW
			Application.Current.Resources.Add("list_view",
				new Style (typeof(ListView)) {
					Setters = {
						new Setter { Property = VisualElement.BackgroundColorProperty, Value = Color.Transparent},
						new Setter { Property = ListView.SeparatorColorProperty, Value = Color.Transparent },
						new Setter { Property = ListView.VerticalOptionsProperty, Value ="FillAndExpand" },

					}
				}
			);

			#endregion

		}
	}
}

