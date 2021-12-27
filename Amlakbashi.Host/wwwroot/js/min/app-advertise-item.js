/** Abstract base class for collection plugins v1.0.1.
	Written by Keith Wood (kbwood{at}iinet.com.au) December 2013.
	Licensed under the MIT (https://github.com/jquery/jquery/blob/master/MIT-LICENSE.txt) license. */
(function(){var j=false;window.JQClass=function(){};JQClass.classes={};JQClass.extend=function extender(f){var g=this.prototype;j=true;var h=new this();j=false;for(var i in f){h[i]=typeof f[i]=='function'&&typeof g[i]=='function'?(function(d,e){return function(){var b=this._super;this._super=function(a){return g[d].apply(this,a||[])};var c=e.apply(this,arguments);this._super=b;return c}})(i,f[i]):f[i]}function JQClass(){if(!j&&this._init){this._init.apply(this,arguments)}}JQClass.prototype=h;JQClass.prototype.constructor=JQClass;JQClass.extend=extender;return JQClass}})();(function($){JQClass.classes.JQPlugin=JQClass.extend({name:'plugin',defaultOptions:{},regionalOptions:{},_getters:[],_getMarker:function(){return'is-'+this.name},_init:function(){$.extend(this.defaultOptions,(this.regionalOptions&&this.regionalOptions[''])||{});var c=camelCase(this.name);$[c]=this;$.fn[c]=function(a){var b=Array.prototype.slice.call(arguments,1);if($[c]._isNotChained(a,b)){return $[c][a].apply($[c],[this[0]].concat(b))}return this.each(function(){if(typeof a==='string'){if(a[0]==='_'||!$[c][a]){throw'Unknown method: '+a;}$[c][a].apply($[c],[this].concat(b))}else{$[c]._attach(this,a)}})}},setDefaults:function(a){$.extend(this.defaultOptions,a||{})},_isNotChained:function(a,b){if(a==='option'&&(b.length===0||(b.length===1&&typeof b[0]==='string'))){return true}return $.inArray(a,this._getters)>-1},_attach:function(a,b){a=$(a);if(a.hasClass(this._getMarker())){return}a.addClass(this._getMarker());b=$.extend({},this.defaultOptions,this._getMetadata(a),b||{});var c=$.extend({name:this.name,elem:a,options:b},this._instSettings(a,b));a.data(this.name,c);this._postAttach(a,c);this.option(a,b)},_instSettings:function(a,b){return{}},_postAttach:function(a,b){},_getMetadata:function(d){try{var f=d.data(this.name.toLowerCase())||'';f=f.replace(/'/g,'"');f=f.replace(/([a-zA-Z0-9]+):/g,function(a,b,i){var c=f.substring(0,i).match(/"/g);return(!c||c.length%2===0?'"'+b+'":':b+':')});f=$.parseJSON('{'+f+'}');for(var g in f){var h=f[g];if(typeof h==='string'&&h.match(/^new Date\((.*)\)$/)){f[g]=eval(h)}}return f}catch(e){return{}}},_getInst:function(a){return $(a).data(this.name)||{}},option:function(a,b,c){a=$(a);var d=a.data(this.name);if(!b||(typeof b==='string'&&c==null)){var e=(d||{}).options;return(e&&b?e[b]:e)}if(!a.hasClass(this._getMarker())){return}var e=b||{};if(typeof b==='string'){e={};e[b]=c}this._optionsChanged(a,d,e);$.extend(d.options,e)},_optionsChanged:function(a,b,c){},destroy:function(a){a=$(a);if(!a.hasClass(this._getMarker())){return}this._preDestroy(a,this._getInst(a));a.removeData(this.name).removeClass(this._getMarker())},_preDestroy:function(a,b){}});function camelCase(c){return c.replace(/-([a-z])/g,function(a,b){return b.toUpperCase()})}$.JQPlugin={createPlugin:function(a,b){if(typeof a==='object'){b=a;a='JQPlugin'}a=camelCase(a);var c=camelCase(b.name);JQClass.classes[c]=JQClass.classes[a].extend(b);new JQClass.classes[c]()}}})(jQuery);
/*
     _ _      _       _
 ___| (_) ___| | __  (_)___
/ __| | |/ __| |/ /  | / __|
\__ \ | | (__|   < _ | \__ \
|___/_|_|\___|_|\_(_)/ |___/
                   |__/

 Version: 1.8.0
  Author: Ken Wheeler
 Website: http://kenwheeler.github.io
    Docs: http://kenwheeler.github.io/slick
    Repo: http://github.com/kenwheeler/slick
  Issues: http://github.com/kenwheeler/slick/issues

 */
