using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using System.Threading.Tasks;
using System.Linq;
using System.ComponentModel;
using System.Windows.Input;
using Media.Plugin;
using System.IO;
using XLabs.Platform.Device;
using XLabs.Platform;
using XLabs.Ioc;
using XLabs.Platform.Services.Geolocation;
using System.Net;
using System.Threading;
using System.Text.RegularExpressions;
using XLabs.Platform.Services.Media;

namespace TaVazando
{
	public partial class frmMain : ContentPage
	{


		public bool NeedShowMegaLog = false;

		public frmMain ()
		{

			NavigationPage.SetHasNavigationBar (this, false);
			InitializeComponent ();


			activityIndicator.IsRunning = true;
			activityIndicator.IsEnabled = true;
			activityIndicator.BindingContext = this;
			activityIndicator.SetBinding (ActivityIndicator.IsVisibleProperty, "IsBusy");

			activityIndicator.PropertyChanged+= (object sender, PropertyChangedEventArgs e) => {};

			Iniciar();

		}

		protected async void Iniciar()
		{
			try {


				//IsBusy = true;

				#region GEO
				//processa gps e endereço
				//testa para ver se a parte de geo da ocorrencia já está preenchida
				if (Sessao.OcorrenciaAtiva == null || !Sessao.OcorrenciaAtiva.LocalizacaoReconhecida) {
					//Sessao.OcorrenciaAtiva= new Ocorrencia();
					
					//caso negativo inicia o GPS para pegar as coordenadas correntes do usuario
					if(Sessao.OcorrenciaAtiva.Latitude == 0 || Sessao.OcorrenciaAtiva.Longitude == 0)
					{
						await IniciarGPS ();

						if (Sessao.OcorrenciaAtiva.Latitude != 0 && Sessao.OcorrenciaAtiva.Longitude != 0) {
							//buscar endereço na API do google geocode
							await ObtemEnderecoPorCoordenada ();
						} else {
							//o GPS aparentemente falhou na busca das coordenadas, vamos solicitar o cep do usuário
							//await ObtemEnderecoPorCEP ();
							SolicitarCEP();
						}
					}
				} else {
					//se positivo então criar o mapa de acordo com as coordenadas
//					CriarMapa (Sessao.OcorrenciaAtiva.Latitude, Sessao.OcorrenciaAtiva.Longitude);
					CriarMapa ();
				}
				#endregion
				
				#region SelecionarOcorrencia
				//atribui evento ao clique do icone de seleção de tipos de ocorrencias
				AtribuiEventoAbrirTipoOcorrencia ();
				#endregion
				
				#region SelecionarFoto
				//atribui evento para captura de fotos
				AtribuiEventoCapturaFotos ();
				#endregion

				#region LinkHelp
				AtribuirLinkHelp();
				#endregion

				#region EnderecoPorCEP
				AtribuiCEPClick();
				#endregion



			} catch (Exception ex) {
				DisplayLog (string.Format("{0} : {1}", "iniciar", ex.Message));

			}finally{
				//IsBusy = false;
			}
		}


		protected void AtribuirLinkHelp() 
		{

			try {
				var tapGestureRecognizerLink = new TapGestureRecognizer ();
				
				tapGestureRecognizerLink.Tapped +=(object sender, EventArgs e) => {
					var uri = new Uri("http://tavazando.com/");
					Device.OpenUri(uri);
				};
				
				helpLinker.GestureRecognizers.Add (tapGestureRecognizerLink);
			} catch (Exception ex) {
				DisplayLog (string.Format("{0} : {1}", "AtribuirLinkHelp", ex.Message));
			}

		}

