import React from "react";
import { useState } from "react";

const symbols = {
  add: "+",
  subtract: "−",
  multiply: "×",
  divide: "÷",
  modulus: "%",
  power: "^",
  sqrt: "√",
};

function formatNumber(value) {
  return new Intl.NumberFormat("en-US", {
    maximumFractionDigits: 10,
  }).format(value);
}

export default function App() {
  const [display, setDisplay] = useState("0");
  const [storedNumber, setStoredNumber] = useState(null);
  const [operation, setOperation] = useState(null);
  const [waitingForNumber, setWaitingForNumber] = useState(false);
  const [history, setHistory] = useState([]);
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);

  // Keep calculations in the C# API; React only sends input and renders the result.
  async function calculate(firstNumber, selectedOperation, secondNumber = null) {
    setLoading(true);
    setError("");

    try {
      const response = await fetch("/api/calculator/calculate", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          operation: selectedOperation,
          firstNumber,
          secondNumber,
        }),
      });
      const data = await response.json();

      if (!response.ok) {
        throw new Error(data.detail || "The calculation could not be completed.");
      }

      return data.result;
    } catch (requestError) {
      setError(
        requestError.message === "Failed to fetch"
          ? "Cannot reach the C# calculator. Start the API and try again."
          : requestError.message,
      );
      return null;
    } finally {
      setLoading(false);
    }
  }

  function enterDigit(digit) {
    setError("");
    if (waitingForNumber) {
      setDisplay(digit);
      setWaitingForNumber(false);
      return;
    }

    setDisplay((current) => current === "0" ? digit : current + digit);
  }

  function enterDecimal() {
    setError("");
    if (waitingForNumber) {
      setDisplay("0.");
      setWaitingForNumber(false);
    } else if (!display.includes(".")) {
      setDisplay((current) => current + ".");
    }
  }

  function chooseOperation(nextOperation) {
    const currentNumber = Number(display);
    setError("");

    if (nextOperation === "sqrt") {
      void (async () => {
        const result = await calculate(currentNumber, "sqrt");
        if (result !== null) {
          setHistory((items) => [
            { expression: `√ ${formatNumber(currentNumber)}`, result },
            ...items,
          ].slice(0, 5));
          setDisplay(String(result));
          setWaitingForNumber(true);
        }
      })();
      return;
    }

    setStoredNumber(currentNumber);
    setOperation(nextOperation);
    setWaitingForNumber(true);
  }

  async function getResult() {
    if (storedNumber === null || operation === null) return;

    const secondNumber = Number(display);
    const result = await calculate(storedNumber, operation, secondNumber);
    if (result === null) return;

    setHistory((items) => [
      {
        expression: `${formatNumber(storedNumber)} ${symbols[operation]} ${formatNumber(secondNumber)}`,
        result,
      },
      ...items,
    ].slice(0, 5));
    setDisplay(String(result));
    setStoredNumber(null);
    setOperation(null);
    setWaitingForNumber(true);
  }

  function clear() {
    setDisplay("0");
    setStoredNumber(null);
    setOperation(null);
    setWaitingForNumber(false);
    setError("");
  }

  function backspace() {
    if (waitingForNumber) return;
    setDisplay((current) => current.length > 1 ? current.slice(0, -1) : "0");
  }

  return (
    <main className="page-shell">
      <header className="topbar">
        <a className="brand" href="/" aria-label="Clearline home">
          <span className="brand-mark">C</span>
          <span>clearline<span className="brand-light">.calc</span></span>
        </a>
        <span className="api-status"><span className="status-dot" /> C# engine</span>
      </header>

      <section className="workspace" aria-label="Calculator">
        <div className="intro">
          <p className="eyebrow">YOUR EVERYDAY TOOL</p>
          <h1>Make numbers<br /><span>make sense.</span></h1>
          <p className="intro-copy">A little clarity, one calculation at a time.</p>
          <div className="feature-list">
            <div><span className="feature-icon">↗</span><span>Simple, focused workspace</span></div>
            <div><span className="feature-icon">⌘</span><span>Powered by your C# logic</span></div>
          </div>
        </div>

        <div className="calculator-card">
          <div className="card-heading">
            <div>
              <p className="eyebrow">CALCULATOR</p>
              <h2>Quick math</h2>
            </div>
            <button className="clear-button" onClick={clear} type="button">AC</button>
          </div>

          <div className="display-panel" aria-live="polite">
            <div className="expression">
              {storedNumber !== null && operation
                ? `${formatNumber(storedNumber)} ${symbols[operation]}`
                : "Ready when you are"}
            </div>
            <div className="display-value">{display}</div>
          </div>

          <div className="keypad">
            <button className="key utility" onClick={() => chooseOperation("sqrt")} type="button" disabled={loading}>√x</button>
            <button className="key utility" onClick={() => chooseOperation("power")} type="button" disabled={loading}>xʸ</button>
            <button className="key utility" onClick={backspace} type="button" aria-label="Backspace">⌫</button>
            <button className="key operator" onClick={() => chooseOperation("divide")} type="button" disabled={loading}>÷</button>

            {[7, 8, 9].map((digit) => <button className="key" key={digit} onClick={() => enterDigit(String(digit))} type="button">{digit}</button>)}
            <button className="key operator" onClick={() => chooseOperation("multiply")} type="button" disabled={loading}>×</button>

            {[4, 5, 6].map((digit) => <button className="key" key={digit} onClick={() => enterDigit(String(digit))} type="button">{digit}</button>)}
            <button className="key operator" onClick={() => chooseOperation("subtract")} type="button" disabled={loading}>−</button>

            {[1, 2, 3].map((digit) => <button className="key" key={digit} onClick={() => enterDigit(String(digit))} type="button">{digit}</button>)}
            <button className="key operator" onClick={() => chooseOperation("add")} type="button" disabled={loading}>+</button>

            <button className="key utility" onClick={() => chooseOperation("modulus")} type="button" disabled={loading}>%</button>
            <button className="key" onClick={() => enterDigit("0")} type="button">0</button>
            <button className="key" onClick={enterDecimal} type="button">.</button>
            <button className="key equals" onClick={getResult} type="button" disabled={loading}>{loading ? "…" : "="}</button>
          </div>
          <p className={`error-message ${error ? "visible" : ""}`} role="alert">{error || " "}</p>
          <p className="card-footnote">Calculations run on your C# API</p>
        </div>

        <aside className="history-card">
          <div className="history-heading">
            <div><p className="eyebrow">RECENT</p><h2>History</h2></div>
            <span className="history-count">{history.length}</span>
          </div>
          {history.length === 0 ? (
            <div className="empty-history">
              <span className="empty-icon">↳</span>
              <p>Your calculations will<br />show up here.</p>
            </div>
          ) : (
            <ul className="history-list">
              {history.map((item, index) => (
                <li key={`${item.expression}-${index}`}>
                  <span>{item.expression}</span>
                  <strong>{formatNumber(item.result)}</strong>
                </li>
              ))}
            </ul>
          )}
          <div className="history-note"><span className="status-dot" /> Connected to local API</div>
        </aside>
      </section>

      <footer className="footer">
        <span>Clearline Calculator</span>
        <span>React interface <span className="footer-divider">·</span> C# calculations</span>
      </footer>
    </main>
  );
}
