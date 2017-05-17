define(["sitecore"], function (sitecore) {
    var app = sitecore.Definitions.App.extend({
        initialized: function () {
            this.InputTextBox.on("change:text", this.addItem, this);
        },

        addItem: function () {
            var text = this.InputTextBox.get("text");
            if (!text) {
                return;
            }

            var self = this;
            this.request("post", { text: text }, "POST", function() {
                self.InputTextBox.set("text", "");
            });
        },

        removeItem: function (data) {
            this.request("remove", { itemid: data.itemId });
        },

        toggleDone: function (data) {
            this.request("put", { itemid: data.itemId, Done: data.Done }, "POST");
        },

        request: function (action, data, method, success) {
            data.a = action;

            this.ErrorMessageText.set("isVisible", false);

            var self = this;
            $.ajax({
                url: "/sitecore/shell/client/Applications/TodoMvc/content/TodoMvcService.ashx",
                method: method,
                data: data
            }).done(function () {
                self.TodoItemDataSource.refresh();
                if (success)
                {
                    success();
                }
            }).fail(function () {
                self.ErrorMessageText.set("text", "An error occured.");
                self.ErrorMessageText.set("isVisible", true);
            });
        }
    });

    return app;
});