using System;
using System.Configuration;
using System.Diagnostics;

using MongoDB.Driver;
using System.Collections.Generic;
using MongoDB.Bson;
using System.Linq;
using System.Windows;

namespace SPT
{
	public class DBConnector
    {
		/// <summary>
		/// URL for connection to database
		/// </summary>
		private string connectionString = ConfigurationManager.ConnectionStrings["MongoDB"].ConnectionString;
		public MongoClient Client = null;

		public const string DB_NAME = "spt";

		public DBConnector()
		{
			OpenConnection();
		}
		~DBConnector()
		{
			CloseConnection();
		}

		/// <summary>
		/// Open connection to databsase via Client
		/// </summary>
		private void OpenConnection()
		{
			try
			{
				MongoClient client = new MongoClient(connectionString);

				Debug.WriteLine($"MongoDB connection opend to {client.Settings.Server.Host}");

				Client = client;
			}
			catch (Exception ex)
			{
				Debug.Fail($"Error connecting to database: {ex.Message}");
			}
		}

		/// <summary>
		/// Set Client = null
		/// </summary>
		private void CloseConnection()
		{
			Client = null;
		}

		/// <summary>
		/// Find and return database by name in MongoDB
		/// </summary>
		/// <param name="databaseName"></param>
		/// <returns>BsonDocument - database</returns>
		public IMongoDatabase GetDatabase(string databaseName = DB_NAME)
		{
			IMongoDatabase database = null;
			try
			{
				database = Client.GetDatabase(databaseName);
                
			}
			catch(Exception ex)
			{
				Debug.Fail($"Couldn't find database: {ex.Message}");
			}

			return database;
		}
    }
}
