using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using Microsoft.ML.Trainers.FastTree;
using Microsoft.ML.Trainers.LightGbm;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PredictAssetsML
{
    internal class Comparer
    {
        public void Compare()
        {
            var assetReturnsData = LoadAssetReturnsData("Data.csv");
            var mlContext = new MLContext();

            var processedData = ProcessAssetReturns(assetReturnsData);

            IDataView data = mlContext.Data.LoadFromEnumerable(processedData);

            var trainTestData = mlContext.Data.TrainTestSplit(data, 0.2);

            Func<EstimatorChain<Microsoft.ML.Transforms.NormalizingTransformer>, EstimatorChain<RegressionPredictionTransformer<FastForestRegressionModelParameters>>> generateFastForest =
                _ => _.Append(mlContext.Regression.Trainers.FastForest(labelColumnName: nameof(AssetReturnData.Yield), numberOfLeaves: 100, numberOfTrees: 1000));

            GetMetrics(mlContext, trainTestData, generateFastForest);

            Func<EstimatorChain<Microsoft.ML.Transforms.NormalizingTransformer>, EstimatorChain<RegressionPredictionTransformer<LinearRegressionModelParameters>>> generateSdca =
                _ => _.Append(mlContext.Regression.Trainers.Sdca(labelColumnName: nameof(AssetReturnData.Yield)));

            GetMetrics(mlContext, trainTestData, generateSdca);

            Func<EstimatorChain<Microsoft.ML.Transforms.NormalizingTransformer>, EstimatorChain<RegressionPredictionTransformer<PoissonRegressionModelParameters>>> generateLasso =
                _ => _.Append(mlContext.Regression.Trainers.LbfgsPoissonRegression(labelColumnName: nameof(AssetReturnData.Yield)));

            GetMetrics(mlContext, trainTestData, generateLasso);

            Func<EstimatorChain<Microsoft.ML.Transforms.NormalizingTransformer>, EstimatorChain<RegressionPredictionTransformer<FastTreeRegressionModelParameters>>> generateFastTree =
               _ => _.Append(mlContext.Regression.Trainers.FastTree(labelColumnName: nameof(AssetReturnData.Yield), numberOfLeaves: 100, numberOfTrees: 1000));

            GetMetrics(mlContext, trainTestData, generateFastTree);

            Func<EstimatorChain<Microsoft.ML.Transforms.NormalizingTransformer>, EstimatorChain<RegressionPredictionTransformer<LightGbmRegressionModelParameters>>> generateLightGbm =
                _ => _.Append(mlContext.Regression.Trainers.LightGbm(labelColumnName: nameof(AssetReturnData.Yield)));

            GetMetrics(mlContext, trainTestData, generateLightGbm);

            Func<EstimatorChain<Microsoft.ML.Transforms.NormalizingTransformer>, EstimatorChain<RegressionPredictionTransformer<LinearRegressionModelParameters>>> generateOnlineGradientDescent =
                _ => _.Append(mlContext.Regression.Trainers.OnlineGradientDescent(labelColumnName: nameof(AssetReturnData.Yield)));

            GetMetrics(mlContext, trainTestData, generateOnlineGradientDescent);

            Func<EstimatorChain<Microsoft.ML.Transforms.NormalizingTransformer>, EstimatorChain<RegressionPredictionTransformer<PoissonRegressionModelParameters>>> generateLbfgsPoisson =
                _ => _.Append(mlContext.Regression.Trainers.LbfgsPoissonRegression(labelColumnName: nameof(AssetReturnData.Yield)));

            GetMetrics(mlContext, trainTestData, generateLbfgsPoisson);

            Func<EstimatorChain<Microsoft.ML.Transforms.NormalizingTransformer>, EstimatorChain<RegressionPredictionTransformer<GamRegressionModelParameters>>> generateGam =
                _ => _.Append(mlContext.Regression.Trainers.Gam(labelColumnName: nameof(AssetReturnData.Yield)));

            GetMetrics(mlContext, trainTestData, generateGam);
        }

        // Método para processar as rentabilidades e calcular acumulados
        public static List<AssetReturnData> ProcessAssetReturns(List<AssetReturnData> assetReturnsData)
        {
            // Aqui você irá calcular as rentabilidades acumuladas para os períodos de 6, 12 e 18 meses
            // Similar ao que discutimos anteriormente
            // Retorna a lista processada
            var processedData = new List<AssetReturnData>();

            for (var i = 10; i < assetReturnsData.Count; i++)
            {
                if (assetReturnsData[i].Yield == 0) continue;

                if (i > 10)
                {
                    //assetReturnsData[i].TD10 = assetReturnsData[i].Price/ assetReturnsData[i-10].Price;
                    assetReturnsData[i].TD10Y = assetReturnsData[i].Yield / assetReturnsData[i - 10].Yield;
                }

                if (i > 30)
                {
                    //assetReturnsData[i].TD30 = assetReturnsData[i].Price / assetReturnsData[i - 30].Price;
                    assetReturnsData[i].TD30Y = assetReturnsData[i].Yield / assetReturnsData[i - 30].Yield;
                }

                if (i > 90)
                {
                    //assetReturnsData[i].TD90 = assetReturnsData[i].Price / assetReturnsData[i - 90].Price;
                    assetReturnsData[i].TD90Y = assetReturnsData[i].Yield / assetReturnsData[i - 90].Yield;
                }

                processedData.Add(assetReturnsData[i]);
            }

            return processedData;
        }

        public static EstimatorChain<Microsoft.ML.Transforms.NormalizingTransformer> CreateEstimator(MLContext mlContext)
        {
            return mlContext.Transforms.Concatenate("Features",
                            //nameof(AssetReturnData.TD30),
                            //nameof(AssetReturnData.TD90),
                            //nameof(AssetReturnData.TD10),
                            nameof(AssetReturnData.TD30Y),
                            nameof(AssetReturnData.TD90Y),
                            nameof(AssetReturnData.TD10Y),
                            //nameof(AssetReturnData.Price),
                            nameof(AssetReturnData.Yield)
                            )
                            .Append(mlContext.Transforms.NormalizeMinMax("Features"));
        }

        public static void GetMetrics<T>(MLContext mlContext, DataOperationsCatalog.TrainTestData trainTestData,
            Func<EstimatorChain<Microsoft.ML.Transforms.NormalizingTransformer>, EstimatorChain<RegressionPredictionTransformer<T>>> createPipeline) where T : class
        {
            var estimator = CreateEstimator(mlContext);

            var pipeline = createPipeline.Invoke(estimator);

            var model = pipeline.Fit(trainTestData.TrainSet);

            var predictions = model.Transform(trainTestData.TestSet);
            var metrics = mlContext.Regression.Evaluate(predictions, labelColumnName: nameof(AssetReturnData.Yield));

            Console.WriteLine(typeof(T).Name + ":");
            Console.WriteLine("RSquared: " + metrics.RSquared);
            Console.WriteLine("MeanSquaredError: " + metrics.MeanSquaredError);
            Console.WriteLine("RootMeanSquaredError: " + metrics.RootMeanSquaredError);
            Console.WriteLine("MeanAbsoluteError: " + metrics.MeanAbsoluteError);
        }

        public static List<AssetReturnData> LoadAssetReturnsData(string filePath)
        {
            var lines = File.ReadAllLines(filePath);
            var data = new List<AssetReturnData>();

            foreach (var line in lines.Skip(1))
            {
                var values = line.Split(',');
                var assetReturnData = new AssetReturnData
                {
                    Date = DateTime.Parse(values[0]),
                    //Price = float.Parse(values[1], CultureInfo.InvariantCulture),
                    Yield = float.Parse(values[2], CultureInfo.InvariantCulture),
                };
                data.Add(assetReturnData);
            }

            return data;
        }
    }
}
