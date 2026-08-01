// Initialize Firebase
var config = {
apiKey: "AIzaSyCaqJAXNu2zAqMJLqk1EF45PAx6QYtcqZg",
authDomain: "amlakbashi-7e6b2.firebaseapp.com",
databaseURL: "https://amlakbashi-7e6b2.firebaseio.com",
projectId: "amlakbashi-7e6b2",
storageBucket: "amlakbashi-7e6b2.appspot.com",
messagingSenderId: "173434342546"
};
firebase.initializeApp(config);

// Retrieve Firebase Messaging object.
const messaging = firebase.messaging();
// Add the public key generated from the console here.
messaging.usePublicVapidKey("BEqrg-tXWz1Cugk_uBMJq0P-z6etCVnnBcwdc4_EYsNcOYGmWseqvi2fUTPqi6pCqVGlAv4-KxiEFCAzZ6JRNu4");