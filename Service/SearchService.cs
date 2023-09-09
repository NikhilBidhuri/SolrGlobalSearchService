public SearchResultsForGlobal GetSearchResultsForGlobal(string searchedText, int pageNumber, int pageSize)
{
    var result = new SearchResultsForGlobal();
    try
    {
        using (var context = ContentSearchManager.GetIndex(Settings.GetSetting(Constants.MasterIndexSetting)).CreateSearchContext())
        {
            var queryable = BuildQueryforGlobal(context, searchedText);

            if (queryable != null)
            {
                queryable = queryable.Select(x => new SearchFieldsForGlobal
                {
                    FullPath = x.FullPath,
                    Title = x.Title,
                    DataSourceData = x.DataSourceData,
                    Description = x.Description,
                    Language = x.Language
                });

                // Calculate the number of items to skip based on the page number and page size
                int itemsToSkip = (pageNumber - 1) * pageSize;

                // Apply pagination to the queryable
                queryable = queryable.Skip(itemsToSkip).Take(pageSize);

                var results = queryable.GetResults();

                if (results != null && results.Hits != null && results.Hits.Any())
                {
                    result.GlobalResults = results.Hits.Select(x => x.Document).ToList();
                    result.TotalResult = results.TotalSearchResults;
                }
            }
        }
    }
    catch (Exception ex)
    {
        Sitecore.Diagnostics.Log.Error("Error in SolrService - ContentSearchManager.GetIndex - " + ex.Message, this);
    }
    return result;
}