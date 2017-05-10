# Layout as files
Layouts can be specified as part of items files, but it can be tricky to get right as the syntax is very abstract.

As an alternative you can specify a layout as an Html file which Pathfinder compiles into a Sitecore layout definition.
The Html can contains "rendering-tags" which refer to Sitecore renderings.

Use the "Layout.Page" property on an item to point to a .page.html file in the project.

Example:
```xml
<Item xmlns="http://www.sitecore.net/pathfinder/item" 
  Template="/sitecore/templates/Sample/Sample Item" 
  Layout.Page="~/pages/helloworld.page.html" />
```

The *.page.html are Html files, but with inline Sitecore renderings (much like server tags in WebForms) in a separate namespace.

Example: 
```html
<!DOCTYPE html>
<html xmlns:r="http://www.sitecore.net/pathfinder/rendering" xmlns:p="http://www.sitecore.net/pathfinder/placeholder">
<head>
</head>
<body>
    <div style="padding:4px" onclick="javascript: alert('Hello');">
        <h1>Hello World</h1>
        <r:timer start="100">
            <p:Content>
                <span>Hola</span>
            </p:Content>
        </r:timer>
    </div>
</body>
</html>
```

In the above, "timer" is a Sitecore rendering. Placeholders are  explicitly named as children of the rendering tag - 
in the example above, timer exposes the "Content" placeholder.

Pathfinder compiles *.page.html to a real Sitecore layout definition. Html tags are compiled to a "Literal"
rendering (much the same way the ASP.NET compiles .aspx pages).
The example above compiles to something like:

```xml
<r>
  <d id="..." l="...">
    <r id="Literal" par="text=<!DOCTYPE html><html..." />
    <r id="timer" />
    <r id="Literal" ph="Content" par="text=<span>Hola</span>" />
    <r id="Literal" " par="text=</div></body></html>" />
  </d>
</r>
```

Of course this supports standard Sitecore layout definitions. Here is an example with no inline Html:

MyLayout.page.html:
```html
<MyLayout xmlns:r="http://www.sitecore.net/pathfinder/rendering" xmlns:p="http://www.sitecore.net/pathfinder/placeholder">
  <p:Menu>
    <r:TopMenu />
  </p:Menu>
  <p:Sidebar>
    <r:Sidebar />
  </p:Sidebar>
  <p:Main>
    <r:Document />
  </p:Main>
</MyLayout>
```

