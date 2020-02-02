using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;

namespace DataTableRetriever
{
     public class Retriever
    {
        private string _connectionString;
        private string _tableName;
        private List<string> _columns;

        public Retriever(string connectionString, string tableName, List<string> columns)
        {
            _connectionString = connectionString;
            _tableName = tableName;
            _columns = columns;
        }

        public IDbConnection Connection => new SqlConnection(_connectionString);
        /// <summary>
        /// Method for retrieving data for use in a jquery datatable. Uses Select with Offset and fetch to return a range of data for a page that varies by the specified page size
        /// and selected page. Also handles sorting by column and direction as well as a search term. 
        /// </summary>
        /// <param name="start">Paging parameter indicating where to begin pulling data</param>
        /// <param name="length">Paging parameter for number of records to return for page size</param>
        /// <param name="sortColumn">Which column is used as the sorting column</param>
        /// <param name="sortColumnDirection">The direction to sort the selected column</param>
        /// <param name="searchValue">The search term, returns records with a column containing the search value.</param>
        /// <returns>An instance of a Result object that contains an enumerable of results, an integer of the total size of the search result, and an integer of the draw.</returns>
        public Result GetData(string start, string length, string sortColumn, string sortColumnDirection, string searchValue)
        {
            //Result is custom object that stores the dynamic list of results, the total size of records selected from after searching, and the Draw
            Result returnData = new Result();

            DynamicParameters parameters = new DynamicParameters();
            //default sorting column is the first in the list
            string sortCol = _columns.First();
            int listSize = _columns.Count;

            //Set Skip and PageSize from draw, start, and length

            int skip = start != null ? Convert.ToInt32(start) : 0;
            int pageSize = length != null ? Convert.ToInt32(length) : 0;

            //if the context has a sort columns specified, overwrite the default with the requested column
            if (!String.IsNullOrEmpty(sortColumn))
            {
                sortCol = sortColumn;
            }
            //begin building the query to select a range from the search result
            string rangeQuery = "SELECT";
            //select the columns specified
            for (int count = 0; count < listSize; count++)
            {
                rangeQuery += $" {_columns[count]}";
                if (count != listSize - 1)
                {
                    rangeQuery += ",";
                }
            }
            rangeQuery += $" FROM {_tableName} ";
            if (!String.IsNullOrEmpty(searchValue))
            {
                parameters.Add("@searchTerm", "%" + searchValue + "%", DbType.String, ParameterDirection.Input);
                rangeQuery += "WHERE ";
                for (int count = 0; count < listSize; count++)
                {
                    rangeQuery += $"{_columns[count]} LIKE @searchTerm ";
                    if (count != listSize - 1)
                    {
                        rangeQuery += "OR ";
                    }
                }
            }
            rangeQuery += $"ORDER BY {sortCol} {sortColumnDirection} OFFSET {skip} ROWS FETCH NEXT {pageSize} ROWS ONLY; ";
            rangeQuery += $"SELECT COUNT(*) FROM {_tableName} ";
            if (!String.IsNullOrEmpty(searchValue))
            {
                rangeQuery += "WHERE ";
                for (int count = 0; count < listSize; count++)
                {
                    rangeQuery += $"{_columns[count]} LIKE @searchTerm ";
                    if (count != listSize - 1)
                    {
                        rangeQuery += "OR ";
                    }
                }
            }
            //open the connection and populate fields of the Result object
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                var multi = dbConnection.QueryMultiple(rangeQuery, parameters);
                returnData.Results = multi.Read<dynamic>().ToList();
                returnData.Size = multi.Read<int>().Single();
            }
            return returnData;
        }
    }
}