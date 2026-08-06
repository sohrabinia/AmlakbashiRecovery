/**
 * AmlakBashi V10 Enterprise — Core Frontend App Controller
 * Dedicated, zero-dependency, premium Vanilla JS State Engine.
 * Fully supports RTL Persian layout, interactive dashboards, dynamic listings, wizards, and moderation workflows.
 */

// Global State
const state = {
    currentPortal: 'public', // public, guest, host, admin
    publicPage: 'home', // home, search, detail, seo
    activeSEOPage: '', // travel-guides, about-us, faq, rules, contact-us, privacy
    activeDetailId: 1, // Currently viewed property

    // Auth State
    isLogged: false,
    user: {
        name: 'پشتیبان املاک‌باشی',
        phone: '۰۹۱۲۳۴۵۶۷۸۹',
        role: 'guest', // guest, host, admin
        walletBalance: 25000000, // 25 Million Rial (2.5 Million Toman)
    },

    // Panel Sub-tabs
    guestTab: 'profile', // profile, favorites, history, reviews, wallet
    hostTab: 'dashboard', // dashboard, my-ads, promotions, wallet
    adminTab: 'moderation', // moderation, users, listings, reservations, finance

    // Mock Database
    properties: [
        {
            id: 1,
            title: "ویلا استخردار رویال رامسر",
            category: "ویلا",
            province: "مازندران",
            city: "رامسر",
            price: 18000000,
            rooms: 3,
            capacity: 8,
            rating: 4.9,
            reviewsCount: 32,
            image: "https://images.unsplash.com/photo-1580587771525-78b9dba3b914?auto=format&fit=crop&w=800&q=80",
            gallery: [
                "https://images.unsplash.com/photo-1580587771525-78b9dba3b914?auto=format&fit=crop&w=800&q=80",
                "https://images.unsplash.com/photo-1512917774080-9991f1c4c750?auto=format&fit=crop&w=800&q=80",
                "https://images.unsplash.com/photo-1613977257363-707ba9348227?auto=format&fit=crop&w=800&q=80"
            ],
            description: "این ویلای مدرن و لوکس در رادیو دریا رامسر واقع شده و دارای استخر آب گرم سرپوشیده با سیستم تصفیه مکانیزه، سالن بیلیارد، تراس رو به جنگل و نزدیکی ۱۰۰ متری به ساحل دریا می‌باشد. تمامی امکانات سرمایشی و گرمایشی مجهز و کامل است.",
            address: "رامسر، بلوار کازینو، کوچه یاس، پلاک ۱۲",
            host: {
                name: "آقای علیرضا رضایی",
                phone: "۰۹۱۲۳۴۵۶۷۸۹",
                whatsapp: "989123456789",
                rating: 5.0,
                isSuperhost: true,
                responseTime: "زیر ۵ دقیقه"
            },
            amenities: { wifi: true, pool: true, parking: true, ac: true, bbq: true },
            isPinned: true,
            isLastChance: false,
            status: "APPROVED" // APPROVED, PENDING, REJECTED
        },
        {
            id: 2,
            title: "آپارتمان پنت‌هاوس لوکس ساحلی کیش",
            category: "آپارتمان",
            province: "هرمزگان",
            city: "کیش",
            price: 25000000,
            rooms: 2,
            capacity: 5,
            rating: 4.8,
            reviewsCount: 15,
            image: "https://images.unsplash.com/photo-1512917774080-9991f1c4c750?auto=format&fit=crop&w=800&q=80",
            gallery: [
                "https://images.unsplash.com/photo-1512917774080-9991f1c4c750?auto=format&fit=crop&w=800&q=80",
                "https://images.unsplash.com/photo-1613977257363-707ba9348227?auto=format&fit=crop&w=800&q=80"
            ],
            description: "پنت‌هاوس مجلل با دید ۳۶۰ درجه مستقیم به خلیج همیشه فارس واقع در برج‌های دوقلوی کیش. دارای دکوراسیون تمام ژورنالی، دسترسی عالی به مراکز خرید، اینترنت پرسرعت فیبر نوری و لابی ۲۴ ساعته.",
            address: "کیش، برج‌های دوقلو، طبقه ۱۵، پلاک ۱۵۰۲",
            host: {
                name: "سرکار خانم مرادی",
                phone: "۰۹۹۹۸۸۸۷۷۶۶",
                whatsapp: "989998887766",
                rating: 4.7,
                isSuperhost: true,
                responseTime: "زیر ۱۰ دقیقه"
            },
            amenities: { wifi: true, pool: false, parking: true, ac: true, bbq: true },
            isPinned: false,
            isLastChance: true,
            status: "APPROVED"
        },
        {
            id: 3,
            title: "بوم‌گردی سنتی هورامان کردستان",
            category: "بوم‌گردی",
            province: "کردستان",
            city: "اورامان",
            price: 7500000,
            rooms: 1,
            capacity: 10,
            rating: 4.7,
            reviewsCount: 24,
            image: "https://images.unsplash.com/photo-1566073771259-6a8506099945?auto=format&fit=crop&w=800&q=80",
            gallery: [
                "https://images.unsplash.com/photo-1566073771259-6a8506099945?auto=format&fit=crop&w=800&q=80"
            ],
            description: "اقامتگاهی سنگی و کهن در دل کوه‌های سر به فلک کشیده هورامان تخت با بافت پلکانی بی‌نظیر. سرو غذاهای محلی ارگانیک و چای ذغالی در تراس رو به دره عمیق با چشم‌انداز ابرهای بهاری.",
            address: "کردستان، منطقه پلکانی اورامان، محله بالا",
            host: {
                name: "کاک لقمان اورامی",
                phone: "۰۹۱۸۱۲۳۴۵۶۷",
                whatsapp: "989181234567",
                rating: 4.9,
                isSuperhost: false,
                responseTime: "سریع"
            },
            amenities: { wifi: false, pool: false, parking: true, ac: false, bbq: true },
            isPinned: false,
            isLastChance: false,
            status: "APPROVED"
        },
        {
            id: 4,
            title: "سوئیت مستر مدرن شیراز",
            category: "سوئیت",
            province: "فارس",
            city: "شیراز",
            price: 9000000,
            rooms: 1,
            capacity: 3,
            rating: 4.5,
            reviewsCount: 8,
            image: "https://images.unsplash.com/photo-1502672260266-1c1ef2d93688?auto=format&fit=crop&w=800&q=80",
            gallery: [
                "https://images.unsplash.com/photo-1502672260266-1c1ef2d93688?auto=format&fit=crop&w=800&q=80"
            ],
            description: "سوئیت شیک و دنج در خیابان قصر دشت شیراز با دسترسی فوق‌العاده به باغ ارم و شاهچراغ. مجهز به وسایل پخت و پز، مبل تخت‌خواب‌شو و پارکینگ اختصاصی مسقف.",
            address: "شیراز، خیابان قصر دشت، کوچه ۸، پلاک ۴",
            host: {
                name: "آقای شیرازی",
                phone: "۰۹۳۵۷۷۷۸۸۹۹",
                whatsapp: "989357778899",
                rating: 4.5,
                isSuperhost: false,
                responseTime: "زیر ۱۵ دقیقه"
            },
            amenities: { wifi: true, pool: false, parking: true, ac: true, bbq: false },
            isPinned: false,
            isLastChance: false,
            status: "APPROVED"
        },
        {
            id: 5,
            title: "کلبه چوبی جنگلی ماسال (در انتظار تایید)",
            category: "ویلا",
            province: "گیلان",
            city: "ماسال",
            price: 14000000,
            rooms: 2,
            capacity: 6,
            rating: 0.0,
            reviewsCount: 0,
            image: "https://images.unsplash.com/photo-1542718610-a1d656d1884c?auto=format&fit=crop&w=800&q=80",
            gallery: [
                "https://images.unsplash.com/photo-1542718610-a1d656d1884c?auto=format&fit=crop&w=800&q=80"
            ],
            description: "کلبه‌ای چوبی و نوساز در دل ییلاقات مه آلود اولسبلنگاه ماسال. فضایی رویایی و کاملا بکر بدون مزاحم برای طرفداران سکوت جنگل و طبیعت‌گردی بکر.",
            address: "گیلان، ماسال، مسیر ییلاقات، ییلاق اولسبلنگاه",
            host: {
                name: "میزبان گیلانی",
                phone: "۰۹۱۱۳۳۳۴۴۵۵",
                whatsapp: "989113334455",
                rating: 4.6,
                isSuperhost: false,
                responseTime: "متوسط"
            },
            amenities: { wifi: false, pool: false, parking: true, ac: false, bbq: true },
            isPinned: false,
            isLastChance: false,
            status: "PENDING"
        }
    ],

    // User favorites
    favorites: [1, 2],

    // Contact history logs
    contactHistory: [
        { id: 101, propertyId: 1, timestamp: '۱۴۰۳/۰۵/۱۴ - ۱۲:۳۰', hostName: 'آقای علیرضا رضایی', phone: '۰۹۱۲۳۴۵۶۷۸۹', status: 'موفق' },
        { id: 102, propertyId: 2, timestamp: '۱۴۰۳/۰۵/۱۰ - ۱۰:۱۵', hostName: 'سرکار خانم مرادی', phone: '۰۹۹۹۸۸۸۷۷۶۶', status: 'موفق' }
    ],

    // Reviews mock database
    reviews: [
        { id: 1, propertyId: 1, author: 'مهدی حسینی', rating: 5, date: '۱۴۰۳/۰۵/۰1', comment: 'فوق‌العاده تمیز و با صفا بود. استخر و گرمایش عالی کار می‌کردند. میزبان بسیار با اخلاق بودند.' },
        { id: 2, propertyId: 2, author: 'سارا امینی', rating: 4, date: '۱۴۰۳/۰۴/۱۸', comment: 'چشم‌انداز شگفت‌انگیز بود. تنها مشکل سرعت کم اینترنت بود که رفع شد.' }
    ],

    // Transactions mock database
    transactions: [
        { id: 301, amount: 5000000, type: 'شارژ کیف پول', date: '۱۴۰۳/۰۵/۱۲', refCode: 'TR-89745', status: 'موفق', description: 'افزایش موجودی از درگاه پاسارگاد' },
        { id: 302, amount: -2500000, type: 'خرید نردبان آگهی', date: '۱۴۰۳/۰۵/۰۸', refCode: 'TR-34512', status: 'موفق', description: 'نردبان آگهی ویلا استخردار رویال رامسر' },
        { id: 303, amount: 15000000, type: 'تسویه حساب هماهنگ', date: '۱۴۰۳/۰۴/۳۰', refCode: 'TR-11098', status: 'موفق', description: 'تسویه مستقیم توسط میزبان' }
    ],

    // Historical reservations for admin pane
    reservations: [
        { id: "RES-9801", guestName: "حمید رضا رضوی", hostName: "آقای علیرضا رضایی", propertyTitle: "ویلا استخردار رویال رامسر", dateIn: "۱۴۰۳/۰۵/۰۱", dateOut: "۱۴۰۳/۰۵/۰۴", totalAmount: 54000000, status: "پایان یافته" },
        { id: "RES-9802", guestName: "نازنین غفاری", hostName: "سرکار خانم مرادی", propertyTitle: "آپارتمان پنت‌هاوس لوکس ساحلی کیش", dateIn: "۱۴۰۳/۰۴/۱۵", dateOut: "۱۴۰۳/۰۴/۱۸", totalAmount: 75000000, status: "پایان یافته" }
    ],

    // Active wizard steps tracking
    wizardStep: 1,

    // Search filters active state
    filters: {
        region: '',
        category: '',
        rooms: '',
        wifi: false,
        pool: false,
        priceMin: '',
        priceMax: '',
    }
};

// INITIALIZATION
document.addEventListener('DOMContentLoaded', () => {
    initApp();
});

function initApp() {
    renderPublic();
    updateHeaderAuthStatus();
}

// ROUTING & PORTAL SWAPPING
function switchPortal(portalName) {
    state.currentPortal = portalName;

    // Hide all portals
    document.getElementById('portal-public').classList.add('hidden');
    document.getElementById('portal-guest').classList.add('hidden');
    document.getElementById('portal-host').classList.add('hidden');
    document.getElementById('portal-admin').classList.add('hidden');

    // Reset Swapper active buttons
    const portals = ['public', 'guest', 'host', 'admin'];
    portals.forEach(p => {
        const btn = document.getElementById(`btn-portal-${p}`);
        if (btn) {
            btn.className = "px-3 py-1 rounded text-xs transition font-semibold bg-gray-800 text-gray-300 hover:text-white";
        }
    });

    // Make target active
    document.getElementById(`portal-${portalName}`).classList.remove('hidden');
    const activeBtn = document.getElementById(`btn-portal-${portalName}`);
    if (activeBtn) {
        let activeClass = "px-3 py-1 rounded text-xs transition font-semibold bg-primary text-white ring-1 ring-accent";
        if (portalName === 'admin') activeClass = "px-3 py-1 rounded text-xs transition font-semibold bg-red-700 text-white ring-1 ring-red-200";
        activeBtn.className = activeClass;
    }

    // Force Auth simulation if going to panels & not logged in
    if (portalName !== 'public' && !state.isLogged) {
        openAuthModal('login');
        // fallback to public to force auth login
        state.currentPortal = 'public';
        document.getElementById('portal-public').classList.remove('hidden');
        document.getElementById('portal-guest').classList.add('hidden');
        document.getElementById('portal-host').classList.add('hidden');
        document.getElementById('portal-admin').classList.add('hidden');
        if (activeBtn) activeBtn.className = "px-3 py-1 rounded text-xs transition font-semibold bg-gray-800 text-gray-300 hover:text-white";
        const pubBtn = document.getElementById('btn-portal-public');
        if (pubBtn) pubBtn.className = "px-3 py-1 rounded text-xs transition font-semibold bg-primary text-white ring-1 ring-accent";
        return;
    }

    // Load portal content
    if (portalName === 'public') renderPublic();
    if (portalName === 'guest') renderGuest();
    if (portalName === 'host') renderHost();
    if (portalName === 'admin') renderAdmin();
}

