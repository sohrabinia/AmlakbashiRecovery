//settings: color, buttons, onClose, contentUrl, onContentReady, onOpen, disableKeyEvents, fullScreen, yesText, noText, autoClose

var isLoadingContent = false;
var msgPopupKeyPressEnabled = true;

function showInfoMessage(title, content, setting) {
    if (setting == undefined) {
        setting = {};
    }
    setting.color = '#4485F2';
    return showMessagePopup(title, content, setting);
}

function showErrorMessage(title, content, setting) {
    if (setting == undefined) {
        setting = {};
    }
    setting.color = '#EA4335';
    return showMessagePopup(title, content, setting);
}

function showSuccessMessage(title, content, setting) {
    if (setting == undefined) {
        setting = {};
    }
    setting.color = '#34A853';
    return showMessagePopup(title, content, setting);
}

function showNoYesMessage(title, content, onYes, onNo, setting) {
    if (setting == undefined) {
        setting = {};
    }
    var buttons = [{
        title: setting.noText != undefined ? setting.noText : 'خیر',
        color: 'white',
        bgColor: '#EA4335',
        onclick: onNo
    },
    {
        title: setting.yesText != undefined ? setting.yesText : 'بله',
        color: 'white',
        bgColor: '#34A853',
        onclick: onYes
    }];
    setting.buttons = buttons;
    setting.color = '#4485F2';
    return showMessagePopup(title, content, setting);
}

function showMessagePopup(title, content, setting) {
    if (setting == undefined) {
        setting = {};
    }
    if (setting.autoClose == undefined) {
        setting.autoClose = true;
    }
    if (setting.color == undefined) {
        setting.color = '#fdd835';
    }
    if (setting.buttons == undefined) {
        setting.buttons = [{
            title: 'بستن',
            color: '#242424',
            bgColor: '#ededed',
        }];
    }
    if (popupMsgList == undefined) {
        popupMsgList = [];
    }
    var msg = {
        buttons: setting.buttons,
        autoClose: setting.autoClose,
        onClose: setting.onClose,
        enterKeyDisabled: setting.enterKeyDisabled,
        elementId: 'js-msg-popup-' + (popupMsgList.length + 1),
        closePopup: function () {
            if (popupMsgList.length > 1) {
                $('#' + popupMsgList[popupMsgList.length - 2].elementId).css(
                    'z-index', 10000000000 + popupMsgList.length - 1);
            }
            $('#' + this.elementId).hide(100,
                function () {
                    $(this).remove();
                });
            if (this.onClose != undefined) {
                setTimeout(this.onClose, 200);
            }
            for (var i = 0; i < popupMsgList.length; i++) {
                if (popupMsgList[i] === this) {
                    popupMsgList.splice(i, 1);
                    i--;
                }
            }
            updateBlurBg();
        },
        btnClick: function (index, forceClose) {
            if ((forceClose || this.autoClose) && !isLoadingContent) {
                this.closePopup();
            }
            if (this.buttons[index].onclick != undefined) {
                setTimeout(this.buttons[index].onclick, 200);
            }
        },
        close: function () {
            this.btnClick(0, true);
        },
        disableKeyEvents: setting.disableKeyEvents
    };
    if (popupMsgList.length > 0) {
        $('#' + popupMsgList[popupMsgList.length - 1].elementId).css(
            'z-index', 2147483640);
    }
    popupMsgList.push(msg);
    var $blurBg = getBlurBg();
    var $parent = getMessageParent();
    $parent.append('<div style="margin-top:' + (30 * (popupMsgList.length - 1)) + 'px;" id="' + msg.elementId + '" class="message-popup__message-container"></div>')
    var $elem = $('#' + msg.elementId);
    $elem.css('display', 'none');
    $elem.css('z-index', 10000000000 + popupMsgList.length);
    var fullScreen = setting.fullScreen = undefined ? false : setting.fullScreen;
    if (fullScreen) {
        $elem.css('width', '100%');
        $elem.css('top', 0);
        $elem.css('bottom', 0);
        $elem.css('display', 'flex');
        $elem.css('flex-flow', 'column');
        $elem.css('justify-content', 'space-between');
        $elem.css('background-color', 'white');
    }
    $elem.load('/post/getmessagepopup?fullScreen=' + fullScreen, function () {
        var $titleElem = $elem.find('.message-popup__message-title');
        var $contentElem = $elem.find('.message-popup__message-content');
        var $buttonContainer = $elem.find('.message-popup__message-button-container');
        $titleElem.css('background-color', setting.color);
        $titleElem.css('color', 'white');
        setting.buttons.forEach(function (btn, index) {
            $buttonContainer.append('<div ' +
                'style="color:' + btn.color + ';background-color:' + btn.bgColor + '" ' +
                'onclick="popupMsgList[' + (popupMsgList.length - 1) + '].btnClick(' + index + ', ' + (index == 0 ? "true" : "false") + ')"' +
                'class="message-popup__message-button">' +
                btn.title + '</div>');
        });
        $titleElem.html(title);
        if (setting.contentUrl != undefined) {
            msgPopupShowLoading();
            isLoadingContent = true;
            $contentElem.load(setting.contentUrl, function () {
                $blurBg.css('z-index', 10000000000 + popupMsgList.length);
                $blurBg.show(100, function () {
                    isLoadingContent = false;
                });
                msgPopupHideLoading();
                if (setting.onContentReady != undefined) {
                    setting.onContentReady();
                }
                if (setting.onOpen != undefined) {
                    setting.onOpen();
                }
            })
        }
        else {
            $contentElem.html(content);
            isLoadingContent = true;
            $blurBg.show(100, function () {
                isLoadingContent = false;
            });
            if (setting.onOpen != undefined) {
                setting.onOpen();
            }
        }
        $elem.show(100);
    });
    return msg;
}

function updateBlurBg() {
    if (popupMsgList.length < 1) {
        $('.message-popup__blur-bg').hide(100);
    }
}

function getBlurBg() {
    var $elem = $('.message-popup__blur-bg');
    if ($elem.length == 0) {
        $('body').prepend('<div class="message-popup__blur-bg" style="z-index:9999999999"></div>');
        $elem = $('.message-popup__blur-bg');
    }
    return $elem;
}
function getMessageParent() {
    var $elem = $('.message-popup__parent');
    if ($elem.length == 0) {
        $('body').append('<div class="message-popup__parent"></div>');
        $elem = $('.message-popup__parent');
    }
    return $elem;
}

function ButtonClickLastMessage(btnIndex, forceClose) {
    var msg = popupMsgList[popupMsgList.length - 1];
    if (msg.buttons != undefined && msg.buttons.length > btnIndex) {
        msg.btnClick(btnIndex, forceClose);
    }
}
document.addEventListener("keydown", msgPopupKeyDown, false);

function msgPopupKeyDown(e) {
    if (msgPopupKeyPressEnabled == false)
        return;
    if (popupMsgList == undefined || popupMsgList.length == 0) {
        return;
    }
    var msg = popupMsgList[popupMsgList.length - 1];
    if (msg.disableKeyEvents) {
        return;
    }
    var key = e.which;
    if (key == 13) {
        if (!msg.enterKeyDisabled) {
            if (msg.buttons.length == 1) {
                ButtonClickLastMessage(0);
            }
            else {
                ButtonClickLastMessage(1);
            }
        }
    }
    else if (key == 27) {
        ButtonClickLastMessage(0, true);
    }
}

function msgPopupShowLoading() {
    $('.message-popup__loading').show();
}

function msgPopupHideLoading() {
    $('.message-popup__loading').hide();
}

