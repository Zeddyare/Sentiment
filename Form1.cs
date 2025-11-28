using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.ML;
using Microsoft.ML.Data;
using SentimentAnalysisApp;
using System.Windows.Forms;

namespace Sentiment
{
    public partial class Form1 : Form
    {
        private readonly string _dataPath;
        private MLContext? _mlContext;
        private ITransformer? _model;
        private PredictionEngine<SentimentData, SentimentPrediction>? _predictionEngine;

        public Form1()
        {
            InitializeComponent();
            _dataPath = Path.Combine(AppContext.BaseDirectory, "Data", "yelp_labelled.txt");
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            var text = this.txtInput.Text ?? string.Empty;
            if (string.IsNullOrWhiteSpace(text))
            {
                WriteLine("Please enter text to submit.");
                return;
            }

            bool label = this.chkLabelPositive.Checked;

            try
            {
                // Sanitize text to avoid breaking the TSV format
                var safeText = text.Replace('\t', ' ').Replace('\r', ' ').Replace('\n', ' ').Trim();

                // Ensure directory exists
                var dir = Path.GetDirectoryName(_dataPath);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                // Append to data file in the same format: text <tab> label
                using (var sw = File.AppendText(_dataPath))
                {
                    sw.WriteLine($"{safeText}\t{(label ? 1 : 0)}");
                }

                WriteLine($"Submitted: '{safeText}' with label {(label ? 1 : 0)}");

                // Retrain and evaluate on updated data
                RetrainAndEvaluate();
            }
            catch (Exception ex)
            {
                WriteLine($"Submit failed: {ex.Message}");
            }
        }

        private void RetrainAndEvaluate()
        {
            try
            {
                // Reset model and prediction engine
                _mlContext = new MLContext();

                var split = LoadData(_mlContext);
                _model = BuildAndTrainModel(_mlContext, split.TrainSet);

                Evaluate(_mlContext, _model, split.TestSet);

                // recreate prediction engine
                _predictionEngine = _mlContext.Model.CreatePredictionEngine<SentimentData, SentimentPrediction>(_model);
                WriteLine("Retrain and evaluation completed.");
            }
            catch (Exception ex)
            {
                WriteLine($"Retrain failed: {ex.Message}");
            }
        }

        private void EnsureModel()
        {
            if (_model != null && _mlContext != null && _predictionEngine != null)
                return;

            _mlContext = new MLContext();
            var split = LoadData(_mlContext);
            _model = BuildAndTrainModel(_mlContext, split.TrainSet);
            _predictionEngine = _mlContext.Model.CreatePredictionEngine<SentimentData, SentimentPrediction>(_model);
        }

        private void btnPredict_Click(object sender, EventArgs e)
        {
            var text = this.txtInput.Text ?? string.Empty;
            if (string.IsNullOrWhiteSpace(text))
            {
                WriteLine("Please enter text to predict.");
                return;
            }

            try
            {
                EnsureModel();
                if (_mlContext == null || _model == null || _predictionEngine == null)
                {
                    WriteLine("Model is not ready.");
                    return;
                }

                var sample = new SentimentData { SentimentText = text };
                var prediction = _predictionEngine.Predict(sample);
                WriteLine($"Input: {text}");
                WriteLine($"Prediction: {(Convert.ToBoolean(prediction.Prediction) ? "Positive" : "Negative")} | Positive Probability: {prediction.Probability:P2}");
            }
            catch (Exception ex)
            {
                WriteLine($"Prediction failed: {ex.Message}");
            }
        }

        private void WriteLine(string text = "")
        {
            if (this.txtOutput.InvokeRequired)
            {
                this.txtOutput.Invoke(new Action(() => {
                    this.txtOutput.AppendText(text + Environment.NewLine);
                }));
            }
            else
            {
                this.txtOutput.AppendText(text + Environment.NewLine);
            }
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            try
            {
                this.txtOutput.Clear();

                if (!File.Exists(_dataPath))
                {
                    WriteLine($"Data file not found: {_dataPath}");
                    return;
                }

                MLContext mlContext = new MLContext();

                var splitDataView = LoadData(mlContext);

                var model = BuildAndTrainModel(mlContext, splitDataView.TrainSet);

                Evaluate(mlContext, model, splitDataView.TestSet);
                UseModelWithSingleItem(mlContext, model);
                UseModelWithBatchItems(mlContext, model);
            }
            catch (Exception ex)
            {
                WriteLine($"Run failed: {ex.Message}");
            }
        }

