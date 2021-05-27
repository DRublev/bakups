using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace SPT.Models
{
	public abstract class Model
	{
		/// <summary>
		/// Table/collection name in database (Model name in lower case + 's').
		/// </summary>
		public string DatabaseTableName
		{
			get
			{
				string name = GetType().Name;
				string tableName = name.ToLower() + 's';

				return tableName;
			}
		}

		/// <summary>
		/// Get data from database
		/// </summary>
		public virtual List<T> GetData<T>()
		{
			var collection = GetMongoCollection<T>();

			return collection.Find(_ => true).ToList();
		}

		public IMongoCollection<T> GetMongoCollection<T>()
		{
			// TODO: Change on MondoDBConnector and refactor DBConnector
			DBConnector connector = new DBConnector();

			var database = connector.GetDatabase();

			string collectionName = DatabaseTableName;

			IMongoCollection<T> collection = null;

			try
			{
				collection = database.GetCollection<T>(collectionName);
			}
			catch (Exception ex)
			{
				Debug.Fail($"Error getting collection: {ex.Message}");
			}

			return collection;
		}
	}
}