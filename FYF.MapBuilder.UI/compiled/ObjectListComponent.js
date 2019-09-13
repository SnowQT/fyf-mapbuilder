"use strict";
var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    }
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var __importStar = (this && this.__importStar) || function (mod) {
    if (mod && mod.__esModule) return mod;
    var result = {};
    if (mod != null) for (var k in mod) if (Object.hasOwnProperty.call(mod, k)) result[k] = mod[k];
    result["default"] = mod;
    return result;
};
exports.__esModule = true;
var React = __importStar(require("react"));
var JsonObjectsList = __importStar(require("./assets/meta/objects.json"));
var ObjectListItem = (function () {
    function ObjectListItem(name, image, variants, category, tags) {
        this.name = name;
        this.image = image;
        this.variants = variants;
        this.category = category;
        this.tags = tags;
    }
    return ObjectListItem;
}());
var ObjectListComponent = (function (_super) {
    __extends(ObjectListComponent, _super);
    function ObjectListComponent(props) {
        var _this = _super.call(this, props) || this;
        _this.maxSelectSize = 21;
        _this.OnObjectSelected = function (event) {
            var objectName = event.target.value;
            var foundObject = JsonObjectsList.find(function (obj) { return obj.name == objectName; });
            if (foundObject == undefined) {
                console.log("Unknown selected object.");
                return;
            }
            var objectListItem = new ObjectListItem(foundObject.name, foundObject.image, foundObject.variants, foundObject.category, foundObject.tags);
            _this.setState({ currentSelectedObject: objectListItem });
        };
        _this.OnObjectVariantChanged = function (event) {
            console.log(event.target.value);
        };
        var objectListItem = new ObjectListItem("unknown", "", [], [], []);
        _this.state = { currentSelectedObject: objectListItem };
        return _this;
    }
    ObjectListComponent.prototype.render = function () {
        var currentSelectedObject = this.state.currentSelectedObject;
        var currentObjectVariantsSelect = currentSelectedObject.variants.map(function (variant) {
            return React.createElement("option", { value: variant }, variant);
        });
        var listItemsObject = JsonObjectsList.map(function (obj) {
            return (React.createElement("option", { value: obj.name },
                " ",
                obj.name,
                " "));
        });
        return (React.createElement("div", { id: "object-list" },
            React.createElement("h3", null, "Objects"),
            React.createElement("select", { name: "objects", onChange: this.OnObjectSelected, size: this.maxSelectSize }, listItemsObject),
            React.createElement("h3", null, "Object properties"),
            React.createElement("div", null,
                React.createElement("h5", null,
                    "Name: ",
                    currentSelectedObject.name),
                React.createElement("h5", null, "Variants:"),
                React.createElement("select", { name: "variants", onChange: this.OnObjectVariantChanged }, currentObjectVariantsSelect))));
    };
    return ObjectListComponent;
}(React.Component));
exports["default"] = ObjectListComponent;
//# sourceMappingURL=ObjectListComponent.js.map