		protected void AtribuiEventoCapturaFotos()
		{

			try 
			{
				//evento para capturar foto
				var tapGestureRecognizerSnapShot = new TapGestureRecognizer ();
				
				//se o dispositivo não possuir camera, desabilita o clique de chamada da opção de foto e
				//informa os procedimentos ao usuario quanto a carga de fotos local (galeria)
				
				//não possui camera?
				if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported) 
				{
					//não, então testa se as fotos estão liberadas para serem escolhidas no aparelho pode escolher fotos?
					if (!CrossMedia.Current.IsPickPhotoSupported) 
					{
						//não, não pode escolher
						btSnapshot.Source = "ic_picture_blocked.png";
						lblSnapshotGuidance.Text = "NÃO É POSSÍVEL INCLUIR UMA FOTO. MESMO ASSIM VOCÊ PODE CONTINUAR A PUBLICAÇÃO DA SUA OCORRÊNCIA.";					
					}
					else
					{
						//sim, pode escolher
						btSnapshot.Source = "ic_picture_gallery.png";
						lblSnapshotGuidance.Text = "CLIQUE NO ÍCONE AO LADO PARA ESCOLHER UMA FOTO EM SEU DISPOSITIVO.";
				
						//ao clicar executa o evento
						tapGestureRecognizerSnapShot.Tapped += (object sender, EventArgs e) => {
							PickPhotoFromGallery ();
						};
				
						//adiciona evento a imagem
						btSnapshot.GestureRecognizers.Add (tapGestureRecognizerSnapShot);
					}
				} 
				else
				{
				
					//possui camera 
					tapGestureRecognizerSnapShot.Tapped += async  (object sender, EventArgs e) => {
						//variavel para retenção da resposta
						string result = string.Empty;
				
						// é permitido escolher foto na galeria também?
						if (CrossMedia.Current.IsPickPhotoSupported) 
						{
							//sim, pode escolher, então mostra um pop up com as opções para captura de foto
							result = await DisplayActionSheet ("TaVazando - Foto", "Fechar", "", new string[] {"Nova Foto","Escolher na Galeria"});
						}
						else 
						{
							//não, não pode
							result = "nova foto";
						}
				
						//testa resultado
						if (!string.IsNullOrEmpty (result) && result.ToLower ().Equals ("nova foto")) 
						{
							//cria objeto de midia
							Media.Plugin.Abstractions.StoreCameraMediaOptions options = new Media.Plugin.Abstractions.StoreCameraMediaOptions ();
				
							//cria novo nome para a imagem utilizando os 10 primeiros digitos de um novo Guid
							options.Name = string.Format ("ocr_{0}", Guid.NewGuid ().ToString ().Substring (0, 10));
				
							//ativa a camera
							Media.Plugin.Abstractions.MediaFile photo = await CrossMedia.Current.TakePhotoAsync (options);
				
							Sessao.OcorrenciaAtiva.CaminhoImage = photo.Path;
							Sessao.OcorrenciaAtiva.NomeImagem = options.Name;
							Sessao.OcorrenciaAtiva.Image = photo;
							ApplyPhoto();

						}
						else
						{
							//escolhe foto diretamente do celular
							PickPhotoFromGallery ();
						}
					};
				
					btSnapshot.GestureRecognizers.Add (tapGestureRecognizerSnapShot);
				
				}

			}
			catch (Exception ex) 
			{
				DisplayLog (ex.Message);
				DisplayLog (string.Format("{0} : {1}", "AtribuiEventoCapturaFotos", ex.Message));
			}
		}

		protected void SolicitarCEP(){

			try 
			{
				XLabs.Forms.Controls.PopupLayout _PopUpLayoutCEP = new XLabs.Forms.Controls.PopupLayout ();
				
				Label title = new Label ();
				title.Text = "Usuário, não foi possível localizar as coordenadas de sua posição pelo GPS de seu celular. Por favor informe abaixo o CEP da localização da ocorrẽncia e clique no botão abaixo.";
				title.Style = (Style)Application.Current.Resources ["regular_text"];
				
				Entry txtUserZipCode = new Entry ();
				txtUserZipCode.Style = (Style)Application.Current.Resources ["entry_default"];
				txtUserZipCode.Placeholder = "Digite aqui o CEP";

				Button btnSearch = new Button ();
				btnSearch.Style = (Style)Application.Current.Resources ["button"];
				btnSearch.Text = "Localizar Endereço";
				btnSearch.Clicked += async (sender, e) => 
				{

					try {
						IsBusy=true;
						
						if (string.IsNullOrEmpty (txtUserZipCode.Text)) 
						{
							await DisplayAlert ("TáVazando", "Informe um CEP válido", "Fechar");
						} 
						else 
						{
							await ObtemEnderecoPorCEP (txtUserZipCode.Text);

							Application.Current.MainPage = new NavigationPage (new frmMain ());

							if (_PopUpLayoutCEP.IsPopupActive) 
							{
								_PopUpLayoutCEP.DismissPopup ();
								ctrlPopup.IsVisible = false;
								btnPublicar.IsVisible = true;
								controls.IsVisible = true;
							}
						}
					
					}
					finally 
					{
						
						IsBusy = false;
					}
				
				};
				
				StackLayout header = new StackLayout ();
				header.Style = (Style)Application.Current.Resources ["dynamic_toolbar"];
				header.Padding = 10;
				header.Spacing = 10;
				header.Orientation = StackOrientation.Horizontal;
				header.Children.Add (new Image (){ Style = (Style)Application.Current.Resources ["toolbar_icon"] });
				header.Children.Add (new Label () {
					Style = (Style)Application.Current.Resources ["toolbar_text"],
					Text = "Localização da Ocorrência"
				});

				StackLayout subPop = new StackLayout{
					WidthRequest = this.Width, // Important, the Popup hast to have a size to be showed
					HeightRequest = 300D,
					BackgroundColor = Color.White, // for Android and WP
					Orientation = StackOrientation.Vertical,
					Padding = 20,
					Spacing = 10,
					Children = {
						title,
						txtUserZipCode,
						btnSearch
					}
				};


				StackLayout PopUp = new StackLayout {
					WidthRequest = this.Width, // Important, the Popup hast to have a size to be showed
					HeightRequest = this.Height,
					BackgroundColor = Color.White, // for Android and WP
					Orientation = StackOrientation.Vertical,
					Padding = 0,
					Spacing = 20,
					Children = {
						header,
						subPop
					}
				};
				
				ctrlPopup.Children.Add (PopUp);
				ctrlPopup.IsVisible = true;
				btnPublicar.IsVisible = false;
				controls.IsVisible = false;
				
				_PopUpLayoutCEP.Content = ctrlPopup; 
				_PopUpLayoutCEP.ShowPopup (PopUp);
			} catch (Exception ex) {
				DisplayLog (string.Format("SolicitarCEP {0}", ex.Message));
			}finally{
				IsBusy = false; 
			}

		}

