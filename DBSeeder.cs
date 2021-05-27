using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MongoDB.Bson;
using MongoDB.Driver;
using SPT.Models;

namespace SPT.DatabaseWorking
{
	/// <summary>
	/// Managing collections in database
	/// </summary>
	public class DBSeeder
	{
		private static DBSeeder instance;
		public static DBSeeder Instance
		{
			get
			{
				if(instance == null)
				{
					instance = new DBSeeder();
				}

				return instance;
			}
		}

		private DBConnector Connector
		{
			get
			{
				return new DBConnector();
			}
		}

		

		/// <summary>
		/// All the clases that have Model as base class
		/// </summary>
		private List<TypeInfo> AllModels
		{
			get
			{
				Assembly currentAssembly = Assembly.GetExecutingAssembly();
				IEnumerable<TypeInfo> types = currentAssembly.DefinedTypes;
				List<TypeInfo> models = types.Where(t => t.BaseType == typeof(Model)).ToList();
				
				return models;
			}
		}

		/// <summary>
		/// Creates collections for all models
		/// </summary>
		public void Up()
		{
			List<TypeInfo> models = AllModels;

			models.ForEach(model => CreateCollectionFromModel(model));
		}

		/// <summary>
		/// Delete all collections, that corresponding the models, from database
		/// </summary>
		public void Down()
		{
			List<TypeInfo> models = AllModels;

			models.ForEach(model => DeleteModelCollection(model));
		}
        
        /// <summary>
        /// Default options for creating collection in database
        /// </summary>
        private CreateCollectionOptions optionsDefault = new CreateCollectionOptions() { AutoIndexId = true };

		/// <summary>
		/// Creates collection from model
		/// </summary>
		/// <param name="model">Model for creating collection</param>
		private void CreateCollectionFromModel(Type model, CreateCollectionOptions options = null)
		{
			var database = Connector.GetDatabase();

			string collectionName = GetCollectionNameFromModel(model);
			List<BsonDocument> collections = database.ListCollections().ToList();
            var collectionsByName = collections.Find(c => c["name"] == collectionName);

            try
			{
                if(collectionsByName == null)
                {
                    database.CreateCollection(collectionName);
                }
			}
			catch (Exception ex)
			{
				Debug.Fail($"Failed creating collection: {ex.Message}");
			}
		}

		/// <summary>
		/// Delete collection from database
		/// </summary>
		/// <param name="model">Model corresponding collection</param>
		private void DeleteModelCollection(Type model)
		{
			var database = Connector.GetDatabase();

			string collectionName = GetCollectionNameFromModel(model);

			try
			{
				database.DropCollection(collectionName);
			}
			catch(Exception ex)
			{
				Debug.Fail($"Ooops! Something went wrong while dropping collection {collectionName}: {ex.Message}");
			}
		}
		private string GetCollectionNameFromModel(Type model)
		{
			string collectionName = model.Name.ToLower() + 's';
			return collectionName;
		}

        private List<PropertyInfo> GetAllPropertiesOfClass(Type instance)
        {
            List<PropertyInfo> properties = instance.GetProperties().ToList();
            return properties;
        }
    }
}
