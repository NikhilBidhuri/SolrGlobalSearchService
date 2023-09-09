using System;
using System.Linq;
using CodeWithNikhil.Foundation.Search.Models;
using Sitecore.Diagnostics;
using CodeWithNikhil.Foundation.DependencyInjection;
using System.Collections.Generic;
using Newtonsoft.Json;
using CodeWithNikhil.Foundation.Search.Interface;
using Sitecore.Data.Items;
using Sitecore.Links;

namespace CodeWithNikhil.Foundation.Search.Repository
{
    // This class implements the ISearchResultRepository interface.
    [Service(typeof(ISearchResultRepository))]
    public class SearchResultRepository : ISearchResultRepository
    {
        private ISearchResultService _searchResultService;

        // Constructor that receives an ISearchResultService instance via dependency injection.
        public SearchResultRepository(ISearchResultService searchResultService)
        {
            // Ensure that the searchResultService parameter is not null.
            Assert.ArgumentNotNull(searchResultService, nameof(ISearchResultService));
            this._searchResultService = searchResultService;
        }

        // Retrieves global search results with pagination support.
        public List<SearchValueForGlobalWithSearchText> GetSearchResultsForGlobal(string searchedText, int pageNumber, int pageSize)
        {
            var searchResult = new List<SearchValueForGlobal>();
            var searchResultWithSearchText = new List<SearchValueForGlobalWithSearchText>();
            try
            {
                // Call the searchResultService to retrieve global search results with pagination.
                var result = _searchResultService.GetSearchResultsForGlobal(searchedText, pageNumber, pageSize);
                if (result.GlobalResults != null)
                {
                    foreach (var value in result.GlobalResults)
                    {
                        var data = new SearchValueForGlobal();
                        data.Title = value.Title;
                        data.URL = ConvertItemPathtoLink(value.FullPath);
                        data.Description = value.Description;
                        data.Language = value.Language;

                        searchResult.Add(data);
                    }
                }

                var listData = new SearchValueForGlobalWithSearchText();
                listData.SearchedText = searchedText;
                listData.TotalResults = result.TotalResult;
                listData.Results = searchResult;

                searchResultWithSearchText.Add(listData);
            }
            catch (Exception ex)
            {
                // Log any errors that occur during the search process.
                Sitecore.Diagnostics.Log.Error("GetSearchResultsForGlobal is causing an error", ex.ToString());
            }
            return searchResultWithSearchText;
        }

        // Converts an item path to its corresponding URL using Sitecore's LinkManager.
        public string ConvertItemPathtoLink(string ItemPath)
        {
            Item item = Sitecore.Context.Database.GetItem(ItemPath);
            if (item != null)
            {
                string itemURL = LinkManager.GetItemUrl(item);
                return itemURL;
            }
            return string.Empty;
        }
    }
}