		protected void AtribuiEventoAbrirTipoOcorrencia()
		{
			try 
			{
//				//cria um novo controlador de reconhecimento de toque
				var tapGestureRecognizer = new TapGestureRecognizer ();
//				
//				//cria o evento
				tapGestureRecognizer.Tapped += (object s, EventArgs ev) => {
					//AbreListaTipoOcorrencia ();

					XLabs.Forms.Controls.PopupLayout _PopUpLayout = new XLabs.Forms.Controls.PopupLayout();

					ListView lstOcorrencias = new ListView ();
					lstOcorrencias.Style = (Style)Application.Current.Resources ["list_view"];
					lstOcorrencias.ItemsSource = Sessao.Ocorrencias;
					lstOcorrencias.ItemTemplate = new DataTemplate (typeof(ListViewCell));
					lstOcorrencias.SeparatorColor = Color.FromHex(theme.COLOR_GREY_LIGHT);
					lstOcorrencias.HasUnevenRows =true;
					lstOcorrencias.ItemSelected += (object sender, SelectedItemChangedEventArgs e) => 
					{
						Sessao.OcorrenciaAtiva.TipoOcorrencia = ((TipoOcorrencia)e.SelectedItem);
						Application.Current.MainPage = new NavigationPage(new frmMain());

						if (_PopUpLayout.IsPopupActive){
							_PopUpLayout.DismissPopup();
						}

						ctrlPopup.IsVisible=false;
						btnPublicar.IsVisible=true;
						controls.IsVisible=true;
					};


					StackLayout header =  new StackLayout ();
					header.Style =  (Style)Application.Current.Resources ["dynamic_toolbar"];
					header.Padding=10;
					header.Spacing = 10;
					header.Orientation = StackOrientation.Horizontal;
					header.Children.Add(new Image(){ Style = (Style)Application.Current.Resources ["toolbar_icon"]});
					header.Children.Add(new Label(){ Style = (Style)Application.Current.Resources ["toolbar_text"], Text = "Tipos de Ocorrência"});

					StackLayout PopUp = new StackLayout
					{
						WidthRequest = this.Width, // Important, the Popup hast to have a size to be showed
						HeightRequest = this.Height,
						BackgroundColor = Color.White, // for Android and WP
						Orientation = StackOrientation.Vertical,
						Padding=0,
						Spacing=10,
						Children =
						{
							header, // my Label on top
							lstOcorrencias // The Listview (all Cities/Zip-Codes in the Datasurce -> List)
						}
					};

					ctrlPopup.Children.Add(PopUp);
					ctrlPopup.IsVisible=true;
					btnPublicar.IsVisible=false;
					controls.IsVisible=false;

					_PopUpLayout.Content = ctrlPopup; 
					_PopUpLayout.ShowPopup(PopUp);

				};

				//adiciona o evento ao controle de interface grafica
				btSeletor.GestureRecognizers.Add (tapGestureRecognizer);
				clickArea.GestureRecognizers.Add (tapGestureRecognizer);
			}
			catch (Exception ex) 
			{
				DisplayLog (string.Format("{0} : {1}", "AtribuiEventoAbrirTipoOcorrencia", ex.Message));

			}
		}

		protected void AbreListaTipoOcorrencia()
		{
			try 
			{
				//carrega nova tela com as opções da lista de tipos de ocorrência
				Navigation.PushModalAsync (new ListaOcorrencias (), true);
			}
			catch (Exception ex) 
			{
				DisplayLog (string.Format("{0} : {1}", "AbreListaTipoOcorrencia", ex.Message));

			}
		}

		protected async void PickPhotoFromGallery()
		{
			try 
			{
				//aciona a escolha da foto via plugin
				Media.Plugin.Abstractions.MediaFile  photo = await CrossMedia.Current.PickPhotoAsync ();

				//se o retorno não for "nulo", ou seja o usuário cancelou a operação, então pega o nome da foto escolhida
				if(photo!=null){
					Sessao.OcorrenciaAtiva.CaminhoImage= photo.Path;
					Sessao.OcorrenciaAtiva.NomeImagem=photo.Path.Substring(photo.Path.LastIndexOf("/")+1, photo.Path.Length-photo.Path.LastIndexOf("/")-1);
					Sessao.OcorrenciaAtiva.Image = photo;
					ApplyPhoto();
				}

			}
			catch(Exception ex)
			{
				DisplayLog (string.Format("{0} : {1}", "PickPhotoFromGallery", ex.Message));
			}
		}


