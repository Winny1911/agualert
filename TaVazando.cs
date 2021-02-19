using System;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace TaVazando
{
	public class App : Application
	{
		public App ()
		{
			// The root page of your application

			//carrega os temas de UX da aplicação
			theme.LoadTheme ();

			//DependencyService.Get<IGSMSignal> ().GetSignalStrength ();

			Sessao.obtemConexao().Wait(1000);
			Sessao.GetCurrentCulture ();

		}

		protected override async void OnStart ()
		{

			if (Sessao.onLine)
			{
				Sessao.ObtemListaOcorrencias ();
				MainPage = new NavigationPage(new frmMain());
			}
			else
			{
				//não há conexão, navega para a página de orientação e encerramento do app
				Sessao.ConexaoInfo = "Caso usuário, é necessário estar conectado à internet e ter o GPS habilitado para publicar uma ocorrência. Verifique as configurações de seu aparelho e tente novamente.";
				MainPage = new NavigationPage(new StatusConexao());
			}
		}

		protected override void OnSleep (){
			// Handle when your app sleeps

		}

		protected override void OnResume (){
			// Handle when your app resumes

		}

			
	}
}

