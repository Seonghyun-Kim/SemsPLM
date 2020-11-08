/*
jQWidgets v10.1.2 (2020-Sep)
Copyright (c) 2011-2020 jQWidgets.
License: https://jqwidgets.com/license/
*/
/* eslint-disable */

/* tslint:disable */
/* eslint-disable */
(function ($) {

    $.jqx.jqxWidget("jqxHeatMap", "", {});

    $.extend($.jqx._jqxHeatMap.prototype, {
        defineInstance: function () {
            var settings = {
                width: 'auto',
                xAxis: {
                    labels: [],
                    opposedPosition: false,
                    isInversed: false,
                    minimum: null,
                    maximum: null,
                    labelFormat: 'short', // 'numeric', '2-digit', 'narrow', 'short', 'long'
                },
                yAxis: {
                    labels: [],
                    opposedPosition: false,
                    isInversed: false,
                },
                source: [],
                title: 'Title',
                paletteSettings: {
                    palette: [
                        { value: 0, color: '#5dc3f0', label: '' },
                        { value: 50, color: '#4bb1de', label: '' },
                        { value: 100, color: '#3885a6', label: '' }
                    ],
                    type: 'Gradient', // 'Gradient', 'Fixed'
                    emptyPointColor: '#eee'
                },
                legendSettings: {
                    position: 'Right' // 'Top', 'Bottom', 'Left', 'Right'
                },
                tooltipRender: null,
                _isXAxisInversed: false,
                _isYAxisInversed: false,
            };
            if (this === $.jqx._jqxHeatMap.prototype) {
                return settings;
            }
            $.extend(true, this, settings);
            return settings;
        },

        createInstance: function () {
            var that = this;

            if ('NodeList' in window && !NodeList.prototype.forEach) {
                console.info('polyfill for IE11');
                NodeList.prototype.forEach = function (callback, thisArg) {
                  thisArg = thisArg || window;
                  for (var i = 0; i < this.length; i++) {
                    callback.call(thisArg, this[i], i, this);
                  }
                };
              }
              
            that._setDefaultValues();
            that._renderAllElements();
            that._addHandlers();
        },

        _addHandlers: function () {
            var that = this;

            that.host.find('.jqx-heatmap-cell').on('mouseenter', function (event) {
                var target = event.target;
                var value = target.getAttribute('data-value');

                if (that.legendSettings.position === 'Top' || that.legendSettings.position === 'Bottom') {
                    that._showLegendArrow('horizontal', value)
                } else {
                    that._showLegendArrow('vertical', value)
                }

                if (that._hideTimer) {
                    clearTimeout(that._hideTimer);
                }

                that._showTooltip(event.target);
                target.classList.toggle('jqx-opacity-50');
            });

            that.host.find('.jqx-heatmap-cell').on('mouseleave', function () {
                var target = event.target;
                var hideTimer;

                that._arrow.style.display = '';
                target.classList.toggle('jqx-opacity-50');

                hideTimer = setTimeout(function () {
                    that._tooltipContainer.classList.add('jqx-heatmap-hidden');
                }, 300);

                that._hideTimer = hideTimer;
            });

            that.host.find('.jqx-color-container, .jqx-color-container-tick').on('click', function () {
                var value = $(this).attr('data-value')
                var colorIndex = $(this).attr('data-color-index')

                that._hideCells(colorIndex);
                that._toggleClass('jqx-color-container', 'jqx-selected-color', value);
                that._toggleClass('jqx-color-container-tick', 'jqx-opacity-50', value);
            });
        },

        _setDefaultValues: function () {
            var that = this;
            var xAxisProperties = Object.keys(that.xAxis);
            var yAxisProperties = Object.keys(that.yAxis);
            var legendSettingsProperties = Object.keys(that.legendSettings);
            var paletteSettingsProperties = Object.keys(that.paletteSettings);

            if (xAxisProperties.indexOf('labels') === -1) {
                that.xAxis.labels = [];
            }
            if (xAxisProperties.indexOf('opposedPosition') === -1) {
                that.xAxis.opposedPosition = false;
            }
            if (xAxisProperties.indexOf('isInversed') === -1) {
                that.xAxis.isInversed = false;
            }
            if (yAxisProperties.indexOf('opposedPosition') === -1) {
                that.yAxis.opposedPosition = false;
            }
            if (yAxisProperties.indexOf('isInversed') === -1) {
                that.yAxis.isInversed = false;
            }
            if (legendSettingsProperties.indexOf('position') === -1) {
                that.legendSettings.position = 'Right';
            }
            if (paletteSettingsProperties.indexOf('type') === -1) {
                that.paletteSettings.type = 'Gradient';
            }
            if (paletteSettingsProperties.indexOf('palette') === -1) {
                that.paletteSettings.palette = [
                    { value: 0, color: '#5dc3f0', label: '' },
                    { value: 50, color: '#4bb1de', label: '' },
                    { value: 100, color: '#3885a6', label: '' }
                ];
            }
            if (paletteSettingsProperties.indexOf('emptyPointColor') === -1) {
                that.paletteSettings.emptyPointColor = '#eee';
            }
        },

        _showTooltip: function (targetElement) {
            var that = this;
            var tooltipContainer = document.createElement('div');
            var value = targetElement.getAttribute('data-value');
            var date = targetElement.getAttribute('data-date');
            var xLabelIndex = targetElement.getAttribute('data-column');
            var xLabel = that.xAxis.labels[xLabelIndex];
            var yLabelIndex = targetElement.getAttribute('data-row');
            var yLabel = that.yAxis.labels[yLabelIndex];
            var content = xLabel + ' | ' + yLabel + ' : ' + value;
            var targetElementWidth = targetElement.offsetWidth;
            var tooltipArrowHeight = 10;
            var tooltipWidth;
            var tooltipHeight;
            var widthDifference;

            if (!that._tooltipContainer) {
                that._tooltipContainer = tooltipContainer;
            }

            that.element.appendChild(that._tooltipContainer);

            that._tooltipContainer.classList.remove('jqx-heatmap-hidden');
            that._tooltipContainer.className = 'jqx-heatmap-tooltip';

            if (that.tooltipRender) {
                if (that.xAxis.minimum && that.xAxis.maximum) {
                    var args = { xLabel: xLabel, yLabel: yLabel, value: value, date: date, content: content };
                } else {
                    var args = { xLabel: xLabel, yLabel: yLabel, value: value, content: content };
                }

                that.tooltipRender(args);
                content = args.content;
            }

            if (that.xAxis.minimum && that.xAxis.maximum) {
                that._tooltipContainer.innerHTML = value + ' on ' + that._defaultDateTooltipFormat(new Date(date));
            } else {
                that._tooltipContainer.innerHTML = content;
            }

            tooltipWidth = that._tooltipContainer.offsetWidth;
            tooltipHeight = that._tooltipContainer.offsetHeight;
            widthDifference = tooltipWidth - targetElementWidth;

            that._tooltipContainer.style.top = (targetElement.offsetTop - tooltipHeight + tooltipArrowHeight) + 'px';

            if (widthDifference > 0) {
                that._tooltipContainer.style.left = (targetElement.offsetLeft - widthDifference / 2) + 'px';
            } else {
                that._tooltipContainer.style.left = (targetElement.offsetLeft + Math.abs(widthDifference) / 2) + 'px';
            }
        },

        _defaultDateTooltipFormat: function (date) {
            var options = {
                day: 'numeric',
                weekday: 'short',
                month: 'short',
                year: 'numeric'
            };
            var intl = new Intl.DateTimeFormat('en-US', options);

            return intl.format(date);
        },

        _getAllDates: function () {
            var that = this;
            var minDate = new Date(that.xAxis.minimum);
            var maxDate = new Date(that.xAxis.maximum);
            var gridColumnLength = that.source[0].length;
            var dateArray = [];
            var columnDateArray = [];

            for (var day = minDate, counter = 0; day <= maxDate; day.setDate(day.getDate() + 1), counter++) {
                if (counter < gridColumnLength) {
                    columnDateArray.push(day.toISOString());
                    continue;
                }

                dateArray.push(columnDateArray);
                that._addMonthLabel(columnDateArray);
                columnDateArray = [];
                counter = 0;
                columnDateArray.push(day.toISOString());
            }

            that._dateArray = dateArray;
        },

        _addMonthLabel: function (dateArray) {
            var that = this;
            var options = { month: that.xAxis.labelFormat };
            var intl = new Intl.DateTimeFormat('en-US', options);
            var date = new Date(dateArray[0]);
            var month = intl.format(date);

            that.xAxis.labels.push(month);
        },

        _renderGrid: function () {
            var that = this;
            var grid = document.createElement('div');
            var gridXAxisContainer = document.createElement('div');
            var gridYAxisContainer = document.createElement('div');
            var gridContainerWrapper = document.createElement('div');

            grid.className = 'jqx-heatmap-grid';
            gridXAxisContainer.className = 'jqx-x-axis-container';
            gridYAxisContainer.className = 'jqx-y-axis-container';
            gridContainerWrapper.className = 'jqx-heatmap-container-wrapper';

            that._setAxisOrigin();

            for (var column = 0, length = that.source.length; column < length; column++) {
                var columnBox = document.createElement('div');
                var currentColumn = that.source[column];

                that._setGridColumn(columnBox, column, length);

                for (var row = 0, _length = currentColumn.length; row < _length; row++) {
                    var cellBox = document.createElement('div');

                    that._setGridCell(cellBox, column, row, currentColumn);
                    columnBox.appendChild(cellBox);
                }

                grid.appendChild(columnBox);
            }

            gridContainerWrapper.style.width = that._toPx(that.width);

            gridXAxisContainer.appendChild(grid);
            gridYAxisContainer.appendChild(gridXAxisContainer);
            gridContainerWrapper.appendChild(gridYAxisContainer);

            that._gridXAxisContainer = gridXAxisContainer;
            that._gridYAxisContainer = gridYAxisContainer;
            that._gridContainerWrapper = gridContainerWrapper;
            that.element.appendChild(gridContainerWrapper);
        },

        _renderAxis: function (axis, array, axisClass, axisElementClass) {
            var that = this;
            var elementsContainer = document.createElement('div');
            var gridHeight = that.element.querySelector('.jqx-heatmap-grid').offsetHeight;

            for (var index = 0, length = array.length; index < length; index++) {
                var currentValue = array[index];
                var element = document.createElement('div');

                element.innerHTML = currentValue;
                element.className = axisElementClass;

                if (axis === 'x') {
                    element.style.width = 'calc(100% /' + length + ')';

                    if (that.xAxis.minimum && that.xAxis.maximum) {
                        var totalColumns = that.source.length;
                        var labelWidth = (currentValue.duplicates / totalColumns) * 100;

                        element.innerHTML = currentValue.name;
                        element.style.width = labelWidth + '%';
                        if (that.xAxis.isInversed) {
                            element.style.textAlign = 'right';
                        } else {
                            element.style.textAlign = 'left';
                        }
                    }
                } else {
                    elementsContainer.style.height = gridHeight + 'px';
                    element.style.height = 'calc(100% /' + length + ')';
                }

                elementsContainer.appendChild(element);
            }

            if (axis === 'x') {
                elementsContainer.className = axisClass;
                that._gridXAxisContainer.appendChild(elementsContainer);
            } else {
                elementsContainer.className = axisClass;
                that._gridYAxisContainer.appendChild(elementsContainer);
            }
        },

        _getLabels: function (monthsArray) {
            var that = this;
            var month = {};
            var labels = [];
            var count = 0;

            monthsArray.forEach(function (name, index) {
                if (name === monthsArray[index + 1]) {
                    count++;
                } else {
                    count++;
                    month = {
                        name: name,
                        duplicates: count
                    };
                    labels.push(month);
                    count = 0;
                }
            });

            that.xAxis.labels = labels;
        },

        _renderAllAxis: function () {
            var that = this;
            var xAxisLabels;
            var yAxisLabels;
            var yAxis;

            if (that.xAxis.minimum && that.xAxis.maximum) {
                that._getLabels(that.xAxis.labels);
            }

            that._renderAxis('x', that.xAxis.labels, 'jqx-x-axis', 'jqx-x-axis-label');
            that._renderAxis('y', that.yAxis.labels, 'jqx-y-axis', 'jqx-y-axis-label');

            yAxis = that.element.querySelector('.jqx-y-axis');
            xAxisLabels = that.element.querySelectorAll('.jqx-x-axis-label');
            yAxisLabels = that.element.querySelectorAll('.jqx-y-axis-label');

            if (that.xAxis.opposedPosition) {
                that._gridXAxisContainer.style.flexDirection = 'column-reverse';
                yAxis.classList.add('jqx-margin-top-auto');
                xAxisLabels.forEach(function (element) {
                    element.classList.add('jqx-top');
                });
            }

            if (that.yAxis.opposedPosition) {
                that._gridYAxisContainer.style.flexDirection = 'row';
                yAxisLabels.forEach(function (element) {
                    element.classList.add('jqx-right');
                });
            }
        },

        _setGridColumn: function (columnItem, columnNumber, totalColumns) {
            columnItem.id = 'jqx-column-' + columnNumber;
            columnItem.classList.add('jqx-column');
            columnItem.style.width = 'calc(100% / ' + totalColumns + ')';
        },

        _setGridCell: function (cellItem, columnNumber, rowNumber, columnData) {
            var that = this;
            var cellValue = columnData[rowNumber];

            cellItem.classList.add('jqx-heatmap-cell');
            cellItem.setAttribute('data-row', rowNumber);
            cellItem.setAttribute('data-column', columnNumber);
            cellItem.setAttribute('data-value', cellValue);

            if (that.xAxis.minimum && that.xAxis.maximum) {
                cellItem.setAttribute('data-date', that._dateArray[columnNumber][rowNumber]);
            }

            for (var index = 0, length = that.paletteSettings.palette.length; index < length; index++) {
                if (cellValue === null) {
                    continue;
                }

                var cellValueNumber = Number(cellValue);
                var currentElement = that.paletteSettings.palette[index].value;
                var currentElementNumber = Number(currentElement);
                var nextElement = null;
                var nextElementNumber = null;

                if (index + 1 < length) {
                    nextElement = that.paletteSettings.palette[index + 1].value;
                    nextElementNumber = Number(nextElement);
                }

                if (nextElementNumber && ((cellValueNumber >= currentElementNumber && cellValueNumber < nextElementNumber) || cellValueNumber < that._minValue)) {
                    cellItem.setAttribute('data-color-index', index);
                    break;
                } else if (!nextElementNumber && (cellValueNumber >= currentElementNumber)) {
                    cellItem.setAttribute('data-color-index', index);
                }
            }

            if (that.xAxis.minimum && that.xAxis.maximum) {
                if (cellValue === null) {
                    cellItem.classList.add('jqx-empty-cell');
                }

                return;
            }

            if (cellValue === null) {
                cellItem.innerHTML = '&nbsp';
                cellItem.classList.add('jqx-empty-cell');

                return;
            }

            cellItem.innerHTML = cellValue;
        },

        _getTicksMinMax: function () {
            var that = this;
            var paletteArray = JSON.parse(JSON.stringify(that.paletteSettings.palette));
            var minValue = paletteArray[0].value;
            var maxValue = paletteArray[0].value;
            var ticksArray = [];

            for (var index = 0, length = paletteArray.length; index < length; index++) {
                var currentElementValue = paletteArray[index].value;
                var currentElementLabel = paletteArray[index].label || '';

                ticksArray.push({ value: currentElementValue, label: currentElementLabel });

                if (currentElementValue < minValue) {
                    minValue = currentElementValue;
                    continue;
                }

                if (currentElementValue > maxValue) {
                    maxValue = currentElementValue;
                    continue;
                }
            }

            that._minValue = minValue;
            that._maxValue = maxValue;
            that._ticksArray = ticksArray;
        },

        _renderTitle: function () {
            var that = this;
            var titleBox = document.createElement('div');

            titleBox.className = 'jqx-heatmap-title';
            titleBox.innerHTML = that.title;
            titleBox.style.width = that._toPx(that.width);
            that._title = titleBox;
            that.element.appendChild(titleBox);
        },

        _renderLegend: function (position) {
            var that = this;
            var legend = document.createElement('div');
            var palette = that.paletteSettings.type === 'Gradient' ? document.createElement('canvas') : document.createElement('div');
            var arrow = document.createElement('div');

            legend.classList.add('jqx-legend-' + position);
            palette.classList.add('jqx-palette-' + position);
            arrow.classList.add('jqx-arrow-' + position);

            that._drawLegend(legend, palette, position);

            legend.appendChild(palette);
            legend.appendChild(arrow);
            that._gridContainerWrapper.appendChild(legend);

            that._arrow = arrow;
            that._palette = palette;
            that._legend = legend;

            that._adjustLegend(position);
        },

        _drawLegend: function (legendElement, paletteElement, position) {
            var that = this;

            if (position === 'vertical') {
                if (that.xAxis.opposedPosition === true) {
                    legendElement.classList.add('jqx-margin-top-auto');
                }

                if (that.paletteSettings.type === 'Gradient') {
                    that._drawGradientLegend(paletteElement, 'vertical');
                } else {
                    that._drawFixedLegend(paletteElement, 'vertical');
                }

                if (that.legendSettings.position === 'Left') {
                    legendElement.classList.add('jqx-left');
                }

                that._gridXAxisContainer.width = 'calc(100% - ' + legendElement.offsetWidth + 'px)';
            } else {
                if (that.paletteSettings.type === 'Gradient') {
                    that._drawGradientLegend(paletteElement, 'horizontal');
                } else {
                    that._drawFixedLegend(paletteElement, 'horizontal');
                }

                if (that.legendSettings.position === 'Bottom') {
                    legendElement.classList.add('jqx-bottom');
                }

                if (that.yAxis.opposedPosition === true) {
                    legendElement.classList.add('jqx-margin-right-auto');
                }
            }
        },

        _drawFixedLegend: function (element, position) {
            var that = this;
            var paletteArray = JSON.parse(JSON.stringify(that.paletteSettings.palette));
            var gridWidth = that.element.querySelector('.jqx-heatmap-grid').offsetWidth;
            var gridHeight = that.element.querySelector('.jqx-heatmap-grid').offsetHeight;

            if (position === 'horizontal') {
                element.classList.add('jqx-fixed');
            }

            for (var index = 0, length = paletteArray.length; index < length; index++) {
                var currentElement = paletteArray[index];
                var colorBox = document.createElement('div');
                var widthRatio = ((gridWidth / length) / gridWidth) * 100;

                colorBox.id = 'jqx-color-container-' + index;
                colorBox.className = 'jqx-color-container';
                colorBox.setAttribute('data-value', currentElement.value);
                colorBox.setAttribute('data-color-index', index);
                colorBox.setAttribute('data-color', currentElement.color);
                colorBox.style.background = currentElement.color;

                if (position === 'vertical') {
                    colorBox.style.width = '10px';
                    colorBox.style.height = 'calc(' + gridHeight + 'px / ' + length + ')';
                } else {
                    colorBox.style.width = widthRatio + '%';
                    colorBox.style.height = '10px';
                }

                element.appendChild(colorBox);
            }
        },

        _drawGradientLegend: function (element, position) {
            var that = this;
            var paletteArray = JSON.parse(JSON.stringify(that.paletteSettings.palette));
            var context = element.getContext('2d');

            if (position === 'vertical') {
                var gradient = context.createLinearGradient(0, 0, 0, 150);
            } else {
                var gradient = context.createLinearGradient(0, 0, 300, 0);
            }

            if (that.paletteSettings.type === 'Gradient') {
                that._checkNewMinMax();
            }

            for (var index = 0, length = paletteArray.length; index < length; index++) {
                var currentElement = paletteArray[index];
                var calculatedValue = ((currentElement.value - that._minValue) / (that._maxValue - that._minValue));

                gradient.addColorStop(calculatedValue, currentElement.color);
            }

            context.fillStyle = gradient;
            context.fillRect(0, 0, 300, 150);

            that._canvasContext = context;
        },

        _getSourceMinMax: function () {
            var that = this;
            var min = that.source[0][0];
            var max = that.source[0][0];

            for (var index = 0, length = that.source.length; index < length; index++) {
                for (var _index = 0, _length = that.source[index].length; _index < _length; _index++) {
                    var currentElement = that.source[index][_index];

                    if (currentElement === null) {
                        continue;
                    }

                    if (min > currentElement) {
                        min = currentElement;
                        continue;
                    }

                    if (max < currentElement) {
                        max = currentElement;
                        continue
                    }
                }
            }

            return {
                min: min,
                max: max
            }
        },

        _checkNewMinMax: function () {
            var that = this;
            var paletteArray = JSON.parse(JSON.stringify(that.paletteSettings.palette));
            var sourceMinMax = that._getSourceMinMax();

            if (that._minValue > sourceMinMax.min) {
                that._minValue = sourceMinMax.min;
                paletteArray.splice(0, 0, { value: sourceMinMax.min, color: paletteArray[0].color });
            }

            if (that._maxValue < sourceMinMax.max) {
                that._maxValue = sourceMinMax.max;
                paletteArray.push({ value: sourceMinMax.max, color: paletteArray[paletteArray.length - 1].color });
            }
        },

        _adjustLegend: function (position) {
            var that = this;

            if (position === 'vertical') {
                that._setVerticalLegendHeight();
                that._renderScale('vertical');
            } else {
                that._setHorizontalLegendWidth();
                that._renderScale('horizontal');
            }
        },

        _renderScale: function (position) {
            var that = this;

            for (var index = 0, length = that._ticksArray.length; index < length; index++) {
                var currentElementValue = that._ticksArray[index].value;
                var currentElementLabel = that._ticksArray[index].label;
                var tick = document.createElement('div');

                if (that.paletteSettings.type === 'Fixed') {
                    tick.classList.add('jqx-color-container-tick');
                }
                tick.classList.add('jqx-tick', 'jqx-tick-' + position);
                tick.setAttribute('data-value', currentElementValue);
                tick.setAttribute('data-color-index', index);
                tick.innerHTML = currentElementLabel || currentElementValue;

                that._setLegendTicksPosition(tick, currentElementValue, index, position);
                that._legend.appendChild(tick);
            }

            if (position === 'vertical') {
                that._tick = tick;
                that._setVerticalLegendWidth();
            }
        },

        _setLegendTicksPosition: function (tickElement, tickValue, tickIndex, position) {
            var that = this;
            var arrowBorder = '6px'

            if (that.paletteSettings.type === 'Gradient') {
                if (tickValue === that._minValue || tickValue === that._maxValue) {
                    if (tickValue === that._minValue) {
                        if (position === 'vertical') {
                            tickElement.style.top = '-0.5%';
                        } else {
                            tickElement.style.left = '-3px';
                        }
                    } else {
                        if (position === 'vertical') {
                            tickElement.style.top = '96%';
                        } else {
                            tickElement.style.right = '-11px';
                        }
                    }

                    tickElement.classList.add('jqx-no-line');
                } else {
                    var calculatedValue = ((tickValue - that._minValue) / (that._maxValue - that._minValue)) * 100;

                    if (position === 'vertical') {
                        tickElement.style.top = 'calc(' + calculatedValue + '% ' + '- ' + arrowBorder + ')';

                    } else {
                        tickElement.style.left = 'calc(' + calculatedValue + '% ' + '- ' + arrowBorder + ')';
                    }
                }
            } else {
                var colorContainerHeight = that.element.querySelector('.jqx-color-container').offsetHeight;
                var colorContainerWidth = that.element.querySelector('.jqx-color-container').offsetWidth;
                var alignCenter = 5;

                if (position === 'vertical') {
                    if (tickIndex === 0) {
                        tickElement.style.top = tickIndex * colorContainerHeight + 'px';
                    } else {
                        tickElement.style.top = tickIndex * colorContainerHeight - alignCenter + 'px';
                    }
                } else {
                    if (tickIndex === 0) {
                        tickElement.style.left = tickIndex * colorContainerWidth + 'px';
                    } else {
                        var ticksNumber = that.paletteSettings.palette.length;
                        var gridWidth = that.element.querySelector('.jqx-heatmap-grid').offsetWidth;
                        var widthRatio = ((gridWidth / ticksNumber) / gridWidth) * 100;

                        tickElement.style.left = 'calc(' + widthRatio * tickIndex + '% - ' + alignCenter + 'px)';
                    }
                }

                tickElement.classList.add('jqx-no-line');
            }
        },

        _showLegendArrow: function (position, value) {
            var that = this;
            var arrowBorder = '6px'
            var calculatedValue = ((value - that._minValue) / (that._maxValue - that._minValue)) * 100;

            if (that.paletteSettings.type === 'Gradient' && value <= that._maxValue) {
                that._arrow.style.display = 'block';

                if (position === 'vertical') {
                    that._arrow.style.top = 'calc(' + calculatedValue + '% ' + '- ' + arrowBorder + ')';
                } else {
                    that._arrow.style.left = 'calc(' + calculatedValue + '% ' + '- ' + arrowBorder + ')';
                }
            }
        },

        _setVerticalLegendWidth: function () {
            var that = this;
            var tickAbsolutePositionLeft = 15;
            var margin = 15;

            that._legend.style.width = tickAbsolutePositionLeft + that._tick.offsetWidth + 'px';
            that._gridXAxisContainer.style.width = 'calc(100% - ' + (that._legend.offsetWidth + margin) + 'px)';
        },

        _setVerticalLegendHeight: function () {
            var that = this;
            var gridHeight = that.element.querySelector('.jqx-heatmap-grid').offsetHeight;

            that._legend.style.height = gridHeight + 'px';
        },

        _setHorizontalLegendWidth: function () {
            var that = this;
            var yAxisWidth = that.element.querySelector('.jqx-y-axis').offsetWidth;

            that._legend.style.width = 'calc(100% - ' + yAxisWidth + 'px)';
        },

        _hideCells: function (colorIndex) {
            var that = this;
            var cells = that.element.querySelectorAll('.jqx-heatmap-cell[data-color-index="' + colorIndex + '"]');

            for (var index = 0, length = cells.length; index < length; index++) {
                var currentElement = cells[index];

                currentElement.classList.toggle('jqx-heatmap-hidden');
            }
        },

        _toggleClass: function (searchingClass, toggleClass, value) {
            var that = this;
            var array = that.element.querySelectorAll('.' + searchingClass);

            for (var index = 0, length = array.length; index < length; index++) {
                var currentElement = array[index];
                var currentElementValue = currentElement.getAttribute('data-value');

                if (value === currentElementValue) {
                    currentElement.classList.toggle(toggleClass);

                    return;
                }
            }
        },

        _toPx: function (value) {
            if (typeof value === 'number') {
                return value + 'px';
            } else {
                return value;
            }
        },

        _getLegendColor: function (value, position) {
            var that = this;
            var canvasHeight = 149;
            var canvasWidth = 299;
            var calculatedVerticalValue = ((value - that._minValue) / (that._maxValue - that._minValue)) * canvasHeight;
            var calculatedHorizontalValue = ((value - that._minValue) / (that._maxValue - that._minValue)) * canvasWidth;
            var pixelData = null;
            var rgba = null;

            if (value > that._maxValue || value < that._minValue) {
                return;
            }

            if (position === 'vertical') {
                pixelData = that._canvasContext.getImageData(0, calculatedVerticalValue, 1, 1).data;
            } else {
                pixelData = that._canvasContext.getImageData(calculatedHorizontalValue, 0, 1, 1).data;
            }

            rgba = 'rgba(' + pixelData[0] + ', ' + pixelData[1] + ', ' + pixelData[2] + ', ' + (pixelData[3] / 255) + ')';

            return rgba;
        },

        _getFixedLegendColor: function (value) {
            var that = this;
            var paletteArray = JSON.parse(JSON.stringify(that.paletteSettings.palette));

            for (var i = 0, length = paletteArray.length; i < length; i++) {
                var currentElement = paletteArray[i];
                var nextElement = paletteArray[i + 1];

                if (nextElement && ((value >= currentElement.value && value < nextElement.value) || value < that._minValue)) {
                    return currentElement.color;
                } else if (!nextElement && value >= currentElement.value) {
                    return currentElement.color;
                }
            }
        },

        _setCellsBackground: function (position) {
            var that = this;
            var cells = that.element.querySelectorAll('.jqx-heatmap-cell')

            for (var index = 0, length = cells.length; index < length; index++) {
                var cell = cells[index];
                var cellValue = cells[index].getAttribute('data-value');

                if (cellValue === 'null') {
                    cell.style.background = that.paletteSettings.emptyPointColor;
                } else {
                    if (that.paletteSettings.type === 'Gradient') {
                        var cellColor = that._getLegendColor(cellValue, position);
                    } else {
                        var cellColor = that._getFixedLegendColor(cellValue);
                    }

                    cell.style.background = cellColor;
                }
            }
        },

        _renderLegendPosition: function () {
            var that = this;

            switch (that.legendSettings.position) {
                default:
                case 'Right':
                    that._gridContainerWrapper.style.flexDirection = 'row';
                    that._renderLegend('vertical');
                    that._setCellsBackground('vertical');
                    break;
                case 'Left':
                    that._gridContainerWrapper.style.flexDirection = 'row-reverse';
                    that._renderLegend('vertical');
                    that._setCellsBackground('vertical');
                    break;
                case 'Top':
                    that._gridContainerWrapper.style.flexDirection = 'column-reverse';
                    that._renderLegend('horizontal');
                    that._setCellsBackground('horizontal');
                    break;
                case 'Bottom':
                    that._gridContainerWrapper.style.flexDirection = 'column';
                    that._renderLegend('horizontal');
                    that._setCellsBackground('horizontal');
                    break;
            }
        },

        _reverseXAxisOrigin: function () {
            var that = this;

            that.xAxis.labels.reverse();
            that.source.reverse();
            that._isXAxisInversed = !that._isXAxisInversed;
            if (that.xAxis.minimum && that.xAxis.maximum) {
                that._dateArray.reverse();
            }
        },

        _reverseYAxisOrigin: function () {
            var that = this;

            that.yAxis.labels.reverse();
            that._isYAxisInversed = !that._isYAxisInversed;
            that._reverseNestedArray(that.source);
            if (that.xAxis.minimum && that.xAxis.maximum) {
                that._reverseNestedArray(that._dateArray);
            }
        },

        _reverseNestedArray: function (array) {
            for (var index = 0, length = array.length; index < length; index++) {
                var element = array[index];

                element.reverse()
            }
        },

        _setAxisOrigin: function () {
            var that = this;

            if (that.xAxis.isInversed !== that._isXAxisInversed) {
                that._reverseXAxisOrigin();
            }

            if (that.yAxis.isInversed !== that._isYAxisInversed) {
                that._reverseYAxisOrigin();
            }
        },

        _destroyGrid: function () {
            var that = this;

            $(that._gridContainerWrapper).remove();
            if (that.xAxis.minimum && that.xAxis.maximum) {
                that.xAxis.labels = [];
            }
        },

        _destroyTitle: function () {
            var that = this;

            $(that._title).remove();
        },

        _renderAllElements: function () {
            var that = this;

            that._renderTitle();
            that._getTicksMinMax();
            if (that.xAxis.minimum && that.xAxis.maximum) {
                that._getAllDates();
            }
            that._renderGrid();
            that._renderAllAxis();
            that._renderLegendPosition();
        },

        _rerender: function () {
            var that = this;

            that._destroyTitle();
            that._destroyGrid();
            that._renderAllElements();
            that._addHandlers();
        },

        setLegendPosition: function (position) {
            var that = this;

            if (that.legendSettings.position !== position) {
                that.legendSettings.position = position;
                that._rerender();
            }
        },

        setOpposedXAxisPosition: function (value) {
            var that = this;

            if (that.xAxis.opposedPosition !== value) {
                that.xAxis.opposedPosition = value;
                that._rerender();
            }
        },

        setOpposedYAxisPosition: function (value) {
            var that = this;

            if (that.yAxis.opposedPosition !== value) {
                that.yAxis.opposedPosition = value;
                that._rerender();
            }
        },

        reverseXAxisPosition: function (value) {
            var that = this;

            that.xAxis.isInversed = value;
            that._rerender();
        },

        reverseYAxisPosition: function (value) {
            var that = this;

            that.yAxis.isInversed = value;
            that._rerender();
        },

        setPaletteType: function (type) {
            var that = this;

            that.paletteSettings.type = type;
            that._rerender();
        },

        destroy: function () {
            var that = this;

            that.host.remove();
        }
    });

})(jqxBaseFramework);


