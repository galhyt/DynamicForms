// !!!! The function declared in MicrosoftAjax.js for which some pages do link
// but the function not declared properly so must be overriden
//if (typeof (String.format) === "undefined") {
String.format = function (s, args) {
    $(args).each(function (i, val) {
        var regNoChar = new RegExp("\\{" + i + "\\,(\\d+)\\}", "gm");
        while (regNoChar.test(s)) {
            var match = s.match(regNoChar)[0];
            var no = Number(match.replace(regNoChar, "$1"));
            var val1 = val;
            if (isNaN(val)) {
                for (var j = val1.length ; j < no ; j++)
                    val1 += ' ';
            }
            else {
                for (var j = val1.toString().length ; j < no ; j++)
                    val1 = '0' + val1;
            }
            s = s.replace(match, val1);
        }

        var reg = new RegExp("\\{" + i + "\\}", "gm");
        s = s.replace(reg, val);
    })
    return s;
}

$(function () {
    if (!$.fn.live) {
        $.fn.live = function (eventType, callbachFunc) {
            var selector = this.selector;
            var _eventType = (eventType == 'hover' ? 'mouseover' : eventType);
            $(document).on(_eventType, function (event) {
                var $el = $(event.target);
                if ($el.is(selector)) {
                    callbachFunc.call(event.target, event);
                    return false;
                }

                if ($el.closest(selector).length > 0) {
                    callbachFunc.call($el.closest(selector)[0], event);
                }
            });
        }
    }

    $.fn.rightClick = function (method) {
        $(this).bind('contextmenu rightclick', function (e) {
            e.preventDefault();
            method();
            return false;
        });
    }

    if (Array.prototype.union == null) {
        Array.prototype.union = function (a) {
            var r = this.slice(0);
            a.forEach(function (i) { if (r.indexOf(i) < 0) r.push(i); });
            return r;
        };
    }

    if (Array.prototype.sum == null) {
        Array.prototype.sum = function () {
            var a = this;
            var sum = 0;
            for (var i = 0 ; i < a.length ; i++) {
                sum += a[i];
            }
            return sum;
        };
    }

    if (Array.prototype.avg == null) {
        Array.prototype.avg = function () {
            var a = this;
            if (a.length == 0) return null;
            return a.sum() / a.length;
        }
    }

    if (Array.prototype.max == null) {
        Array.prototype.max = function () {
            var a = this;
            if (a.length == 0) return null;

            var max = a[0];
            for (var i = 1 ; i < a.length ; i++) {
                if (a[i] > max) max = a[i];
            }
            return max;
        };
    }

    if (Array.prototype.stdev == null) {
        Array.prototype.stdev = function () {
            var a = this;
            if (a == null || a.length <= 1) return null;
            var avg = a.avg();
            return Math.sqrt($.makeArray($.map(a, function (x, i) {
                return Math.pow(x - avg, 2);
            })).sum() / (a.length - 1));
        }
    }

    if (Math.slope == null) {
        Math.slope = function (aX, aY) {
            avgX = aX.avg();
            avgY = aY.avg();

            var aArr = $.makeArray($.map(aX, function (x, i) {
                return (x - avgX) * (aY[i] - avgY);
            }));
            var a = aArr.sum();

            var b = $.makeArray($.map(aX, function (x, i) {
                return Math.pow(x - avgX, 2);
            })).sum();

            if (b == 0) return null;
            return a / b;
        }
    }

    if (Math.intercept == null) {
        Math.intercept = function (aX, aY) {
            return aY.avg() - Math.slope(aX, aY) * aX.avg();
        }
    }

    // Handlebars
    Handlebars.registerHelper("mv_fext", function (str) {
        var reg = new RegExp("[^\.]+$");
        var arr = reg.exec(str);
        if (arr == null || arr.length == 0) return '';
        return arr[0];
    });
    Handlebars.registerHelper('for', function (from, to, incr, block) {
        var accum = '';
        for (var i = from; i < to; i += incr)
            accum += block.fn(i);
        return accum;
    });
    Handlebars.registerHelper('ifCond', function (v1, operator, v2, options) {
        switch (operator) {
            case '==':
                return (v1 == v2) ? options.fn(this) : options.inverse(this);
            case '!=':
                return (v1 != v2) ? options.fn(this) : options.inverse(this);
            case '===':
                return (v1 === v2) ? options.fn(this) : options.inverse(this);
            case '<':
                return (v1 < v2) ? options.fn(this) : options.inverse(this);
            case '<=':
                return (v1 <= v2) ? options.fn(this) : options.inverse(this);
            case '>':
                return (v1 > v2) ? options.fn(this) : options.inverse(this);
            case '>=':
                return (v1 >= v2) ? options.fn(this) : options.inverse(this);
            case '&&':
                return (v1 && v2) ? options.fn(this) : options.inverse(this);
            case '||':
                return (v1 || v2) ? options.fn(this) : options.inverse(this);
            default:
                return options.inverse(this);
        }
    });
    Handlebars.registerHelper('arithmetics', function (v1, op1, v2, op2, v3, op3, v4, op4, v5, op5, v6) {
        var res = v1;
        var ops = [op1, op2, op3, op4, op5];
        var vals = [v2, v3, v4, v5, v6];
        $(ops).each(function (i, op) {
            if (vals[i] == null) return false;
            switch (op) {
                case '+':
                    res += vals[i];
                    break;
                case '-':
                    res -= vals[i];
                    break;
                case '*':
                    res *= vals[i];
                    break;
                default:
                    return false;
                    break;
            }
        });

        return res;
    });
});