// HEADER & AUTH OPERATIONS
function updateHeaderAuthStatus() {
    const unlogged = document.getElementById('auth-unlogged');
    const logged = document.getElementById('auth-logged');
    const balSpan = document.getElementById('header-wallet-balance');

    if (state.isLogged) {
        unlogged.classList.add('hidden');
        logged.classList.remove('hidden');
        document.getElementById('user-display-name').textContent = state.user.name;
        balSpan.textContent = formatMoney(state.user.walletBalance);
    } else {
        unlogged.classList.remove('hidden');
        logged.classList.add('hidden');
    }
}

function openAuthModal(mode) {
    const modal = document.getElementById('auth-modal');
    modal.classList.remove('hidden');

    const title = document.getElementById('auth-title');
    const subtitle = document.getElementById('auth-subtitle');
    const toggleBtn = document.getElementById('auth-toggle-btn');
    const submitBtn = document.getElementById('auth-submit-btn');

    if (mode === 'login') {
        title.textContent = "ورود به املاک‌باشی";
        subtitle.textContent = "برای دسترسی به پنل ویژه و اطلاعات تماس با میزبانان وارد شوید";
        toggleBtn.textContent = "ثبت‌نام در سامانه";
        submitBtn.textContent = "ادامه و دریافت کد تایید";
    } else {
        title.textContent = "ثبت‌نام در املاک‌باشی";
        subtitle.textContent = "به عنوان مهمان یا میزبان به جمع هزاران کاربر فعال بپیوندید";
        toggleBtn.textContent = "ورود به حساب کاربری";
        submitBtn.textContent = "ایجاد حساب کاربری طلایی";
    }
}

function closeAuthModal() {
    document.getElementById('auth-modal').classList.add('hidden');
    // reset form states
    document.getElementById('auth-otp-section').classList.add('hidden');
    document.getElementById('auth-submit-btn').textContent = "ادامه و دریافت کد تایید";
}

function toggleAuthMode() {
    const title = document.getElementById('auth-title');
    if (title.textContent.includes("ورود")) {
        openAuthModal('register');
    } else {
        openAuthModal('login');
    }
}

// Auto role selection mapping
function handleAuthSubmit(e) {
    e.preventDefault();
    const phone = document.getElementById('auth-phone').value;
    const otpSection = document.getElementById('auth-otp-section');
    const submitBtn = document.getElementById('auth-submit-btn');

    if (otpSection.classList.contains('hidden')) {
        // Step 1: Send SMS
        otpSection.classList.remove('hidden');
        submitBtn.textContent = "تایید نهایی و ورود";
        showToast("کد تایید چهاررقمی به شماره تلفن ارسال شد.");
    } else {
        // Step 2: Validate OTP
        const otpVal = document.getElementById('auth-otp').value;
        if (otpVal === '1234' || otpVal === '۱۲۳۴' || otpVal.trim() !== '') {
            state.isLogged = true;
            state.user.phone = phone;
            state.user.name = 'پشتیبان املاک‌باشی';
            state.user.role = 'guest';

            // Special phone redirects to ease developer evaluation
            if (phone === '09121111111') {
                state.user.role = 'host';
                state.user.name = 'علیرضا رضایی (میزبان ممتاز)';
            } else if (phone === '09122222222') {
                state.user.role = 'admin';
                state.user.name = 'مدیر ارشد املاک‌باشی';
            }

            updateHeaderAuthStatus();
            closeAuthModal();
            showToast("خوش آمدید! ورود موفقیت‌آمیز بود.", "success");

            // Auto swap if we were trying to access panels
            if (state.user.role === 'admin') {
                switchPortal('admin');
            } else if (state.user.role === 'host') {
                switchPortal('host');
            } else {
                switchPortal('guest');
            }
        } else {
            showToast("کد وارد شده صحیح نیست. لطفا از ۱۲۳۴ استفاده کنید.", "error");
        }
    }
}

function logoutUser() {
    state.isLogged = false;
    updateHeaderAuthStatus();
    switchPortal('public');
    showToast("با موفقیت از سیستم خارج شدید.");
}

function toggleUserDropdown() {
    const dd = document.getElementById('user-dropdown');
    dd.classList.toggle('hidden');
}


// TOAST MESSAGES UTILITY
function showToast(message, type = "info") {
    let colorClass = "bg-primary text-secondary border border-accent";
    if (type === "success") colorClass = "bg-emerald-800 text-white border border-emerald-500";
    if (type === "error") colorClass = "bg-red-800 text-white border border-red-500";

    const toast = document.createElement('div');
    toast.className = `fixed bottom-5 right-5 z-50 ${colorClass} py-3.5 px-6 rounded-2xl shadow-xl font-bold text-sm flex items-center gap-2.5 transition duration-300 transform translate-y-10 opacity-0`;
    toast.innerHTML = `<i class="fa-solid ${type === 'success' ? 'fa-circle-check' : type === 'error' ? 'fa-triangle-exclamation' : 'fa-circle-info'} text-base"></i><span>${message}</span>`;

    document.body.appendChild(toast);

    // animate in
    setTimeout(() => {
        toast.classList.remove('translate-y-10', 'opacity-0');
    }, 10);

    // animate out
    setTimeout(() => {
        toast.classList.add('translate-y-10', 'opacity-0');
        setTimeout(() => toast.remove(), 300);
    }, 3000);
}


// HELPERS
function formatMoney(amount) {
    return Number(amount).toLocaleString('fa-IR');
}

// Persiand digit ratings
function formatRating(val) {
    return Number(val).toLocaleString('fa-IR', { minimumFractionDigits: 1 });
}

function getCategoryIcon(cat) {
    switch (cat) {
        case "ویلا": return "fa-house-chimney";
        case "آپارتمان": return "fa-building";
        case "بوم‌گردی": return "fa-mountain-sun";
        case "سوئیت": return "fa-person-shelter";
        default: return "fa-house";
    }
}


// ==========================================
// SECTION 6: REUSABLE COMPONENTS DEFINITION
// ==========================================

// 1. PropertyCard Component (Reusable Component #3)
function PropertyCard(p) {
    const isFav = state.favorites.includes(p.id);
    const pinBadge = p.isPinned ? `<span class="absolute top-3 right-3 bg-accent text-primary text-[10px] font-bold px-2.5 py-1 rounded-lg border border-accent shadow-sm flex items-center gap-1"><i class="fa-solid fa-bookmark text-[9px]"></i> ویژه ممتاز</span>` : '';
    const lastChanceBadge = p.isLastChance ? `<span class="absolute top-3 left-3 bg-red-600 text-white text-[10px] font-bold px-2.5 py-1 rounded-lg shadow-sm flex items-center gap-1"><i class="fa-solid fa-bolt text-[9px] animate-bounce"></i> لحظه آخری</span>` : '';

    return `
    <div class="bg-white rounded-3xl overflow-hidden border border-gray-100 shadow-sm hover:shadow-xl transition duration-300 flex flex-col h-full relative group">
        <!-- Photo and tags -->
        <div class="relative h-56 overflow-hidden bg-gray-100">
            <img src="${p.image}" class="w-full h-full object-cover group-hover:scale-105 transition duration-500" alt="${p.title}">
            ${pinBadge}
            ${lastChanceBadge}
            <button onclick="toggleFavorite(${p.id}, event)" class="absolute bottom-3 right-3 w-9 h-9 rounded-full bg-white/95 text-gray-400 hover:text-red-500 transition shadow flex items-center justify-center border">
                <i class="${isFav ? 'fa-solid fa-heart text-red-500' : 'fa-regular fa-heart'}"></i>
            </button>
        </div>

        <!-- Meta specs -->
        <div class="p-5 flex-grow flex flex-col justify-between text-right">
            <div class="text-right">
                <div class="flex justify-between items-center text-xs text-gray-400 font-bold mb-2">
                    <span class="flex items-center gap-1"><i class="fa-solid fa-map-pin text-accent text-[10px]"></i> ${p.province}، ${p.city}</span>
                    <span class="text-accent bg-accent/10 px-2 py-0.5 rounded-md text-[10px]"><i class="fa-solid ${getCategoryIcon(p.category)} ml-1"></i> ${p.category}</span>
                </div>

                <h4 onclick="showPropertyDetail(${p.id})" class="font-extrabold text-sm text-gray-900 group-hover:text-primary transition cursor-pointer leading-6 line-clamp-2 mb-3 text-right">
                    ${p.title}
                </h4>

                <div class="flex gap-4 text-[11px] text-gray-500 font-semibold mb-4 border-t border-b border-gray-50 py-2.5">
                    <span class="flex items-center gap-1"><i class="fa-solid fa-door-open text-gray-400"></i> ${formatMoney(p.rooms)} خوابه</span>
                    <span class="flex items-center gap-1"><i class="fa-solid fa-users text-gray-400"></i> ظرفیت ${formatMoney(p.capacity)} نفر</span>
                </div>
            </div>

            <div class="flex justify-between items-center mt-2 pt-2 border-t border-gray-50">
                <div class="flex items-baseline gap-1">
                    <span class="text-base font-extrabold text-primary">${formatMoney(p.price)}</span>
                    <span class="text-[10px] text-gray-400 font-bold">ریال / شب</span>
                </div>
                <div class="flex items-center gap-1 text-xs font-bold text-gray-900 bg-gray-50 px-2.5 py-1 rounded-xl">
                    <i class="fa-solid fa-star text-amber-500 text-[10px]"></i>
                    <span>${formatRating(p.rating)}</span>
                </div>
            </div>
        </div>
    </div>
    `;
}

// 2. SearchBox Component (Reusable Component #4)
function SearchBox() {
    return `
    <div class="bg-white rounded-3xl shadow-xl border border-gray-100 p-5 md:p-6 max-w-4xl mx-auto -mt-10 relative z-30">
        <form onsubmit="handleSearchSubmitForm(event)" class="grid grid-cols-1 md:grid-cols-4 gap-4 items-center">

            <!-- City / Province -->
            <div class="space-y-1.5 text-right">
                <label class="block text-xs font-bold text-gray-500 pr-1">کجا سفر می‌کنید؟</label>
                <div class="relative">
                    <i class="fa-solid fa-map-location text-accent absolute right-3.5 top-3.5 text-sm"></i>
                    <select id="search-region-select" class="w-full bg-gray-50 pr-10 pl-4 py-3 border border-gray-200 rounded-2xl text-xs font-bold text-gray-700 outline-none focus:ring-2 focus:ring-primary">
                        <option value="">همه استان‌های ایران</option>
                        <option value="مازندران" ${state.filters.region === 'مازندران' ? 'selected' : ''}>مازندران (شمال)</option>
                        <option value="هرمزگان" ${state.filters.region === 'هرمزگان' ? 'selected' : ''}>کیش و قشم</option>
                        <option value="کردستان" ${state.filters.region === 'کردستان' ? 'selected' : ''}>کردستان (اورامان)</option>
                        <option value="فارس" ${state.filters.region === 'فارس' ? 'selected' : ''}>شیراز و جنوب</option>
                    </select>
                </div>
            </div>

            <!-- Category -->
            <div class="space-y-1.5 text-right">
                <label class="block text-xs font-bold text-gray-500 pr-1">نوع اقامتگاه</label>
                <div class="relative">
                    <i class="fa-solid fa-house-user text-accent absolute right-3.5 top-3.5 text-sm"></i>
                    <select id="search-category-select" class="w-full bg-gray-50 pr-10 pl-4 py-3 border border-gray-200 rounded-2xl text-xs font-bold text-gray-700 outline-none focus:ring-2 focus:ring-primary">
                        <option value="">همه دسته‌ها</option>
                        <option value="ویلا" ${state.filters.category === 'ویلا' ? 'selected' : ''}>ویلا لوکس</option>
                        <option value="آپارتمان" ${state.filters.category === 'آپارتمان' ? 'selected' : ''}>آپارتمان شهری</option>
                        <option value="بوم‌گردی" ${state.filters.category === 'بوم‌گردی' ? 'selected' : ''}>بوم‌گردی سنتی</option>
                        <option value="سوئیت" ${state.filters.category === 'سوئیت' ? 'selected' : ''}>سوئیت مدرن</option>
                    </select>
                </div>
            </div>

            <!-- Rooms count -->
            <div class="space-y-1.5 text-right">
                <label class="block text-xs font-bold text-gray-500 pr-1">تعداد اتاق</label>
                <div class="relative">
                    <i class="fa-solid fa-door-open text-accent absolute right-3.5 top-3.5 text-sm"></i>
                    <select id="search-rooms-select" class="w-full bg-gray-50 pr-10 pl-4 py-3 border border-gray-200 rounded-2xl text-xs font-bold text-gray-700 outline-none focus:ring-2 focus:ring-primary">
                        <option value="">مهم نیست</option>
                        <option value="1" ${state.filters.rooms === '1' ? 'selected' : ''}>۱ خوابه</option>
                        <option value="2" ${state.filters.rooms === '2' ? 'selected' : ''}>۲ خوابه</option>
                        <option value="3" ${state.filters.rooms === '3' ? 'selected' : ''}>۳ خوابه و بیشتر</option>
                    </select>
                </div>
            </div>

            <!-- Submit trigger -->
            <div class="pt-5">
                <button type="submit" class="w-full py-3.5 bg-primary hover:bg-primary-dark text-white rounded-2xl font-bold transition flex items-center justify-center gap-2 shadow-lg shadow-primary/20 text-sm">
                    <i class="fa-solid fa-magnifying-glass"></i>
                    <span>جستجوی هوشمند</span>
                </button>
            </div>

        </form>
    </div>
    `;
}