/* global window, document, define, jQuery, setInterval, clearInterval */
;(function(factory) {
    'use strict';
    if (typeof define === 'function' && define.amd) {
        define(['jquery'], factory);
    } else if (typeof exports !== 'undefined') {
        module.exports = factory(require('jquery'));
    } else {
        factory(jQuery);
    }

}(function($) {
    'use strict';
    var Slick = window.Slick || {};

    Slick = (function() {

        var instanceUid = 0;

        function Slick(element, settings) {

            var _ = this, dataSettings;

            _.defaults = {
                accessibility: true,
                adaptiveHeight: false,
                appendArrows: $(element),
                appendDots: $(element),
                arrows: true,
                asNavFor: null,
                prevArrow: '<button class="slick-prev" aria-label="Previous" type="button"><i class="fa fa-arrow-circle-left"></i></button>',
                nextArrow: '<button class="slick-next" aria-label="Next" type="button"><i class="fa fa-arrow-circle-right"></i></button>',
                autoplay: false,
                autoplaySpeed: 3000,
                centerMode: false,
                centerPadding: '50px',
                cssEase: 'ease',
                customPaging: function(slider, i) {
                    return $('<button type="button" />').text(i + 1);
                },
                dots: false,
                dotsClass: 'slick-dots',
                draggable: true,
                easing: 'linear',
                edgeFriction: 0.35,
                fade: false,
                focusOnSelect: false,
                focusOnChange: false,
                infinite: true,
                initialSlide: 0,
                lazyLoad: 'ondemand',
                mobileFirst: false,
                pauseOnHover: true,
                pauseOnFocus: true,
                pauseOnDotsHover: false,
                respondTo: 'window',
                responsive: null,
                rows: 1,
                rtl: false,
                slide: '',
                slidesPerRow: 1,
                slidesToShow: 1,
                slidesToScroll: 1,
                speed: 500,
                swipe: true,
                swipeToSlide: false,
                touchMove: true,
                touchThreshold: 5,
                useCSS: true,
                useTransform: true,
                variableWidth: false,
                vertical: false,
                verticalSwiping: false,
                waitForAnimate: true,
                zIndex: 1000
            };

            _.initials = {
                animating: false,
                dragging: false,
                autoPlayTimer: null,
                currentDirection: 0,
                currentLeft: null,
                currentSlide: 0,
                direction: 1,
                $dots: null,
                listWidth: null,
                listHeight: null,
                loadIndex: 0,
                $nextArrow: null,
                $prevArrow: null,
                scrolling: false,
                slideCount: null,
                slideWidth: null,
                $slideTrack: null,
                $slides: null,
                sliding: false,
                slideOffset: 0,
                swipeLeft: null,
                swiping: false,
                $list: null,
                touchObject: {},
                transformsEnabled: false,
                unslicked: false
            };

            $.extend(_, _.initials);

            _.activeBreakpoint = null;
            _.animType = null;
            _.animProp = null;
            _.breakpoints = [];
            _.breakpointSettings = [];
            _.cssTransitions = false;
            _.focussed = false;
            _.interrupted = false;
            _.hidden = 'hidden';
            _.paused = true;
            _.positionProp = null;
            _.respondTo = null;
            _.rowCount = 1;
            _.shouldClick = true;
            _.$slider = $(element);
            _.$slidesCache = null;
            _.transformType = null;
            _.transitionType = null;
            _.visibilityChange = 'visibilitychange';
            _.windowWidth = 0;
            _.windowTimer = null;

            dataSettings = $(element).data('slick') || {};

            _.options = $.extend({}, _.defaults, settings, dataSettings);

            _.currentSlide = _.options.initialSlide;

            _.originalSettings = _.options;

            if (typeof document.mozHidden !== 'undefined') {
                _.hidden = 'mozHidden';
                _.visibilityChange = 'mozvisibilitychange';
            } else if (typeof document.webkitHidden !== 'undefined') {
                _.hidden = 'webkitHidden';
                _.visibilityChange = 'webkitvisibilitychange';
            }

            _.autoPlay = $.proxy(_.autoPlay, _);
            _.autoPlayClear = $.proxy(_.autoPlayClear, _);
            _.autoPlayIterator = $.proxy(_.autoPlayIterator, _);
            _.changeSlide = $.proxy(_.changeSlide, _);
            _.clickHandler = $.proxy(_.clickHandler, _);
            _.selectHandler = $.proxy(_.selectHandler, _);
            _.setPosition = $.proxy(_.setPosition, _);
            _.swipeHandler = $.proxy(_.swipeHandler, _);
            _.dragHandler = $.proxy(_.dragHandler, _);
            _.keyHandler = $.proxy(_.keyHandler, _);

            _.instanceUid = instanceUid++;

            // A simple way to check for HTML strings
            // Strict HTML recognition (must start with <)
            // Extracted from jQuery v1.11 source
            _.htmlExpr = /^(?:\s*(<[\w\W]+>)[^>]*)$/;


            _.registerBreakpoints();
            _.init(true);

        }

        return Slick;

    }());

    Slick.prototype.activateADA = function() {
        var _ = this;

        _.$slideTrack.find('.slick-active').attr({
            'aria-hidden': 'false'
        }).find('a, input, button, select').attr({
            'tabindex': '0'
        });

    };

    Slick.prototype.addSlide = Slick.prototype.slickAdd = function(markup, index, addBefore) {

        var _ = this;

        if (typeof(index) === 'boolean') {
            addBefore = index;
            index = null;
        } else if (index < 0 || (index >= _.slideCount)) {
            return false;
        }

        _.unload();

        if (typeof(index) === 'number') {
            if (index === 0 && _.$slides.length === 0) {
                $(markup).appendTo(_.$slideTrack);
            } else if (addBefore) {
                $(markup).insertBefore(_.$slides.eq(index));
            } else {
                $(markup).insertAfter(_.$slides.eq(index));
            }
        } else {
            if (addBefore === true) {
                $(markup).prependTo(_.$slideTrack);
            } else {
                $(markup).appendTo(_.$slideTrack);
            }
        }

        _.$slides = _.$slideTrack.children(this.options.slide);

        _.$slideTrack.children(this.options.slide).detach();

        _.$slideTrack.append(_.$slides);

        _.$slides.each(function(index, element) {
            $(element).attr('data-slick-index', index);
        });

        _.$slidesCache = _.$slides;

        _.reinit();

    };

    Slick.prototype.animateHeight = function() {
        var _ = this;
        if (_.options.slidesToShow === 1 && _.options.adaptiveHeight === true && _.options.vertical === false) {
            var targetHeight = _.$slides.eq(_.currentSlide).outerHeight(true);
            _.$list.animate({
                height: targetHeight
            }, _.options.speed);
        }
    };

    Slick.prototype.animateSlide = function(targetLeft, callback) {

        var animProps = {},
            _ = this;

        _.animateHeight();

        if (_.options.rtl === true && _.options.vertical === false) {
            targetLeft = -targetLeft;
        }
        if (_.transformsEnabled === false) {
            if (_.options.vertical === false) {
                _.$slideTrack.animate({
                    left: targetLeft
                }, _.options.speed, _.options.easing, callback);
            } else {
                _.$slideTrack.animate({
                    top: targetLeft
                }, _.options.speed, _.options.easing, callback);
            }

        } else {

            if (_.cssTransitions === false) {
                if (_.options.rtl === true) {
                    _.currentLeft = -(_.currentLeft);
                }
                $({
                    animStart: _.currentLeft
                }).animate({
                    animStart: targetLeft
                }, {
                    duration: _.options.speed,
                    easing: _.options.easing,
                    step: function(now) {
                        now = Math.ceil(now);
                        if (_.options.vertical === false) {
                            animProps[_.animType] = 'translate(' +
                                now + 'px, 0px)';
                            _.$slideTrack.css(animProps);
                        } else {
                            animProps[_.animType] = 'translate(0px,' +
                                now + 'px)';
                            _.$slideTrack.css(animProps);
                        }
                    },
                    complete: function() {
                        if (callback) {
                            callback.call();
                        }
                    }
                });

            } else {

                _.applyTransition();
                targetLeft = Math.ceil(targetLeft);

                if (_.options.vertical === false) {
                    animProps[_.animType] = 'translate3d(' + targetLeft + 'px, 0px, 0px)';
                } else {
                    animProps[_.animType] = 'translate3d(0px,' + targetLeft + 'px, 0px)';
                }
                _.$slideTrack.css(animProps);

                if (callback) {
                    setTimeout(function() {

                        _.disableTransition();

                        callback.call();
                    }, _.options.speed);
                }

            }

        }

    };

    Slick.prototype.getNavTarget = function() {

        var _ = this,
            asNavFor = _.options.asNavFor;

        if ( asNavFor && asNavFor !== null ) {
            asNavFor = $(asNavFor).not(_.$slider);
        }

        return asNavFor;

    };

    Slick.prototype.asNavFor = function(index) {

        var _ = this,
            asNavFor = _.getNavTarget();

        if ( asNavFor !== null && typeof asNavFor === 'object' ) {
            asNavFor.each(function() {
                var target = $(this).slick('getSlick');
                if(!target.unslicked) {
                    target.slideHandler(index, true);
                }
            });
        }

    };

    Slick.prototype.applyTransition = function(slide) {

        var _ = this,
            transition = {};

        if (_.options.fade === false) {
            transition[_.transitionType] = _.transformType + ' ' + _.options.speed + 'ms ' + _.options.cssEase;
        } else {
            transition[_.transitionType] = 'opacity ' + _.options.speed + 'ms ' + _.options.cssEase;
        }

        if (_.options.fade === false) {
            _.$slideTrack.css(transition);
        } else {
            _.$slides.eq(slide).css(transition);
        }

    };

    Slick.prototype.autoPlay = function() {

        var _ = this;

        _.autoPlayClear();

        if ( _.slideCount > _.options.slidesToShow ) {
            _.autoPlayTimer = setInterval( _.autoPlayIterator, _.options.autoplaySpeed );
        }

    };

    Slick.prototype.autoPlayClear = function() {

        var _ = this;

        if (_.autoPlayTimer) {
            clearInterval(_.autoPlayTimer);
        }

    };

    Slick.prototype.autoPlayIterator = function() {

        var _ = this,
            slideTo = _.currentSlide + _.options.slidesToScroll;

        if ( !_.paused && !_.interrupted && !_.focussed ) {

            if ( _.options.infinite === false ) {

                if ( _.direction === 1 && ( _.currentSlide + 1 ) === ( _.slideCount - 1 )) {
                    _.direction = 0;
                }

                else if ( _.direction === 0 ) {

                    slideTo = _.currentSlide - _.options.slidesToScroll;

                    if ( _.currentSlide - 1 === 0 ) {
                        _.direction = 1;
                    }

                }

            }

            _.slideHandler( slideTo );

        }

    };

    Slick.prototype.buildArrows = function() {

        var _ = this;

        if (_.options.arrows === true ) {

            _.$prevArrow = $(_.options.prevArrow).addClass('slick-arrow');
            _.$nextArrow = $(_.options.nextArrow).addClass('slick-arrow');

            if( _.slideCount > _.options.slidesToShow ) {

                _.$prevArrow.removeClass('slick-hidden').removeAttr('aria-hidden tabindex');
                _.$nextArrow.removeClass('slick-hidden').removeAttr('aria-hidden tabindex');

                if (_.htmlExpr.test(_.options.prevArrow)) {
                    _.$prevArrow.prependTo(_.options.appendArrows);
                }

                if (_.htmlExpr.test(_.options.nextArrow)) {
                    _.$nextArrow.appendTo(_.options.appendArrows);
                }

                if (_.options.infinite !== true) {
                    _.$prevArrow
                        .addClass('slick-disabled')
                        .attr('aria-disabled', 'true');
                }

            } else {

                _.$prevArrow.add( _.$nextArrow )

                    .addClass('slick-hidden')
                    .attr({
                        'aria-disabled': 'true',
                        'tabindex': '-1'
                    });

            }

        }

    };

    Slick.prototype.buildDots = function() {

        var _ = this,
            i, dot;

        if (_.options.dots === true && _.slideCount > _.options.slidesToShow) {

            _.$slider.addClass('slick-dotted');

            dot = $('<ul />').addClass(_.options.dotsClass);

            for (i = 0; i <= _.getDotCount(); i += 1) {
                dot.append($('<li />').append(_.options.customPaging.call(this, _, i)));
            }

            _.$dots = dot.appendTo(_.options.appendDots);

            _.$dots.find('li').first().addClass('slick-active');

        }

    };

    Slick.prototype.buildOut = function() {

        var _ = this;

        _.$slides =
            _.$slider
                .children( _.options.slide + ':not(.slick-cloned)')
                .addClass('slick-slide');

        _.slideCount = _.$slides.length;

        _.$slides.each(function(index, element) {
            $(element)
                .attr('data-slick-index', index)
                .data('originalStyling', $(element).attr('style') || '');
        });

        _.$slider.addClass('slick-slider');

        _.$slideTrack = (_.slideCount === 0) ?
            $('<div class="slick-track"/>').appendTo(_.$slider) :
            _.$slides.wrapAll('<div class="slick-track"/>').parent();

        _.$list = _.$slideTrack.wrap(
            '<div class="slick-list"/>').parent();
        _.$slideTrack.css('opacity', 0);

        if (_.options.centerMode === true || _.options.swipeToSlide === true) {
            _.options.slidesToScroll = 1;
        }

        $('img[data-lazy]', _.$slider).not('[src]').addClass('slick-loading');

        _.setupInfinite();

        _.buildArrows();

        _.buildDots();

        _.updateDots();


        _.setSlideClasses(typeof _.currentSlide === 'number' ? _.currentSlide : 0);

        if (_.options.draggable === true) {
            _.$list.addClass('draggable');
        }

    };

    Slick.prototype.buildRows = function() {

        var _ = this, a, b, c, newSlides, numOfSlides, originalSlides,slidesPerSection;

        newSlides = document.createDocumentFragment();
        originalSlides = _.$slider.children();

        if(_.options.rows > 0) {

            slidesPerSection = _.options.slidesPerRow * _.options.rows;
            numOfSlides = Math.ceil(
                originalSlides.length / slidesPerSection
            );

            for(a = 0; a < numOfSlides; a++){
                var slide = document.createElement('div');
                for(b = 0; b < _.options.rows; b++) {
                    var row = document.createElement('div');
                    for(c = 0; c < _.options.slidesPerRow; c++) {
                        var target = (a * slidesPerSection + ((b * _.options.slidesPerRow) + c));
                        if (originalSlides.get(target)) {
                            row.appendChild(originalSlides.get(target));
                        }
                    }
                    slide.appendChild(row);
                }
                newSlides.appendChild(slide);
            }

            _.$slider.empty().append(newSlides);
            _.$slider.children().children().children()
                .css({
                    'width':(100 / _.options.slidesPerRow) + '%',
                    'display': 'inline-block'
                });

        }

    };

    Slick.prototype.checkResponsive = function(initial, forceUpdate) {

        var _ = this,
            breakpoint, targetBreakpoint, respondToWidth, triggerBreakpoint = false;
        var sliderWidth = _.$slider.width();
        var windowWidth = window.innerWidth || $(window).width();

        if (_.respondTo === 'window') {
            respondToWidth = windowWidth;
        } else if (_.respondTo === 'slider') {
            respondToWidth = sliderWidth;
        } else if (_.respondTo === 'min') {
            respondToWidth = Math.min(windowWidth, sliderWidth);
        }

        if ( _.options.responsive &&
            _.options.responsive.length &&
            _.options.responsive !== null) {

            targetBreakpoint = null;

            for (breakpoint in _.breakpoints) {
                if (_.breakpoints.hasOwnProperty(breakpoint)) {
                    if (_.originalSettings.mobileFirst === false) {
                        if (respondToWidth < _.breakpoints[breakpoint]) {
                            targetBreakpoint = _.breakpoints[breakpoint];
                        }
                    } else {
                        if (respondToWidth > _.breakpoints[breakpoint]) {
                            targetBreakpoint = _.breakpoints[breakpoint];
                        }
                    }
                }
            }

            if (targetBreakpoint !== null) {
                if (_.activeBreakpoint !== null) {
                    if (targetBreakpoint !== _.activeBreakpoint || forceUpdate) {
                        _.activeBreakpoint =
                            targetBreakpoint;
                        if (_.breakpointSettings[targetBreakpoint] === 'unslick') {
                            _.unslick(targetBreakpoint);
                        } else {
                            _.options = $.extend({}, _.originalSettings,
                                _.breakpointSettings[
                                    targetBreakpoint]);
                            if (initial === true) {
                                _.currentSlide = _.options.initialSlide;
                            }
                            _.refresh(initial);
                        }
                        triggerBreakpoint = targetBreakpoint;
                    }
                } else {
                    _.activeBreakpoint = targetBreakpoint;
                    if (_.breakpointSettings[targetBreakpoint] === 'unslick') {
                        _.unslick(targetBreakpoint);
                    } else {
                        _.options = $.extend({}, _.originalSettings,
                            _.breakpointSettings[
                                targetBreakpoint]);
                        if (initial === true) {
                            _.currentSlide = _.options.initialSlide;
                        }
                        _.refresh(initial);
                    }
                    triggerBreakpoint = targetBreakpoint;
                }
            } else {
                if (_.activeBreakpoint !== null) {
                    _.activeBreakpoint = null;
                    _.options = _.originalSettings;
                    if (initial === true) {
                        _.currentSlide = _.options.initialSlide;
                    }
                    _.refresh(initial);
                    triggerBreakpoint = targetBreakpoint;
                }
            }

            // only trigger breakpoints during an actual break. not on initialize.
            if( !initial && triggerBreakpoint !== false ) {
                _.$slider.trigger('breakpoint', [_, triggerBreakpoint]);
            }
        }

    };

    Slick.prototype.changeSlide = function(event, dontAnimate) {

        var _ = this,
            $target = $(event.currentTarget),
            indexOffset, slideOffset, unevenOffset;

        // If target is a link, prevent default action.
        if($target.is('a')) {
            event.preventDefault();
        }

        // If target is not the <li> element (ie: a child), find the <li>.
        if(!$target.is('li')) {
            $target = $target.closest('li');
        }

        unevenOffset = (_.slideCount % _.options.slidesToScroll !== 0);
        indexOffset = unevenOffset ? 0 : (_.slideCount - _.currentSlide) % _.options.slidesToScroll;

        switch (event.data.message) {

            case 'previous':
                slideOffset = indexOffset === 0 ? _.options.slidesToScroll : _.options.slidesToShow - indexOffset;
                if (_.slideCount > _.options.slidesToShow) {
                    _.slideHandler(_.currentSlide - slideOffset, false, dontAnimate);
                }
                break;

            case 'next':
                slideOffset = indexOffset === 0 ? _.options.slidesToScroll : indexOffset;
                if (_.slideCount > _.options.slidesToShow) {
                    _.slideHandler(_.currentSlide + slideOffset, false, dontAnimate);
                }
                break;

            case 'index':
                var index = event.data.index === 0 ? 0 :
                    event.data.index || $target.index() * _.options.slidesToScroll;

                _.slideHandler(_.checkNavigable(index), false, dontAnimate);
                $target.children().trigger('focus');
                break;

            default:
                return;
        }

    };

    Slick.prototype.checkNavigable = function(index) {

        var _ = this,
            navigables, prevNavigable;

        navigables = _.getNavigableIndexes();
        prevNavigable = 0;
        if (index > navigables[navigables.length - 1]) {
            index = navigables[navigables.length - 1];
        } else {
            for (var n in navigables) {
                if (index < navigables[n]) {
                    index = prevNavigable;
                    break;
                }
                prevNavigable = navigables[n];
            }
        }

        return index;
    };

    Slick.prototype.cleanUpEvents = function() {

        var _ = this;

        if (_.options.dots && _.$dots !== null) {

            $('li', _.$dots)
                .off('click.slick', _.changeSlide)
                .off('mouseenter.slick', $.proxy(_.interrupt, _, true))
                .off('mouseleave.slick', $.proxy(_.interrupt, _, false));

            if (_.options.accessibility === true) {
                _.$dots.off('keydown.slick', _.keyHandler);
            }
        }

        _.$slider.off('focus.slick blur.slick');

        if (_.options.arrows === true && _.slideCount > _.options.slidesToShow) {
            _.$prevArrow && _.$prevArrow.off('click.slick', _.changeSlide);
            _.$nextArrow && _.$nextArrow.off('click.slick', _.changeSlide);

            if (_.options.accessibility === true) {
                _.$prevArrow && _.$prevArrow.off('keydown.slick', _.keyHandler);
                _.$nextArrow && _.$nextArrow.off('keydown.slick', _.keyHandler);
            }
        }

        _.$list.off('touchstart.slick mousedown.slick', _.swipeHandler);
        _.$list.off('touchmove.slick mousemove.slick', _.swipeHandler);
        _.$list.off('touchend.slick mouseup.slick', _.swipeHandler);
        _.$list.off('touchcancel.slick mouseleave.slick', _.swipeHandler);

        _.$list.off('click.slick', _.clickHandler);

        $(document).off(_.visibilityChange, _.visibility);

        _.cleanUpSlideEvents();

        if (_.options.accessibility === true) {
            _.$list.off('keydown.slick', _.keyHandler);
        }

        if (_.options.focusOnSelect === true) {
            $(_.$slideTrack).children().off('click.slick', _.selectHandler);
        }

        $(window).off('orientationchange.slick.slick-' + _.instanceUid, _.orientationChange);

        $(window).off('resize.slick.slick-' + _.instanceUid, _.resize);

        $('[draggable!=true]', _.$slideTrack).off('dragstart', _.preventDefault);

        $(window).off('load.slick.slick-' + _.instanceUid, _.setPosition);

    };

    Slick.prototype.cleanUpSlideEvents = function() {

        var _ = this;

        _.$list.off('mouseenter.slick', $.proxy(_.interrupt, _, true));
        _.$list.off('mouseleave.slick', $.proxy(_.interrupt, _, false));

    };

    Slick.prototype.cleanUpRows = function() {

        var _ = this, originalSlides;

        if(_.options.rows > 0) {
            originalSlides = _.$slides.children().children();
            originalSlides.removeAttr('style');
            _.$slider.empty().append(originalSlides);
        }

    };

    Slick.prototype.clickHandler = function(event) {

        var _ = this;

        if (_.shouldClick === false) {
            event.stopImmediatePropagation();
            event.stopPropagation();
            event.preventDefault();
        }

    };

    Slick.prototype.destroy = function(refresh) {

        var _ = this;

        _.autoPlayClear();

        _.touchObject = {};

        _.cleanUpEvents();

        $('.slick-cloned', _.$slider).detach();

        if (_.$dots) {
            _.$dots.remove();
        }

        if ( _.$prevArrow && _.$prevArrow.length ) {

            _.$prevArrow
                .removeClass('slick-disabled slick-arrow slick-hidden')
                .removeAttr('aria-hidden aria-disabled tabindex')
                .css('display','');

            if ( _.htmlExpr.test( _.options.prevArrow )) {
                _.$prevArrow.remove();
            }
        }

        if ( _.$nextArrow && _.$nextArrow.length ) {

            _.$nextArrow
                .removeClass('slick-disabled slick-arrow slick-hidden')
                .removeAttr('aria-hidden aria-disabled tabindex')
                .css('display','');

            if ( _.htmlExpr.test( _.options.nextArrow )) {
                _.$nextArrow.remove();
            }
        }


        if (_.$slides) {

            _.$slides
                .removeClass('slick-slide slick-active slick-center slick-visible slick-current')
                .removeAttr('aria-hidden')
                .removeAttr('data-slick-index')
                .each(function(){
                    $(this).attr('style', $(this).data('originalStyling'));
                });

            _.$slideTrack.children(this.options.slide).detach();

            _.$slideTrack.detach();

            _.$list.detach();

            _.$slider.append(_.$slides);
        }

        _.cleanUpRows();

        _.$slider.removeClass('slick-slider');
        _.$slider.removeClass('slick-initialized');
        _.$slider.removeClass('slick-dotted');

        _.unslicked = true;

        if(!refresh) {
            _.$slider.trigger('destroy', [_]);
        }

    };

    Slick.prototype.disableTransition = function(slide) {

        var _ = this,
            transition = {};

        transition[_.transitionType] = '';

        if (_.options.fade === false) {
            _.$slideTrack.css(transition);
        } else {
            _.$slides.eq(slide).css(transition);
        }

    };

    Slick.prototype.fadeSlide = function(slideIndex, callback) {

        var _ = this;

        if (_.cssTransitions === false) {

            _.$slides.eq(slideIndex).css({
                zIndex: _.options.zIndex
            });

            _.$slides.eq(slideIndex).animate({
                opacity: 1
            }, _.options.speed, _.options.easing, callback);

        } else {

            _.applyTransition(slideIndex);

            _.$slides.eq(slideIndex).css({
                opacity: 1,
                zIndex: _.options.zIndex
            });

            if (callback) {
                setTimeout(function() {

                    _.disableTransition(slideIndex);

                    callback.call();
                }, _.options.speed);
            }

        }

    };

    Slick.prototype.fadeSlideOut = function(slideIndex) {

        var _ = this;

        if (_.cssTransitions === false) {

            _.$slides.eq(slideIndex).animate({
                opacity: 0,
                zIndex: _.options.zIndex - 2
            }, _.options.speed, _.options.easing);

        } else {

            _.applyTransition(slideIndex);

            _.$slides.eq(slideIndex).css({
                opacity: 0,
                zIndex: _.options.zIndex - 2
            });

        }

    };

    Slick.prototype.filterSlides = Slick.prototype.slickFilter = function(filter) {

        var _ = this;

        if (filter !== null) {

            _.$slidesCache = _.$slides;

            _.unload();

            _.$slideTrack.children(this.options.slide).detach();

            _.$slidesCache.filter(filter).appendTo(_.$slideTrack);

            _.reinit();

        }

    };

    Slick.prototype.focusHandler = function() {

        var _ = this;

        _.$slider
            .off('focus.slick blur.slick')
            .on('focus.slick blur.slick', '*', function(event) {

            event.stopImmediatePropagation();
            var $sf = $(this);

            setTimeout(function() {

                if( _.options.pauseOnFocus ) {
                    _.focussed = $sf.is(':focus');
                    _.autoPlay();
                }

            }, 0);

        });
    };

    Slick.prototype.getCurrent = Slick.prototype.slickCurrentSlide = function() {

        var _ = this;
        return _.currentSlide;

    };

    Slick.prototype.getDotCount = function() {

        var _ = this;

        var breakPoint = 0;
        var counter = 0;
        var pagerQty = 0;

        if (_.options.infinite === true) {
            if (_.slideCount <= _.options.slidesToShow) {
                 ++pagerQty;
            } else {
                while (breakPoint < _.slideCount) {
                    ++pagerQty;
                    breakPoint = counter + _.options.slidesToScroll;
                    counter += _.options.slidesToScroll <= _.options.slidesToShow ? _.options.slidesToScroll : _.options.slidesToShow;
                }
            }
        } else if (_.options.centerMode === true) {
            pagerQty = _.slideCount;
        } else if(!_.options.asNavFor) {
            pagerQty = 1 + Math.ceil((_.slideCount - _.options.slidesToShow) / _.options.slidesToScroll);
        }else {
            while (breakPoint < _.slideCount) {
                ++pagerQty;
                breakPoint = counter + _.options.slidesToScroll;
                counter += _.options.slidesToScroll <= _.options.slidesToShow ? _.options.slidesToScroll : _.options.slidesToShow;
            }
        }

        return pagerQty - 1;

    };

    Slick.prototype.getLeft = function(slideIndex) {

        var _ = this,
            targetLeft,
            verticalHeight,
            verticalOffset = 0,
            targetSlide,
            coef;

        _.slideOffset = 0;
        verticalHeight = _.$slides.first().outerHeight(true);

        if (_.options.infinite === true) {
            if (_.slideCount > _.options.slidesToShow) {
                _.slideOffset = (_.slideWidth * _.options.slidesToShow) * -1;
                coef = -1

                if (_.options.vertical === true && _.options.centerMode === true) {
                    if (_.options.slidesToShow === 2) {
                        coef = -1.5;
                    } else if (_.options.slidesToShow === 1) {
                        coef = -2
                    }
                }
                verticalOffset = (verticalHeight * _.options.slidesToShow) * coef;
            }
            if (_.slideCount % _.options.slidesToScroll !== 0) {
                if (slideIndex + _.options.slidesToScroll > _.slideCount && _.slideCount > _.options.slidesToShow) {
                    if (slideIndex > _.slideCount) {
                        _.slideOffset = ((_.options.slidesToShow - (slideIndex - _.slideCount)) * _.slideWidth) * -1;
                        verticalOffset = ((_.options.slidesToShow - (slideIndex - _.slideCount)) * verticalHeight) * -1;
                    } else {
                        _.slideOffset = ((_.slideCount % _.options.slidesToScroll) * _.slideWidth) * -1;
                        verticalOffset = ((_.slideCount % _.options.slidesToScroll) * verticalHeight) * -1;
                    }
                }
            }
        } else {
            if (slideIndex + _.options.slidesToShow > _.slideCount) {
                _.slideOffset = ((slideIndex + _.options.slidesToShow) - _.slideCount) * _.slideWidth;
                verticalOffset = ((slideIndex + _.options.slidesToShow) - _.slideCount) * verticalHeight;
            }
        }

        if (_.slideCount <= _.options.slidesToShow) {
            _.slideOffset = 0;
            verticalOffset = 0;
        }

        if (_.options.centerMode === true && _.slideCount <= _.options.slidesToShow) {
            _.slideOffset = ((_.slideWidth * Math.floor(_.options.slidesToShow)) / 2) - ((_.slideWidth * _.slideCount) / 2);
        } else if (_.options.centerMode === true && _.options.infinite === true) {
            _.slideOffset += _.slideWidth * Math.floor(_.options.slidesToShow / 2) - _.slideWidth;
        } else if (_.options.centerMode === true) {
            _.slideOffset = 0;
            _.slideOffset += _.slideWidth * Math.floor(_.options.slidesToShow / 2);
        }

        if (_.options.vertical === false) {
            targetLeft = ((slideIndex * _.slideWidth) * -1) + _.slideOffset;
        } else {
            targetLeft = ((slideIndex * verticalHeight) * -1) + verticalOffset;
        }

        if (_.options.variableWidth === true) {

            if (_.slideCount <= _.options.slidesToShow || _.options.infinite === false) {
                targetSlide = _.$slideTrack.children('.slick-slide').eq(slideIndex);
            } else {
                targetSlide = _.$slideTrack.children('.slick-slide').eq(slideIndex + _.options.slidesToShow);
            }

            if (_.options.rtl === true) {
                if (targetSlide[0]) {
                    targetLeft = (_.$slideTrack.width() - targetSlide[0].offsetLeft - targetSlide.width()) * -1;
                } else {
                    targetLeft =  0;
                }
            } else {
                targetLeft = targetSlide[0] ? targetSlide[0].offsetLeft * -1 : 0;
            }

            if (_.options.centerMode === true) {
                if (_.slideCount <= _.options.slidesToShow || _.options.infinite === false) {
                    targetSlide = _.$slideTrack.children('.slick-slide').eq(slideIndex);
                } else {
                    targetSlide = _.$slideTrack.children('.slick-slide').eq(slideIndex + _.options.slidesToShow + 1);
                }

                if (_.options.rtl === true) {
                    if (targetSlide[0]) {
                        targetLeft = (_.$slideTrack.width() - targetSlide[0].offsetLeft - targetSlide.width()) * -1;
                    } else {
                        targetLeft =  0;
                    }
                } else {
                    targetLeft = targetSlide[0] ? targetSlide[0].offsetLeft * -1 : 0;
                }

                targetLeft += (_.$list.width() - targetSlide.outerWidth()) / 2;
            }
        }

        return targetLeft;

    };

    Slick.prototype.getOption = Slick.prototype.slickGetOption = function(option) {

        var _ = this;

        return _.options[option];

    };

    Slick.prototype.getNavigableIndexes = function() {

        var _ = this,
            breakPoint = 0,
            counter = 0,
            indexes = [],
            max;

        if (_.options.infinite === false) {
            max = _.slideCount;
        } else {
            breakPoint = _.options.slidesToScroll * -1;
            counter = _.options.slidesToScroll * -1;
            max = _.slideCount * 2;
        }

        while (breakPoint < max) {
            indexes.push(breakPoint);
            breakPoint = counter + _.options.slidesToScroll;
            counter += _.options.slidesToScroll <= _.options.slidesToShow ? _.options.slidesToScroll : _.options.slidesToShow;
        }

        return indexes;

    };

    Slick.prototype.getSlick = function() {

        return this;

    };

    Slick.prototype.getSlideCount = function() {

        var _ = this,
            slidesTraversed, swipedSlide, centerOffset;

        centerOffset = _.options.centerMode === true ? _.slideWidth * Math.floor(_.options.slidesToShow / 2) : 0;

        if (_.options.swipeToSlide === true) {
            _.$slideTrack.find('.slick-slide').each(function(index, slide) {
                if (slide.offsetLeft - centerOffset + ($(slide).outerWidth() / 2) > (_.swipeLeft * -1)) {
                    swipedSlide = slide;
                    return false;
                }
            });

            slidesTraversed = Math.abs($(swipedSlide).attr('data-slick-index') - _.currentSlide) || 1;

            return slidesTraversed;

        } else {
            return _.options.slidesToScroll;
        }

    };

    Slick.prototype.goTo = Slick.prototype.slickGoTo = function(slide, dontAnimate) {

        var _ = this;

        _.changeSlide({
            data: {
                message: 'index',
                index: parseInt(slide)
            }
        }, dontAnimate);

    };

    Slick.prototype.init = function(creation) {

        var _ = this;

        if (!$(_.$slider).hasClass('slick-initialized')) {

            $(_.$slider).addClass('slick-initialized');

            _.buildRows();
            _.buildOut();
            _.setProps();
            _.startLoad();
            _.loadSlider();
            _.initializeEvents();
            _.updateArrows();
            _.updateDots();
            _.checkResponsive(true);
            _.focusHandler();

        }

        if (creation) {
            _.$slider.trigger('init', [_]);
        }

        if (_.options.accessibility === true) {
            _.initADA();
        }

        if ( _.options.autoplay ) {

            _.paused = false;
            _.autoPlay();

        }

    };

    Slick.prototype.initADA = function() {
        var _ = this,
                numDotGroups = Math.ceil(_.slideCount / _.options.slidesToShow),
                tabControlIndexes = _.getNavigableIndexes().filter(function(val) {
                    return (val >= 0) && (val < _.slideCount);
                });

        _.$slides.add(_.$slideTrack.find('.slick-cloned')).attr({
            'aria-hidden': 'true',
            'tabindex': '-1'
        }).find('a, input, button, select').attr({
            'tabindex': '-1'
        });

        if (_.$dots !== null) {
            _.$slides.not(_.$slideTrack.find('.slick-cloned')).each(function(i) {
                var slideControlIndex = tabControlIndexes.indexOf(i);

                $(this).attr({
                    'role': 'tabpanel',
                    'id': 'slick-slide' + _.instanceUid + i,
                    'tabindex': -1
                });

                if (slideControlIndex !== -1) {
                   var ariaButtonControl = 'slick-slide-control' + _.instanceUid + slideControlIndex
                   if ($('#' + ariaButtonControl).length) {
                     $(this).attr({
                         'aria-describedby': ariaButtonControl
                     });
                   }
                }
            });

            _.$dots.attr('role', 'tablist').find('li').each(function(i) {
                var mappedSlideIndex = tabControlIndexes[i];

                $(this).attr({
                    'role': 'presentation'
                });

                $(this).find('button').first().attr({
                    'role': 'tab',
                    'id': 'slick-slide-control' + _.instanceUid + i,
                    'aria-controls': 'slick-slide' + _.instanceUid + mappedSlideIndex,
                    'aria-label': (i + 1) + ' of ' + numDotGroups,
                    'aria-selected': null,
                    'tabindex': '-1'
                });

            }).eq(_.currentSlide).find('button').attr({
                'aria-selected': 'true',
                'tabindex': '0'
            }).end();
        }

        for (var i=_.currentSlide, max=i+_.options.slidesToShow; i < max; i++) {
          if (_.options.focusOnChange) {
            _.$slides.eq(i).attr({'tabindex': '0'});
          } else {
            _.$slides.eq(i).removeAttr('tabindex');
          }
        }

        _.activateADA();

    };

    Slick.prototype.initArrowEvents = function() {

        var _ = this;

        if (_.options.arrows === true && _.slideCount > _.options.slidesToShow) {
            _.$prevArrow
               .off('click.slick')
               .on('click.slick', {
                    message: 'previous'
               }, _.changeSlide);
            _.$nextArrow
               .off('click.slick')
               .on('click.slick', {
                    message: 'next'
               }, _.changeSlide);

            if (_.options.accessibility === true) {
                _.$prevArrow.on('keydown.slick', _.keyHandler);
                _.$nextArrow.on('keydown.slick', _.keyHandler);
            }
        }

    };

    Slick.prototype.initDotEvents = function() {

        var _ = this;

        if (_.options.dots === true && _.slideCount > _.options.slidesToShow) {
            $('li', _.$dots).on('click.slick', {
                message: 'index'
            }, _.changeSlide);

            if (_.options.accessibility === true) {
                _.$dots.on('keydown.slick', _.keyHandler);
            }
        }

        if (_.options.dots === true && _.options.pauseOnDotsHover === true && _.slideCount > _.options.slidesToShow) {

            $('li', _.$dots)
                .on('mouseenter.slick', $.proxy(_.interrupt, _, true))
                .on('mouseleave.slick', $.proxy(_.interrupt, _, false));

        }

    };

    Slick.prototype.initSlideEvents = function() {

        var _ = this;

        if ( _.options.pauseOnHover ) {

            _.$list.on('mouseenter.slick', $.proxy(_.interrupt, _, true));
            _.$list.on('mouseleave.slick', $.proxy(_.interrupt, _, false));

        }

    };

    Slick.prototype.initializeEvents = function() {

        var _ = this;

        _.initArrowEvents();

        _.initDotEvents();
        _.initSlideEvents();

        _.$list.on('touchstart.slick mousedown.slick', {
            action: 'start'
        }, _.swipeHandler);
        _.$list.on('touchmove.slick mousemove.slick', {
            action: 'move'
        }, _.swipeHandler);
        _.$list.on('touchend.slick mouseup.slick', {
            action: 'end'
        }, _.swipeHandler);
        _.$list.on('touchcancel.slick mouseleave.slick', {
            action: 'end'
        }, _.swipeHandler);

        _.$list.on('click.slick', _.clickHandler);

        $(document).on(_.visibilityChange, $.proxy(_.visibility, _));

        if (_.options.accessibility === true) {
            _.$list.on('keydown.slick', _.keyHandler);
        }

        if (_.options.focusOnSelect === true) {
            $(_.$slideTrack).children().on('click.slick', _.selectHandler);
        }

        $(window).on('orientationchange.slick.slick-' + _.instanceUid, $.proxy(_.orientationChange, _));

        $(window).on('resize.slick.slick-' + _.instanceUid, $.proxy(_.resize, _));

        $('[draggable!=true]', _.$slideTrack).on('dragstart', _.preventDefault);

        $(window).on('load.slick.slick-' + _.instanceUid, _.setPosition);
        $(_.setPosition);

    };

    Slick.prototype.initUI = function() {

        var _ = this;

        if (_.options.arrows === true && _.slideCount > _.options.slidesToShow) {

            _.$prevArrow.show();
            _.$nextArrow.show();

        }

        if (_.options.dots === true && _.slideCount > _.options.slidesToShow) {

            _.$dots.show();

        }

    };

    Slick.prototype.keyHandler = function(event) {

        var _ = this;
         //Dont slide if the cursor is inside the form fields and arrow keys are pressed
        if(!event.target.tagName.match('TEXTAREA|INPUT|SELECT')) {
            if (event.keyCode === 37 && _.options.accessibility === true) {
                _.changeSlide({
                    data: {
                        message: _.options.rtl === true ? 'next' :  'previous'
                    }
                });
            } else if (event.keyCode === 39 && _.options.accessibility === true) {
                _.changeSlide({
                    data: {
                        message: _.options.rtl === true ? 'previous' : 'next'
                    }
                });
            }
        }

    };

    Slick.prototype.lazyLoad = function() {

        var _ = this,
            loadRange, cloneRange, rangeStart, rangeEnd;

        function loadImages(imagesScope) {

            $('img[data-lazy]', imagesScope).each(function() {

                var image = $(this),
                    imageSource = $(this).attr('data-lazy'),
                    imageSrcSet = $(this).attr('data-srcset'),
                    imageAlt = $(this).attr('data-alt'),
                    imageTitle = $(this).attr('data-title'),
                    imageSizes  = $(this).attr('data-sizes') || _.$slider.attr('data-sizes'),
                    imageToLoad = document.createElement('img');

                imageToLoad.onload = function() {

                    image
                        .animate({ opacity: 0 }, 100, function() {

                            if (imageSrcSet) {
                                image
                                    .attr('srcset', imageSrcSet );

                                if (imageSizes) {
                                    image
                                        .attr('sizes', imageSizes );
                                }
                            }

                            image
                                .attr('src', imageSource)
                                .attr('alt', imageAlt)
                                .attr('title', imageTitle)
                                .animate({ opacity: 1 }, 200, function() {
                                    image
                                        .removeAttr('data-lazy data-srcset data-sizes')
                                        .removeClass('slick-loading');
                                });
                            _.$slider.trigger('lazyLoaded', [_, image, imageSource]);
                        });

                };

                imageToLoad.onerror = function() {

                    image
                        .removeAttr( 'data-lazy' )
                        .removeClass( 'slick-loading' )
                        .addClass( 'slick-lazyload-error' );

                    _.$slider.trigger('lazyLoadError', [ _, image, imageSource ]);

                };

                imageToLoad.src = imageSource;

            });

        }

        if (_.options.centerMode === true) {
            if (_.options.infinite === true) {
                rangeStart = _.currentSlide + (_.options.slidesToShow / 2 + 1);
                rangeEnd = rangeStart + _.options.slidesToShow + 2;
            } else {
                rangeStart = Math.max(0, _.currentSlide - (_.options.slidesToShow / 2 + 1));
                rangeEnd = 2 + (_.options.slidesToShow / 2 + 1) + _.currentSlide;
            }
        } else {
            rangeStart = _.options.infinite ? _.options.slidesToShow + _.currentSlide : _.currentSlide;
            rangeEnd = Math.ceil(rangeStart + _.options.slidesToShow);
            if (_.options.fade === true) {
                if (rangeStart > 0) rangeStart--;
                if (rangeEnd <= _.slideCount) rangeEnd++;
            }
        }

        loadRange = _.$slider.find('.slick-slide').slice(rangeStart, rangeEnd);

        if (_.options.lazyLoad === 'anticipated') {
            var prevSlide = rangeStart - 1,
                nextSlide = rangeEnd,
                $slides = _.$slider.find('.slick-slide');

            for (var i = 0; i < _.options.slidesToScroll; i++) {
                if (prevSlide < 0) prevSlide = _.slideCount - 1;
                loadRange = loadRange.add($slides.eq(prevSlide));
                loadRange = loadRange.add($slides.eq(nextSlide));
                prevSlide--;
                nextSlide++;
            }
        }

        loadImages(loadRange);

        if (_.slideCount <= _.options.slidesToShow) {
            cloneRange = _.$slider.find('.slick-slide');
            loadImages(cloneRange);
        } else
        if (_.currentSlide >= _.slideCount - _.options.slidesToShow) {
            cloneRange = _.$slider.find('.slick-cloned').slice(0, _.options.slidesToShow);
            loadImages(cloneRange);
        } else if (_.currentSlide === 0) {
            cloneRange = _.$slider.find('.slick-cloned').slice(_.options.slidesToShow * -1);
            loadImages(cloneRange);
        }

    };

    Slick.prototype.loadSlider = function() {

        var _ = this;

        _.setPosition();

        _.$slideTrack.css({
            opacity: 1
        });

        _.$slider.removeClass('slick-loading');

        _.initUI();

        if (_.options.lazyLoad === 'progressive') {
            _.progressiveLazyLoad();
        }

    };

    Slick.prototype.next = Slick.prototype.slickNext = function() {

        var _ = this;

        _.changeSlide({
            data: {
                message: 'next'
            }
        });

    };

    Slick.prototype.orientationChange = function() {

        var _ = this;

        _.checkResponsive();
        _.setPosition();

    };

    Slick.prototype.pause = Slick.prototype.slickPause = function() {

        var _ = this;

        _.autoPlayClear();
        _.paused = true;

    };

    Slick.prototype.play = Slick.prototype.slickPlay = function() {

        var _ = this;

        _.autoPlay();
        _.options.autoplay = true;
        _.paused = false;
        _.focussed = false;
        _.interrupted = false;

    };

    Slick.prototype.postSlide = function(index) {

        var _ = this;

        if( !_.unslicked ) {

            _.$slider.trigger('afterChange', [_, index]);

            _.animating = false;

            if (_.slideCount > _.options.slidesToShow) {
                _.setPosition();
            }

            _.swipeLeft = null;

            if ( _.options.autoplay ) {
                _.autoPlay();
            }

            if (_.options.accessibility === true) {
                _.initADA();

                if (_.options.focusOnChange) {
                    var $currentSlide = $(_.$slides.get(_.currentSlide));
                    $currentSlide.attr('tabindex', 0).focus();
                }
            }

        }

    };

    Slick.prototype.prev = Slick.prototype.slickPrev = function() {

        var _ = this;

        _.changeSlide({
            data: {
                message: 'previous'
            }
        });

    };

    Slick.prototype.preventDefault = function(event) {

        event.preventDefault();

    };

    Slick.prototype.progressiveLazyLoad = function( tryCount ) {

        tryCount = tryCount || 1;

        var _ = this,
            $imgsToLoad = $( 'img[data-lazy]', _.$slider ),
            image,
            imageSource,
            imageSrcSet,
            imageSizes,
            imageToLoad;

        if ( $imgsToLoad.length ) {

            image = $imgsToLoad.first();
            imageSource = image.attr('data-lazy');
            imageSrcSet = image.attr('data-srcset');
            var imageAlt = $(this).attr('data-alt'),
            imageTitle = $(this).attr('data-title'),
            imageSizes  = image.attr('data-sizes') || _.$slider.attr('data-sizes');
            imageToLoad = document.createElement('img');

            imageToLoad.onload = function() {

                if (imageSrcSet) {
                    image
                        .attr('srcset', imageSrcSet );

                    if (imageSizes) {
                        image
                            .attr('sizes', imageSizes );
                    }
                }

                image
                    .attr('src', imageSource)
                    .attr('alt', imageAlt)
                    .attr('title', imageTitle)
                    .removeAttr('data-lazy data-srcset data-sizes')
                    .removeClass('slick-loading');

                if ( _.options.adaptiveHeight === true ) {
                    _.setPosition();
                }

                _.$slider.trigger('lazyLoaded', [ _, image, imageSource ]);
                _.progressiveLazyLoad();

            };

            imageToLoad.onerror = function() {

                if ( tryCount < 3 ) {

                    /**
                     * try to load the image 3 times,
                     * leave a slight delay so we don't get
                     * servers blocking the request.
                     */
                    setTimeout( function() {
                        _.progressiveLazyLoad( tryCount + 1 );
                    }, 500 );

                } else {

                    image
                        .removeAttr( 'data-lazy' )
                        .removeClass( 'slick-loading' )
                        .addClass( 'slick-lazyload-error' );

                    _.$slider.trigger('lazyLoadError', [ _, image, imageSource ]);

                    _.progressiveLazyLoad();

                }

            };

            imageToLoad.src = imageSource;

        } else {

            _.$slider.trigger('allImagesLoaded', [ _ ]);

        }

    };

    Slick.prototype.refresh = function( initializing ) {

        var _ = this, currentSlide, lastVisibleIndex;

        lastVisibleIndex = _.slideCount - _.options.slidesToShow;

        // in non-infinite sliders, we don't want to go past the
        // last visible index.
        if( !_.options.infinite && ( _.currentSlide > lastVisibleIndex )) {
            _.currentSlide = lastVisibleIndex;
        }

        // if less slides than to show, go to start.
        if ( _.slideCount <= _.options.slidesToShow ) {
            _.currentSlide = 0;

        }

        currentSlide = _.currentSlide;

        _.destroy(true);

        $.extend(_, _.initials, { currentSlide: currentSlide });

        _.init();

        if( !initializing ) {

            _.changeSlide({
                data: {
                    message: 'index',
                    index: currentSlide
                }
            }, false);

        }

    };

    Slick.prototype.registerBreakpoints = function() {

        var _ = this, breakpoint, currentBreakpoint, l,
            responsiveSettings = _.options.responsive || null;

        if ( $.type(responsiveSettings) === 'array' && responsiveSettings.length ) {

            _.respondTo = _.options.respondTo || 'window';

            for ( breakpoint in responsiveSettings ) {

                l = _.breakpoints.length-1;

                if (responsiveSettings.hasOwnProperty(breakpoint)) {
                    currentBreakpoint = responsiveSettings[breakpoint].breakpoint;

                    // loop through the breakpoints and cut out any existing
                    // ones with the same breakpoint number, we don't want dupes.
                    while( l >= 0 ) {
                        if( _.breakpoints[l] && _.breakpoints[l] === currentBreakpoint ) {
                            _.breakpoints.splice(l,1);
                        }
                        l--;
                    }

                    _.breakpoints.push(currentBreakpoint);
                    _.breakpointSettings[currentBreakpoint] = responsiveSettings[breakpoint].settings;

                }

            }

            _.breakpoints.sort(function(a, b) {
                return ( _.options.mobileFirst ) ? a-b : b-a;
            });

        }

    };

    Slick.prototype.reinit = function() {

        var _ = this;

        _.$slides =
            _.$slideTrack
                .children(_.options.slide)
                .addClass('slick-slide');

        _.slideCount = _.$slides.length;

        if (_.currentSlide >= _.slideCount && _.currentSlide !== 0) {
            _.currentSlide = _.currentSlide - _.options.slidesToScroll;
        }

        if (_.slideCount <= _.options.slidesToShow) {
            _.currentSlide = 0;
        }

        _.registerBreakpoints();

        _.setProps();
        _.setupInfinite();
        _.buildArrows();
        _.updateArrows();
        _.initArrowEvents();
        _.buildDots();
        _.updateDots();
        _.initDotEvents();
        _.cleanUpSlideEvents();
        _.initSlideEvents();

        _.checkResponsive(false, true);

        if (_.options.focusOnSelect === true) {
            $(_.$slideTrack).children().on('click.slick', _.selectHandler);
        }

        _.setSlideClasses(typeof _.currentSlide === 'number' ? _.currentSlide : 0);

        _.setPosition();
        _.focusHandler();

        _.paused = !_.options.autoplay;
        _.autoPlay();

        _.$slider.trigger('reInit', [_]);

    };

    Slick.prototype.resize = function() {

        var _ = this;

        if ($(window).width() !== _.windowWidth) {
            clearTimeout(_.windowDelay);
            _.windowDelay = window.setTimeout(function() {
                _.windowWidth = $(window).width();
                _.checkResponsive();
                if( !_.unslicked ) { _.setPosition(); }
            }, 50);
        }
    };

    Slick.prototype.removeSlide = Slick.prototype.slickRemove = function(index, removeBefore, removeAll) {

        var _ = this;

        if (typeof(index) === 'boolean') {
            removeBefore = index;
            index = removeBefore === true ? 0 : _.slideCount - 1;
        } else {
            index = removeBefore === true ? --index : index;
        }

        if (_.slideCount < 1 || index < 0 || index > _.slideCount - 1) {
            return false;
        }

        _.unload();

        if (removeAll === true) {
            _.$slideTrack.children().remove();
        } else {
            _.$slideTrack.children(this.options.slide).eq(index).remove();
        }

        _.$slides = _.$slideTrack.children(this.options.slide);

        _.$slideTrack.children(this.options.slide).detach();

        _.$slideTrack.append(_.$slides);

        _.$slidesCache = _.$slides;

        _.reinit();

    };

    Slick.prototype.setCSS = function(position) {

        var _ = this,
            positionProps = {},
            x, y;

        if (_.options.rtl === true) {
            position = -position;
        }
        x = _.positionProp == 'left' ? Math.ceil(position) + 'px' : '0px';
        y = _.positionProp == 'top' ? Math.ceil(position) + 'px' : '0px';

        positionProps[_.positionProp] = position;

        if (_.transformsEnabled === false) {
            _.$slideTrack.css(positionProps);
        } else {
            positionProps = {};
            if (_.cssTransitions === false) {
                positionProps[_.animType] = 'translate(' + x + ', ' + y + ')';
                _.$slideTrack.css(positionProps);
            } else {
                positionProps[_.animType] = 'translate3d(' + x + ', ' + y + ', 0px)';
                _.$slideTrack.css(positionProps);
            }
        }

    };

    Slick.prototype.setDimensions = function() {

        var _ = this;

        if (_.options.vertical === false) {
            if (_.options.centerMode === true) {
                _.$list.css({
                    padding: ('0px ' + _.options.centerPadding)
                });
            }
        } else {
            _.$list.height(_.$slides.first().outerHeight(true) * _.options.slidesToShow);
            if (_.options.centerMode === true) {
                _.$list.css({
                    padding: (_.options.centerPadding + ' 0px')
                });
            }
        }

        _.listWidth = _.$list.width();
        _.listHeight = _.$list.height();


        if (_.options.vertical === false && _.options.variableWidth === false) {
            _.slideWidth = Math.ceil(_.listWidth / _.options.slidesToShow);
            _.$slideTrack.width(Math.ceil((_.slideWidth * _.$slideTrack.children('.slick-slide').length)));

        } else if (_.options.variableWidth === true) {
            _.$slideTrack.width(5000 * _.slideCount);
        } else {
            _.slideWidth = Math.ceil(_.listWidth);
            _.$slideTrack.height(Math.ceil((_.$slides.first().outerHeight(true) * _.$slideTrack.children('.slick-slide').length)));
        }

        var offset = _.$slides.first().outerWidth(true) - _.$slides.first().width();
        if (_.options.variableWidth === false) _.$slideTrack.children('.slick-slide').width(_.slideWidth - offset);

    };

    Slick.prototype.setFade = function() {

        var _ = this,
            targetLeft;

        _.$slides.each(function(index, element) {
            targetLeft = (_.slideWidth * index) * -1;
            if (_.options.rtl === true) {
                $(element).css({
                    position: 'relative',
                    right: targetLeft,
                    top: 0,
                    zIndex: _.options.zIndex - 2,
                    opacity: 0
                });
            } else {
                $(element).css({
                    position: 'relative',
                    left: targetLeft,
                    top: 0,
                    zIndex: _.options.zIndex - 2,
                    opacity: 0
                });
            }
        });

        _.$slides.eq(_.currentSlide).css({
            zIndex: _.options.zIndex - 1,
            opacity: 1
        });

    };

    Slick.prototype.setHeight = function() {

        var _ = this;

        if (_.options.slidesToShow === 1 && _.options.adaptiveHeight === true && _.options.vertical === false) {
            var targetHeight = _.$slides.eq(_.currentSlide).outerHeight(true);
            _.$list.css('height', targetHeight);
        }

    };

    Slick.prototype.setOption =
    Slick.prototype.slickSetOption = function() {

        /**
         * accepts arguments in format of:
         *
         *  - for changing a single option's value:
         *     .slick("setOption", option, value, refresh )
         *
         *  - for changing a set of responsive options:
         *     .slick("setOption", 'responsive', [{}, ...], refresh )
         *
         *  - for updating multiple values at once (not responsive)
         *     .slick("setOption", { 'option': value, ... }, refresh )
         */

        var _ = this, l, item, option, value, refresh = false, type;

        if( $.type( arguments[0] ) === 'object' ) {

            option =  arguments[0];
            refresh = arguments[1];
            type = 'multiple';

        } else if ( $.type( arguments[0] ) === 'string' ) {

            option =  arguments[0];
            value = arguments[1];
            refresh = arguments[2];

            if ( arguments[0] === 'responsive' && $.type( arguments[1] ) === 'array' ) {

                type = 'responsive';

            } else if ( typeof arguments[1] !== 'undefined' ) {

                type = 'single';

            }

        }

        if ( type === 'single' ) {

            _.options[option] = value;


        } else if ( type === 'multiple' ) {

            $.each( option , function( opt, val ) {

                _.options[opt] = val;

            });


        } else if ( type === 'responsive' ) {

            for ( item in value ) {

                if( $.type( _.options.responsive ) !== 'array' ) {

                    _.options.responsive = [ value[item] ];

                } else {

                    l = _.options.responsive.length-1;

                    // loop through the responsive object and splice out duplicates.
                    while( l >= 0 ) {

                        if( _.options.responsive[l].breakpoint === value[item].breakpoint ) {

                            _.options.responsive.splice(l,1);

                        }

                        l--;

                    }

                    _.options.responsive.push( value[item] );

                }

            }

        }

        if ( refresh ) {

            _.unload();
            _.reinit();

        }

    };

    Slick.prototype.setPosition = function() {

        var _ = this;

        _.setDimensions();

        _.setHeight();

        if (_.options.fade === false) {
            _.setCSS(_.getLeft(_.currentSlide));
        } else {
            _.setFade();
        }

        _.$slider.trigger('setPosition', [_]);

    };

    Slick.prototype.setProps = function() {

        var _ = this,
            bodyStyle = document.body.style;

        _.positionProp = _.options.vertical === true ? 'top' : 'left';

        if (_.positionProp === 'top') {
            _.$slider.addClass('slick-vertical');
        } else {
            _.$slider.removeClass('slick-vertical');
        }

        if (bodyStyle.WebkitTransition !== undefined ||
            bodyStyle.MozTransition !== undefined ||
            bodyStyle.msTransition !== undefined) {
            if (_.options.useCSS === true) {
                _.cssTransitions = true;
            }
        }

        if ( _.options.fade ) {
            if ( typeof _.options.zIndex === 'number' ) {
                if( _.options.zIndex < 3 ) {
                    _.options.zIndex = 3;
                }
            } else {
                _.options.zIndex = _.defaults.zIndex;
            }
        }

        if (bodyStyle.OTransform !== undefined) {
            _.animType = 'OTransform';
            _.transformType = '-o-transform';
            _.transitionType = 'OTransition';
            if (bodyStyle.perspectiveProperty === undefined && bodyStyle.webkitPerspective === undefined) _.animType = false;
        }
        if (bodyStyle.MozTransform !== undefined) {
            _.animType = 'MozTransform';
            _.transformType = '-moz-transform';
            _.transitionType = 'MozTransition';
            if (bodyStyle.perspectiveProperty === undefined && bodyStyle.MozPerspective === undefined) _.animType = false;
        }
        if (bodyStyle.webkitTransform !== undefined) {
            _.animType = 'webkitTransform';
            _.transformType = '-webkit-transform';
            _.transitionType = 'webkitTransition';
            if (bodyStyle.perspectiveProperty === undefined && bodyStyle.webkitPerspective === undefined) _.animType = false;
        }
        if (bodyStyle.msTransform !== undefined) {
            _.animType = 'msTransform';
            _.transformType = '-ms-transform';
            _.transitionType = 'msTransition';
            if (bodyStyle.msTransform === undefined) _.animType = false;
        }
        if (bodyStyle.transform !== undefined && _.animType !== false) {
            _.animType = 'transform';
            _.transformType = 'transform';
            _.transitionType = 'transition';
        }
        _.transformsEnabled = _.options.useTransform && (_.animType !== null && _.animType !== false);
    };


    Slick.prototype.setSlideClasses = function(index) {

        var _ = this,
            centerOffset, allSlides, indexOffset, remainder;

        allSlides = _.$slider
            .find('.slick-slide')
            .removeClass('slick-active slick-center slick-current')
            .attr('aria-hidden', 'true');

        _.$slides
            .eq(index)
            .addClass('slick-current');

        if (_.options.centerMode === true) {

            var evenCoef = _.options.slidesToShow % 2 === 0 ? 1 : 0;

            centerOffset = Math.floor(_.options.slidesToShow / 2);

            if (_.options.infinite === true) {

                if (index >= centerOffset && index <= (_.slideCount - 1) - centerOffset) {
                    _.$slides
                        .slice(index - centerOffset + evenCoef, index + centerOffset + 1)
                        .addClass('slick-active')
                        .attr('aria-hidden', 'false');

                } else {

                    indexOffset = _.options.slidesToShow + index;
                    allSlides
                        .slice(indexOffset - centerOffset + 1 + evenCoef, indexOffset + centerOffset + 2)
                        .addClass('slick-active')
                        .attr('aria-hidden', 'false');

                }

                if (index === 0) {

                    allSlides
                        .eq(allSlides.length - 1 - _.options.slidesToShow)
                        .addClass('slick-center');

                } else if (index === _.slideCount - 1) {

                    allSlides
                        .eq(_.options.slidesToShow)
                        .addClass('slick-center');

                }

            }

            _.$slides
                .eq(index)
                .addClass('slick-center');

        } else {

            if (index >= 0 && index <= (_.slideCount - _.options.slidesToShow)) {

                _.$slides
                    .slice(index, index + _.options.slidesToShow)
                    .addClass('slick-active')
                    .attr('aria-hidden', 'false');

            } else if (allSlides.length <= _.options.slidesToShow) {

                allSlides
                    .addClass('slick-active')
                    .attr('aria-hidden', 'false');

            } else {

                remainder = _.slideCount % _.options.slidesToShow;
                indexOffset = _.options.infinite === true ? _.options.slidesToShow + index : index;

                if (_.options.slidesToShow == _.options.slidesToScroll && (_.slideCount - index) < _.options.slidesToShow) {

                    allSlides
                        .slice(indexOffset - (_.options.slidesToShow - remainder), indexOffset + remainder)
                        .addClass('slick-active')
                        .attr('aria-hidden', 'false');

                } else {

                    allSlides
                        .slice(indexOffset, indexOffset + _.options.slidesToShow)
                        .addClass('slick-active')
                        .attr('aria-hidden', 'false');

                }

            }

        }

        if (_.options.lazyLoad === 'ondemand' || _.options.lazyLoad === 'anticipated') {
            _.lazyLoad();
        }
    };

    Slick.prototype.setupInfinite = function() {

        var _ = this,
            i, slideIndex, infiniteCount;

        if (_.options.fade === true) {
            _.options.centerMode = false;
        }

        if (_.options.infinite === true && _.options.fade === false) {

            slideIndex = null;

            if (_.slideCount > _.options.slidesToShow) {

                if (_.options.centerMode === true) {
                    infiniteCount = _.options.slidesToShow + 1;
                } else {
                    infiniteCount = _.options.slidesToShow;
                }

                for (i = _.slideCount; i > (_.slideCount -
                        infiniteCount); i -= 1) {
                    slideIndex = i - 1;
                    $(_.$slides[slideIndex]).clone(true).attr('id', '')
                        .attr('data-slick-index', slideIndex - _.slideCount)
                        .prependTo(_.$slideTrack).addClass('slick-cloned');
                }
                for (i = 0; i < infiniteCount  + _.slideCount; i += 1) {
                    slideIndex = i;
                    $(_.$slides[slideIndex]).clone(true).attr('id', '')
                        .attr('data-slick-index', slideIndex + _.slideCount)
                        .appendTo(_.$slideTrack).addClass('slick-cloned');
                }
                _.$slideTrack.find('.slick-cloned').find('[id]').each(function() {
                    $(this).attr('id', '');
                });

            }

        }

    };

    Slick.prototype.interrupt = function( toggle ) {

        var _ = this;

        if( !toggle ) {
            _.autoPlay();
        }
        _.interrupted = toggle;

    };

    Slick.prototype.selectHandler = function(event) {

        var _ = this;

        var targetElement =
            $(event.target).is('.slick-slide') ?
                $(event.target) :
                $(event.target).parents('.slick-slide');

        var index = parseInt(targetElement.attr('data-slick-index'));

        if (!index) index = 0;

        if (_.slideCount <= _.options.slidesToShow) {

            _.slideHandler(index, false, true);
            return;

        }

        _.slideHandler(index);

    };

    Slick.prototype.slideHandler = function(index, sync, dontAnimate) {

        var targetSlide, animSlide, oldSlide, slideLeft, targetLeft = null,
            _ = this, navTarget;

        sync = sync || false;

        if (_.animating === true && _.options.waitForAnimate === true) {
            return;
        }

        if (_.options.fade === true && _.currentSlide === index) {
            return;
        }

        if (sync === false) {
            _.asNavFor(index);
        }

        targetSlide = index;
        targetLeft = _.getLeft(targetSlide);
        slideLeft = _.getLeft(_.currentSlide);

        _.currentLeft = _.swipeLeft === null ? slideLeft : _.swipeLeft;

        if (_.options.infinite === false && _.options.centerMode === false && (index < 0 || index > _.getDotCount() * _.options.slidesToScroll)) {
            if (_.options.fade === false) {
                targetSlide = _.currentSlide;
                if (dontAnimate !== true && _.slideCount > _.options.slidesToShow) {
                    _.animateSlide(slideLeft, function() {
                        _.postSlide(targetSlide);
                    });
                } else {
                    _.postSlide(targetSlide);
                }
            }
            return;
        } else if (_.options.infinite === false && _.options.centerMode === true && (index < 0 || index > (_.slideCount - _.options.slidesToScroll))) {
            if (_.options.fade === false) {
                targetSlide = _.currentSlide;
                if (dontAnimate !== true && _.slideCount > _.options.slidesToShow) {
                    _.animateSlide(slideLeft, function() {
                        _.postSlide(targetSlide);
                    });
                } else {
                    _.postSlide(targetSlide);
                }
            }
            return;
        }

        if ( _.options.autoplay ) {
            clearInterval(_.autoPlayTimer);
        }

        if (targetSlide < 0) {
            if (_.slideCount % _.options.slidesToScroll !== 0) {
                animSlide = _.slideCount - (_.slideCount % _.options.slidesToScroll);
            } else {
                animSlide = _.slideCount + targetSlide;
            }
        } else if (targetSlide >= _.slideCount) {
            if (_.slideCount % _.options.slidesToScroll !== 0) {
                animSlide = 0;
            } else {
                animSlide = targetSlide - _.slideCount;
            }
        } else {
            animSlide = targetSlide;
        }

        _.animating = true;

        _.$slider.trigger('beforeChange', [_, _.currentSlide, animSlide]);

        oldSlide = _.currentSlide;
        _.currentSlide = animSlide;

        _.setSlideClasses(_.currentSlide);

        if ( _.options.asNavFor ) {

            navTarget = _.getNavTarget();
            navTarget = navTarget.slick('getSlick');

            if ( navTarget.slideCount <= navTarget.options.slidesToShow ) {
                navTarget.setSlideClasses(_.currentSlide);
            }

        }

        _.updateDots();
        _.updateArrows();

        if (_.options.fade === true) {
            if (dontAnimate !== true) {

                _.fadeSlideOut(oldSlide);

                _.fadeSlide(animSlide, function() {
                    _.postSlide(animSlide);
                });

            } else {
                _.postSlide(animSlide);
            }
            _.animateHeight();
            return;
        }

        if (dontAnimate !== true && _.slideCount > _.options.slidesToShow) {
            _.animateSlide(targetLeft, function() {
                _.postSlide(animSlide);
            });
        } else {
            _.postSlide(animSlide);
        }

    };

    Slick.prototype.startLoad = function() {

        var _ = this;

        if (_.options.arrows === true && _.slideCount > _.options.slidesToShow) {

            _.$prevArrow.hide();
            _.$nextArrow.hide();

        }

        if (_.options.dots === true && _.slideCount > _.options.slidesToShow) {

            _.$dots.hide();

        }

        _.$slider.addClass('slick-loading');

    };

    Slick.prototype.swipeDirection = function() {

        var xDist, yDist, r, swipeAngle, _ = this;

        xDist = _.touchObject.startX - _.touchObject.curX;
        yDist = _.touchObject.startY - _.touchObject.curY;
        r = Math.atan2(yDist, xDist);

        swipeAngle = Math.round(r * 180 / Math.PI);
        if (swipeAngle < 0) {
            swipeAngle = 360 - Math.abs(swipeAngle);
        }

        if ((swipeAngle <= 45) && (swipeAngle >= 0)) {
            return (_.options.rtl === false ? 'left' : 'right');
        }
        if ((swipeAngle <= 360) && (swipeAngle >= 315)) {
            return (_.options.rtl === false ? 'left' : 'right');
        }
        if ((swipeAngle >= 135) && (swipeAngle <= 225)) {
            return (_.options.rtl === false ? 'right' : 'left');
        }
        if (_.options.verticalSwiping === true) {
            if ((swipeAngle >= 35) && (swipeAngle <= 135)) {
                return 'down';
            } else {
                return 'up';
            }
        }

        return 'vertical';

    };

    Slick.prototype.swipeEnd = function(event) {

        var _ = this,
            slideCount,
            direction;

        _.dragging = false;
        _.swiping = false;

        if (_.scrolling) {
            _.scrolling = false;
            return false;
        }

        _.interrupted = false;
        _.shouldClick = ( _.touchObject.swipeLength > 10 ) ? false : true;

        if ( _.touchObject.curX === undefined ) {
            return false;
        }

        if ( _.touchObject.edgeHit === true ) {
            _.$slider.trigger('edge', [_, _.swipeDirection() ]);
        }

        if ( _.touchObject.swipeLength >= _.touchObject.minSwipe ) {

            direction = _.swipeDirection();

            switch ( direction ) {

                case 'left':
                case 'down':

                    slideCount =
                        _.options.swipeToSlide ?
                            _.checkNavigable( _.currentSlide + _.getSlideCount() ) :
                            _.currentSlide + _.getSlideCount();

                    _.currentDirection = 0;

                    break;

                case 'right':
                case 'up':

                    slideCount =
                        _.options.swipeToSlide ?
                            _.checkNavigable( _.currentSlide - _.getSlideCount() ) :
                            _.currentSlide - _.getSlideCount();

                    _.currentDirection = 1;

                    break;

                default:


            }

            if( direction != 'vertical' ) {

                _.slideHandler( slideCount );
                _.touchObject = {};
                _.$slider.trigger('swipe', [_, direction ]);

            }

        } else {

            if ( _.touchObject.startX !== _.touchObject.curX ) {

                _.slideHandler( _.currentSlide );
                _.touchObject = {};

            }

        }

    };

    Slick.prototype.swipeHandler = function(event) {

        var _ = this;

        if ((_.options.swipe === false) || ('ontouchend' in document && _.options.swipe === false)) {
            return;
        } else if (_.options.draggable === false && event.type.indexOf('mouse') !== -1) {
            return;
        }

        _.touchObject.fingerCount = event.originalEvent && event.originalEvent.touches !== undefined ?
            event.originalEvent.touches.length : 1;

        _.touchObject.minSwipe = _.listWidth / _.options
            .touchThreshold;

        if (_.options.verticalSwiping === true) {
            _.touchObject.minSwipe = _.listHeight / _.options
                .touchThreshold;
        }

        switch (event.data.action) {

            case 'start':
                _.swipeStart(event);
                break;

            case 'move':
                _.swipeMove(event);
                break;

            case 'end':
                _.swipeEnd(event);
                break;

        }

    };

    Slick.prototype.swipeMove = function(event) {

        var _ = this,
            edgeWasHit = false,
            curLeft, swipeDirection, swipeLength, positionOffset, touches, verticalSwipeLength;

        touches = event.originalEvent !== undefined ? event.originalEvent.touches : null;

        if (!_.dragging || _.scrolling || touches && touches.length !== 1) {
            return false;
        }

        curLeft = _.getLeft(_.currentSlide);

        _.touchObject.curX = touches !== undefined ? touches[0].pageX : event.clientX;
        _.touchObject.curY = touches !== undefined ? touches[0].pageY : event.clientY;

        _.touchObject.swipeLength = Math.round(Math.sqrt(
            Math.pow(_.touchObject.curX - _.touchObject.startX, 2)));

        verticalSwipeLength = Math.round(Math.sqrt(
            Math.pow(_.touchObject.curY - _.touchObject.startY, 2)));

        if (!_.options.verticalSwiping && !_.swiping && verticalSwipeLength > 4) {
            _.scrolling = true;
            return false;
        }

        if (_.options.verticalSwiping === true) {
            _.touchObject.swipeLength = verticalSwipeLength;
        }

        swipeDirection = _.swipeDirection();

        if (event.originalEvent !== undefined && _.touchObject.swipeLength > 4) {
            _.swiping = true;
            event.preventDefault();
        }

        positionOffset = (_.options.rtl === false ? 1 : -1) * (_.touchObject.curX > _.touchObject.startX ? 1 : -1);
        if (_.options.verticalSwiping === true) {
            positionOffset = _.touchObject.curY > _.touchObject.startY ? 1 : -1;
        }


        swipeLength = _.touchObject.swipeLength;

        _.touchObject.edgeHit = false;

        if (_.options.infinite === false) {
            if ((_.currentSlide === 0 && swipeDirection === 'right') || (_.currentSlide >= _.getDotCount() && swipeDirection === 'left')) {
                swipeLength = _.touchObject.swipeLength * _.options.edgeFriction;
                _.touchObject.edgeHit = true;
            }
        }

        if (_.options.vertical === false) {
            _.swipeLeft = curLeft + swipeLength * positionOffset;
        } else {
            _.swipeLeft = curLeft + (swipeLength * (_.$list.height() / _.listWidth)) * positionOffset;
        }
        if (_.options.verticalSwiping === true) {
            _.swipeLeft = curLeft + swipeLength * positionOffset;
        }

        if (_.options.fade === true || _.options.touchMove === false) {
            return false;
        }

        if (_.animating === true) {
            _.swipeLeft = null;
            return false;
        }

        _.setCSS(_.swipeLeft);

    };

    Slick.prototype.swipeStart = function(event) {

        var _ = this,
            touches;

        _.interrupted = true;

        if (_.touchObject.fingerCount !== 1 || _.slideCount <= _.options.slidesToShow) {
            _.touchObject = {};
            return false;
        }

        if (event.originalEvent !== undefined && event.originalEvent.touches !== undefined) {
            touches = event.originalEvent.touches[0];
        }

        _.touchObject.startX = _.touchObject.curX = touches !== undefined ? touches.pageX : event.clientX;
        _.touchObject.startY = _.touchObject.curY = touches !== undefined ? touches.pageY : event.clientY;

        _.dragging = true;

    };

    Slick.prototype.unfilterSlides = Slick.prototype.slickUnfilter = function() {

        var _ = this;

        if (_.$slidesCache !== null) {

            _.unload();

            _.$slideTrack.children(this.options.slide).detach();

            _.$slidesCache.appendTo(_.$slideTrack);

            _.reinit();

        }

    };

    Slick.prototype.unload = function() {

        var _ = this;

        $('.slick-cloned', _.$slider).remove();

        if (_.$dots) {
            _.$dots.remove();
        }

        if (_.$prevArrow && _.htmlExpr.test(_.options.prevArrow)) {
            _.$prevArrow.remove();
        }

        if (_.$nextArrow && _.htmlExpr.test(_.options.nextArrow)) {
            _.$nextArrow.remove();
        }

        _.$slides
            .removeClass('slick-slide slick-active slick-visible slick-current')
            .attr('aria-hidden', 'true')
            .css('width', '');

    };

    Slick.prototype.unslick = function(fromBreakpoint) {

        var _ = this;
        _.$slider.trigger('unslick', [_, fromBreakpoint]);
        _.destroy();

    };

    Slick.prototype.updateArrows = function() {

        var _ = this,
            centerOffset;

        centerOffset = Math.floor(_.options.slidesToShow / 2);

        if ( _.options.arrows === true &&
            _.slideCount > _.options.slidesToShow &&
            !_.options.infinite ) {

            _.$prevArrow.removeClass('slick-disabled').attr('aria-disabled', 'false');
            _.$nextArrow.removeClass('slick-disabled').attr('aria-disabled', 'false');

            if (_.currentSlide === 0) {

                _.$prevArrow.addClass('slick-disabled').attr('aria-disabled', 'true');
                _.$nextArrow.removeClass('slick-disabled').attr('aria-disabled', 'false');

            } else if (_.currentSlide >= _.slideCount - _.options.slidesToShow && _.options.centerMode === false) {

                _.$nextArrow.addClass('slick-disabled').attr('aria-disabled', 'true');
                _.$prevArrow.removeClass('slick-disabled').attr('aria-disabled', 'false');

            } else if (_.currentSlide >= _.slideCount - 1 && _.options.centerMode === true) {

                _.$nextArrow.addClass('slick-disabled').attr('aria-disabled', 'true');
                _.$prevArrow.removeClass('slick-disabled').attr('aria-disabled', 'false');

            }

        }

    };

    Slick.prototype.updateDots = function() {

        var _ = this;

        if (_.$dots !== null) {

            _.$dots
                .find('li')
                    .removeClass('slick-active')
                    .end();

            _.$dots
                .find('li')
                .eq(Math.floor(_.currentSlide / _.options.slidesToScroll))
                .addClass('slick-active');

        }

    };

    Slick.prototype.visibility = function() {

        var _ = this;

        if ( _.options.autoplay ) {

            if ( document[_.hidden] ) {

                _.interrupted = true;

            } else {

                _.interrupted = false;

            }

        }

    };

    $.fn.slick = function() {
        var _ = this,
            opt = arguments[0],
            args = Array.prototype.slice.call(arguments, 1),
            l = _.length,
            i,
            ret;
        for (i = 0; i < l; i++) {
            if (typeof opt == 'object' || typeof opt == 'undefined')
                _[i].slick = new Slick(_[i], opt);
            else
                ret = _[i].slick[opt].apply(_[i].slick, args);
            if (typeof ret != 'undefined') return ret;
        }
        return _;
    };

}));

