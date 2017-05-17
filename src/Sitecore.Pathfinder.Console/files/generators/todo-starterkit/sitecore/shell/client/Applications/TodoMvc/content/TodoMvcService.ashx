<%@ WebHandler Language="C#" Class="Sitecore.Website.Client.Applications.TodoMvc.TodoMvcService" %>

using System;
using System.Linq;
using System.Web;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.SecurityModel;

namespace Sitecore.Website.Client.Applications.TodoMvc
{
    public class TodoMvcService : IHttpHandler
    {
        private const string TodoMvcItemPath = "/sitecore/client/Applications/TodoMvc/content/TodoMvc";
        private const string TodoMvcItemTemplatePath = "/sitecore/client/Applications/TodoMvc/templates/TodoItem";

        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";

            var databaseName = context.Request.Params["database"] ?? "core";
            var database = Factory.GetDatabase(databaseName);

            // almost never use a SecurityDisabler (unless you are absolutely sure you know what you are doing)!
            using (new SecurityDisabler())
            {
                var action = context.Request.Params["a"];

                if (string.Equals(action, "post", StringComparison.OrdinalIgnoreCase))
                {
                    Post(context, database);
                }

                if (string.Equals(action, "put", StringComparison.OrdinalIgnoreCase))
                {
                    Put(context, database);
                }

                if (string.Equals(action, "remove", StringComparison.OrdinalIgnoreCase))
                {
                    Remove(context, database);
                }
            }
        }

        private void Post(HttpContext context, Database database)
        {
            var text = context.Request.Params["Text"] ?? string.Empty;

            var parentItem = database.GetItem(TodoMvcItemPath);
            var templateItem = new TemplateItem(database.GetItem(TodoMvcItemTemplatePath));

            string newItemName;
            var index = 0;
            do
            {
                newItemName = "TodoItem" + index;
                index++;
            }
            while (parentItem.Children[newItemName] != null);

            var newItem = parentItem.Add(newItemName, templateItem);
            using (new EditContext(newItem))
            {
                newItem["Text"] = text;
            }

            context.Response.Output.WriteLine("{}");
        }

        private void Put(HttpContext context, Database database)
        {
            var itemId = context.Request.Params["itemid"] ?? string.Empty;
            
            var item = database.GetItem(itemId);
            if (item == null || !item.Paths.Path.StartsWith(TodoMvcItemPath, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            using (new EditContext(item))
            {
                foreach (var key in context.Request.Form.Keys.OfType<string>())
                {
                    var field = item.Fields[key];
                    if (field == null)
                    {
                        continue;
                    }

                    var value = context.Request.Form[key];
                    if (string.Equals(field.Type, "checkbox", StringComparison.OrdinalIgnoreCase))
                    {
                        value = value == "true" ? "1" : string.Empty;
                    }

                    item[key] = value;
                }
            }

            context.Response.Output.WriteLine("{}");
        }

        private void Remove(HttpContext context, Database database)
        {
            var itemId = context.Request.Params["itemid"] ?? string.Empty;

            var item = database.GetItem(itemId);
            if (item == null || !item.Paths.Path.StartsWith(TodoMvcItemPath, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            item.Recycle();

            context.Response.Output.WriteLine("{}");
        }
    }
}