var popupMsgList;
/*global define*/
(function (global, undefined) {
	"use strict";

	var document = global.document,
	    Alertify;

	Alertify = function () {

		var _alertify = {},
		    dialogs   = {},
		    isopen    = false,
		    keys      = { ENTER: 13, ESC: 27, SPACE: 32 },
		    queue     = [],
		    $, btnCancel, btnOK, btnReset, btnResetBack, btnFocus, elCallee, elCover, elDialog, elLog, form, input, getTransitionEvent;

		/**
		 * Markup pieces
		 * @type {Object}
		 */
		dialogs = {
			buttons : {
				holder : "<nav class=\"alertify-buttons\">{{buttons}}</nav>",
				submit : "<button type=\"submit\" class=\"alertify-button alertify-button-ok\" id=\"alertify-ok\">{{ok}}</button>",
				ok     : "<button class=\"alertify-button alertify-button-ok\" id=\"alertify-ok\">{{ok}}</button>",
				cancel : "<button class=\"alertify-button alertify-button-cancel\" id=\"alertify-cancel\">{{cancel}}</button>"
			},
			input   : "<div class=\"alertify-text-wrapper\"><input type=\"text\" class=\"alertify-text\" id=\"alertify-text\"></div>",
			message : "<p class=\"alertify-message\">{{message}}</p>",
			log     : "<article class=\"alertify-log{{class}}\">{{message}}</article>"
		};

		/**
		 * Return the proper transitionend event
		 * @return {String}    Transition type string
		 */
		getTransitionEvent = function () {
			var t,
			    type,
			    supported   = false,
			    el          = document.createElement("fakeelement"),
			    transitions = {
				    "WebkitTransition" : "webkitTransitionEnd",
				    "MozTransition"    : "transitionend",
				    "OTransition"      : "otransitionend",
				    "transition"       : "transitionend"
			    };

			for (t in transitions) {
				if (el.style[t] !== undefined) {
					type      = transitions[t];
					supported = true;
					break;
				}
			}

			return {
				type      : type,
				supported : supported
			};
		};

		/**
		 * Shorthand for document.getElementById()
		 *
		 * @param  {String} id    A specific element ID
		 * @return {Object}       HTML element
		 */
		$ = function (id) {
			return document.getElementById(id);
		};

		/**
		 * Alertify private object
		 * @type {Object}
		 */
		_alertify = {

			/**
			 * Labels object
			 * @type {Object}
			 */
			labels : {
				ok     : "OK",
				cancel : "Cancel"
			},

			/**
			 * Delay number
			 * @type {Number}
			 */
			delay : 5000,

			/**
			 * Whether buttons are reversed (default is secondary/primary)
			 * @type {Boolean}
			 */
			buttonReverse : false,

			/**
			 * Which button should be focused by default
			 * @type {String}	"ok" (default), "cancel", or "none"
			 */
			buttonFocus : "ok",

			/**
			 * Set the transition event on load
			 * @type {[type]}
			 */
			transition : undefined,

			/**
			 * Set the proper button click events
			 *
			 * @param {Function} fn    [Optional] Callback function
			 *
			 * @return {undefined}
			 */
			addListeners : function (fn) {
				var hasOK     = (typeof btnOK !== "undefined"),
				    hasCancel = (typeof btnCancel !== "undefined"),
				    hasInput  = (typeof input !== "undefined"),
				    val       = "",
				    self      = this,
				    ok, cancel, common, key, reset;

				// ok event handler
				ok = function (event) {
					if (typeof event.preventDefault !== "undefined") event.preventDefault();
					common(event);
					if (typeof input !== "undefined") val = input.value;
					if (typeof fn === "function") {
						if (typeof input !== "undefined") {
							fn(true, val);
						}
						else fn(true);
					}
					return false;
				};

				// cancel event handler
				cancel = function (event) {
					if (typeof event.preventDefault !== "undefined") event.preventDefault();
					common(event);
					if (typeof fn === "function") fn(false);
					return false;
				};

				// common event handler (keyup, ok and cancel)
				common = function (event) {
					self.hide();
					self.unbind(document.body, "keyup", key);
					self.unbind(btnReset, "focus", reset);
					if (hasOK) self.unbind(btnOK, "click", ok);
					if (hasCancel) self.unbind(btnCancel, "click", cancel);
				};

				// keyup handler
				key = function (event) {
					var keyCode = event.keyCode;
					if ((keyCode === keys.SPACE && !hasInput) || (hasInput && keyCode === keys.ENTER)) ok(event);
					if (keyCode === keys.ESC && hasCancel) cancel(event);
				};

				// reset focus to first item in the dialog
				reset = function (event) {
					if (hasInput) input.focus();
					else if (!hasCancel || self.buttonReverse) btnOK.focus();
					else btnCancel.focus();
				};

				// handle reset focus link
				// this ensures that the keyboard focus does not
				// ever leave the dialog box until an action has
				// been taken
				this.bind(btnReset, "focus", reset);
				this.bind(btnResetBack, "focus", reset);
				// handle OK click
				if (hasOK) this.bind(btnOK, "click", ok);
				// handle Cancel click
				if (hasCancel) this.bind(btnCancel, "click", cancel);
				// listen for keys, Cancel => ESC
				this.bind(document.body, "keyup", key);
				if (!this.transition.supported) {
					this.setFocus();
				}
			},

			/**
			 * Bind events to elements
			 *
			 * @param  {Object}   el       HTML Object
			 * @param  {Event}    event    Event to attach to element
			 * @param  {Function} fn       Callback function
			 *
			 * @return {undefined}
			 */
			bind : function (el, event, fn) {
				if (typeof el.addEventListener === "function") {
					el.addEventListener(event, fn, false);
				} else if (el.attachEvent) {
					el.attachEvent("on" + event, fn);
				}
			},

			/**
			 * Use alertify as the global error handler (using window.onerror)
			 *
			 * @return {boolean} success
			 */
			handleErrors : function () {
				if (typeof global.onerror !== "undefined") {
					var self = this;
					global.onerror = function (msg, url, line) {
						self.error("[" + msg + " on line " + line + " of " + url + "]", 0);
					};
					return true;
				} else {
					return false;
				}
			},

			/**
			 * Append button HTML strings
			 *
			 * @param {String} secondary    The secondary button HTML string
			 * @param {String} primary      The primary button HTML string
			 *
			 * @return {String}             The appended button HTML strings
			 */
			appendButtons : function (secondary, primary) {
				return this.buttonReverse ? primary + secondary : secondary + primary;
			},

			/**
			 * Build the proper message box
			 *
			 * @param  {Object} item    Current object in the queue
			 *
			 * @return {String}         An HTML string of the message box
			 */
			build : function (item) {
				var html    = "",
				    type    = item.type,
				    message = item.message,
				    css     = item.cssClass || "";

				html += "<div class=\"alertify-dialog\">";
				html += "<a id=\"alertify-resetFocusBack\" class=\"alertify-resetFocus\" href=\"#\">Reset Focus</a>";

				if (_alertify.buttonFocus === "none") html += "<a href=\"#\" id=\"alertify-noneFocus\" class=\"alertify-hidden\"></a>";

				// doens't require an actual form
				if (type === "prompt") html += "<div id=\"alertify-form\">";

				html += "<article class=\"alertify-inner\">";
				html += dialogs.message.replace("{{message}}", message);

				if (type === "prompt") html += dialogs.input;

				html += dialogs.buttons.holder;
				html += "</article>";

				if (type === "prompt") html += "</div>";

				html += "<a id=\"alertify-resetFocus\" class=\"alertify-resetFocus\" href=\"#\">Reset Focus</a>";
				html += "</div>";

				switch (type) {
				case "confirm":
					html = html.replace("{{buttons}}", this.appendButtons(dialogs.buttons.cancel, dialogs.buttons.ok));
					html = html.replace("{{ok}}", this.labels.ok).replace("{{cancel}}", this.labels.cancel);
					break;
				case "prompt":
					html = html.replace("{{buttons}}", this.appendButtons(dialogs.buttons.cancel, dialogs.buttons.submit));
					html = html.replace("{{ok}}", this.labels.ok).replace("{{cancel}}", this.labels.cancel);
					break;
				case "alert":
					html = html.replace("{{buttons}}", dialogs.buttons.ok);
					html = html.replace("{{ok}}", this.labels.ok);
					break;
				default:
					break;
				}

				elDialog.className = "alertify alertify-" + type + " " + css;
				elCover.className  = "alertify-cover";
				return html;
			},

			/**
			 * Close the log messages
			 *
			 * @param  {Object} elem    HTML Element of log message to close
			 * @param  {Number} wait    [optional] Time (in ms) to wait before automatically hiding the message, if 0 never hide
			 *
			 * @return {undefined}
			 */
			close : function (elem, wait) {
				// Unary Plus: +"2" === 2
				var timer = (wait && !isNaN(wait)) ? +wait : this.delay,
				    self  = this,
				    hideElement, transitionDone;

				// set click event on log messages
				this.bind(elem, "click", function () {
					hideElement(elem);
				});
				// Hide the dialog box after transition
				// This ensure it doens't block any element from being clicked
				transitionDone = function (event) {
					event.stopPropagation();
					// unbind event so function only gets called once
					self.unbind(this, self.transition.type, transitionDone);
					// remove log message
					elLog.removeChild(this);
					if (!elLog.hasChildNodes()) elLog.className += " alertify-logs-hidden";
				};
				// this sets the hide class to transition out
				// or removes the child if css transitions aren't supported
				hideElement = function (el) {
					// ensure element exists
					if (typeof el !== "undefined" && el.parentNode === elLog) {
						// whether CSS transition exists
						if (self.transition.supported) {
							self.bind(el, self.transition.type, transitionDone);
							el.className += " alertify-log-hide";
						} else {
							elLog.removeChild(el);
							if (!elLog.hasChildNodes()) elLog.className += " alertify-logs-hidden";
						}
					}
				};
				// never close (until click) if wait is set to 0
				if (wait === 0) return;
				// set timeout to auto close the log message
				setTimeout(function () { hideElement(elem); }, timer);
			},

			/**
			 * Create a dialog box
			 *
			 * @param  {String}   message        The message passed from the callee
			 * @param  {String}   type           Type of dialog to create
			 * @param  {Function} fn             [Optional] Callback function
			 * @param  {String}   placeholder    [Optional] Default value for prompt input field
			 * @param  {String}   cssClass       [Optional] Class(es) to append to dialog box
			 *
			 * @return {Object}
			 */
			dialog : function (message, type, fn, placeholder, cssClass) {
				// set the current active element
				// this allows the keyboard focus to be resetted
				// after the dialog box is closed
				elCallee = document.activeElement;
				// check to ensure the alertify dialog element
				// has been successfully created
				var check = function () {
					if ((elLog && elLog.scrollTop !== null) && (elCover && elCover.scrollTop !== null)) return;
					else check();
				};
				// error catching
				if (typeof message !== "string") throw new Error("message must be a string");
				if (typeof type !== "string") throw new Error("type must be a string");
				if (typeof fn !== "undefined" && typeof fn !== "function") throw new Error("fn must be a function");
				// initialize alertify if it hasn't already been done
				this.init();
				check();

				queue.push({ type: type, message: message, callback: fn, placeholder: placeholder, cssClass: cssClass });
				if (!isopen) this.setup();

				return this;
			},

			/**
			 * Extend the log method to create custom methods
			 *
			 * @param  {String} type    Custom method name
			 *
			 * @return {Function}
			 */
			extend : function (type) {
				if (typeof type !== "string") throw new Error("extend method must have exactly one paramter");
				return function (message, wait) {
					this.log(message, type, wait);
					return this;
				};
			},

			/**
			 * Hide the dialog and rest to defaults
			 *
			 * @return {undefined}
			 */
			hide : function () {
				var transitionDone,
				    self = this;
				// remove reference from queue
				queue.splice(0,1);
				// if items remaining in the queue
				if (queue.length > 0) this.setup(true);
				else {
					isopen = false;
					// Hide the dialog box after transition
					// This ensure it doens't block any element from being clicked
					transitionDone = function (event) {
						event.stopPropagation();
						// unbind event so function only gets called once
						self.unbind(elDialog, self.transition.type, transitionDone);
					};
					// whether CSS transition exists
					if (this.transition.supported) {
						this.bind(elDialog, this.transition.type, transitionDone);
						elDialog.className = "alertify alertify-hide alertify-hidden";
					} else {
						elDialog.className = "alertify alertify-hide alertify-hidden alertify-isHidden";
					}
					elCover.className  = "alertify-cover alertify-cover-hidden";
					// set focus to the last element or body
					// after the dialog is closed
					elCallee.focus();
				}
			},

			/**
			 * Initialize Alertify
			 * Create the 2 main elements
			 *
			 * @return {undefined}
			 */
			init : function () {
				// ensure legacy browsers support html5 tags
				document.createElement("nav");
				document.createElement("article");
				document.createElement("section");
				// cover
				if ($("alertify-cover") == null) {
					elCover = document.createElement("div");
					elCover.setAttribute("id", "alertify-cover");
					elCover.className = "alertify-cover alertify-cover-hidden";
					document.body.appendChild(elCover);
				}
				// main element
				if ($("alertify") == null) {
					isopen = false;
					queue = [];
					elDialog = document.createElement("section");
					elDialog.setAttribute("id", "alertify");
					elDialog.className = "alertify alertify-hidden";
					document.body.appendChild(elDialog);
				}
				// log element
				if ($("alertify-logs") == null) {
					elLog = document.createElement("section");
					elLog.setAttribute("id", "alertify-logs");
					elLog.className = "alertify-logs alertify-logs-hidden";
					document.body.appendChild(elLog);
				}
				// set tabindex attribute on body element
				// this allows script to give it focus
				// after the dialog is closed
				document.body.setAttribute("tabindex", "0");
				// set transition type
				this.transition = getTransitionEvent();
			},

			/**
			 * Show a new log message box
			 *
			 * @param  {String} message    The message passed from the callee
			 * @param  {String} type       [Optional] Optional type of log message
			 * @param  {Number} wait       [Optional] Time (in ms) to wait before auto-hiding the log
			 *
			 * @return {Object}
			 */
			log : function (message, type, wait) {
				// check to ensure the alertify dialog element
				// has been successfully created
				var check = function () {
					if (elLog && elLog.scrollTop !== null) return;
					else check();
				};
				// initialize alertify if it hasn't already been done
				this.init();
				check();

				elLog.className = "alertify-logs";
				this.notify(message, type, wait);
				return this;
			},

			/**
			 * Add new log message
			 * If a type is passed, a class name "alertify-log-{type}" will get added.
			 * This allows for custom look and feel for various types of notifications.
			 *
			 * @param  {String} message    The message passed from the callee
			 * @param  {String} type       [Optional] Type of log message
			 * @param  {Number} wait       [Optional] Time (in ms) to wait before auto-hiding
			 *
			 * @return {undefined}
			 */
			notify : function (message, type, wait) {
				var log = document.createElement("article");
				log.className = "alertify-log" + ((typeof type === "string" && type !== "") ? " alertify-log-" + type : "");
				log.innerHTML = message;
				// append child
				elLog.appendChild(log);
				// triggers the CSS animation
				setTimeout(function() { log.className = log.className + " alertify-log-show"; }, 50);
				this.close(log, wait);
			},

			/**
			 * Set properties
			 *
			 * @param {Object} args     Passing parameters
			 *
			 * @return {undefined}
			 */
			set : function (args) {
				var k;
				// error catching
				if (typeof args !== "object" && args instanceof Array) throw new Error("args must be an object");
				// set parameters
				for (k in args) {
					if (args.hasOwnProperty(k)) {
						this[k] = args[k];
					}
				}
			},

			/**
			 * Common place to set focus to proper element
			 *
			 * @return {undefined}
			 */
			setFocus : function () {
				if (input) {
					input.focus();
					input.select();
				}
				else btnFocus.focus();
			},

			/**
			 * Initiate all the required pieces for the dialog box
			 *
			 * @return {undefined}
			 */
			setup : function (fromQueue) {
				var item = queue[0],
				    self = this,
				    transitionDone;

				// dialog is open
				isopen = true;
				// Set button focus after transition
				transitionDone = function (event) {
					event.stopPropagation();
					self.setFocus();
					// unbind event so function only gets called once
					self.unbind(elDialog, self.transition.type, transitionDone);
				};
				// whether CSS transition exists
				if (this.transition.supported && !fromQueue) {
					this.bind(elDialog, this.transition.type, transitionDone);
				}
				// build the proper dialog HTML
				elDialog.innerHTML = this.build(item);
				// assign all the common elements
				btnReset  = $("alertify-resetFocus");
				btnResetBack  = $("alertify-resetFocusBack");
				btnOK     = $("alertify-ok")     || undefined;
				btnCancel = $("alertify-cancel") || undefined;
				btnFocus  = (_alertify.buttonFocus === "cancel") ? btnCancel : ((_alertify.buttonFocus === "none") ? $("alertify-noneFocus") : btnOK),
				input     = $("alertify-text")   || undefined;
				form      = $("alertify-form")   || undefined;
				// add placeholder value to the input field
				if (typeof item.placeholder === "string" && item.placeholder !== "") input.value = item.placeholder;
				if (fromQueue) this.setFocus();
				this.addListeners(item.callback);
			},

			/**
			 * Unbind events to elements
			 *
			 * @param  {Object}   el       HTML Object
			 * @param  {Event}    event    Event to detach to element
			 * @param  {Function} fn       Callback function
			 *
			 * @return {undefined}
			 */
			unbind : function (el, event, fn) {
				if (typeof el.removeEventListener === "function") {
					el.removeEventListener(event, fn, false);
				} else if (el.detachEvent) {
					el.detachEvent("on" + event, fn);
				}
			}
		};

		return {
			alert   : function (message, fn, cssClass) { _alertify.dialog(message, "alert", fn, "", cssClass); return this; },
			confirm : function (message, fn, cssClass) { _alertify.dialog(message, "confirm", fn, "", cssClass); return this; },
			extend  : _alertify.extend,
			init    : _alertify.init,
			log     : function (message, type, wait) { _alertify.log(message, type, wait); return this; },
			prompt  : function (message, fn, placeholder, cssClass) { _alertify.dialog(message, "prompt", fn, placeholder, cssClass); return this; },
			success : function (message, wait) { _alertify.log(message, "success", wait); return this; },
			error   : function (message, wait) { _alertify.log(message, "error", wait); return this; },
			set     : function (args) { _alertify.set(args); },
			labels  : _alertify.labels,
			debug   : _alertify.handleErrors
		};
	};

	// AMD and window support
	if (typeof define === "function") {
		define([], function () { return new Alertify(); });
	} else if (typeof global.alertify === "undefined") {
		global.alertify = new Alertify();
	}

}(this));

