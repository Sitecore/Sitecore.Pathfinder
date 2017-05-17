define(["sitecore"], function (sitecore) {
    var model = sitecore.Definitions.Models.ControlModel.extend({
        initialize: function (options) {
            this._super();
            this.set("Items", []);
            this.set("Filter", "");
        }
    });

    var view = sitecore.Definitions.Views.ControlView.extend(
    {
        initialize: function (options) {
            this._super();
        },

        filterAll: function () {
            this.model.set("Filter", "");
        },

        filterActive: function () {
            this.model.set("Filter", "Active");
        },

        filterCompleted: function () {
            this.model.set("Filter", "Completed");
        },

        removeItem: function (data) {
            this.app.removeItem(data);
        },

        toggleDone: function (data) {
            this.app.toggleDone(data);
        }
    });

    sitecore.Factories.createComponent("TodoMvcList", model, view, ".sc-todomvclist");
});