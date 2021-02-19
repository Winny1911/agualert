using System;
using System.Collections.Generic;

namespace TaVazando
{
	public class Ocorrencia
	{
		public Ocorrencia (){

			TipoOcorrencia = new TipoOcorrencia ();
		}
		public TipoOcorrencia TipoOcorrencia	{get;set;}
		public DateTime Data 	{get;set;}
		public string Endereco	{get;set;}
		public string Cep	{get;set;}
		public string Cidade	{get;set;}
		public string Bairro	{get;set;}
		public string Estado	{get;set;}
		public double Latitude	{get;set;}
		public double Longitude	{get;set;}
		public int Status {get;set;}
		public string CaminhoImage {get;set;}
		public string NomeImagem {get;set;}
		public string ImageId {get;set;}
		public bool LocalizacaoReconhecida {get;set;}
		public string EnderecoFormatado {get;set;}
		public string Numero{get;set;}
		public string Descricao {get;set;}
		public Media.Plugin.Abstractions.MediaFile Image {get;set;}
		public string CepCorrigido { get; set; }

		public List<GoogleGeoCodeResponse> EnderecosColetados {get;set;}
	}

	public class TipoOcorrencia
	{
		public TipoOcorrencia(){
		}
		public int Id {get;set;}
		public string Descricao {get;set;}
	}
}