!function(t){"function"==typeof define&&define.amd?define(["jquery"],t):"object"==typeof module&&module.exports?module.exports=t(require("jquery")):t(jQuery)}(function(t){var e=function(){function e(){var e=this,n=function(){var n=["br-wrapper"];""!==e.options.theme&&n.push("br-theme-"+e.options.theme),e.$elem.wrap(t("<div />",{"class":n.join(" ")}))},i=function(){e.$elem.unwrap()},a=function(n){return t.isNumeric(n)&&(n=Math.floor(n)),t('option[value="'+n+'"]',e.$elem)},r=function(){var n=e.options.initialRating;return n?a(n):t("option:selected",e.$elem)},o=function(){var n=e.$elem.find('option[value="'+e.options.emptyValue+'"]');return!n.length&&e.options.allowEmpty?(n=t("<option />",{value:e.options.emptyValue}),n.prependTo(e.$elem)):n},l=function(t){var n=e.$elem.data("barrating");return"undefined"!=typeof t?n[t]:n},s=function(t,n){null!==n&&"object"==typeof n?e.$elem.data("barrating",n):e.$elem.data("barrating")[t]=n},u=function(){var t=r(),n=o(),i=t.val(),a=t.data("html")?t.data("html"):t.text(),l=null!==e.options.allowEmpty?e.options.allowEmpty:!!n.length,u=n.length?n.val():null,d=n.length?n.text():null;s(null,{userOptions:e.options,ratingValue:i,ratingText:a,originalRatingValue:i,originalRatingText:a,allowEmpty:l,emptyRatingValue:u,emptyRatingText:d,readOnly:e.options.readonly,ratingMade:!1})},d=function(){e.$elem.removeData("barrating")},c=function(){return l("ratingText")},f=function(){return l("ratingValue")},g=function(){var n=t("<div />",{"class":"br-widget"});return e.$elem.find("option").each(function(){var i,a,r,o;i=t(this).val(),i!==l("emptyRatingValue")&&(a=t(this).text(),r=t(this).data("html"),r&&(a=r),o=t("<a />",{href:"#","data-rating-value":i,"data-rating-text":a,html:e.options.showValues?a:""}),n.append(o))}),e.options.showSelectedRating&&n.append(t("<div />",{text:"","class":"br-current-rating"})),e.options.reverse&&n.addClass("br-reverse"),e.options.readonly&&n.addClass("br-readonly"),n},p=function(){return l("userOptions").reverse?"nextAll":"prevAll"},h=function(t){a(t).prop("selected",!0),l("userOptions").triggerChange&&e.$elem.change()},m=function(){t("option",e.$elem).prop("selected",function(){return this.defaultSelected}),l("userOptions").triggerChange&&e.$elem.change()},v=function(t){t=t?t:c(),t==l("emptyRatingText")&&(t=""),e.options.showSelectedRating&&e.$elem.parent().find(".br-current-rating").text(t)},y=function(t){return Math.round(Math.floor(10*t)/10%1*100)},b=function(){e.$widget.find("a").removeClass(function(t,e){return(e.match(/(^|\s)br-\S+/g)||[]).join(" ")})},w=function(){var n,i,a=e.$widget.find('a[data-rating-value="'+f()+'"]'),r=l("userOptions").initialRating,o=t.isNumeric(f())?f():0,s=y(r);if(b(),a.addClass("br-selected br-current")[p()]().addClass("br-selected"),!l("ratingMade")&&t.isNumeric(r)){if(o>=r||!s)return;n=e.$widget.find("a"),i=a.length?a[l("userOptions").reverse?"prev":"next"]():n[l("userOptions").reverse?"last":"first"](),i.addClass("br-fractional"),i.addClass("br-fractional-"+s)}},$=function(t){return l("allowEmpty")&&l("userOptions").deselectable?f()==t.attr("data-rating-value"):!1},x=function(n){n.on("click.barrating",function(n){var i,a,r=t(this),o=l("userOptions");return n.preventDefault(),i=r.attr("data-rating-value"),a=r.attr("data-rating-text"),$(r)&&(i=l("emptyRatingValue"),a=l("emptyRatingText")),s("ratingValue",i),s("ratingText",a),s("ratingMade",!0),h(i),v(a),w(),o.onSelect.call(e,f(),c(),n),!1})},C=function(e){e.on("mouseenter.barrating",function(){var e=t(this);b(),e.addClass("br-active")[p()]().addClass("br-active"),v(e.attr("data-rating-text"))})},O=function(t){e.$widget.on("mouseleave.barrating blur.barrating",function(){v(),w()})},R=function(e){e.on("touchstart.barrating",function(e){e.preventDefault(),e.stopPropagation(),t(this).click()})},V=function(t){t.on("click.barrating",function(t){t.preventDefault()})},S=function(t){x(t),e.options.hoverState&&(C(t),O(t))},T=function(t){t.off(".barrating")},j=function(t){var n=e.$widget.find("a");l("userOptions").fastClicks&&R(n),t?(T(n),V(n)):S(n)};this.show=function(){l()||(n(),u(),e.$widget=g(),e.$widget.insertAfter(e.$elem),w(),v(),j(e.options.readonly),e.$elem.hide())},this.readonly=function(t){"boolean"==typeof t&&l("readOnly")!=t&&(j(t),s("readOnly",t),e.$widget.toggleClass("br-readonly"))},this.set=function(t){var n=l("userOptions");0!==e.$elem.find('option[value="'+t+'"]').length&&(s("ratingValue",t),s("ratingText",e.$elem.find('option[value="'+t+'"]').text()),s("ratingMade",!0),h(f()),v(c()),w(),n.silent||n.onSelect.call(this,f(),c()))},this.clear=function(){var t=l("userOptions");s("ratingValue",l("originalRatingValue")),s("ratingText",l("originalRatingText")),s("ratingMade",!1),m(),v(c()),w(),t.onClear.call(this,f(),c())},this.destroy=function(){var t=f(),n=c(),a=l("userOptions");T(e.$widget.find("a")),e.$widget.remove(),d(),i(),e.$elem.show(),a.onDestroy.call(this,t,n)}}return e.prototype.init=function(e,n){return this.$elem=t(n),this.options=t.extend({},t.fn.barrating.defaults,e),this.options},e}();t.fn.barrating=function(n,i){return this.each(function(){var a=new e;if(t(this).is("select")||t.error("Sorry, this plugin only works with select fields."),a.hasOwnProperty(n)){if(a.init(i,this),"show"===n)return a.show(i);if(a.$elem.data("barrating"))return a.$widget=t(this).next(".br-widget"),a[n](i)}else{if("object"==typeof n||!n)return i=n,a.init(i,this),a.show();t.error("Method "+n+" does not exist on jQuery.barrating")}})},t.fn.barrating.defaults={theme:"",initialRating:null,allowEmpty:null,emptyValue:"",showValues:!1,showSelectedRating:!0,deselectable:!0,reverse:!1,readonly:!1,fastClicks:!0,hoverState:!0,silent:!1,triggerChange:!0,onSelect:function(t,e,n){},onClear:function(t,e){},onDestroy:function(t,e){}},t.fn.barrating.BarRating=e});
//# sourceMappingURL=jquery.barrating.min.js.map
/**
 * Copyright (c) 2007 Ariel Flesler - aflesler ○ gmail • com | https://github.com/flesler
 * Licensed under MIT
 * @author Ariel Flesler
 * @version 2.1.2
 */
;(function(f){"use strict";"function"===typeof define&&define.amd?define(["jquery"],f):"undefined"!==typeof module&&module.exports?module.exports=f(require("jquery")):f(jQuery)})(function($){"use strict";function n(a){return!a.nodeName||-1!==$.inArray(a.nodeName.toLowerCase(),["iframe","#document","html","body"])}function h(a){return $.isFunction(a)||$.isPlainObject(a)?a:{top:a,left:a}}var p=$.scrollTo=function(a,d,b){return $(window).scrollTo(a,d,b)};p.defaults={axis:"xy",duration:0,limit:!0};$.fn.scrollTo=function(a,d,b){"object"=== typeof d&&(b=d,d=0);"function"===typeof b&&(b={onAfter:b});"max"===a&&(a=9E9);b=$.extend({},p.defaults,b);d=d||b.duration;var u=b.queue&&1<b.axis.length;u&&(d/=2);b.offset=h(b.offset);b.over=h(b.over);return this.each(function(){function k(a){var k=$.extend({},b,{queue:!0,duration:d,complete:a&&function(){a.call(q,e,b)}});r.animate(f,k)}if(null!==a){var l=n(this),q=l?this.contentWindow||window:this,r=$(q),e=a,f={},t;switch(typeof e){case "number":case "string":if(/^([+-]=?)?\d+(\.\d+)?(px|%)?$/.test(e)){e= h(e);break}e=l?$(e):$(e,q);case "object":if(e.length===0)return;if(e.is||e.style)t=(e=$(e)).offset()}var v=$.isFunction(b.offset)&&b.offset(q,e)||b.offset;$.each(b.axis.split(""),function(a,c){var d="x"===c?"Left":"Top",m=d.toLowerCase(),g="scroll"+d,h=r[g](),n=p.max(q,c);t?(f[g]=t[m]+(l?0:h-r.offset()[m]),b.margin&&(f[g]-=parseInt(e.css("margin"+d),10)||0,f[g]-=parseInt(e.css("border"+d+"Width"),10)||0),f[g]+=v[m]||0,b.over[m]&&(f[g]+=e["x"===c?"width":"height"]()*b.over[m])):(d=e[m],f[g]=d.slice&& "%"===d.slice(-1)?parseFloat(d)/100*n:d);b.limit&&/^\d+$/.test(f[g])&&(f[g]=0>=f[g]?0:Math.min(f[g],n));!a&&1<b.axis.length&&(h===f[g]?f={}:u&&(k(b.onAfterFirst),f={}))});k(b.onAfter)}})};p.max=function(a,d){var b="x"===d?"Width":"Height",h="scroll"+b;if(!n(a))return a[h]-$(a)[b.toLowerCase()]();var b="client"+b,k=a.ownerDocument||a.document,l=k.documentElement,k=k.body;return Math.max(l[h],k[h])-Math.min(l[b],k[b])};$.Tween.propHooks.scrollLeft=$.Tween.propHooks.scrollTop={get:function(a){return $(a.elem)[a.prop]()}, set:function(a){var d=this.get(a);if(a.options.interrupt&&a._last&&a._last!==d)return $(a.elem).stop();var b=Math.round(a.now);d!==b&&($(a.elem)[a.prop](b),a._last=this.get(a))}};return p});

String.prototype.replaceAll = function (search, replacement) {
    var target = this;
    return target.replace(new RegExp(search, 'g'), replacement);
};

function myajax(url, data, State, show_loading) {
    show_loading = typeof show_loading !== 'undefined' ? show_loading : true;
    if (show_loading)
        show_loading_icon();
    $.ajax({
        type: "GET",
        url: "/" + url,
        contentType: "application/json; charset=utf-8",
        data: data,
        cache: false,
        success: function (result) {
            hide_loading_icon();
            if (typeof State == 'function') {
                State(result);
            }
        },
        error: function (result) { hide_loading_icon(); alert("خطایی رخ داده است."); },
        complete: function (result) { hide_loading_icon(); }
    });
}
function clientIsInIran(callback) {
    $.ajax('https://www.extreme-ip-lookup.com/json/')
    .then(
        function success(response) {
            callback(response.countryCode == "IR");
        }
    );
}

function show_loading_icon() {
    $("#loading-icon").fadeIn();
}

function hide_loading_icon() {
    $("#loading-icon").fadeOut();
}

$(window).focus(function () {
    hide_loading_icon();
});

$(document).ajaxComplete(function () {
    hide_loading_icon();
});
/******rating*******/

function add_comment(user_id) {

    if (user_id > 0) {

        if ($(".js-cm-text").val() == "") {
            alertify.error("لطفا نظر خود را وارد کنید");
            $(".js-cm-text").focus();
            return false;
        }

        $('#addCommentForm').submit();
    }
    else {
        alertify.error("لطفا ابتدا وارد سایت شوید");
    }
}

function onAddComment(data) {
    if (data.status == 1) {
        alertify.success("نظر شما با موفقیت ثبت شد. پس از تایید کارشناس بر روی سایت قرار میگیرد");
    }
    else {
        alertify.error(data.val);
    }
}

$(document).ready(function () {
    bar_rating();
});

function bar_rating() {
    $('.score-rating').barrating({
        theme: 'fontawesome-stars',
        readonly: false,
        deselectable: false,
        onSelect: function (value, text, event) {
            if (typeof (event) !== 'undefined') {
                // rating was selected by a user
                var report_elem = $($(this)[0].$elem);
                if ($('#UserId').val() > 0) {
                    myajax("Advertise/AddScore", "advertiseID=" + $("#AdvertiseID").val()
                        + "&ReportID=" + report_elem.attr("report_id")
                        + "&value=" + value
                        + "&user_id=" + $('#UserId').val(), function (ret) {
                            if (ret.status == 1) {
                                //alertify.success("امتیاز شما با موفقیت ثبت شد");
                            }
                            else {
                                alertify.error(ret.val);
                            }
                        }, false);
                }
            } else {
                alertify.error("خطایی رخ داد، لطفا بعدا امتحان کنید.");
            }
        }
    });
}

function ratingDialog(reserve_id, onClose) {
    //var advertise = guestAdvertiseRatingArray[index];
    //settings: color, buttons, onClose, contentUrl, onContentReady, onOpen, disableKeyEvents
    var url = '/reserve/ratingdialog?reserveid=' + reserve_id;
    showMessagePopup('نظرسنجی', '', {
        contentUrl: url,
        onContentReady: bar_rating,
        onClose: onClose,
    });
    //$.confirm({
    //    title: 'نظرسنجی',
    //    content: 'لطفا با امتیاز دهی و نظر درباره اقامتگاه دیگران را راهنمایی کنید.' +
    //        '<div id="js-rating-container"></div>',
    //    type: 'blue',
    //    typeAnimated: true,
    //    columnClass: 'xlarge',
    //    buttons: {
    //        بستن: function () {
    //            if (onClose != undefined && onClose != null) {
    //                onClose();
    //            }
    //        }
    //    },
    //    onContentReady: function () {
    //        $('#js-rating-container').load('/reserve/ratingdialog?reserveid=' +
    //            reserve_id, bar_rating);
    //    }
    //});
}

/******end rating*******/
function validateEmail(elementValue) {
    var emailPattern = /^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/;
    return emailPattern.test(elementValue);
}

function validateMobile(mobile) {
    return (mobile.match(/[0][9][0-9]{9}/) && mobile.length == 11) ||
        (mobile.substring(0, 2) == "00" && mobile.length > 11);
}

function persianNumberToEnglish(persian_number) {
    var persian = ["۰", "۱", "۲", "۳", "۴", "۵", "۶", "۷", "۸", "۹"];
    var arabic = ["٠", "١", "٢", "٣", "٤", "٥", "٦", "٧", "٨", "٩"];
    persian_number = persian_number.trim();
    for (var i = 0; i < persian.length; i++) {
        persian_number = persian_number.replaceAll(persian[i], i.toString());
        persian_number = persian_number.replaceAll(arabic[i], i.toString());
    }
    return persian_number;
}

function validateTell(tell) {
    if (!tell.match(/0+[1-8]+\d{9}/)) {
        return false
    }
    else {
        if (tell.length != 11)
            return false;
        return true;

    }
}

function validateNumber(element) {
    var value = element.value;
    value = persianNumberToEnglish(value);
    if (!value.match(/^\d+$/)) {
        value = value.replace(/\D/g, '');
    }
    element.value = value;
}
//Menu
var menu_shown = false;

var messageQuestionList = [];

function searchAdvertiseId() {
    showInfoMessage('جستجو با کد آگهی',
        '<div style="display:flex;flex-flow:row;width:100%;max-width:320px;justify-content:space-around;">' +
        '<input onpaste="onSearchAdvertiseIdChange(event, this)" onkeyup="onSearchAdvertiseIdChange(event, this)" onchange="onSearchAdvertiseIdChange(event, this)" style="width: 65%;max-width: 170px;padding: 5px;" autofocus placeholder="کد آگهی را وارد کنید" />' +
        '<div onclick="doSearchByAdvertiseId(this.previousSibling)" style="background-color:#fdd835;border-radius:5px;padding:5px 10px;cursor:pointer;">جستجو</div>' +
        '</div>'
        );
}

function onSearchAdvertiseIdChange(e, elem) {
    if (e != null && e.keyCode === 13) {
        setTimeout(function () {
            doSearchByAdvertiseId(elem);
        }, 500);
        return;
    }
}

function doSearchByAdvertiseId(inputElem) {
    var id = $(inputElem).val();
    if (typeof id == 'undefined' ||
        id == null ||
        id == '') {
        id = 0;
    }
    myajax('accomodation/getaccurlbyid', 'id=' + id, function (ret) {
        if (ret.status == 0) {
            showErrorMessage('خطا', 'کد آگهی یافت نشد. لطفا کد وارد شده را بررسی کنید.');
            return;
        }
        window.open(ret.url, '_self');
    });
}

function toggleMenu() {
    if (menu_shown) {
        $(".master__header-menu").removeClass("master__header-menu-open");
        $(".master__menu").slideUp();
        menu_shown = false;
    }
    else {
        $(".master__header-menu").addClass("master__header-menu-open");
        $(".master__menu").slideDown(function () {
            if ($(this).is(':visible'))
                $(this).css('display', 'flex');
        });
        menu_shown = true;
    }
}

function sendTokenToServer(token) {
    myajax("user/updateusernotificationtoken", "token=" + token, function (ret) {
    }, false);
}

//End Menu  

function userAskQuestion(id, question) {
    messageQuestionList.push(question);
    if ($('.support-chat__chat-box').is(":visible")) {
        var url = "/supportchat/getchatpopup?id=" + id +
            "&questionlist=" + JSON.stringify(messageQuestionList);
        $(".support-chat__chat-box").load(url, function () {
            var chatPopupChildren = $('.chat-popup__container').children();
            console.log("scrolling into view");
            chatPopupChildren[1].scrollIntoView();
        });
    }
    //var chatPopupChildren = $('.chat-popup__container').children();
    //chatPopupChildren[chatPopupChildren.length - 1].scrollIntoView();
    //myajax('supportchat/useraskquestion',
    //    'id=' + id + '&question=' + question, function (ret) {
    //        if (ret.status == 1) {

    //            if ($('.support-chat__chat-box').is(":visible")) {
    //                var url = "/supportchat/getchatpopup?id=" + id;
    //                $(".support-chat__chat-box").load(url);
    //            }
    //            var chatPopupChildren = $('.chat-popup__container').children();
    //            chatPopupChildren[chatPopupChildren.length - 1].scrollIntoView();
    //        }
    //    });
}

