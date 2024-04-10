# -*- coding: utf-8 -*-
"""Fraud Classification Pipeline.ipynb

Automatically generated by Colaboratory.

Original file is located at
    https://colab.research.google.com/drive/1zEmlPhBFdvTzZ3AG0ze3lQ5t-ZEWPRE3
"""

# Step 1: Data problem
# Predict fraudulent orders and flag them on the admin’s order summary view

def import_data(path, messages=True):
  import pandas as pd

  df = pd.read_csv(path)

  if messages: print(df.shape)

  return df

# This is the customers table!!
import pandas as pd
pd.set_option('display.max_columns', None)
df = import_data('/content/drive/MyDrive/IS 455/Intex Files/INTEX W24 Dataset.xlsx - Orders.csv')
df.head()

# Data understanding

def basic_wrangling(df, features=[], missing_threshold=0.95, unique_threshold=0.95, messages=True):
  import pandas as pd

  if len(features) == 0: features = df.columns

  for feat in features:
    if feat in df.columns:
      missing = df[feat].isna().sum()
      unique = df[feat].nunique()
      rows = df.shape[0]

      if missing / rows >= missing_threshold:
        if messages: print(f"Too much missing ({missing} out of {rows}, {round(missing/rows, 0)}) for {feat}")
        df.drop(columns=[feat], inplace=True)
      elif unique / rows >= unique_threshold:
        if df[feat].dtype in ['int64', 'object']:
          if messages: print(f"Too many unique values ({unique} out of {rows}, {round(unique/rows, 0)}) for {feat}")
          df.drop(columns=[feat], inplace=True)
      elif unique == 1:
        if messages: print(f"Only one value ({df[feat].unique()[0]}) for {feat}")
        df.drop(columns=[feat], inplace=True)
    else:
      if messages: print(f"The feature \"{feat}\" doesn't exist as spelled in the DataFrame provided")

  return df
  import pandas as pd

  for col in df:
    missing = df[col].isna().sum()
    unique = df[col].nunique()
    rows = df.shape[0]

    if missing / rows >= missing_threshold:
      df.drop(columns=[col], inplace=True)
      if messages: print(f'Too many missing values ({round(missing/rows, 2)*100}%) in column {col}')
    if unique / rows >= unique_threshold:
      if df[col].dtype in ['object', 'int64']:
        df.drop(columns=[col], inplace=True)
        if messages: print(f'Too many unique values ({round(unique/rows, 2)*100}%) in column {col}')
    if unique == 1:
      df.drop(columns=[col], inplace=True)
      if messages: print(f'Only one value ({df[col].unique()[0]}) in column {col}')

  return df

def univariate(df, sample=500):
  import seaborn as sns
  import matplotlib.pyplot as plt
  import math

  df_results = pd.DataFrame(columns=['bin_groups', 'type', 'missing', 'unique', 'min',
                                      'median', 'max', 'mode', 'mean', 'std', 'skew'])

  for col in df:
    # Features that apply to all dtypes
    dtype = df[col].dtype
    missing = df[col].isna().sum()
    unique = df[col].nunique()
    mode = df[col].mode()[0]
    if pd.api.types.is_numeric_dtype(df[col]):
      # Features for numeric dtypes only
      min = df[col].min()
      max = df[col].max()
      mean = df[col].mean()
      median = df[col].median()
      std = df[col].std()
      skew = df[col].skew()
      df_results.loc[col] = ['-', dtype, missing, unique, min, median, max, mode,
                            round(mean, 2), round(std, 2), round(skew, 2)]
    else:
      # Features for object dtypes only
      flag = df[col].value_counts()[(df[col].value_counts() / df.shape[0]) < 0.05].shape[0]
      df_results.loc[col] = [flag, dtype, missing, unique, '-', '-', '-', mode, '-', '-', '-']

  # Make a sub-DataFrame of features that are objects or have only two values; they will need countplots
  countplots = df_results[(df_results['type']=='object') | (df_results['unique']==2)]
  # Make a sub-DataFrame of features that are floats or ints with many values which will need histograms
  histograms = df_results[(df_results['type']=='float64') | ((df_results['unique']>10) & (df_results['type']=='int64'))]
  histograms = histograms[histograms['unique']>2] # Remove those that are binary

  # Create a set of countplots for the categorical features
  f, ax = plt.subplots(1, countplots.shape[0], figsize=[countplots.shape[0] * 1.5, 1.5])
  for i, col in enumerate(countplots.index):
    g = sns.countplot(data=df, x=col, color='g', ax=ax[i]);
    g.set_yticklabels('')
    g.set_ylabel('')
    ax[i].tick_params(labelrotation=90, left=False)
    ax[i].xaxis.set_label_position('top')
    sns.despine(left=True, top=True, right=True)

  plt.subplots_adjust(hspace=2, wspace=.5)
  plt.show()

  # Create a set of histograms for the numeric features
  f, ax = plt.subplots(1, histograms.shape[0], figsize=[histograms.shape[0] * 1.5, 1.5])
  for i, col in enumerate(histograms.index):
    g = sns.histplot(data=df.sample(n=sample, random_state=1), x=col, color='b', ax=ax[i], kde=True);
    g.set_yticklabels(labels=[])
    g.set_ylabel('')
    ax[i].tick_params(left=False)
    sns.despine(left=True, top=True, right=True)

  plt.subplots_adjust(hspace=2, wspace=.5)
  plt.show()

  return df_results

