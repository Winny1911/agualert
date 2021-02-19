using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace TaVazando
{
	public partial class StatusConexao : ContentPage
	{
		public StatusConexao ()
		{
			NavigationPage.SetHasNavigationBar (this, false);
			BackgroundImage = "TileBackground.xml";
			InitializeComponent ();

			lblMensagem.Text = Sessao.ConexaoInfo; //"Caso usuário, é necessário estar conectado à internet e ter o GPS habilitado para publicar uma ocorrência. Verifique as configurações de seu aparelho e tente novamente.";


			btFinish.Clicked+= (object sender, EventArgs e) => {

				if(Device.OS == TargetPlatform.Android)
					DependencyService.Get<IAndroidMethods>().CloseApp();

			};


		}
	}
}

