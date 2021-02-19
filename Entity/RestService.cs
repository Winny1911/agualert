using System;
using PortableRest;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Net.Http.Headers;
using Xamarin.Forms;


namespace TaVazando
{
	public class RestService: PortableRest.RestClient
	{
		public const string SERVER_URI = "";
		public const string APP_ID = "de8291e8-5443-4897-a9e2-e1b74296baf1"; // ID DO APP (NUNCA MUDA)
		public const string APP_SECRET = "a5bbf957-4450-4371-97f2-d49beec4112e"; // SECRET/SENHA NUNCA MUDA
		public const string FIELD_NI = "Não informado";
		//nova chave de teste
		public const string GEOCODE_API_SERVER_KEY = "AIzaSyD9EGfs_fhJ0bj4ZrifM8KCPbg8yLoHrxk";


		string resource { get; set; }
		string param1;
		string param2;
		bool bDesabilitarFiltros;

		public RestService (string resource, string endpoint,string server_uri="", string param1="", string param2="", bool desabilitarFiltros = false)
		{ 

			try
			{

				if(!string.IsNullOrEmpty(server_uri))
					BaseUrl = string.Format("{0}{1}", server_uri, endpoint);
				else
					BaseUrl = string.Format("{0}{1}", SERVER_URI, endpoint);

				this.resource = resource;

				if(param1.IndexOf(",")>-1)
					this.param1 = double.Parse(param1).ToString().Replace(",",".");
				else
					this.param1 = param1;


				if(param2.IndexOf(",")>-1)
					this.param2 = double.Parse(param2).ToString().Replace(",",".");
				else
					this.param2 = param2;

				this.bDesabilitarFiltros = desabilitarFiltros;

			}
			catch(Exception ex){
				throw new Exception (string.Format("{0} : {1}", "RestService main", ex.Message));
			}
		}


		public async Task<T> PegaEnderecoGeoReversoPorCoordenada<T>() where T: class
		{
			try
			{
				var request = new RestRequest(resource, HttpMethod.Get, ContentTypes.Json);

				request.AddQueryString("latlng", string.Format("{0},{1}", this.param1, this.param2));

				if(!bDesabilitarFiltros){
					request.AddQueryString("location_type", "ROOFTOP");
					request.AddQueryString("result_type", "street_address");
				}

				request.AddQueryString("key", GEOCODE_API_SERVER_KEY);

				var results = await ExecuteAsync<T>(request);
				return results;

			}catch(Exception ex)
			{
				throw new Exception (string.Format("{0} : {1}", "PegaEnderecoGeoReversoPorCoordenada", ex.Message));

			}
			return null;
		}

		public async Task<T> PegaEnderecoGeoReversoPorCEP<T>() where T: class
		{
			try
			{
				var request = new RestRequest(resource, HttpMethod.Get, ContentTypes.Json);
				request.AddQueryString("components", string.Format("country:BR|postal_code:{0}", this.param1));
				request.AddQueryString("key", GEOCODE_API_SERVER_KEY);

				var results = await ExecuteAsync<T>(request);
				return results;

			}catch(Exception er)
			{
				throw new Exception (er.Message);
			}
			return null;
		}

		public async Task<T> SolicitarToken<T>() where T: class
		{
			try
			{
				var hash =   System.Text.Encoding.UTF8.GetBytes(string.Format("{0}:{1}", APP_ID, APP_SECRET));

				var request = new RestRequest(resource, HttpMethod.Post, ContentTypes.FormUrlEncoded);
				request.AddHeader("authorization", string.Format("Basic {0}",  System.Convert.ToBase64String(hash)));
				request.AddParameter("grant_type","client_credentials");

				var results = await ExecuteAsync<T>(request);
				return results;

			}catch(Exception ex)
			{
				throw new Exception (string.Format("{0} : {1}", "SolicitarToken", ex.Message));

			}
			return null;
		}