function getReserveToRate() {
    myajax('reserve/getreservetorate', null, function (ret) {
        if (ret.status == 1) {
            ratingDialog(ret.reserveToRate);
        }
    }, false);
}

function sendTokenToServer(token) {
    myajax("user/updateusernotificationtoken", "token=" + token, function (ret) {
    }, false);
}

//$('body').prepend('<a href="#" class="back-to-top"></a>');
//var amountScrolled = 300;

//$(window).scroll(function () {
//    if ($(window).scrollTop() > amountScrolled) {
//        $('a.back-to-top').fadeIn('slow');
//    } else {
//        $('a.back-to-top').fadeOut('slow');
//    }
//});
//$('a.back-to-top').click(function () {
//    $('html, body').animate({
//        scrollTop: 0
//    }, 700);
//    return false;
//});

var current_user_id = 0;
var presentPopupAfterLogin = false;
var isUserloggedIn = false;

function on_login() {
    isUserLoggedIn = true;
    if (typeof on_login_action !== "undefined"){
        on_login_action();
    }
    myajax('user/fetchuserid', '', function(ret) {
        if (ret.status == 1) {
            current_user_id = ret.userId;
        }
    });
    if (presentPopupAfterLogin) {
        showPresentPopup();
    }
}

const portalHubConnection = new signalR.HubConnectionBuilder()
    .withUrl("/PortalHub")
    .build();

var registered_service_worker;

function updateAccDetail() {
    var accIds = "";
    var accId;
    $('.home-page__category-item-container-large').each(function () {
        accId = $(this).attr("id");
        if (accId != undefined && accId.match(/^advertise_\d+/)) {
            accIds += accId.match(/\d+/)[0] + ",";
        }
    });
    if (accIds != "") {
        accIds = accIds.substring(0, accIds.length - 1);
        $.ajax({
            type: "POST",
            url: "/accomodation/getacclistdynamicviewbag",
            data: "{ids:'" + accIds + "'}",
            contentType: "application/json",
            dataType: "json",
            success: function (ret) {
                var dict = ret.price_dict;
                for (var key in dict) {
                    // check if the property/key is defined in the object itself, not in parent
                    if (dict.hasOwnProperty(key)) {
                        $("#advertise_" + key).find('#js-daily-price').html("شروع قیمت: " + dict[key]);
                    }
                }
            }
        });
    }
}

$(document).ready(function () {
    initializePage();
    //if (!lockInPage) {
        //$('#header').load('/post/getheader?ishomepage=' + isHomePage);
        //$('.footer').load('/post/getfooter', function () {
        //});
    //}
});

//function onLoadFooter() {
//    if (current_user_id == 0) {
//        $('.js-present-prize-link').attr('href', '#');
//        $('.js-present-prize-link').attr('onclick', 'showPresentPopup()');
//    }
//    //$('#drftsguidrftnbpewmcs').attr('src', 'https://trustseal.enamad.ir/logo.aspx?id=10128&p=lznbgthvlznbqesgukaq');
//    $('#jxlznbqejxlzfukzapfuapfu').attr('src', '/file/resourceimagewebp?file_name=samandehi');
//    $('#js-footer-map-image').attr('src', '/file/resourceimagewebp?file_name=map');

//    $('#js-facebook-icon').attr('src', '/file/resourceimagepng?file_name=socialnetwork/facebook');
//    $('#js-twitter-icon').attr('src', '/file/resourceimagepng?file_name=socialnetwork/twitter');
//    $('#js-googleplus-icon').attr('src', '/file/resourceimagepng?file_name=socialnetwork/googleplus');
//    $('#js-instagram-icon').attr('src', '/file/resourceimagepng?file_name=socialnetwork/instagram');
//    $('#js-aparat-icon').attr('src', '/file/resourceimagepng?file_name=socialnetwork/aparat');
//    $('#js-telegram-icon').attr('src', '/file/resourceimagepng?file_name=socialnetwork/telegram');
//}
(function () {
    if (current_user_id == 0)
    {
      $('.js-present-prize-link').attr('href', '#');
      $('.js-present-prize-link').attr('onclick', 'showPresentPopup()');
    }
})
//$(window).load(function () {
//    initializePage();
//});
//document.addEventListener("DOMContentLoaded", function () {
//    initializePage();
//});

function initializeSupportChat() {
    $('body').append('<div id="js-temp-holder-master"></div>');
    var $tempHolder = $('#js-temp-holder-master');
    $tempHolder.load('/supportchat/getsupportchatuser', function () {
        $tempHolder.children().each(function () {
            $('body').prepend($(this));
        });
        $tempHolder.empty();
        $(".support-chat__text-input").keydown(function () {
            $(this).css('height', 'auto');
            $(this).css('height', this.scrollHeight);
        });
    });
    $tempHolder.remove();
    if (!checkUserVisited()) {
        setTimeout(showSupportChatInform, 500)
    }
}

function initializePresentPrize() {
    debugger;
    var shown = checkPresentShown();
    if (!shown) {
        $('.present-prize__button').css('display', 'flex');
    }
    else {
        $('.present-prize__button').css('display', 'none');
    }
}

function initializeLoginPopup() {
    //$('body').append('<div id="js-temp-holder-master-2"></div>');
    //var $tempHolder_2 = $('#js-temp-holder-master-2');
    //$tempHolder_2.load('/user/getloginpopup', function () {
    //    $tempHolder_2.children().each(function () {
    //        $('body').prepend($(this));
    //    });
    //    $tempHolder_2.empty();
    //});
    //$tempHolder_2.remove();
}

function initializeMasterHub() {
    portalHubConnection.on('reloadSupportChat', (supportChatId, newCount, userId) => {
        var id = $('#js-support-chat-id').val();
        supportChatId = parseInt(supportChatId);
        id = parseInt(id);
        if (supportChatId == id || (userId > 0 && userId == current_user_id)) {
            $('#js-support-chat-id').val(supportChatId);
            refreshChatBox(supportChatId, newCount);
        }
    });
    portalHubConnection.start()
        .then(() => console.log('portal hub connected!'))
        .catch(console.error);
}

function initializeServiceWorker() {
    if ('serviceWorker' in navigator) {
        navigator.serviceWorker.getRegistration("/service_worker.js").then(registration => {
            if (registration == undefined) {
                navigator.serviceWorker.register('/service_worker.js')
                 .then(reg => {
                     console.log('Service worker registered! 😎', reg);
                     messaging.useServiceWorker(reg);
                     registered_service_worker = reg;
                     check_user_login();
                 })
                 .catch(err => {
                     console.log('😥 Service worker registration failed: ', err);
                     check_user_login();
                     sendTokenToServer(null);
                 });
            }
            else {
                messaging.useServiceWorker(registration);
                registered_service_worker = registration;
                check_user_login();
            }
        });
    }
    else {
        check_user_login();
        sendTokenToServer(null);
    }
}

function initializePage() {
    try {
        initializeServiceWorker();
    }
    catch (err) {
    }
    initializeLoginPopup();
    getReserveToRate();
    initializeMasterHub();
    //window.$crisp = []; window.CRISP_WEBSITE_ID = "76fba52f-295b-48d8-a14e-4ace311f993e"; (function () { d = document; s = d.createElement("script"); s.src = "https://client.crisp.chat/l.js"; s.async = 1; d.getElementsByTagName("head")[0].appendChild(s); })();
    initializeSupportChat();
    initializePresentPrize();
    setTimeout(updateAccDetail, 1000);
}

function checkUserVisited() {
    var visited = getCookie("visited") === "yes";
    setCookie("visited", "yes", 365);
    return visited;
}

function checkPresentShown() {
    debugger;
    var shown = getCookie("reserveprizeshown") === "yes";
    return shown;
}

function setPresentShown(temp) {
    if (temp) {
        setCookieForHours("reserveprizeshown", "yes", 3);
    }
    else {
        setCookie("reserveprizeshown", "yes", 365);
    }
}

presentPrizePopup = null;

function showPresentPopup() {
    $('.present-prize__button').hide();
    var setting = {};
    setting.contentUrl = '/post/getpresentandprizepopup';
    var buttons = [{
        title: 'نمایش نده',
        color: 'white',
        bgColor: '#EA4335',
        onclick: function () {
            setPresentShown(false);
        }
    },
    {
        title: 'بعدا',
        color: 'white',
        bgColor: '#34A853',
        onclick: function () {
            setPresentShown(true);
        }
    }];
    setting.buttons = buttons;
    setting.color = '#4485F2';
    showMessagePopup('هدیه سفر', '', setting);
}

function hidePresentPopup() {
    if (presentPrizePopup) {
        presentPrizePopup.close();
    }
}

function check_user_login() {
    myajax("user/isuserauthenticated", "", function (ret) {
        debugger;
        if (ret.val) {
            isUserLoggedIn = true;
            if (ret.impersonateData.state) {
                $('#js-impersonate-banner').show();
                $('#js-impersonate-fullname').html('ورود به عنوان ' + ret.impersonateData.fullName);
            }
            myajax("user/isuserloginbanned", "", function (ret) {
                $('.master_header-account').css('display', 'flex');
                if (ret.val) {
                    myajax("user/logoutajax", "", function (ret) { });
                    $('.master_header-account').attr('href', '#');
                    $('.master_header-account').attr('onclick', 'toggle_login()');
                    $('.master_header-account').children('p').html('<i class="fa fa-user"></i> ورود');
                    $('.js-present-prize-link').attr('href', '#');
                    $('.js-present-prize-link').attr('onclick', 'showPresentPopup()');
                }
                else {
                    $('.master_header-account').attr('href', "/dashboard");
                    $('.master_header-account').attr('onclick', '');
                    $('.master_header-account').children('p').html('<i class="fa fa-user"></i> حساب من ');
                    current_user_id = ret.user_id;
                    if (Notification.permission !== "granted" &&
                        Notification.permission !== "denied") {
                        myajax('user/getpermissionrequestdate', '', function (ret) {
                            var date;
                            if (ret.ticks !== 0) {
                                var now_ticks = ((new Date().getTime() * 10000) + 621355968000000000);
                                var last_ticks = ret.ticks;
                                var diff = now_ticks - last_ticks;
                                var one_day_passed = diff > 864000000000;
                            }
                            if (ret.status === 0 || ret.ticks === 0 || one_day_passed) {
                                showNoYesMessage("امکان اطلاع رسانی رزرو", "کاربر گرامی، لطفا جهت اطلاع رسانی از مراحل رزرو، پیغامی را که بعد از تایید نمایش داده میشود را تایید فرمایید",
                                function () {
                                    messaging.requestPermission().then(function () {
                                        console.log('Notification permission granted.');
                                        // Get Instance ID token. Initially this makes a network call, once retrieved
                                        // subsequent calls to getToken will return from cache.
                                        messaging.getToken().then(function (currentToken) {
                                            if (currentToken) {
                                                sendTokenToServer(currentToken);
                                            } else {
                                                // Show permission request.
                                                console.log('No Instance ID token available. Request permission to generate one.');
                                                // Show permission UI.
                                                sendTokenToServer(null);
                                            }
                                        }).catch(function (err) {
                                            console.log('An error occurred while retrieving token. ', err);
                                            sendTokenToServer(null);
                                        });
                                    }).catch(function (err) {
                                        console.log('Unable to get permission to notify.', err);
                                        sendTokenToServer(null);
                                    });
                                }, function () {
                                    var t = ((new Date().getTime() * 10000) + 621355968000000000);
                                    myajax('user/setpermissionrequestdate', 'ticks=' + t, function (ret) { }, false);
                                }, { yesText: 'باشه', noText: 'بعدا سوال شود' });
                            }
                        }, false);
                    }
                    else if (Notification.permission === "granted") {
                        messaging.getToken().then(function (currentToken) {
                            if (currentToken) {
                                sendTokenToServer(currentToken);
                            } else {
                                // Show permission request.
                                console.log('No Instance ID token available. Request permission to generate one.');
                                // Show permission UI.
                                sendTokenToServer(null);
                            }
                        }).catch(function (err) {
                            console.log('An error occurred while retrieving token. ', err);
                            sendTokenToServer(null);
                        });
                    }
                    else {
                        sendTokenToServer(null);
                    }
                    //messaging.onMessage(function (payload) {
                    //    console.log('Message received. ', payload);
                    //    var actions = [];
                    //    if (payload.data.btn1) {
                    //        actions.push(
                    //            { action: payload.data.btn1, title: payload.data.btn1_title }
                    //        );
                    //    }
                    //    if (payload.data.btn2) {
                    //        actions.push(
                    //            { action: payload.data.btn2, title: payload.data.btn2_title }
                    //        );
                    //    }
                    //    if (payload.data.btn3) {
                    //        actions.push(
                    //            { action: payload.data.btn3, title: payload.data.btn3_title }
                    //        );
                    //    }
                    //    if (payload.data.btn4) {
                    //        actions.push(
                    //            { action: payload.data.btn4, title: payload.data.btn4_title }
                    //        );
                    //    }
                    //    var notificationOptions = {
                    //        body: payload.notification.body,
                    //        icon: '/Resource/img/siteicons/icon-144x144.png',
                    //        badge: '/Resource/img/siteicons/badge.png',
                    //        data: payload.data,
                    //        actions: actions,
                    //        vibrate: [200, 100, 200, 100, 200, 100, 200]
                    //    };
                    //    registered_service_worker.showNotification(payload.notification.title,
                    //        notificationOptions);
                    //});
                }
            }, false);
        }
        else {
            isUserLoggedIn = false;
            $('.master_header-account').css('display', 'flex');
            $('.master_header-account').attr('href', '#');
            $('.master_header-account').attr('onclick', 'toggle_login()');
            $('.master_header-account').children('p').html('<i class="fa fa-user"></i> ورود');
            $('.js-present-prize-link').attr('href', '#');
            $('.js-present-prize-link').attr('onclick', 'showPresentPopup()');
        }
    }, false);
    // Callback fired if Instance ID token is updated.
    messaging.onTokenRefresh(function () {
        messaging.getToken().then(function (refreshedToken) {
            console.log('Token refreshed.');
            // Send Instance ID token to app server.
            sendTokenToServer(refreshedToken);
        }).catch(function (err) {
            console.log('Unable to retrieve refreshed token ', err);
        });
    });
}

function chat_box_inputkeyup() {
    if (event.keyCode === 13) {
        sendSupportChatMessage();
    }
}

var is_sending_chat = false;

function showSupportChat() {
    hideSupportChatInform();
    var id = parseInt($('#js-support-chat-id').val());
    var url = "/supportchat/getchatpopup?id=" + id;
    $('.support-chat__chat-box').load(url, function () {
        if (chatInputIsOpen)
            openChatInput();
    });
    $('.support-chat__button').hide();
    $('.support-chat__container').show(500);
    $(".support-chat__text-input").focus();
    $('.installPopup').slideUp();
}

