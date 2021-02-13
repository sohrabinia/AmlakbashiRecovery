const CACHE_VERSION = 36;
const staticCacheName = 'file-cache-v' + CACHE_VERSION;
var CURRENT_CACHES = {
    ajaxrequest: 'ajaxrequest-cahce-v' + CACHE_VERSION
};
const OFFLINE_URL = 'offline.html';
const precacheResources = [
  'offline.html',
  'manifest.json',
  'favicon.png',
  'favicon.ico',
  'Resource/img/siteicons/icon-24x24.png',
  'Resource/img/siteicons/icon-36x36.png',
  'Resource/img/siteicons/icon-48x48.png',
  'Resource/img/siteicons/icon-72x72.png',
  'Resource/img/siteicons/icon-96x96.png',
  'Resource/img/siteicons/icon-128x128.png',
  'Resource/img/siteicons/icon-144x144.png',
  'Resource/img/siteicons/icon-152x152.png',
  'Resource/img/siteicons/icon-192x192.png',
  'Resource/img/siteicons/icon-384x384.png',
  'Resource/img/siteicons/icon-512x512.png',
  'resource/img/amlakbashi-logo-680w.gif',
  'resource/tmmegamenu/css/fonts/fontawesome-webfont862f.woff?v=4.1.0',
  'resource/images/prev.png',
  'resource/images/next.png',
  'resource/images/loading.gif',
  'resource/images/close.png',
  'resource/fonts/IRANSans-Medium-web.woff',
  'resource/fonts/IRANSans-Light-web.woff',
  'resource/fonts/IRANSans-Bold-web.woff',
  'resource/fontawesome-free-5.6.3-web/webfonts/fa-solid-900.woff2',
  'resource/fontawesome-free-5.0.8/web-fonts-with-css/webfonts/fa-solid-900.woff2',
  'resource/img/logo.gif'
];

self.addEventListener('install', event => {
    self.skipWaiting();
    //console.log('Service worker install event!');
    event.waitUntil(
      caches.open(staticCacheName)
        .then(cache => {
            return cache.addAll(precacheResources);
        })
    );
});

self.addEventListener('activate', event => {
    //console.log('Activating new service worker...');

    const cacheWhitelist = [staticCacheName].concat(Object.values(CURRENT_CACHES));

    event.waitUntil(
      caches.keys().then(cacheNames => {
          return Promise.all(
            cacheNames.map(cacheName => {
                if (cacheWhitelist.indexOf(cacheName) === -1) {
                    return caches.delete(cacheName);
                }
            })
          );
      })
    );

    // Active worker won't be treated as activated until promise
    // resolves successfully.
    event.waitUntil(
      caches.keys().then(function (cacheNames) {
          return Promise.all(
            cacheNames.map(function (cacheName) {
                if (!cacheWhitelist.includes(cacheName)) {
                    //console.log('Deleting out of date cache:', cacheName);

                    return caches.delete(cacheName);
                }
            })
          );
      })
    );


});

self.addEventListener('fetch', event => {
    var url_lower = event.request.url.toLowerCase();
    if (url_lower.match(/resourceimage/g) ||
        url_lower.match(/mostviewcities/g) ||
        url_lower.match(/getjs/g) ||
        url_lower.match(/getcss/g) ||
        url_lower.match(/homepageslider/g)
        )
    {
        //console.log('Handling fetch event for', event.request.url);

        event.respondWith(

          // Opens Cache objects that start with 'font'.
          caches.open(CURRENT_CACHES['ajaxrequest']).then(function (cache) {
              return cache.match(event.request).then(function (response) {
                  if (response) {
                      //console.log('Found response in cache:', response);

                      return response;
                  }
                  else {
                      return fetch(event.request).then(function (networkResponse) {
                          if (!networkResponse.ok) {
                              throw new TypeError('Bad response status');
                          }
                          //console.log('Fetching request from the network: ' + event.request.url);
                          cache.put(event.request, networkResponse.clone());

                          return networkResponse;
                      });
                  }
              }).catch(function (error) {

                  // Handles exceptions that arise from match() or fetch().
                  //console.error('Error in fetch handler:', error);

                  throw error;
              });
          })
        );
    }
    else
    {
        if (event.request.cache === 'only-if-cached' && event.request.mode !== 'same-origin') {
            return;
        }
        event.respondWith(
            caches.open(staticCacheName).then(function (cache) {
                return cache.match(event.request).then(function (response) {
                    if (response) {
                        //console.log('Found response in cache:', response);

                        return response;
                    }
                    else {
                        return fetch(event.request).then(function (networkResponse) {
                            //console.log('Fetching request from the network: ' + event.request.url);
                            return networkResponse;
                        }).catch(function (error) {
                            //console.log('Error: Fetching request from the network: ' + event.request.url + ' ErrorMessage: ' + error);
                            if (event.request.mode === 'navigate' ||
                              (event.request.method === 'GET' &&
                               event.request.headers.get('accept').includes('text/html'))) {
                                //console.log('now loading offline html. request:' + event.request.url);
                                return caches.match('/offline.html');
                            }
                            else {
                                //console.log('resource not found in cache. request:' + event.request.url)
                                return;
                            }
                        });
                    }
                })
            })
        );
    }
});