/*
 Sticky-kit v1.1.2 | WTFPL | Leaf Corcoran 2015 | http://leafo.net
*/
(function () {
    var b, f;
    b = this.jQuery || window.jQuery;
    f = b(window);
    b.fn.stick_in_parent = function (d) {
        var A, w, J, n, B, K, p, q, k, E, t;
        null == d && (d = {});
        t = d.sticky_class;
        B = d.inner_scrolling;
        E = d.recalc_every;
        k = d.parent;
        q = d.offset_top;
        p = d.spacer;
        w = d.bottoming;
        null == q && (q = 0);
        null == k && (k = void 0);
        null == B && (B = !0);
        null == t && (t = "is_stuck");
        A = b(document);
        null == w && (w = !0);
        J = function (a, d, n, C, F, u, r, G) {
            var v, H, m, D, I, c, g, x, y, z, h, l;
            if (!a.data("sticky_kit")) {
                a.data("sticky_kit", !0);
                I = A.height();
                g = a.parent();
                null != k && (g = g.closest(k));
                if (!g.length) throw "failed to find stick parent";
                v = m = !1;
                (h = null != p ? p && a.closest(p) : b("<div />")) && h.css("position", a.css("position"));
                x = function () {
                    var c, f, e;
                    if (!G && (I = A.height(), c = parseInt(g.css("border-top-width"), 10), f = parseInt(g.css("padding-top"), 10), d = parseInt(g.css("padding-bottom"), 10), n = g.offset().top + c + f, C = g.height(), m && (v = m = !1, null == p && (a.insertAfter(h), h.detach()), a.css({
                        position: "",
                        top: "",
                        width: "",
                        bottom: ""
                    }).removeClass(t), e = !0), F = a.offset().top - (parseInt(a.css("margin-top"), 10) || 0) - q,
                            u = a.outerHeight(!0), r = a.css("float"), h && h.css({
                        width: a.outerWidth(!0),
                        height: u,
                        display: a.css("display"),
                                "vertical-align": a.css("vertical-align"),
                                "float": r
                    }), e)) return l()
                };
                x();
                if (u !== C) return D = void 0, c = q, z = E, l = function () {
                    var b, l, e, k;
                    if (!G && (e = !1, null != z && (--z, 0 >= z && (z = E, x(), e = !0)), e || A.height() === I || x(), e = f.scrollTop(), null != D && (l = e - D), D = e, m ? (w && (k = e + u + c > C + n, v && !k && (v = !1, a.css({
                        position: "fixed",
                        bottom: "",
                        top: c
                    }).trigger("sticky_kit:unbottom"))), e < F && (m = !1, c = q, null == p && ("left" !== r && "right" !== r || a.insertAfter(h),
                            h.detach()), b = {
                        position: "",
                        width: "",
                        top: ""
                    }, a.css(b).removeClass(t).trigger("sticky_kit:unstick")), B && (b = f.height(), u + q > b && !v && (c -= l, c = Math.max(b - u, c), c = Math.min(q, c), m && a.css({
                        top: c + "px"
                    })))) : e > F && (m = !0, b = {
                        position: "fixed",
                        top: c
                    }, b.width = "border-box" === a.css("box-sizing") ? a.outerWidth() + "px" : a.width() + "px", a.css(b).addClass(t), null == p && (a.after(h), "left" !== r && "right" !== r || h.append(a)), a.trigger("sticky_kit:stick")), m && w && (null == k && (k = e + u + c > C + n), !v && k))) return v = !0, "static" === g.css("position") && g.css({
                        position: "relative"
                    }),
                    a.css({
                        position: "absolute",
                        bottom: d,
                        top: "auto"
                    }).trigger("sticky_kit:bottom")
                }, y = function () {
                    x();
                    return l()
                }, H = function () {
                    G = !0;
                    f.off("touchmove", l);
                    f.off("scroll", l);
                    f.off("resize", y);
                    b(document.body).off("sticky_kit:recalc", y);
                    a.off("sticky_kit:detach", H);
                    a.removeData("sticky_kit");
                    a.css({
                        position: "",
                        bottom: "",
                        top: "",
                        width: ""
                    });
                    g.position("position", "");
                    if (m) return null == p && ("left" !== r && "right" !== r || a.insertAfter(h), h.remove()), a.removeClass(t)
                }, f.on("touchmove", l), f.on("scroll", l), f.on("resize",
                    y), b(document.body).on("sticky_kit:recalc", y), a.on("sticky_kit:detach", H), setTimeout(l, 0)
            }
        };
        n = 0;
        for (K = this.length; n < K; n++) d = this[n], J(b(d));
        return this
    }
}).call(this);
/*!
 * Lightbox v2.10.0
 * by Lokesh Dhakar
 *
 * More info:
 * http://lokeshdhakar.com/projects/lightbox2/
 *
 * Copyright 2007, 2018 Lokesh Dhakar
 * Released under the MIT license
 * https://github.com/lokesh/lightbox2/blob/master/LICENSE
 *
 * @preserve
 */