import sys
df = basic_wrangling(df)
univariate(df)

def bin_categories(df, features=[], cutoff=0.05, replace_with='Other', messages=True):
  import pandas as pd

  for feat in features:
    if feat in df.columns:
      if not pd.api.types.is_numeric_dtype(df[feat]):
        other_list = df[feat].value_counts()[df[feat].value_counts() / df.shape[0] < cutoff].index
        df.loc[df[feat].isin(other_list), feat] = replace_with
    else:
      if messages: print(f'{feat} not found in the DataFrame provided. No binning performed')

  return df

def missing_drop(df, label="", features=[], messages=True, row_threshold=2):
  import pandas as pd

  start_count = df.count().sum()

  # Drop the obvious columns and rows
  df.dropna(axis=0, how='all', inplace=True)            # Drop rows where every value is missing
  df.dropna(axis=0, thresh=row_threshold, inplace=True) # Drop rows that are missing more than the threshold allowed
  df.dropna(axis=1, how='all', inplace=True)            # Drop columns where every value is missing
  if label != "": df.dropna(axis=0, how='all', subset=[label], inplace=True)  # Drop rows where the label is missing

  # Get the number of missing values for each feature and calculate the remaining non-nan cells if the column vs rows were dropped
  def generate_missing_table():
    df_results = pd.DataFrame(columns=['Missing', 'column', 'rows'])
    for feat in df:
      missing = df[feat].isna().sum()
      if missing > 0: # Only do the work if there is missing data
        # Compare the non-nan cells remaining after dropping either the column or entire rows that are missing that column
        memory_cols = df.drop(columns=[feat]).count().sum()
        memory_rows = df.dropna(subset=[feat]).count().sum()
        df_results.loc[feat] = [missing, memory_cols, memory_rows]
    return df_results

  df_results = generate_missing_table()
  while df_results.shape[0] > 0: # If any missing data were found
    max = df_results[['column', 'rows']].max(axis=1)[0]
    max_axis = df_results.columns[df_results.isin([max]).any()][0]
    df_results.sort_values(by=[max_axis], ascending=False, inplace=True)
    if messages: print('\n', df_results)
    if max_axis == 'rows':
      df.dropna(axis=0, subset=[df_results.index[0]], inplace=True)
    else:
      df.drop(columns=[df_results.index[0]], inplace=True)
    df_results = generate_missing_table()

    if messages: print(f'{round(df.count().sum()/start_count * 100, 2)}% ({df.count().sum()} / {start_count}) of non-null cells were kept.')
  return df

df = bin_categories(df)
df = missing_drop(df, label='fraud')
df

# drop customer_id column too
df.drop(columns=['customer_ID'])
df

# Change the date column to days of 2023 to help with dummy coding
df['date'] = pd.to_datetime(df['date'], format='%m/%d/%Y')
reference_date = pd.Timestamp('2023-01-01')
df['date'] = (df['date'] - reference_date).dt.days + 1

df

# Save file
df.to_csv('/content/orders.csv', index=False)

from sklearn.tree import DecisionTreeClassifier
from sklearn.model_selection import train_test_split
from sklearn import metrics

y = df.fraud
X = df.drop(columns=['fraud'])
# Convert columns to 1s and 0s
X = pd.get_dummies(X, dtype=int, drop_first=True)
X.head()

# rename United Kingdom column because there is a space
X.rename(columns={'country_of_transaction_United Kingdom': 'country_of_transaction_United_Kingdom'}, inplace=True)
X.rename(columns={'shipping_address_United Kingdom': 'shipping_address_United_Kingdom'}, inplace=True)
X.head()

# Splitting it 50% because it's a big dataset
X_train, X_test, y_train, y_test = train_test_split(X, y, test_size=0.5, random_state=1)

# # What algorithm is best?

# def split_data(X, y, test_size=0.5, random_state=1):
#   import pandas as pd
#   from sklearn.model_selection import train_test_split

#   return train_test_split(X, y, test_size=test_size, random_state=random_state)

# def fit_cv_regression(df, k, label, repeat=True, algorithm='ensemble', random_state=1, messages=True):
#   from sklearn.model_selection import KFold, RepeatedKFold, cross_val_score
#   import pandas as pd
#   from numpy import mean

#   # Create cross-validator object
#   if repeat:
#     cv = RepeatedKFold(n_splits=k, n_repeats=5, random_state=random_state)
#   else:
#     cv = KFold(n_splits=k, random_state=random_state, shuffle=True)

