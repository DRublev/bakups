using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SPT.Models;

using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoDB.Bson;

namespace SPT.DatabaseWorking
{
	public class MongoCRUD : IDatabaseCRUD
	{

		public void Insert(Model data)
		{
			var collection = data.GetMongoCollection<Model>();

			try
			{
				collection.InsertOne(data);
			}
			catch(Exception ex)
			{
				Debug.Write($"Error insert to collection: {ex.Message}");
			}
			
		}

		public IEnumerable<Model> Select(Model model)
		{
			var collection = model.GetMongoCollection<Model>();
			List<Model> data = collection.Find(_ => true).ToList();

			return data;
		}

		public IEnumerable<Model> Select(Model model, Func<Model, bool> filter)
		{
			var collection = model.GetMongoCollection<Model>();
			List<Model> data = ((List<Model>)collection.Find(_ => true)).Where(filter).ToList();

			return data;
		}

		public void Update(FilterDefinition<Model> filter, Model data)
		{
			var collection = data.GetMongoCollection<Model>();

			try
			{
				collection.UpdateOne(filter, data.ToBsonDocument());
			}
			catch(Exception ex)
			{
				Debug.Write($"Error updating model: {ex.Message}");
			}
		}

		public void Delete<T>(FilterDefinition<Model> filter) where T : Model
		{
			var collection = (GetInstance<T>(typeof(T))).GetMongoCollection<Model>();

			try
			{
				var result = collection.DeleteOne(filter);
			}
			catch(Exception ex)
			{
				Debug.Write($"Error deleting model: {ex.Message}");
			}
		}

		private T GetInstance<T>(Type type)
		{
			return (T) Activator.CreateInstance(type);
		}
	}
}