		void ApplyPhoto()
		{
			btSnapshot.Source = "ic_picture_ok.png"; //ImageSource.FromStream(()=>Sessao.OcorrenciaAtiva.Image.GetStream());
			lblSnapshotGuidance.Text = "Foto adicionada com sucesso. Clique no ícone para mudar a foto.";
		}
	

//		protected void CriarMapa(double latitude, double longitude)
		protected void CriarMapa()
		{
			try 
			{

				IsBusy = true;
				Map map = new Map ();

				map.IsShowingUser = true;
				map.HeightRequest = 100;
				map.WidthRequest = 960;
				map.VerticalOptions = LayoutOptions.FillAndExpand;
				map.HorizontalOptions = LayoutOptions.FillAndExpand;

				//criar instancia do mapa
				if(StackMap.Children.Count > 0 && StackMap.Children[0].GetType().Name.Equals("Map")){
					StackMap.Children.RemoveAt(0);
					StackMap.Children.Add (map);
				}else{
					StackMap.Children.Add (map);
				}

				//var position = new Xamarin.Forms.Maps.Position (latitude, longitude);
				var position = new Xamarin.Forms.Maps.Position (Sessao.OcorrenciaAtiva.Latitude, Sessao.OcorrenciaAtiva.Longitude);

				Pin p = new Pin();
				p.Type = PinType.Generic;
				p.Position = position;
				p.Label ="";

				map.Pins.Add(p);
				map.MoveToRegion(new MapSpan(position, 0.01, 0.01));
				map.HasZoomEnabled = true;

			}
			catch (Exception ex) 
			{
				DisplayLog (string.Format("{0} : {1}", "CriarMapa", ex.Message));

			}finally{
				IsBusy = false;
			}
		}

		protected async Task IniciarGPS(){

			IsBusy = true;

			//cria o simple container para o serviço
			var oGeolocator = Resolver.Resolve<IGeolocator> ();

			//define a distancia de precisão do ponto onde o dispositivo se encontra
			oGeolocator.DesiredAccuracy = 0;

			//inicia o serviço
			oGeolocator.StartListening (3000, 0);

			//solicita resultados
			var pos = await oGeolocator.GetPositionAsync (3000); 

			//fecha o serviço
			oGeolocator.StopListening (); 

			try {
				
				//caso retorne latitude e longitude
				if (pos != null) {
					//criar instancia do mapa

					Sessao.OcorrenciaAtiva.Latitude = pos.Latitude;
					Sessao.OcorrenciaAtiva.Longitude = pos.Longitude;

					//CriarMapa (pos.Latitude, pos.Longitude);
					CriarMapa ();
				
					//armazena as coordenadas apuradas
					//Sessao.OcorrenciaAtiva.Latitude = pos.Latitude;
					//Sessao.OcorrenciaAtiva.Longitude = pos.Longitude;
				}

			} 
			catch (Exception ex) 
			{
				DisplayLog (string.Format("{0} : {1}", "IniciarGPS", ex.Message));
				oGeolocator.StopListening (); 

			}finally{
				IsBusy = false;
				oGeolocator.StopListening (); 
			}

		}

		protected async Task ObtemEnderecoPorCoordenada()
		{
			try
			{
				IsBusy = true;

				GoogleGeoCodeResponse enderecoRetorno = await Sessao.PegaEnderecoPorCoordenadas (Sessao.OcorrenciaAtiva.Latitude.ToString(), Sessao.OcorrenciaAtiva.Longitude.ToString());

				if (enderecoRetorno != null)
					AtribuiEndereco(enderecoRetorno);
			}
			catch (Exception ex)
			{
				DisplayLog (string.Format("{0} : {1}", "ObtemEnderecoPorCoordenada", ex.Message));

			}finally{
				IsBusy = false;
			}
		}

		protected void AtribuiEndereco(GoogleGeoCodeResponse enderecoRetorno)
		{

			try 
			{

				if(!AnalisaStatusAPIGoogle(enderecoRetorno.status))
				{
					Application.Current.MainPage = new NavigationPage(new StatusConexao());
					return;
				}
				else
				{
					
					//preenche o objeto ocorrência de acordo com o endereço encontrado
					Sessao.OcorrenciaAtiva.Endereco = enderecoRetorno.results [0].address_components [1].long_name;
					Sessao.OcorrenciaAtiva.Estado = enderecoRetorno.results [0].address_components [5].short_name;

					if(!string.IsNullOrEmpty(entCEP.Text))
						Sessao.OcorrenciaAtiva.Cep = entCEP.Text;
					else
						Sessao.OcorrenciaAtiva.Cep = enderecoRetorno.results [0].address_components [7].long_name;

					Sessao.OcorrenciaAtiva.Cidade = enderecoRetorno.results [0].address_components [3].long_name; 
					Sessao.OcorrenciaAtiva.Bairro = enderecoRetorno.results [0].address_components [2].long_name;
					Sessao.OcorrenciaAtiva.Data = DateTime.UtcNow;
					Sessao.OcorrenciaAtiva.LocalizacaoReconhecida = true;
					
					Sessao.OcorrenciaAtiva.EnderecoFormatado = string.Format ("{0}, {1} - {2}/{3}",
						enderecoRetorno.results [0].address_components [1].long_name,
						enderecoRetorno.results [0].address_components [2].long_name,
						enderecoRetorno.results [0].address_components [3].long_name,
						enderecoRetorno.results [0].address_components [5].short_name);


					lblEndereco.Text = Sessao.OcorrenciaAtiva.Endereco;
					lblCidade.Text = Sessao.OcorrenciaAtiva.Cidade;
					lblCEP.Text = Sessao.OcorrenciaAtiva.Cep;
				}

			}
			catch (Exception ex) 
			{
				DisplayLog (string.Format("{0} : {1} - {2}", enderecoRetorno.status, "AtribuirEndereco: ", ex.Message));
			}
		}


