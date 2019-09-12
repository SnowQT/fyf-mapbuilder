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
var Data = __importStar(require("./assets/meta/objects.json"));
var ObjectListComponent = (function (_super) {
    __extends(ObjectListComponent, _super);
    function ObjectListComponent() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    ObjectListComponent.prototype.componentDidMount = function () {
        console.log('I was triggered during componentDidMount, DATA: ' + JSON.stringify(Data));
    };
    ObjectListComponent.prototype.render = function () {
        return (React.createElement("ul", null,
            React.createElement("li", { key: "life" }, "Awesome!")));
    };
    return ObjectListComponent;
}(React.Component));
exports["default"] = ObjectListComponent;
//# sourceMappingURL=ObjectListComponent.js.map