// 3. FilterPanel Component (Reusable Component #5)
function FilterPanel() {
    return `
    <div class="bg-white rounded-3xl border border-gray-100 p-6 shadow-sm text-right space-y-6 sticky top-[150px]">
        <div class="flex justify-between items-center pb-4 border-b border-gray-100">
            <h4 class="font-extrabold text-sm text-gray-900"><i class="fa-solid fa-sliders text-accent ml-1.5"></i> فیلترهای پیشرفته</h4>
            <button onclick="resetFilters()" class="text-xs text-red-600 font-bold hover:underline">حذف همه</button>
        </div>

        <!-- Price filter slider -->
        <div class="space-y-3">
            <span class="block text-xs font-bold text-gray-700">محدوده قیمت هر شب (ریال)</span>
            <div class="grid grid-cols-2 gap-2">
                <div>
                    <label class="text-[10px] text-gray-400 font-bold block mb-1">حداقل</label>
                    <input type="number" id="filter-price-min" placeholder="مثال: ۵,۰۰۰,۰۰۰" value="${state.filters.priceMin}" class="w-full px-3 py-2 bg-gray-50 border border-gray-100 rounded-xl text-xs outline-none focus:ring-1 focus:ring-primary font-semibold">
                </div>
                <div>
                    <label class="text-[10px] text-gray-400 font-bold block mb-1">حداکثر</label>
                    <input type="number" id="filter-price-max" placeholder="مثال: ۳۰,۰۰۰,۰۰۰" value="${state.filters.priceMax}" class="w-full px-3 py-2 bg-gray-50 border border-gray-100 rounded-xl text-xs outline-none focus:ring-1 focus:ring-primary font-semibold">
                </div>
            </div>
        </div>

        <!-- Amenities Checklist -->
        <div class="space-y-3 border-t border-gray-50 pt-4">
            <span class="block text-xs font-bold text-gray-700">امکانات رفاهی ویژه</span>
            <div class="space-y-2 text-xs font-semibold text-gray-600">
                <label class="flex items-center gap-2 cursor-pointer hover:text-gray-900">
                    <input type="checkbox" id="filter-wifi" ${state.filters.wifi ? 'checked' : ''} onchange="applySidebarFilters()" class="rounded text-primary focus:ring-primary">
                    <span>اینترنت وای‌فای (WiFi)</span>
                </label>
                <label class="flex items-center gap-2 cursor-pointer hover:text-gray-900">
                    <input type="checkbox" id="filter-pool" ${state.filters.pool ? 'checked' : ''} onchange="applySidebarFilters()" class="rounded text-primary focus:ring-primary">
                    <span>استخر اختصاصی</span>
                </label>
            </div>
        </div>

        <button onclick="applySidebarFilters()" class="w-full py-3 bg-primary hover:bg-primary-dark text-white rounded-xl text-xs font-bold transition shadow-sm">
            اعمال تغییرات فیلتر
        </button>
    </div>
    `;
}

// 4. HostContactCard Component (Reusable Component #6)
function HostContactCard(host, propertyId) {
    return `
    <div class="bg-white rounded-3xl border border-gray-100 p-6 shadow-sm text-right space-y-4">
        <h4 class="font-extrabold text-sm text-gray-900 border-r-4 border-accent pr-2">اطلاعات تماس مستقیم میزبان</h4>

        <div class="flex items-center gap-4 py-2 flex-row justify-end">
            <div class="text-right">
                <span class="text-sm font-extrabold text-gray-900 block">${host.name}</span>
                <span class="text-[10px] text-gray-400 font-semibold block mt-1"><i class="fa-solid fa-clock ml-1 text-accent"></i> پاسخگویی سریع: ${host.responseTime}</span>
            </div>
            <div class="w-14 h-14 bg-accent/10 rounded-full flex items-center justify-center text-accent text-xl border-2 border-accent">
                <i class="fa-solid fa-user-tie"></i>
            </div>
        </div>

        <div class="space-y-2 border-t border-gray-50 pt-3">
            <div class="flex justify-between items-center text-xs">
                <span class="text-gray-400 font-semibold">امتیاز میزبان:</span>
                <span class="font-bold text-emerald-600"><i class="fa-solid fa-star text-amber-500 ml-1"></i> ${formatRating(host.rating)}</span>
            </div>
            <div class="flex justify-between items-center text-xs">
                <span class="text-gray-400 font-semibold">تعهد اخلاقی:</span>
                <span class="font-bold text-gray-700">سوپرهاست لوکس تایید شده</span>
            </div>
        </div>

        <!-- Primary CTA to View Host Information and Contact Host -->
        <button onclick="openContactModal(${propertyId})" class="w-full py-3.5 bg-primary hover:bg-primary-dark text-white rounded-xl text-xs font-bold transition flex items-center justify-center gap-2 shadow-md">
            <i class="fa-solid fa-phone"></i>
            <span>مشاهده اطلاعات تماس مستقیم</span>
        </button>
    </div>
    `;
}

// 5. WalletCard Component (Reusable Component #7)
function WalletCard(balance) {
    return `
    <div class="bg-gradient-to-br from-primary to-primary-dark text-white p-6 rounded-3xl border border-accent shadow-xl relative overflow-hidden flex flex-col justify-between h-48">
        <div class="absolute -right-12 -bottom-12 w-32 h-32 rounded-full bg-accent opacity-15 filter blur-xl"></div>

        <div class="flex justify-between items-start">
            <div class="text-right">
                <span class="text-xs text-accent font-bold block">موجودی کیف پول شبیه‌سازی‌شده V10</span>
                <span class="text-3xl font-extrabold mt-2.5 block tracking-wider text-secondary">${formatMoney(balance)} <span class="text-xs font-semibold text-gray-300">ریال</span></span>
            </div>
            <i class="fa-solid fa-wallet text-3xl text-accent"></i>
        </div>

        <div class="flex justify-between items-center border-t border-white/15 pt-4">
            <span class="text-[10px] text-gray-300 font-medium">پشتیبانی بانکی شتاب کشور</span>

            <!-- Quick Add Cash simulation trigger -->
            <button onclick="simulateAddCash()" class="px-4 py-2 bg-accent hover:bg-accent-dark text-primary rounded-xl text-xs font-bold transition shadow-md">
                <i class="fa-solid fa-circle-plus ml-1"></i> افزایش سریع شارژ
            </button>
        </div>
    </div>
    `;
}

// 6. TransactionTable Component (Reusable Component #8)
function TransactionTable(txs) {
    if (txs.length === 0) return EmptyState("هنوز هیچ تراکنش مالی برای شما ثبت نشده است.");

    let rows = txs.map(t => {
        const isDebit = t.amount < 0;
        const badgeColor = isDebit ? 'bg-red-50 text-red-700 border-red-200' : 'bg-emerald-50 text-emerald-700 border-emerald-200';
        const sign = isDebit ? '-' : '+';

        return `
        <tr class="hover:bg-gray-50/50 transition">
            <td class="px-6 py-4 whitespace-nowrap text-xs font-semibold text-gray-400 text-right">${t.date}</td>
            <td class="px-6 py-4 whitespace-nowrap text-xs font-bold text-gray-900 text-right">${t.type}</td>
            <td class="px-6 py-4 whitespace-nowrap text-right" dir="ltr">
                <span class="inline-flex items-center px-2.5 py-1 rounded-lg text-xs font-bold border ${badgeColor}">
                    ${sign}${formatMoney(Math.abs(t.amount))} ریال
                </span>
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-xs font-semibold text-gray-600 tracking-wider text-right">${t.refCode}</td>
            <td class="px-6 py-4 whitespace-nowrap text-xs text-gray-500 max-w-xs truncate text-right">${t.description}</td>
        </tr>
        `;
    }).join('');

    return `
    <div class="overflow-x-auto">
        <table class="min-w-full divide-y divide-gray-150 text-right">
            <thead class="bg-gray-50">
                <tr>
                    <th class="px-6 py-3.5 text-xs font-bold text-gray-500 uppercase text-right">تاریخ تراکنش</th>
                    <th class="px-6 py-3.5 text-xs font-bold text-gray-500 uppercase text-right">نوع عملیات</th>
                    <th class="px-6 py-3.5 text-xs font-bold text-gray-500 uppercase text-right">مبلغ</th>
                    <th class="px-6 py-3.5 text-xs font-bold text-gray-500 uppercase text-right">کد پیگیری شتاب</th>
                    <th class="px-6 py-3.5 text-xs font-bold text-gray-500 uppercase text-right">توضیحات بابت</th>
                </tr>
            </thead>
            <tbody class="divide-y divide-gray-100 bg-white">
                ${rows}
            </tbody>
        </table>
    </div>
    `;
}

// 7. DashboardCard Component (Reusable Component #9)
function DashboardCard(title, value, icon, colorClass, subText) {
    return `
    <div class="bg-white rounded-2xl border border-gray-100 p-5 shadow-sm hover:shadow-md transition duration-200 flex justify-between items-center text-right">
        <div class="text-right">
            <span class="text-xs text-gray-400 font-bold block">${title}</span>
            <span class="text-2xl font-extrabold text-gray-900 mt-2 block tracking-wide">${value}</span>
            <span class="text-[10px] text-gray-400 mt-1 block font-semibold">${subText}</span>
        </div>
        <div class="w-12 h-12 rounded-xl flex items-center justify-center text-lg ${colorClass}">
            <i class="fa-solid ${icon}"></i>
        </div>
    </div>
    `;
}

// 8. AdvertisementCard Component (Reusable Component #10)
function AdvertisementCard(p) {
    let statusBadge = '';
    if (p.status === 'APPROVED') statusBadge = `<span class="px-2.5 py-1 text-[10px] font-bold text-emerald-700 bg-emerald-50 border border-emerald-200 rounded-lg"><i class="fa-solid fa-check ml-1"></i> تایید شده و فعال</span>`;
    if (p.status === 'PENDING') statusBadge = `<span class="px-2.5 py-1 text-[10px] font-bold text-amber-700 bg-amber-50 border border-amber-200 rounded-lg animate-pulse"><i class="fa-solid fa-clock ml-1"></i> در انتظار بررسی ادمین</span>`;
    if (p.status === 'REJECTED') statusBadge = `<span class="px-2.5 py-1 text-[10px] font-bold text-red-700 bg-red-50 border border-red-200 rounded-lg"><i class="fa-solid fa-circle-xmark ml-1"></i> رد شده / نیازمند اصلاح</span>`;

    const pinBtn = p.isPinned ? `<span class="text-xs text-accent bg-accent/10 px-2.5 py-1 rounded-lg border border-accent/20 font-bold"><i class="fa-solid fa-star ml-1"></i> ارتقاء ویژه ممتاز</span>` : `<button onclick="applyNardeban(${p.id})" class="px-3 py-1.5 bg-accent hover:bg-accent-dark text-primary rounded-lg text-xs font-bold transition">خرید نردبان طلایی</button>`;
    const lastChanceBtn = p.isLastChance ? `<span class="text-xs text-red-600 bg-red-50 px-2.5 py-1 rounded-lg border border-red-200 font-bold"><i class="fa-solid fa-bolt ml-1 animate-bounce"></i> لحظه آخری فعال</span>` : `<button onclick="applyLastChance(${p.id})" class="px-3 py-1.5 bg-red-600 hover:bg-red-700 text-white rounded-lg text-xs font-bold transition">ارتقاء لحظه آخری</button>`;

    return `
    <div class="bg-white rounded-2xl border border-gray-100 p-4 shadow-sm flex flex-col md:flex-row gap-4 justify-between items-center group">
        <div class="flex flex-col md:flex-row gap-4 items-center text-center md:text-right w-full">
            <div class="w-24 h-20 rounded-xl overflow-hidden bg-gray-100 flex-shrink-0">
                <img src="${p.image}" class="w-full h-full object-cover">
            </div>
            <div class="text-right w-full">
                <h5 class="font-extrabold text-sm text-gray-900 line-clamp-1 text-right">${p.title}</h5>
                <p class="text-xs text-gray-400 mt-1 font-semibold text-right">${p.province}، ${p.city} | قیمت هر شب: ${formatMoney(p.price)} ریال</p>
                <div class="flex gap-2 items-center mt-3 justify-center md:justify-start text-right">
                    ${statusBadge}
                    <span class="text-[10px] text-gray-400 font-bold"><i class="fa-solid fa-eye ml-1"></i> ۱۳۴ بازدید واقعی</span>
                </div>
            </div>
        </div>

        <div class="flex gap-2 flex-wrap justify-end w-full md:w-auto">
            ${pinBtn}
            ${lastChanceBtn}
            <button onclick="editAdvertisement(${p.id})" class="p-1.5 text-gray-400 hover:text-gray-900 hover:bg-gray-100 rounded-lg transition"><i class="fa-solid fa-pen-to-square text-sm"></i></button>
        </div>
    </div>
    `;
}

