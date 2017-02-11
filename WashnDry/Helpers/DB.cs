using System;
using System.Collections.Generic;
using System.Json;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using DT = System.Data;            // System.Data.dll  
using QC = System.Data.SqlClient;


namespace WashnDry
{
	public static class DB
	{
		
		public enum sql { selectq, insert, update, delete}


		public static dynamic DBOperation(sql type, string sqlString)
		{
			string server = "mmpl4w8m2p.database.windows.net";
			string dbName = "seconddb";
			string user = "seconddb@mmpl4w8m2p";
			string pw = "Wash&dry123";

			string connsqlstring = string.Format("Server=tcp:" + server + ";Database=" + dbName + ";User ID=" + user + ";Password=" + pw + ";Integrated Security=False;Connection Timeout=30;");
			using (var sqlconn = new QC.SqlConnection(connsqlstring))
			{
				sqlconn.Open();
				Console.WriteLine("I Connected successfully....");

				var command = new QC.SqlCommand();
				command.Connection = sqlconn;
				command.CommandType = DT.CommandType.Text;
				command.CommandText = @"" + sqlString;

				if (type == sql.selectq)
				{
					QC.SqlDataReader r = command.ExecuteReader();
					Console.WriteLine("Inside the returning fo the reader");
					return r;
				}
				else {
					command.ExecuteScalar();
					return null;
				}
			}




			//QC.SqlConnection sqlconn = new QC.SqlConnection(connsqlstring);

		}

	}
}	