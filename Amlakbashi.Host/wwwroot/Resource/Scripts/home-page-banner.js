class Slider {
    constructor(selector, config = {}) {
        this.slider = $(selector);
        this.slides = this.slider.children();
        this.activeSlide = this.slides.first();
        this.activeClass = config.activeClass ? config.activeClass : 'slide-item-active';
        this.nextElement = config.nextElement ? $(config.nextElement) : null;
        this.prevElement = config.prevElement ? $(config.prevElement) : null;
        this.timer = config.timer ? config.timer : null;
        this.timerInterval = null;
        this.initSlider();
    }

    initSlider() {
        this.slides.addClass('slide-item');
        this.activeSlide.addClass(this.activeClass);
        this.setupEventListeners();
        if (this.timer) {
            this.attachTimer()
        }
    }

    attachTimer() {
        this.timerInterval = setInterval(() => {
            this.goNextSlide()
        }, this.timer);
    }

    reAttachTimer() {
        if (this.timerInterval && this.timer) {
            clearInterval(this.timerInterval);
            this.attachTimer();
        }
    }

    setEventListenerIfExist(element, eventName, callback) {
        if (element && element.length > 0) {
            element.on(eventName, callback);
        }
    }

    setupEventListeners() {
        this.setEventListenerIfExist(this.nextElement, 'click', () => {
            this.goNextSlide();
            this.reAttachTimer();
        });
        this.setEventListenerIfExist(this.prevElement, 'click', () => {
            this.goPreviousSlide();
            this.reAttachTimer();
        });
    }

    goNextSlide() {
        var nextSlide = this.activeSlide.next();
        this.activeSlide.removeClass(this.activeClass);
        if (nextSlide.length) {
            nextSlide.addClass(this.activeClass);
            this.activeSlide = nextSlide;
            return this.activeSlide;
        }
        this.activeSlide = this.slides.first();
        this.activeSlide.addClass(this.activeClass);
        return this.activeSlide;
    }

    goPreviousSlide() {
        var previousSlide = this.activeSlide.prev()
        this.activeSlide.removeClass(this.activeClass);
        if (previousSlide.length) {
            previousSlide.addClass(this.activeClass);
            this.activeSlide = previousSlide;
            return this.activeSlide;
        }
        this.activeSlide = this.slides.last();
        this.activeSlide.addClass(this.activeClass);
        return this.activeSlide;
    }
}

var slider = new Slider('.home-page__banner-container', {
    nextElement: '.next',
    prevElement: '.prev',
    timer: 5000
})


//carousel 

class Carousel {
    constructor(selector, config = {}) {
        this.parentSlider = document.querySelector(selector);
        this.slider = config.slider ? $(config.slider) : null;
        this.slides = this.slider.children();
        this.firstChild = this.slides.first();
        this.startOffSetLeft = $(this.firstChild).offset().left;
        this.activeSlide = this.firstChild;
        this.activeClass = config.activeClass ? config.activeClass : 'slide-item-active';
        this.nextElement = config.nextElement ? $(config.nextElement) : null;
        this.prevElement = config.prevElement ? $(config.prevElement) : null;
        this.slidesPerViewXl = config.slidesPerViewXl ? config.slidesPerViewXl : this.slidesPerView;
        this.slidesPerViewLg = config.slidesPerViewLg ? config.slidesPerViewLg : this.slidesPerView;
        this.slidesPerViewMd = config.slidesPerViewMd ? config.slidesPerViewMd : this.slidesPerView;
        this.slidesPerViewSm = config.slidesPerViewSm ? config.slidesPerViewSm : this.slidesPerView;
        this.slidesPerViewXs = config.slidesPerViewXs ? config.slidesPerViewXs : this.slidesPerView;
        this.slidesPerView = null;
        this.widthSlides = null;
        this.isDown = false;
        this.startX;
        this.elmScrollLeft;
        this.initSlider();
    }
    initSlider() {
        debugger;
        this.slides.addClass('slide-item');
        this.activeSlide.addClass(this.activeClass);
        this.calculetslidesPerView();
        this.setupEventListeners();
    }
    calculetslidesPerView() {
        var widthShowItems = this.slider.parent().width();
        if (widthShowItems >= 1200) {
            this.slidesPerView = this.slidesPerViewXl;
        } else if (widthShowItems >= 992) {
            this.slidesPerView = this.slidesPerViewLg;
        } else if (widthShowItems >= 768) {
            this.slidesPerView = this.slidesPerViewMd;
        } else if (widthShowItems >= 450) {
            this.slidesPerView = this.slidesPerViewSm;
        } else if (widthShowItems >= 360) {
            this.slidesPerView = this.slidesPerViewXs;
        }
        this.widthSlides = widthShowItems / this.slidesPerView;
        this.slides.css('min-width', this.widthSlides);
        return this.slidesPerView;
    }