        private Microsoft.ML.DataOperationsCatalog.TrainTestData LoadData(MLContext mlContext)
        {
            // Try to read lines and parse manually to avoid issues with malformed or unescaped text
            var samples = new List<SentimentData>();
            try
            {
                foreach (var line in File.ReadLines(_dataPath))
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    // Expect last tab-separated token to be label
                    var idx = line.LastIndexOf('\t');
                    if (idx <= 0 || idx >= line.Length - 1)
                    {
                        // skip malformed line
                        continue;
                    }

                    var text = line.Substring(0, idx).Trim();
                    var labelToken = line.Substring(idx + 1).Trim();
                    if (int.TryParse(labelToken, out int labelInt))
                    {
                        samples.Add(new SentimentData { SentimentText = text, Label = labelInt == 1 });
                    }
                }

                if (samples.Count > 0)
                {
                    IDataView dataView = mlContext.Data.LoadFromEnumerable(samples);
                    Microsoft.ML.DataOperationsCatalog.TrainTestData splitDataView = mlContext.Data.TrainTestSplit(dataView, testFraction: 0.2);
                    return splitDataView;
                }
            }
            catch (Exception ex)
            {
                WriteLine($"Warning: manual data parsing failed: {ex.Message}");
            }

            // Fallback to ML.NET loader which may throw if file malformed
            IDataView fallback = mlContext.Data.LoadFromTextFile<SentimentAnalysisApp.SentimentData>(_dataPath, hasHeader: false);
            Microsoft.ML.DataOperationsCatalog.TrainTestData fallbackSplit = mlContext.Data.TrainTestSplit(fallback, testFraction: 0.2);
            return fallbackSplit;
        }

        private ITransformer BuildAndTrainModel(MLContext mlContext, IDataView splitTrainSet)
        {
            var estimator = mlContext.Transforms.Text.FeaturizeText(outputColumnName: "Features", inputColumnName: nameof(SentimentAnalysisApp.SentimentData.SentimentText))
                .Append(mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(labelColumnName: "Label", featureColumnName: "Features"));

            WriteLine("=============== Create and Train the Model ===============");
            var model = estimator.Fit(splitTrainSet);
            WriteLine("=============== End of training ===============");
            WriteLine("");

            return model;
        }

        private void Evaluate(MLContext mlContext, ITransformer model, IDataView splitTestSet)
        {
            WriteLine("=============== Evaluating Model accuracy with Test data===============");
            IDataView predictions = model.Transform(splitTestSet);
            var metrics = mlContext.BinaryClassification.Evaluate(predictions, "Label");
            WriteLine("");
            WriteLine("Model quality metrics evaluation");
            WriteLine("--------------------------------");
            WriteLine($"Accuracy: {metrics.Accuracy:P2}");
            WriteLine($"Auc: {metrics.AreaUnderRocCurve:P2}");
            WriteLine($"F1Score: {metrics.F1Score:P2}");
            WriteLine("=============== End of model evaluation ===============");
        }

        private void UseModelWithSingleItem(MLContext mlContext, ITransformer model)
        {
            var predictionFunction = mlContext.Model.CreatePredictionEngine<SentimentAnalysisApp.SentimentData, SentimentAnalysisApp.SentimentPrediction>(model);

            var sampleStatement = new SentimentAnalysisApp.SentimentData
            {
                SentimentText = "This was a very bad steak"
            };
            var resultPrediction = predictionFunction.Predict(sampleStatement);

            WriteLine("");
            WriteLine("=============== Prediction Test of model with a single sample ===============");
            WriteLine($"Sentiment: {resultPrediction.SentimentText} | Prediction: {(Convert.ToBoolean(resultPrediction.Prediction) ? "Positive" : "Negative")} | Probability: {resultPrediction.Probability} ");
            WriteLine("=============== End of Predictions ===============");
            WriteLine("");
        }

        private void UseModelWithBatchItems(MLContext mlContext, ITransformer model)
        {
            var sentiments = new[]
            {
                new SentimentAnalysisApp.SentimentData { SentimentText = "This was a horrible meal" },
                new SentimentAnalysisApp.SentimentData { SentimentText = "I love this spaghetti." }
            };

            IDataView batchComments = mlContext.Data.LoadFromEnumerable(sentiments);
            IDataView predictions = model.Transform(batchComments);
            var predictedResults = mlContext.Data.CreateEnumerable<SentimentAnalysisApp.SentimentPrediction>(predictions, reuseRowObject: false);

            WriteLine("");
            WriteLine("=============== Prediction Test of loaded model with multiple samples ===============");
            foreach (var prediction in predictedResults)
            {
                WriteLine($"Sentiment: {prediction.SentimentText} | Prediction: {(Convert.ToBoolean(prediction.Prediction) ? "Positive" : "Negative")} | Probability: {prediction.Probability} ");
            }
            WriteLine("=============== End of predictions ===============");
        }
    }
}