		void AtribuiCEPClick()
		{

			try {

				var tapGestureRecognizer = new TapGestureRecognizer ();
				
				tapGestureRecognizer.Tapped += async (object sender, EventArgs e) => 
				{
					await ObtemEnderecoPorCEP ();
				};
				
				//adiciona o evento ao controle de interface grafica
				btSeletorCEP.GestureRecognizers.Add (tapGestureRecognizer);
			}
			catch (Exception ex)
			{
				DisplayLog (string.Format("AtribuiuCEPClick {0}", ex.Message));
			}

		}

		protected async Task ObtemEnderecoPorCEP(string zipcode = "")
		{
			IsBusy = true;

			try 
			{
				if(!string.IsNullOrEmpty(zipcode))
				
					entCEP.Text = zipcode;

					if(!string.IsNullOrEmpty(entCEP.Text.ToString()))
					{

						string cep =  Regex.Replace (entCEP.Text, "[^0-9]", "");

						if(!string.IsNullOrEmpty(cep)  || cep.Length >= 7)
						{
							//carrega o endereço pelo cep?
							GoogleGeoCodeResponse enderecoRetorno = await Sessao.PegaEnderecoPorCEP (cep);

								
							//ao escolher a lista temos que atribuir a latitude e longitude ao objeto de ocorrencia
								
							if(enderecoRetorno!=null)
							{

								Sessao.GeoReturn = enderecoRetorno;

								string lat = enderecoRetorno.results[0].geometry.location.lat;
								string lng = enderecoRetorno.results[0].geometry.location.lng;

								if((string.IsNullOrEmpty(lat) || lat.Equals("0")) || (string.IsNullOrEmpty(lng)|| lng.Equals("0"))){
									await DisplayAlert("TáVazando!", "Coordenadas não encontradas. Informe um CEP inválido.", "Ok");
									return;
								}

								//faz uma nova chamada a API para pegar os endereços nesse cep
								GoogleGeoCodeResponse  endereco = await Sessao.PegaEnderecoPorCoordenadas(lat, lng);

								if(endereco!=null)
								{
									//preenche o objeto ocorrência de acordo com o endereço encontrado
									AtribuiEndereco(endereco);

//									double latitude = Sessao.ConvertCoordenate(enderecoRetorno.results[0].geometry.location.lat);//enderecoRetorno.results[0].geometry.location.lat.IndexOf(",") >-1 ? Convert.ToDouble(enderecoRetorno.results[0].geometry.location.lat.Replace(",",".")): Convert.ToDouble(enderecoRetorno.results[0].geometry.location.lat);
//									double longitude = Sessao.ConvertCoordenate(enderecoRetorno.results[0].geometry.location.lng);//enderecoRetorno.results[0].geometry.location.lng.IndexOf(",") >-1 ? Convert.ToDouble(enderecoRetorno.results[0].geometry.location.lng.Replace(",",".")): Convert.ToDouble(enderecoRetorno.results[0].geometry.location.lng);

								Sessao.OcorrenciaAtiva.Latitude = Sessao.ConvertCoordenate(enderecoRetorno.results[0].geometry.location.lat); // latitude;
								Sessao.OcorrenciaAtiva.Longitude = Sessao.ConvertCoordenate(enderecoRetorno.results[0].geometry.location.lng); //longitude;
								Sessao.OcorrenciaAtiva.CepCorrigido = cep;

								CriarMapa();

							}
							else
							{
								await DisplayAlert("TáVazando!", "Nenhum endereço encontrado", "Ok");
							}
						}
						else
						{
							await DisplayAlert("TáVazando!", "Nenhum endereço encontrado", "Ok");
						}
					}
					else
					{
						await DisplayAlert("TáVazando!", "Informe um CEP para pesquisar", "Ok");
					}
				}
			} 
			catch (Exception ex) 
			{
				DisplayLog (string.Format("{0} : {1}", "ObtemEnderecoPorCEP", ex.Message));
			}
			finally
			{
				IsBusy = false;
			}
		}


		protected bool AnalisaStatusAPIGoogle(string status){

			try 
			{
				if (status.Equals ("REQUEST_DENIED")) 
				{
					DisplayLog (string.Format ("{0} : {1}", "AnalisaStatusAPIGoogle", status));
					return false;
				}

			} catch (Exception ex) {
				DisplayLog (string.Format("{0} : {1}", "AnalisaStatusAPIGoogle", ex.Message));
			}
			return true;
		}