// 9. PromotionCard Component (Reusable Component #11)
function PromotionCard() {
    return `
    <div class="grid grid-cols-1 md:grid-cols-2 gap-6 text-right font-medium">
        <!-- Nardeban promo -->
        <div class="bg-white rounded-3xl border border-gray-100 p-6 shadow-sm flex flex-col justify-between space-y-4 relative overflow-hidden group">
            <div class="absolute top-0 left-0 bg-accent text-primary font-bold text-[10px] px-3.5 py-1 rounded-br-2xl shadow-sm"><i class="fa-solid fa-fire"></i> پکیج محبوب</div>
            <div class="space-y-2 text-right">
                <div class="w-12 h-12 bg-accent/10 rounded-xl flex items-center justify-center text-accent text-lg"><i class="fa-solid fa-rocket"></i></div>
                <h5 class="font-extrabold text-base text-gray-900">سرویس نردبان طلایی (Nardeban)</h5>
                <p class="text-xs text-gray-400 leading-5">با استفاده از سرویس نردبان، آگهی اقامتگاه شما به صدر نتایج جستجوی شهر مربوطه منتقل شده و شانس کلیک و دید ارتباط با مشتری تا ۴ برابر افزایش می‌یابد.</p>
            </div>
            <div class="flex justify-between items-center pt-4 border-t border-gray-50 text-right">
                <div>
                    <span class="text-xs text-gray-400 font-bold block">تعرفه فعال‌سازی یک‌هفته</span>
                    <span class="text-base font-extrabold text-primary">۲,۵۰۰,۰۰۰ <span class="text-xs font-semibold">ریال</span></span>
                </div>
                <button onclick="switchHostTab('my-ads')" class="px-4 py-2 bg-primary hover:bg-primary-dark text-white rounded-xl text-xs font-bold transition">ارتقاء آگهی‌ها</button>
            </div>
        </div>

        <!-- Last chance promo -->
        <div class="bg-white rounded-3xl border border-gray-100 p-6 shadow-sm flex flex-col justify-between space-y-4 relative overflow-hidden group text-right">
            <div class="space-y-2 text-right">
                <div class="w-12 h-12 bg-red-50 rounded-xl flex items-center justify-center text-red-600 text-lg"><i class="fa-solid fa-bolt"></i></div>
                <h5 class="font-extrabold text-base text-gray-900">سرویس آگهی لحظه آخری (Last Minute)</h5>
                <p class="text-xs text-gray-400 leading-5">مخصوص روزهایی که قصد پرکردن سریع ظرفیت خالی با اعمال تخفیف شگفت‌انگیز را دارید. آگهی شما با برچسب جذاب قرمز رنگ در هدر اصلی و صفحه اول برای مخاطبان نمایش داده می‌شود.</p>
            </div>
            <div class="flex justify-between items-center pt-4 border-t border-gray-50 text-right">
                <div>
                    <span class="text-xs text-gray-400 font-bold block">تعرفه فعال‌سازی ۱ شب</span>
                    <span class="text-base font-extrabold text-primary">۱,۵۰۰,۰۰۰ <span class="text-xs font-semibold">ریال</span></span>
                </div>
                <button onclick="switchHostTab('my-ads')" class="px-4 py-2 bg-red-600 hover:bg-red-700 text-white rounded-xl text-xs font-bold transition shadow shadow-red-100">فعال‌سازی فوری</button>
            </div>
        </div>
    </div>
    `;
}

// 10. EmptyState Component (Reusable Component #12)
function EmptyState(message) {
    return `
    <div class="text-center py-16 px-4">
        <div class="w-20 h-20 bg-gray-50 rounded-full mx-auto flex items-center justify-center text-gray-300 border border-gray-100 mb-4">
            <i class="fa-solid fa-folder-open text-3xl"></i>
        </div>
        <h5 class="font-bold text-sm text-gray-800">${message}</h5>
        <p class="text-xs text-gray-400 mt-1 text-center">از طریق پنل‌ها یا جستجوی همگانی می‌توانید موارد جدیدی ایجاد کنید.</p>
    </div>
    `;
}

// 11. ErrorState Component (Reusable Component #13)
function ErrorState(message) {
    return `
    <div class="p-4 bg-red-50 border border-red-100 rounded-2xl flex items-center gap-3 text-red-800 text-xs font-semibold leading-5 text-right">
        <i class="fa-solid fa-triangle-exclamation text-base"></i>
        <p class="text-right">${message}</p>
    </div>
    `;
}

// 12. LoadingState Component (Reusable Component #14)
function LoadingState() {
    return `
    <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
        <div class="bg-white rounded-3xl overflow-hidden border border-gray-100 p-4 space-y-4">
            <div class="h-44 shimmer rounded-2xl"></div>
            <div class="h-4 shimmer rounded-full w-2/3"></div>
            <div class="h-3 shimmer rounded-full w-1/2"></div>
        </div>
        <div class="bg-white rounded-3xl overflow-hidden border border-gray-100 p-4 space-y-4">
            <div class="h-44 shimmer rounded-2xl"></div>
            <div class="h-4 shimmer rounded-full w-2/3"></div>
            <div class="h-3 shimmer rounded-full w-1/2"></div>
        </div>
        <div class="bg-white rounded-3xl overflow-hidden border border-gray-100 p-4 space-y-4">
            <div class="h-44 shimmer rounded-2xl"></div>
            <div class="h-4 shimmer rounded-full w-2/3"></div>
            <div class="h-3 shimmer rounded-full w-1/2"></div>
        </div>
    </div>
    `;
}


// ==========================================
// SECTION 7: PUBLIC WEBSITE VIEWS IMPLEMENTATION
// ==========================================

function renderPublic() {
    const container = document.getElementById('public-content-container');
    if (!container) return;

    if (state.publicPage === 'home') {
        renderHomePage(container);
    } else if (state.publicPage === 'search') {
        renderSearchResultsPage(container);
    } else if (state.publicPage === 'detail') {
        renderPropertyDetailPage(container, state.activeDetailId);
    } else if (state.publicPage === 'seo') {
        renderSEOPage(container, state.activeSEOPage);
    }
}

function navigateHome() {
    state.publicPage = 'home';
    state.currentPortal = 'public';
    switchPortal('public');
}

// HOMEPAGE VIEW
function renderHomePage(container) {
    const featured = state.properties.filter(p => p.status === 'APPROVED' && p.isPinned);
    const featuredCards = featured.map(p => PropertyCard(p)).join('');

    container.innerHTML = `
    <!-- Hero Banner with high-fidelity branding -->
    <section class="relative bg-primary text-white py-20 px-4 md:py-28 overflow-hidden text-right">
        <div class="absolute inset-0 bg-cover bg-center filter opacity-15" style="background-image: url('https://images.unsplash.com/photo-1542718610-a1d656d1884c?auto=format&fit=crop&w=1600&q=80');"></div>
        <div class="absolute -left-20 -bottom-20 w-80 h-80 rounded-full bg-accent opacity-20 filter blur-2xl"></div>

        <div class="max-w-5xl mx-auto text-center relative z-10 space-y-6">
            <span class="bg-accent/15 text-accent border border-accent/25 px-4 py-1.5 rounded-full text-xs font-extrabold tracking-wide inline-block"><i class="fa-solid fa-star ml-1"></i> بازارگاه بی‌واسطه رزرو اقامتگاه‌های لوکس ایران</span>
            <h1 class="text-3xl md:text-5xl font-black text-secondary leading-tight md:leading-snug text-center">به املاک‌باشی خوش آمدید</h1>
            <p class="text-sm md:text-base text-gray-300 max-w-2xl mx-auto font-medium leading-relaxed text-center">ویلاهای استخردار مجلل، بوم‌گردی‌های سنتی بکر، و سوئیت‌های شیک شهری را مستقیماً از خود میزبان و بدون واسطه اجاره کنید.</p>
        </div>
    </section>

    <!-- Interactive SearchBox reusable component -->
    <section class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        ${SearchBox()}
    </section>

    <!-- Fast Destinations navigation (SEO / Region filter) -->
    <section class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-16 text-right">
        <div class="text-right mb-8">
            <h3 class="text-xl font-extrabold text-gray-900 border-r-4 border-accent pr-3">محبوب‌ترین مقاصد سفر در ایران</h3>
            <p class="text-xs text-gray-400 mt-1 font-semibold text-right">انتخاب کنید و لیست اقامتگاه‌های لوکس هر استان را ببینید</p>
        </div>
        <div class="grid grid-cols-2 md:grid-cols-4 gap-4">
            <div onclick="triggerSearchWithRegion('مازندران')" class="relative rounded-2xl overflow-hidden h-36 bg-gray-100 group cursor-pointer shadow-sm">
                <img src="https://images.unsplash.com/photo-1580587771525-78b9dba3b914?auto=format&fit=crop&w=300&q=80" class="w-full h-full object-cover group-hover:scale-105 transition duration-300">
                <div class="absolute inset-0 bg-gradient-to-t from-black/80 via-black/20 to-transparent flex items-end p-4 flex-row justify-end">
                    <span class="text-sm font-extrabold text-white text-right">مازندران (ویلا شمال)</span>
                </div>
            </div>
            <div onclick="triggerSearchWithRegion('هرمزگان')" class="relative rounded-2xl overflow-hidden h-36 bg-gray-100 group cursor-pointer shadow-sm">
                <img src="https://images.unsplash.com/photo-1512917774080-9991f1c4c750?auto=format&fit=crop&w=300&q=80" class="w-full h-full object-cover group-hover:scale-105 transition duration-300">
                <div class="absolute inset-0 bg-gradient-to-t from-black/80 via-black/20 to-transparent flex items-end p-4 flex-row justify-end">
                    <span class="text-sm font-extrabold text-white text-right">جزیره لوکس کیش</span>
                </div>
            </div>
            <div onclick="triggerSearchWithRegion('کردستان')" class="relative rounded-2xl overflow-hidden h-36 bg-gray-100 group cursor-pointer shadow-sm">
                <img src="https://images.unsplash.com/photo-1566073771259-6a8506099945?auto=format&fit=crop&w=300&q=80" class="w-full h-full object-cover group-hover:scale-105 transition duration-300">
                <div class="absolute inset-0 bg-gradient-to-t from-black/80 via-black/20 to-transparent flex items-end p-4 flex-row justify-end">
                    <span class="text-sm font-extrabold text-white text-right">بوم‌گردی کردستان</span>
                </div>
            </div>
            <div onclick="triggerSearchWithRegion('فارس')" class="relative rounded-2xl overflow-hidden h-36 bg-gray-100 group cursor-pointer shadow-sm">
                <img src="https://images.unsplash.com/photo-1502672260266-1c1ef2d93688?auto=format&fit=crop&w=300&q=80" class="w-full h-full object-cover group-hover:scale-105 transition duration-300">
                <div class="absolute inset-0 bg-gradient-to-t from-black/80 via-black/20 to-transparent flex items-end p-4 flex-row justify-end">
                    <span class="text-sm font-extrabold text-white text-right">شیراز و آثار تاریخی</span>
                </div>
            </div>
        </div>
    </section>

    <!-- Featured Properties Slider/Grid -->
    <section class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 pb-20 text-right">
        <div class="text-right mb-8 flex justify-between items-end">
            <div>
                <h3 class="text-xl font-extrabold text-gray-900 border-r-4 border-accent pr-3 text-right">اقامتگاه‌های ویژه و ممتاز</h3>
                <p class="text-xs text-gray-400 mt-1 font-semibold text-right">تایید شده، با بالاترین سطح رضایت مهمانان</p>
            </div>
            <button onclick="triggerSearchAll()" class="text-xs text-primary font-bold hover:underline flex items-center gap-1">مشاهده همه آگهی‌ها <i class="fa-solid fa-arrow-left"></i></button>
        </div>
        <div class="grid grid-cols-1 md:grid-cols-3 gap-6 text-right">
            ${featuredCards}
        </div>
    </section>
    `;
}

function triggerSearchWithRegion(reg) {
    state.filters.region = reg;
    state.publicPage = 'search';
    renderPublic();
}

function triggerSearchAll() {
    state.filters = { region: '', category: '', rooms: '', wifi: false, pool: false, priceMin: '', priceMax: '' };
    state.publicPage = 'search';
    renderPublic();
}

// SEARCH RESULTS VIEW
function renderSearchResultsPage(container) {
    container.innerHTML = `
    <!-- Top search bar container -->
    <div class="bg-primary/5 border-b border-gray-100 py-6 px-4">
        <div class="max-w-7xl mx-auto">
            ${SearchBox()}
        </div>
    </div>

    <!-- Main side-by-side search area -->
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-10">
        <div class="grid grid-cols-1 lg:grid-cols-4 gap-8">
            <!-- Sidebar Filter panel Reusable Component -->
            <div class="lg:col-span-1">
                ${FilterPanel()}
            </div>

            <!-- Property card list grid -->
            <div class="lg:col-span-3 text-right">
                <div class="flex justify-between items-center mb-6">
                    <span id="results-count-text" class="text-xs text-gray-400 font-bold">در حال دریافت نتایج...</span>
                    <select id="results-sort" onchange="sortSearchResults()" class="bg-white border text-xs font-bold px-3 py-2 rounded-xl outline-none">
                        <option value="pop">محبوب‌ترین‌ها</option>
                        <option value="cheap">ارزان‌ترین</option>
                        <option value="expensive">گران‌ترین</option>
                    </select>
                </div>

                <div id="search-results-grid">
                    <!-- Dynamic rendering under v10-app.js controller -->
                </div>
            </div>
        </div>
    </div>
    `;

    // Trigger async filter simulation
    applySidebarFilters();
}

function handleSearchSubmitForm(e) {
    e.preventDefault();
    state.filters.region = document.getElementById('search-region-select').value;
    state.filters.category = document.getElementById('search-category-select').value;
    state.filters.rooms = document.getElementById('search-rooms-select').value;

    state.publicPage = 'search';
    renderPublic();
}

function applySidebarFilters() {
    const grid = document.getElementById('search-results-grid');
    if (!grid) return;

    // Show LoadingState
    grid.innerHTML = LoadingState();

    // read filter values if sidebar elements are loaded
    const minVal = document.getElementById('filter-price-min');
    const maxVal = document.getElementById('filter-price-max');
    const wifiBox = document.getElementById('filter-wifi');
    const poolBox = document.getElementById('filter-pool');

    if (minVal) state.filters.priceMin = minVal.value;
    if (maxVal) state.filters.priceMax = maxVal.value;
    if (wifiBox) state.filters.wifi = wifiBox.checked;
    if (poolBox) state.filters.pool = poolBox.checked;

    // Simulate short network delay (500ms) for professional realism
    setTimeout(() => {
        let list = state.properties.filter(p => p.status === 'APPROVED');

        if (state.filters.region) {
            list = list.filter(p => p.province === state.filters.region);
        }
        if (state.filters.category) {
            list = list.filter(p => p.category === state.filters.category);
        }
        if (state.filters.rooms) {
            list = list.filter(p => p.rooms >= parseInt(state.filters.rooms));
        }
        if (state.filters.priceMin) {
            list = list.filter(p => p.price >= parseInt(state.filters.priceMin));
        }
        if (state.filters.priceMax) {
            list = list.filter(p => p.price <= parseInt(state.filters.priceMax));
        }
        if (state.filters.wifi) {
            list = list.filter(p => p.amenities.wifi);
        }
        if (state.filters.pool) {
            list = list.filter(p => p.amenities.pool);
        }

        // update results text count
        const countSpan = document.getElementById('results-count-text');
        if (countSpan) {
            countSpan.textContent = `تعداد ${formatMoney(list.length)} اقامتگاه شگفت‌انگیز یافت شد`;
        }

        if (list.length === 0) {
            grid.innerHTML = EmptyState("اقامتگاهی با مشخصات فوق یافت نشد. لطفا فیلترها را تغییر دهید.");
        } else {
            // Sort list
            const sortVal = document.getElementById('results-sort') ? document.getElementById('results-sort').value : 'pop';
            if (sortVal === 'cheap') {
                list.sort((a, b) => a.price - b.price);
            } else if (sortVal === 'expensive') {
                list.sort((a, b) => b.price - a.price);
            }

            grid.className = "grid grid-cols-1 md:grid-cols-2 gap-6";
            grid.innerHTML = list.map(p => PropertyCard(p)).join('');
        }
    }, 500);
}