#   if algorithm == 'linear':
#     from sklearn.linear_model import Ridge, LassoLars
#     model1 = Ridge(random_state=random_state)
#     model2 = LassoLars(random_state=random_state)
#     score1 = mean(cross_val_score(model1, X, y, scoring='r2', cv=cv, n_jobs=-1))
#     score2 = mean(cross_val_score(model2, X, y, scoring='r2', cv=cv, n_jobs=-1))
#   elif algorithm == 'ensemble':
#     from sklearn.ensemble import RandomForestRegressor, GradientBoostingRegressor
#     model1 = RandomForestRegressor(random_state=random_state)
#     model2 = GradientBoostingRegressor(random_state=random_state)
#     score1 = mean(cross_val_score(model1, X, y, scoring='r2', cv=cv, n_jobs=-1))
#     score2 = mean(cross_val_score(model2, X, y, scoring='r2', cv=cv, n_jobs=-1))
#   else:
#     from sklearn.neural_network import MLPRegressor
#     from sklearn.neighbors import KNeighborsRegressor
#     model1 = MLPRegressor(random_state=random_state, max_iter=10000)
#     model2 = KNeighborsRegressor()
#     score1 = mean(cross_val_score(model1, X, y, scoring='r2', cv=cv, n_jobs=-1))
#     score2 = mean(cross_val_score(model2, X, y, scoring='r2', cv=cv, n_jobs=-1))

#   if messages:
#     print('R2', '{: <25}'.format(type(model1).__name__), round(score1, 4))
#     print('R2', '{: <25}'.format(type(model2).__name__), round(score2, 4))

#   if score1 > score2:
#     return model1.fit(X, y)
#   else:
#     return model2.fit(X, y)

# df_reduced = df.sample(n=1000, random_state=1)

# model = fit_cv_regression(df_reduced, 5, 'fraud', algorithm='linear')
# model = fit_cv_regression(df_reduced, 5, 'fraud', algorithm='other')
# model = fit_cv_regression(df_reduced, 5, 'fraud', algorithm='ensemble')

clf = DecisionTreeClassifier()

clf = clf.fit(X_train,y_train)

y_pred = clf.predict(X_test)

output_df = pd.DataFrame({'Actual': y_test, 'Predicted': y_pred})
output_df.head(10)

from sklearn import metrics
from matplotlib import pyplot as plt

from matplotlib import pyplot as plt
cm = metrics.confusion_matrix(y_test, y_pred)
cm_display = metrics.ConfusionMatrixDisplay(cm, display_labels=['not fraud', 'fraud'])
cm_display.plot(values_format='d')
plt.show()

y_test_dummies = pd.get_dummies(y_test, drop_first=True)
y_pred_dummies = pd.get_dummies(y_pred, drop_first=True)

# Accuracy  = (true positives + true negatives) / (total cases); ranges from 0 (worst) to 1 (best)
print(f"Accuracy:\t{metrics.accuracy_score(y_test, y_pred)}")

# Precision = (true positives / (true positives + false positives))
print(f"Precision:\t{metrics.precision_score(y_test_dummies, y_pred_dummies, labels=['no', 'yes'])}")

# Recall    = (true positives / (true positives + false negatives))
print(f"Recall:\t\t{metrics.recall_score(y_test_dummies, y_pred_dummies, labels=['no', 'yes'])}")

# F1        = (2 * (precision * recall) / (precision + recall))
print(f"F1:\t\t{metrics.f1_score(y_test_dummies, y_pred_dummies, labels=['no', 'yes'])}")

# This is the highest r2 score we found
r_squared = clf.score(X, y)
print(r_squared)

# Test prediction but reshape to 2D array to not get the error
input_data = [589, 257, 16, 200.0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 1]
reshaped_input_data = [input_data]  # Reshape to 2D array

# Predict using the reshaped input data
prediction = clf.predict(reshaped_input_data)

prediction

# # Convert to onnx format
# # install onnx and skl2onnx but then comment it out
# #!pip install onnx
# #!pip install skl2onnx

import onnx
from skl2onnx import convert_sklearn
from skl2onnx.common.data_types import FloatTensorType

initial_type = [('float_input', FloatTensorType([None, X_train.shape[1]]))]
onnx_model = convert_sklearn(clf, initial_types=initial_type)

# Save model
with open("decision_tree_model.onnx", "wb") as f:
  f.write(onnx_model.SerializeToString())

"""# Clean customers table too"""

import pandas as pd
df_customers = pd.read_csv('/content/drive/MyDrive/IS 455/Intex Files/INTEX W24 Dataset.xlsx - Customers.csv')
df_customers

# Convert age to int instead of float
df_customers['age'] = df_customers['age'].astype(int)
df_customers

null_values = df_customers.isnull()

# Print the DataFrame showing where null values are
print(null_values)

# Additionally, to get a summary of null values for each column
null_summary = df_customers.isnull().sum()
print("\nSummary of null values per column:")
print(null_summary)

# We checked null values, but we're not too worried about gender being null
rows_with_null_gender = df_customers[df_customers['gender'].isnull()]
rows_with_null_gender

# Save file
df_customers.to_csv('/content/customers.csv', index=False)