		public async Task<T> EnviarOcorrencia<T>(Ocorrencia ocorrencia, string token) where T: class
		{
			try
			{

				if(ocorrencia==null)
					throw new Exception("Não foi possível ler o objeto ocorrẽncia");

				if(string.IsNullOrEmpty(token))
					throw new Exception("Token de segurança não informado");

				//trata os valores 
				string lat=string.Empty;
				string lng=string.Empty;
				string image_id=string.Empty;
				string type=string.Empty;
				string description=string.Empty;
				string address=string.Empty;
				string zipcode=string.Empty;
				string city=string.Empty;

				if(!string.IsNullOrEmpty(ocorrencia.TipoOcorrencia.Id.ToString()))
					type = ocorrencia.TipoOcorrencia.Id.ToString();
				else
					throw new Exception("Tipo de ocorrẽncia deve ser informado");

				if(!string.IsNullOrEmpty(ocorrencia.Descricao))
					description = ocorrencia.Descricao;
				else
					description = FIELD_NI;

				if(!string.IsNullOrEmpty(ocorrencia.EnderecoFormatado))
					address = ocorrencia.EnderecoFormatado;
				else
					address = FIELD_NI;

				if(!string.IsNullOrEmpty(ocorrencia.Cep))
					zipcode = ocorrencia.Cep;
				else
					zipcode = FIELD_NI;

				if(!string.IsNullOrEmpty(ocorrencia.Cidade))
					city = ocorrencia.Cidade;
				else
					city = FIELD_NI;

				if(!string.IsNullOrEmpty(ocorrencia.ImageId))
					image_id = ocorrencia.ImageId;
				else
					image_id = "0";

				if(ocorrencia.Latitude!= null )
				{
					if(ocorrencia.Latitude.ToString().IndexOf(",")>-1)
						lat = ocorrencia.Latitude.ToString().Replace(",",".");
					else
						lat = ocorrencia.Latitude.ToString();
				}else{
					lat = "0";
				}

				if(ocorrencia.Longitude != null)
				{
					if(ocorrencia.Longitude.ToString().IndexOf(",")>-1)
						lng = ocorrencia.Longitude.ToString().Replace(",",".");
					else
						lng = ocorrencia.Longitude.ToString();
				}else{
					lng = "0";
				}

				var request = new RestRequest(resource, HttpMethod.Post, ContentTypes.FormUrlEncoded);

//				request.AddHeader("authorization", string.Format("Bearer {0}", token));
//				request.AddParameter("tipo", ocorrencia.TipoOcorrencia.Id.ToString());
//				request.AddParameter("descricao", ocorrencia.Descricao);
//				request.AddParameter("endereco", ocorrencia.EnderecoFormatado);
//				request.AddParameter("cep", ocorrencia.Cep);
//				request.AddParameter("cidade", ocorrencia.Cidade);
//
//				if(!string.IsNullOrEmpty(ocorrencia.ImageId))
//					request.AddParameter("arquivos", ocorrencia.ImageId);
//				else
//					request.AddParameter("arquivos", string.Empty);
//
//				request.AddParameter("longitude", lng);
//				request.AddParameter("latitude", lat);

				request.AddHeader("authorization", string.Format("Bearer {0}", token));
				request.AddParameter("tipo", type);
				request.AddParameter("descricao", description);
				request.AddParameter("endereco", address);
				request.AddParameter("cep", zipcode);
				request.AddParameter("cidade", city);
				request.AddParameter("arquivos", image_id);
				request.AddParameter("longitude", lng);
				request.AddParameter("latitude", lat);

				var results = await ExecuteAsync<T>(request);
				return results;

			}
			catch(Exception ex)
			{
				throw new Exception (string.Format("{0} : {1}", "EnviarOcorrencia", ex.Message));
			}
		}


		public async Task<string> PostMultiPartForm(byte[] file, string paramName, string contentType, Dictionary<String, string> nvc,string cookie="", string token="",string filename="")
		{
			try
			{
				var result = DependencyService.Get<IRestfulFileUpload> ().SendFileToRest (file, filename, token, "https://srvtavazando-50601.onmodulus.net/", "api/v1/save-file", nvc);
				return result;
			}
			catch (Exception ex)
			{
				throw new Exception (string.Format("{0} : {1}", "PostMultiPartForm", ex.Message));
			}

		}

	}
}