// Uses Node, AMD or browser globals to create a module.
(function (root, factory) {
    if (typeof define === 'function' && define.amd) {
        // AMD. Register as an anonymous module.
        define(['jquery'], factory);
    } else if (typeof exports === 'object') {
        // Node. Does not work with strict CommonJS, but
        // only CommonJS-like environments that support module.exports,
        // like Node.
        module.exports = factory(require('jquery'));
    } else {
        // Browser globals (root is window)
        root.lightbox = factory(root.jQuery);
    }
}(this, function ($) {

    function Lightbox(options) {
        this.album = [];
        this.currentImageIndex = void 0;
        this.init();

        // options
        this.options = $.extend({}, this.constructor.defaults);
        this.option(options);
    }

    // Descriptions of all options available on the demo site:
    // http://lokeshdhakar.com/projects/lightbox2/index.html#options
    Lightbox.defaults = {
        albumLabel: 'Image %1 of %2',
        alwaysShowNavOnTouchDevices: false,
        fadeDuration: 600,
        fitImagesInViewport: true,
        imageFadeDuration: 600,
        // maxWidth: 800,
        // maxHeight: 600,
        positionFromTop: 50,
        resizeDuration: 700,
        showImageNumberLabel: true,
        wrapAround: false,
        disableScrolling: false,
        /*
        Sanitize Title
        If the caption data is trusted, for example you are hardcoding it in, then leave this to false.
        This will free you to add html tags, such as links, in the caption.
    
        If the caption data is user submitted or from some other untrusted source, then set this to true
        to prevent xss and other injection attacks.
         */
        sanitizeTitle: false
    };

    Lightbox.prototype.option = function (options) {
        $.extend(this.options, options);
    };

    Lightbox.prototype.imageCountLabel = function (currentImageNum, totalImages) {
        return this.options.albumLabel.replace(/%1/g, currentImageNum).replace(/%2/g, totalImages);
    };

    Lightbox.prototype.init = function () {
        var self = this;
        // Both enable and build methods require the body tag to be in the DOM.
        $(document).ready(function () {
            self.enable();
            self.build();
        });
    };

    // Loop through anchors and areamaps looking for either data-lightbox attributes or rel attributes
    // that contain 'lightbox'. When these are clicked, start lightbox.
    Lightbox.prototype.enable = function () {
        var self = this;
        $('body').on('click', 'a[rel^=lightbox], area[rel^=lightbox], a[data-lightbox], area[data-lightbox]', function (event) {
            self.start($(event.currentTarget));
            return false;
        });
    };

    // Build html for the lightbox and the overlay.
    // Attach event handlers to the new DOM elements. click click click
    Lightbox.prototype.build = function () {
        if ($('#lightbox').length > 0) {
            return;
        }

        var self = this;
        $('<div id="lightboxOverlay" class="lightboxOverlay"></div><div id="lightbox" class="lightbox"><div class="lb-outerContainer"><div class="lb-container"><img class="lb-image" src="data:image/gif;base64,R0lGODlhAQABAIAAAP///wAAACH5BAEAAAAALAAAAAABAAEAAAICRAEAOw==" /><div class="lb-nav"><a class="lb-prev" href="" ></a><a class="lb-next" href="" ></a></div><div class="lb-loader"><a class="lb-cancel"></a></div></div></div><div class="lb-dataContainer"><div class="lb-data"><div class="lb-details"><span class="lb-caption"></span><span class="lb-number"></span></div><div class="lb-closeContainer"><a class="lb-close"></a></div></div></div></div>').appendTo($('body'));

        // Cache jQuery objects
        this.$lightbox = $('#lightbox');
        this.$overlay = $('#lightboxOverlay');
        this.$outerContainer = this.$lightbox.find('.lb-outerContainer');
        this.$container = this.$lightbox.find('.lb-container');
        this.$image = this.$lightbox.find('.lb-image');
        this.$nav = this.$lightbox.find('.lb-nav');

        // Store css values for future lookup
        this.containerPadding = {
            top: parseInt(this.$container.css('padding-top'), 10),
            right: parseInt(this.$container.css('padding-right'), 10),
            bottom: parseInt(this.$container.css('padding-bottom'), 10),
            left: parseInt(this.$container.css('padding-left'), 10)
        };

        this.imageBorderWidth = {
            top: parseInt(this.$image.css('border-top-width'), 10),
            right: parseInt(this.$image.css('border-right-width'), 10),
            bottom: parseInt(this.$image.css('border-bottom-width'), 10),
            left: parseInt(this.$image.css('border-left-width'), 10)
        };

        // Attach event handlers to the newly minted DOM elements
        this.$overlay.hide().on('click', function () {
            self.end();
            return false;
        });

        this.$lightbox.hide().on('click', function (event) {
            if ($(event.target).attr('id') === 'lightbox') {
                self.end();
            }
            return false;
        });

        this.$outerContainer.on('click', function (event) {
            if ($(event.target).attr('id') === 'lightbox') {
                self.end();
            }
            return false;
        });

        this.$lightbox.find('.lb-prev').on('click', function () {
            if (self.currentImageIndex === 0) {
                self.changeImage(self.album.length - 1);
            } else {
                self.changeImage(self.currentImageIndex - 1);
            }
            return false;
        });

        this.$lightbox.find('.lb-next').on('click', function () {
            if (self.currentImageIndex === self.album.length - 1) {
                self.changeImage(0);
            } else {
                self.changeImage(self.currentImageIndex + 1);
            }
            return false;
        });

        /*
          Show context menu for image on right-click
    
          There is a div containing the navigation that spans the entire image and lives above of it. If
          you right-click, you are right clicking this div and not the image. This prevents users from
          saving the image or using other context menu actions with the image.
    
          To fix this, when we detect the right mouse button is pressed down, but not yet clicked, we
          set pointer-events to none on the nav div. This is so that the upcoming right-click event on
          the next mouseup will bubble down to the image. Once the right-click/contextmenu event occurs
          we set the pointer events back to auto for the nav div so it can capture hover and left-click
          events as usual.
         */
        this.$nav.on('mousedown', function (event) {
            if (event.which === 3) {
                self.$nav.css('pointer-events', 'none');

                self.$lightbox.one('contextmenu', function () {
                    setTimeout(function () {
                        this.$nav.css('pointer-events', 'auto');
                    }.bind(self), 0);
                });
            }
        });


        this.$lightbox.find('.lb-loader, .lb-close').on('click', function () {
            self.end();
            return false;
        });
    };

    // Show overlay and lightbox. If the image is part of a set, add siblings to album array.
    Lightbox.prototype.start = function ($link) {
        var self = this;
        var $window = $(window);

        $window.on('resize', $.proxy(this.sizeOverlay, this));

        $('select, object, embed').css({
            visibility: 'hidden'
        });

        this.sizeOverlay();

        this.album = [];
        var imageNumber = 0;

        function addToAlbum($link) {
            self.album.push({
                alt: $link.attr('data-alt'),
                link: $link.attr('href'),
                title: $link.attr('data-title') || $link.attr('title')
            });
        }

        // Support both data-lightbox attribute and rel attribute implementations
        var dataLightboxValue = $link.attr('data-lightbox');
        var $links;

        if (dataLightboxValue) {
            $links = $($link.prop('tagName') + '[data-lightbox="' + dataLightboxValue + '"]');
            for (var i = 0; i < $links.length; i = ++i) {
                addToAlbum($($links[i]));
                if ($links[i] === $link[0]) {
                    imageNumber = i;
                }
            }
        } else {
            if ($link.attr('rel') === 'lightbox') {
                // If image is not part of a set
                addToAlbum($link);
            } else {
                // If image is part of a set
                $links = $($link.prop('tagName') + '[rel="' + $link.attr('rel') + '"]');
                for (var j = 0; j < $links.length; j = ++j) {
                    addToAlbum($($links[j]));
                    if ($links[j] === $link[0]) {
                        imageNumber = j;
                    }
                }
            }
        }

        // Position Lightbox
        var top = $window.scrollTop() + this.options.positionFromTop;
        var left = $window.scrollLeft();
        this.$lightbox.css({
            top: top + 'px',
            left: left + 'px'
        }).fadeIn(this.options.fadeDuration);

        // Disable scrolling of the page while open
        if (this.options.disableScrolling) {
            $('html').addClass('lb-disable-scrolling');
        }

        this.changeImage(imageNumber);
    };

    // Hide most UI elements in preparation for the animated resizing of the lightbox.
    Lightbox.prototype.changeImage = function (imageNumber) {
        var self = this;

        this.disableKeyboardNav();
        var $image = this.$lightbox.find('.lb-image');

        this.$overlay.fadeIn(this.options.fadeDuration);

        $('.lb-loader').fadeIn('slow');
        this.$lightbox.find('.lb-image, .lb-nav, .lb-prev, .lb-next, .lb-dataContainer, .lb-numbers, .lb-caption').hide();

        this.$outerContainer.addClass('animating');

        // When image to show is preloaded, we send the width and height to sizeContainer()
        var preloader = new Image();
        preloader.onload = function () {
            var $preloader;
            var imageHeight;
            var imageWidth;
            var maxImageHeight;
            var maxImageWidth;
            var windowHeight;
            var windowWidth;

            $image.attr({
                'alt': self.album[imageNumber].alt,
                'src': self.album[imageNumber].link
            });

            $preloader = $(preloader);

            $image.width(preloader.width);
            $image.height(preloader.height);

            if (self.options.fitImagesInViewport) {
                // Fit image inside the viewport.
                // Take into account the border around the image and an additional 10px gutter on each side.

                windowWidth = $(window).width();
                windowHeight = $(window).height();
                maxImageWidth = windowWidth - self.containerPadding.left - self.containerPadding.right - self.imageBorderWidth.left - self.imageBorderWidth.right - 20;
                maxImageHeight = windowHeight - self.containerPadding.top - self.containerPadding.bottom - self.imageBorderWidth.top - self.imageBorderWidth.bottom - 120;

                // Check if image size is larger then maxWidth|maxHeight in settings
                if (self.options.maxWidth && self.options.maxWidth < maxImageWidth) {
                    maxImageWidth = self.options.maxWidth;
                }
                if (self.options.maxHeight && self.options.maxHeight < maxImageWidth) {
                    maxImageHeight = self.options.maxHeight;
                }

                // Is the current image's width or height is greater than the maxImageWidth or maxImageHeight
                // option than we need to size down while maintaining the aspect ratio.
                if ((preloader.width > maxImageWidth) || (preloader.height > maxImageHeight)) {
                    if ((preloader.width / maxImageWidth) > (preloader.height / maxImageHeight)) {
                        imageWidth = maxImageWidth;
                        imageHeight = parseInt(preloader.height / (preloader.width / imageWidth), 10);
                        $image.width(imageWidth);
                        $image.height(imageHeight);
                    } else {
                        imageHeight = maxImageHeight;
                        imageWidth = parseInt(preloader.width / (preloader.height / imageHeight), 10);
                        $image.width(imageWidth);
                        $image.height(imageHeight);
                    }
                }
            }
            self.sizeContainer($image.width(), $image.height());
        };

        preloader.src = this.album[imageNumber].link;
        this.currentImageIndex = imageNumber;
    };

    // Stretch overlay to fit the viewport
    Lightbox.prototype.sizeOverlay = function () {
        this.$overlay
            .width($(document).width())
            .height($(document).height());
    };

    // Animate the size of the lightbox to fit the image we are showing
    Lightbox.prototype.sizeContainer = function (imageWidth, imageHeight) {
        var self = this;

        var oldWidth = this.$outerContainer.outerWidth();
        var oldHeight = this.$outerContainer.outerHeight();
        var newWidth = imageWidth + this.containerPadding.left + this.containerPadding.right + this.imageBorderWidth.left + this.imageBorderWidth.right;
        var newHeight = imageHeight + this.containerPadding.top + this.containerPadding.bottom + this.imageBorderWidth.top + this.imageBorderWidth.bottom;

        function postResize() {
            self.$lightbox.find('.lb-dataContainer').width(newWidth);
            self.$lightbox.find('.lb-prevLink').height(newHeight);
            self.$lightbox.find('.lb-nextLink').height(newHeight);
            self.showImage();
        }

        if (oldWidth !== newWidth || oldHeight !== newHeight) {
            this.$outerContainer.animate({
                width: newWidth,
                height: newHeight
            }, this.options.resizeDuration, 'swing', function () {
                postResize();
            });
        } else {
            postResize();
        }
    };

    // Display the image and its details and begin preload neighboring images.
    Lightbox.prototype.showImage = function () {
        this.$lightbox.find('.lb-loader').stop(true).hide();
        this.$lightbox.find('.lb-image').fadeIn(this.options.imageFadeDuration);

        this.updateNav();
        this.updateDetails();
        this.preloadNeighboringImages();
        this.enableKeyboardNav();
    };

    // Display previous and next navigation if appropriate.
    Lightbox.prototype.updateNav = function () {
        // Check to see if the browser supports touch events. If so, we take the conservative approach
        // and assume that mouse hover events are not supported and always show prev/next navigation
        // arrows in image sets.
        var alwaysShowNav = false;
        try {
            document.createEvent('TouchEvent');
            alwaysShowNav = (this.options.alwaysShowNavOnTouchDevices) ? true : false;
        } catch (e) { }

        this.$lightbox.find('.lb-nav').show();

        if (this.album.length > 1) {
            if (this.options.wrapAround) {
                if (alwaysShowNav) {
                    this.$lightbox.find('.lb-prev, .lb-next').css('opacity', '1');
                }
                this.$lightbox.find('.lb-prev, .lb-next').show();
            } else {
                if (this.currentImageIndex > 0) {
                    this.$lightbox.find('.lb-prev').show();
                    if (alwaysShowNav) {
                        this.$lightbox.find('.lb-prev').css('opacity', '1');
                    }
                }
                if (this.currentImageIndex < this.album.length - 1) {
                    this.$lightbox.find('.lb-next').show();
                    if (alwaysShowNav) {
                        this.$lightbox.find('.lb-next').css('opacity', '1');
                    }
                }
            }
        }
    };

    // Display caption, image number, and closing button.
    Lightbox.prototype.updateDetails = function () {
        var self = this;

        // Enable anchor clicks in the injected caption html.
        // Thanks Nate Wright for the fix. @https://github.com/NateWr
        if (typeof this.album[this.currentImageIndex].title !== 'undefined' &&
            this.album[this.currentImageIndex].title !== '') {
            var $caption = this.$lightbox.find('.lb-caption');
            if (this.options.sanitizeTitle) {
                $caption.text(this.album[this.currentImageIndex].title);
            } else {
                $caption.html(this.album[this.currentImageIndex].title);
            }
            $caption.fadeIn('fast')
                .find('a').on('click', function (event) {
                    if ($(this).attr('target') !== undefined) {
                        window.open($(this).attr('href'), $(this).attr('target'));
                    } else {
                        location.href = $(this).attr('href');
                    }
                });
        }

        if (this.album.length > 1 && this.options.showImageNumberLabel) {
            var labelText = this.imageCountLabel(this.currentImageIndex + 1, this.album.length);
            this.$lightbox.find('.lb-number').text(labelText).fadeIn('fast');
        } else {
            this.$lightbox.find('.lb-number').hide();
        }

        this.$outerContainer.removeClass('animating');

        this.$lightbox.find('.lb-dataContainer').fadeIn(this.options.resizeDuration, function () {
            return self.sizeOverlay();
        });
    };

    // Preload previous and next images in set.
    Lightbox.prototype.preloadNeighboringImages = function () {
        if (this.album.length > this.currentImageIndex + 1) {
            var preloadNext = new Image();
            preloadNext.src = this.album[this.currentImageIndex + 1].link;
        }
        if (this.currentImageIndex > 0) {
            var preloadPrev = new Image();
            preloadPrev.src = this.album[this.currentImageIndex - 1].link;
        }
    };

    Lightbox.prototype.enableKeyboardNav = function () {
        $(document).on('keyup.keyboard', $.proxy(this.keyboardAction, this));
    };

    Lightbox.prototype.disableKeyboardNav = function () {
        $(document).off('.keyboard');
    };

    Lightbox.prototype.keyboardAction = function (event) {
        var KEYCODE_ESC = 27;
        var KEYCODE_LEFTARROW = 37;
        var KEYCODE_RIGHTARROW = 39;

        var keycode = event.keyCode;
        var key = String.fromCharCode(keycode).toLowerCase();
        if (keycode === KEYCODE_ESC || key.match(/x|o|c/)) {
            this.end();
        } else if (key === 'p' || keycode === KEYCODE_LEFTARROW) {
            if (this.currentImageIndex !== 0) {
                this.changeImage(this.currentImageIndex - 1);
            } else if (this.options.wrapAround && this.album.length > 1) {
                this.changeImage(this.album.length - 1);
            }
        } else if (key === 'n' || keycode === KEYCODE_RIGHTARROW) {
            if (this.currentImageIndex !== this.album.length - 1) {
                this.changeImage(this.currentImageIndex + 1);
            } else if (this.options.wrapAround && this.album.length > 1) {
                this.changeImage(0);
            }
        }
    };

    // Closing time. :-(
    Lightbox.prototype.end = function () {
        this.disableKeyboardNav();
        $(window).off('resize', this.sizeOverlay);
        this.$lightbox.fadeOut(this.options.fadeDuration);
        this.$overlay.fadeOut(this.options.fadeDuration);
        $('select, object, embed').css({
            visibility: 'visible'
        });
        if (this.options.disableScrolling) {
            $('html').removeClass('lb-disable-scrolling');
        }
    };

    return new Lightbox();
}));