// DETAILS PAGE VIEW
function renderPropertyDetailPage(container, id) {
    const p = state.properties.find(item => item.id === id);
    if (!p) {
        container.innerHTML = ErrorState("اقامتگاه مورد نظر یافت نشد یا غیرفعال شده است.");
        return;
    }

    const activeReviews = state.reviews.filter(r => r.propertyId === p.id);
    const reviewsHTML = activeReviews.map(r => `
    <div class="border-b border-gray-100 pb-4 text-right">
        <div class="flex justify-between items-center mb-2">
            <span class="font-extrabold text-sm text-gray-900">${r.author}</span>
            <span class="text-xs text-gray-400 font-semibold">${r.date}</span>
        </div>
        <div class="flex items-center gap-1 mb-2">
            ${Array.from({ length: r.rating }).map(() => `<i class="fa-solid fa-star text-amber-500 text-[10px]"></i>`).join('')}
        </div>
        <p class="text-xs text-gray-600 leading-6 text-right">${r.comment}</p>
    </div>
    `).join('');

    const carouselIndicators = p.gallery.map((g, idx) => `
    <button onclick="changeCarouselSlide(${idx})" class="w-3 h-3 rounded-full bg-white/70 hover:bg-white shadow border border-gray-200"></button>
    `).join('');

    container.innerHTML = `
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-10 text-right">
        <!-- Back navigation button -->
        <button onclick="navigateBackToSearch()" class="mb-6 px-4 py-2 bg-gray-100 hover:bg-gray-200 text-gray-700 rounded-xl text-xs font-bold transition flex items-center gap-1">
            <i class="fa-solid fa-chevron-right text-[10px]"></i> بازگشت به نتایج جستجو
        </button>

        <div class="grid grid-cols-1 lg:grid-cols-3 gap-8 text-right">
            <!-- Left detail details & carousel Column -->
            <div class="lg:col-span-2 space-y-6">

                <!-- Title & Location Header -->
                <div class="space-y-2">
                    <div class="flex items-center gap-2">
                        <span class="bg-primary-100 text-primary text-[10px] font-bold px-2.5 py-1 rounded-lg border border-primary/10"><i class="fa-solid ${getCategoryIcon(p.category)} ml-1"></i> ${p.category}</span>
                        ${p.isPinned ? `<span class="bg-accent text-primary text-[10px] font-bold px-2.5 py-1 rounded-lg"><i class="fa-solid fa-bookmark ml-1"></i> ویژه ممتاز</span>` : ''}
                    </div>
                    <h2 class="text-2xl font-black text-gray-900 leading-9 text-right">${p.title}</h2>
                    <p class="text-xs text-gray-400 font-bold flex items-center gap-1.5 mt-2 text-right"><i class="fa-solid fa-map-pin text-accent text-xs"></i> ${p.province}، ${p.city}، ${p.address}</p>
                </div>

                <!-- Custom Image Carousel Component -->
                <div class="relative rounded-3xl overflow-hidden h-96 shadow-md border group">
                    <img id="detail-carousel-img" src="${p.image}" class="w-full h-full object-cover">
                    <!-- Carousel Left/Right indicators -->
                    <div class="absolute inset-x-0 bottom-4 flex justify-center gap-2">
                        ${carouselIndicators}
                    </div>
                </div>

                <!-- Fast Specs specs row -->
                <div class="grid grid-cols-3 gap-4 border border-gray-100 bg-white p-5 rounded-2xl shadow-sm text-right">
                    <div class="text-center">
                        <span class="text-xs text-gray-400 block font-bold">تعداد اتاق</span>
                        <span class="text-base font-extrabold text-gray-900 mt-1 block">${formatMoney(p.rooms)} خوابه</span>
                    </div>
                    <div class="text-center border-r border-l border-gray-100">
                        <span class="text-xs text-gray-400 block font-bold">ظرفیت استاندارد</span>
                        <span class="text-base font-extrabold text-gray-900 mt-1 block">${formatMoney(p.capacity)} نفر</span>
                    </div>
                    <div class="text-center">
                        <span class="text-xs text-gray-400 block font-bold">امتیاز رضایت</span>
                        <span class="text-base font-extrabold text-primary mt-1 block flex items-center justify-center gap-1"><i class="fa-solid fa-star text-amber-500"></i> ${formatRating(p.rating)}</span>
                    </div>
                </div>

                <!-- Description -->
                <div class="bg-white rounded-2xl border border-gray-100 p-6 shadow-sm space-y-3 text-right">
                    <h4 class="font-extrabold text-sm text-gray-900 border-r-4 border-accent pr-2 text-right">توضیحات اقامتگاه</h4>
                    <p class="text-xs text-gray-600 leading-7 text-justify font-medium text-right">${p.description}</p>
                </div>

                <!-- Amenities Checklist -->
                <div class="bg-white rounded-2xl border border-gray-100 p-6 shadow-sm space-y-4 text-right">
                    <h4 class="font-extrabold text-sm text-gray-900 border-r-4 border-accent pr-2 text-right">امکانات رفاهی ویژه</h4>
                    <div class="grid grid-cols-2 md:grid-cols-4 gap-4 text-xs font-semibold text-gray-700 text-right">
                        <div class="flex items-center gap-2 ${p.amenities.wifi ? 'text-primary' : 'text-gray-300'}"><i class="fa-solid fa-wifi text-base"></i> اینترنت بی‌سیم وای‌فای</div>
                        <div class="flex items-center gap-2 ${p.amenities.pool ? 'text-primary' : 'text-gray-300'}"><i class="fa-solid fa-water-ladder text-base"></i> استخر اختصاصی</div>
                        <div class="flex items-center gap-2 ${p.amenities.parking ? 'text-primary' : 'text-gray-300'}"><i class="fa-solid fa-car text-base"></i> پارکینگ خودرو</div>
                        <div class="flex items-center gap-2 ${p.amenities.ac ? 'text-primary' : 'text-gray-300'}"><i class="fa-solid fa-snowflake text-base"></i> سیستم سرمایشی (اسپلیت)</div>
                    </div>
                </div>

                <!-- Reviews and Dynamic feedback creation -->
                <div class="bg-white rounded-2xl border border-gray-100 p-6 shadow-sm space-y-6 text-right">
                    <h4 class="font-extrabold text-sm text-gray-900 border-r-4 border-accent pr-2 text-right">نظرات و ارزیابی مهمانان (${formatMoney(activeReviews.length)})</h4>

                    <div class="space-y-4 text-right">
                        ${reviewsHTML || EmptyState("هنوز هیچ نظری برای این اقامتگاه ثبت نشده است.")}
                    </div>

                    <!-- Add Comment Form -->
                    <div class="border-t border-gray-100 pt-6 space-y-4 text-right">
                        <span class="block text-xs font-bold text-gray-900"><i class="fa-solid fa-pen-nib ml-1"></i> ثبت نظر جدید درباره اقامتگاه</span>
                        <form onsubmit="handleReviewSubmit(event, ${p.id})" class="space-y-4 text-right">
                            <div class="grid grid-cols-1 md:grid-cols-2 gap-4 text-right">
                                <div class="text-right">
                                    <label class="block text-xs font-bold text-gray-500 mb-2">امتیاز شما</label>
                                    <select id="review-score" class="w-full px-3 py-2 bg-gray-50 border rounded-xl text-xs outline-none">
                                        <option value="5">۵ ستاره (عالی)</option>
                                        <option value="4">۴ ستاره (خوب)</option>
                                        <option value="3">۳ ستاره (متوسط)</option>
                                        <option value="2">۲ ستاره (ضعیف)</option>
                                        <option value="1">۱ ستاره (بسیار بد)</option>
                                    </select>
                                </div>
                            </div>
                            <div class="text-right">
                                <label class="block text-xs font-bold text-gray-500 mb-2">متن نظر شما</label>
                                <textarea id="review-comment" required placeholder="تجربه خود را از اقامت در این مکان با دیگران به اشتراک بگذارید..." rows="3" class="w-full px-4 py-2.5 bg-gray-50 border border-gray-200 rounded-xl focus:ring-2 focus:ring-primary text-xs font-semibold outline-none"></textarea>
                            </div>
                            <button type="submit" class="px-5 py-2.5 bg-primary hover:bg-primary-dark text-white rounded-xl text-xs font-bold transition shadow-sm">
                                ثبت و انتشار نظر
                            </button>
                        </form>
                    </div>
                </div>

            </div>

            <!-- Right Column Sidebar HostContactCard -->
            <div class="lg:col-span-1 space-y-6 text-right">
                <!-- Pricing detail -->
                <div class="bg-primary text-white p-6 rounded-3xl border border-accent shadow-md text-right relative overflow-hidden">
                    <div class="absolute -right-8 -bottom-8 w-24 h-24 rounded-full bg-accent opacity-10"></div>
                    <span class="text-[10px] text-accent font-bold block">قیمت بابت هر شب اجاره</span>
                    <h3 class="text-2xl font-black mt-2 text-secondary">${formatMoney(p.price)} <span class="text-xs font-semibold text-gray-300">ریال</span></h3>
                    <p class="text-[10px] text-gray-300 mt-2 font-medium leading-5"><i class="fa-solid fa-circle-info text-accent ml-1"></i> رزرو این ملک کاملاً مستقیم و بدون دریافت کارمزد از مهمان صورت می‌پذیرد.</p>
                </div>

                <!-- Host contact card reusable component -->
                ${HostContactCard(p.host, p.id)}
            </div>
        </div>
    </div>
    `;
}

function openContactModal(propertyId) {
    const p = state.properties.find(item => item.id === propertyId);
    if (!p) return;

    const modal = document.getElementById('contact-modal');
    if (modal) modal.classList.remove('hidden');

    const hName = document.getElementById('contact-host-name');
    if (hName) hName.textContent = p.host.name;

    const hiddenBlock = document.getElementById('contact-details-hidden');
    const revealedBlock = document.getElementById('contact-details-revealed');

    if (state.isLogged) {
        if (hiddenBlock) hiddenBlock.classList.add('hidden');
        if (revealedBlock) revealedBlock.classList.remove('hidden');

        const revMobile = document.getElementById('contact-revealed-mobile');
        if (revMobile) revMobile.textContent = p.host.phone;

        const cBtn = document.getElementById('contact-call-btn');
        if (cBtn) cBtn.href = `tel:${p.host.phone}`;

        const waBtn = document.getElementById('contact-wa-btn');
        if (waBtn) waBtn.href = `https://wa.me/${p.host.whatsapp}`;

        // Push contact history log if not already there
        const alreadyContacted = state.contactHistory.some(log => log.propertyId === p.id);
        if (!alreadyContacted) {
            state.contactHistory.push({
                id: Date.now(),
                propertyId: p.id,
                timestamp: 'امروز - همین حالا',
                hostName: p.host.name,
                phone: p.host.phone,
                status: 'موفق'
            });
        }
    } else {
        if (hiddenBlock) hiddenBlock.classList.remove('hidden');
        if (revealedBlock) revealedBlock.classList.add('hidden');
    }
}

function closeContactModal() {
    const modal = document.getElementById('contact-modal');
    if (modal) modal.classList.add('hidden');
}

function handleReviewSubmit(e, propertyId) {
    e.preventDefault();
    if (!state.isLogged) {
        openAuthModal('login');
        return;
    }

    const comment = document.getElementById('review-comment').value;
    const rating = parseInt(document.getElementById('review-score').value);

    // add to state reviews list
    state.reviews.push({
        id: Date.now(),
        propertyId: propertyId,
        author: state.user.name,
        rating: rating,
        date: 'امروز',
        comment: comment
    });

    // recalculate average rating for property
    const prop = state.properties.find(p => p.id === propertyId);
    if (prop) {
        const propReviews = state.reviews.filter(r => r.propertyId === propertyId);
        const total = propReviews.reduce((sum, r) => sum + r.rating, 0);
        prop.rating = total / propReviews.length;
        prop.reviewsCount = propReviews.length;
    }

    showToast("نظر شما با موفقیت ثبت شد و به صورت آنی منتشر گردید.", "success");
    renderPropertyDetailPage(document.getElementById('public-content-container'), propertyId);
}

function showPropertyDetail(id) {
    state.activeDetailId = id;
    state.publicPage = 'detail';
    renderPublic();
}

function navigateBackToSearch() {
    state.publicPage = 'search';
    renderPublic();
}

function changeCarouselSlide(idx) {
    const p = state.properties.find(item => item.id === state.activeDetailId);
    if (!p) return;
    const img = document.getElementById('detail-carousel-img');
    if (img && p.gallery[idx]) {
        img.src = p.gallery[idx];
    }
}


