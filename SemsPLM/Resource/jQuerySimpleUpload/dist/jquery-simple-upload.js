!function (e) { var t = {}; function n(r) { if (t[r]) return t[r].exports; var o = t[r] = { i: r, l: !1, exports: {} }; return e[r].call(o.exports, o, o.exports, n), o.l = !0, o.exports } n.m = e, n.c = t, n.d = function (e, t, r) { n.o(e, t) || Object.defineProperty(e, t, { enumerable: !0, get: r }) }, n.r = function (e) { "undefined" != typeof Symbol && Symbol.toStringTag && Object.defineProperty(e, Symbol.toStringTag, { value: "Module" }), Object.defineProperty(e, "__esModule", { value: !0 }) }, n.t = function (e, t) { if (1 & t && (e = n(e)), 8 & t) return e; if (4 & t && "object" == typeof e && e && e.__esModule) return e; var r = Object.create(null); if (n.r(r), Object.defineProperty(r, "default", { enumerable: !0, value: e }), 2 & t && "string" != typeof e) for (var o in e) n.d(r, o, function (t) { return e[t] }.bind(null, o)); return r }, n.n = function (e) { var t = e && e.__esModule ? function () { return e.default } : function () { return e }; return n.d(t, "a", t), t }, n.o = function (e, t) { return Object.prototype.hasOwnProperty.call(e, t) }, n.p = "/dist", n(n.s = 5) }([function (e, t) { e.exports = jQuery }, function (e, t, n) { var r = n(2); "string" == typeof r && (r = [[e.i, r, ""]]); var o = { insert: "head", singleton: !1 }; n(4)(r, o); r.locals && (e.exports = r.locals) }, function (e, t, n) { (e.exports = n(3)(!1)).push([e.i, ".simple-upload-dragover {\n  background-color: #eef; }\n\n.simple-upload-filename {\n  margin-right: 0.5em; }\n", ""]) }, function (e, t) { e.exports = function (e) { var t = []; return t.toString = function () { return this.map(function (t) { var n = function (e, t) { var n = e[1] || "", r = e[3]; if (!r) return n; if (t && "function" == typeof btoa) { var o = (i = r, "/*# sourceMappingURL=data:application/json;charset=utf-8;base64," + btoa(unescape(encodeURIComponent(JSON.stringify(i)))) + " */"), a = r.sources.map(function (e) { return "/*# sourceURL=" + r.sourceRoot + e + " */" }); return [n].concat(a).concat([o]).join("\n") } var i; return [n].join("\n") }(t, e); return t[2] ? "@media " + t[2] + "{" + n + "}" : n }).join("") }, t.i = function (e, n) { "string" == typeof e && (e = [[null, e, ""]]); for (var r = {}, o = 0; o < this.length; o++) { var a = this[o][0]; "number" == typeof a && (r[a] = !0) } for (o = 0; o < e.length; o++) { var i = e[o]; "number" == typeof i[0] && r[i[0]] || (n && !i[2] ? i[2] = n : n && (i[2] = "(" + i[2] + ") and (" + n + ")"), t.push(i)) } }, t } }, function (e, t, n) { "use strict"; var r, o = {}, a = function () { return void 0 === r && (r = Boolean(window && document && document.all && !window.atob)), r }, i = function () { var e = {}; return function (t) { if (void 0 === e[t]) { var n = document.querySelector(t); if (window.HTMLIFrameElement && n instanceof window.HTMLIFrameElement) try { n = n.contentDocument.head } catch (e) { n = null } e[t] = n } return e[t] } }(); function s(e, t) { for (var n = [], r = {}, o = 0; o < e.length; o++) { var a = e[o], i = t.base ? a[0] + t.base : a[0], s = { css: a[1], media: a[2], sourceMap: a[3] }; r[i] ? r[i].parts.push(s) : n.push(r[i] = { id: i, parts: [s] }) } return n } function u(e, t) { for (var n = 0; n < e.length; n++) { var r = e[n], a = o[r.id], i = 0; if (a) { for (a.refs++; i < a.parts.length; i++)a.parts[i](r.parts[i]); for (; i < r.parts.length; i++)a.parts.push(v(r.parts[i], t)) } else { for (var s = []; i < r.parts.length; i++)s.push(v(r.parts[i], t)); o[r.id] = { id: r.id, refs: 1, parts: s } } } } function l(e) { var t = document.createElement("style"); if (void 0 === e.attributes.nonce) { var r = n.nc; r && (e.attributes.nonce = r) } if (Object.keys(e.attributes).forEach(function (n) { t.setAttribute(n, e.attributes[n]) }), "function" == typeof e.insert) e.insert(t); else { var o = i(e.insert || "head"); if (!o) throw new Error("Couldn't find a style target. This probably means that the value for the 'insert' parameter is invalid."); o.appendChild(t) } return t } var p, d = (p = [], function (e, t) { return p[e] = t, p.filter(Boolean).join("\n") }); function c(e, t, n, r) { var o = n ? "" : r.css; if (e.styleSheet) e.styleSheet.cssText = d(t, o); else { var a = document.createTextNode(o), i = e.childNodes; i[t] && e.removeChild(i[t]), i.length ? e.insertBefore(a, i[t]) : e.appendChild(a) } } var f = null, h = 0; function v(e, t) { var n, r, o; if (t.singleton) { var a = h++; n = f || (f = l(t)), r = c.bind(null, n, a, !1), o = c.bind(null, n, a, !0) } else n = l(t), r = function (e, t, n) { var r = n.css, o = n.media, a = n.sourceMap; if (o && e.setAttribute("media", o), a && btoa && (r += "\n/*# sourceMappingURL=data:application/json;base64,".concat(btoa(unescape(encodeURIComponent(JSON.stringify(a)))), " */")), e.styleSheet) e.styleSheet.cssText = r; else { for (; e.firstChild;)e.removeChild(e.firstChild); e.appendChild(document.createTextNode(r)) } }.bind(null, n, t), o = function () { !function (e) { if (null === e.parentNode) return !1; e.parentNode.removeChild(e) }(n) }; return r(e), function (t) { if (t) { if (t.css === e.css && t.media === e.media && t.sourceMap === e.sourceMap) return; r(e = t) } else o() } } e.exports = function (e, t) { (t = t || {}).attributes = "object" == typeof t.attributes ? t.attributes : {}, t.singleton || "boolean" == typeof t.singleton || (t.singleton = a()); var n = s(e, t); return u(n, t), function (e) { for (var r = [], a = 0; a < n.length; a++) { var i = n[a], l = o[i.id]; l && (l.refs--, r.push(l)) } e && u(s(e, t), t); for (var p = 0; p < r.length; p++) { var d = r[p]; if (0 === d.refs) { for (var c = 0; c < d.parts.length; c++)d.parts[c](); delete o[d.id] } } } } }, function (e, t, n) { "use strict"; n.r(t); var r = n(0), o = n.n(r), a = "simple-upload"; n(1); function i(e, t) { return function (e) { if (Array.isArray(e)) return e }(e) || function (e, t) { var n = [], r = !0, o = !1, a = void 0; try { for (var i, s = e[Symbol.iterator](); !(r = (i = s.next()).done) && (n.push(i.value), !t || n.length !== t); r = !0); } catch (e) { o = !0, a = e } finally { try { r || null == s.return || s.return() } finally { if (o) throw a } } return n }(e, t) || function () { throw new TypeError("Invalid attempt to destructure non-iterable instance") }() } function s(e, t) { for (var n = 0; n < t.length; n++) { var r = t[n]; r.enumerable = r.enumerable || !1, r.configurable = !0, "value" in r && (r.writable = !0), Object.defineProperty(e, r.key, r) } } var u = { url: "", method: "post", headers: {}, dataType: null, params: {}, timeout: 0, async: !0, dropZone: null, progress: null, validator: null, maxFileSize: null, maxFileNum: null, allowedMimeType: [] }, l = function () { function e(t) { var n = arguments.length > 1 && void 0 !== arguments[1] ? arguments[1] : {}; !function (e, t) { if (!(e instanceof t)) throw new TypeError("Cannot call a class as a function") }(this, e), this.options = o.a.extend({}, u, n), this.$input = o()(t), this.$dropZone = o()(this.options.dropZone), this.$progress = o()(this.options.progress), this.uid = (new Date).getTime() + Math.random(), this.namespace = "".concat(a, "-").concat(this.uid), this.dragCounter = 0, this.init() } var t, n, r; return t = e, r = [{ key: "makeParams", value: function (e) { var t = {}; switch (Object.prototype.toString.call(e)) { case "[object Function]": t = e(); break; case "[object Array]": e.forEach(function (e) { t[e.name] = e.value }); break; case "[object Object]": o.a.extend(t, e) }return t } }, { key: "getDefaults", value: function () { return u } }, { key: "setDefaults", value: function (e) { return o.a.extend(u, e) } }], (n = [{ key: "init", value: function () { this.$input.addClass(a), this.$dropZone.addClass(a).addClass("simple-upload-droppable"), this.$progress.addClass(a), this.unbind(), this.bind() } }, { key: "destroy", value: function () { this.$input.removeClass(a), this.$dropZone.removeClass(a).removeClass("simple-upload-droppable"), this.$progress.removeClass(a), this.unbind() } }, { key: "bind", value: function () { var e = this; this.$input.on("change.".concat(this.namespace), function (t) { e.process(t.target.files), t.target.value = "" }), this.$dropZone.length && (this.$dropZone.on("drop.".concat(this.namespace), function (t) { t.preventDefault(), t.stopPropagation(), e.dragCounter = 0, e.$dropZone.removeClass("simple-upload-dragover"), e.process(t.originalEvent.dataTransfer.files) }).on("dragenter.".concat(this.namespace), function (t) { t.preventDefault(), e.dragCounter++, e.$dropZone.addClass("simple-upload-dragover") }).on("dragleave.".concat(this.namespace), function (t) { t.preventDefault(), e.dragCounter--, 0 == e.dragCounter && e.$dropZone.removeClass("simple-upload-dragover") }), o()(document).on("drop.".concat(this.namespace), function (e) { e.preventDefault() }).on("dragover.".concat(this.namespace), function (e) { e.preventDefault() })) } }, { key: "unbind", value: function () { this.$input.off(".".concat(this.namespace)), this.$dropZone.off(".".concat(this.namespace)), o()(document).off(".".concat(this.namespace)); var e = o.a._data(this.$input.get(0), "events"); if (e) { var t = Object.keys(e).filter(function (e) { return e.match(/^upload:/) }); this.$input.off(t.join(" ")) } } }, { key: "process", value: function (e) { if (!this.$input.prop("disabled")) if (this.options.maxFileNum && e.length > this.options.maxFileNum) this.$input.trigger("upload:over", [e]); else { var t = e; this.options.validator && (t = this.options.validator(t)); var n = i(this.validate(t), 2), r = n[0], o = n[1]; o.length > 0 && this.$input.trigger("upload:invalid", [o]), r.length > 0 && this.uploadFiles(r) } } }, { key: "validate", value: function (e) { for (var t = [], n = [], r = 0; r < e.length; r++) { var o = e[r]; this.options.maxFileSize && o.size > this.options.maxFileSize ? (o.reason = "size", n.push(o)) : this.options.allowedMimeType.length > 0 && $.inArray(o.type, this.options.allowedMimeType) < 0?(o.reason="type",n.push(o)):t.push(o)}return[t,n]}},{key:"uploadFiles",value:function(e){var t=this;this.$input.prop("disabled",!0),this.before(e);for(var n=(new o.a.Deferred).resolve(),r=function(r){n=n.then(function(){return t.uploadFile(e[r],r)})},a=0;a<e.length;a++)r(a);n.then(function(){t.after(e),t.$input.prop("disabled",!1)})}},{key:"uploadFile",value:function(e,t){var n=this,r=new o.a.Deferred;return o.a.ajax({url:this.options.url,method:this.options.method,headers:this.options.headers,dataType:this.options.dataType,data:this.buildFormData(e),timeout:this.options.timeout,async:this.options.async,processData:!1,contentType:!1,beforeSend:function(){n.start(e,t)},xhr:function(){var r=o.a.ajaxSettings.xhr();return r.upload&&r.upload.addEventListener("progress",function(r){n.progress(e,t,r.loaded,r.total)},!1),r}}).done(function(r,o,a){n.done(e,t,r,o,a)}).fail(function(r,o,a){n.fail(e,t,r,o,a)}).always(function(){n.end(e,t),r.resolve()}),r.promise()}},{key:"before",value:function(e){var t=this;this.$progress.length&&e.forEach(function(e,n){t.buildProgress(e,n)}),this.$input.trigger("upload:before",[e])}},{key:"after",value:function(e){this.$input.trigger("upload:after",[e])}},{key:"start",value:function(e,t){this.$input.trigger("upload:start",[e,t])}},{key:"progress",value:function(e,t,n,r){this.findProgress(t).find(".simple-upload-percent").text(Math.ceil(n/r*100)+"%"),this.$input.trigger("upload:progress",[e,t,n,r])}},{key:"done",value:function(e,t,n,r,o){this.$input.trigger("upload:done",[e,t,n,r,o])}},{key:"fail",value:function(e,t,n,r,o){this.$input.trigger("upload:fail",[e,t,n,r,o])}},{key:"end",value:function(e,t){this.findProgress(t).hide("fast",function(e){return o()(e).remove()}),this.$input.trigger("upload:end",[e,t])}},{key:"buildProgress",value:function(e,t){var n=o()("<div>").addClass("simple-upload-progress").data("upload-index",t);o()("<span>").addClass("simple-upload-filename").text(e.name).appendTo(n),o()("<span>").addClass("simple-upload-percent").text("...").appendTo(n),this.$progress.append(n)}},{key:"findProgress",value:function(e){return this.$progress.find(".simple-upload-progress").filter(function(t,n){return o()(n).data("upload-index")==e})}},{key:"buildFormData",value:function(t){var n=new FormData;n.append(this.$input.attr("name"),t);var r=e.makeParams(this.options.params);for(var o in r)n.append(o,r[o]);return n}}])&&s(t.prototype,n),r&&s(t,r),e}();o.a.fn.simpleUpload=function(e){return this.each(function(t,n){var r=o()(n);r.data(a)&&r.data(a).destroy(),r.data(a,new l(r,e))})},o.a.SimpleUpload=l}]);