var searchByRegionMsg;

function showSearchByRegion() {
    var setting = {};
    setting.contentUrl = '/category/searchbyregionpopup?province='+
            (typeof initialProvince == 'undefined' ? -1 : initialProvince)+
            (typeof initialCity == 'undefined' ? -1 : initialCity) +
            (typeof initialArea == 'undefined' ? -1 : initialArea);
    var buttons = [{
        title: 'بستن',
        color: '#242424',
        bgColor: '#eaeaea',
        onclick: function () {
            searchByRegionMsg.closePopup();
        }
    },
    {
        title: 'انتخاب',
        color: '#242424',
        bgColor: '#fdd835',
        onclick: function () {
            doGeneralSearchRegion();
            searchByRegionMsg.closePopup();
        }
    }];
    setting.autoClose = false;
    setting.buttons = buttons;
    setting.color = '#4485F2';
    searchByRegionMsg = showMessagePopup('لیست شهر ها', '', setting);
}

function selectMostViewRegion(url, title) {
    currentSelectedRegion = {
        href: url,
        title: typeof title == 'undefined' ? '' : title
    };
    if (typeof isPortalHomePage == 'undefined' ||
        !isPortalHomePage) {
        doHomePageSearch();
    }
    $(".home-page__search-input").val(currentSelectedRegion.title);
    if (isMobileDevice) {
        toggleRegionSearchPopup(false);
    }
    else {
        toggleSearchListBox(false);
    }
}