// SEO PAGES VIEW
function renderSEOPage(container, id) {
    state.activeSEOPage = id;
    state.publicPage = 'seo';

    let title = '';
    let content = '';

    if (id === 'travel-guides') {
        title = "راهنمای سفر لوکس به شمال و جنوب ایران";
        content = `
        <div class="space-y-6 leading-8 font-medium text-gray-700 text-right">
            <h4 class="text-lg font-bold text-primary">۱. جادوی ماسال و ییلاقات سرسبز تالش</h4>
            <p>ماسال یکی از مرتفع‌ترین ییلاقات استان گیلان و غرق در اقیانوس ابر رویایی است. برای سفر به ماسال بهترین زمان اردیبهشت تا شهریور ماه می‌باشد. کلبه‌های چوبی سنتی با دید رویایی غبارآلود بهترین انتخاب اقامتی در این بخش هستند.</p>

            <h4 class="text-lg font-bold text-primary">۲. تلاقی کوهستان و جنگل در عروس شهرهای شمال (رامسر)</h4>
            <p>رامسر غنی از استخرهای آب گرم معدنی، ویلاهای استخردار ساحلی مجلل، و جواهرده مه‌آلود است. شما می‌توانید مستقیماً تلفن میزبانان ممتاز رامسر را در املاک‌باشی برداشته و پیش از سفر ویلای ایده‌آل خود را رزرو کنید.</p>

            <h4 class="text-lg font-bold text-primary">۳. غروب طلایی و تفریحات مهیج دریایی در جزیره کیش</h4>
            <p>جزیره کیش با هتل‌ها و برج‌های دوقلوی لوکس ساحلی، انتخابی بی‌نظیر برای مسافرانی است که به دنبال تفریحات مدرن و قایق‌سواری‌های کاتاماران هستند. آپارتمان‌های پنت‌هاوس با دید دریا بالاترین تقاضا را دارند.</p>
        </div>
        `;
    } else if (id === 'about-us') {
        title = "درباره املاک‌باشی (سکو لوکس گردشگری V10)";
        content = `
        <div class="space-y-4 leading-7 text-gray-700 text-right">
            <p>املاک‌باشی با افتخار به عنوان اولین و جامع‌ترین بازارگاه هوشمند ارتباط مستقیم مهمان و میزبان در ایران فعالیت می‌کند. ما با حذف کامل واسطه‌ها و پورسانت‌های سنگین آژانس‌های آنلاین، مدل نوین و پیشتاز Lead-Generation مستقیم را به جامعه مهمان‌نوازی ارائه نموده‌ایم.</p>
            <p>در املاک‌باشی ویلاها، آپارتمان‌ها، سوئیت‌های شیک و خانه‌های سنتی بوم‌گردی تحت نظارت دقیق مدیریت تایید شده و به کاربران لوکس با بالاترین درجه استاندارد ارائه می‌گردند.</p>
        </div>
        `;
    } else if (id === 'faq') {
        title = "سوالات متداول کاربران گرامی";
        content = `
        <div class="space-y-4 text-right">
            <div class="p-4 bg-gray-50 rounded-2xl">
                <span class="block font-bold text-sm text-primary mb-1">چگونه می‌توانم شماره تلفن میزبان را مشاهده کنم؟</span>
                <p class="text-xs text-gray-600 leading-6">کافیست در سایت وارد حساب کاربری خود شوید و در صفحه جزئیات هر اقامتگاه روی دکمه طلایی «مشاهده اطلاعات تماس مستقیم» کلیک کنید.</p>
            </div>
            <div class="p-4 bg-gray-50 rounded-2xl">
                <span class="block font-bold text-sm text-primary mb-1">آیا املاک‌باشی کارمزدی دریافت می‌کند؟</span>
                <p class="text-xs text-gray-600 leading-6">خیر، در پطلافروم املاک‌باشی V10 دریافت کارمزد و پورسانت از معاملات رزرو حذف شده و تمامی پرداخت‌ها به صورت مستقیم بین مهمان و میزبان تصفیه می‌گردد.</p>
            </div>
        </div>
        `;
    } else {
        title = "قوانین و حریم خصوصی املاک‌باشی V10";
        content = `<p class="leading-7 text-gray-600 font-medium text-right">فعالیت تمامی اعضا، مهمانان و میزبانان محترم سامانه املاک‌باشی منطبق بر قوانین جاری تجارت الکترونیکی کشور و دستورالعمل‌های وزارت میراث فرهنگی، صنایع دستی و گردشگری می‌باشد. حفظ حریم خصوصی کاربران اولویت کلیدی این پلتفرم لوکس است.</p>`;
    }

    container.innerHTML = `
    <div class="max-w-4xl mx-auto px-4 py-16 text-right space-y-8">
        <h2 class="text-2xl font-black text-primary border-r-4 border-accent pr-3 leading-none">${title}</h2>
        <div class="bg-white rounded-3xl border border-gray-100 p-8 shadow-sm">
            ${content}
        </div>
        <button onclick="navigateHome()" class="px-6 py-2.5 bg-primary text-white hover:bg-primary-dark font-bold rounded-xl text-xs transition">بازگشت به صفحه اصلی</button>
    </div>
    `;
}

function showSEOPage(id) {
    state.activeSEOPage = id;
    state.publicPage = 'seo';
    switchPortal('public');
}

function toggleFavorite(id, e) {
    if (e) e.stopPropagation();
    if (!state.isLogged) {
        openAuthModal('login');
        return;
    }

    const idx = state.favorites.indexOf(id);
    if (idx !== -1) {
        state.favorites.splice(idx, 1);
        showToast("اقامتگاه از لیست علاقمندی‌ها حذف شد.");
    } else {
        state.favorites.push(id);
        showToast("اقامتگاه به لیست علاقمندی‌ها اضافه گردید.", "success");
    }

    if (state.currentPortal === 'public') renderPublic();
    if (state.currentPortal === 'guest') renderGuest();
}


// ==========================================
// SECTION 8: GUEST PANEL VIEWS IMPLEMENTATION
// ==========================================

function renderGuest() {
    const container = document.getElementById('guest-panel-main');
    if (!container) return;

    const tabs = ['profile', 'favorites', 'history', 'reviews', 'wallet'];
    tabs.forEach(t => {
        const btn = document.getElementById(`guest-tab-${t}`);
        if (btn) btn.className = "text-right px-4 py-3 rounded-xl transition hover:bg-gray-50 text-gray-600 flex items-center gap-3 w-full";
    });

    const activeBtn = document.getElementById(`guest-tab-${state.guestTab}`);
    if (activeBtn) {
        activeBtn.className = "text-right px-4 py-3 rounded-xl transition bg-primary-100 text-primary flex items-center gap-3 w-full";
    }

    if (state.guestTab === 'profile') {
        renderGuestProfile(container);
    } else if (state.guestTab === 'favorites') {
        renderGuestFavorites(container);
    } else if (state.guestTab === 'history') {
        renderGuestHistory(container);
    } else if (state.guestTab === 'reviews') {
        renderGuestReviews(container);
    } else if (state.guestTab === 'wallet') {
        renderGuestWallet(container);
    }
}

function switchGuestTab(tabName) {
    state.guestTab = tabName;
    renderGuest();
}

function renderGuestProfile(container) {
    container.innerHTML = `
    <div class="text-right space-y-6">
        <h4 class="font-extrabold text-base text-gray-900 border-r-4 border-accent pr-2.5">ویرایش پروفایل ممتاز مهمان</h4>
        <p class="text-xs text-gray-400 font-semibold text-right">مشخصات کاربری خود را در شبکه گردشگری لوکس املاک‌باشی ارتقاء دهید</p>

        <form onsubmit="saveGuestProfile(event)" class="space-y-4 grid grid-cols-1 md:grid-cols-2 gap-4 text-right">
            <div class="md:col-span-1 text-right">
                <label class="block text-xs font-bold text-gray-500 mb-2">نام و نام خانوادگی</label>
                <input required type="text" id="profile-name-input" value="${state.user.name}" class="w-full px-4 py-2.5 bg-gray-50 border rounded-xl text-xs font-bold outline-none focus:ring-2 focus:ring-primary">
            </div>
            <div class="md:col-span-1 text-right">
                <label class="block text-xs font-bold text-gray-500 mb-2">تلفن همراه (غیرقابل تغییر)</label>
                <input type="text" disabled value="${state.user.phone || '۰۹۱۲۳۴۵۶۷۸۹'}" class="w-full px-4 py-2.5 bg-gray-100 border text-gray-400 rounded-xl text-xs font-bold outline-none cursor-not-allowed">
            </div>
            <div class="md:col-span-2 pt-4">
                <button type="submit" class="px-6 py-2.5 bg-primary hover:bg-primary-dark text-white rounded-xl text-xs font-bold transition shadow-sm">
                    ذخیره تغییرات کاربری
                </button>
            </div>
        </form>
    </div>
    `;
}

function saveGuestProfile(e) {
    e.preventDefault();
    const newName = document.getElementById('profile-name-input').value;
    state.user.name = newName;
    updateHeaderAuthStatus();
    showToast("مشخصات کاربری شما با موفقیت ذخیره و به‌روزرسانی شد.", "success");
    renderGuest();
}

function renderGuestFavorites(container) {
    const favProperties = state.properties.filter(p => state.favorites.includes(p.id));

    if (favProperties.length === 0) {
        container.innerHTML = EmptyState("لیست علاقمندی‌های شما خالی است.");
        return;
    }

    const cards = favProperties.map(p => PropertyCard(p)).join('');
    container.innerHTML = `
    <div class="text-right space-y-6">
        <h4 class="font-extrabold text-base text-gray-900 border-r-4 border-accent pr-2.5">لیست علاقمندی‌های شما</h4>
        <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
            ${cards}
        </div>
    </div>
    `;
}

function renderGuestHistory(container) {
    container.innerHTML = `
    <div class="text-right space-y-6">
        <h4 class="font-extrabold text-base text-gray-900 border-r-4 border-accent pr-2.5">تاریخچه تماس با میزبانان</h4>
        <div class="overflow-x-auto border border-gray-100 rounded-2xl">
            <table class="min-w-full divide-y divide-gray-150 text-right text-xs">
                <thead class="bg-gray-50">
                    <tr>
                        <th class="px-6 py-3 text-gray-500 font-bold text-right">زمان درخواست</th>
                        <th class="px-6 py-3 text-gray-500 font-bold text-right">نام میزبان</th>
                        <th class="px-6 py-3 text-gray-500 font-bold text-right">شماره همراه مستقیم</th>
                        <th class="px-6 py-3 text-gray-500 font-bold text-right">ارتباط</th>
                    </tr>
                </thead>
                <tbody class="divide-y divide-gray-100 bg-white">
                    ${state.contactHistory.map(log => `
                    <tr>
                        <td class="px-6 py-4 text-gray-400 font-bold text-right">${log.timestamp}</td>
                        <td class="px-6 py-4 text-gray-900 font-bold text-right">${log.hostName}</td>
                        <td class="px-6 py-4 text-gray-700 tracking-widest font-bold text-right">${log.phone}</td>
                        <td class="px-6 py-4 text-right">
                            <a href="tel:${log.phone}" class="text-xs text-primary font-bold hover:underline"><i class="fa-solid fa-phone"></i> تماس تلفنی</a>
                        </td>
                    </tr>
                    `).join('')}
                </tbody>
            </table>
        </div>
    </div>
    `;
}

function renderGuestReviews(container) {
    const myReviews = state.reviews.filter(r => r.author === state.user.name);

    if (myReviews.length === 0) {
        container.innerHTML = EmptyState("شما هنوز نظری ارسال نکرده‌اید.");
        return;
    }

    const listHTML = myReviews.map(r => {
        const prop = state.properties.find(p => p.id === r.propertyId) || { title: 'اقامتگاه نمونه' };
        return `
        <div class="p-5 border border-gray-100 rounded-2xl space-y-3 relative text-right">
            <div class="flex justify-between items-center text-right">
                <span class="font-extrabold text-sm text-primary cursor-pointer hover:underline text-right" onclick="showPropertyDetail(${r.propertyId})">${prop.title}</span>
                <span class="text-xs text-gray-400 font-bold">${r.date}</span>
            </div>
            <p class="text-xs text-gray-600 leading-6 text-right">${r.comment}</p>
        </div>
        `;
    }).join('');

    container.innerHTML = `
    <div class="text-right space-y-6">
        <h4 class="font-extrabold text-base text-gray-900 border-r-4 border-accent pr-2.5">نظرات ثبت شده من</h4>
        <div class="space-y-4">
            ${listHTML}
        </div>
    </div>
    `;
}

function renderGuestWallet(container) {
    container.innerHTML = `
    <div class="text-right space-y-6">
        <h4 class="font-extrabold text-base text-gray-900 border-r-4 border-accent pr-2.5">کیف پول و مدیریت مالی</h4>
        <div class="grid grid-cols-1 md:grid-cols-3 gap-6 items-start">
            <div class="md:col-span-2 text-right">
                ${WalletCard(state.user.walletBalance)}
            </div>
            <div class="bg-gray-50 rounded-3xl p-5 border border-gray-100 text-right">
                <h5 class="font-extrabold text-xs text-gray-900 mb-3 text-right">فرم شارژ حساب</h5>
                <form onsubmit="handleWalletDeposit(event)" class="space-y-3 text-right">
                    <input required type="number" id="deposit-amount-input" placeholder="مبلغ شارژ (ریال)" class="w-full px-3 py-2 bg-white border rounded-xl text-xs font-bold outline-none text-center">
                    <button type="submit" class="w-full py-2 bg-primary hover:bg-primary-dark text-white rounded-xl text-xs font-bold transition">شارژ درگاه بانک ملی</button>
                </form>
            </div>
        </div>
        <div class="space-y-3 text-right">
            ${TransactionTable(state.transactions)}
        </div>
    </div>
    `;
}


// ==========================================
// SECTION 9: HOST PANEL VIEWS IMPLEMENTATION
// ==========================================

