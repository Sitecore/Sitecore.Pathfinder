using System.Collections.Generic;
using Sitecore.Pathfinder.Code.Data;

namespace Sitecore.Pathfinder.Code.Sitecore.Content.Home
{
    public class Items2 : IProvidesItems
    {
        public static Item About { get; } = new Item("About", "/sitecore/content/Home/CleanBlog/NavBar/About", "/sitecore/templates/CleanBlog/CleanBlog")
        {
            { "PageTitle", "Clean Blog - About Me" },
            { "Header", "About Me" },
            { "Subheader", "This is what I do." },
            { "Text", @"<p>Lorem ipsum dolor sit amet, consectetur adipisicing elit. Saepe nostrum ullam eveniet pariatur voluptates odit, fuga atque ea nobis sit soluta odio, adipisci quas excepturi maxime quae totam ducimus consectetur?</p>
            <p>Lorem ipsum dolor sit amet, consectetur adipisicing elit. Eius praesentium recusandae illo eaque architecto error, repellendus iusto reprehenderit, doloribus, minus sunt. Numquam at quae voluptatum in officia voluptas voluptatibus, minus!</p>
            <p>Lorem ipsum dolor sit amet, consectetur adipisicing elit. Nostrum molestiae debitis nobis, quod sapiente qui voluptatum, placeat magni repudiandae accusantium fugit quas labore non rerum possimus, corrupti enim modi! Et.</p>"}
        };

        public static Item Contact { get; } = new Item("Contact", "/sitecore/content/Home/CleanBlog/NavBar/Contact", "/sitecore/templates/CleanBlog/CleanBlog")
        {
            { "PageTitle", "Clean Blog - Contact" },
            { "Header", "Contact Me" },
            { "Subheader", " Have questions? I have answers (maybe)." },
            { "Text", @" <p>Want to get in touch with me? Fill out the form below to send me a message and I will try to get back to you within 24 hours!</p>" },
        };

        public static Item NavBar { get; } = new Item("NavBar", "/sitecore/content/Home/CleanBlog/NavBar", "/sitecore/templates/Common/Folder");

        public static Item CleanBlog { get; } = new Item("CleanBlog", "/sitecore/content/Home/CleanBlog", "/sitecore/templates/CleanBlog/CleanBlog")
        {
            { "PageTitle", "Clean Blog" },
            { "Header", "Clean Blog" },
            { "Subheader", "A Clean Blog Theme by Start Bootstrap" },
            { "ToggleNavigation", "Toggle Navigation" },
            { "StartBootstrap", "Start Bootstrap" },
            { "Copyright", "Copyright (C); Your Website 2014" },
            { "Older Posts", "Older Posts" },
            { NavBar, new[] {
                About,
                Contact
            }}
        };

        public IEnumerable<Item> ProvideItems()
        {
            yield return CleanBlog;
        }
    }
}