function doGeneralSearchRegion() {
    var $province = $("select[name='generalSearchProvince']");
    var $city = $("select[name='generalSearchCity']");
    var $area = $("select[name='generalSearchArea']");

    myajax("category/regionsearchtourl", "province=" + $province.val() +
        "&city=" + $city.val() + "&area=" + $area.val(), function (ret) {
            if (ret.status == 0) {
                showErrorMessage(ret.msg);
            }
            else {
                currentSelectedRegion = {
                    href: ret.url,
                    title: ret.title
                };
                if (typeof isPortalHomePage == 'undefined' ||
                    !isPortalHomePage) {
                    doHomePageSearch();
                }
                $(".home-page__search-input").val(currentSelectedRegion.title);
                if (isMobileDevice) {
                    toggleRegionSearchPopup(false);
                }
                else {
                    toggleSearchListBox(false);
                }
            }
        }, false);
}
setTimeout(function(){ clearSearch(true) }, 500);
function doHomePageSearch() {
    if (currentSelectedRegion != undefined &&
        currentSelectedRegion.href == undefined) {
        currentSelectedRegion = undefined;
    }
    var regionHref = currentSelectedRegion == undefined ? "/ایران" : currentSelectedRegion.href;
    var fromDate = empty_range_from == undefined ? null : empty_range_from;
    var toDate = empty_range_to == undefined ? null : empty_range_to;
    var guestCount = currentGuestCount > 0 ? currentGuestCount.toString() : null;

    if (guestCount > 10) {
        guestCount = 11;
    }

    var firstQueryAdded = false;

    if (fromDate != null) {
        if (firstQueryAdded) {
            regionHref += "&";
        }
        else {
            regionHref += "?";
        }
        regionHref += "empty_range_from=" + fromDate;
        firstQueryAdded = true;
    }
    if (toDate != null) {
        if (firstQueryAdded) {
            regionHref += "&";
        }
        else {
            regionHref += "?";
        }
        regionHref += "empty_range_to=" + toDate;
        firstQueryAdded = true;
    }
    if (guestCount != null) {
        if (firstQueryAdded) {
            regionHref += "&";
        }
        else {
            regionHref += "?";
        }
        regionHref += "capacity=" + guestCount;
        firstQueryAdded = true;
    }
    window.open(regionHref, "_self");
}


$(".home-page__search-box").click(function () {
    //clearSearch();
    if (isMobileDevice) {
        toggleRegionSearchPopup(true);
        $('.home-page__search-input').focus();
    }
    else {
        toggleSearchListBox(true);
    }
    //search_catrgories();
});

function toggleRegionSearchPopup(toggle) {
    if (toggle) {
        $('#js-search-region-popup').show();
    }
    else {
        $('#js-search-region-popup').hide();
    }
}

$(document).click(function (e) {
    if ($(e.target).closest('.home-page__search-box').length === 0) {
        toggleSearchListBox(false);
    }
});

function toggleSearchListBox(toggle) {
    if (toggle) {
        $(".home-page__search-list-box").show();
        $(".home-page__search-box").css("border-bottom-right-radius", "0");
        $(".home-page__search-box").css("border-bottom-left-radius", "0");
    }
    else {
        $(".home-page__search-list-box:not('.js-dont-close')").hide();
        $(".home-page__search-box").css("border-bottom-right-radius", "");
        $(".home-page__search-box").css("border-bottom-left-radius", "");
    }
}

if ($(".home-page__search-input").val() != '') {
    toggleSearchHolder(false);
}

function openFirstSearchResult() {
    var href = $(".home-page__search-list-result-container").find("a:first").attr("href");
    if (href != null)
        window.location.href = href;
}

var currentSelectedRegion = undefined;
var currentGuestCount = 0;

function selectSearchRegion(elem) {
    if (typeof elem == 'undefined' || elem == null) {
        currentSelectedRegion = undefined;
    }
    else {
        currentSelectedRegion = {
            href: $(elem).attr('data-href'),
            title: $(elem).attr('data-title')
        };
        $(".home-page__search-input").val(currentSelectedRegion.title);
        toggleSearchListBox(false);
        doHomePageSearch();
    }
    if (isMobileDevice) {
        toggleRegionSearchPopup(false);
    }
}

function search_catrgories(e) {
    if ((e != null && e.keyCode === 13)) {
        var target = $(".home-page__search-list-result-container").find("div:first");
        if (target.length > 0) {
            selectSearchRegion(target[0]);
        }
        return;
    }
    var $input;
    if (isMobileDevice) {
        $input = $('.home-page__search-box-popup').find('.home-page__search-input');
    }
    else {
        $input = $(".home-page__search-input:visible");
    }
    var search_string = $input.val();

    //if (search_string == '') {
    //    toggleSearchHolder(true);
    //    $(".home-page__search-list-result-container").empty();
    //    selectSearchRegion(null);
    //    return;
    //}
    toggleSearchHolder(false);
    if (typeof search_string == 'undefined' || search_string == null || search_string == '') {
        return;
    }
    if (search_string != '' && /^[A-Za-z]*$/.test(search_string)) {
        //$(".home-page__search-list-result-container").empty();
        $(".home-page__search-input").val(search_string.replace(/[A-Za-z]/g, ""));
        alertify.error("لطفا فارسی تایپ کنید");
        return;
    }
    var url = "/Category/SearchCategory?search_string="
        + search_string +
        "&province=" + (typeof initialProvince == 'undefined' ? '-1' : initialProvince) +
        "&city=" + (typeof initialCity == 'undefined' ? '-1' : initialCity) +
        "&area=" + (typeof initialArea == 'undefined' ? '-1' : initialArea);
    $.get(url, function (data) {
        $(".home-page__search-list-result-container").html(data);
    });
    var target = $(".home-page__search-list-result-container").find("div:first");
    if (target.length > 0) {
        currentSelectedRegion = {
            href: $(target).attr('data-href'),
            title: $(target).attr('data-title')
        };
    }
}

function clearSearch(dontFocus) {
    $(".home-page__search-input").val("");
    if (!dontFocus)
        $(".home-page__search-input").focus();
    toggleSearchHolder(true);
    selectSearchRegion(null);
}

var search_holder_shown = true;

function toggleSearchHolder(toggle) {
    if (toggle) {
        $('#search_holder_root').show();
    }
    else {
        $('#search_holder_root').hide();
    }
    search_holder_shown = toggle;
}

$("span.holder").click(function () {
    $(".home-page__search-input").focus();
});

$(".home-page__search-input").click(search_catrgories);

$('.home-page__search-container').find('input').each(function () {
    if ($(this)[0].parentNode.tagName.toString().toLowerCase() == 'div') {
        $(this)[0].onfocus = function () {
            $(this).parent().addClass('focused-input');
        }
        $(this)[0].onblur = function () {
            $(this).parent().removeClass('focused-input');
        }
    }
});

for (i = 0; i < $('.home-page__search-container').length; i++) {
    // you can omit the 'if' if you want to style the parent node regardless of its
    // element type

}
var isMobileDevice = $('.body').width() < 681;

//if (/Android|webOS|iPhone|iPad|iPod|BlackBerry/i.test(navigator.userAgent)) {
//    isMobileDevice = true;
//}

/*
  Jalaali years starting the 33-year rule.
*/
var breaks =  [ -61, 9, 38, 199, 426, 686, 756, 818, 1111, 1181, 1210
  , 1635, 2060, 2097, 2192, 2262, 2324, 2394, 2456, 3178
  ]

/*
  Converts a Gregorian date to Jalaali.
*/

function toJalaali(gDate) {
    return toJalaali(gDate.getFullYear(), gDate.getMonth(), gDate.getDate());
}

function toJalaali(gy, gm, gd) {
  if (Object.prototype.toString.call(gy) === '[object Date]') {
    gd = gy.getDate()
    gm = gy.getMonth() + 1
    gy = gy.getFullYear()
  }
  return d2j(g2d(gy, gm, gd))
}

/*
  This function determines if the Jalaali (Persian) year is
  leap (366-day long) or is the common year (365 days), and
  finds the day in March (Gregorian calendar) of the first
  day of the Jalaali year (jy).
  @param jy Jalaali calendar year (-61 to 3177)
  @param withoutLeap when don't need leap (true or false) default is false
  @return
    leap: number of years since the last leap year (0 to 4)
    gy: Gregorian year of the beginning of Jalaali year
    march: the March day of Farvardin the 1st (1st day of jy)
  @see: http://www.astro.uni.torun.pl/~kb/Papers/EMP/PersianC-EMP.htm
  @see: http://www.fourmilab.ch/documents/calendar/
*/
function jalCal(jy, withoutLeap) {  
  var bl = breaks.length
    , gy = jy + 621
    , leapJ = -14
    , jp = breaks[0]
    , jm
    , jump
    , leap
    , leapG
    , march
    , n
    , i

  if (jy < jp || jy >= breaks[bl - 1])
    throw new Error('Invalid Jalaali year ' + jy)

  // Find the limiting years for the Jalaali year jy.
  for (i = 1; i < bl; i += 1) {
    jm = breaks[i]
    jump = jm - jp
    if (jy < jm)
      break
    leapJ = leapJ + div(jump, 33) * 8 + div(mod(jump, 33), 4)
    jp = jm
  }
  n = jy - jp

  // Find the number of leap years from AD 621 to the beginning
  // of the current Jalaali year in the Persian calendar.
  leapJ = leapJ + div(n, 33) * 8 + div(mod(n, 33) + 3, 4)
  if (mod(jump, 33) === 4 && jump - n === 4)
    leapJ += 1

  // And the same in the Gregorian calendar (until the year gy).
  leapG = div(gy, 4) - div((div(gy, 100) + 1) * 3, 4) - 150

  // Determine the Gregorian date of Farvardin the 1st.
  march = 20 + leapJ - leapG

  // return with gy and march when we don't need leap
  if (withoutLeap) return { gy: gy, march: march };


  // Find how many years have passed since the last leap year.
  if (jump - n < 6)
    n = n - jump + div(jump + 4, 33) * 33
  leap = mod(mod(n + 1, 33) - 1, 4)
  if (leap === -1) {
    leap = 4
  }  

  return  { leap: leap
          , gy: gy
          , march: march
          }
}

function j2d(jy, jm, jd) {
  var r = jalCal(jy, true)
  return g2d(r.gy, 3, r.march) + (jm - 1) * 31 - div(jm, 7) * (jm - 7) + jd - 1
}

function d2j(jdn) {
  var gy = d2g(jdn).gy // Calculate Gregorian year (gy).
    , jy = gy - 621
    , r = jalCal(jy, false)
    , jdn1f = g2d(gy, 3, r.march)
    , jd
    , jm
    , k

  // Find number of days that passed since 1 Farvardin.
  k = jdn - jdn1f
  if (k >= 0) {
    if (k <= 185) {
      // The first 6 months.
      jm = 1 + div(k, 31)
      jd = mod(k, 31) + 1
      return  { jy: jy
              , jm: jm
              , jd: jd
              }
    } else {
      // The remaining months.
      k -= 186
    }
  } else {
    // Previous Jalaali year.
    jy -= 1
    k += 179
    if (r.leap === 1)
      k += 1
  }
  jm = 7 + div(k, 30)
  jd = mod(k, 30) + 1
  return  { jy: jy
          , jm: jm
          , jd: jd
          }
}
function g2d(gy, gm, gd) {
  var d = div((gy + div(gm - 8, 6) + 100100) * 1461, 4)
      + div(153 * mod(gm + 9, 12) + 2, 5)
      + gd - 34840408
  d = d - div(div(gy + 100100 + div(gm - 8, 6), 100) * 3, 4) + 752
  return d
}
function d2g(jdn) {
  var j
    , i
    , gd
    , gm
    , gy
  j = 4 * jdn + 139361631
  j = j + div(div(4 * jdn + 183187720, 146097) * 3, 4) * 4 - 3908
  i = div(mod(j, 1461), 4) * 5 + 308
  gd = div(mod(i, 153), 5) + 1
  gm = mod(div(i, 153), 12) + 1
  gy = div(j, 1461) - 100100 + div(8 - gm, 6)
  return  { gy: gy
          , gm: gm
          , gd: gd
          }
}
function div(a, b) {
  return ~~(a / b)
}

function mod(a, b) {
  return a - ~~(a / b) * b
}
var
persianNumbers = [/۰/g, /۱/g, /۲/g, /۳/g, /۴/g, /۵/g, /۶/g, /۷/g, /۸/g, /۹/g],
arabicNumbers = [/٠/g, /١/g, /٢/g, /٣/g, /٤/g, /٥/g, /٦/g, /٧/g, /٨/g, /٩/g];
persianDigits = ['۰', '۱', '۲', '۳', '۴', '۵', '۶', '۷', '۸', '۹'],

jalaliWeekDays = ['شنبه', 'یکشنبه', 'دوشنبه', 'سه شنبه', 'چهارشنبه', 'پنجشنبه', 'جمعه'];
jalaliMonthNames = ['فروردین', 'اردیبهشت', 'خرداد', 'تیر', 'مرداد', 'شهریور', 'مهر', 'آبان', 'آذر', 'دی', 'بهمن', 'اسفند'];
jalaliWeekDaysShort = ['ش', 'ی', 'د', 'س', 'چ', 'پ', 'ج'];

var jalaliHolidays = ["1400/1/1", "1400/1/2", "1400/1/3",
                       "1400/1/4", "1400/1/9", "1400/1/12",
                       "1400/2/14", "1400/2/23", "1400/3/15",
                       "1400/3/16", "1400/4/30", "1400/5/7",
                       "1400/5/27", "1400/5/28", "1400/7/5",
                       "1400/7/13", "1400/7/15",
                       "1400/8/2", "1400/10/16", "1400/11/26",
                        "1400/12/10", "1400/12/29",
                        "1401/1/1", "1401/1/2", "1401/1/3",
                        "1401/1/4", "1401/1/13", "1401/2/12",
                        "1401/2/13", "1401/3/5", "1401/3/14",
                        "1401/3/15", "1401/4/18", "1401/4/26",
                        "1401/5/16", "1401/5/17", "1401/7/2",
                        "1401/7/4", "1401/7/12", "1401/7/21",
                        "1401/10/6", "1401/11/15", "1401/11/22",
                        "1401/11/29", "1401/12/16", "1401/12/29"];


String.prototype.replaceAll = function (search, replacement) {
    var target = this;
    return target.replace(new RegExp(search, 'g'), replacement);
};

function persianToEnglishNumber (str) {
    if (typeof str === 'string') {
        for (var i = 0; i < 10; i++) {
            str = str.replace(persianNumbers[i], i).replace(arabicNumbers[i], i);
        }
    }
    return str;
};

function englishToPersianNumber(str) {
    str = str.toString();
    for (var i = 0; i < 10; i++) {
        str = str.replaceAll(i.toString(), persianDigits[i]);
    }
    return str;
};

var pastDayOffset;

function gregorianToJalaliDate(gDate) {
    var gDateClone = new Date(gDate.getTime());
    pastDayOffset = pastDayOffset == undefined ? 0 : pastDayOffset;
    var gToday = new Date();
    var pastDayBoundary = new Date();
    if (pastDayOffset != 0) {
        pastDayBoundary.setDate(pastDayBoundary.getDate() + pastDayOffset);
    }
    gToday.setHours(0, 0, 0, 0);

    pastDayBoundary.setHours(0, 0, 0, 0);
    //var localDateString = gDate.toLocaleDateString('fa-IR');
    //var dateStringEnglishDigit = persianToEnglishNumber(localDateString);
    //dateStringEnglishDigit = dateStringEnglishDigit.replace(/[^\/\d]/g, '');
    //if (parseInt(dateStringEnglishDigit.split('/')[0]) > 1900) {
        var convertedDate = toJalaali(gDate);
        var dateStringEnglishDigit = convertedDate.jy + '/' + convertedDate.jm + '/' + convertedDate.jd;
    //}
    var dateStringPersianDigit = englishToPersianNumber(dateStringEnglishDigit);
    let gregorianDayOfWeek = gDate.getDay();
    let jalaliDayOfWeek = gregorianDayOfWeek == 6 ? 0 : (gregorianDayOfWeek + 1);
    var jalaliDateSplit = dateStringEnglishDigit.split('/');
    var jYear = parseInt(jalaliDateSplit[0]);
    var jMonth = parseInt(jalaliDateSplit[1]);
    var jDay = parseInt(jalaliDateSplit[2]);
    gDateClone.setHours(0, 0, 0, 0);
    return {
        year: jYear,
        month: jMonth,
        day: jDay,
        monthString: jalaliMonthNames[jMonth - 1],
        dayOfWeek: jalaliDayOfWeek,
        dayOfWeekString: jalaliWeekDays[jalaliDayOfWeek],
        dayOfWeekStringShort: jalaliWeekDaysShort[jalaliDayOfWeek],
        dateStringPersian: dateStringPersianDigit,
        dateString: dateStringEnglishDigit,
        gregorianDate: new Date(gDate.getTime()),
        isToday: gDate.toDateString() === gToday.toDateString(),
        isPastDay: gDate < pastDayBoundary,
        isHoliday: jalaliDayOfWeek == 6 || jalaliHolidays.includes(dateStringEnglishDigit),
        value: jYear == 1400 && jMonth == 1 && jDay == 2 ? 1616358600000 : gDateClone.valueOf()
    };
}

function getJalaliToday() {
    return gregorianToJalaliDate(new Date());
}

function getJalaliTomorrow() {
    var date = new Date();
    date.setDate(date.getDate() + 1);
    return gregorianToJalaliDate(date);
}

function getJalaliYesterday() {
    var date = new Date();
    date.setDate(date.getDate() - 1);
    return gregorianToJalaliDate(date);
}

function getJalaliMonthDays(jDate) {
    var gDate = jDate.gregorianDate;
    var dayList = [];
    var gPrevDay = new Date(gDate.getTime());
    var gNextDay = new Date(gDate.getTime());
    if (getJalaliToday().month > 6) {
        gNextDay.addHours(1);
    }
    if (jDate.day > 1) {
        while (true) {
            gPrevDay.setDate(gPrevDay.getDate() - 1);
            var jPrevDay = gregorianToJalaliDate(gPrevDay);
            dayList.unshift(jPrevDay);
            if (jPrevDay.day <= 1) {
                break;
            }
        }
    }
    dayList.push(gregorianToJalaliDate(gDate));
    while (true) {
        gNextDay.setDate(gNextDay.getDate() + 1);
        var jNextDay = gregorianToJalaliDate(gNextDay);
        var jDay = jNextDay.day;
        if (jDay > 1) {
            dayList.push(jNextDay);
        }
        else {
            break;
        }
    }
    return dayList;
}

function getJalaliMonthWeeksCount(dayList) {
    var count = 0;
    for (var i = 0; i < dayList.length; i++) {
        if (dayList[i].dayOfWeek == 6) {
            count++;
        }
        else if (i == dayList.length - 1) {
            count++;
        }
    }
    return count;
}

function getNextMonthJalali(jDate) {
    var gNextDay = new Date(jDate.gregorianDate.getTime());
    while (true) {
        gNextDay.setDate(gNextDay.getDate() + 1);
        var jNextDay = gregorianToJalaliDate(gNextDay);
        var jDay = jNextDay.day;
        if (jDay < 2) {
            var result = gregorianToJalaliDate(gNextDay);
            return result;
        }
    }
}

function getPreviousMonthJalali(jDate) {
    var gPrevDay = new Date(jDate.gregorianDate.getTime());
    while (true) {
        gPrevDay.setDate(gPrevDay.getDate() - 1);
        var jPrevDay = gregorianToJalaliDate(gPrevDay);
        if (jDate.month != jPrevDay.month &&
            jPrevDay.day < 2) {
            var result = gregorianToJalaliDate(gPrevDay);
            return result;
        }
    }
}


function getDiffDays(date1, date2) {
    const diffTime = Math.abs(date2 - date1);
    return Math.ceil(diffTime / (1000 * 60 * 60 * 24));
}

function diffDaysMs(date1_ms, date2_ms) {
    var one_day = 1000 * 60 * 60 * 24;
    var difference_ms = date2_ms - date1_ms;
    var days = Math.ceil(difference_ms / one_day);
    return days;
}
var jalaliToday = getJalaliToday();
var jalaliCurrentMonth = jalaliToday;

var currentSelectedDay;
var firstSelectedDay;
var secondSelectedDay;
var firstOccupiedValue;

//elems: date picker elements
//month: initial jalali month
//setting: priceDict (dict), occupiedList (arr),
//monthOffset (int), selectionType (single, multi),
//occupiedSelectEnabled (bool), fromDateLabel (elem)
//toDateLabel (elem), onUpdateDate(function)
//maxSupportedMonth (jalali month)