function renderHost() {
    const container = document.getElementById('host-panel-main');
    if (!container) return;

    const tabs = ['dashboard', 'my-ads', 'promotions', 'wallet'];
    tabs.forEach(t => {
        const btn = document.getElementById(`host-tab-${t}`);
        if (btn) btn.className = "text-right px-4 py-3 rounded-xl transition hover:bg-gray-50 text-gray-600 flex items-center gap-3 w-full";
    });

    const activeBtn = document.getElementById(`host-tab-${state.hostTab}`);
    if (activeBtn) {
        activeBtn.className = "text-right px-4 py-3 rounded-xl transition bg-primary-100 text-primary flex items-center gap-3 w-full";
    }

    if (state.hostTab === 'dashboard') {
        renderHostDashboard(container);
    } else if (state.hostTab === 'my-ads') {
        renderHostAds(container);
    } else if (state.hostTab === 'promotions') {
        renderHostPromotions(container);
    } else if (state.hostTab === 'wallet') {
        renderHostWallet(container);
    }
}

function switchHostTab(tabName) {
    state.hostTab = tabName;
    renderHost();
}

function renderHostDashboard(container) {
    const totalAds = state.properties.length;
    const pendingAds = state.properties.filter(p => p.status === 'PENDING').length;
    const leadsCount = state.contactHistory.length;

    container.innerHTML = `
    <div class="text-right space-y-8">
        <h4 class="font-extrabold text-base text-gray-900 border-r-4 border-accent pr-2.5 text-right">داشبورد آمار عملکرد میزبانی</h4>
        <div class="grid grid-cols-1 md:grid-cols-3 gap-6 text-right">
            ${DashboardCard("کل آگهی‌های ثبت شده", totalAds, "fa-folder-open", "bg-primary-100 text-primary", "دارای مجوز رسمی سامانه")}
            ${DashboardCard("آگهی‌های معلق (منتظر تایید ادمین)", pendingAds, "fa-clock", "bg-amber-100 text-amber-700", "بررسی سریع زیر ۱ ساعت")}
            ${DashboardCard("کل لیدهای دریافتی (تماس میزبان)", leadsCount + 12, "fa-mobile-screen-button", "bg-emerald-100 text-emerald-700", "ارتباط آنی و مستقیم بدون واسطه")}
        </div>
        <div class="bg-gray-50 rounded-3xl p-6 border border-gray-100 space-y-4 text-right">
            <h5 class="font-extrabold text-sm text-gray-900 text-right">آمار لیدهای ماهانه (مشتریان معرفی شده)</h5>
            <div class="flex items-end justify-between h-44 pt-4 px-4 bg-white rounded-2xl border border-gray-50">
                <div class="flex flex-col items-center gap-2 w-12">
                    <div class="w-full bg-primary/20 rounded-t-lg h-20"></div>
                    <span class="text-[10px] font-bold text-gray-400">اردیبهشت</span>
                </div>
                <div class="flex flex-col items-center gap-2 w-12">
                    <div class="w-full bg-primary/20 rounded-t-lg h-28"></div>
                    <span class="text-[10px] font-bold text-gray-400">خرداد</span>
                </div>
                <div class="flex flex-col items-center gap-2 w-12">
                    <div class="w-full bg-primary rounded-t-lg h-40"></div>
                    <span class="text-[10px] font-bold text-gray-900">تیر (جاری)</span>
                </div>
            </div>
        </div>
    </div>
    `;
}

function renderHostAds(container) {
    const cards = state.properties.map(p => AdvertisementCard(p)).join('');
    container.innerHTML = `
    <div class="text-right space-y-6">
        <div class="flex justify-between items-center text-right">
            <h4 class="font-extrabold text-base text-gray-900 border-r-4 border-accent pr-2.5 text-right">مدیریت آگهی‌های ملکی شما</h4>
            <button onclick="openAdsWizard()" class="px-4 py-2 bg-primary hover:bg-primary-dark text-white rounded-xl text-xs font-bold transition flex items-center gap-1">
                ثبت آگهی جدید (ویزارد)
            </button>
        </div>
        <div class="space-y-4">
            ${cards}
        </div>
    </div>
    `;
}

function renderHostPromotions(container) {
    container.innerHTML = `
    <div class="text-right space-y-6">
        <h4 class="font-extrabold text-base text-gray-900 border-r-4 border-accent pr-2.5 text-right">مرکز ارتقاء و نردبان آگهی‌ها (Promotions)</h4>
        ${PromotionCard()}
    </div>
    `;
}

function renderHostWallet(container) {
    container.innerHTML = `
    <div class="text-right space-y-6">
        <h4 class="font-extrabold text-base text-gray-900 border-r-4 border-accent pr-2.5 text-right">امور مالی و کیف پول میزبان</h4>
        <div class="grid grid-cols-1 md:grid-cols-3 gap-6 items-start">
            <div class="md:col-span-2">
                ${WalletCard(state.user.walletBalance)}
            </div>
            <div class="bg-gray-50 rounded-3xl p-5 border border-gray-100 text-right">
                <h5 class="font-extrabold text-xs text-gray-900 mb-3 text-right">تسویه حساب با شبا</h5>
                <form onsubmit="handleRequestPayout(event)" class="space-y-3 text-right">
                    <input required type="text" placeholder="شماره شبا (IR...)" class="w-full px-3 py-2 bg-white border rounded-xl text-xs font-bold outline-none text-center">
                    <input required type="number" id="payout-amount-input" placeholder="مبلغ درخواستی (ریال)" class="w-full px-3 py-2 bg-white border rounded-xl text-xs font-bold outline-none text-center">
                    <button type="submit" class="w-full py-2 bg-primary hover:bg-primary-dark text-white rounded-xl text-xs font-bold transition">درخواست تسویه شبا</button>
                </form>
            </div>
        </div>
        <div class="space-y-3">
            ${TransactionTable(state.transactions)}
        </div>
    </div>
    `;
}

function handleRequestPayout(e) {
    e.preventDefault();
    const amt = parseInt(document.getElementById('payout-amount-input').value);
    if (!amt || amt <= 0) return;

    if (amt > state.user.walletBalance) {
        showToast("موجودی کیف پول شما کافی نیست.", "error");
        return;
    }

    state.user.walletBalance -= amt;
    state.transactions.unshift({
        id: Date.now(),
        amount: -amt,
        type: 'درخواست تسویه شبا',
        date: 'امروز',
        refCode: 'TR-' + Math.floor(10000 + Math.random() * 90000),
        status: 'موفق',
        description: 'درخواست واریز پایا به شبا بانک کارآفرین'
    });

    updateHeaderAuthStatus();
    showToast(`درخواست تسویه ${formatMoney(amt)} ریال ثبت و تا سیکل بعدی پایا واریز خواهد شد.`, "success");
    renderHost();
}


// ADS WIZARD LOGIC (Step-by-step form controllers)
const wizardCitiesByProvince = {
    "تهران": ["تهران - دماوند", "فشم", "کردان", "شیمران"],
    "مازندران": ["رامسر", "متل قو", "چالوس", "نوشهر"],
    "هرمزگان": ["کیش", "قشم", "بندرعباس"],
    "فارس": ["شیراز", "مرودشت", "قصر دشت"]
};

function openAdsWizard() {
    state.wizardStep = 1;
    updateWizardUI();
    const modal = document.getElementById('wizard-modal');
    if (modal) modal.classList.remove('hidden');
    updateWizardCities();
}

function closeAdsWizard() {
    const modal = document.getElementById('wizard-modal');
    if (modal) modal.classList.add('hidden');
}

function updateWizardCities() {
    const prov = document.getElementById('wizard-province').value;
    const citySelect = document.getElementById('wizard-city');
    if (!citySelect) return;

    const cities = wizardCitiesByProvince[prov] || [];
    citySelect.innerHTML = cities.map(c => `<option value="${c}">${c}</option>`).join('');
}

function updateWizardUI() {
    for (let i = 1; i <= 4; i++) {
        const block = document.getElementById(`wizard-step-content-${i}`);
        const indicator = document.getElementById(`wizard-step-indicator-${i}`);

        if (i === state.wizardStep) {
            if (block) block.classList.remove('hidden');
            if (indicator) indicator.className = "w-6 h-6 rounded-full bg-accent text-primary flex items-center justify-center font-bold text-xs shadow ring-4 ring-accent/30";
        } else {
            if (block) block.classList.add('hidden');
            const past = i < state.wizardStep;
            if (indicator) indicator.className = `w-5 h-5 rounded-full flex items-center justify-center font-bold text-[10px] ${past ? 'bg-primary-light text-white' : 'bg-gray-600 text-white'}`;
        }
    }

    const pLine = document.getElementById('progress-line');
    if (pLine) {
        const progressPercent = ((state.wizardStep - 1) / 3) * 100;
        pLine.style.width = `${progressPercent}%`;
    }

    const prevBtn = document.getElementById('wizard-prev-btn');
    if (prevBtn) prevBtn.disabled = state.wizardStep === 1;

    const nextBtnSpan = document.querySelector('#wizard-next-btn span');
    if (nextBtnSpan) {
        if (state.wizardStep === 4) {
            nextBtnSpan.textContent = "تایید نهایی و ثبت آگهی";
        } else {
            nextBtnSpan.textContent = "مرحله بعد";
        }
    }
}

function wizardNext() {
    if (state.wizardStep < 4) {
        state.wizardStep++;
        updateWizardUI();
    } else {
        handleWizardSubmit(new Event('submit'));
    }
}

function wizardPrev() {
    if (state.wizardStep > 1) {
        state.wizardStep--;
        updateWizardUI();
    }
}

function formatPriceInput(el) {
    let val = el.value.replace(/,/g, '');
    if (val && !isNaN(val)) {
        el.value = Number(val).toLocaleString('en-US');
        const tomanVal = Math.floor(val / 10);
        document.getElementById('wizard-price-text').textContent = `معادل ${tomanVal.toLocaleString('fa-IR')} تومان`;
    }
}

function triggerFileSelect() {
    document.getElementById('wizard-file-input').click();
}

function handleWizardFileSelect(e) {
    const files = e.target.files;
    if (files.length > 0) {
        showToast("فایل عکس با موفقیت بارگذاری و پیش‌نمایش ذخیره شد.", "success");
    }
}

function handleWizardSubmit(e) {
    if (e) e.preventDefault();

    const title = document.getElementById('wizard-title').value;
    const category = document.getElementById('wizard-category').value;
    const province = document.getElementById('wizard-province').value;
    const city = document.getElementById('wizard-city').value;
    const priceStr = document.getElementById('wizard-price').value.replace(/,/g, '');
    const price = parseInt(priceStr) || 10000000;
    const rooms = parseInt(document.getElementById('wizard-rooms').value) || 2;
    const capacity = parseInt(document.getElementById('wizard-capacity').value) || 4;
    const desc = document.getElementById('wizard-desc').value;
    const address = document.getElementById('wizard-address').value;

    const wifi = document.getElementById('wizard-wifi').checked;
    const pool = document.getElementById('wizard-pool').checked;
    const parking = document.getElementById('wizard-parking').checked;
    const ac = document.getElementById('wizard-ac').checked;
    const bbq = document.getElementById('wizard-bbq').checked;

    const newProperty = {
        id: state.properties.length + 1,
        title: title || "اقامتگاه لوکس ثبت شده جدید",
        category: category,
        province: province,
        city: city,
        price: price,
        rooms: rooms,
        capacity: capacity,
        rating: 0.0,
        reviewsCount: 0,
        image: "https://images.unsplash.com/photo-1542718610-a1d656d1884c?auto=format&fit=crop&w=800&q=80",
        gallery: [
            "https://images.unsplash.com/photo-1542718610-a1d656d1884c?auto=format&fit=crop&w=800&q=80"
        ],
        description: desc || "توضیحاتی برای این مورد ثبت نشده است.",
        address: address || "ثبت نشده است",
        host: {
            name: state.user.name,
            phone: state.user.phone || "۰۹۱۲۳۴۵۶۷۸۹",
            whatsapp: "989123456789",
            rating: 5.0,
            isSuperhost: false,
            responseTime: "سریع"
        },
        amenities: { wifi, pool, parking, ac, bbq },
        isPinned: false,
        isLastChance: false,
        status: "PENDING"
    };

    state.properties.push(newProperty);
    closeAdsWizard();
    showToast("اقامتگاه لوکس شما با موفقیت ثبت شد و در صف تایید مدیریت قرار گرفت.", "success");

    state.hostTab = 'my-ads';
    renderHost();
}


// PROMOTIONS ACTIONS
function applyNardeban(id) {
    const cost = 2500000;
    if (state.user.walletBalance < cost) {
        showToast("موجودی کیف پول شما کافی نیست. لطفا حساب خود را شارژ کنید.", "error");
        return;
    }

    const p = state.properties.find(item => item.id === id);
    if (!p) return;

    state.user.walletBalance -= cost;
    p.isPinned = true;

    state.transactions.unshift({
        id: Date.now(),
        amount: -cost,
        type: 'خرید نردبان آگهی',
        date: 'امروز',
        refCode: 'TR-' + Math.floor(10000 + Math.random() * 90000),
        status: 'موفق',
        description: `نردبان طلایی آگهی: ${p.title}`
    });

    updateHeaderAuthStatus();
    showToast(`سرویس نردبان با موفقیت برای آگهی «${p.title}» فعال گردید.`, "success");
    renderHost();
}

function applyLastChance(id) {
    const cost = 1500000;
    if (state.user.walletBalance < cost) {
        showToast("موجودی کیف پول شما کافی نیست. لطفا حساب خود را شارژ کنید.", "error");
        return;
    }

    const p = state.properties.find(item => item.id === id);
    if (!p) return;

    state.user.walletBalance -= cost;
    p.isLastChance = true;

    state.transactions.unshift({
        id: Date.now(),
        amount: -cost,
        type: 'خرید لحظه آخری',
        date: 'امروز',
        refCode: 'TR-' + Math.floor(10000 + Math.random() * 90000),
        status: 'موفق',
        description: `فعال‌سازی لحظه آخری آگهی: ${p.title}`
    });

    updateHeaderAuthStatus();
    showToast(`سرویس لحظه آخری با موفقیت برای آگهی «${p.title}» فعال گردید.`, "success");
    renderHost();
}

