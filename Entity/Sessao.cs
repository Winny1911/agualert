using System;
using Connectivity.Plugin;
using System.Threading.Tasks;
using System.Collections.Generic;
using Media.Plugin;
using System.Threading;
using Xamarin.Forms;
using Geolocator.Plugin;
using XLabs.Ioc;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Xamarin.Forms.Maps;
using Newtonsoft.Json.Linq;
using System.Globalization;

namespace TaVazando
{
	public class Sessao
	{
		public Sessao ()
		{
			
		}

		public const string GOOGLE_API_BASE_URI = "https://maps.googleapis.com/maps/";
		public const string GOOGLE_API_END_POINT = "api/";
		public const string GOOGLE_API_RESOURCE = "geocode/json";

		public const string TAVAZANDO_API_END_POINT = "api/v1/";
		public const string TAVAZANDO_API_TOKEN_RESOURCE = "oauth2/token";
		public const string TAVAZANDO_API_SAVE_RESOURCE = "ocorrencia/save";
		public const string TAVAZANDO_SERVER_URI = "https://srvtavazando-50601.onmodulus.net/"; //passar ip do ws do tavazando
		public const string TAVAZANDO_API_SAVE_IMAGE = "save-file";

		public static string CurrentCultureInfo { get; set; }

		public static string ConexaoInfo { get; set; }
		public static int GSM_SIGNAL_STRENGTH { get; set; }

		public static string location_info { get; set; }
		public static GoogleGeoCodeResponse GeoReturn { get; set; }
		private static Ocorrencia ocorrencia_ativa = null;

		public static bool onLine {get;set;}
		public static List<TipoOcorrencia> Ocorrencias { get; set; }
		public static Ocorrencia OcorrenciaAtiva 
		{ 
			get
			{ 
				if(ocorrencia_ativa == null)
					ocorrencia_ativa = new Ocorrencia ();

				return ocorrencia_ativa;
			} 
			set 
			{
				ocorrencia_ativa = value;
			}
		}

		public static string GeoLocationErr{ get; set; }


		public static void GetCurrentCulture(){
			CurrentCultureInfo = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
		}


		public static async Task obtemConexao(){
			onLine = CrossConnectivity.Current.IsConnected;
		}

		public static async Task checaQualidadeSinal(){
			
		}

		public static void ObtemListaOcorrencias()
		{
			Ocorrencias = new List<TipoOcorrencia>()
			{
				new TipoOcorrencia(){  Id=1, Descricao="Vazamento de água"},
				new TipoOcorrencia(){  Id=2, Descricao="Falta d’água"},
				new TipoOcorrencia(){  Id=3, Descricao="Alteração na cor da água"},
				new TipoOcorrencia(){  Id=4, Descricao="Denúncia de ligações clandestinas"},
				new TipoOcorrencia(){  Id=5, Descricao="Mau cheiro de esgoto"},
				new TipoOcorrencia(){  Id=6, Descricao="Tampa de esgoto aberta/quebrada/ausente"},
				new TipoOcorrencia(){  Id=7, Descricao="Baixa/Alta pressão de água"},
			};
		}

		public static byte[] ConvertToBase64(Stream stream)
		{
			Byte[] inArray = new Byte[(int)stream.Length];
			Char[] outArray = new Char[(int)(stream.Length * 1.34)];
			stream.Read(inArray, 0, (int)stream.Length);
			Convert.ToBase64CharArray(inArray, 0, inArray.Length, outArray, 0);
			return System.Text.Encoding.UTF8.GetBytes(outArray);
		}

		public static async Task<GoogleGeoCodeResponse> PegaEnderecoPorCoordenadas (string latitude, string longitude, bool desabilitarfiltros = false)
		{
			//chama a api rest
			var result = await new RestService(
				GOOGLE_API_RESOURCE, 
				GOOGLE_API_END_POINT , 
				GOOGLE_API_BASE_URI, 
				latitude, 
				longitude, 
				desabilitarfiltros).PegaEnderecoGeoReversoPorCoordenada<GoogleGeoCodeResponse>();

			//testa o retorno
			if (result!=null){
				GoogleGeoCodeResponse address = (GoogleGeoCodeResponse)result;
				return address;
			}

			return null;

		}

