# 📱 Book Store App (MAUI)

This is a cross-platform mobile application built with **.NET MAUI**, designed to provide a modern and responsive UI for browsing, selecting, and ordering books. It integrates with a custom **ASP.NET Core Web API** for authentication, data management, and ordering functionality.

The app also features **offline support** using SQLite, allowing users to browse cached books even when not connected to the internet.

---

## 🎯 Key Features

- 🔐 JWT Authentication (via backend API)
- 📘 Browse books with search and filtering
- 📄 Book details with full information
- 🛒 Add books to cart and place orders
- 💾 Offline support with local SQLite storage
- 🎨 Modern, responsive, and intuitive UI using .NET MAUI

---

## 🗂️ Main Screens

| Page             | Description                                                                 |
|------------------|-----------------------------------------------------------------------------|
| **Home Page**     | Shows featured or new books and provides navigation to other sections       |
| **All Books Page**| Displays all available books with pagination, filtering, and search         |
| **Book Details**  | Shows detailed information about a selected book                            |
| **Cart Page**     | Displays the user’s cart with options to remove items or proceed to order   |
| **Login/Register**| Authentication pages for users                                              |
| **Orders Page**   | (Optional) Shows order history for logged-in users                          |

> 📌 Note: Unregistered users can browse public pages, but must log in to place orders.

---

## 🌐 API Integration

This app communicates with the [Book Store API](https://github.com/Assaad-Assaad/BookstoreApi) for:
- Authentication and JWT token handling
- Book data retrieval
- Placing and managing orders

---

## 🗄️ Offline Functionality

- **SQLite** is used to store a local copy of the book list
- Users can browse cached books when offline
- Orders and cart actions require internet connectivity

---

## 🛠️ Technologies Used

- **.NET 8**
- **.NET MAUI**
- **SQLite.NET** for local database
- **HttpClient** + JWT Bearer for secure API calls
- **MVVM** architecture pattern
- **Dependency Injection**
- **Data annotations** for form validation

---

## 🧪 Getting Started

### 📦 Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- Visual Studio 2022+ with .NET MAUI workload
- Android/iOS emulator or device
- The backend [Book Store API](https://github.com/Assaad-Assaad/BookstoreApi) running and accessible


