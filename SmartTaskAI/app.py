from flask import Flask, request, jsonify
import pandas as pd
from sklearn.linear_model import LogisticRegression
import pickle

app = Flask(__name__)

# --- Train a simple model on fake data ---
data = pd.DataFrame({
    "DurationSeconds": [5, 10, 20, 50, 80, 30, 15, 100],
    "ErrorCount": [0, 1, 2, 4, 6, 3, 1, 8],
    "LastRunDaysAgo": [0, 1, 2, 5, 10, 3, 1, 12],
    "WillFail": [0, 0, 0, 1, 1, 1, 0, 1]
})

X = data[["DurationSeconds", "ErrorCount", "LastRunDaysAgo"]]
y = data["WillFail"]

model = LogisticRegression()
model.fit(X, y)
pickle.dump(model, open("model.pkl", "wb"))

@app.route("/predict", methods=["POST"])
def predict():
    payload = request.get_json()
    X_test = pd.DataFrame([payload])
    pred = model.predict_proba(X_test)[0][1]  # probability of failure
    return jsonify({"failure_probability": round(float(pred), 2)})

if __name__ == "__main__":
    app.run(port=5001)