		protected override void OnDisappearing ()
		{
			try {

				AtribuiValores();

			} catch (Exception ex) {
				DisplayLog (string.Format("{0} : {1}", "OnDisappearing", ex.Message));

			}

		}


		protected void AtribuiValores(){
			
			try {
				
				if (!string.IsNullOrEmpty (entDescricao.Text))
					Sessao.OcorrenciaAtiva.Descricao = entDescricao.Text;
				
				if (!string.IsNullOrEmpty (Sessao.OcorrenciaAtiva.Endereco))
					lblEndereco.Text = Sessao.OcorrenciaAtiva.Endereco;
				
				if (!string.IsNullOrEmpty (Sessao.OcorrenciaAtiva.Cidade))
					lblCidade.Text = Sessao.OcorrenciaAtiva.Cidade;
				
				if (!string.IsNullOrEmpty (Sessao.OcorrenciaAtiva.Cep))
					lblCEP.Text = Sessao.OcorrenciaAtiva.Cep;

				if (Sessao.OcorrenciaAtiva.TipoOcorrencia.Id > 0)
					lblDescricaoOcorrencia.Text = Sessao.OcorrenciaAtiva.TipoOcorrencia.Descricao;

				if (!string.IsNullOrEmpty (Sessao.OcorrenciaAtiva.Descricao))
					entDescricao.Text = Sessao.OcorrenciaAtiva.Descricao;

				if (!string.IsNullOrEmpty (Sessao.OcorrenciaAtiva.CepCorrigido))
					entCEP.Text = Sessao.OcorrenciaAtiva.CepCorrigido;


				if(Sessao.OcorrenciaAtiva.Image!=null)
					ApplyPhoto();

			}
			catch (Exception ex) 
			{
				DisplayLog (string.Format("{0} : {1}", "AtribuiValores", ex.Message));

			}
			
		}

		protected override void OnAppearing ()
		{

			try
			{
				AtribuiValores();

			//	Sessao.GSM_SIGNAL_STRENGTH = DependencyService.Get<IGSMSignal> ().SignalStrength;

			//	Signal.Text = "Força do sinal wifi: " +  Sessao.GSM_SIGNAL_STRENGTH.ToString();
//
//				if (Sessao.GSM_SIGNAL_STRENGTH == 0 || Sessao.GSM_SIGNAL_STRENGTH <= 5) {
//					Sessao.ConexaoInfo = "Sinal de conexão muito baixo. Procure um local com mais potência de sinal e tente novamente.";
//					Application.Current.MainPage = new NavigationPage(new StatusConexao());
//					return;
//				}


			}
			catch(Exception ex)
			{
				DisplayLog (string.Format("{0} : {1}", "OnAppearing", ex.Message));
			}
		}


		public async void PublicarOcorrencia(object sender, EventArgs e)
		{

			IsBusy = true;

			if(validarDados())
			{
				try 
				{
					if(!NeedShowMegaLog)
					{
						string retorno = await Sessao.Publicar (Sessao.OcorrenciaAtiva);
						await DisplayAlert("TáVazando", retorno, "Fechar");
						resetApp();
					}
					else
					{
						ShowMegaLog();
					}
				}
				catch (Exception ex) 
				{
					DisplayLog (string.Format("{0} : {1}", "PublicarOcorrencia", ex.Message));
				}
				finally
				{
					IsBusy = false;
				}
			}
		}

		protected async void resetApp(){
			try {

				IsBusy=true;

				lblEndereco.Text =  string.Empty;
				entCEP.Text = string.Empty;
				lblCidade.Text = string.Empty;
				lblDescricaoOcorrencia.Text = "ESCOLHA UMA CATEGORIA";
				entDescricao.Text = "";
				entDescricao.Placeholder = "DESCREVA O PROBLEMA";
				entCEP.Text="";
				entCEP.Placeholder = "Clique aqui para corrigir o CEP";
				lblCEP.Text = "CEP";
				//containerCEP.IsVisible = false;

				
				//limpa as propriedades do objeto de sessão
				Sessao.OcorrenciaAtiva.Estado = "";
				Sessao.OcorrenciaAtiva.Cidade = "";
				Sessao.OcorrenciaAtiva.Cep = "";
				Sessao.OcorrenciaAtiva.Descricao = "";
				Sessao.OcorrenciaAtiva.Endereco = "";
				Sessao.OcorrenciaAtiva.EnderecoFormatado = "";
				Sessao.OcorrenciaAtiva.Numero = "";
				Sessao.OcorrenciaAtiva.TipoOcorrencia = new TipoOcorrencia ();
				Sessao.OcorrenciaAtiva.CaminhoImage="";
				Sessao.OcorrenciaAtiva.Image=null;
				Sessao.OcorrenciaAtiva.NomeImagem="";


				if(Device.OS == TargetPlatform.Android)
					DependencyService.Get<IAndroidMethods>().CloseApp();


			} catch (Exception ex) {
				DisplayLog (string.Format("{0} : {1}", "resetApp", ex.Message));

			}finally{
				IsBusy = false;
			}
		}