function updateDatePicker(elems, month, setting) {
    updateDatePickerSeek(elems, month, setting);
    updateMonthYearLabel(elems, month);
    updateDatePickerDays(elems, month);
    updateDatePickerPrices(elems, setting);
    updateDatePickerOccupied(elems, setting);
    if (firstSelectedDay != undefined) {
        updateRangeSelection(elems, setting);
    }
    return setting;
}

function selectDate(elems, dayElem, setting) {
    var $dayElem = $(dayElem);
    if ($dayElem.hasClass('jalali-disabled-day')) {
        return;
    }
    var value = parseInt($dayElem.attr('data-value'));
    price = setting.priceDict[value];
    if (price == undefined) {
        price = {price: 0, off: 0};
    }
    var dayObj = {
        date: $dayElem.attr('data-date'),
        value: value,
        elem: dayElem,
        occupied: setting.occupiedList && setting.occupiedList.includes(value),
        price: price
    }
    if (setting.selectionType == 'multi') {
        selectDateMulti(elems, dayObj, setting);
        return;
    }
    selectDateSignle(dayObj, setting);
}
 
function selectDateSignle(dayObj, setting) {
    if (currentSelectedDay) {
        $(currentSelectedDay.elem).removeClass('jalali-selected-day');
    }
    if (currentSelectedDay != undefined &&
        currentSelectedDay.value == dayObj.value) {
        currentSelectedDay = undefined;
    }
    else {
        currentSelectedDay = dayObj;
        $(dayObj.elem).addClass('jalali-selected-day');
    }
    if (setting.onUpdateDate) {
        setting.onUpdateDate();
    }
}

function selectDateMulti(elems, dayObj, setting) {
    if ((firstSelectedDay == undefined &&
        secondSelectedDay == undefined) ||
        (firstSelectedDay != undefined &&
        secondSelectedDay != undefined))
    {
        secondSelectedDay = undefined;
        firstSelectedDay = dayObj;
        firstOccupiedValue = Math.min.apply(null, setting.occupiedList.filter(function (x) { return x > firstSelectedDay.value }));
        if (setting.fromDateLabel != undefined) {
            $(setting.fromDateLabel).html(dayObj.date.substring(2));
        }
        if (setting.toDateLabel != undefined) {
            $(setting.toDateLabel).html('');
        }
    }
    else {
        if (dayObj.value <= firstSelectedDay.value) {
            firstSelectedDay = undefined;
            if (setting.fromDateLabel != undefined) {
                $(setting.fromDateLabel).html('');
            }
        }
        else {
            secondSelectedDay = dayObj;
            if (setting.toDateLabel != undefined) {
                $(setting.toDateLabel).html(dayObj.date.substring(2));
            }
        }
        firstOccupiedValue = undefined;
    }
    updateRangeSelection(elems, setting);
    if (setting.onUpdateDate) {
        setting.onUpdateDate();
    }
}

function updateRangeSelection(elems, setting) {
    var maxValue = Number.MAX_VALUE;
    if (!setting.occupiedSelectEnabled &&
        firstOccupiedValue != undefined) {
        maxValue = firstOccupiedValue;
    }
    elems.forEach(function (elem) {
        $(elem).find('.jalali-real-day').each(function () {
            var value = parseInt($(this).attr('data-value'));
            if (firstSelectedDay == undefined) {
                $(this).removeClass('jalali-selected-day');
                if (($(this).hasClass('jalali-occupied-day'))) {
                    if (!setting.occupiedSelectEnabled &&
                        !($(this).hasClass('jalali-disabled-day'))) {
                        $(this).addClass('jalali-disabled-day');
                    }
                }
                else {
                    if (!($(this).hasClass('jalali-past-day'))) {
                        $(this).removeClass('jalali-disabled-day');
                    }
                }
                if ($(this).hasClass('jalali-occupied-day-temp')) {
                    $(this).addClass('jalali-occupied-day');
                    $(this).addClass('jalali-disabled-day');
                    $(this).removeClass('alali-occupied-day-temp');
                    $(this).find('.jalali-reserved-label').remove();
                    $(this).append('<span class="jalali-reserved-label">رزرو شده</span>');
                    $(this).off('click');
                    $(this).off('hover');
                }
            }
            else {
                if (secondSelectedDay == undefined) {
                    if (value == firstSelectedDay.value) {
                        $(this).addClass('jalali-selected-day');
                    }
                    else {
                        $(this).removeClass('jalali-selected-day');
                    }
                    if (!setting.occupiedSelectEnabled) {
                        if (value == maxValue) {
                            $(this).removeClass('jalali-occupied-day');
                            $(this).removeClass('jalali-disabled-day');
                            $(this).find('.jalali-reserved-label').remove();
                            $(this).find('.jalali-price-label').remove();

                            price = setting.priceDict[$(this).attr('data-value')];
                            if (price) {
                                $(this).append('<span class="jalali-price-label">' + priceToSpecialString(price.price) + '</span>');
                            }
                            $(this).click(function () {
                                selectDate(elems, this, setting);
                            });
                            $(this).hover(function () {
                                handleDayHover(elems, this, setting);
                            });
                            $(this).addClass('jalali-occupied-day-temp');
                        }
                        else if (value > maxValue) {
                            $(this).addClass('jalali-disabled-day');
                        }
                    }
                }
                else {
                    if (value >= firstSelectedDay.value &&
                        value <= secondSelectedDay.value) {
                        $(this).addClass('jalali-selected-day');
                    }
                    else {
                        $(this).removeClass('jalali-selected-day');
                    }
                    if (($(this).hasClass('jalali-occupied-day'))) {
                        if (!setting.occupiedSelectEnabled &&
                            !($(this).hasClass('jalali-disabled-day'))) {
                            $(this).addClass('jalali-disabled-day');
                        }
                    }
                    else {
                        if (!($(this).hasClass('jalali-past-day'))) {
                            $(this).removeClass('jalali-disabled-day');
                        }
                    }
                    if ($(this).hasClass('jalali-occupied-day-temp')) {
                        $(this).addClass('jalali-disabled-day');
                        $(this).off('click');
                        $(this).off('hover');
                    }
                }
            }
        });
    })
}

function handleDayHover(elems, dayElem, setting) {
    if (setting.selectionType != 'multi' ||
        !(firstSelectedDay != undefined &&
          secondSelectedDay == undefined)) {
        elems.forEach(function (elem) {
            $(elem).find('.jalali-real-day').removeClass('jalali-range-candidate-day');
        });
        return;
    }
    var value = parseInt($(dayElem).attr('data-value'));
    if (value <= firstSelectedDay.value) {
        elems.forEach(function (elem) {
            $(elem).find('.jalali-real-day').removeClass('jalali-range-candidate-day');
        });
    }
    elems.forEach(function (elem) {
        $(elem).find('.jalali-real-day').each(function () {
            var dayValue = parseInt($(this).attr('data-value'));
            if (dayValue < value && dayValue > firstSelectedDay.value &&
                !($(this).hasClass('jalali-disabled-day'))) {
                $(this).addClass('jalali-range-candidate-day');
            }
            else {
                $(this).removeClass('jalali-range-candidate-day');
            }
        });
    });
}

function updateMonthYearLabel(elems, month) {
    elems.forEach(function (elem) {
        var $elem = $(elem);
        $elem.find('.jalali-year-label').html(month.monthString + ' ' + englishToPersianNumber(month.year));
    });
}

function updateDatePickerSeek(elems, month, setting) {
    elems.forEach(function (elem) {
        var $elem = $(elem);
        var $prevBtn = $elem.find('.js-prev-month-btn');
        if (month.year < jalaliCurrentMonth.year ||
            (month.year == jalaliCurrentMonth.year &&
            month.month <= jalaliCurrentMonth.month)) {
            $prevBtn.css('color', '#f4f4f4');
            $prevBtn.css('borderColor', '#f4f4f4');
            $prevBtn.off('mouseup');
        }
        else {
            $prevBtn.css('color', '#242424');
            $prevBtn.css('borderColor', '#242424');
            $prevBtn.mouseup(function () {
                $(this).off("mouseup");
                $elem.find('.js-next-month-btn').off('mouseup');
                var prevMonth = getPreviousMonthJalali(month);
                setting.monthOffset--;
                updateDatePicker(elems, prevMonth, setting);
            });
        }
        var $nextBtn = $elem.find('.js-next-month-btn');
        if (setting.maxSupportedMonth != undefined &&
            (month.year > setting.maxSupportedMonth.year ||
            (month.year == setting.maxSupportedMonth.year &&
            month.month >= setting.maxSupportedMonth.month))) {
            $nextBtn.css('color', '#f4f4f4');
            $nextBtn.css('borderColor', '#f4f4f4');
            $nextBtn.off('mouseup');
        }
        else {
            $nextBtn.css('color', '#242424');
            $nextBtn.css('borderColor', '#242424');
            $nextBtn.mouseup(function () {
                $(this).off("mouseup");
                $elem.find('.js-prev-month-btn').off('mouseup');
                var nextMonth = getNextMonthJalali(month);
                setting.monthOffset++;
                updateDatePicker(elems, nextMonth, setting);
            });
        }
    });
}

function updateDatePickerDays(elems, month) {
    elems.forEach(function (elem) {
        var $elem = $(elem);
        var $monthElem = $elem.find('.jalali-month');
        $monthElem.empty();
        var days = getJalaliMonthDays(month);
        var currWeek = 0;

        if (days[0].dayOfWeek > 0) {
            currWeek++;
            $monthElem.append('<div class="jalali-week js-week-' + currWeek + '"></div>');
            for (var i = 0; i < days[0].dayOfWeek; i++) {
                $monthElem.find('.js-week-' + currWeek).append('<div class="jalali-day jalali-empty-day"></div>');
            }
        }
        days.forEach(function (item, index) {
            if (item.dayOfWeek == 0) {
                currWeek++;
                $monthElem.append('<div class="jalali-week js-week-' + currWeek + '"></div>');
            }
            $monthElem.find('.js-week-' + currWeek).append('<div data-value="' + item.value + '" data-date="' + item.dateString + '" class="jalali-day jalali-real-day' +
                (item.isPastDay ? ' jalali-past-day' : '') +
                (item.isToday ? ' jalali-today' : '') +
                (item.isPastDay ? ' jalali-disabled-day' : '') +
                (item.isHoliday && !item.isPastDay ? ' jalali-holiday' : '') +
                '">' +
                '<span>' + englishToPersianNumber(item.day) + '</span>' +
                '</div>');
        });
        for (var i = days[days.length - 1].dayOfWeek + 1; i < 7; i++) {
            $monthElem.find('.js-week-' + currWeek).append('<div class="jalali-day jalali-empty-day"></div>');
        }
        while (currWeek < 6) {
            currWeek++;
            $monthElem.append('<div class="jalali-week js-week-' + currWeek + '"></div>');
            for (var i = 0; i < 7; i++) {
                $monthElem.find('.js-week-' + currWeek).append('<div class="jalali-day jalali-empty-day"></div>');
            }
        }
    });
}

function updateDatePickerPrices(elems, setting) {
    if (setting.priceDict == undefined) {
        return;
    }
    elems.forEach(function (elem) {
        var $elem = $(elem);
        var $monthElem = $elem.find('.jalali-month');
        $monthElem.find('.jalali-price-label').remove();
        var price;
        $monthElem.find('.jalali-real-day').each(function () {
            price = setting.priceDict[$(this).attr('data-value')];
            if (price) {
                $(this).append('<span class="jalali-price-label">' + priceToSpecialString(price.price) + '</span>');
            }
        });
    });
}

function updateDatePickerOccupied(elems, setting) {
    elems.forEach(function (elem) {
        var $elem = $(elem);
        var $monthElem = $elem.find('.jalali-month');
        var $occDays = $monthElem.find('.jalali-occupied-day')
        $occDays.removeClass('jalali-occupied-day');
        $occDays.removeClass('jalali-disabled-day');
        var occupied;
        $monthElem.find('.jalali-real-day').each(function () {
            var value = parseInt($(this).attr('data-value'));
            occupied = setting.occupiedList && setting.occupiedList.includes(value);
            var isHostPanel = false;
            var isExtrinsic = false;
            if (setting.extrinsicList != undefined) {
                isHostPanel = true;
                isExtrinsic = occupied && setting.extrinsicList.includes(value);
            }
            if (occupied) {
                $(this).find('.jalali-price-label').hide();
                if (isExtrinsic) {
                    $(this).append('<span class="jalali-filled-label">پر شده</span>');
                }
                else {
                    if (isHostPanel) {
                        $(this).append('<span class="jalali-reserved-label-host">رزرو شده</span>');
                    }
                    else {
                        $(this).append('<span class="jalali-reserved-label">رزرو شده</span>');
                    }
                }
                $(this).addClass('jalali-occupied-day');
                if (!setting.occupiedSelectEnabled) {
                    $(this).addClass('jalali-disabled-day');
                }
                $(this).off('click');
                $(this).off('hover');
            }
            else {
                $(this).find('.jalali-price-label').show();
            }
            if (!occupied || setting.occupiedSelectEnabled) {
                $(this).click(function () {
                    selectDate(elems, this, setting);
                });
                $(this).hover(function () {
                    handleDayHover(elems, this, setting);
                });
                if (setting.selectionType === 'multi') {
                    if (firstSelectedDay && firstSelectedDay.value === value) {
                        $(this).addClass('jalali-selected-day');
                        firstSelectedDay.elem = this;
                    }
                    if (secondSelectedDay && secondSelectedDay.value === value) {
                        $(this).addClass('jalali-selected-day');
                        secondSelectedDay.elem = this;
                    }
                    updateRangeSelection(elems, setting);
                }
                else {
                    if (currentSelectedDay && currentSelectedDay.value === value) {
                        $(this).addClass('jalali-selected-day');
                        currentSelectedDay.elem = this;
                    }
                }
            }
        });
    });
}

function jalaliShowLoading() {
    $('.jalali-loading').css('display', 'flex');
}

function jalaliHideLoading() {
    $('.jalali-loading').css('display', 'none');
}

function priceToSpecialString(price) {
    var priceStr = price.toString();
    priceStr = priceStr.slice(0, -3);
    return priceStr;
}
function showRatingDetail(id, userid) {
    var url = "/accomodation/userratingdetailpopup?id=" + id + "&userid=" + userid;
    showInfoMessage('', '', { contentUrl: url });
}
function getPriceString(price_val) {
    str_toman = "";
    if (price_val >= 1000000000) {
        str_toman += Math.floor(price_val / 1000000000) + " میلیارد";
        price_val = price_val % 1000000000;
    }
    if (price_val >= 1000000) {
        if (str_toman.length > 3) {
            str_toman += " و " + Math.floor(price_val / 1000000) + " میلیون";
        }
        else {
            str_toman += Math.floor(price_val / 1000000) + " میلیون";
        }
        price_val = price_val % 1000000;
    }
    if (price_val >= 1000) {
        if (str_toman.length > 3) {
            str_toman += " و " + Math.floor(price_val / 1000) + " هزار";
        }
        else {
            str_toman += Math.floor(price_val / 1000) + " هزار";
        }
        price_val = price_val % 1000;

    }
    if (price_val > 0) {

        if (str_toman.length > 3) {
            str_toman += " و " + price_val;
        }
        else {
            str_toman += price_val;
        }
    }

    return str_toman + " تومان";
}

function getPriceThousandSeperatorStr(price) {
    return price.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
}
var user_is_autenticated = false;
var isNumberForIran = false;
var verifyEmail = false;
var userEmailAddress = "";
if (!(new Date().getHours() > 3)) {
    pastDayOffset = -1;
}
function showReservePopup(advertise_id) {
    firstSelectedDay = undefined;
    secondSelectedDay = undefined;
    $('#reserve_popup').slideUp(500, function () {
        $('#reserve_popup_container').empty();
        $('#reserve_popup_container').load(
            '../../../accomodation/getreservepopup?accomodationid=' + advertise_id,
            function () {
                updateReserveInfo(advertise_id, function () {
                    $('#reserve_popup').slideDown(500);
                    $('.advertise-page__reserve-container').show();
                });
                $(document).on("change", "#guest_count", function () {
                    onChangeGuestCount(this);
                });
            });
    });
}

function clickGuestIncDec(elem) {
    onClickIncDecButton(elem);
    onChangeGuestCount();
}

function onChangeGuestCount(elem) {
    var maxGuestCount = parseInt($(elem).attr('max'));
    if (parseInt($(elem).val()) > maxGuestCount) {
        alertify.error("حداکثر ظرفیت این اقامتگاه " + maxGuestCount + " نفر است");
        $(elem).val($(elem).attr('max'));
    }
    updateReservePrice();
    updateReserveLabels();
}

function moreComments() {
    if (shownCommentsCount < commentsCount) {
        var nextItemsCount = Math.min(commentsCount - shownCommentsCount, 3);
        var $lastShown = $('.js-comment-last-shown').first();
        $lastShown.removeClass('js-comment-last-shown');
        for (var i = 0; i < nextItemsCount; i++) {
            $lastShown = $lastShown.next();
            $lastShown.show();
            if (i == nextItemsCount - 1) {
                $lastShown.addClass('js-comment-last-shown')
            }
        }
        shownCommentsCount += nextItemsCount;
    }
    if (shownCommentsCount >= commentsCount) {
        $('#js-more-comment-btn').hide();
    }
}

var sharePostState = false;

$(document).mouseup(function (e) {
    var container = $('#main-date-picker');

    // if the target of the click isn't the container nor a descendant of the container
    if (!container.is(e.target) && container.has(e.target).length === 0) {
        if (firstSelectedDay != undefined &&
            secondSelectedDay == undefined) {
            $(firstSelectedDay.elem.nextSibling).click();
            onUpdateDate();
        }
        container.hide();
    }

    container = $('.share-post__container');
    if (!container.is(e.target) && container.has(e.target).length === 0) {
        if (sharePostState) {
            sharePostState = false;
            $('.share-post__container').slideUp();
        }
    }
});

$(".share-post__button").on("click", function () {
    if (!sharePostState) {
        sharePostState = true;
        $(".share-post__container").slideDown();
        $('.share-post__container').css('display', 'flex');
    }
});

function hideReservePopup() {
    $('.datepicker-container').remove();
    $('#reserve_popup').slideUp(500);
}
function updateReserveInfo(advertise_id, onDone) {
    jalaliShowLoading();
    myajax('reserve/getreserveinfo', 'accommodation_id=' + advertise_id, function (ret) {
        jalaliHideLoading();
        fillReserveInfo(advertise_id, ret.occupiedList, ret.priceDict,
            rules_string, onDone);
        date_price_dict = ret.priceDict;
    });
}

function hideReserveContainer() {
    $('.advertise-page__reserve-container').slideUp(200);
    hideReservePopup();
}

function showReserveContainer() {
    $('.advertise-page__reserve-container').slideDown(200);
}

function showDatePicker() {
    $('#main-date-picker').show();
}

function fillReserveInfo(id, occupiedList, priceDict, rules_str, onDone) {
    rules_string = rules_str;
    advertise_id = id;
    if (onDone != undefined && onDone != null) {
        onDone();
    }
    updateDatePicker([$('#main-date-picker')[0]], jalaliCurrentMonth,
        {
            priceDict: priceDict,
            occupiedList: occupiedList,
            monthOffset: 0,
            selectionType: 'multi',
            occupiedSelectEnabled: false,
            fromDateLabel: $('#js-from-date-label')[0],
            toDateLabel: $('#js-to-date-label')[0],
            onUpdateDate: function () {
                if (firstSelectedDay != undefined && secondSelectedDay != undefined) {
                    $('#main-date-picker').hide();
                    var guestCount = $("#guest_count").val();
                    if (guestCount < 1) {
                        showGuestCountSelect();
                    }
                }
                onUpdateDate();
            }
        });
}

function showGuestCountSelect() {
    $("#guest_count").focus();
}

