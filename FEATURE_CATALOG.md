# TradeYar AI Machine Learning Feature Catalog

---

## 1. Purpose and Scope
This Feature Catalog details the machine learning feature engineering representations, technical indicators, and prediction targets mapped for the TradeYar AI platform. It serves as an authoritative specification for feature engineering, establishing clear operational boundaries between existing infrastructure and proposed machine learning components.

---

## 2. Feature Lifecycle
Features tracked in this catalog progress through a defined lifecycle based on code readiness and evidence of execution:

* **Implemented**: Feature engineering code is written and integrated into core source modules.
* **Tested**: Tests exist in the test suite to verify calculation accuracy under fixed inputs.
* **Persisted**: Feature values are successfully recorded in persistent state buffers or databases.
* **Runtime-consumed**: Features are actively consumed by the trading engine or live signals.
* **Simulation-only**: Feature calculation is restricted to offline replays, sandboxes, fixtures, or backtesting.
* **Documentation/specification only**: Feature definition exists strictly as a design pattern or target specification.

---

## 3. Existing Operational Features

### Price Features
* **OHLC Candlesticks & Returns**: Standard multi-timeframe bar representations and rolling logarithmic price differences.
  * *Status*: **NOT VERIFIED** (No active ingestion or OHLC feature engineering code is present in this workspace).
* **Price Distance Indicators**: standard distance of prices from major Simple and Exponential Moving Averages (SMA/EMA).
  * *Status*: **NOT VERIFIED**

### Volatility Features
* **Average True Range (ATR)**: Standard technical volatility measure representing absolute transaction ranges.
  * *Status*: **NOT VERIFIED**
* **Rolling Volatility Index**: Standard rolling standard deviation of log returns.
  * *Status*: **NOT VERIFIED**

### Volume Features
* **Volume Rate of Change (VROC)**: Tracking the percentage change in asset trading volume.
  * *Status*: **NOT VERIFIED**
* **On-Balance Volume (OBV)**: Running cumulative sum of volume flow indicating buy/sell pressure.
  * *Status*: **NOT VERIFIED**
* **Volume-Weighted Average Price (VWAP) Deviation**: Measurement of current price distance from intraday VWAP.
  * *Status*: **NOT VERIFIED**

### Pattern / Structure Features
* **Candlestick Structure Detectors**: Rule-based classifiers for classic patterns (e.g., Doji, Hammer, Engulfing).
  * *Status*: **NOT VERIFIED**
* **Support and Resistance Extremas**: Historical price extremas and local boundaries.
  * *Status*: **NOT VERIFIED**

### Strategy Features
* **Entry Filters & Signal Triggers**: Deterministic threshold gates derived from traditional technical indicators (e.g., RSI, MACD, Stochastic Oscillators).
  * *Status*: **NOT VERIFIED**

### Risk Features
* **Maximum Drawdown Tracker**: Calculates peek-to-trough drop over specified observation periods.
  * *Status*: **NOT VERIFIED**
* **Trade Metric Profiles (R:R, MAE, MFE)**: Standard trade statistics.
  * *Status*: **NOT VERIFIED**

### Memory-derived Features
* **Rolling Window Buffers**: Bound ring-buffers storing recent tick streams.
  * *Status*: **NOT VERIFIED**

---

## 4. Feature Sources and Runtime Evidence
Every claim of functionality within this catalog is subject to direct repository search validation.

Since the current active workspace does not contain the YarTrader trading or ML codebases, **no runtime evidence is verified** in the active environment.
- All referenced features are categorized as **NOT VERIFIED** regarding their runtime consumption and production deployment. No proprietary AI trading edge has been verified in the codebase.

---

## 5. Simulated / Test-only Features
All advanced features involving simulated tick feed replay, synthetic indicators, and backtest-specific indicators are classified as **NOT VERIFIED / DOCUMENTATION ONLY**. There are no active backtest replays or simulation engines present in the local codebase.

---

## 6. Missing ML-specific Features
The following advanced machine learning operations are completely absent from the current active repository code:

* **SHAP (Shapley Additive exPlanations) Feature Importance**: **MISSING** (No SHAP calculation, feature attribution, or explainability framework is implemented).
* **Feature Drift Metrics**: **MISSING** (No Population Stability Index [PSI], KS-test trackers, or data drift detection mechanisms exist).
* **Calibrated Probabilistic Sizing**: **MISSING** (No dynamic positioning or sizing engines guided by machine learning output probabilities are active).

---

## 7. Prediction Targets

These items represent candidate/derivable target labels for future machine learning model training and evaluation. They do **not** represent active outputs from any trained production machine learning model.

* **Win_Loss**: Binary trade direction outcome defined strictly by absolute target/stop execution boundaries.
  * *Status*: **NOT VERIFIED / DEFINED ONLY** (Can support future training but no active target calculation is implemented in runtime).
* **Expected_Return**: Mean logarithmic return over defined post-trade holding periods.
  * *Status*: **NOT VERIFIED / DEFINED ONLY**
* **Success_Probability**: Multi-factor model probabilistic estimation of trade boundary success.
  * *Status*: **MISSING** (No probabilistic model, calibration algorithm, or success probability engine exists).
* **Trade_Quality**: Categorical rating based on drawdown minimization and execution efficiency.
  * *Status*: **MISSING**