		protected bool validarDados(){

			try {
				
				if (string.IsNullOrEmpty (entDescricao.Text))
					Sessao.OcorrenciaAtiva.Descricao = "n/i";
				else
					Sessao.OcorrenciaAtiva.Descricao = entDescricao.Text;
				
				if (string.IsNullOrEmpty (lblEndereco.Text))
					Sessao.OcorrenciaAtiva.Endereco = "n/i";
				else
					Sessao.OcorrenciaAtiva.Endereco = lblEndereco.Text;

				if (string.IsNullOrEmpty (Sessao.OcorrenciaAtiva.Cep) && (string.IsNullOrEmpty(Sessao.OcorrenciaAtiva.CepCorrigido))){
					DisplayAlert ("TáVazando", "Informe um CEP", "Ok").Wait (1000);
					return false;
				}

				if (string.IsNullOrEmpty (Sessao.OcorrenciaAtiva.Cep) && string.IsNullOrEmpty(Sessao.OcorrenciaAtiva.CepCorrigido))
				{
					Sessao.OcorrenciaAtiva.Cep = "0";
				}
				else
				{
					if(!string.IsNullOrEmpty(Sessao.OcorrenciaAtiva.CepCorrigido))
					{
						string cepCorrigido = Regex.Replace (Sessao.OcorrenciaAtiva.CepCorrigido, "[^0-9]", "");

						if(!string.IsNullOrEmpty(cepCorrigido)){
							Sessao.OcorrenciaAtiva.Cep =  Sessao.OcorrenciaAtiva.CepCorrigido;
						}else{
							Sessao.OcorrenciaAtiva.Cep =  Sessao.OcorrenciaAtiva.Cep;
						}
					}
					else if(string.IsNullOrEmpty (Sessao.OcorrenciaAtiva.Cep))
					{
						Sessao.OcorrenciaAtiva.Cep =  Sessao.OcorrenciaAtiva.Cep;
					}
				}
				
				if (string.IsNullOrEmpty (lblCidade.Text))
					Sessao.OcorrenciaAtiva.Cidade = "n/i";
				else 
					Sessao.OcorrenciaAtiva.Cidade = lblCidade.Text;
				
				if (Sessao.OcorrenciaAtiva.TipoOcorrencia.Id == 0) {
					DisplayAlert ("TáVazando", "Escolha o tipo de ocorrência", "Ok").Wait (1000);
					return false;
				}

			} catch (Exception ex) {
				DisplayLog (string.Format("{0} : {1}", "validarDados", ex.Message));

			} 
			return true;
		}

		protected async void DisplayLog(string msg){
			await DisplayAlert("TáVazando - Erro", msg, "Reportar");
		}

		protected void cepCorrigidoChange(object sender, TextChangedEventArgs e){
			Sessao.OcorrenciaAtiva.CepCorrigido = e.NewTextValue;
		}

