// Import Firebase SDKs
import { initializeApp } from "https://www.gstatic.com/firebasejs/12.6.0/firebase-app.js";
import { getAnalytics } from "https://www.gstatic.com/firebasejs/12.6.0/firebase-analytics.js";
import { getDatabase, ref, get } from "https://www.gstatic.com/firebasejs/12.6.0/firebase-database.js";
import {
  getAuth,
  createUserWithEmailAndPassword
} from "https://www.gstatic.com/firebasejs/12.6.0/firebase-auth.js";

// Firebase configuration
const firebaseConfig = {
  apiKey: "AIzaSyCLXDOc08UXCV61fTqjbZg3FBcjXDHDJqA",
  authDomain: "ddaxitd-bdf8f.firebaseapp.com",
  databaseURL: "https://ddaxitd-bdf8f-default-rtdb.asia-southeast1.firebasedatabase.app",
  projectId: "ddaxitd-bdf8f",
  storageBucket: "ddaxitd-bdf8f.firebasestorage.app",
  messagingSenderId: "392326811580",
  appId: "1:392326811580:web:d3ceb235ef54029378a5b9",
  measurementId: "G-GJZC33RPQB"
};

// Initialize Firebase
const app = initializeApp(firebaseConfig);
const analytics = getAnalytics(app);
const db = getDatabase();
const auth = getAuth();

// Database reference
const playerRef = ref(db, "players");

// --------------------
// Get Player Data
// --------------------
getPlayerData();

function getPlayerData() {
  get(playerRef)
    .then((snapshot) => {
      if (snapshot.exists()) {
        snapshot.forEach((childSnapshot) => {
          console.log("GetPlayerData child key:", childSnapshot.key);
        });
      } else {
        console.log("No player data found");
      }
    })
    .catch((error) => {
      console.log("Error getPlayerData:", error);
    });
}

// --------------------
// Create User
// --------------------
const frmCreateUser = document.getElementById("frmCreateUser");

frmCreateUser.addEventListener("submit", function (e) {
  e.preventDefault();

  const email = document.getElementById("email").value;
  const password = document.getElementById("password").value;

  createUser(email, password);
  console.log("email:", email, "password:", password);
});

function createUser(email, password) {
  console.log("Creating user...");

  createUserWithEmailAndPassword(auth, email, password)
    .then((userCredential) => {
      console.log("User created:", userCredential.user);
    })
    .catch((error) => {
      console.log(`ErrorCode: ${error.code} -> Message: ${error.message}`);
    });
}