// DatePicker
$(function () {
    if ($.datepicker != undefined) {
        $.datepicker._defaults.yearRange = 'c-20:c+10';
        $.datepicker._defaults.dateFormat = (config_values != null ? config_values.DateFormat.toLowerCase().replace("yyyy", "yy") : "dd/mm/yy");

        setTimeout(function () {
            $('img.ui-datepicker-trigger').each(function () {
                var src = $(this).attr('src');
                $(this).attr('src', src)
            });
        }, 0);
    }
});

// handlebars
var handlebarsCompile = function ($mv_chat_template, context) {
    var source = $mv_chat_template.html();
    var template = Handlebars.compile(source);
    if ($.parseHTML)
        return $.parseHTML(template(context));
    else
        return template(context);
};

var TemplateDynamicFormConfiguration = (function () {
    var getDragableClassName = function (src) {
        var className = null;
        $(['DynamicFormRow', 'DynamicFormColumn', 'DynamicFormField']).each(function (indx, clsName) {
            if ($(src).hasClass(clsName)) {
                className = clsName;
                return false;
            }
        });
        return className;
    };
    var allowDrop = function (ev) {
        ev.preventDefault();
    };
    var drag = function (ev) {
        ev.originalEvent.dataTransfer.setData("text", ev.target.id);
    };
    var drop = function (ev) {
        ev.preventDefault();
        var srcId = ev.originalEvent.dataTransfer.getData("text");
        var src = document.getElementById(srcId);
        var $srcCol = $(src).closest('DynamicFormColumn');

        var $trg = null;
        var clsName = getDragableClassName(src);
        if (clsName == null) return;
        $trg = $(ev.target).closest('.' + clsName);

        if ($trg.prev().length > 0 && $trg.prev().attr('id') == src.id)
            $trg.after($(src));
        else
            $trg.before($(src));

        ev.stopPropagation();
        rearangeTable();
    };
    var rearangeTable = function () {
        $('.DynamicFormRow').each(function (indx, row) {
            var $cells = $(row).find('.DynamicFormColumn');
            var cellsNo = $cells.length;
            if (cellsNo == 0) return true;

            var cellWidth = Math.floor(90 / cellsNo);
            $cells.each(function (indx, cell) {
                $(cell).css('width', cellWidth + '%');
            });

            row.id = row.id.replace(/\[\d+\]/i, "[" + indx + "]");
            rearangeColumnIndexes(row);
        });
    };
    var rearangeItemIndexes = function (item) {
        var clsName = getDragableClassName(item);
        var fn = { 'DynamicFormColumn': rearangeColumnIndexes, 'DynamicFormField': rearangeFieldIndexes }[clsName];
        if (fn == null) return;
        fn(item);
    };
    var rearangeColumnIndexes = function (row) {
        var rowIndx = row.id.match(/\[\d+(?=\]$)/)[0].replace('[', '');
        $(row).find('.DynamicFormColumn').each(function (colIndx, col) {
            col.id = String.format('DynamicFormColumn[{0}][{1}]', [rowIndx, colIndx]);
            rearangeFieldIndexes(col);
        });
    };
    var rearangeFieldIndexes = function (col) {
        // check if same column and row, if so return
        var matches = $(col).attr('id').match(/\[\d+\]/g);
        var rowIndx = matches[0].replace(/[\[\]]/g, '');
        var colIndx = matches[1].replace(/[\[\]]/g, '');

        $(col).find('.DynamicFormField').each(function (fIndx, field) {
            field.id = String.format('DynamicFormField[{0}][{1}][{2}]', [rowIndx, colIndx, fIndx]);
            ["HtmlType", "FieldName"].forEach(function (propertyName) {
                var el = $(field).find('[id$="' + propertyName + '"]')[0];
                el.id = el.id.replace(/\_\d+\_\_\d+\_\_\d+\_\_/, String.format('_{0}__{1}__{2}__', [rowIndx, colIndx, fIndx]));
                el.name = el.name.replace(/\[\d+\]\[\d+\]\[\d+\]/, String.format('[{0}][{1}][{2}]', [rowIndx, colIndx, fIndx]));
            })
        });
    };
    var document_click = function () {
        if (!$(event.target).hasClass('template-actions-btn') && !$(event.target).hasClass('template-html-ctrl'))
            $('.template-actions-menu').hide();
    };
    var menuDic = {
        'DynamicFormRow': {'item': 'Row', 'child': 'Cell'},
        'DynamicFormColumn': {'item': 'Cell', 'child': 'Field'}
    };
    var block_actions_click = function () {
        var blockClsName = getDragableClassName($(this).closest('.ItemBlock')[0]);
        var handlebarsContext = {'blockId': $(this).closest('.ItemBlock')[0].id};
        $.extend(handlebarsContext, menuDic[blockClsName])
        $('.template-actions-menu').empty().append($(handlebarsCompile($('#template-actions-menu'), handlebarsContext)));

        $('.template-actions-menu').css({ top: $(this).offset().top + 20, left: $(this).offset().left });
        $('.template-actions-menu').toggle();
    };
    var action_btn_hover = function () {
        $(this).closest('.ItemBlock').css('opacity', '0.5');
    };
    var action_btn_out = function () {
        $(this).closest('.ItemBlock').css('opacity', '1');
    };
    var registerDragDropHandlres = function () {
        $('.draggable').each(function (indx, draggable) {
            $(draggable).off('dragstart').on('dragstart', drag);
            $(draggable).off('drop').on('drop', drop);
            $(draggable).off('dragover').on('dragover', allowDrop);
        });
    };
    var registerActionBtnHandlers = function () {
        $('.template-actions-btn').unbind('mouseenter mouseleave').hover(action_btn_hover, action_btn_out);
    };
    var html_type_ctrl_click = function () {
        var handlebarsContext = { 'blockId': $(this).closest('.ItemBlock')[0].id, 'mIndx': $(this).closest('.ItemBlock').attr('mIndx') };
        $('.template-actions-menu').empty().append($(handlebarsCompile($('#template-field-type-menu'), handlebarsContext)));

        $('.template-actions-menu').css({ top: $(this).offset().top + 20, left: $(this).offset().left });
        $('.template-actions-menu').toggle();
    };
    var template_field_type_item_click = function () {
        var itemType = $(this).attr('item-type');
        if (itemType == 'delete') {
            $block(this).remove();
            return;
        }
        var blockId = $block(this).attr('id');
        var handlebarsContext = {
            'itemType': itemType,
            'fieldType': htmlTypeToFieldType[itemType],
            'FieldName': $block(this).find('[id*="UITable"][id$="FieldName"]').val(),
            'mIndx': $block(this).attr('mIndx'),
            'rIndx': blockId.match(/\[\d+\]/g)[0].replace(/[\[\]]/g, ''),
            'cIndx': blockId.match(/\[\d+\]/g)[1].replace(/[\[\]]/g, ''),
            'fIndx': blockId.match(/\[\d+\]/g)[2].replace(/[\[\]]/g, '')
        };
        $block(this).find('.FormFieldConfigContent').empty().append($(handlebarsCompile($('#template-field-type-item'), handlebarsContext)));
        $('.template-actions-menu').toggle();
    };
    var htmlTypeToFieldType = {
        'checkbox': 'System.Boolean',
        'text': 'System.String',
        'textarea': 'System.String',
        'number': 'System.Double'
    };
    var $block = function (actionItem) {
        return $(String.format('[id="{0}"]', [$(actionItem).attr('block-id')]));
    };
    var _configurationToJson = function () {
        // FieldsMetaData
        var FieldsMetaData = $.makeArray($.map($('.DynamicFormField'), function (field, indx) {
            return {
                'FieldName': $(field).find('[id*="FieldsMetaData"][id$="FieldName"]').val(),
                'FieldTitle': $(field).find('[id*="FieldsMetaData"][id$="FieldTitle"]').val(),
                'FieldType': $(field).find('[id*="FieldsMetaData"][id$="FieldType"]').val()
            }
        }));

        // UITable
        var UITable = $.makeArray($.map($('.DynamicFormRow'), function (row, indx) {
            return [$.makeArray($.map($(row).find('.DynamicFormColumn'), function (col, indx1) {
                return [$.makeArray($.map($(col).find('.DynamicFormField'), function (field, indx2) {
                    return {
                        'FieldName': $(field).find('[id*="FieldsMetaData"][id$="FieldName"]').val(),
                        'HtmlType': $(field).find('[id*="UITable"][id$="HtmlType"]').val()
                    }
                }))];
            }))];
        }));

        return {
            'FormTitle': $('[id*="__FormTitle"]').val(),
            'FieldsMetaData': FieldsMetaData,
            'UITable': UITable
        };
    };
    var ActionsMenu = (function () {
        Handlebars.registerHelper('renderNewField', function () {
            var handlebarContext = GetNewFieldIndexes(this.id);
            return Handlebars.compile($('#template-dynamic-form-item').html())(handlebarContext);
        });
        Handlebars.registerHelper('renderNewFieldContent', function () {
            return Handlebars.compile($('#template-dynamic-form-field').html())(this);
        });
        var GetNewFieldIndexes = function (parentColId) {
            var indxes = $.makeArray($.map(parentColId.match(/(\[\d+\])/g), function (res, indx) {
                return Number(res.replace(/[\[\]]/g, ''));
            }));
            var $colFields = $(String.format('[id="{0}"]', [parentColId])).find('.DynamicFormField');
            var fIndx = ($colFields.length > 0 ? getNextIndx('DynamicFormField', $colFields.last()[0]) : 0);
            return {
                id: String.format("DynamicFormField[{0}][{1}][{2}]", [indxes[0], indxes[1], fIndx]),
                className: 'DynamicFormField',
                mIndx: getNextMIndx(),
                rIndx: indxes[0],
                cIndx: indxes[1],
                fIndx: fIndx,
                fieldName: getNextFieldName()
            };
        };
        var blockDic = null;
        var getNextMIndx = function () {
            return $.makeArray($.map($('.DynamicFormField [name*="FieldsMetaData["][name$=".FieldName"]'), function (el, indx) {
                return Number($(el).attr('name').match(/FieldsMetaData\[\d+(?=\])/)[0].replace('FieldsMetaData[', ''));
            })).max() + 1;
        };
        var getNextFieldName = function () {
            var newIndx = $.makeArray($.map($('.DynamicFormField [name*="FieldsMetaData["][name$=".FieldName"]'), function (el, indx) {
                return Number($(el).val().match(/\d+$/)[0]);
            })).max();
            if (newIndx == null) return "Field1";

            return String.format('Field{0}', newIndx + 1);
        };
        var invokePostAction = function (clsName, actionName) {
            if (blockDic[clsName] != null && blockDic[clsName].postAction[actionName] != null) {
                var fPostActions = blockDic[clsName].postAction[actionName];
                for (var i = 0 ; i < fPostActions.length ; i++) {
                    fPostActions[i](this);
                }
            }
            $('.template-actions-menu').hide();
        };
        var getNextItemId = function (curItem) {
            var clsName = getDragableClassName(curItem);
            var iIndx = getNextIndx(clsName, curItem);
            var reg = new RegExp(String.format('^{0}(\\[\\d+\\])+(?=\\[\\d+\\]$)', [clsName]));
            var prefix = clsName;
            var matches = reg.exec(curItem.id);
            if (matches != null) prefix = matches[0];

            return prefix + '[' + iIndx + ']';
        };
        var getNextIndx = function (blockClsName, itemBefore) {
            var siblings = $.makeArray($(itemBefore).siblings('.' + blockClsName));
            siblings.push(itemBefore);
            return $.makeArray($.map(siblings, function (block, indx) {
                var matches = /\[\d+(?=\]$)/.exec(block.id);
                if (matches != null) return Number(matches[0].replace('[', ''));
            })).max()+1;
        };
        var action_delete = function () {
            var $item = $block(this);
            var clsName = getDragableClassName($item[0]);
            $item.remove();
            invokePostAction.call(null, clsName, 'action_delete');
        };
        var action_add_item = function () {
            var $item = $block(this);
            add_item($item[0]);
        };
        var action_add_child = function () {
            var $item = $block(this);
            add_child($item[0]);
        };
        var add_item = function (itemBefore) {
            var clsName = getDragableClassName(itemBefore);
            handlebarContext = {
                'className': clsName,
                'id': getNextItemId(itemBefore),
                'childItem': blockDic[clsName].childItem
            };
            if (clsName == 'DynamicFormField') {
                $.extend(handlebarContext, GetNewFieldIndexes($(itemBefore).closest('.DynamicFormColumn').attr('id')));
            }

            $(itemBefore).after($(handlebarsCompile($('#template-dynamic-form-item'), handlebarContext)));
            invokePostAction.call($(itemBefore).next()[0], clsName, 'action_add_item');
            registerDragDropHandlres();
            registerActionBtnHandlers();
        };
        var add_child = function (parent) {
            var childCls = $(parent).attr('child-item');
            if (childCls == null) return;

            var $children = $(parent).find('.' + childCls);
            if ($children.length > 0)
                add_item($children[$children.length - 1]);
            else {
                var clsName = getDragableClassName(parent);
                var parentSufix = parent.id.replace(clsName, '');
                handlebarContext = {
                    'className': childCls,
                    'id': String.format('{0}{1}[0]',[childCls, parentSufix]),
                    'childItem': blockDic[childCls].childItem
                };
                if (clsName == 'DynamicFormField')
                    handlebarContext['mIndx'] = getNextMIndx();

                $(parent).append($(handlebarsCompile($('#template-dynamic-form-item'), handlebarContext)));
            }

            invokePostAction.call($(parent).find('.' + childCls).last()[0], childCls, 'action_add_child');
        };
        var add_field = function () {
        };
        return {
            init: function (deleteSel, addItemSel, addChildSel) {
                $(deleteSel).live('click', action_delete);
                $(addItemSel).live('click', action_add_item);
                $(addChildSel).live('click', action_add_child);
                blockDic = {
                    'DynamicFormRow': {
                        'postAction': { 'action_delete': null,
                            'action_add_item': [add_child],
                            'action_add_child': null
                        },
                        'childItem': 'DynamicFormColumn'
                    },
                    'DynamicFormColumn': {
                        'postAction': {
                            'action_delete': [rearangeTable],
                            'action_add_item': [rearangeTable],
                            'action_add_child': [rearangeTable]
                        },
                        'childItem': 'DynamicFormField'
                    },
                    'DynamicFormField': {
                        'postAction': {
                            'action_delete': null,
                            'action_add_item': null,
                            'action_add_child': null
                        },
                        'childItem': null
                    }
                };
            }
        }
    })();
    return {
        init: function () {
            registerDragDropHandlres();
            registerActionBtnHandlers();
            $('.template-actions-btn').live('click', block_actions_click);
            ActionsMenu.init('.template_action_delete', '.template_action_add_item', '.template_action_add_child');
            $('.template-html-ctrl').live('click', html_type_ctrl_click);
            $('.template_field_type_item').live('click', template_field_type_item_click);
            $(document).click(document_click);
        },
        configurationToJson: function () {
            return _configurationToJson();
        }
    }
})();
//*****