		protected void ShowMegaLog()
		{
			XLabs.Forms.Controls.PopupLayout _PopUpLayoutCEP = new XLabs.Forms.Controls.PopupLayout ();

			var oGeolocator = Resolver.Resolve<IGeolocator> ();

			List<Label> items = new List<Label>();

			items.Add (new Label () {
				Style = (Style)Application.Current.Resources ["regular_text"],
				Text = string.Format ("Rede OK? >> {0} ", Sessao.onLine)
			});

			items.Add (new Label(){ Style = (Style)Application.Current.Resources ["regular_text"], Text = string.Format("GPS? >> {0} ", oGeolocator.IsGeolocationAvailable) });
			items.Add (new Label(){ Style = (Style)Application.Current.Resources ["regular_text"], Text = string.Format("Ocorrencia: {0} - {1}", Sessao.OcorrenciaAtiva.TipoOcorrencia.Id, Sessao.OcorrenciaAtiva.TipoOcorrencia.Descricao) });
			items.Add (new Label(){ Style = (Style)Application.Current.Resources ["regular_text"], Text = string.Format("Descrição: {0} ", Sessao.OcorrenciaAtiva.Descricao) });
			items.Add (new Label(){ Style = (Style)Application.Current.Resources ["regular_text"], Text = string.Format("Endereço: {0} ", Sessao.OcorrenciaAtiva.EnderecoFormatado) });
			items.Add (new Label(){ Style = (Style)Application.Current.Resources ["regular_text"], Text = string.Format("Cidade: {0} ", Sessao.OcorrenciaAtiva.Cidade) });
			items.Add (new Label(){ Style = (Style)Application.Current.Resources ["regular_text"], Text = string.Format("CEP: {0} ", Sessao.OcorrenciaAtiva.Cep) });
			items.Add (new Label(){ Style = (Style)Application.Current.Resources ["regular_text"], Text = string.Format("CEP corrigido: {0} ", Sessao.OcorrenciaAtiva.CepCorrigido) });
			items.Add (new Label(){ Style = (Style)Application.Current.Resources ["regular_text"], Text = string.Format("Imagem: {0} ", Sessao.OcorrenciaAtiva.NomeImagem) });
			items.Add (new Label(){ Style = (Style)Application.Current.Resources ["regular_text"], Text = string.Format("Latitude: {0} ", Sessao.OcorrenciaAtiva.Latitude.ToString()) });
			items.Add (new Label(){ Style = (Style)Application.Current.Resources ["regular_text"], Text = string.Format("Longitude: {0} ", Sessao.OcorrenciaAtiva.Longitude.ToString()) });

			if(Sessao.GeoReturn!=null)
			{
				items.Add (new Label () {
					Style = (Style)Application.Current.Resources ["regular_text"],
					Text = string.Format ("Registros: {0}", Sessao.GeoReturn.results.Length.ToString ())
				});
					items.Add (new Label () {
					Style = (Style)Application.Current.Resources ["regular_text"],
					Text = string.Format ("Reg Lat: {0}", Sessao.GeoReturn.results [0].geometry.location.lat.ToString ())
				});
				items.Add (new Label () {
					Style = (Style)Application.Current.Resources ["regular_text"],
					Text = string.Format ("Reg Lng: {0}", Sessao.GeoReturn.results [0].geometry.location.lng.ToString ())
				});
				items.Add (new Label () {
					Style = (Style)Application.Current.Resources ["regular_text"],
					Text = string.Format ("Reg bds ne lat: {0}", Sessao.GeoReturn.results [0].geometry.bounds.northeast.lat.ToString ())
				});
				items.Add (new Label () {
						Style = (Style)Application.Current.Resources ["regular_text"],
						Text = string.Format ("Reg bds ne lng: {0}", Sessao.GeoReturn.results [0].geometry.bounds.northeast.lng.ToString ())
				});
				items.Add (new Label () {
					Style = (Style)Application.Current.Resources ["regular_text"],
					Text = string.Format ("Reg bds sw lat: {0}", Sessao.GeoReturn.results [0].geometry.bounds.southwest.lat.ToString ())
				});
				items.Add (new Label () {
					Style = (Style)Application.Current.Resources ["regular_text"],
					Text = string.Format ("Reg bds sw lng: {0}", Sessao.GeoReturn.results [0].geometry.bounds.southwest.lng.ToString ())
				});
				items.Add (new Label () {
					Style = (Style)Application.Current.Resources ["regular_text"],
					Text = string.Format ("Reg view ne lat: {0}", Sessao.GeoReturn.results [0].geometry.viewport.northeast.lat.ToString ())
				});
				items.Add (new Label () {
					Style = (Style)Application.Current.Resources ["regular_text"],
					Text = string.Format ("Reg view ne lng: {0}", Sessao.GeoReturn.results [0].geometry.viewport.northeast.lng.ToString ())
				});
				items.Add (new Label () {
					Style = (Style)Application.Current.Resources ["regular_text"],
					Text = string.Format ("Reg view sw lat: {0}", Sessao.GeoReturn.results [0].geometry.viewport.southwest.lat.ToString ())
				});
				items.Add (new Label () {
					Style = (Style)Application.Current.Resources ["regular_text"],
					Text = string.Format ("Reg view sw lng: {0}", Sessao.GeoReturn.results [0].geometry.viewport.southwest.lng.ToString ())
				});
			}

			StackLayout header = new StackLayout ();
			header.Style = (Style)Application.Current.Resources ["dynamic_toolbar"];
			header.Padding = 10;
			header.Spacing = 10;
			header.Orientation = StackOrientation.Horizontal;
			header.Children.Add (new Image (){ Style = (Style)Application.Current.Resources ["toolbar_icon"] });
			header.Children.Add (new Label () {
				Style = (Style)Application.Current.Resources ["toolbar_text"],
				Text = "MEGA-HIPER-DARTH-VADER-LOG"
			});

			StackLayout subPop = new StackLayout{
				WidthRequest = this.Width, // Important, the Popup hast to have a size to be showed
				VerticalOptions = LayoutOptions.FillAndExpand,
				BackgroundColor = Color.White, // for Android and WP
				Orientation = StackOrientation.Vertical,
				Padding = 20,
				Spacing = 10
			};


			foreach(Label l in items){
				if(!string.IsNullOrEmpty(l.Text))
					subPop.Children.Add(l);
			}

			ScrollView s = new ScrollView {
				HorizontalOptions  = LayoutOptions.FillAndExpand
			};
			s.Content = subPop;


			StackLayout PopUp = new StackLayout {
				WidthRequest = this.Width, // Important, the Popup hast to have a size to be showed
				HeightRequest = this.Height,
				BackgroundColor = Color.White, // for Android and WP
				Orientation = StackOrientation.Vertical,
				Padding = 0,
				Spacing = 20,
				Children = {
					header,
					s
				}
			};


			

			ctrlPopup.Children.Add (s);
			ctrlPopup.IsVisible = true;
			btnPublicar.IsVisible = false;
			controls.IsVisible = false;

			_PopUpLayoutCEP.Content = ctrlPopup; 
			_PopUpLayoutCEP.ShowPopup (PopUp);


		}




	}



}