		public static async Task<GoogleGeoCodeResponse> PegaEnderecoPorCEP (string cep)
		{

			try 
			{
				//limpa o cep removendo tudo que não for númerico
				cep = Regex.Replace (cep, "[^0-9]", "");
				
				string sCep = "";
				
				if (cep.IndexOf ("-") < 0)
					sCep = string.Format ("{0}-{1}", cep.Substring (0, 5), cep.Substring (5, 3));
				
				//chama a api rest
				var result = await new RestService (GOOGLE_API_RESOURCE, GOOGLE_API_END_POINT, GOOGLE_API_BASE_URI, sCep).PegaEnderecoGeoReversoPorCEP<GoogleGeoCodeResponse> ();
				
				//testa o retorno
				if (result != null) {
					//convert o retorno para o tipo esperado
					GoogleGeoCodeResponse addresses = (GoogleGeoCodeResponse)result;
					return addresses;
				}

			}
			catch (Exception ex) 
			{
				throw new Exception(string.Format("{0} : {1}", "PegaEnderecoPorCEP", ex.Message));

			}

			return null;

		}



		public static  async Task<string> Publicar(Ocorrencia ocorrencia)
		{
			
			string msg_return ="Não foi possível publicar sua ocorrẽncia. Tente novamente dentro de alguns minutos.";

			try 
			{
				string token = await RequestSecurityToken(); 
				ocorrencia.ImageId = await SendOccurencyPicture(token);

				var result = await new RestService (TAVAZANDO_API_SAVE_RESOURCE, TAVAZANDO_API_END_POINT, TAVAZANDO_SERVER_URI).EnviarOcorrencia<string>(ocorrencia, token);

				if(result!=null && Newtonsoft.Json.Linq.JObject.Parse(result)["message"].ToString().ToLower().Equals("ok"))
					msg_return= "Ocorrência publicada com sucesso. Obrigado por sua colaboração";
					

			}
			catch(Exception ex)
			{
				msg_return=  string.Format("{0} : {1} - {2}", "Publicar", ex.Message, ex.StackTrace);
			}


			return msg_return;

		}

		public static async Task<string> RequestSecurityToken()
		{

			try 
			{
				var token = await new RestService (TAVAZANDO_API_TOKEN_RESOURCE, TAVAZANDO_API_END_POINT, TAVAZANDO_SERVER_URI).SolicitarToken<string> ();

				if(token!=null && token.IndexOf("token")<=0)
					throw new Exception("Falha ao gerar token de segurança");

				return (string)Newtonsoft.Json.Linq.JObject.Parse(token)["access_token"]["token"].ToString();

			} 
			catch (Exception ex) 
			{
				throw new Exception( string.Format("{0} : {1} - {2}", "Publicar", ex.Message, ex.StackTrace));
			}

			return "";

		}

		public static async Task<string> SendOccurencyPicture(string token)
		{


			if (Sessao.OcorrenciaAtiva.Image == null)
				return "";

			string image_id = string.Empty;
			var memoryStream = new MemoryStream();

			try 
			{

				Dictionary<string, string> parameters = new Dictionary<string, string> ();
				parameters.Add ("Name", "arquivo");

				var stream = Sessao.OcorrenciaAtiva.Image.GetStream();
				stream.CopyTo(memoryStream);
				stream.Dispose();

				var photo_result = await new RestService ( 
					TAVAZANDO_API_SAVE_IMAGE, 
					TAVAZANDO_API_END_POINT, 
					TAVAZANDO_SERVER_URI, 
					param1:Sessao.OcorrenciaAtiva.NomeImagem).PostMultiPartForm(memoryStream.ToArray(), 
					"file", 
					"image/jpg", 
					parameters, 
					"cookie", 
					token, 
					Sessao.OcorrenciaAtiva.NomeImagem
				);

				if(photo_result!=null  && photo_result.IndexOf("_id")>-1)
					image_id = JArray.Parse(photo_result)[0]["_id"].ToString();

				return image_id;


			} 
			catch(Exception ex)
			{
				throw new Exception(string.Format("{0} : {1}", "Publicar Foto", ex.Message));
			}
			finally 
			{
				if(memoryStream!=null)
					memoryStream.Dispose();
			}

			return "";
		}


		public static double ConvertCoordenate(string coordinate)
		{
			double converted = 0;


			if(CurrentCultureInfo == "pt")
				converted = Convert.ToDouble(coordinate.Replace(".",","));
			else
				converted = Convert.ToDouble(coordinate.Replace(",","."));

//			if(coordinate.IndexOf(".") >-1)
//				converted = Convert.ToDouble(coordinate.Replace(".",","));
//			else if(coordinate.IndexOf(",") >-1)
//				converted = Convert.ToDouble(coordinate.Replace(",","."));
			
			return converted; //coordinate.IndexOf(".") >-1 ? Convert.ToDouble(coordinate.Replace(".",",")): Convert.ToDouble(coordinate);
		}
	}

}