    setEventListenerIfExist(element, eventName, callback) {
        if (element && element.length > 0) {
            element.on(eventName, callback);
        }
    }

    setupEventListeners() {
        this.setEventListenerIfExist(this.nextElement, 'click', () => {
            this.goNextSlide();
        });
        this.setEventListenerIfExist(this.prevElement, 'click', () => {
            this.goPreviousSlide();
        });
        this.parentSlider.addEventListener('mousedown', (e) => {
            this.onDragStart(e);
        });
        this.parentSlider.addEventListener('mouseleave', (e) => {
            this.isDown = false;
        });
        this.parentSlider.addEventListener('mouseup', (e) => {
            this.isDown = false;
            this.onDragEnd(e);
        });
        this.parentSlider.addEventListener('mousemove', (e) => {
            if (!this.isDown) return;
            e.preventDefault();
            this.onDragMoving(e);
        });
    }

    onDragStart(e) {
        this.isDown = true;
        this.startX = e.pageX - this.parentSlider.offsetLeft;
        this.elmScrollLeft = this.parentSlider.scrollLeft;
    }

    onDragEnd(e) {
        var mouseupSlid = $(e.target).closest('.carousel-item');
        if (!mouseupSlid.hasClass('slide-item-active')) {
            this.slides.removeClass('slide-item-active');
            mouseupSlid.addClass('slide-item-active');
            this.activeSlide = mouseupSlid;
            this.calculetCurrentItem();
            return this.activeSlide;
        }
        this.calculetCurrentItem();
    }

    onDragMoving(e) {
        const x = e.pageX - this.parentSlider.offsetLeft;
        const walk = (x - this.startX) * 3; //scroll-fast
        this.parentSlider.scrollLeft = this.elmScrollLeft - walk;
        this.calculetCurrentItem();
    }

    goNextSlide() {
        this.prevElement.css('display', 'flex');
        var nextSlide = this.activeSlide.next();
        this.activeSlide.removeClass(this.activeClass);
        $(this.parentSlider).animate({
            scrollLeft: '-=' + this.widthSlides,
            transition: 'all 2s cubic-bezier(0.0, 0.0, 0.58, 1.0);'
        }, 150);
        nextSlide.addClass(this.activeClass);
        this.activeSlide = nextSlide;
        this.calculetCurrentItem();
        return this.activeSlide;
    }

    goPreviousSlide() {
        this.nextElement.css('display', 'flex');
        var previousSlide = this.activeSlide.prev()
        this.activeSlide.removeClass(this.activeClass);
        $(this.parentSlider).animate({
            scrollLeft: '+=' + this.widthSlides,
            transition: 'all 2s cubic-bezier(0.0, 0.0, 0.58, 1.0);'
        }, 150);
        previousSlide.addClass(this.activeClass);
        this.activeSlide = previousSlide;
        this.calculetCurrentItem();
        return this.activeSlide;
    }

    calculetCurrentItem() {
        var lastOffsetLeft = this.slides.last().offset().left;
        var firstOfffsetLeft = this.slides.first().offset().left;
        if (lastOffsetLeft >= 0 - this.widthSlides / 2) {
            this.nextElement.css("display", "none");
            this.prevElement.css("display", "flex");
        } else if (firstOfffsetLeft <= this.startOffSetLeft) {
            this.nextElement.css("display", "flex");
            this.prevElement.css("display", "none");
        } else {
            this.prevElement.css("display", "flex");
            this.nextElement.css("display", "flex");
        }
    }
}

var carouselMedium = new Carousel('.home-page_amlakbashi-medium', {
    slider: '.home-page_amlakbashi-medium .carousel',
    nextElement: '.home-page-medium-box .btnPrevious',
    prevElement: '.home-page-medium-box .btnNext',
    slidesPerViewXl: '6',
    slidesPerViewLg: '5',
    slidesPerViewMd: '4',
    slidesPerViewSm: '3',
    slidesPerViewXs: '2',
})

var carouselVisited = new Carousel('.home-page_box-visited', {
    slider: '.home-page_box-visited .carousel',
    nextElement: '.home-page-visited-box .btnPrevious',
    prevElement: '.home-page-visited-box .btnNext',
    slidesPerViewXl: '5',
    slidesPerViewLg: '4.5',
    slidesPerViewMd: '3.5',
    slidesPerViewSm: '2.5',
    slidesPerViewXs: '1.5',
})
