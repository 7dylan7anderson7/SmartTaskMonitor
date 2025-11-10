from flask import Flask, request, jsonify
import numpy as np
import pandas as pd
from sklearn.linear_model import LogisticRegression
import pickle

app = Flask(__name__)

#Generate fake task data
np.random.seed(42) #Seeds rng to produce the same results for consistency
N = 200 #Number of rows of data to simulate
data = pd.DataFrame({
    "DurationSeconds": np.random.uniform(5, 120, N),
    "ErrorCount": np.random.randint(0, 10, N),
    "LastRunDaysAgo": np.random.randint(0, 15, N)
})

# Defines WillFail with a rule-based pattern with randomness
data["WillFail"] = ((data["ErrorCount"] > 3) | (data["DurationSeconds"] > 80)).astype(int)
data.loc[np.random.rand(N) < 0.1, "WillFail"] = 1  # add randomness


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