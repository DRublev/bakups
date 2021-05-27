using System;
using System.Configuration;
using System.Diagnostics;

using MongoDB.Driver;

namespace SPT
{
	public class DBConnector
    {
		private string connectionString = ConfigurationManager.ConnectionStrings["MongoDB"].ConnectionString;
		public MongoClient Client = null;

		public const string DB_NAME = "spt";
				
		/* TODO: 
		 * Возможность подключения к нескольким бд
		 * Каждый коннект заносится в лист 
		 * При вызове деконструктора коннекты закрываются и лист очищается
		 */

		public DBConnector()
		{
			OpenConnection();
		}

		~DBConnector()
		{
			CloseConnection();
		}

		public void OpenConnection()
		{
			try
			{
				MongoClient client = new MongoClient(connectionString);

				Debug.WriteLine($"MongoDB connection opend to {client.Settings.IPv6}");

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
		public void CloseConnection()
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
