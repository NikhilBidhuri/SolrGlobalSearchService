# SolrGlobalSearchService
lets say a page item has 4 rendering with different datasources 

So in this case we will create a computed field which will extract datasource data from every rendering on item and put it in a list of strings and we will return this list as computed field,
now using LINQ we can search the list if the searched text is there in list or not.

In other words, Imagine a webpage with four distinct sections, each powered by different data sources. To enable comprehensive search,
 we'll craft a "computed field" that compiles content from all these sections into a list of strings. 
This list, acting as a summary, is then integrated into Solr. 
When users search, smart querying using LINQ checks if their terms match items in this list, offering a unified search experience across the diverse sections.

So this branch contains code for REPO,SERVICE,CONTROLLER AND COMPUTED FIELD TO EXTRACT DATA FROM DATASOURCE.

ive not added interfaces and models, make it accordingly as per your requirements.
