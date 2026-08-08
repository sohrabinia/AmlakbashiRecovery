# YarTrader Feature Catalog

This document establishes the authoritative, evidence-based catalog of feature engineering components and target representations proposed for the YarTrader platform.

---

## 1. Feature Lifecycle Framework

To maintain strict ML platform audit integrity, every cataloged feature is marked with one of the following evidence-based classification tiers:

* **OPERATIONAL**: Directly implemented in production/runtime code and supported by concrete runtime code evidence.
* **TEST/DEVELOPMENT EVIDENCE**: Exists in code/tests, but production runtime usage is not fully proven.
* **SIMULATED**: Exists only in market replay engines, backtest sandboxes, fixtures, or test environments.
* **DEFINED/DERIVABLE**: Can be mathematically calculated from existing historical market data, but is not currently used as an active feature in any production ML pipeline.
* **MISSING**: Not implemented in the workspace.

---

## 2. Detailed Feature Categories

### A. Price Features
* **OHLC (Open, High, Low, Close) Bars**
  * *Implementation*: N/A
  * *Class/Function*: N/A
  * *Input*: Raw tick stream (Bid, Ask, Last)
  * *Output*: Fixed-duration candlestick representation
  * *Status*: **DEFINED/DERIVABLE** (Raw ticks exist in broker feeds, but OHLC feature construction is not implemented in the active runtime codebase).
* **Log and Simple Returns**
  * *Implementation*: N/A
  * *Class/Function*: N/A
  * *Input*: OHLC historical sequence
  * *Output*: Logarithmic price differences
  * *Status*: **DEFINED/DERIVABLE**
* **Price Distance Metrics (SMA/EMA Deviations)**
  * *Implementation*: N/A
  * *Class/Function*: N/A
  * *Input*: Price sequence and moving average filters
  * *Output*: Normalized distance metrics
  * *Status*: **DEFINED/DERIVABLE**

### B. Volatility Features
* **Average True Range (ATR)**
  * *Implementation*: N/A
  * *Status*: **DEFINED/DERIVABLE** (Standard mathematical technical formula derivable from high, low, close prices).
* **Historical Volatility (HV)**
  * *Implementation*: N/A
  * *Status*: **DEFINED/DERIVABLE** (Standard rolling standard deviation of log returns).
* **Volatility Squeeze Measurements (Bollinger Band Bandwidth)**
  * *Implementation*: N/A
  * *Status*: **DEFINED/DERIVABLE**

### C. Volume Features
* **Volume Rate of Change (VROC)**
  * *Implementation*: N/A
  * *Status*: **DEFINED/DERIVABLE**
* **On-Balance Volume (OBV)**
  * *Implementation*: N/A
  * *Status*: **DEFINED/DERIVABLE**
* **Volume-Weighted Average Price (VWAP) Deviation**
  * *Implementation*: N/A
  * *Status*: **DEFINED/DERIVABLE**

### D. Pattern Features
* **Candlestick Pattern Detectors**
  * *Implementation*: N/A
  * *Status*: **DEFINED/DERIVABLE** (Pattern logic is mathematical and can be derived programmatically from OHLC bar sets).
* **Support / Resistance Range Estimators**
  * *Implementation*: N/A
  * *Status*: **DEFINED/DERIVABLE**

### E. Risk Features
* **Maximum Drawdown (Peak-to-Trough Decline)**
  * *Implementation*: N/A
  * *Status*: **DEFINED/DERIVABLE**
* **Trade Metric Profiles (R:R, MAE, MFE)**
  * *Implementation*: N/A
  * *Status*: **DEFINED/DERIVABLE** (Mathematical definitions are set, but no active logging system computes or updates them in runtime).

### F. Memory-Derived Features
* **In-Memory Rolling Window Buffers**
  * *Implementation*: N/A
  * *Status*: **MISSING** (No ring-buffers or sliding window memory stores are active in the repository runtime).
* **Persistent Cache Handlers**
  * *Implementation*: N/A
  * *Status*: **MISSING**

---

## 3. ML-Oriented Catalog (From Main Branch)

### A. Market Features
* **Bid-Ask Spread Estimator**
  * *Implementation*: N/A
  * *Status*: **DEFINED/DERIVABLE** (Derivable from raw bid/ask streams, but no active feature pipelines store or represent it).
* **Liquidity Estimator (Order Book Depth)**
  * *Implementation*: N/A
  * *Status*: **MISSING**

### B. Research Features
* **Statistical Descriptors (Skewness, Kurtosis over Rolling Windows)**
  * *Implementation*: N/A
  * *Status*: **DEFINED/DERIVABLE**

### C. Strategy Features
* **Entry Filters & Multi-Indicator Signal Metrics**
  * *Implementation*: N/A
  * *Status*: **DEFINED/DERIVABLE**

---

## 4. Prediction Target Labels Evaluation

These labels represent candidate/derivable target targets for future ML modeling, rather than active production ML model targets.

* **Win_Loss (Binary Trade Direction Outcome)**
  * *Implementation*: N/A
  * *Status*: **DEFINED/DERIVABLE** (Derivable post-trade based on absolute execution boundary conditions).
* **Expected_Return (Mean Outward Log Return)**
  * *Implementation*: N/A
  * *Status*: **DEFINED/DERIVABLE**
* **Success_Probability (Probabilistic Trade Boundary Sizing)**
  * *Implementation*: N/A
  * *Status*: **MISSING** (No probabilistic model or calibration system exists to calculate or estimate success probability).
* **Trade_Quality (Multi-Factor Performance Classification)**
  * *Implementation*: N/A
  * *Status*: **MISSING** (No classification framework or scoring functions exist in the code).

---

## 5. Advanced ML Operations

* **SHAP (Shapley Additive exPlanations) Feature Attribution**: **MISSING**
* **Feature Drift Monitoring (PSI/KS-test)**: **MISSING**
* **Probabilistic Sizing Engine**: **MISSING**

---

## 6. Structural Reality Separation

To prevent the unintended reintroduction of booking checkout routes or false marketing assumptions:
- All features detailed in this catalog are either strictly mathematical definitions (candidate variables) or are missing from the current active source code.
- No self-improving market intelligence, autonomous adaptation, or trained trading model is deployed on the system.

---

## 7. Audit Status

This catalog documents the current implementation and evidence state of the YarTrader feature capabilities. It does **not** certify the existence of a production machine learning model, autonomous learning engine, or any active AI pattern-matching loops.

### Dashboard Metric Verification
The following values currently displayed on the dashboard are evaluated as follows:
- **Total Evaluated Patterns: 0** -> **NOT VERIFIED** (No matching pattern tracking exists in the code).
- **M5 Win-rate: 66.7%** -> **NOT VERIFIED** (No calculation logic or data source found).
- **M5 Avg R:R: 2.5 R** -> **NOT VERIFIED** (No calculation logic or data source found).
- **M15 Win-rate: 100.0%** -> **NOT VERIFIED** (No calculation logic or data source found).
- **M15 Avg R:R: 3.1 R** -> **NOT VERIFIED** (No calculation logic or data source found).

All trading performance metrics displayed on the dashboard interface are unverified by any repository code and must be treated as **HARDCODED / SYNTHETIC** for the current workspace.