function editAdvertisement(id) {
    showToast("فرم ویرایش آگهی شبیه‌سازی‌شده فعال شد. مقدار به عنوان دمو بروز شد.", "success");
    const p = state.properties.find(item => item.id === id);
    if (p) {
        p.title += " (ویرایش شده)";
        renderHost();
    }
}


// ==========================================
// SECTION 10: ADMIN PANEL VIEWS IMPLEMENTATION
// ==========================================

function renderAdmin() {
    const container = document.getElementById('admin-panel-main');
    if (!container) return;

    const tabs = ['moderation', 'users', 'listings', 'reservations', 'finance'];
    tabs.forEach(t => {
        const btn = document.getElementById(`admin-tab-${t}`);
        if (btn) btn.className = "text-right px-4 py-3 rounded-xl transition hover:bg-gray-50 text-gray-600 flex items-center gap-3 w-full";
    });

    const activeBtn = document.getElementById(`admin-tab-${state.adminTab}`);
    if (activeBtn) {
        activeBtn.className = "text-right px-4 py-3 rounded-xl transition bg-red-50 text-red-700 flex items-center gap-3 w-full";
    }

    if (state.adminTab === 'moderation') {
        renderAdminModeration(container);
    } else if (state.adminTab === 'users') {
        renderAdminUsers(container);
    } else if (state.adminTab === 'listings') {
        renderAdminListings(container);
    } else if (state.adminTab === 'reservations') {
        renderAdminReservations(container);
    } else if (state.adminTab === 'finance') {
        renderAdminFinance(container);
    }
}

function switchAdminTab(tabName) {
    state.adminTab = tabName;
    renderAdmin();
}

function renderAdminModeration(container) {
    const pendingList = state.properties.filter(p => p.status === 'PENDING');

    if (pendingList.length === 0) {
        container.innerHTML = EmptyState("هیچ آگهی معلقی در انتظار تایید مدیریت وجود ندارد.");
        return;
    }

    const cards = pendingList.map(p => `
    <div class="bg-white rounded-2xl border border-gray-100 p-5 shadow-sm space-y-4 text-right hover:border-red-100 transition duration-200">
        <div class="flex flex-col md:flex-row gap-5 text-right">
            <div class="w-32 h-24 rounded-xl overflow-hidden bg-gray-50 flex-shrink-0">
                <img src="${p.image}" class="w-full h-full object-cover">
            </div>
            <div class="space-y-1.5 flex-grow text-right">
                <span class="text-[10px] text-accent font-bold"><i class="fa-solid ${getCategoryIcon(p.category)} ml-1"></i> ${p.category}</span>
                <h5 class="font-extrabold text-sm text-gray-900 text-right">${p.title}</h5>
                <p class="text-xs text-gray-400 font-semibold text-right">${p.province}، ${p.city} | قیمت هر شب: ${formatMoney(p.price)} ریال</p>
                <p class="text-xs text-gray-500 line-clamp-2 leading-relaxed font-medium mt-2 text-right">${p.description}</p>
            </div>
        </div>

        <div class="flex justify-between items-center pt-4 border-t border-gray-50 flex-wrap gap-2 text-right">
            <span class="text-[10px] text-gray-400 font-bold block text-right">توسط میزبان: ${p.host.name}</span>
            <div class="flex gap-2 text-right">
                <button onclick="approveListing(${p.id})" class="px-4 py-2 bg-emerald-600 hover:bg-emerald-700 text-white rounded-xl text-xs font-bold transition shadow-sm flex items-center gap-1">
                    <i class="fa-solid fa-check"></i> تایید و انتشار فوری
                </button>
                <button onclick="rejectListing(${p.id})" class="px-4 py-2 bg-red-600 hover:bg-red-700 text-white rounded-xl text-xs font-bold transition shadow-sm flex items-center gap-1">
                    <i class="fa-solid fa-xmark"></i> رد درخواست
                </button>
            </div>
        </div>
    </div>
    `).join('');

    container.innerHTML = `
    <div class="text-right space-y-6">
        <h4 class="font-extrabold text-base text-gray-900 border-r-4 border-red-500 pr-2.5 text-right">تایید و تعدیل آگهی‌های جدید (Pending Moderation)</h4>
        <p class="text-xs text-gray-400 font-semibold mb-4 text-right">آگهی‌های ثبت شده میزبانان را بررسی، ویرایش، تایید یا رد کنید</p>

        <div class="space-y-4">
            ${cards}
        </div>
    </div>
    `;
}

function approveListing(id) {
    const p = state.properties.find(item => item.id === id);
    if (p) {
        p.status = 'APPROVED';
        showToast(`آگهی «${p.title}» تایید و به صورت آنی منتشر شد.`, "success");
        renderAdmin();
    }
}

function rejectListing(id) {
    const p = state.properties.find(item => item.id === id);
    if (p) {
        p.status = 'REJECTED';
        showToast(`آگهی «${p.title}» به علت عدم تطابق رد گردید.`, "error");
        renderAdmin();
    }
}

function renderAdminUsers(container) {
    container.innerHTML = `
    <div class="text-right space-y-6">
        <h4 class="font-extrabold text-base text-gray-900 border-r-4 border-red-500 pr-2.5 text-right">مدیریت کاربران و میزبانان سیستم V10</h4>
        <div class="overflow-x-auto border border-gray-100 rounded-2xl">
            <table class="min-w-full divide-y divide-gray-150 text-right text-xs">
                <thead class="bg-gray-50">
                    <tr>
                        <th class="px-6 py-3 text-gray-500 font-bold text-right">نام کاربر</th>
                        <th class="px-6 py-3 text-gray-500 font-bold text-right">شماره همراه</th>
                        <th class="px-6 py-3 text-gray-500 font-bold text-right">سطح دسترسی</th>
                        <th class="px-6 py-3 text-gray-500 font-bold text-right">تاریخ عضویت</th>
                        <th class="px-6 py-3 text-gray-500 font-bold text-right">وضعیت</th>
                    </tr>
                </thead>
                <tbody class="divide-y divide-gray-100 bg-white">
                    <tr>
                        <td class="px-6 py-4 text-gray-900 font-bold text-right">علیرضا رضایی</td>
                        <td class="px-6 py-4 text-gray-700 tracking-widest font-bold text-right">۰۹۱۲۱۱۱۱۱۱۱</td>
                        <td class="px-6 py-4 text-right"><span class="bg-amber-50 text-amber-700 border border-amber-200 px-2.5 py-0.5 rounded-lg font-bold">میزبان طلا</span></td>
                        <td class="px-6 py-4 text-gray-400 font-semibold text-right">۱۴۰۲/۰۱/۱۵</td>
                        <td class="px-6 py-4 text-right"><span class="text-emerald-600 font-bold"><i class="fa-solid fa-circle-check"></i> فعال</span></td>
                    </tr>
                    <tr>
                        <td class="px-6 py-4 text-gray-900 font-bold text-right">سارا امینی</td>
                        <td class="px-6 py-4 text-gray-700 tracking-widest font-bold text-right">۰۹۱۲۹۹۹۸۸۷۷</td>
                        <td class="px-6 py-4 text-right"><span class="bg-blue-50 text-blue-700 border border-blue-200 px-2.5 py-0.5 rounded-lg font-bold">مهمان عادی</span></td>
                        <td class="px-6 py-4 text-gray-400 font-semibold text-right">۱۴۰۲/۰۶/۲۰</td>
                        <td class="px-6 py-4 text-right"><span class="text-emerald-600 font-bold"><i class="fa-solid fa-circle-check"></i> فعال</span></td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
    `;
}

function renderAdminListings(container) {
    const list = state.properties;

    const rows = list.map(p => `
    <tr>
        <td class="px-6 py-4 text-gray-900 font-bold text-right line-clamp-1">${p.title}</td>
        <td class="px-6 py-4 text-gray-700 font-bold text-right">${p.province}، ${p.city}</td>
        <td class="px-6 py-4 text-accent font-bold text-right">${p.category}</td>
        <td class="px-6 py-4 text-gray-900 font-extrabold text-right">${formatMoney(p.price)} ریال</td>
        <td class="px-6 py-4 text-right">
            <span class="px-2 py-0.5 rounded-lg text-[10px] font-bold ${p.status === 'APPROVED' ? 'bg-emerald-50 text-emerald-700 border border-emerald-200' : 'bg-amber-50 text-amber-700 border border-amber-200'}">
                ${p.status}
            </span>
        </td>
    </tr>
    `).join('');

    container.innerHTML = `
    <div class="text-right space-y-6">
        <h4 class="font-extrabold text-base text-gray-900 border-r-4 border-red-500 pr-2.5 text-right">مدیریت اقامتگاه‌های فعال سیستم</h4>
        <div class="overflow-x-auto border border-gray-100 rounded-2xl">
            <table class="min-w-full divide-y divide-gray-150 text-right text-xs">
                <thead class="bg-gray-50">
                    <tr>
                        <th class="px-6 py-3 text-gray-500 font-bold text-right">عنوان اقامتگاه</th>
                        <th class="px-6 py-3 text-gray-500 font-bold text-right">شهر</th>
                        <th class="px-6 py-3 text-gray-500 font-bold text-right">نوع</th>
                        <th class="px-6 py-3 text-gray-500 font-bold text-right">قیمت هر شب</th>
                        <th class="px-6 py-3 text-gray-500 font-bold text-right">وضعیت تایید</th>
                    </tr>
                </thead>
                <tbody class="divide-y divide-gray-100 bg-white">
                    ${rows}
                </tbody>
            </table>
        </div>
    </div>
    `;
}

function renderAdminReservations(container) {
    container.innerHTML = `
    <div class="text-right space-y-6">
        <h4 class="font-extrabold text-base text-gray-900 border-r-4 border-red-500 pr-2.5 text-right">آرشیو رزروهای سیستمی (داده‌های تاریخی صادرشده)</h4>
        <p class="text-xs text-gray-400 font-semibold text-right">نمایش تاریخچه تراکنش‌های رزرو انجام شده برای گزارش‌گیری مالی</p>

        <div class="overflow-x-auto border border-gray-100 rounded-2xl">
            <table class="min-w-full divide-y divide-gray-150 text-right text-xs">
                <thead class="bg-gray-50">
                    <tr>
                        <th class="px-6 py-3 text-gray-500 font-bold text-right">کد فاکتور</th>
                        <th class="px-6 py-3 text-gray-500 font-bold text-right">نام مهمان</th>
                        <th class="px-6 py-3 text-gray-500 font-bold text-right">نام میزبان</th>
                        <th class="px-6 py-3 text-gray-500 font-bold text-right">اقامتگاه مورد رزرو</th>
                        <th class="px-6 py-3 text-gray-500 font-bold text-right">تاریخ اقامت</th>
                        <th class="px-6 py-3 text-gray-500 font-bold text-right">مبلغ پرداختی</th>
                    </tr>
                </thead>
                <tbody class="divide-y divide-gray-100 bg-white">
                    ${state.reservations.map(res => `
                    <tr>
                        <td class="px-6 py-4 text-gray-400 font-bold text-right tracking-wider">${res.id}</td>
                        <td class="px-6 py-4 text-gray-900 font-bold text-right">${res.guestName}</td>
                        <td class="px-6 py-4 text-gray-900 font-bold text-right">${res.hostName}</td>
                        <td class="px-6 py-4 text-gray-600 font-semibold text-right">${res.propertyTitle}</td>
                        <td class="px-6 py-4 text-gray-500 text-right">${res.dateIn} الی ${res.dateOut}</td>
                        <td class="px-6 py-4 text-emerald-700 font-extrabold text-right">${formatMoney(res.totalAmount)} ریال</td>
                    </tr>
                    `).join('')}
                </tbody>
            </table>
        </div>
    </div>
    `;
}

function renderAdminFinance(container) {
    const totalTransactions = state.transactions.reduce((sum, tx) => sum + Math.abs(tx.amount), 0);

    container.innerHTML = `
    <div class="text-right space-y-6">
        <h4 class="font-extrabold text-base text-gray-900 border-r-4 border-red-500 pr-2.5 text-right">دفتر کل مالی و حسابداری (Accounting Facade)</h4>

        <div class="grid grid-cols-1 md:grid-cols-2 gap-6 text-right">
            ${DashboardCard("حجم کل گردش تراکنش‌های دمو", formatMoney(totalTransactions) + " ریال", "fa-calculator", "bg-red-50 text-red-700 border-red-200", "حسابرسی کل سیستم")}
            ${DashboardCard("مجموع تراکنش‌های موفق شتاب", "۱۰,۰۰۰,۰۰۰ ریال", "fa-credit-card", "bg-emerald-50 text-emerald-700 border-emerald-200", "بر اساس کدهای پیگیری تایید شده")}
        </div>

        <div class="space-y-3 text-right">
            <h5 class="font-bold text-sm text-gray-900 text-right">دفتر روزنامه حسابداری کل</h5>
            ${TransactionTable(state.transactions)}
        </div>
    </div>
    `;
}


// ==========================================
// REUSABLE ACTIONS INTERFACES FOR DOM
// ==========================================

function simulateAddCash() {
    state.user.walletBalance += 10000000; // 10 Million
    state.transactions.unshift({
        id: Date.now(),
        amount: 10000000,
        type: 'شارژ هوشمند سریع',
        date: 'همین حالا',
        refCode: 'TR-' + Math.floor(10000 + Math.random() * 90000),
        status: 'موفق',
        description: 'شارژ دمو ۱ میلیون تومانی سریع'
    });

    updateHeaderAuthStatus();
    showToast("مبلغ ۱۰,۰۰۰,۰۰۰ ریال به صورت دمو شارژ شد.", "success");

    if (state.currentPortal === 'guest') renderGuest();
    if (state.currentPortal === 'host') renderHost();
}