function checkReserve(confirm_required) {
    if (firstSelectedDay == undefined ||
        secondSelectedDay == undefined) {
        showDatePicker();
        return;
    }
    var guestCount = $("#guest_count").val();
    if (guestCount < 1) {
        showGuestCountSelect();
    }
    var from_date = firstSelectedDay.date.replaceAll('/', ',');
    var to_date = secondSelectedDay.date.replaceAll('/', ',');
    myajax("reserve/checkreserve", "advertise_id=" + advertise_id +
        "&from_date=" + from_date + "&to_date=" + to_date +
        "&number_of_guests=" + guestCount, function (ret) {
            if (ret.val == 1) {
                var date_string = 'از ' + firstSelectedDay.date +
                    ' تا ' + secondSelectedDay.date +
                    ' به مدت ' + diffDaysMs(firstSelectedDay.value, secondSelectedDay.value) + ' شب';
                var guest_number_string = 'تعداد نفرات: ' + $("#guest_count").val() + ' نفر';
                price_string = $("#reserve_price_label").html();
                var time_string = 'ساعت ورود: 2 بعداز ظهر - ساعت خروج: 12 ظهر';
                var site_rules_str = '<a target="_blank" href="/rules" style="display:flex;margin-top:5px;color:#242424;">' + 'قوانین سایت' + ' ' + '<span style="padding:2px 15px;margin: 0 5px; cursor:pointer;background-color:#fdd835;color:#242424;border-radius: 5px;max-height: 25px;">' + '?' + '</span>' + '</a>';
                var rules_all_button = '<span id="advertise_rules_link" style="cursor:pointer;color:#242424;background-color:#fdd835;padding:2px 15px;border-radius:5px;margin: 0 10px;">' + '?' + '</span>';
                var content_msg = '' +
                    //date_string + '<br/>' +
                    //guest_number_string + '<br/>' +
                    //price_string + '<br/>' +
                    time_string + '<br/>' +
                    site_rules_str +
                    short_rules_string +
                    rules_all_button +
                    //'<div id="rules_accept_parent">' +
                    //'<input type="checkbox" id="rules_accept_checkbox" class="item__rules_accept_checkbox">' +
                    //'<a class="item_rules_link" href="/rules" target="_blank">قوانین سایت</a> و <span class="fake-link" class="item_rules_link" id="advertise_rules_link">قوانین این اقامتگاه</span> را قبول دارم' +
                    //'</div>' +
                    '<br/>' + '<br/>' +
                    //'<div>قوانین کنسلی: (با توافق با میزبان)</div>' +
                    //'<div>تا ۷۲ ساعت قبل از شروع سفر: 20 درصد از مبلغ به میزبان پرداخت میشود</div>' +
                    //'<div>بعد از 72 ساعت: هزینه یک شب به میزبان پرداخت میشود</div>' +
                    //'<br/>' +
                    '<div style={{fontSize:16}}>آیا قوانین سایت و قوانین اقامتگاه را قبول دارید؟</div>';
                //ret.msg;
                if (!confirm_required) {
                    //if ($('#rules_accept_checkbox').is(':checked')) {
                    if (!user_is_autenticated) {
                        showNoYesMessage('ورود به سایت',
                            'برای ثبت درخواست رزرو ابتدا باید وارد سایت شوید',
                            function () {
                                reserve_wait_for_login = true;
                                $('.login__root').appendTo('body');
                                toggle_login();
                            }, undefined, { yesText: 'ورود', noText: 'بستن' });
                        return false;
                    }
                    if (isNumberForIran == false && verifyEmail == false) {
                        showNoYesMessage('ورود به سایت',
                            'برای ثبت درخواست رزرو، ابتدا باید ایمیل خود را ثبت و تایید کنید',
                            function () {
                                showRegisterEmailForm(setEmailToInput);
                                //$("#profileEmail").val(userEmailAddress);
                            }, undefined, { yesText: 'ثبت ایمیل', noText: 'بستن' });
                        return false;
                    }
                    reserve_wait_for_login = false;
                    myajax("reserve/reserverequest", "advertise_id=" + advertise_id +
                        "&from_date=" + from_date + "&to_date=" + to_date +
                        "&number_of_guests=" + $("#guest_count").val() +
                        "&instant_reserve=" + (instantReserveAvailable && instantReserveActivated).toString(), function (ret) {
                            if (ret.val == 1) {
                                //gtag('event', 'book', {
                                //    "items": [
                                //        {
                                //            "id": advertise_id.toString(),
                                //            "name": advertiseTitle,
                                //            "category": provinceName + "/" + cityName,
                                //            "price": last_reserve_price,
                                //            "label": advertiseTitle,
                                //            "title": advertiseTitle,
                                //            "value": last_reserve_price
                                //        }
                                //    ]
                                //});
                                $("#guest_count").val(0);
                                $("#days_label").hide();
                                $("#from_date_label").hide();
                                //$("#to_date_label").hide();
                                $("#reserve_price_label").hide();
                                window.location.href = (instantReserveAvailable && instantReserveActivated) ?
                                    '/app/reserve/list?selecttype=1&reserve_id=' + ret.reserveId + '&initialPayId=' + ret.reserveId
                                    : '/app/reserve/list?selecttype=1&msg=' + 'reserve_request';
                            }
                            else if (ret.val == 0) {
                                showErrorMessage('خطا', ret.msg);
                            }
                            else if (ret.val == 2) {
                                showNoYesMessage('ورود به سایت',
                                    'برای ثبت درخواست رزرو ابتدا باید وارد سایت شوید',
                                    function () {
                                        reserve_wait_for_login = true;
                                        $('.login__root').appendTo('body');
                                        toggle_login();
                                    }, undefined, { yesText: 'ورود', noText: 'بستن' });
                            }
                            else if (ret.val == 3) {
                                showErrorMessage('مسدود', 'امکان درخواست رزرو برای شما مسدود شده است, جهت فعالسازی با پشتیبانی تماس بگیرید: ' +
                                    '<a href="/contact">تماس با پشتیبانی</a>');
                            }
                        }
                    );
                    return;
                }
                showNoYesMessage('اطلاعات رزرو',
                    content_msg,
                    function () {
                        //if ($('#rules_accept_checkbox').is(':checked')) {
                        if (!user_is_autenticated) {
                            showNoYesMessage('ورود به سایت',
                                'برای ثبت درخواست رزرو ابتدا باید وارد سایت شوید',
                                function () {
                                    reserve_wait_for_login = true;
                                    $('.login__root').appendTo('body');
                                    toggle_login();
                                }, undefined, { yesText: 'ورود', noText: 'بستن' });
                            return false;
                        }
                        if (isNumberForIran == false && verifyEmail == false) {
                            showNoYesMessage('ورود به سایت',
                                'برای ثبت درخواست رزرو، ابتدا باید ایمیل خود را ثبت و تایید کنید',
                                function () {
                                    showRegisterEmailForm(setEmailToInput);
                                }, undefined, { yesText: 'ثبت ایمیل', noText: 'بستن' });
                            return false;
                        }
                        reserve_wait_for_login = false;
                        myajax("reserve/reserverequest", "advertise_id=" + advertise_id +
                            "&from_date=" + from_date + "&to_date=" + to_date +
                            "&number_of_guests=" + $("#guest_count").val() +
                            "&instant_reserve=" + (instantReserveAvailable && instantReserveActivated).toString(), function (ret) {
                                if (ret.val == 1) {
                                    //gtag('event', 'book', {
                                    //    "items": [
                                    //        {
                                    //            "id": advertise_id.toString(),
                                    //            "name": advertiseTitle,
                                    //            "category": provinceName + "/" + cityName,
                                    //            "price": last_reserve_price,
                                    //            "label": advertiseTitle,
                                    //            "title": advertiseTitle,
                                    //            "value": last_reserve_price
                                    //        }
                                    //    ]
                                    //});
                                    $("#guest_count").val(0);
                                    $("#days_label").hide();
                                    $("#from_date_label").hide();
                                    //$("#to_date_label").hide();
                                    $("#reserve_price_label").hide();
                                    window.location.href = (instantReserveAvailable && instantReserveActivated) ?
                                        '/app/reserve/list?selecttype=1&reserve_id=' + ret.reserveId + '&initialPayId=' + ret.reserveId
                                        : '/app/reserve/list?selecttype=1&msg=' + 'reserve_request';
                                }
                                else if (ret.val == 0) {
                                    showErrorMessage('خطا', ret.msg);
                                }
                                else if (ret.val == 2) {
                                    showNoYesMessage('ورود به سایت',
                                        'برای ثبت درخواست رزرو ابتدا باید وارد سایت شوید',
                                        function () {
                                            reserve_wait_for_login = true;
                                            $('.login__root').appendTo('body');
                                            toggle_login();
                                        }, undefined, { yesText: 'ورود', noText: 'بستن' });
                                }
                                else if (ret.val == 3) {
                                    showErrorMessage('مسدود', 'امکان درخواست رزرو برای شما مسدود شده است, جهت فعالسازی با پشتیبانی تماس بگیرید: ' +
                                        '<a href="/contact">تماس با پشتیبانی</a>');
                                }
                            }
                        );
                    },
                    function () {
                    },
                    {
                        yesText: (instantReserveAvailable && instantReserveActivated) ? 'تایید و ادامه' : 'تایید و درخواست',
                        noText: (instantReserveAvailable && instantReserveActivated) ? 'لغو رزرو' : 'لغو درخواست',
                        onOpen: function () {
                            $("#advertise_rules_link").click(function () {
                                showInfoMessage('قوانین این اقامتگاه', rules_string);
                            });
                        },
                        autoClose: false
                    }
                );
            }
            else if (ret.val == 0) {
                showErrorMessage('خطا', ret.msg);
            }
        });
}

function onUpdateDate() {
    if (firstSelectedDay) {
        instantReserveActivated = firstSelectedDay.value <= maxInstantReserveStartUnix;
        updateReserveRequestButton();
    }
    updateReservePrice();
    updateReserveLabels();
}

var last_reserve_price;
function updateReservePrice() {
    $("#reserve_price_label").html("در حال محاسبه");
    if (!firstSelectedDay || !secondSelectedDay || $("#guest_count").val() == 0) {
        $("#reserve_price_label").hide();
        return;
    }
    var from_date = firstSelectedDay.date.replaceAll('/', ',');
    var to_date = secondSelectedDay.date.replaceAll('/', ',');
    if (from_date == to_date) {
        $("#reserve_price_label").hide();
        return;
    }
    myajax("reserve/getreserveprice", "advertise_id=" + advertise_id +
        "&from_date=" + from_date + "&to_date=" + to_date +
        "&number_of_guests=" + $("#guest_count").val(), function (ret) {
            if (ret.val == 1) {
                last_reserve_price = parseInt(ret.price);
                var price_string;
                if (ret.price == ret.without_discount_price) {
                    price_string = "مبلغ " + getPriceString(ret.price);
                }
                else {
                    price_string = "مبلغ <strike>" + getPriceString(ret.without_discount_price) + "</strike> " + "<br />" + getPriceString(ret.price);
                }
                $("#reserve_price_label").html(price_string);
                $("#reserve_price_label").show();
            }
        }, false);
}
$(document).on("click", ".occupied_day_label", function () {
    alertify.error("این روز قبلا رزرو شده است");
});
$(document).on("change", "#guest_count", function () {
    onChangeGuestCount(this);
});
function setEmailToInput() {
    $("#profileEmail").val(userEmailAddress);
}
function updateReserveLabels() {
    var data_is_incorrect = false;
    if ($("#guest_count").val() == 0 || $("#guest_count").val() == '') {
        data_is_incorrect = true;
    }
    if (!firstSelectedDay || !secondSelectedDay) {
        data_is_incorrect = true;
    }
    if (data_is_incorrect) {
        $("#days_label").hide();
        $("#from_date_label").hide();
        //$("#to_date_label").hide();
        return;
    }
    $("#days_label").html(diffDaysMs(firstSelectedDay.value, secondSelectedDay.value) + " شب " + "&nbsp&nbsp" + $("#guest_count").val() + " نفر");
    var from_date = firstSelectedDay.date.slice(2);
    var to_date = secondSelectedDay.date.slice(2);
    var from_hour = '2 بعد از ظهر';
    var to_hour = '12 ظهر';
    $("#from_date_label").html("از " + from_date + " تا " + to_date);
    //$("#to_date_label").html("تا " + to_date + " " + to_hour);
    $("#days_label").show();
    $("#from_date_label").show();
    //$("#to_date_label").show();
}
function on_login_action() {
    //$("#write_comment_root").css("display", "unset");
    //$(".comment__login").css("display", "none");
    user_is_autenticated = true;
    $("#write_comment_root").show();
    $(".comment__login").hide();
    if (reserve_wait_for_login) {
        checkReserve(false);
    }
    if (show_contact_wait_for_login) {
        show_contact(show_contact_element, show_contact_id);
    }
}

$(document).on("click", ".thumbs img", function (val) {
    var info = $(this).attr("info");
    $(".largeimg .largeitem").hide();
    $(".largeimg .largeitem[info='" + info + "']").show();
})

//$.fn.digits = function () {
//    return this.each(function () {
//        $(this).text($(this).text().replace(/(\d)(?=(\d\d\d)+(?!\d))/g, "$1,"));
//    })
//}
$('.advertise-page__slider-image').show();
$('.advertise-image-library').each(function () {
    $(this).slick({
        infinite: true,
        lazyLoad: 'ondemand',
        slidesToShow: 1,
        slidesToScroll: 1,
        autoplay: false,
        autoplaySpeed: 8000,
        responsive: [
            {
                breakpoint: 1201,
                settings: {
                    slidesToShow: 1,
                }
            },
            {
                breakpoint: 1101,
                settings: {
                    slidesToShow: 1,
                }
            },
            {
                breakpoint: 901,
                settings: {
                    slidesToShow: 1,
                }
            },
            {
                breakpoint: 781,
                settings: {
                    slidesToShow: 1,
                }
            },
            {
                breakpoint: 551,
                settings: {
                    slidesToShow: 1,
                }
            },
            {
                breakpoint: 250,
                settings: {
                    slidesToShow: 1,
                    arrows: false
                }
            }
            // You can unslick at a given breakpoint now by adding:
            // settings: "unslick"
            // instead of a settings object
        ]
    });
});

function ToggleFavorite($this, $id) {
    myajax("Advertise/TggleFavorite", "id=" + $id + "&flag=" + $($this).hasClass("active"), function (ret) {
        //'status == 2' means login required
        if (ret.status == 2) {
            window.location.href = "../user/publiclogin?returnUrl=" + window.location.href;
        }
        else if (ret.status == 1) {
            if ($($this).hasClass("active")) {
                $($this).removeClass("active");
                //$this.children('span').html('افزودن به علاقه مندی ها');
                //alertify.error('از علاقه مندی ها حذف شد.');
            }
            else {
                $($this).addClass("active");
                //$this.children('span').html('حذف از علاقه مندی ها');
                //alertify.success('به علاقه مندی ها  اضافه شد .');
            }
        }
    }, false);
}
function onClickFavouriteBtn(elem) {
    ToggleFavorite($(elem), $(elem).attr("advertise_id"));
}

$(document).ready(function () {
    lightbox.option({
        'alwaysShowNavOnTouchDevices': true,
        'positionFromTop': 10
    });
    $('.item-content__profile-rate').barrating({
        theme: 'fontawesome-stars',
        readonly: true,
        initialRating: hostUserRating
    });
    $("#phonnum").hide();
    $(".thumbs img").first().click();

    //$.onCreate('div,a', function (elements) {
    //    elements.each(function () {
    //        if ($(this).hasClass("advertise-list-container")) {
    //            findLazyImages();
    //            $(this).children("*").each(function () {
    //                var new_parent = $(this).parent().parent();
    //                $(this).detach();
    //                $(this).appendTo(new_parent);
    //            });
    //            $(this).remove();
    //            if ($(".advertise-page__contact-box").css("position") == "fixed") {
    //                $(".advertise-page__contact-box").stick_in_parent();
    //            }
    //        }
    //        else if ($(this).hasClass('home-page__advertise-item-container')) {
    //            $(this).find('.average-rating').barrating({
    //                theme: 'fontawesome-stars',
    //                readonly: true,
    //                initialRating: null
    //            });
    //            //$(this).find(".home-page__favourite-button").click(function (e) {
    //            //    ToggleFavorite($(this), $(this).attr("advertise_id"));
    //            //    //e.stopPropagation();
    //            //    return false;
    //            //});
    //            if ($(".advertise-page__contact-box").css("position") == "fixed") {
    //                $(".advertise-page__contact-box").stick_in_parent();
    //            }
    //        }
    //    });
    //}, true);

    //$(".digit").digits();
    $('.average-rating').barrating({
        theme: 'fontawesome-stars',
        readonly: true,
        initialRating: null
    });
    initializeGuestAndDate();

    $('#guest_count').focus(function () {
        $(this).val('');
    });
});

function initializeGuestAndDate() {
    if (initialGuestCount > 0) {
        $('#guest_count').val(initialGuestCount);
    }
    if (typeof fromDateValue != 'undefined' ||
        typeof toDateValue != 'undefined') {
        if (typeof fromDateValue != 'undefined') {
            var fromDateG = new Date(fromDateValue);
            var fromDateJ = gregorianToJalaliDate(fromDateG);
            firstSelectedDay = {
                date: fromDateJ.dateString,
                value: fromDateValue
            }
            $('#js-from-date-label').html(fromDateJ.dateString.substring(2));
        }
        if (typeof toDateValue != 'undefined') {
            var toDateG = new Date(toDateValue);
            var toDateJ = gregorianToJalaliDate(toDateG);
            secondSelectedDay = {
                date: toDateJ.dateString,
                value: toDateValue
            }
            $('#js-to-date-label').html(toDateJ.dateString.substring(2));
        }
    }

    onUpdateDate();
}

function shareOnWathsapp(text) {
    var isMobile = /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent);
    if (!isMobile) {
        window.open('https://wa.me/?text=' + text, 'whatsappShare', 'width=626,height=436'); return false;
        return false;
    }
}
$(".js-detail-button").each(function () {
    registerDetailButton(this);
});
function registerDetailButton(btn) {
    $(btn).click(function () {
        var current_state = $(this).attr('data-state') == '1';
        if (!current_state) {
            $('.js-detail-button').each(function () {
                if (!$(this).hasClass('js-stay-expand')) {
                    detailButtonChangeState(this, false);
                }
            });
        }
        detailButtonChangeState(this, !current_state);
    });
}
function detailButtonChangeState(elem, target_state) {
    $(elem).attr('data-state', target_state ? '1' : '0');
    var $target_elem = $('#' + $(elem).attr('data-target-id'));
    var change_text = $(this).attr('data-change-text') == '1';
    if (change_text) {
        $(elem).children('span').html(target_state ? $(this).attr('data-text-collapse') : $(this).attr('data-text-expand'));
    }
    var expand_icon = $(elem).children('i').attr('data-expand-icon');
    var collapse_icon = $(elem).children('i').attr('data-collapse-icon');
    if (expand_icon == undefined || expand_icon == null) {
        expand_icon = 'fa fa-angle-double-down';
    }
    if (collapse_icon == undefined || collapse_icon == null) {
        collapse_icon = 'fa fa-angle-double-up';
    }
    $(elem).children('i').attr('class', target_state ? collapse_icon : expand_icon);
    if (target_state) {
        $target_elem.slideDown();
    }
    else {
        $target_elem.slideUp();
    }
}
function onClickIncDecButton(elem) {
    var $button = $(elem);
    var $inputElement;
    if ($button.attr("data-action") === "plus") {
        $inputElement = $button.next();
    }
    else {
        $inputElement = $button.prev();
    }
    var min = $inputElement.attr("min");
    var max = $inputElement.attr("max");
    var oldValue = $inputElement.val();
    if (oldValue == '')
        oldValue = 0;

    if ($button.attr("data-action") === "plus") {
        if (max == null || parseInt(max) >= parseInt(oldValue) + 1) {
            $inputElement.val(parseInt(oldValue) + 1);
        }
    } else {
        if (min == null || parseInt(min) <= parseInt(oldValue) - 1) {
            $inputElement.val(parseInt(oldValue) - 1);
        }
    }
}

function clampNumberInput(elem) {
    if ($(elem).attr('min') != null) {
        if ($(elem).val() < parseInt($(elem).attr('min'))) {
            $(elem).val($(elem).attr('min'));
        }
        if ($(elem).val() > parseInt($(elem).attr('max'))) {
            $(elem).val($(elem).attr('max'));
        }
    }
}
var emailEditMessage = undefined;
var emailSent = false;

function showRegisterEmailForm(callback) {
    emailSent = false;
    emailEditMessage = showInfoMessage(
        'ثبت ایمیل', '', {
        contentUrl: '/user/popupregisteremail', buttons: [{
            title: 'انصراف',
            color: 'white',
            bgColor: '#EA4335'
        },
        {
            title: 'تایید',
            color: 'white',
            bgColor: '#34A853',
            onclick: doRegisterEmailAction
        }], autoClose: false, onContentReady: callback
    });
}

function doRegisterEmailAction() {
    if (emailSent) {
        profileConfirmEmail();
    }
    else {
        profileRegisterEmail();
    }
}

function profileRegisterEmail() {
    var email = $("#profileEmail").val();
    $.ajax({
        url: "/user/PopupRegisterEmail",
        type: "post",
        data: { email: email },
        success: function (ret) {
            if (ret.status == 1) {
                emailSent = true;
                $("#profileRegisterEmailForm").hide();
                $("#profileConfirmEmailForm").show();
            }
            else {
                alertify.error(ret.msg);
            }
        },
        error: function (ret) {
            alertify.error(ret.msg);
        }
    });
}

function profileConfirmEmail() {
    var emailCode = $("#profileEmailCode").val();
    if (emailCode == '') {
        alertify.error("لطفا کد تایید ارسال شده به ایمیل خود را وارد کنید");
        return;
    }
    $.ajax({
        url: "/user/PopupConfirmEmail",
        type: "post",
        data: { emailCode: emailCode },
        success: function (ret) {
            if (ret.status == 1) {
                alertify.success("ایمیل شما با موفقیت ثبت شد");
                emailSent = false;
                emailEditMessage.close();
                verifyEmail = true;
                $("#emailSpan").html($("#profileEmail").val());
            }
            else {
                alertify.error("کد وارد شده اشتباه است");
            }
        },
        error: function (ret) {
            alertify.error(ret.msg);
        }
    });
}