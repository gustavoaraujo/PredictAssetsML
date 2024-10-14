using Microsoft.ML;
using Microsoft.ML.Data;

namespace PredictAssetsML
{
    public class PredictionOutput
    {
        public float Score { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            //new Comparer().Compare();

            new Tester().Test();

            // 4. Treina o modelo
            //var model = TrainModel(mlContext, trainTestData.TrainSet);

            //Func<EstimatorChain<Microsoft.ML.Transforms.NormalizingTransformer>, EstimatorChain<RegressionPredictionTransformer<FastForestRegressionModelParameters>>> generateFastForest = 
            //    _ => _.Append(mlContext.Regression.Trainers.FastForest(labelColumnName: nameof(AssetReturnData.Yield), numberOfLeaves: 100, numberOfTrees: 1000));

            //GetMetrics(mlContext, trainTestData, generateFastForest);

            //Func<EstimatorChain<Microsoft.ML.Transforms.NormalizingTransformer>, EstimatorChain<RegressionPredictionTransformer<LinearRegressionModelParameters>>> generateSdca =
            //    _ => _.Append(mlContext.Regression.Trainers.Sdca(labelColumnName: nameof(AssetReturnData.Yield)));

            //GetMetrics(mlContext, trainTestData, generateSdca);

            //Func<EstimatorChain<Microsoft.ML.Transforms.NormalizingTransformer>, EstimatorChain<RegressionPredictionTransformer<PoissonRegressionModelParameters>>> generateLasso =
            //    _ => _.Append(mlContext.Regression.Trainers.LbfgsPoissonRegression(labelColumnName: nameof(AssetReturnData.Yield)));

            //GetMetrics(mlContext, trainTestData, generateLasso);

            //var predictedValues = mlContext.Data.CreateEnumerable<PredictionOutput>(predictions, reuseRowObject: false).ToList();

            //// 6. Exibe as previsões
            //foreach (var prediction in predictedValues)
            //{
            //    Console.WriteLine(prediction.Score);
            //}

            //var assetReturnsData = LoadAssetReturnsData("Data.csv");

            //var mlContext = new MLContext();

            //// 2. Importa os dados de rentabilidade de ativos

            //// 3. Processa os dados para calcular rentabilidades acumuladas
            //var processedData = ProcessAssetReturns(assetReturnsData);

            //IDataView data = mlContext.Data.LoadFromEnumerable(processedData);

            //var trainTestData = mlContext.Data.TrainTestSplit(data, 0.2);
            //Func<EstimatorChain<Microsoft.ML.Transforms.NormalizingTransformer>, EstimatorChain<RegressionPredictionTransformer<FastTreeRegressionModelParameters>>> generateFastTree =
            //_ => _.Append(mlContext.Regression.Trainers.FastTree(labelColumnName: nameof(AssetReturnData.Yield), numberOfLeaves: 100, numberOfTrees: 1000));

            //Func<EstimatorChain<Microsoft.ML.Transforms.NormalizingTransformer>, EstimatorChain<RegressionPredictionTransformer<LinearRegressionModelParameters>>> generateSdca =
            //    _ => _.Append(mlContext.Regression.Trainers.Sdca(labelColumnName: nameof(AssetReturnData.Yield)));

            //for (var i = 0; i < 30; i++) 
            //{
            //    var recentData = processedData.Skip(Math.Max(0, processedData.Count() - 30)).Take(30).ToList();

            //    var lastAssetReturnData = new AssetReturnData 
            //    {
            //        Yield = recentData.Average(_ => _.Yield),
            //        TD10Y = recentData.Average(_ => _.TD10Y),
            //        TD30Y = recentData.Average(_ => _.TD30Y),
            //        TD90Y = recentData.Average(_ => _.TD90Y),
            //    };

            //    //lastAssetReturnData.Yield = 0;
            //    var newYield = GetPredictedValue(mlContext, trainTestData, generateFastTree, lastAssetReturnData);
            //    Console.WriteLine(newYield);

            //    processedData.Add(new AssetReturnData { Date = lastAssetReturnData.Date.AddDays(1), Yield = newYield });
            //    processedData = ProcessAssetReturns(processedData);
            //}
        }

        private static float GetPredictedValue<T>(MLContext mlContext, DataOperationsCatalog.TrainTestData trainTestData, 
            Func<EstimatorChain<Microsoft.ML.Transforms.NormalizingTransformer>, EstimatorChain<RegressionPredictionTransformer<T>>> createPipeline, AssetReturnData lastAssetReturnData) where T : class
        {
            var estimator = Comparer.CreateEstimator(mlContext);
                //.Append(mlContext.Regression.Trainers.Sdca(labelColumnName: nameof(AssetReturnData.IBOV), maximumNumberOfIterations: 100));
                
            //var pipeline = estimator.Append(mlContext.Regression.Trainers.FastForest(labelColumnName: nameof(AssetReturnData.Yield), numberOfLeaves: 100, numberOfTrees: 1000));

            var pipeline = createPipeline.Invoke(estimator);

            var model = pipeline.Fit(trainTestData.TrainSet);

            var predictions = model.Transform(trainTestData.TestSet);
            //var metrics = mlContext.Regression.Evaluate(predictions, labelColumnName: nameof(AssetReturnData.Yield));

            var predictedValues = mlContext.Data.CreateEnumerable<PredictionOutput>(predictions, reuseRowObject: false).ToList();
            float lastPrediction = 0;
            foreach (var prediction in predictedValues)
            {
                if (float.IsNaN(prediction.Score))
                {
                    prediction.Score = lastPrediction;
                }

                lastPrediction = prediction.Score;
                Console.WriteLine(prediction.Score);
            }

            var predictionEngine = mlContext.Model.CreatePredictionEngine<AssetReturnData, AssetReturnPrediction>(model);

            AssetReturnPrediction result = new();
            predictionEngine.Predict(lastAssetReturnData, ref result);

            return result.Score;

            //Console.WriteLine(nameof(T) + ":");
            //Console.WriteLine("RSquared: " + metrics.RSquared);
            //Console.WriteLine("MeanSquaredError: " + metrics.MeanSquaredError);
            //Console.WriteLine("RootMeanSquaredError: " + metrics.RootMeanSquaredError);
            //Console.WriteLine("MeanAbsoluteError: " + metrics.MeanAbsoluteError);
        }
    }

    public class AssetReturnPrediction
    {
        public float Score { get; set; }  // O valor previsto (Yield) será armazenado aqui.
    }
}