function showSupportChatInform() {
    $('.support-chat__user-inform').show(500);
}

function hideSupportChatInform() {
    $('.support-chat__user-inform').hide(500);
}

function hideSupportChat() {
    $('.support-chat__container').hide(500);
    $('.support-chat__button').show();
}

var chatInputIsOpen = false;

function openChatInput() {
    $('.support-chat__input-container').show();
    $(".support-chat__text-input").focus();
    chatInputIsOpen = true;
}

function sendSupportChatMessage(id, text, questionNumber) {
    if (id == undefined) {
        id = parseInt($('#js-support-chat-id').val());
    }
    if (is_sending_chat) {
        return;
    }
    if (typeof text === 'undefined') {
        text = $(".support-chat__text-input").val();
    }
    if (text == '')
        return;
    if (!isUserLoggedIn) {
        showNoYesMessage('ورود به سایت',
            'برای چت با پشتیبانی ابتدا باید وارد سایت شوید', toggle_login, undefined,
            { yesText: 'ورود', noText: 'بستن' });
        return;
    }
    is_sending_chat = true;
    $(".support-chat__text-input").val("");

    $.ajax({
        type: "POST",
        url: "/supportchat/sendtextuser",
        data: "{user_id:" + current_user_id + ",id:" + id +
            ",text:'" + text + "'" +
            (typeof questionNumber === 'undefined' ? "" : ",questionnumber:" + questionNumber) +
            "}",
        contentType: "application/json",
        dataType: "json",
        success: function (ret) {
            is_sending_chat = false;
            if (ret.status == 1) {
                debugger;
                $('#js-support-chat-id').val(ret.id);
                hub.server.reloadSupportChat(ret.id, 0,
                    current_user_id);
                $(".support-chat__text-input").focus();
                $(".support-chat__chat-box").stop().animate({ scrollTop: $(".support-chat__chat-box")[0].scrollHeight }, 1000);
            }
            else {
                showErrorMessage('خطا',
                    'متاسفانه ارسال پیام با خطا مواجه شد');
            }
        }
    });
}

//function refreshSupporterName(id){
//    if (id == undefined) {
//        id = parseInt($('#js-support-chat-id').val());
//    }
//    if (id > 0) {
//        myajax('supportchat/getsupportername','id=' + id,function(ret){
//            $('#js-supporter-label').html('پشتیبان: ' + ret.name);
//        },false);
//    }
//}

function refreshChatBox(id, newCount) {
    if ($('.support-chat__chat-box').is(":visible")) {
        var url = "/supportchat/getchatpopup?id=" + id +
            "&questionlist=" + JSON.stringify(messageQuestionList);
        $(".support-chat__chat-box").load(url);
    }
    newCount = parseInt(newCount);
    if (newCount > 0) {
        $('.support-chat__new-count').html(newCount);
        $('.support-chat__new-count').show();
    }
    else {
        $('.support-chat__new-count').hide();
    }
    var chatPopupChildren = $('.chat-popup__container').children();
    chatPopupChildren[chatPopupChildren.length-1].scrollIntoView();
}

var iOS = false,
p = navigator.platform;
if (p === 'iPad' || p === 'iPhone' || p === 'iPod') {
    iOS = true;
}
//if (iOS === false) {
    //let deferredPrompt;
    //window.addEventListener('beforeinstallprompt', event => {

    //    // Prevent Chrome 67 and earlier from automatically showing the prompt
    //    event.preventDefault();

    //    // Stash the event so it can be triggered later.
    //    deferredPrompt = event;

    //    // Attach the install prompt to a user gesture
    //    $('.installBtn').click(function () {
    //        // Show the prompt
    //        deferredPrompt.prompt();

    //        // Wait for the user to respond to the prompt
    //        deferredPrompt.userChoice
    //          .then((choiceResult) => {
    //              if (choiceResult.outcome === 'accepted') {
    //                  $('.installBanner').css('display', 'none');
    //                  $('.installPopup').slideUp();
    //                  console.log('User accepted the A2HS prompt');
    //              } else {
    //                  console.log('User dismissed the A2HS prompt');
    //              }
    //              deferredPrompt = null;
    //          });
    //    });
        // Update UI notify the user they can add to home screen
        //$('.installBanner').css('display', 'flex');
        //$('.installPopup').slideDown();
    //});
    //document.addEventListener("scroll", function () {
    //    if (window.pageYOffset > 1000) {
    //        $('.installPopup').slideUp();
    //    }
    //});
//}
//document.addEventListener("DOMContentLoaded", function () {

//});

$(document).ready(function () {
    findLazyImages();
    lazyLoad();
    document.addEventListener("scroll", lazyLoad);
    window.addEventListener("resize", lazyLoad);
    window.addEventListener("orientationchange", lazyLoad);

    document.addEventListener("scroll", asyncPartialLoad);
    window.addEventListener("resize", asyncPartialLoad);
    window.addEventListener("orientationchange", asyncPartialLoad);
    $(".initialPartialContents").each(function (index, item) {
        $(item).removeClass("initialPartialContents");
        $(item).empty();
        var url = $(item).data("url");
        var onLoad = $(item).data("onLoad");
        if (url && url.length > 0) {
            var onLoad = $(item).attr("data-onLoad");
            var onLoadfn;
            if (onLoad != undefined && onLoad != null) {
                onLoadfn = window[onLoad];
            }
            $(item).load(url, onLoadfn);
        }
    });
});

function elementInViewport(el) {
    var top = el.offsetTop;
    var left = el.offsetLeft;
    var width = el.offsetWidth;
    var height = el.offsetHeight;

    while (el.offsetParent) {
        el = el.offsetParent;
        top += el.offsetTop;
        left += el.offsetLeft;
    }

    return (
      top < (window.pageYOffset + window.innerHeight) &&
      left < (window.pageXOffset + window.innerWidth) &&
      (top + height) > window.pageYOffset &&
      (left + width) > window.pageXOffset
    );
}

function asyncPartialLoad() {
    $(".partialContents").each(function (index, item) {
        if (elementInViewport(item)) {
            $(item).removeClass("partialContents");
            $(item).empty();
            var url = $(item).data("url");
            if (url && url.length > 0) {
                var onLoad = $(item).attr("data-onLoad");
                var onLoadfn;
                if (onLoad != undefined && onLoad != null) {
                    onLoadfn = window[onLoad];
                }
                $(item).load(url, onLoadfn);
            }
        }
    });
}

var $images;

function findLazyImages() {
    $images = $('img.lazy');
}

var lazy_load_active = false;

function lazyLoad() {
    if ($images != null && $images.length > 0) {
        if (!lazy_load_active) {
            lazy_load_active = true;
            $images.each(function () {
                lazyImage = this;
                if ((lazyImage.getBoundingClientRect().top <= window.innerHeight && lazyImage.getBoundingClientRect().bottom >= 0)) {
                    var attr = $(this).attr('data-src');
                    if (attr != undefined && attr != null) {
                        $(this).attr('src', attr);
                    }
                    attr = $(this).attr('data-srcset');
                    if (attr != undefined && attr != null) {
                        $(this).attr('srcset', attr);
                    }
                    $(this).removeClass('lazy');
                }
            });
            lazy_load_active = false;
        }
    }
}
/*
 * International Telephone Input v17.0.3
 * https://github.com/jackocnr/intl-tel-input.git
 * Licensed under the MIT license
 */

