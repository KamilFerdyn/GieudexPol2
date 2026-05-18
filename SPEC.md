# SPECIFICATION: GieudexPol Trading Engine (v1.0)

## 🎯 1. Project Overview & Goals

*   **Project Name:** GieudexPol
*   **Mission Statement:** To maximize profit from market fluctuations by providing advanced live data analytics and arbitrage detection capabilities.
*   **Core Functionality:** Automated aggregation of foreign exchange rates from multiple sources (e.g., NBP, commercial APIs) to identify real-time pricing discrepancies for profitable trading opportunities.
*   **Key Objectives:**
    *   **Data Aggregation:** Seamless fetching of live currency rates from diverse external APIs.
    *   **Margin Engine:** Real-time calculation and application of transaction commissions/margins on base rates.
    *   **Anomaly Detection:** Implementing an algorithm to flag price errors or sudden, significant rate spikes.
    *   **Notification Speed:** Minimizing latency between opportunity detection and user notification.

## 🏗️ 2. Architecture & Technology Stack

*   **Architecture Pattern:** Clean Architecture (Ensuring separation of concerns: Domain -> Application -> Infrastructure).
*   **Technology Stack:**
    *   **Frontend:** Angular 21 (Reactive client interface).
    *   **Backend:** .NET 10 WebAPI.
    *   **Database:** MS SQL Server (Relational data storage for history and profiles).
    *   **AI/Agent Support:** Cline (Gemini-2.5-flash) Agent, utilized for code development assistance and auditing via an Agentic Loop methodology.
    *   **Documentation:** PlantUML (For architectural diagrams).

### 📂 Directory Structure:
*   `GieudexPol.Domain`: Core entities (`Currency`, `Rate`, `Alert`) and business rules.
*   `GieudexPol.Application`: Business services (BLL), calculation logic, and notification interfaces.
*   `GieudexPol.Infrastructure`: Data Access Layer (DAL) implementation using Entity Framework Core; handles external API integration (e.g., bank APIs).
*   `GieudexPol.API`: REST Controllers and Middleware (JWT, CORS) serving as the primary entry point for the Angular frontend.

## 💾 3. Data Model & Entities (MS SQL Server Schema)

*   **Users:** Stores profile data, hashed passwords (BCrypt/Identity), and user roles (Admin/User).
*   **Wallets:** Tracks the current balance of specific currencies for a given user.
*   **Currencies:** Defines all managed assets (Symbol, Name, Activity Status).
*   **ExchangeRates:** Historical records of rates (Buy/Sell) with high precision (`decimal(18,4)`).
*   **Transactions:** Immutable ledger of all operations (buy/sell), including calculated commissions.
*   **UserAlerts:** Configuration for user-defined price thresholds and associated currencies.

## 🚀 4. System Scope & Functionalities

### A. User Features (Client Facing)
1.  **Authentication:** Full registration, login mechanism with server-side validation. Dashboard defaults to the login screen.
2.  **Digital Wallet:** Display of current balance and available funds for trading.
3.  **Order Placement:** Intuitive form for market buy/sell orders at prevailing rates.
4.  **Orderbook:** Real-time display of active user bids and asks.
5.  **Interactive Charts:** Visualization tools for price trend analysis.
6.  **Transaction History:** Comprehensive, auditable log of all operations (deposits, trades).
7.  **Price Alerts:** System to notify users when an asset reaches a specified target price.
8.  **Wallet Management (NEW):** Centralized module for viewing and executing currency exchange transactions. Wykorzystuje endpoint `POST /api/wallet/trade` do bezpiecznego transferu środków między walutami użytkownika, zarządzając stanem salda w rekordzie `Wallets`.

### B. Administrator Features (Management)
1.  **User Management:** Full CRUD capabilities for user profiles (blocking, deletion, password reset).
2.  **Commission Configuration:** Global setting for percentage-based transaction fees.
3.  **Market Management:** Ability to add new trading pairs and temporarily suspend markets.
4.  **Security Monitoring:** Viewing system logs and detecting suspicious activity/intrusion attempts.
5.  **Financial Reporting:** Generating reports on total turnover volume and platform profit.

## ⚙️ 5. Deployment & Setup Instructions (DevOps)

*   **Deployment Platform:** Railway.app (Production URL: [URL]).
*   **Local Setup Method:** Docker Compose (Recommended for isolated environment).
    1.  `git clone ...`
    2.  `docker-compose up -d` (Builds all necessary services: DB, API, Frontend).
*   **Database Initialization:** Use `dotnet ef database update` to apply migrations and seed initial data if needed.