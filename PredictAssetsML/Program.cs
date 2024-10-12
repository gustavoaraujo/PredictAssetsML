using Microsoft.ML;
using Microsoft.ML.Data;
using System.Globalization;

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
            // 1. Inicializa o contexto do ML
            var mlContext = new MLContext();

            // 2. Importa os dados de rentabilidade de ativos
            var assetReturnsData = LoadAssetReturnsData("Data.csv");

            // 3. Processa os dados para calcular rentabilidades acumuladas
            var processedData = ProcessAssetReturns(assetReturnsData);

            IDataView data = mlContext.Data.LoadFromEnumerable(processedData);

            var trainTestData = mlContext.Data.TrainTestSplit(data, 0.2);

            // 4. Treina o modelo
            var model = TrainModel(mlContext, trainTestData.TrainSet);

            var predictions = model.Transform(trainTestData.TestSet);
            var metrics = mlContext.Regression.Evaluate(predictions, labelColumnName: nameof(AssetReturnData.Price));


            var predictedValues = mlContext.Data.CreateEnumerable<PredictionOutput>(predictions, reuseRowObject: false).ToList();

            // 6. Exibe as previsões
            foreach (var prediction in predictedValues)
            {
                Console.WriteLine(prediction.Score);
            }
        }

        // Método para carregar dados de um arquivo CSV
        private static List<AssetReturnData> LoadAssetReturnsData(string filePath)
        {
            var lines = File.ReadAllLines(filePath);
            var data = new List<AssetReturnData>();

            // Lógica para carregar e converter os dados do CSV
            foreach (var line in lines.Skip(1)) // Ignora o cabeçalho
            {
                var values = line.Split(',');
                var assetReturnData = new AssetReturnData
                {
                    Date = DateTime.Parse(values[0]),
                    Price = float.Parse(values[1], CultureInfo.InvariantCulture),
                    Yield = float.Parse(values[2], CultureInfo.InvariantCulture),
                };
                data.Add(assetReturnData);
            }

            return data;
        }

        // Método para processar as rentabilidades e calcular acumulados
        private static List<AssetReturnData> ProcessAssetReturns(List<AssetReturnData> assetReturnsData)
        {
            // Aqui você irá calcular as rentabilidades acumuladas para os períodos de 6, 12 e 18 meses
            // Similar ao que discutimos anteriormente
            // Retorna a lista processada
            var processedData = new List<AssetReturnData>();

            for (var i = 10; i < assetReturnsData.Count; i++)
            {
                if (assetReturnsData[i].Price == 0 || assetReturnsData[i].Yield == 0) continue;

                if (i > 10)
                {
                    assetReturnsData[i].TD10 = assetReturnsData[i].Price/ assetReturnsData[i-10].Price;
                    assetReturnsData[i].TD10Y = assetReturnsData[i].Yield/ assetReturnsData[i-10].Yield;
                }

                if (i > 30)
                {
                    assetReturnsData[i].TD30 = assetReturnsData[i].Price / assetReturnsData[i - 30].Price;
                    assetReturnsData[i].TD30Y = assetReturnsData[i].Yield / assetReturnsData[i - 30].Yield;
                }

                if (i > 90)
                {
                    assetReturnsData[i].TD90 = assetReturnsData[i].Price / assetReturnsData[i - 90].Price;
                    assetReturnsData[i].TD90Y = assetReturnsData[i].Yield / assetReturnsData[i - 90].Yield;
                }

                processedData.Add(assetReturnsData[i]);
            }

            return processedData;
        }

        // Método para treinar o modelo
        private static ITransformer TrainModel(MLContext mlContext, IDataView trainData)
        {
            var pipeline = mlContext.Transforms.Concatenate("Features",
                nameof(AssetReturnData.TD30),
                nameof(AssetReturnData.TD90),
                nameof(AssetReturnData.TD10),
                nameof(AssetReturnData.TD30Y),
                nameof(AssetReturnData.TD90Y),
                nameof(AssetReturnData.TD10Y),
                nameof(AssetReturnData.Price),
                nameof(AssetReturnData.Yield)
                )
                .Append(mlContext.Transforms.NormalizeMinMax("Features"))
                //.Append(mlContext.Regression.Trainers.Sdca(labelColumnName: nameof(AssetReturnData.IBOV), maximumNumberOfIterations: 100));
                .Append(mlContext.Regression.Trainers.FastForest(labelColumnName: nameof(AssetReturnData.Price),numberOfLeaves:100,numberOfTrees:1000));

            return pipeline.Fit(trainData);
        }

        public class PredictionResult
        {
            [ColumnName("Score")]
            public float PredictedReturn { get; set; } // A previsão de rentabilidade do IBOV
        }

        // Método para prever as rentabilidades futuras
        private static float PredictFutureReturn(MLContext mlContext, ITransformer model, AssetReturnData currentData)
        {
            var predictionEngine = mlContext.Model.CreatePredictionEngine<AssetReturnData, PredictionResult>(model);
            var prediction = predictionEngine.Predict(currentData);
            return prediction.PredictedReturn;
        }

        // Adicione aqui a função UpdateAssetReturnDataWithPrediction
        private static AssetReturnData UpdateAssetReturnDataWithPrediction(AssetReturnData previousData, float predictedReturn)
        {
            // Implementação para atualizar as rentabilidades
            return previousData; // Retorna os dados atualizados
        }
    }

    public class PredictionResult
    {
        [ColumnName("Score")]
        public float PredictedReturn { get; set; } // A previsão de rentabilidade do IBOV
    }

}
