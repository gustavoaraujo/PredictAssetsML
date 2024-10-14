using Microsoft.ML.Data;
using Microsoft.ML.Trainers.FastTree;
using Microsoft.ML;
using Microsoft.ML.Transforms;
using static Microsoft.ML.DataOperationsCatalog;

namespace PredictAssetsML
{
    internal class Tester
    {
        public void Test()
        {
            var assetReturnsData = Comparer.LoadAssetReturnsData("Data.csv");
            var mlContext = new MLContext();

            var processedData = Comparer.ProcessAssetReturns(assetReturnsData);

            IDataView data = mlContext.Data.LoadFromEnumerable(processedData);

            var trainTestData = mlContext.Data.TrainTestSplit(data, 0.2);

            Func<EstimatorChain<NormalizingTransformer>, EstimatorChain<RegressionPredictionTransformer<FastTreeRegressionModelParameters>>> generateFastTree =
               _ => _.Append(mlContext.Regression.Trainers.FastTree(labelColumnName: nameof(AssetReturnData.Yield), numberOfLeaves: 100, numberOfTrees: 1000));


            Func<EstimatorChain<NormalizingTransformer>, EstimatorChain<RegressionPredictionTransformer<GamRegressionModelParameters>>> generateGam =
                _ => _.Append(mlContext.Regression.Trainers.Gam(labelColumnName: nameof(AssetReturnData.Yield)));

            GenerateTest(mlContext, trainTestData, generateGam);
        }

        private void GenerateTest<T>(MLContext mlContext, TrainTestData trainTestData, Func<EstimatorChain<NormalizingTransformer>, EstimatorChain<RegressionPredictionTransformer<T>>> createPipeline) where T : class
        {
            var estimator = Comparer.CreateEstimator(mlContext);
            
            var pipeline = createPipeline.Invoke(estimator);

            var model = pipeline.Fit(trainTestData.TrainSet);

            var predictions = model.Transform(trainTestData.TestSet);

            var predictedValues = mlContext.Data.CreateEnumerable<PredictionOutput>(predictions, reuseRowObject: false).ToList();

            foreach (var prediction in predictedValues)
            {
                Console.WriteLine(prediction.Score);
            }
        }
    }
}
