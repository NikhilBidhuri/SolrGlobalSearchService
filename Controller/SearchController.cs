using CodeWithNikhil.Foundation.DependencyInjection;
using CodeWithNikhil.Foundation.Search.Repository;
using Newtonsoft.Json;
using Sitecore.Mvc.Controllers;
using System;
using System.Web.Mvc;
using CodeWithNikhil.Foundation.Search.Interface;
using System.Web.Http.Cors;
using CodeWithNikhil.Foundation.Search.Models;
using System.Linq;
using System.Collections.Generic;

namespace CodeWithNikhil.Foundation.Search.Controllers
{
    // This class represents a controller for handling search-related requests.
    public class SearchController : SitecoreController
    {
        private ISearchResultRepository _searchResultRepo;

        // Constructor that receives an ISearchResultRepository instance via dependency injection.
        public SearchController(ISearchResultRepository searchResultRepo)
        {
            _searchResultRepo = searchResultRepo;
        }


        // Handles HTTP GET requests for global search results with pagination support.
        public ContentResult GetGlobalSearchResults(string query, int pageNumber)
        {
            // Define the number of search results per page.
            const int pageSize = 10;
            var result = new ContentResult();
            try
            {
                // Retrieve search results for the global search with pagination.
                var response = _searchResultRepo.GetSearchResultsForGlobal(query, pageNumber, pageSize);
                // Serialize the search results to JSON.
                var outputResponse = JsonConvert.SerializeObject(response);
                result = Content(outputResponse, "application/json");

            }
            catch (Exception ex)
            {
                // Log any errors that occur during the search process.
                Sitecore.Diagnostics.Log.Error("Error in SearchController - GetGlobalSearchResults - " + ex.Message, this);
            }

            return result;
        }
    }
}