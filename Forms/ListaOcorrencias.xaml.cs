using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace TaVazando
{
	public partial class ListaOcorrencias : ContentPage
	{
		public ListaOcorrencias ()
		{
			NavigationPage.SetHasNavigationBar (this, false);
			InitializeComponent ();

			ListView lstOcorrencias = new ListView ();
			lstOcorrencias.Style = (Style)Application.Current.Resources ["list_view"];
			lstOcorrencias.ItemsSource = Sessao.Ocorrencias;
			lstOcorrencias.ItemTemplate = new DataTemplate (typeof(ListViewCell));
			lstOcorrencias.SeparatorColor = Color.FromHex(theme.COLOR_GREY_LIGHT);
			lstOcorrencias.ItemSelected += (object sender, SelectedItemChangedEventArgs e) => 
			{
				//
				//Sessao.ocorrenciaId = ((TipoOcorrencia)e.SelectedItem).Id;

				Sessao.OcorrenciaAtiva.TipoOcorrencia = ((TipoOcorrencia)e.SelectedItem);

				Application.Current.MainPage = new NavigationPage(new frmMain());
			};

			listContainer.Children.Add (lstOcorrencias);

		}
	}
}