!function(a){"object"==typeof module&&module.exports?module.exports=a():window.intlTelInput=a()}(function(a){"use strict";return function(){function b(a,b){if(!(a instanceof b))throw new TypeError("Cannot call a class as a function")}function c(a,b){for(var c=0;c<b.length;c++){var d=b[c];d.enumerable=d.enumerable||!1,d.configurable=!0,"value"in d&&(d.writable=!0),Object.defineProperty(a,d.key,d)}}function d(a,b,d){return b&&c(a.prototype,b),d&&c(a,d),a}for(var e=[["Afghanistan (‫افغانستان‬‎)","af","93"],["Albania (Shqipëri)","al","355"],["Algeria (‫الجزائر‬‎)","dz","213"],["American Samoa","as","1",5,["684"]],["Andorra","ad","376"],["Angola","ao","244"],["Anguilla","ai","1",6,["264"]],["Antigua and Barbuda","ag","1",7,["268"]],["Argentina","ar","54"],["Armenia (Հայաստան)","am","374"],["Aruba","aw","297"],["Australia","au","61",0],["Austria (Österreich)","at","43"],["Azerbaijan (Azərbaycan)","az","994"],["Bahamas","bs","1",8,["242"]],["Bahrain (‫البحرين‬‎)","bh","973"],["Bangladesh (বাংলাদেশ)","bd","880"],["Barbados","bb","1",9,["246"]],["Belarus (Беларусь)","by","375"],["Belgium (België)","be","32"],["Belize","bz","501"],["Benin (Bénin)","bj","229"],["Bermuda","bm","1",10,["441"]],["Bhutan (འབྲུག)","bt","975"],["Bolivia","bo","591"],["Bosnia and Herzegovina (Босна и Херцеговина)","ba","387"],["Botswana","bw","267"],["Brazil (Brasil)","br","55"],["British Indian Ocean Territory","io","246"],["British Virgin Islands","vg","1",11,["284"]],["Brunei","bn","673"],["Bulgaria (България)","bg","359"],["Burkina Faso","bf","226"],["Burundi (Uburundi)","bi","257"],["Cambodia (កម្ពុជា)","kh","855"],["Cameroon (Cameroun)","cm","237"],["Canada","ca","1",1,["204","226","236","249","250","289","306","343","365","387","403","416","418","431","437","438","450","506","514","519","548","579","581","587","604","613","639","647","672","705","709","742","778","780","782","807","819","825","867","873","902","905"]],["Cape Verde (Kabu Verdi)","cv","238"],["Caribbean Netherlands","bq","599",1,["3","4","7"]],["Cayman Islands","ky","1",12,["345"]],["Central African Republic (République centrafricaine)","cf","236"],["Chad (Tchad)","td","235"],["Chile","cl","56"],["China (中国)","cn","86"],["Christmas Island","cx","61",2,["89164"]],["Cocos (Keeling) Islands","cc","61",1,["89162"]],["Colombia","co","57"],["Comoros (‫جزر القمر‬‎)","km","269"],["Congo (DRC) (Jamhuri ya Kidemokrasia ya Kongo)","cd","243"],["Congo (Republic) (Congo-Brazzaville)","cg","242"],["Cook Islands","ck","682"],["Costa Rica","cr","506"],["Côte d’Ivoire","ci","225"],["Croatia (Hrvatska)","hr","385"],["Cuba","cu","53"],["Curaçao","cw","599",0],["Cyprus (Κύπρος)","cy","357"],["Czech Republic (Česká republika)","cz","420"],["Denmark (Danmark)","dk","45"],["Djibouti","dj","253"],["Dominica","dm","1",13,["767"]],["Dominican Republic (República Dominicana)","do","1",2,["809","829","849"]],["Ecuador","ec","593"],["Egypt (‫مصر‬‎)","eg","20"],["El Salvador","sv","503"],["Equatorial Guinea (Guinea Ecuatorial)","gq","240"],["Eritrea","er","291"],["Estonia (Eesti)","ee","372"],["Ethiopia","et","251"],["Falkland Islands (Islas Malvinas)","fk","500"],["Faroe Islands (Føroyar)","fo","298"],["Fiji","fj","679"],["Finland (Suomi)","fi","358",0],["France","fr","33"],["French Guiana (Guyane française)","gf","594"],["French Polynesia (Polynésie française)","pf","689"],["Gabon","ga","241"],["Gambia","gm","220"],["Georgia (საქართველო)","ge","995"],["Germany (Deutschland)","de","49"],["Ghana (Gaana)","gh","233"],["Gibraltar","gi","350"],["Greece (Ελλάδα)","gr","30"],["Greenland (Kalaallit Nunaat)","gl","299"],["Grenada","gd","1",14,["473"]],["Guadeloupe","gp","590",0],["Guam","gu","1",15,["671"]],["Guatemala","gt","502"],["Guernsey","gg","44",1,["1481","7781","7839","7911"]],["Guinea (Guinée)","gn","224"],["Guinea-Bissau (Guiné Bissau)","gw","245"],["Guyana","gy","592"],["Haiti","ht","509"],["Honduras","hn","504"],["Hong Kong (香港)","hk","852"],["Hungary (Magyarország)","hu","36"],["Iceland (Ísland)","is","354"],["India (भारत)","in","91"],["Indonesia","id","62"],["Iran (‫ایران‬‎)","ir","98"],["Iraq (‫العراق‬‎)","iq","964"],["Ireland","ie","353"],["Isle of Man","im","44",2,["1624","74576","7524","7924","7624"]],["Israel (‫ישראל‬‎)","il","972"],["Italy (Italia)","it","39",0],["Jamaica","jm","1",4,["876","658"]],["Japan (日本)","jp","81"],["Jersey","je","44",3,["1534","7509","7700","7797","7829","7937"]],["Jordan (‫الأردن‬‎)","jo","962"],["Kazakhstan (Казахстан)","kz","7",1,["33","7"]],["Kenya","ke","254"],["Kiribati","ki","686"],["Kosovo","xk","383"],["Kuwait (‫الكويت‬‎)","kw","965"],["Kyrgyzstan (Кыргызстан)","kg","996"],["Laos (ລາວ)","la","856"],["Latvia (Latvija)","lv","371"],["Lebanon (‫لبنان‬‎)","lb","961"],["Lesotho","ls","266"],["Liberia","lr","231"],["Libya (‫ليبيا‬‎)","ly","218"],["Liechtenstein","li","423"],["Lithuania (Lietuva)","lt","370"],["Luxembourg","lu","352"],["Macau (澳門)","mo","853"],["Macedonia (FYROM) (Македонија)","mk","389"],["Madagascar (Madagasikara)","mg","261"],["Malawi","mw","265"],["Malaysia","my","60"],["Maldives","mv","960"],["Mali","ml","223"],["Malta","mt","356"],["Marshall Islands","mh","692"],["Martinique","mq","596"],["Mauritania (‫موريتانيا‬‎)","mr","222"],["Mauritius (Moris)","mu","230"],["Mayotte","yt","262",1,["269","639"]],["Mexico (México)","mx","52"],["Micronesia","fm","691"],["Moldova (Republica Moldova)","md","373"],["Monaco","mc","377"],["Mongolia (Монгол)","mn","976"],["Montenegro (Crna Gora)","me","382"],["Montserrat","ms","1",16,["664"]],["Morocco (‫المغرب‬‎)","ma","212",0],["Mozambique (Moçambique)","mz","258"],["Myanmar (Burma) (မြန်မာ)","mm","95"],["Namibia (Namibië)","na","264"],["Nauru","nr","674"],["Nepal (नेपाल)","np","977"],["Netherlands (Nederland)","nl","31"],["New Caledonia (Nouvelle-Calédonie)","nc","687"],["New Zealand","nz","64"],["Nicaragua","ni","505"],["Niger (Nijar)","ne","227"],["Nigeria","ng","234"],["Niue","nu","683"],["Norfolk Island","nf","672"],["North Korea (조선 민주주의 인민 공화국)","kp","850"],["Northern Mariana Islands","mp","1",17,["670"]],["Norway (Norge)","no","47",0],["Oman (‫عُمان‬‎)","om","968"],["Pakistan (‫پاکستان‬‎)","pk","92"],["Palau","pw","680"],["Palestine (‫فلسطين‬‎)","ps","970"],["Panama (Panamá)","pa","507"],["Papua New Guinea","pg","675"],["Paraguay","py","595"],["Peru (Perú)","pe","51"],["Philippines","ph","63"],["Poland (Polska)","pl","48"],["Portugal","pt","351"],["Puerto Rico","pr","1",3,["787","939"]],["Qatar (‫قطر‬‎)","qa","974"],["Réunion (La Réunion)","re","262",0],["Romania (România)","ro","40"],["Russia (Россия)","ru","7",0],["Rwanda","rw","250"],["Saint Barthélemy","bl","590",1],["Saint Helena","sh","290"],["Saint Kitts and Nevis","kn","1",18,["869"]],["Saint Lucia","lc","1",19,["758"]],["Saint Martin (Saint-Martin (partie française))","mf","590",2],["Saint Pierre and Miquelon (Saint-Pierre-et-Miquelon)","pm","508"],["Saint Vincent and the Grenadines","vc","1",20,["784"]],["Samoa","ws","685"],["San Marino","sm","378"],["São Tomé and Príncipe (São Tomé e Príncipe)","st","239"],["Saudi Arabia (‫المملكة العربية السعودية‬‎)","sa","966"],["Senegal (Sénégal)","sn","221"],["Serbia (Србија)","rs","381"],["Seychelles","sc","248"],["Sierra Leone","sl","232"],["Singapore","sg","65"],["Sint Maarten","sx","1",21,["721"]],["Slovakia (Slovensko)","sk","421"],["Slovenia (Slovenija)","si","386"],["Solomon Islands","sb","677"],["Somalia (Soomaaliya)","so","252"],["South Africa","za","27"],["South Korea (대한민국)","kr","82"],["South Sudan (‫جنوب السودان‬‎)","ss","211"],["Spain (España)","es","34"],["Sri Lanka (ශ්‍රී ලංකාව)","lk","94"],["Sudan (‫السودان‬‎)","sd","249"],["Suriname","sr","597"],["Svalbard and Jan Mayen","sj","47",1,["79"]],["Swaziland","sz","268"],["Sweden (Sverige)","se","46"],["Switzerland (Schweiz)","ch","41"],["Syria (‫سوريا‬‎)","sy","963"],["Taiwan (台灣)","tw","886"],["Tajikistan","tj","992"],["Tanzania","tz","255"],["Thailand (ไทย)","th","66"],["Timor-Leste","tl","670"],["Togo","tg","228"],["Tokelau","tk","690"],["Tonga","to","676"],["Trinidad and Tobago","tt","1",22,["868"]],["Tunisia (‫تونس‬‎)","tn","216"],["Turkey (Türkiye)","tr","90"],["Turkmenistan","tm","993"],["Turks and Caicos Islands","tc","1",23,["649"]],["Tuvalu","tv","688"],["U.S. Virgin Islands","vi","1",24,["340"]],["Uganda","ug","256"],["Ukraine (Україна)","ua","380"],["United Arab Emirates (‫الإمارات العربية المتحدة‬‎)","ae","971"],["United Kingdom","gb","44",0],["United States","us","1",0],["Uruguay","uy","598"],["Uzbekistan (Oʻzbekiston)","uz","998"],["Vanuatu","vu","678"],["Vatican City (Città del Vaticano)","va","39",1,["06698"]],["Venezuela","ve","58"],["Vietnam (Việt Nam)","vn","84"],["Wallis and Futuna (Wallis-et-Futuna)","wf","681"],["Western Sahara (‫الصحراء الغربية‬‎)","eh","212",1,["5288","5289"]],["Yemen (‫اليمن‬‎)","ye","967"],["Zambia","zm","260"],["Zimbabwe","zw","263"],["Åland Islands","ax","358",1,["18"]]],f=0;f<e.length;f++){var g=e[f];e[f]={name:g[0],iso2:g[1],dialCode:g[2],priority:g[3]||0,areaCodes:g[4]||null}}var h={getInstance:function(a){var b=a.getAttribute("data-intl-tel-input-id");return window.intlTelInputGlobals.instances[b]},instances:{}};"object"==typeof window&&(window.intlTelInputGlobals=h);var i=0,j={allowDropdown:!0,autoHideDialCode:!0,autoPlaceholder:"polite",customContainer:"",customPlaceholder:null,dropdownContainer:null,excludeCountries:[],formatOnDisplay:!0,geoIpLookup:null,hiddenInput:"",initialCountry:"",localizedCountries:null,nationalMode:!0,onlyCountries:[],placeholderNumberType:"MOBILE",preferredCountries:["us","gb"],separateDialCode:!1,utilsScript:""},k=["800","822","833","844","855","866","877","880","881","882","883","884","885","886","887","888","889"];"object"==typeof window&&window.addEventListener("load",function(){window.intlTelInputGlobals.windowLoaded=!0});var l=function(a,b){for(var c=Object.keys(a),d=0;d<c.length;d++)b(c[d],a[c[d]])},m=function(a){l(window.intlTelInputGlobals.instances,function(b){window.intlTelInputGlobals.instances[b][a]()})},n=function(){function c(a,d){var e=this;b(this,c),this.id=i++,this.a=a,this.b=null,this.c=null;var f=d||{};this.d={},l(j,function(a,b){e.d[a]=f.hasOwnProperty(a)?f[a]:b}),this.e=Boolean(a.getAttribute("placeholder"))}return d(c,[{key:"_init",value:function(){var a=this;if(this.d.nationalMode&&(this.d.autoHideDialCode=!1),this.d.separateDialCode&&(this.d.autoHideDialCode=this.d.nationalMode=!1),this.g=/Android.+Mobile|webOS|iPhone|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent),this.g&&(document.body.classList.add("iti-mobile"),this.d.dropdownContainer||(this.d.dropdownContainer=document.body)),"undefined"!=typeof Promise){var b=new Promise(function(b,c){a.h=b,a.i=c}),c=new Promise(function(b,c){a.i0=b,a.i1=c});this.promise=Promise.all([b,c])}else this.h=this.i=function(){},this.i0=this.i1=function(){};this.s={},this._b(),this._f(),this._h(),this._i(),this._i3()}},{key:"_b",value:function(){this._d(),this._d2(),this._e(),this.d.localizedCountries&&this._d0(),(this.d.onlyCountries.length||this.d.localizedCountries)&&this.p.sort(this._d1)}},{key:"_c",value:function(b,c,d){c.length>this.countryCodeMaxLen&&(this.countryCodeMaxLen=c.length),this.q.hasOwnProperty(c)||(this.q[c]=[]);for(var e=0;e<this.q[c].length;e++)if(this.q[c][e]===b)return;var f=d!==a?d:this.q[c].length;this.q[c][f]=b}},{key:"_d",value:function(){if(this.d.onlyCountries.length){var a=this.d.onlyCountries.map(function(a){return a.toLowerCase()});this.p=e.filter(function(b){return a.indexOf(b.iso2)>-1})}else if(this.d.excludeCountries.length){var b=this.d.excludeCountries.map(function(a){return a.toLowerCase()});this.p=e.filter(function(a){return-1===b.indexOf(a.iso2)})}else this.p=e}},{key:"_d0",value:function(){for(var a=0;a<this.p.length;a++){var b=this.p[a].iso2.toLowerCase();this.d.localizedCountries.hasOwnProperty(b)&&(this.p[a].name=this.d.localizedCountries[b])}}},{key:"_d1",value:function(a,b){return a.name.localeCompare(b.name)}},{key:"_d2",value:function(){this.countryCodeMaxLen=0,this.dialCodes={},this.q={};for(var a=0;a<this.p.length;a++){var b=this.p[a];this.dialCodes[b.dialCode]||(this.dialCodes[b.dialCode]=!0),this._c(b.iso2,b.dialCode,b.priority)}for(var c=0;c<this.p.length;c++){var d=this.p[c];if(d.areaCodes)for(var e=this.q[d.dialCode][0],f=0;f<d.areaCodes.length;f++){for(var g=d.areaCodes[f],h=1;h<g.length;h++){var i=d.dialCode+g.substr(0,h);this._c(e,i),this._c(d.iso2,i)}this._c(d.iso2,d.dialCode+g)}}}},{key:"_e",value:function(){this.preferredCountries=[];for(var a=0;a<this.d.preferredCountries.length;a++){var b=this.d.preferredCountries[a].toLowerCase(),c=this._y(b,!1,!0);c&&this.preferredCountries.push(c)}}},{key:"_e2",value:function(a,b,c){var d=document.createElement(a);return b&&l(b,function(a,b){return d.setAttribute(a,b)}),c&&c.appendChild(d),d}},{key:"_f",value:function(){this.a.hasAttribute("autocomplete")||this.a.form&&this.a.form.hasAttribute("autocomplete")||this.a.setAttribute("autocomplete","off");var a="iti";this.d.allowDropdown&&(a+=" iti--allow-dropdown"),this.d.separateDialCode&&(a+=" iti--separate-dial-code"),this.d.customContainer&&(a+=" ",a+=this.d.customContainer);var b=this._e2("div",{"class":a});if(this.a.parentNode.insertBefore(b,this.a),this.k=this._e2("div",{"class":"iti__flag-container"},b),b.appendChild(this.a),this.selectedFlag=this._e2("div",{"class":"iti__selected-flag",role:"combobox","aria-owns":"iti-".concat(this.id,"__country-listbox"),"aria-expanded":"false"},this.k),this.l=this._e2("div",{"class":"iti__flag"},this.selectedFlag),this.d.separateDialCode&&(this.t=this._e2("div",{"class":"iti__selected-dial-code"},this.selectedFlag)),this.d.allowDropdown&&(this.selectedFlag.setAttribute("tabindex","0"),this.u=this._e2("div",{"class":"iti__arrow"},this.selectedFlag),this.m=this._e2("ul",{"class":"iti__country-list iti__hide",id:"iti-".concat(this.id,"__country-listbox"),role:"listbox"}),this.preferredCountries.length&&(this._g(this.preferredCountries,"iti__preferred",!0),this._e2("li",{"class":"iti__divider",role:"separator","aria-disabled":"true"},this.m)),this._g(this.p,"iti__standard"),this.d.dropdownContainer?(this.dropdown=this._e2("div",{"class":"iti iti--container"}),this.dropdown.appendChild(this.m)):this.k.appendChild(this.m)),this.d.hiddenInput){var c=this.d.hiddenInput,d=this.a.getAttribute("name");if(d){var e=d.lastIndexOf("[");-1!==e&&(c="".concat(d.substr(0,e),"[").concat(c,"]"))}this.hiddenInput=this._e2("input",{type:"hidden",name:c}),b.appendChild(this.hiddenInput)}}},{key:"_g",value:function(a,b,c){for(var d="",e=0;e<a.length;e++){var f=a[e],g=c?"-preferred":"";d+="<li class='iti__country ".concat(b,"' tabIndex='-1' id='iti-").concat(this.id,"__item-").concat(f.iso2).concat(g,"' role='option' data-dial-code='").concat(f.dialCode,"' data-country-code='").concat(f.iso2,"'>"),d+="<div class='iti__flag-box'><div class='iti__flag iti__".concat(f.iso2,"'></div></div>"),d+="<span class='iti__country-name'>".concat(f.name,"</span>"),d+="<span class='iti__dial-code'>+".concat(f.dialCode,"</span>"),d+="</li>"}this.m.insertAdjacentHTML("beforeend",d)}},{key:"_h",value:function(){var a=this.a.value,b=this._5(a),c=this._w(a),d=this.d,e=d.initialCountry,f=d.nationalMode,g=d.autoHideDialCode,h=d.separateDialCode;b&&!c?this._v(a):"auto"!==e&&(e?this._z(e.toLowerCase()):b&&c?this._z("us"):(this.j=this.preferredCountries.length?this.preferredCountries[0].iso2:this.p[0].iso2,a||this._z(this.j)),a||f||g||h||(this.a.value="+".concat(this.s.dialCode))),a&&this._u(a)}},{key:"_i",value:function(){this._j(),this.d.autoHideDialCode&&this._l(),this.d.allowDropdown&&this._i2(),this.hiddenInput&&this._i0()}},{key:"_i0",value:function(){var a=this;this._a14=function(){a.hiddenInput.value=a.getNumber()},this.a.form&&this.a.form.addEventListener("submit",this._a14)}},{key:"_i1",value:function(){for(var a=this.a;a&&"LABEL"!==a.tagName;)a=a.parentNode;return a}},{key:"_i2",value:function(){var a=this;this._a9=function(b){a.m.classList.contains("iti__hide")?a.a.focus():b.preventDefault()};var b=this._i1();b&&b.addEventListener("click",this._a9),this._a10=function(){!a.m.classList.contains("iti__hide")||a.a.disabled||a.a.readOnly||a._n()},this.selectedFlag.addEventListener("click",this._a10),this._a11=function(b){a.m.classList.contains("iti__hide")&&-1!==["ArrowUp","Up","ArrowDown","Down"," ","Enter"].indexOf(b.key)&&(b.preventDefault(),b.stopPropagation(),a._n()),"Tab"===b.key&&a._2()},this.k.addEventListener("keydown",this._a11)}},{key:"_i3",value:function(){var a=this;this.d.utilsScript&&!window.intlTelInputUtils?window.intlTelInputGlobals.windowLoaded?window.intlTelInputGlobals.loadUtils(this.d.utilsScript):window.addEventListener("load",function(){window.intlTelInputGlobals.loadUtils(a.d.utilsScript)}):this.i0(),"auto"===this.d.initialCountry?this._i4():this.h()}},{key:"_i4",value:function(){window.intlTelInputGlobals.autoCountry?this.handleAutoCountry():window.intlTelInputGlobals.startedLoadingAutoCountry||(window.intlTelInputGlobals.startedLoadingAutoCountry=!0,"function"==typeof this.d.geoIpLookup&&this.d.geoIpLookup(function(a){window.intlTelInputGlobals.autoCountry=a.toLowerCase(),setTimeout(function(){return m("handleAutoCountry")})},function(){return m("rejectAutoCountryPromise")}))}},{key:"_j",value:function(){var a=this;this._a12=function(){a._v(a.a.value)&&a._8()},this.a.addEventListener("keyup",this._a12),this._a13=function(){setTimeout(a._a12)},this.a.addEventListener("cut",this._a13),this.a.addEventListener("paste",this._a13)}},{key:"_j2",value:function(a){var b=this.a.getAttribute("maxlength");return b&&a.length>b?a.substr(0,b):a}},{key:"_l",value:function(){var a=this;this._a8=function(){a._l2()},this.a.form&&this.a.form.addEventListener("submit",this._a8),this.a.addEventListener("blur",this._a8)}},{key:"_l2",value:function(){if("+"===this.a.value.charAt(0)){var a=this._m(this.a.value);a&&this.s.dialCode!==a||(this.a.value="")}}},{key:"_m",value:function(a){return a.replace(/\D/g,"")}},{key:"_m2",value:function(a){var b=document.createEvent("Event");b.initEvent(a,!0,!0),this.a.dispatchEvent(b)}},{key:"_n",value:function(){this.m.classList.remove("iti__hide"),this.selectedFlag.setAttribute("aria-expanded","true"),this._o(),this.b&&(this._x(this.b,!1),this._3(this.b,!0)),this._p(),this.u.classList.add("iti__arrow--up"),this._m2("open:countrydropdown")}},{key:"_n2",value:function(a,b,c){c&&!a.classList.contains(b)?a.classList.add(b):!c&&a.classList.contains(b)&&a.classList.remove(b)}},{key:"_o",value:function(){var a=this;if(this.d.dropdownContainer&&this.d.dropdownContainer.appendChild(this.dropdown),!this.g){var b=this.a.getBoundingClientRect(),c=window.pageYOffset||document.documentElement.scrollTop,d=b.top+c,e=this.m.offsetHeight,f=d+this.a.offsetHeight+e<c+window.innerHeight,g=d-e>c;if(this._n2(this.m,"iti__country-list--dropup",!f&&g),this.d.dropdownContainer){var h=!f&&g?0:this.a.offsetHeight;this.dropdown.style.top="".concat(d+h,"px"),this.dropdown.style.left="".concat(b.left+document.body.scrollLeft,"px"),this._a4=function(){return a._2()},window.addEventListener("scroll",this._a4)}}}},{key:"_o2",value:function(a){for(var b=a;b&&b!==this.m&&!b.classList.contains("iti__country");)b=b.parentNode;return b===this.m?null:b}},{key:"_p",value:function(){var a=this;this._a0=function(b){var c=a._o2(b.target);c&&a._x(c,!1)},this.m.addEventListener("mouseover",this._a0),this._a1=function(b){var c=a._o2(b.target);c&&a._1(c)},this.m.addEventListener("click",this._a1);var b=!0;this._a2=function(){b||a._2(),b=!1},document.documentElement.addEventListener("click",this._a2);var c="",d=null;this._a3=function(b){b.preventDefault(),"ArrowUp"===b.key||"Up"===b.key||"ArrowDown"===b.key||"Down"===b.key?a._q(b.key):"Enter"===b.key?a._r():"Escape"===b.key?a._2():/^[a-zA-ZÀ-ÿа-яА-Я ]$/.test(b.key)&&(d&&clearTimeout(d),c+=b.key.toLowerCase(),a._s(c),d=setTimeout(function(){c=""},1e3))},document.addEventListener("keydown",this._a3)}},{key:"_q",value:function(a){var b="ArrowUp"===a||"Up"===a?this.c.previousElementSibling:this.c.nextElementSibling;b&&(b.classList.contains("iti__divider")&&(b="ArrowUp"===a||"Up"===a?b.previousElementSibling:b.nextElementSibling),this._x(b,!0))}},{key:"_r",value:function(){this.c&&this._1(this.c)}},{key:"_s",value:function(a){for(var b=0;b<this.p.length;b++)if(this._t(this.p[b].name,a)){var c=this.m.querySelector("#iti-".concat(this.id,"__item-").concat(this.p[b].iso2));this._x(c,!1),this._3(c,!0);break}}},{key:"_t",value:function(a,b){return a.substr(0,b.length).toLowerCase()===b}},{key:"_u",value:function(a){var b=a;if(this.d.formatOnDisplay&&window.intlTelInputUtils&&this.s){var c=!this.d.separateDialCode&&(this.d.nationalMode||"+"!==b.charAt(0)),d=intlTelInputUtils.numberFormat,e=d.NATIONAL,f=d.INTERNATIONAL,g=c?e:f;b=intlTelInputUtils.formatNumber(b,this.s.iso2,g)}b=this._7(b),this.a.value=b}},{key:"_v",value:function(a){var b=a,c=this.s.dialCode,d="1"===c;b&&this.d.nationalMode&&d&&"+"!==b.charAt(0)&&("1"!==b.charAt(0)&&(b="1".concat(b)),b="+".concat(b)),this.d.separateDialCode&&c&&"+"!==b.charAt(0)&&(b="+".concat(c).concat(b));var e=this._5(b,!0),f=this._m(b),g=null;if(e){var h=this.q[this._m(e)],i=-1!==h.indexOf(this.s.iso2)&&f.length<=e.length-1;if(!("1"===c&&this._w(f))&&!i)for(var j=0;j<h.length;j++)if(h[j]){g=h[j];break}}else"+"===b.charAt(0)&&f.length?g="":b&&"+"!==b||(g=this.j);return null!==g&&this._z(g)}},{key:"_w",value:function(a){var b=this._m(a);if("1"===b.charAt(0)){var c=b.substr(1,3);return-1!==k.indexOf(c)}return!1}},{key:"_x",value:function(a,b){var c=this.c;c&&c.classList.remove("iti__highlight"),this.c=a,this.c.classList.add("iti__highlight"),b&&this.c.focus()}},{key:"_y",value:function(a,b,c){for(var d=b?e:this.p,f=0;f<d.length;f++)if(d[f].iso2===a)return d[f];if(c)return null;throw new Error("No country data for '".concat(a,"'"))}},{key:"_z",value:function(a){var b=this.s.iso2?this.s:{};this.s=a?this._y(a,!1,!1):{},this.s.iso2&&(this.j=this.s.iso2),this.l.setAttribute("class","iti__flag iti__".concat(a));var c=a?"".concat(this.s.name,": +").concat(this.s.dialCode):"Unknown";if(this.selectedFlag.setAttribute("title",c),this.d.separateDialCode){var d=this.s.dialCode?"+".concat(this.s.dialCode):"";this.t.innerHTML=d;var e=this.selectedFlag.offsetWidth||this._getHiddenSelectedFlagWidth();this.a.style.paddingLeft="".concat(e+6,"px")}if(this._0(),this.d.allowDropdown){var f=this.b;if(f&&(f.classList.remove("iti__active"),f.setAttribute("aria-selected","false")),a){var g=this.m.querySelector("#iti-".concat(this.id,"__item-").concat(a,"-preferred"))||this.m.querySelector("#iti-".concat(this.id,"__item-").concat(a));g.setAttribute("aria-selected","true"),g.classList.add("iti__active"),this.b=g,this.selectedFlag.setAttribute("aria-activedescendant",g.getAttribute("id"))}}return b.iso2!==a}},{key:"_getHiddenSelectedFlagWidth",value:function(){var a=this.a.parentNode.cloneNode();a.style.visibility="hidden",document.body.appendChild(a);var b=this.k.cloneNode();a.appendChild(b);var c=this.selectedFlag.cloneNode(!0);b.appendChild(c);var d=c.offsetWidth;return a.parentNode.removeChild(a),d}},{key:"_0",value:function(){var a="aggressive"===this.d.autoPlaceholder||!this.e&&"polite"===this.d.autoPlaceholder;if(window.intlTelInputUtils&&a){var b=intlTelInputUtils.numberType[this.d.placeholderNumberType],c=this.s.iso2?intlTelInputUtils.getExampleNumber(this.s.iso2,this.d.nationalMode,b):"";c=this._7(c),"function"==typeof this.d.customPlaceholder&&(c=this.d.customPlaceholder(c,this.s)),this.a.setAttribute("placeholder",c)}}},{key:"_1",value:function(a){var b=this._z(a.getAttribute("data-country-code"));this._2(),this._4(a.getAttribute("data-dial-code"),!0),this.a.focus();var c=this.a.value.length;this.a.setSelectionRange(c,c),b&&this._8()}},{key:"_2",value:function(){this.m.classList.add("iti__hide"),this.selectedFlag.setAttribute("aria-expanded","false"),this.u.classList.remove("iti__arrow--up"),document.removeEventListener("keydown",this._a3),document.documentElement.removeEventListener("click",this._a2),this.m.removeEventListener("mouseover",this._a0),this.m.removeEventListener("click",this._a1),this.d.dropdownContainer&&(this.g||window.removeEventListener("scroll",this._a4),this.dropdown.parentNode&&this.dropdown.parentNode.removeChild(this.dropdown)),this._m2("close:countrydropdown")}},{key:"_3",value:function(a,b){var c=this.m,d=window.pageYOffset||document.documentElement.scrollTop,e=c.offsetHeight,f=c.getBoundingClientRect().top+d,g=f+e,h=a.offsetHeight,i=a.getBoundingClientRect().top+d,j=i+h,k=i-f+c.scrollTop,l=e/2-h/2;if(i<f)b&&(k-=l),c.scrollTop=k;else if(j>g){b&&(k+=l);var m=e-h;c.scrollTop=k-m}}},{key:"_4",value:function(a,b){var c,d=this.a.value,e="+".concat(a);if("+"===d.charAt(0)){var f=this._5(d);c=f?d.replace(f,e):e}else{if(this.d.nationalMode||this.d.separateDialCode)return;if(d)c=e+d;else{if(!b&&this.d.autoHideDialCode)return;c=e}}this.a.value=c}},{key:"_5",value:function(a,b){var c="";if("+"===a.charAt(0))for(var d="",e=0;e<a.length;e++){var f=a.charAt(e);if(!isNaN(parseInt(f,10))){if(d+=f,b)this.q[d]&&(c=a.substr(0,e+1));else if(this.dialCodes[d]){c=a.substr(0,e+1);break}if(d.length===this.countryCodeMaxLen)break}}return c}},{key:"_6",value:function(){var a=this.a.value.trim(),b=this.s.dialCode,c=this._m(a);return(this.d.separateDialCode&&"+"!==a.charAt(0)&&b&&c?"+".concat(b):"")+a}},{key:"_7",value:function(a){var b=a;if(this.d.separateDialCode){var c=this._5(b);if(c){c="+".concat(this.s.dialCode);var d=" "===b[c.length]||"-"===b[c.length]?c.length+1:c.length;b=b.substr(d)}}return this._j2(b)}},{key:"_8",value:function(){this._m2("countrychange")}},{key:"handleAutoCountry",value:function(){"auto"===this.d.initialCountry&&(this.j=window.intlTelInputGlobals.autoCountry,this.a.value||this.setCountry(this.j),this.h())}},{key:"handleUtils",value:function(){window.intlTelInputUtils&&(this.a.value&&this._u(this.a.value),this._0()),this.i0()}},{key:"destroy",value:function(){var a=this.a.form;if(this.d.allowDropdown){this._2(),this.selectedFlag.removeEventListener("click",this._a10),this.k.removeEventListener("keydown",this._a11);var b=this._i1();b&&b.removeEventListener("click",this._a9)}this.hiddenInput&&a&&a.removeEventListener("submit",this._a14),this.d.autoHideDialCode&&(a&&a.removeEventListener("submit",this._a8),this.a.removeEventListener("blur",this._a8)),this.a.removeEventListener("keyup",this._a12),this.a.removeEventListener("cut",this._a13),this.a.removeEventListener("paste",this._a13),this.a.removeAttribute("data-intl-tel-input-id");var c=this.a.parentNode;c.parentNode.insertBefore(this.a,c),c.parentNode.removeChild(c),delete window.intlTelInputGlobals.instances[this.id]}},{key:"getExtension",value:function(){return window.intlTelInputUtils?intlTelInputUtils.getExtension(this._6(),this.s.iso2):""}},{key:"getNumber",value:function(a){if(window.intlTelInputUtils){var b=this.s.iso2;return intlTelInputUtils.formatNumber(this._6(),b,a)}return""}},{key:"getNumberType",value:function(){
return window.intlTelInputUtils?intlTelInputUtils.getNumberType(this._6(),this.s.iso2):-99}},{key:"getSelectedCountryData",value:function(){return this.s}},{key:"getValidationError",value:function(){if(window.intlTelInputUtils){var a=this.s.iso2;return intlTelInputUtils.getValidationError(this._6(),a)}return-99}},{key:"isValidNumber",value:function(){var a=this._6().trim(),b=this.d.nationalMode?this.s.iso2:"";return window.intlTelInputUtils?intlTelInputUtils.isValidNumber(a,b):null}},{key:"setCountry",value:function(a){var b=a.toLowerCase();this.l.classList.contains("iti__".concat(b))||(this._z(b),this._4(this.s.dialCode,!1),this._8())}},{key:"setNumber",value:function(a){var b=this._v(a);this._u(a),b&&this._8()}},{key:"setPlaceholderNumberType",value:function(a){this.d.placeholderNumberType=a,this._0()}}]),c}();h.getCountryData=function(){return e};var o=function(a,b,c){var d=document.createElement("script");d.onload=function(){m("handleUtils"),b&&b()},d.onerror=function(){m("rejectUtilsScriptPromise"),c&&c()},d.className="iti-load-utils",d.async=!0,d.src=a,document.body.appendChild(d)};return h.loadUtils=function(a){if(!window.intlTelInputUtils&&!window.intlTelInputGlobals.startedLoadingUtilsScript){if(window.intlTelInputGlobals.startedLoadingUtilsScript=!0,"undefined"!=typeof Promise)return new Promise(function(b,c){return o(a,b,c)});o(a)}return null},h.defaults=j,h.version="17.0.3",function(a,b){var c=new n(a,b);return c._init(),a.setAttribute("data-intl-tel-input-id",c.id),window.intlTelInputGlobals.instances[c.id]=c,c}}()});
//---------intltelInput------------
var input = document.querySelector("#mobile")
var intl = window.intlTelInput(input, {
    preferredCountries: ["ir"],
    utilsScript: "/file/getjs?src=/resource/scripts/general/login/utils.js"
});