// Give the service worker access to Firebase Messaging.
// Note that you can only use Firebase Messaging here, other Firebase libraries
// are not available in the service worker.
importScripts('https://www.gstatic.com/firebasejs/5.9.4/firebase-app.js');
importScripts('https://www.gstatic.com/firebasejs/5.9.4/firebase-messaging.js');

 //Initialize the Firebase app in the service worker by passing in the
 //messagingSenderId.
//firebase.initializeApp({
//    'messagingSenderId': '173434342546'
//});

// Retrieve an instance of Firebase Messaging so that it can handle background
// messages.

var config = {
    apiKey: "AIzaSyCaqJAXNu2zAqMJLqk1EF45PAx6QYtcqZg",
    authDomain: "amlakbashi-7e6b2.firebaseapp.com",
    databaseURL: "https://amlakbashi-7e6b2.firebaseio.com",
    projectId: "amlakbashi-7e6b2",
    storageBucket: "amlakbashi-7e6b2.appspot.com",
    messagingSenderId: "173434342546"
};
firebase.initializeApp(config);

const messaging = firebase.messaging();

console.log('firebase messaging initialized. ', messaging);

// If you would like to customize notifications that are received in the background (Web app is closed or not in browser focus) then you should implement this optional method
messaging.setBackgroundMessageHandler(function (payload) {
    self.registration.hideNotification();
    //console.log('[service_worker.js] Received background message ', payload);
    //// Customize notification here
    //var notificationTitle = payload.notification.title;
    //var actions = [];
    //var data = payload.data;
    //data.url = "https://www.amlakbashi.com" + data.url;
    //if (payload.data.btn1) {
    //    actions.push(
    //        { action: payload.data.btn1, title: payload.data.btn1_title }
    //    );
    //}
    //if (payload.data.btn2) {
    //    actions.push(
    //        { action: payload.data.btn2, title: payload.data.btn2_title }
    //    );
    //}
    //if (payload.data.btn3) {
    //    actions.push(
    //        { action: payload.data.btn3, title: payload.data.btn3_title }
    //    );
    //}
    //if (payload.data.btn4) {
    //    actions.push(
    //        { action: payload.data.btn4, title: payload.data.btn4_title }
    //    );
    //}
    //var notificationOptions = {
    //    body: payload.notification.body,
    //    data: data,
    //    actions: actions
    //};
    //return self.registration.showNotification(notificationTitle,
    //    notificationOptions);
});

self.addEventListener('push', function (event) {
        console.log('[service_worker.js] Received push message event ', event);
        var payload = {};
        if (event.data) {
            payload = event.data.json();
        }
        console.log('[service_worker.js] Received push message ', payload);
        // Customize notification here
        var actions = [];
        if (payload.data.btn1) {
            actions.push(
                { action: payload.data.btn1, title: payload.data.btn1_title }
            );
        }
        if (payload.data.btn2) {
            actions.push(
                { action: payload.data.btn2, title: payload.data.btn2_title }
            );
        }
        if (payload.data.btn3) {
            actions.push(
                { action: payload.data.btn3, title: payload.data.btn3_title }
            );
        }
        if (payload.data.btn4) {
            actions.push(
                { action: payload.data.btn4, title: payload.data.btn4_title }
            );
        }
        var notificationOptions = {
            body: payload.notification.body,
            icon: '/Resource/img/siteicons/icon-144x144.png',
            badge: '/Resource/img/siteicons/badge.png',
            data: payload.data,
            actions: actions,
            vibrate: [200, 100, 200, 100, 200, 100, 200]
        };
        return self.registration.showNotification(payload.notification.title,
            notificationOptions);
});


self.addEventListener('notificationclick', function (event) {
    //console.log('[service_worker.js] Notification click detected ', event);
    event.notification.close();
    if (event.action) {
        //console.log('[service_worker.js] answered: ' + event.action);
        clients.openWindow(event.notification.data[event.action + "_url"]);
    }
    else {
        //console.log('[service_worker.js] not answered ');
        clients.openWindow(event.notification.data.url);
    }
});