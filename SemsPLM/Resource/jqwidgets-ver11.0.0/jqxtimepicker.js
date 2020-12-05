/*
jQWidgets v11.0.0 (2020-Nov)
Copyright (c) 2011-2020 jQWidgets.
License: https://jqwidgets.com/license/
*/
/* eslint-disable */

/* tslint:disable */
/* eslint-disable */
(function ($) {
    "use strict";
    $.jqx.jqxWidget("jqxTimePicker", "", {});

    $.extend($.jqx._jqxTimePicker.prototype, {
        defineInstance: function () {
            var settings = {
                autoSwitchToMinutes: false,                
                footer: false,
                footerTemplate: null,
                // possible values: "12-hour", "24-hour"
                format: "12-hour",
                minuteInterval: 1,
                // possible values: "hour", "minute"
                selection: "hour",
                value: new Date(),
                // possible values: "landscape", "portrait"
                view: "portrait",
                width: 500,
                height: 500,
                animation: true,
                disabled: false,
                readonly: false,
                unfocusable: false,
                name: "",
                // events
                _events: [
                    "change"
                ]
            };
            if (this === $.jqx._jqxTimePicker.prototype) {
                return settings;
            }
            $.extend(true, this, settings);
            return settings;
        },

        /**
         * Called when the element is ready. Used for one-time configuration of the TimePicker.
         */
        createInstance: function () {
            var that = this;
            that._createTimePicker();
        },

        /**
         * Applies initial settings to the TimePicker element.
         */
        _createTimePicker: function () {
            this.widgetID = this.element.id;
            var that = this;
            var template =
                '<div class="' + that.toThemeProperty("jqx-container jqx-rc-all jqx-widget") + '">' +
                '<div class="' + that.toThemeProperty("jqx-header jqx-unselectable jqx-widget-header") + '">' +
                '<div class="' + that.toThemeProperty("jqx-hour-minute-container") + '">' +
                '<div class ="' + that.toThemeProperty("jqx-hour-container") + '"></div>' +
                            '<div>:</div>' +
                '<div class="' + that.toThemeProperty("jqx-minute-container") + '"></div>' +
                        '</div>' +
                '<div class="' + that.toThemeProperty("jqx-am-pm-container") + '">' +
                '<div class="' + that.toThemeProperty("jqx-am-container") + '">AM</div>' +
                '<div class="' + that.toThemeProperty("jqx-pm-container") + '">PM</div>' +
                        '</div>' +
                    '</div>' +
                    '<div class="' + that.toThemeProperty("jqx-main-container jqx-widget-content") + '">' +
                        '<div class="' + that.toThemeProperty("jqx-svg-container jqx-item") + '">' +
                            '<div class="' + that.toThemeProperty("jqx-svg-picker jqx-fill-state-normal jqx-item") + '"></div>' +
                        '</div>' +
                        '<div class="' + that.toThemeProperty("jqx-footer") + '"></div>' +
                    '</div>' +
                    '<input name="' + this.name + '" type="hidden">' +
                '</div>';

            that.element.innerHTML = template;
            that.element.className += that.toThemeProperty("jqx-time-picker jqx-rc-all jqx-widget");
            that.element.setAttribute("view", that.view);
            
            that._container = that.element.getElementsByTagName("div")[0];
            that._header = that.element.getElementsByClassName("jqx-header")[0];
            that._hourMinuteContainer = that.element.getElementsByClassName("jqx-hour-minute-container")[0];
            that._hourContainer = that.element.getElementsByClassName("jqx-hour-container")[0];
            that._minuteContainer = that.element.getElementsByClassName("jqx-minute-container")[0];
            that._ampmContainer = that.element.getElementsByClassName("jqx-am-pm-container")[0];
            that._amContainer = that.element.getElementsByClassName("jqx-am-container")[0];
            that._pmContainer = that.element.getElementsByClassName("jqx-pm-container")[0];
            that._main = that.element.getElementsByClassName("jqx-main-container")[0];
            that._svgContainer = that.element.getElementsByClassName("jqx-svg-container")[0];
            that._picker = that.element.getElementsByClassName("jqx-svg-picker")[0];
            that._footer = that.element.getElementsByClassName("jqx-footer")[0];
            that._hiddenInput = that.element.getElementsByTagName("input")[0];

            if (that.footer) {
                that.element.setAttribute("footer", '');
            }

            if (that.readonly) {
                that.element.setAttribute("readonly", '');
            }

            if (that.disabled) {
                that.element.setAttribute("disabled", '');
            }

            that.coerce = true;
            that.min = 0;
            that._drawMin = '0';
            that.startAngle = -270;
            that.endAngle = 90;
            that._angleDifference = that.endAngle - that.startAngle;
            that.ticksVisibility = 'none';
            that._tickIntervalHandler = {};
            that._tickIntervalHandler.labelsSize = {};
            that._distance = { majorTickDistance: 0, minorTickDistance: 0, labelDistance: 10 };
            that._measurements = {};

            $(that.element).css({
                width: that.width,
                height: that.height
            });

            $.jqx.utilities.resize(that.host, function () {
                that._resizeHandler();
            });

            that._validateInitialPropertyValues();
            that._applyInitialSettings();

            if (!that._isVisible()) {
                that._renderingSuspended = true;
                return;
            }

            that._setPickerSize();

            $(that._picker).jqxDraw();
            that._draw = $(that._picker).jqxDraw('getInstance');
            
            that._getMeasurements();
            that._getAngleRangeCoefficient();
            that._renderSVG();
            that._setFocusable();
            that._addEventHandlers();
        },

        /**
         * Gets the angle-range coefficient.
         */
        _getAngleRangeCoefficient: function () {
            var that = this;

            that._angleRangeCoefficient = that._angleDifference / that._range;
        },

        /**
         * Returns the angle equivalent of a value.
         */
        _getAngleByValue: function (value, calculateDrawValue, returnDegrees) {
            var that = this;

            if (calculateDrawValue !== false && that.logarithmicScale) {
                value = Math.log10(value);
            }

            var angleOffset = (value - that._drawMin) * that._angleRangeCoefficient;
            var degrees;

            if (!that.inverted) {
                degrees = that.endAngle - angleOffset;
            }
            else {
                degrees = that.startAngle + angleOffset;
            }

            if (returnDegrees) {
                return degrees;
            }
            return degrees * Math.PI / 180 + Math.PI / 2;
        },

        /**
         * Plots the Gauge's ticks and labels.
         */
        _addGaugeTicksAndLabels: function () {
            var that = this,
                maxLabelHeight = Math.max(that._tickIntervalHandler.labelsSize.minLabelSize, that._tickIntervalHandler.labelsSize.maxLabelSize),
                majorStep = that._majorTicksInterval,
                minorStep = that._minorTicksInterval,
                majorTickValues = {},
                distance = that._distance,
                radius = that._measurements.radius,
                majorTickWidth = radius - distance.majorTickDistance,
                minorTickWidth = radius - distance.minorTickDistance;
            var drawMajor, drawMinor, addLabel, currentAngle, angleAtMin, angleAtMax;

            if (that.ticksVisibility !== 'none' && that._plotTicks !== false) {
                drawMajor = function (angle) {
                    that._drawTick(angle, majorTickWidth, 'major');
                };

                drawMinor = function (value) {
                    that._drawTick(that._getAngleByValue(value, true), minorTickWidth, 'minor');
                };
            }
            else {
                drawMajor = function () { };
                drawMinor = function () { };
            }

            if (that.labelsVisibility !== 'none' && that._plotLabels !== false) {
                addLabel = function (angle, currentLabel, middle) {
                    that._drawLabel(angle, currentLabel, distance.labelDistance, middle);
                };
            }
            else {
                addLabel = function () { };
            }

            if (!that.inverted) {
                angleAtMin = that.endAngle;
                angleAtMax = that.startAngle;
            }
            else {
                angleAtMin = that.startAngle;
                angleAtMax = that.endAngle;
            }

            // first major tick and label
            currentAngle = that._getAngleByValue(that._drawMin, false);
            drawMajor(currentAngle);
            majorTickValues[that._drawMin] = true;
            addLabel(currentAngle, that.min, false);

            var second = that._drawMin - that._drawMin % majorStep,
                firstMinTick;

            if (that._drawMin >= 0) {
                second += majorStep;
            }

            // determines the value at the first minor tick
            for (var i = second; i >= that._drawMin; i = i - minorStep) {
                firstMinTick = i;
            }

            // second major tick and label
            currentAngle = that._getAngleByValue(second, false);
            drawMajor(currentAngle);
            majorTickValues[second] = true;
            if (2 * Math.PI * that._measurements.innerRadius * (this._getAngleDifference(angleAtMin, that._getAngleByValue(second, false, true)) / 360) > maxLabelHeight) {
                addLabel(currentAngle, that._getActualValue(second), second < that._drawMax);
            }

            var i;
            // middle major ticks and labels
            for (i = second + majorStep; i < that._drawMax - majorStep; i += majorStep) {
                currentAngle = that._getAngleByValue(i, false);
                drawMajor(currentAngle);
                majorTickValues[i] = true;
                addLabel(currentAngle, that._getActualValue(i), true);
            }

            if (majorTickValues[i] === undefined && i <= that._drawMax) {
                // second-to-last major tick and label
                currentAngle = that._getAngleByValue(i, false);
                drawMajor(currentAngle);
                majorTickValues[i] = true;
                if (2 * Math.PI * that._measurements.innerRadius * (this._getAngleDifference(angleAtMax, that._getAngleByValue(i, false, true)) / 360) >= maxLabelHeight) {
                    addLabel(currentAngle, that._getActualValue(i), true);
                }

                if (that._normalizedStartAngle !== that.endAngle) {
                    // last major tick and label
                    currentAngle = that._getAngleByValue(that._drawMax, false);
                    drawMajor(currentAngle);
                    majorTickValues[that._drawMax] = true;
                    if (2 * Math.PI * that._measurements.innerRadius * (this._getAngleDifference(angleAtMax, angleAtMin) / 360) >= maxLabelHeight) {
                        addLabel(currentAngle, that.max, false);
                    }
                }
            }

            // minor ticks
            if (!that.logarithmicScale) {
                for (var j = firstMinTick; j < that._drawMax; j += minorStep) {
                    if (majorTickValues[j]) {
                        continue; // does not plot minor ticks over major ones
                    }
                    drawMinor(j);
                }
            }
            else {
                this.drawGaugeLogarithmicScaleMinorTicks(majorTickValues, majorStep, drawMinor);
            }
        },

        /**
         * Returns the difference between two angles.
         */
        _getAngleDifference: function (angle1, angle2) {
            var phi = Math.abs(angle2 - angle1) % 360,
                distance = phi > 180 ? 360 - phi : phi;

            return distance;
        },

        /**
         * Returns a value based on the passed "draw" value.
         */
        _getActualValue: function (value) {
            if (!this.logarithmicScale) {
                return value;
            }
            else {
                return Math.pow(10, value);
            }
        },

        /**
         * Computes arrow body points.
         */
        _computeArrowBodyPoints: function (radius, angle, width, length) {
            var that = this,
                sin = Math.sin(angle),
                cos = Math.cos(angle),
                endX1 = radius - width * cos + length * sin,
                endY1 = radius + width * sin + length * cos,
                endX2 = radius + width * cos + length * sin,
                endY2 = radius - width * sin + length * cos,
                startX1 = radius + width * cos,
                startY1 = radius - width * sin,
                startX2 = radius - width * cos,
                startY2 = radius + width * sin,

                points = 'M ' + startX1 + ',' + startY1 + ' L ' + startX2 + ',' + startY2 + ' L ' + endX1 + ',' + endY1 + ' ' + endX2 + ',' + endY2;

            that._headCenter = { x: (endX1 + endX2) / 2, y: (endY1 + endY2) / 2 };

            return points;
        },

        /**
         * Document move handler.
         */
        _documentMoveHandler: function (event) {
            var that = this;

            if (that._dragging) {
                that._changeSelection(event, true);
            }
        },

        /**
         * Document up handler.
         */
        _documentUpHandler: function () {
            var that = this;

            if (that._dragging) {
                that._inInnerCircle = false;
                that._dragging = false;

                if (that.autoSwitchToMinutes && that.selection === 'hour') {
                    if (that.animation) {
                        $(that._picker).addClass('animate');

                        setTimeout(function () {
                            that.selection = 'minute';
                            that._changeToMinuteSelection();
                        }, 250);

                        setTimeout(function () {
                            $(that._picker).removeClass('animate');
                        }, 550);
                    }
                    else {
                        that.selection = 'minute';
                        that._changeToMinuteSelection();
                    }
                }
            }
        },

        /**
         * Draws/updates the arrow.
         */
        _drawArrow: function (update, value, noAnimation) {
            var that = this,
                hourSelection = that.selection === 'hour',
                twelveHourFormat = that.format === '12-hour';
            var current = that._oldTimePart;

            delete that._oldTimePart;

            if (value === undefined) {
                if (hourSelection) {
                    value = that.value.getHours();

                    if (twelveHourFormat && value > 12) {
                        value -= 12;
                    }
                }
                else {
                    value = that.value.getMinutes();
                }
            }

            if (current === undefined || noAnimation || !that.animation) {
                that._drawArrowSVG(update, value);
                return;
            }

            if (hourSelection && !twelveHourFormat) {
                that._animate24HourView(current, value);
                return;
            }

            var step, max;

            if (hourSelection) {
                step = 0.2;
                max = 12;
                value = value % max;
                current = current % max;
            }
            else {
                step = 1;
                max = 60;
            }

            var distanceCW = value - current,
                distanceCCW = current - value;

            if (distanceCW < 0) {
                distanceCW += max;
            }

            if (distanceCCW < 0) {
                distanceCCW += max;
            }

            if (distanceCCW < distanceCW) {
                step *= -1;
            }

            function animate() {
                current += step;
                current = parseFloat((current % max).toFixed(1));

                if (current < 0) {
                    current += max;
                }

                that._drawArrowSVG(update, current);

                if (current !== value % max) {
                    that._animationFrameId = requestAnimationFrame(animate);
                }
            }

            that._animationFrameId = requestAnimationFrame(animate);
        },

        /**
         * Animates selection in 24-hour view.
         */
        _animate24HourView: function (current, value) {
            var that = this;
            var step = 0.2;

            that._inInnerCircle = false;

            var currentInnerCircle = current === 0 || current > 12,
                valueInnerCircle = value === 0 || value > 12;

            if (currentInnerCircle !== valueInnerCircle) {
                if (currentInnerCircle) {
                    current = Math.abs(current - 12);
                }
                else {
                    current = (current + 12) % 24;
                }

                that._inInnerCircle = valueInnerCircle;
                that._drawArrowSVG(true, current);

                if (current === value) {
                    return;
                }
            }
            else {
                that._inInnerCircle = valueInnerCircle;
            }

            var start = current,
                end = value;

            if (that._inInnerCircle) {
                if (end === 0 && start < 18) {
                    end = 12;
                }
                else if (start === 0 && end < 18) {
                    start = 12;
                }
            }

            var distanceCW = end - start,
                distanceCCW = start - end;

            if (distanceCW < 0) {
                distanceCW += 12;
            }

            if (distanceCCW < 0) {
                distanceCCW += 12;
            }

            if (distanceCCW < distanceCW) {
                step *= -1;
            }

            function animate(inInnerCircle) {
                that._inInnerCircle = inInnerCircle;

                if (inInnerCircle) {
                    current = parseFloat((current + step).toFixed(1));

                    if (current < 0) {
                        current += 24;
                    }
                    else if (current < 1) {
                        current = current + 12;
                    }

                    if (current === 12 || current === 24) {
                        current = 0;
                    }
                }
                else {
                    current += step;

                    if (current !== 12) {
                        current = parseFloat((current % 12).toFixed(1));
                    }

                    if (current <= 0) {
                        current += 12;
                    }
                }

                that._drawArrowSVG(true, current);

                if (current !== value) {
                    that._animationFrameId = requestAnimationFrame(function () {
                        animate(inInnerCircle);
                    });
                }
                else {
                    that._inInnerCircle = false;
                }
            }

            that._animationFrameId = requestAnimationFrame(function () {
                animate(that._inInnerCircle);
            });
        },

        /**
         * Draws/updates the arrow.
         */
        _drawArrowSVG: function (update, value) {
            var that = this,
                measurements = that._measurements,
                angle = that._getAngleByValue(value);
            var arrowBodyPoints;

            if (!that._inInnerCircle) {
                arrowBodyPoints = that._computeArrowBodyPoints(measurements.radius, angle, 1, measurements.innerRadius - that._largestLabelSize / 2);
            }
            else {
                arrowBodyPoints = that._computeArrowBodyPoints(measurements.radius, angle, 1, measurements.innerRadius - that._largestLabelSize / 2 - 45);
            }

            if (update) {
                that._arrow.setAttribute('d', arrowBodyPoints);

                that._head.setAttribute('cx', that._headCenter.x);
                that._head.setAttribute('cy', that._headCenter.y);
                that._head.setAttribute('r', that._largestLabelSize);
                that._headRect = that._head.getBoundingClientRect();

                if (value % 1 === 0) {
                    that._highlightLabel(value);
                }
            }
            else {
                that._arrow = that._draw.path(arrowBodyPoints, { "class": that.toThemeProperty("jqx-needle jqx-widget-header") });
                that._head = that._draw.circle(that._headCenter.x, that._headCenter.y, that._largestLabelSize, { "class": that.toThemeProperty("jqx-needle-central-circle jqx-widget-header") });
                that._headRect = that._head.getBoundingClientRect();
            }
        },

        /**
         * Draws a label.
         */
        _drawLabel: function (angle, value, distance) {
            var that = this,
                measurements = that._measurements,
                r = measurements.radius,
                stylingObj = {
                    "class": that.toThemeProperty("jqx-label jqx-unselectable jqx-item"),
                    "font-size": measurements.fontSize,
                    "font-family": measurements.fontFamily,
                    "font-weight": measurements.fontWeight,
                    "font-style": measurements.fontStyle
                };

            if (that.selection === "hour") {
                if (that._plotInnerCircle) {
                    if (value > 0) {
                        value += 12;
                    }
                    else {
                        value = "00";
                    }
                }
                else {
                    if (value === 0) {
                        value = 12;
                    }
                }
            }
            else {
                if (value.toString().length === 1) {
                    value = "0" + value;
                }
            }

            var textSize = that._draw.measureText(value, 0, stylingObj),
                w = r - distance - that._largestLabelSize / 2,
                x = r + w * Math.sin(angle),
                y = r + w * Math.cos(angle),
                label = that._draw.text(value, Math.round(x) - textSize.width / 2, Math.round(y) - textSize.height / 2, textSize.width, textSize.height, 0, stylingObj);

            label.setAttribute("value", parseFloat(value));
        },

        /**
         * Gets the center coordinates.
         */
        _getCenterCoordinates: function () {
            var that = this,
                offset = that._picker.getBoundingClientRect(),
                radius = that._measurements.radius,
                scrollLeft = document.body.scrollLeft || document.documentElement.scrollLeft,
                scrollTop = document.body.scrollTop || document.documentElement.scrollTop;

            return { x: offset.left + scrollLeft + radius, y: offset.top + scrollTop + radius };
        },

        /**
         * Header click handler.
         */
        _headerClickHandler: function (event) {
            var that = this;

            if (that.disabled || that.readonly) {
                return;
            }

            switch (event.target) {
                case that._hourContainer:
                    if (that.selection !== "hour") {
                        that.selection = "hour";
                        that._changeToHourSelection();
                    }

                    break;
                case that._minuteContainer:
                    if (that.selection !== "minute") {
                        that.selection = "minute";
                        that._changeToMinuteSelection();
                    }

                    break;
                case that._amContainer:
                    if (!$(that._amContainer).hasClass("jqx-selected")) {
                        that._selectAmPm("am");

                        var oldValue = new Date(that.value.getTime());

                        that.value.setHours(that.value.getHours() - 12);

                        // Update the hidden input 
                        that._hiddenInput.value = that.value;

                        that._raiseEvent("0", { "value": that.value, "oldValue": oldValue });
                    }

                    break;
                case that._pmContainer:
                    if (!$(that._pmContainer).hasClass("jqx-selected")) {
                        that._selectAmPm("pm");

                        var oldValue = new Date(that.value.getTime());

                        that.value.setHours(that.value.getHours() + 12);

                        // Update the hidden input 
                        that._hiddenInput.value = that.value;

                        that._raiseEvent("0", { "value": that.value, "oldValue": oldValue });
                    }

                    break;
            }
        },

        /**
         * Highlights a label.
         */
        _highlightLabel: function (value) {
            var that = this;

            if (that._highlightedLabel) {
                if (parseFloat(that._highlightedLabel.getAttribute("value")) === value) {
                    return;
                }

                var removeClass = function (sentence, nameClass) {
                    var pattern = new RegExp("\\s?" + nameClass + "\\s?", "gm");
                    var result = sentence.replace(pattern, "");
                    return result;
                };

                if (that._highlightedLabel.className.baseVal.indexOf(that.toThemeProperty("jqx-selected")) > -1) {
                    var classString = that._highlightedLabel.className.baseVal;
                    var result = removeClass(classString, that.toThemeProperty("jqx-selected"));

                    that._highlightedLabel.className.baseVal = result;
                }
                
                that._highlightedLabel = undefined;
            }

            var roundedValue;

            if (that.selection === "hour") {
                if (value === undefined) {
                    value = that.value.getHours();
                }

                if (that.format === "12-hour") {
                    if (value === 0) {
                        value = 12;
                    }
                    else if (value > 12) {
                        value -= 12;
                    }
                }

                roundedValue = value;
            }
            else {
                if (value === undefined) {
                    value = that.value.getMinutes();
                }

                roundedValue = Math.round(value / 5) * 5;

                if (roundedValue === 60) {
                    roundedValue = 0;
                }
            }

            var labelAtValue = that._picker.querySelector('.jqx-label[value="' + roundedValue + '"]');

            if (labelAtValue && (roundedValue === value || that._overlapsLabel(labelAtValue))) {
                that._highlightedLabel = labelAtValue;
                labelAtValue.className.baseVal += " " + that.toThemeProperty("jqx-selected");
            }
        },

        _isVisible: function () {
            var that = this.host[0];
            return !!(that.offsetWidth || that.offsetHeight || that.getClientRects().length);
        },

        _getMeasurements: function () {
            var that = this,
                measurements = that._measurements,
                measureLabel = document.createElement('div');
            var minLabel, maxLabel, minLabelWidth, maxLabelWidth, minLabelHeight, maxLabelHeight;

            measureLabel.className = 'jqx-label';
            measureLabel.style.position = 'absolute';
            measureLabel.style.visibility = 'hidden';
            that._svgContainer.appendChild(measureLabel);

            if (that.selection === 'hour') {
                minLabel = '1';
                maxLabel = '23';

                that.max = 12;
                that._drawMax = '12';
                that._range = 12;
            }
            else {
                minLabel = '00';
                maxLabel = '55';

                that.max = 60;
                that._drawMax = '60';
                that._range = 60;
            }

            measureLabel.innerHTML = minLabel;
            minLabelWidth = measureLabel.offsetWidth;
            minLabelHeight = measureLabel.offsetHeight;
            measureLabel.innerHTML = maxLabel;
            maxLabelWidth = measureLabel.offsetWidth;
            maxLabelHeight = measureLabel.offsetHeight;

            that._largestLabelSize = Math.max(minLabelWidth, minLabelHeight, maxLabelWidth, maxLabelHeight);
            that._tickIntervalHandler.labelsSize.minLabelSize = minLabelHeight;
            that._tickIntervalHandler.labelsSize.maxLabelSize = maxLabelHeight;

            var measureElementStyle = window.getComputedStyle(measureLabel);

            measurements.fontSize = measureElementStyle.fontSize;
            measurements.fontFamily = measureElementStyle.fontFamily;
            measurements.fontWeight = measureElementStyle.fontWeight;
            measurements.fontStyle = measureElementStyle.fontStyle;
            that._svgContainer.removeChild(measureLabel);
        },

        /**
         * Renders hours view.
         */
        _renderHours: function () {
            var that = this;

            that._highlightedLabel = undefined;

            that._majorTicksInterval = 1;
            that._addGaugeTicksAndLabels();

            if (that.format === '24-hour') {
                that._plotInnerCircle = true;
                that._distance.labelDistance = 55;
                that._addGaugeTicksAndLabels();
                that._plotInnerCircle = false;
                that._distance.labelDistance = 10;
            }
        },

        /**
         * Renders minutes view.
         */
        _renderMinutes: function () {
            var that = this;

            that._highlightedLabel = undefined;

            that._majorTicksInterval = 5;

            that._addGaugeTicksAndLabels();
        },

        /**
         * Renders all SVG elements.
         */
        _renderSVG: function () {
            var that = this;

            if (!that._isVisible() || that._renderingSuspended) {
                that._renderingSuspended = true;
                return;
            }

            that._centralCircle = that._draw.circle(that._measurements.radius, that._measurements.radius, 4, { "class": that.toThemeProperty("jqx-needle-central-circle jqx-widget-header") });

            if (that.selection === "hour" && that.format === "24-hour" && (that.value.getHours() === 0 || that.value.getHours() > 12)) {
                that._inInnerCircle = true;
            }

            that._drawArrow(false);
            that._inInnerCircle = false;

            if (that.selection === "hour") {
                that.interval = 1;
                that._renderHours();

                var hours = that.value.getHours();

                if (that.format === "12-hour" && hours > 12) {
                    hours -= 12;
                }

                that._highlightLabel(hours);
            }
            else {
                that.interval = that.minuteInterval;
                that._renderMinutes();
                that._highlightLabel(that.value.getMinutes());
            }
        },

        /**
         * Selects the appropriate AM/PM label.
         */
        _selectAmPm: function (which) {
            var that = this;

            if (which === "am") {
                that._ampm = "am";
                $(that._pmContainer).removeClass(that.toThemeProperty("jqx-selected"));
                $(that._amContainer).addClass(that.toThemeProperty("jqx-selected"));
            }
            else {
                that._ampm = "pm";
                $(that._amContainer).removeClass(that.toThemeProperty("jqx-selected"));
                $(that._pmContainer).addClass(that.toThemeProperty("jqx-selected"));
            }
        },

        /**
         * Sets whether the element can be focused.
         */
        _setFocusable: function () {
            var that = this;

            if (that.disabled || that.unfocusable) {                
                that._hourContainer.removeAttribute("tabindex");
                that._minuteContainer.removeAttribute("tabindex");
                that._amContainer.removeAttribute("tabindex");
                that._pmContainer.removeAttribute("tabindex");
                that._picker.removeAttribute("tabindex");
                return;
            }
            
            var index = that.tabIndex > 0 ? that.tabIndex : 0;
            
            that._hourContainer.tabIndex = index;
            that._minuteContainer.tabIndex = index;
            that._amContainer.tabIndex = index;
            that._pmContainer.tabIndex = index;
            that._picker.tabIndex = index;
        },

        /**
         * Sets the SVG picker's size.
         */
        _setPickerSize: function () {
            var that = this,
                parentWidth = that._svgContainer.offsetWidth,
                parentHeight = that._svgContainer.offsetHeight;
            var size = Math.min(parentWidth, parentHeight) * 0.9;

            if (that._pickerSize !== undefined && that._pickerSize !== size) {
                that._sizeChanged = true;
            }
            else {
                that._sizeChanged = false;
            }

            that._pickerSize = size;
            that._measurements.radius = size / 2;
            that._measurements.innerRadius = that._measurements.radius - 10;

            size += 'px';
            that._picker.style.width = size;
            that._picker.style.height = size;
        },

        refresh: function (initialRefresh) {
            var that = this;
            if (initialRefresh !== true) {
                that.render();
            }
        },

        /**
         * Sets the hours.
         *
         * @param {Number} hours The hours to set.
         */
        setHours: function (hours) {
            var that = this;
            if (hours == undefined || (isNaN(hours) && Object.keys(hours).length == 0)) {
                throw "The setHours method required a parameter. Its type should be number.";
            }

            if (hours === 24) {
                hours = 0;
            }
            else {
                hours = Math.max(0, Math.min(hours, 23));
            }

            if (hours < 12) {
                that._selectAmPm('am');
            }
            else {
                that._selectAmPm('pm');
            }

            if (that.format === '12-hour') {
                if (hours === 0) {
                    hours = 12
                }
                else if (hours > 12) {
                    hours -= 12;
                }
            }

            that._updateHours(hours, arguments[1]);

            if (that.selection === 'hour') {
                that._inInnerCircle = that.format === '24-hour' && (hours === 0 || hours > 12);

                that._drawArrow(true, hours, arguments[2]);

                if (!that.animation) {
                    that._inInnerCircle = false;
                }
            }
        },

        /**
         * Sets the minutes.
         *
         * @param {Number} minutes The minutes to set.
         */
        setMinutes: function (minutes) {
            var that = this;
            if (minutes == undefined || (isNaN(minutes) && Object.keys(minutes).length == 0)) {
                throw "The setMinutes method required a parameter. Its type should be number.";
            }

            if (minutes === 60) {
                minutes = 0;
            }
            else {
                minutes = Math.max(0, Math.min(minutes, 59));
            }

            that._updateMinutes(minutes);

            if (that.selection === 'minute') {
                that._drawArrow(true, minutes, arguments[1]);
            }
        },

        propertyChangedHandler: function (object, key, oldValue, value) {
            var that = object;

            if (oldValue != value || value instanceof Object) {
                switch (key) {
                    case 'disabled':
                        if (that.disabled) {                            
                            that.host.addClass(that.toThemeProperty("jqx-fill-state-disabled"));
                            that.element.setAttribute("disabled", "");
                        } else {
                            that.host.removeClass(that.toThemeProperty("jqx-fill-state-disabled"));
                            that.element.removeAttribute("disabled");
                        }

                        that._setFocusable();
                        break;
                    case 'footer':
                    case 'view':
                        that._resizeHandler();
                        break;
                    case 'footerTemplate':
                        that._validateFooterTemplate();
                        break;
                    case 'format': {
                        var hours = that.value.getHours();

                        if (value === '12-hour') {
                            that._ampmContainer.removeClass('jqx-hidden');

                            if (that.value.getHours() < 12) {
                                that._selectAmPm('am');
                            }
                            else {
                                that._selectAmPm('pm');
                            }

                            if (hours === 0) {
                                hours = 12;
                            }
                            else if (hours > 12) {
                                hours -= 12;
                            }
                        }
                        else {
                            that._ampmContainer.addClass('jqx-hidden');
                        }

                        that._hourContainer.innerHTML = hours;

                        if (that.selection === 'hour') {
                            that._draw.clear();
                            that._renderSVG();
                        }

                        break;
                    }
                    case 'minuteInterval': {
                        var validValue = Math.max(1, Math.min(value, 60));

                        if (validValue !== value) {
                            that.minuteInterval = validValue;
                        }

                        if (that.selection === 'minute') {
                            that.interval = validValue;
                        }

                        break;
                    }
                    case 'selection':
                        if (value === 'hour') {
                            that._changeToHourSelection();
                        }
                        else {
                            that._changeToMinuteSelection();
                        }

                        break;
                    case 'value': {
                        that._oldValue = oldValue;
                        that._validateValue();

                        var equalHours = that.value.getHours() === oldValue.getHours(),
                            equalMinutes = that.value.getMinutes() === oldValue.getMinutes();

                        if (!(equalHours && equalMinutes)) {
                            if (equalMinutes) {
                                that.setHours(that.value.getHours());
                            }
                            else {
                                if (!equalHours) {
                                    that.setHours(that.value.getHours(), true);
                                }

                                that.setMinutes(that.value.getMinutes());
                            }
                        }

                        delete that._oldValue;
                        break;
                    }
                }

            }
        },

        /**
         * Applies initial settings to the TimePicker element.
         */
        _applyInitialSettings: function () {
            var that = this,
                value = that.value;
            var hours, minutes;

            hours = value.getHours();
            minutes = value.getMinutes();

            if (that.format === "12-hour") {
                if (hours < 12) {
                    that._ampm = "am";
                    that._amContainer.className += " " + that.toThemeProperty("jqx-selected");

                    if (hours === 0) {
                        hours = 12;
                    }
                }
                else {
                    that._ampm = "pm";
                    that._pmContainer.className += " " + that.toThemeProperty("jqx-selected");

                    if (hours > 12) {
                        hours -= 12;
                    }
                }
            }
            else {
                that._ampmContainer.className += " " + that.toThemeProperty("jqx-hidden");
            }

            minutes = minutes.toString();

            if (minutes.length === 1) {
                minutes = "0" + minutes;
            }

            that._hourContainer.innerHTML = hours;
            that._minuteContainer.innerHTML = minutes;

            if (that.selection === "hour") {
                that._hourContainer.className += " " + that.toThemeProperty("jqx-selected");
            }
            else {
                that._minuteContainer.className += " " + that.toThemeProperty("jqx-selected");
            }
        },

        /**
         * Returns the value equivalent of an angle.
         */
        _getValueByAngle: function (angle, integer) {
            var that = this;
            var minuendAngle, subtrahendAngle, value;

            if (!that.inverted) {
                minuendAngle = that.endAngle;
                subtrahendAngle = angle;
            }
            else {
                minuendAngle = angle;
                subtrahendAngle = that._normalizedStartAngle;
            }

            while (minuendAngle < subtrahendAngle) minuendAngle += 360;

            value = ((minuendAngle - subtrahendAngle) / that._angleDifference) * that._range + parseFloat(that._drawMin);

            if (that.logarithmicScale) {
                if (that.customInterval) {
                    return parseFloat(Math.pow(10, this.getCoercedValue(value, true)).toFixed(12));
                }

                value = Math.pow(10, value);
            }

            if (integer && !that.coerce) {
                return Math.round(value);
            }

            return this._getCoercedValue(value, false);
        },

        /**
         * Returns a coerced value based on the interval.
         */
        _getCoercedValue: function (value, useDrawVariables, logarithmicGauge) {
            var that = this;

            if (!that.coerce) {
                return value;
            }

            var normalScale = !that.logarithmicScale,
                minValue, maxValue;

            minValue = parseFloat(that.min);
            maxValue = parseFloat(that.max);


            var noMin = value - minValue,
                remainder = noMin % parseFloat(that.interval),
                coef = 12;

            if (remainder === 0) {
                return value;
            }

            var lowerValue = parseFloat((noMin - remainder).toFixed(coef)),
                greaterValue = lowerValue + parseFloat(that.interval);

            if (((that.max - that.min) <= parseFloat(that.interval)) && normalScale) {
                var min = minValue,
                    max = maxValue;

                return value >= min + (max - min) / 2 ? max : min;
            }

            if (Math.abs(noMin - lowerValue) < Math.abs(noMin - greaterValue)) {
                return lowerValue + minValue;
            }
            else {
                var biggerValue = greaterValue + minValue;

                return biggerValue > maxValue ? lowerValue + minValue : biggerValue;
            }
        },

        /**
         * Changes the TimePicker selection when the user interacts with it.
         */
        _changeSelection: function (event, noAnimation) {
            var that = this,
                x = event.pageX,
                y = event.pageY,
                center = that._getCenterCoordinates(),
                distanceFromCenter = Math.sqrt(Math.pow(center.x - x, 2) + Math.pow(center.y - y, 2));

            that._measurements.center = center;

            if (event.type === 'mousedown') {
                if (distanceFromCenter > that._measurements.radius) {
                    event.stopPropagation();
                    return;
                }
                else {
                    that._dragging = true;
                }
            }

            if (that.format === '24-hour' && that.selection === 'hour' && distanceFromCenter < that._measurements.radius - 50) {
                that._inInnerCircle = true;
            }
            else {
                that._inInnerCircle = false;
            }

            var angleRadians = Math.atan2(y - center.y, x - center.x);
            var angleDeg = -1 * angleRadians * 180 / Math.PI;

            if (angleDeg < 0) {
                angleDeg += 360;
            }

            that._angle = angleDeg;

            var newValue = that._getValueByAngle(that._angle);

            if (that.selection === 'hour') {
                if (that.format === '24-hour') {
                    if (that._inInnerCircle) {
                        if (newValue !== 0 && newValue !== 12) {
                            newValue += 12;
                        }
                        else {
                            newValue = 0;
                        }
                    }
                    else if (newValue === 0) {
                        newValue = 12;
                    }
                }
                else {
                    if (newValue === 0) {
                        newValue = 12;
                    }
                }

                that._updateHours(newValue);
            }
            else {
                if (newValue === 60) {
                    newValue = 0;
                }

                that._updateMinutes(newValue);
            }

            if (that._oldTimePart === undefined) {
                return;
            }

            cancelAnimationFrame(that._animationFrameId);
            that._drawArrow(true, newValue, noAnimation);
        },

        /**
         * Changes to hour selection.
         */
        _changeToHourSelection: function () {
            var that = this,
                svgCanvas = that._centralCircle.parentElement || that._centralCircle.parentNode;

            cancelAnimationFrame(that._animationFrameId);
            that.interval = 1;

            $(that._hourContainer).addClass(that.toThemeProperty("jqx-selected"));
            $(that._minuteContainer).removeClass(that.toThemeProperty("jqx-selected"));

            svgCanvas.removeChild(that._centralCircle);
            svgCanvas.removeChild(that._arrow);
            svgCanvas.removeChild(that._head);

            that._getMeasurements();
            that._getAngleRangeCoefficient();
            that._draw.clear();

            svgCanvas.appendChild(that._centralCircle);
            svgCanvas.appendChild(that._arrow);
            svgCanvas.appendChild(that._head);

            that._renderHours();

            if (that.format === "24-hour" && (that.value.getHours() === 0 || that.value.getHours() > 12)) {
                that._inInnerCircle = true;
            }

            that._drawArrow(true, undefined, true);
            that._inInnerCircle = false;
        },

        /**
         * Changes to minute selection.
         */
        _changeToMinuteSelection: function () {
            var that = this,
                svgCanvas = that._centralCircle.parentElement || that._centralCircle.parentNode;

            that._inInnerCircle = false;

            cancelAnimationFrame(that._animationFrameId);
            that.interval = that.minuteInterval;

            $(that._hourContainer).removeClass(that.toThemeProperty("jqx-selected"));
            $(that._minuteContainer).addClass(that.toThemeProperty("jqx-selected"));

            svgCanvas.removeChild(that._centralCircle);
            svgCanvas.removeChild(that._arrow);
            svgCanvas.removeChild(that._head);

            that._getMeasurements();
            that._getAngleRangeCoefficient();
            that._draw.clear();

            svgCanvas.appendChild(that._centralCircle);
            svgCanvas.appendChild(that._arrow);
            svgCanvas.appendChild(that._head);

            that._renderMinutes();

            that._drawArrow(true, undefined, true);
        },

        /**
         * Returns if the arrow head overlaps a label.
         */
        _overlapsLabel: function (label) {
            var that = this,
                labelRect = label.getBoundingClientRect(),
                headRect = that._headRect,
                overlap = !(labelRect.right - 10 < headRect.left ||
                    labelRect.left + 10 > headRect.right ||
                    labelRect.bottom - 10 < headRect.top ||
                    labelRect.top + 10 > headRect.bottom);

            return overlap;
        },

        _parseDateString: function (value, referenceValue) {
            var indexOfDate = value.indexOf("Date("),
                indexOfBracket = value.indexOf(")");
            var validValue = value;

            if (indexOfDate !== -1 && indexOfBracket !== -1) {
                validValue = value.slice(indexOfDate + 5, indexOfBracket);
                validValue = validValue.replace(/'/g, '').replace(/"/g, '').replace(/^\s+|\s+$|\s+(?=\s)/g, '');

                if (validValue.trim() === '') {
                    return new Date();
                }

                if (new RegExp(/(^(\d+)(\s*,\s*\d+)+$)/g).test(validValue)) {
                    validValue = validValue.replace(/\s/g, '');
                    validValue = validValue.split(',');

                    validValue.map(function (argument, index) {
                        validValue[index] = parseInt(argument);
                    });

                    validValue.unshift(null);
                    validValue = new (Function.prototype.bind.apply(Date, validValue));

                    return validValue;
                }
            }

            if (validValue.trim() === '') {
                return referenceValue;
            }

            if (!isNaN(validValue)) {
                return new Date(parseInt(validValue, 10));
            }

            try {
                validValue = new Date(validValue);
            }
            catch (error) {
                validValue = referenceValue;
            }

            if (isNaN(validValue.getTime())) {
                return referenceValue;
            }

            return validValue;
        },

        /**
         * Updates the hours.
         */
        _updateHours: function (hours, suppressEvent) {
            var that = this;
            var actualHours = hours;

            if (that.format === '12-hour') {
                if (that._ampm === 'am') {
                    if (actualHours === 12) {
                        actualHours = 0;
                    }
                }
                else if (actualHours < 12) {
                    actualHours += 12;
                }
            }
            else {
                actualHours = hours;
            }

            var oldValue = that._oldValue !== undefined ? that._oldValue : new Date(that.value.getTime()),
                oldHours = oldValue.getHours();

            if (actualHours === oldHours) {
                delete that._oldTimePart;
                return;
            }

            that._oldTimePart = oldHours;

            if (that._oldValue === undefined) {
                that.value.setHours(actualHours);
            }

            that._hourContainer.innerHTML = hours;

            if (suppressEvent !== true) {
                // Update the hidden input 
                that._hiddenInput.value = that.value;

                that._raiseEvent('0', { 'value': that.value, 'oldValue': oldValue });
            }
        },

        /**
         * Updates the minutes.
         */
        _updateMinutes: function (minutes) {
            var that = this,
                oldValue = that._oldValue !== undefined ? that._oldValue : new Date(that.value.getTime()),
                oldMinutes = oldValue.getMinutes();

            if (minutes === oldMinutes) {
                delete that._oldTimePart;
                return;
            }

            that._oldTimePart = oldMinutes;

            if (that._oldValue === undefined) {
                that.value.setMinutes(minutes);
            }

            minutes = minutes.toString();

            if (minutes.length === 1) {
                minutes = '0' + minutes;
            }

            that._minuteContainer.innerHTML = minutes;

            // Update the hidden input 
            that._hiddenInput.value = that.value;

            that._raiseEvent('0', { 'value': that.value, 'oldValue': oldValue });
        },

        _validateFooterTemplate: function () {
            var that = this,
                footerTemplate = that.footerTemplate;           
                
            // Check for IE browser
            var userAgent = window.navigator.userAgent;
            var isIE = userAgent.indexOf("MSIE") > -1 || userAgent.indexOf("NET") > -1;

            if (footerTemplate === null || isIE) {
                that._footer.innerHTML = '';
                return;
            }

            var potentialHTMLTemplate;

            if (footerTemplate instanceof HTMLTemplateElement) {
                potentialHTMLTemplate = footerTemplate;
            }
            else if (typeof footerTemplate === 'string') {
                potentialHTMLTemplate = document.getElementById(footerTemplate);

                if (!(potentialHTMLTemplate instanceof HTMLTemplateElement)) {
                    potentialHTMLTemplate = undefined;
                }
            }

            if (potentialHTMLTemplate === undefined) {
                that.footerTemplate = null;
                that._footer.innerHTML = '';
                return;
            }

            var templateContent = document.importNode(potentialHTMLTemplate.content, true);

            that._footer.appendChild(templateContent);
        },

        _validateInitialPropertyValues: function () {
            var that = this;

            that._validateFooterTemplate();

            that.minuteInterval = Math.max(1, Math.min(that.minuteInterval, 60));

            that._validateValue();
        },

        _validateValue: function () {
            var that = this,
                value = that.value,
                referenceValue = that._oldValue !== undefined ? this._oldValue : new Date();

            if (value instanceof Date) {
                return;
            }
            else if (typeof value === 'string') {
                if (/^\d{1,2}:\d{1,2}$/.test(value)) {
                    var timeParts = value.split(':');

                    that.value = new Date(
                        referenceValue.getFullYear(),
                        referenceValue.getMonth(),
                        referenceValue.getDate(),
                        parseFloat(timeParts[0]),
                        parseFloat(timeParts[1]));
                    return;
                }

                that.value = that._parseDateString(value, referenceValue);
            }
            else {
                that.value = referenceValue;
            }
        },

        /**
         * destroy jqxTimePicker.
         */
        destroy: function () {
            var that = this;
            that._removeEventHandlers();
            that._draw.clear();
            that.host.removeData();
            that.host.removeClass();
            that.host.remove();
            delete that.element;
            delete that.host;
        },

        /**
         * keydown handler.
         */
        _keydownHandler: function (event) {
            var that = this;

            if (that._dragging) {
                return;
            }

            var activeElement = that.enableShadowDOM ? (that.shadowRoot.activeElement || document.activeElement) : document.activeElement,
                key = event.key;

            if (that._header.contains(activeElement) && (key === 'Enter' || key === ' ')) {
                event.preventDefault();
                that._headerClickHandler({ target: activeElement });
            }
            else if (activeElement === that._picker && !event.altKey) {
                var coefficient;

                if (key === 'ArrowRight' || key === 'ArrowUp') {
                    coefficient = 1;
                }
                else if (key === 'ArrowLeft' || key === 'ArrowDown') {
                    coefficient = -1;
                }

                if (coefficient) {
                    event.preventDefault();

                    if (that.selection === 'hour') {
                        var hours = that.value.getHours();

                        if (hours === 0 && coefficient === -1) {
                            hours = 23;
                        }
                        else if (hours === 23 && coefficient === 1) {
                            hours = 0;
                        }
                        else {
                            hours += coefficient;
                        }

                        if (that.format === '12-hour') {
                            if (hours >= 12 && $(that._amContainer).hasClass("jqx-selected")) {
                                hours -= 12;
                            }
                            else if (hours < 12 && $(that._pmContainer).hasClass("jqx-selected")) {
                                hours += 12;
                            }
                        }

                        that.setHours(hours, undefined, true);
                    }
                    else {
                        var minutes = that.value.getMinutes();

                        coefficient *= that.minuteInterval;

                        if (minutes + coefficient >= 60) {
                            minutes = 0;
                        }
                        else if (minutes < coefficient * -1) {
                            if (60 % coefficient === 0) {
                                minutes = 60 + coefficient;
                            }
                            else {
                                minutes = 60 - (60 % coefficient);
                            }
                        }
                        else {
                            minutes += coefficient;
                        }

                        that.setMinutes(minutes, true);
                    }
                }
            }
        },

        /**
         * Gauge resize handler.
         */
        _resizeHandler: function () {
            var that = this;
            var shown = false;

            if (!that._isVisible()) {
                that._renderingSuspended = true;
                return;
            }
            else {
                that._renderingSuspended = false;
                shown = true;

                that._getMeasurements();
                that._getAngleRangeCoefficient();
            }

            that._setPickerSize();

            if (that._sizeChanged || shown) {
                that._draw.clear();
                that._renderSVG();
                that._sizeChanged = false;
                that._draw.refresh();
            }
            else {
                that._headRect = that._head.getBoundingClientRect();
                that._highlightLabel();
            }
        },

        /**
         * SVG picker mousedown event handler.
         */
        _pickerDownHandler: function (event) {
            var that = this;

            if (that.disabled || that.readonly || !jqx.mobile.isTouchDevice() && event.which !== 1) {
                return;
            }

            that._changeSelection(event);
        },

        /**
         * SVG picker move event handler.
         */
        _pickerMoveHandler: function (event) {
            // if (event.originalEvent.type === 'touchmove') {
            //     event.originalEvent.preventDefault();
            // }
        },

        _addEventHandlers: function () {
            var that = this;
            that.addHandler($(that.element), 'keydown.timepicker' + that.widgetID, that._keydownHandler.bind(this));
            that.addHandler($(that.element), 'resize.timepicker' + that.widgetID, that._resizeHandler.bind(this));
            that.addHandler($(that._header), 'click.timepicker' + that.widgetID, that._headerClickHandler.bind(this));
            that.addHandler($(that._picker), 'mousedown.timepicker' + that.widgetID, that._pickerDownHandler.bind(this));
            // that.addHandler($(that._picker), 'mousemove.timepicker' + that.widgetID, that._pickerMoveHandler);
            that.addHandler($(document), 'mousemove.timepicker' + that.widgetID, that._documentMoveHandler.bind(this));
            that.addHandler($(document), 'mouseup.timepicker' + that.widgetID, that._documentUpHandler.bind(this));
        },

        _removeEventHandlers: function () {
            var that = this;
            that.removeHandler($(that.element), 'keydown.timepicker' + that.widgetID);
            that.removeHandler($(that.element), 'resize.timepicker' + that.widgetID);
            that.removeHandler($(that._header), 'click.timepicker' + that.widgetID);
            that.removeHandler($(that._picker), 'mousedown.timepicker' + that.widgetID);
            that.removeHandler($(that._picker), 'mousemove.timepicker' + that.widgetID);
            that.removeHandler($(document), 'mousemove.timepicker' + that.widgetID);
            that.removeHandler($(document), 'mouseup.timepicker' + that.widgetID);
        },

        _raiseEvent: function (id, args) {
            var evt = this._events[id];
            var event = new $.Event(evt);
            event.owner = this;
            event.args = args;

            var result;

            try {
                result = this.host.trigger(event);
            }
            catch (error) {
            }

            return result;
        },

        _getEvent: function (event) {
            var that = this;
            if (that._isTouchDevice) {
                return that._touchEvents[event];
            } else {
                return event;
            }
        }
    });
})(jqxBaseFramework);