//------------------------
//$(".edit-number-login").click(function () {
//    restart_login();
//});
function scrollTop() {
    document.body.scrollTop = 0; // For Safari
    document.documentElement.scrollTop = 0; // For Chrome, Firefox, IE and Opera
}
$(".exit__icon").click(function () {
    $(".login__bg").fadeOut(500);
    $(".login__container").css({ 'transition': '0.8s', 'display': 'none' });
    scrollTop();
    onLoginFinish();
});
//------------------------------------
var can_send_login_message = true;
$(".login__bg").click(function () {
    $('#login-box-details').toggleClass('animate');
});
function toggle_login() {
    if ($(".login__container").css("display") == "none") {
        if (!checked_mobile_current)
            restart_login();
        $(".login__bg").show();
        toggle_login_container(true, function () {
            $(".login__container").css("display", "flex");
            $("input#mobile").focus();
        });
        onLoginStart();
    }
    //$(".login__bg").fadeOut(500);
    //toggle_login_container(false);

}
var pressKey = $(document).keypress(function (event) {

    var keycode = (event.keyCode ? event.keyCode : event.which);
    if ($(".login__container").length == 0 ||
        $(".login__container").css("display") == "none") {
        return;
    }
    if (keycode == '13') {
        if ($("#login_form").css("display") != "none" && $(".input-code").css("display") == "none") {
            login();
        }
        else if ($("#login_form_email").css("display") != "none") {
            login("email");
        }
        else if ($("#verification_form").css("display") != "none") {
            login_verification();
        }
    }

});
function toggle_login_container(direction, onDone) {
    if (direction) {
        $(".login__container").css("opacity", 0);
        $(".login__container").css("display", "flex");
        $(".login__container").animate({
            opacity: 1
        }, 200, onDone);
    }
    else {
        $(".login__container").animate({
            opacity: 0
        }, 200, function () {
            $(".login__container").css("display", "none");
            if (onDone != undefined && onDone != null) {
                onDone();
            }
        });
    }
}
$("#login_form").find("input").keyup(function () {
    if ($(this).val() > 0) {
        $(".login__box-button").css('background', '#fdd835');
    } else {
        $(".login__box-button").css('background', '#e2e2e2');
    }
});
$("#email").keyup(function () {
    if ($(this).val() != null && $(this).val() != "") {
        $(".login__box-button").css('background', '#fdd835');
    } else {
        $(".login__box-button").css('background', '#e2e2e2');
    }
});

