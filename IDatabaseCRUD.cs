using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using SPT.Models;

namespace SPT.DatabaseWorking
{
	public interface IDatabaseCRUD
	{
		/// <summary>
		/// Insert Model into table/collection by Model.DatabaseTableName
		/// </summary>
		/// <param name="data">Data to insert</param>
		void Insert(Model data);

		/// <summary>
		/// Select Model from table/collection
		/// </summary>
		/// <returns></returns>
		IEnumerable<Model> Select(Model model);

		/// <summary>
		/// Select Model from table/collection by filter
		/// </summary>
		/// <param name="filter">LINQ filter</param>
		IEnumerable<Model> Select(Model model, Func<Model, bool> filter);

		/// <summary>
		/// Update Model in table/collection (Model.DatabaseTableName) matching the filter
		/// </summary>
		/// <param name="filter"></param>
		/// <param name="data"></param>
		void Update(FilterDefinition<Model> filter, Model data);

		/// <summary>
		/// Delete Model matching the filter from tabel/collection
		/// </summary>
		/// <param name="filter"></param>
		void Delete<T>(FilterDefinition<Model> filter) where T : Model;
	}
}
