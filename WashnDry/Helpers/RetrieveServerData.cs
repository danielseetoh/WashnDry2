// This code requires the Nuget package Microsoft.AspNet.WebApi.Client to be installed.
// Instructions for doing this in Visual Studio:
// Tools -> Nuget Package Manager -> Package Manager Console
// Install-Package Microsoft.AspNet.WebApi.Client

using System;
using System.Collections.Generic;
using System.IO;
using System.Json;
using System.Net;
//using System.Net.Http;
//using System.Net.Http.Formatting;
//using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using Newtonsoft.Json;


namespace WashnDry
{

	public class StringTable
	{
		public string[] ColumnNames { get; set; }
		public string[,] Values { get; set; }
	}

	public class RetrieveServerData
	{

		public static async Task<JsonValue> InvokeRequestResponseService(string[,] weatherData)
		{
			using (var client = new HttpClient())
			{
				//var a = { 
				//	{ "23.544770086977", "95.4807868863463", "0.968145559172742", "23.2511716372802", "3205.22612103135" }, 
				//	{ "0", "0", "0", "0", "0" }, 
				//};

				var scoreRequest = new
				{
					Inputs = new Dictionary<string, StringTable>() {
						{
							"input1",
							new StringTable()
							{
								ColumnNames = new string[] {"Temperature", "Humidity", "Precipitation", "Windspeed", "Drying-time"},
								Values = weatherData
							}
						},
					},
					GlobalParameters = new Dictionary<string, string>()
					{
					}
				};

				const string apiKey = "br2EeDUMna4U7JPg8buaua6qTv11Wo/9Q2DQBL/uftnVMf8hEcNuCi8lGuNb7XslXohqdeuLVX1vC5uoqiX35Q=="; // untrained
				//const string apiKey = "LBH46mrE6Pk7QBAApDmA5z7nF9w2vpiSWmZHIg7Ma9ZKVcsMWkBXZHCU6qw3VRRi9i2HI5HIk5qPIrBBf+ndgw=="; // trained

				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

				client.BaseAddress = new Uri("https://ussouthcentral.services.azureml.net/workspaces/24adfa57a81e45f880ce53d63175dbbd/services/b070ee7c4c4447c4bdf6ada94da92a11/execute?api-version=2.0&details=true");// untrained
				//client.BaseAddress = new Uri("https://ussouthcentral.services.azureml.net/workspaces/24adfa57a81e45f880ce53d63175dbbd/services/a7b87c56c59049e58267a5f6c41293df/execute?api-version=2.0&details=true");// trained

				// WARNING: The 'await' statement below can result in a deadlock if you are calling this code from the UI thread of an ASP.Net application.
				// One way to address this would be to call ConfigureAwait(false) so that the execution does not attempt to resume on the original context.
				// For instance, replace code such as:
				//      result = await DoSomeTask()
				// with the following:
				//      result = await DoSomeTask().ConfigureAwait(false)


				//HttpResponseMessage response = await client.PostAsJsonAsync("", scoreRequest);
				string json = JsonConvert.SerializeObject(scoreRequest);
				var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
				var response = await client.PostAsync("", stringContent);
				if (response.IsSuccessStatusCode)
				{
					Stream stream = await response.Content.ReadAsStreamAsync();

					// Use this stream to build a JSON document object:
					JsonValue jsonDoc = await Task.Run(() => JsonValue.Load(stream));
					//Console.Out.WriteLine("Response: {0}", jsonDoc.ToString());
					// Return the JSON document:
					return jsonDoc;

					//string result = await response.Content.ReadAsStringAsync();
					//Console.WriteLine("Result: {0}", result);
				}
				else
				{
					Console.WriteLine(string.Format("The request failed with status code: {0}", response.StatusCode));

					// Print the headers - they include the requert ID and the timestamp, which are useful for debugging the failure
					Console.WriteLine(response.Headers.ToString());

					string responseContent = await response.Content.ReadAsStringAsync();
					Console.WriteLine(responseContent);

					return null;
				}
			}
		}

	}
}