function SignUp_button() {
    if ($("#lname").val() != null && $("#fname").val() != null && $("#lname").val() != "" && $("#fname").val() != "") {
        $(".login__box-button").css('background', '#fdd835');
    } else {
        $(".login__box-button").css('background', '#e2e2e2');
    }
}
var mobileCurrent
var login_in_progress = false;
var checked_mobile_current = false;
function login(step) {
    debugger
    if (login_in_progress) {
        debugger;
        return;
    }
    if (!check_login_mobile())
        return;
    step = step !== "email" ? "mobile" : "email";
    toggle_login_container(false);
    var email = $('#email').val();
    var number = intl.getNumber();
    var mobile = number.replace("+", "00");
    login_in_progress = true;
    myajax("user/popuplogin", "mobile=" + mobile + "&email=" + email +
        "&step=" + step + "&send_verification=" + can_send_login_message,
        function (ret) {
            debugger;
            login_in_progress = false;
            if (ret.status == 0) {
                $("#login-error-container").html(ret.msg);
            }
            else if (ret.status == 1) {
                debugger;
                checked_mobile_current = true;
                mobileCurrent = function () {
                    debugger;
                    $(".input-code").show();
                    $("#resend_form").show();
                    $("#check-number").show();
                    $(".button-get-number").css('display', 'none');
                    $(".iti.iti--allow-dropdown").css('display', 'none');
                    $(".login__box-button").css('background', '#e2e2e2');
                    $(".icon-back").css('display', 'block');
                    $("#login_form").find('.login__please-enter-mobile').html("");
                    var mobileNumber = mobile.replace("00989", "09");
                    $("#login_form").find('.login__please-enter-mobile').append(`<div class="login__please-enter-mobile">کد تایید به شماره موبایل ${mobileNumber} ارسال شد. </div><div>برای ورود کد تایید را وارد نمایید.</div>`);
                    //$(".iti").css('margin', '15px auto 0 auto')
                    //$("#mobile").prop("disabled", true).css({ 'cursor': 'no-drop', 'opacity': '0.3' });
                    //$(".iti__flag-container").prop("disabled", true).css({ 'cursor': 'no-drop', 'opacity': '0.3' });
                    $("div#selectRoot").css('margin-top', '-23px');
                    $("#login_form .login__please-enter-mobile").css('margin', '35px auto 8px auto');
                    //$("#mobile_label").html(mobileNumber);
                    if (can_send_login_message) {
                        debugger;
                        can_send_login_message = false;
                        startCountDown($("#count_down_timer")[0], function () {
                            can_send_login_message = true;
                            $("#count_down_timer").html("");
                            $("#resend_button").attr("onclick", "resend_login_sms()");
                            $("#resend_button").html("درخواست ارسال مجدد").css({ 'cursor': 'pointer', 'color': '#242424', 'font': '13px Miransans' });
                            $("#resend_button").click(function () {
                                $("#resend_button").css({ 'cursor': 'auto', 'color': '#ccc' });
                            })
                        });
                    }
                    $("#code").on('keypress', function (e) {
                        debugger;
                        if (e.which == 13) {
                            login_success(ret.fname, ret.lname, ret.mobile, ret.isNew);
                        }
                    });
                    $("#success__code").click(function () {
                        debugger;
                        login_success(ret.fname, ret.lname, ret.mobile, ret.isNew);
                    });
                    setTimeout(function () { $("#code").focus() }, 1000);
                }
                mobileCurrent();
                console.log(mobileCurrent);
            }
            else if (ret.status == 2) {
                toggle_login_container(false);
                showErrorMessage('مسدود',
                    'امکان ورود به سایت برای شما مسدود شده است, جهت فعالسازی با پشتیبانی تماس بگیرید: ' +
                    '<a href="/contact">تماس با پشتیبانی</a>');
            }
            else if (ret.status == 3) {
                login_success_email(ret.email);
            }
            else if (ret.status == 4) {
                $(".login_form").hide();
                $("#login_form_email").show();
                $(".login__box-button").css('background', '#e2e2e2');
                setTimeout(function () { $("#email").focus() }, 1300);
            }
            toggle_login_container(true, function () {
                $(".login__container").css("display", "flex");
                if (ret.status == 0) {
                    $('#login-error-container').show();
                }
            });
        });
}
function login_verification() {
    if (login_in_progress) {
        return;
    }
    var code = $('#code').val();
    var number = intl.getNumber();
    var mobile = number.replace("+", "00");
    var fname = $("#fname").val();
    var lname = $("#lname").val();
    var presentorCode = $('#presentorcode').val();
    login_in_progress = true;
    myajax("user/popuploginverification", "mobile=" + mobile + "&code=" +
        code + "&fname=" + fname + "&lname=" + lname +
        "&presentorcode=" + presentorCode,
        function (ret) {
            login_in_progress = false;
            if (ret.status == 1) {
                toggle_login();
                verification_success();
            }
            else {
                alertify.error(ret.msg);
            }
        });
}

function resend_login_sms() {
    if (login_in_progress) {
        return;
    }
    login_in_progress = true;
    var number = intl.getNumber();
    var mobile = number.replace("+", "00");
    myajax("user/popupsendsmsagain", "mobile=" + mobile, function (ret) {
        login_in_progress = false;
        if (ret.status == 1) {
            $("#resend_button").removeAttr("onclick");
            $("#resend_button").append('<span id="count_down_timer"></span>');
            if (can_send_login_message) {
                can_send_login_message = false;
                startCountDown($("#count_down_timer")[0], function () {
                    can_send_login_message = true;
                    $("#count_down_timer").html("");
                    $("#resend_button").attr("onclick", "resend_login_sms()");
                    $("#resend_button").html("درخواست ارسال مجدد ").css({ 'cursor': 'pointer', 'color': '#242424' });
                });
            }
        }
    });
}
$(".icon-back").click(function () {
    can_send_login_message = true;
    $(".input-code").hide();
    $("#check-number").hide();
    $(".icon-back").css('display', 'none');
    $(".button-get-number").css('display', 'block');
    $(".button-get-number p").css('background', '#fdd835');
    $(".iti.iti--allow-dropdown").css('display', 'block');
    $("div#selectRoot").css('margin-top', '0');
    $("#login_form").find('.login__please-enter-mobile').html("");
    $("#login_form").find('.login__please-enter-mobile').append('<div class="login__please-enter-mobile">برای ورود یا ثبت نام، شماره موبایل خود را وارد کنید</div>');
    //$("#mobile").prop("disabled", false).css({ 'cursor': 'pointer', 'opacity': '1' });
    //$(".iti__flag-container").prop("disabled", false).css({ 'cursor': 'pointer', 'opacity': '1' });
});
function resend_login_email(email) {
    if (login_in_progress) {
        return;
    }
    login_in_progress = true;
    myajax("user/popupresendemail", "email=" + email, function (ret) {
        login_in_progress = false;
        showSuccessMessage('ایمیل ارسال شد', 'ایمیل تایید دوباره ارسال شد. لطفا مجددا ایمیل خود را بررسی کنید',
            {
                onClose: function () {
                    $("#email_resend_button").removeAttr("onclick");
                    $("#email_resend_button").append('<span id="email_count_down_timer"></span>');
                    if (can_send_login_message) {
                        can_send_login_message = false;
                        startCountDown($("#email_count_down_timer")[0], function () {
                            can_send_login_message = true;
                            $("#email_count_down_timer").html("");
                            $("#email_resend_button").attr("onclick", "resend_login_email('" + email + "')");
                            $("#email_resend_button").html("دریافت مجدد ایمیل فعال سازی");
                            $("div#resend_form_email").css('background', '#FDD835');
                            $("div#resend_form_email .login__resend-button").css('color', '#242424');
                            $("#resend_form_email").click(function () {
                                $("div#resend_form_email").css('background', '#f3f3f3');
                                $("div#resend_form_email .login__resend-button").css('color', '#ccc');
                            });
                        });
                    }
                }
            });
    });
}
function restart_login() {
    $(".login_form").hide();
    $("#login_form").show();

}
function login_success(fname, lname, mobile, isNew) {
    //check_currentCode(fname, lname, mobile, isNew);
    //show_verification_form(fname, lname, mobile, undefined, isNew);
    $("#fname").val(fname == null ? "" : fname);
    $("#lname").val(lname == null ? "" : lname);
    var code = $('#code').val();
    var number = intl.getNumber();
    var mobile = number.replace("+", "00");
    myajax("user/popupverifycode", "mobile=" + mobile + "&code=" + code, function (ret) {
        if (ret.correct) {
            if (fname != null && fname != "" && lname != null && lname != "") {
                login_verification();
            }
            else {
                $(".login_form").hide();
                show_verification_form(fname, lname, mobile, undefined, isNew);
            }
        } else {
            alertify.error('کد وارد شده اشتباه است');
        }
    });
}

function login_success_email(email, onDone) {
    show_email_verification_form(email, onDone);
}

function show_verification_form(fname, lname, mobile, onDone, isNew) {
    if (isNew == undefined) {
        isNew = false;
    }
    $(".login__box-button").css('background', '#e2e2e2');
    //checked_mobile_current = true;
    //$("#login_form").css("display", "none");
    //$("#verification_form").css("display", "unset");
    $(".icon-back").css('display', 'none');
    $(".input-code").hide();
    $("#verification_form").show();
    $("#resend_form").hide();
    $("#check-number").hide()
    if (isNew) {
        $('#js-presentor-code-container').show();
    }
    if (onDone != undefined && onDone != null) {
        onDone();
    }
}

function show_email_verification_form(email, onDone) {
    checked_mobile_current = true;
    //$("#login_form").css("display", "none");
    //$("#verification_form").css("display", "unset");
    $("#login_form_email").hide();
    $("#email_verification_form").show();
    $("#resend_form_email").show();
    $("#email_label").html(email).css("color", "#f00");
    if (can_send_login_message) {
        can_send_login_message = false;
        startCountDown($("#email_count_down_timer")[0], function () {
            can_send_login_message = true;
            $("#email_count_down_timer").html("");
            $("#email_resend_button").attr("onclick", "resend_login_email('" + email + "')");
            $("#email_resend_button").html("دریافت مجدد ایمیل فعال سازی");
            $("div#resend_form_email").css('background', '#FDD835');
            $("div#resend_form_email .login__resend-button").css('color', '#242424');
            $("#resend_form_email").click(function () {
                $("div#resend_form_email").css('background', '#f3f3f3');
                $("div#resend_form_email .login__resend-button").css('color', '#ccc');
            });
        });
    }
    if (onDone != undefined && onDone != null) {
        onDone();
    }
}
$("#edit_mail_button").click(function () {
    $("#email_verification_form").hide();
    $("#resend_form_email").hide();
    $(".login_form").hide();
    $("#login_form_email").show();
    $("#email").focus;
})
function verification_success() {
    alertify.success("با موفقیت وارد حساب خود شدید");
    $(".master_header-account").attr("href", "/dashboard");
    $(".master_header-account").removeAttr("onclick");
    $(".master_header-account").find("span").html("حساب من");
    $('.login__container').hide();
    $('.login__bg').hide();
    onLoginFinish();
    if (typeof on_login !== "undefined") {
        on_login();
    }
}

if ($("#MobileLoginVerification").length) {
    alertify.success("کد فعال سازی 4 رقمی به تلفن همراه شما پیامک شد");
}

function onMobileKeyPress(e) {
    if (e.which == 32)
        return false;
}
function check_login_mobile() {
    var number = intl.getNumber();
    console.log('number coming from intl: ' + number);
    var mobile = number.replace("+", "00");
    if (mobile == "") {
        alertify.error("لطفا تلفن همراه خود را وارد کنید");
        $("#mobile").focus();
        return false;
    }
    if (!validateMobile(mobile.toString())) {

        alertify.error("لطفا تلفن همراه خود را درست وارد نمایید");
        $("#mobile").focus();
        return false;
    }
    return true;
}

setTimeout(function () {
    $("#resendCode").css("color", "black");
    $("#resendCode").css("cursor", "pointer");
}, 1 * 5 * 1000);

function startCountDown(elem, onDone) {
    var minutes = 2;
    var seconds = 0;
    var x = setInterval(function () {
        if (minutes < 0) {
            clearInterval(x);
            if (onDone != undefined) {
                onDone();
            }
        }
        var formattedMinutes = ("0" + minutes).slice(-2);
        var formattedSeconds = ("0" + seconds).slice(-2);
        $(elem).html(formattedMinutes + ":" + formattedSeconds);
        seconds -= 1;
        if (seconds < 0) {
            seconds = 59;
            minutes -= 1;
        }
    }, 1000);
}
$(function () {
    $('#mobile').keyup(function (e) {
        var ctrlKey = 67, vKey = 86;
        if (e.keyCode != ctrlKey && e.keyCode != vKey) {
            $('#mobile').val(persianToEnglish($(this).val()));
        }
    });
});
function persianToEnglish(input) {
    var inputstring = input;
    var persian = ["۰", "۱", "۲", "۳", "۴", "۵", "۶", "۷", "۸", "۹"]
    var english = ["0", "1", "2", "3", "4", "5", "6", "7", "8", "9"]
    for (var i = 0; i < 10; i++) {
        inputstring = inputstring.toString().replace(persian[i], english[i]);
    }

    return inputstring;
}

function onLoginStart() {
    msgPopupKeyPressEnabled = false;
}

function onLoginFinish() {
    msgPopupKeyPressEnabled = true;
}
