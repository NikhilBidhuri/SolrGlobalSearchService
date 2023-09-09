using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.ComputedFields;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Extensions;
using Sitecore.Layouts;
using Sitecore.Web.UI.WebControls;

namespace Nikhil.Foundation.Search.ComputedFields
{
    public class DatasourceData : IComputedIndexField
    {
        // Properties to define the field name and return type for the computed index field.
        public string FieldName { get; set; }
        public string ReturnType { get; set; }

        // Main method to compute the field value for indexing.
        public object ComputeFieldValue(IIndexable indexable)
        {
            // Convert the indexable item to a Sitecore item.
            Item sitecoreIndexable = indexable as SitecoreIndexableItem;

            if (sitecoreIndexable == null)
                return null;

            // Check if the item belongs to a specific path.
            if (sitecoreIndexable.Paths.FullPath.StartsWith("/sitecore/content"))
            {
                // Retrieve and return data source field values.
                var dataSourceFieldsValues = GetDataSourceFieldsValues(sitecoreIndexable);
                return dataSourceFieldsValues;
            }

            return null;
        }

        // Method to extract data source field values from the item's layout.
        protected virtual string GetDataSourceFieldsValues(Item currentItem)
        {
            var fieldsValues = new List<string>();

            // Get the XML layout definition from the item's final layout field.
            string currentLayoutXml = LayoutField.GetFieldValue(currentItem.Fields[FieldIDs.FinalLayoutField]);
            if (string.IsNullOrEmpty(currentLayoutXml))
                return null;

            // Parse the layout definition XML.
            LayoutDefinition layout = LayoutDefinition.Parse(currentLayoutXml);

            // Iterate through devices and renderings in the layout.
            foreach (DeviceDefinition deviceDefinition in layout.Devices)
            {
                foreach (RenderingDefinition renderingDefinition in deviceDefinition.Renderings)
                {
                    // Exclude specific renderings by their IDs.
                    if (!renderingDefinition.ItemID.ToString().Equals(Constants.SitecoreGUID.HeaderRenderingID) &&
                        !renderingDefinition.ItemID.ToString().Equals(Constants.SitecoreGUID.FooterRenderingID))
                    {
                        string datasource = renderingDefinition.Datasource;

                        if (!string.IsNullOrWhiteSpace(datasource))
                        {
                            var dataSourceItem = ResolveDataSource(datasource, currentItem.Database);

                            if (dataSourceItem != null)
                            {
                                fieldsValues.AddRange(ExtractFieldsFromItem(dataSourceItem));
                            }
                        }
                    }
                }
            }

            // Combine all field values into a single string.
            string AllData = string.Join(" ", fieldsValues);

            return AllData;
        }

        // Method to extract specific fields' values from an item.
        protected virtual IEnumerable<string> ExtractFieldsFromItem(Item item)
        {
            List<string> fieldsValues = new List<string>();

            foreach (Field field in item.Fields)
            {
                // Check if the field's name contains specific keywords.
                if (field != null && (field.Name.ToLower().Contains("title") || field.Name.ToLower().Contains("description") || field.Name.ToLower().Contains("text") || field.Name.ToLower().Contains("banner")))
                {
                    // Check if the field is a text field type and matches the context language.
                    if (IsTextFieldType(field))
                    {
                        fieldsValues.Add(field.Value.ToString());
                    }
                }
            }

            // Recursively extract fields from child items.
            foreach (Item childItem in item.Children)
            {
                fieldsValues.AddRange(ExtractFieldsFromItem(childItem));
            }

            return fieldsValues;
        }

        // Method to check if a field is of text field type.
        protected virtual bool IsTextFieldType(Field field)
        {
            return field.Type.ToString().Equals("Single-Line Text") || field.Type.ToString().Equals("Rich Text");
        }

        // Method to resolve a data source to an item.
        protected virtual Item ResolveDataSource(string dataSourcePath, Database database)
        {
            if (ID.IsID(dataSourcePath))
            {
                return database.GetItem(ID.Parse(dataSourcePath));
            }
            else if (!string.IsNullOrEmpty(dataSourcePath))
            {
                return database.GetItem(dataSourcePath);
            }

            return null;
        }
    }
}