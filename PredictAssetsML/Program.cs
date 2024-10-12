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
            var metrics = mlContext.Regression.Evaluate(predictions, labelColumnName: nameof(AssetReturnData.IBOV));


            var predictedValues = mlContext.Data.CreateEnumerable<PredictionOutput>(predictions, reuseRowObject: false).ToList();



            // 6. Exibe as previsões
            //Console.WriteLine("Previsões para os próximos 12 meses:");
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
                    Month = DateTime.Parse(values[0]),
                    IBOV = float.Parse(values[1], CultureInfo.InvariantCulture),
                    SPX = float.Parse(values[2], CultureInfo.InvariantCulture),
                    CDI = float.Parse(values[3], CultureInfo.InvariantCulture),
                    GOLD11 = float.Parse(values[4], CultureInfo.InvariantCulture),
                    USDBRL = float.Parse(values[5], CultureInfo.InvariantCulture),
                    NASD11 = float.Parse(values[6], CultureInfo.InvariantCulture),
                    UUP = float.Parse(values[7], CultureInfo.InvariantCulture),
                    IMAB = float.Parse(values[8], CultureInfo.InvariantCulture),
                    IPCALongo = float.Parse(values[9], CultureInfo.InvariantCulture)
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

            for (var i = 6; i < assetReturnsData.Count; i++)
            {
                if (i > 6)
                {
                    for (var j = i - 6; j < i; j++)
                    {
                        if (j == i - 6)
                        {
                            assetReturnsData[i].CDIReturn6MonthsAgo = 1 + assetReturnsData[j].CDI / 100;
                            assetReturnsData[i].IBOVReturn6MonthsAgo = 1 + assetReturnsData[j].IBOV / 100;
                            assetReturnsData[i].SPXReturn6MonthsAgo = 1 + assetReturnsData[j].SPX / 100;
                            assetReturnsData[i].GOLD11Return6MonthsAgo = 1 + assetReturnsData[j].GOLD11 / 100;
                            assetReturnsData[i].NASD11Return6MonthsAgo = 1 + assetReturnsData[j].NASD11 / 100;
                            assetReturnsData[i].IMABReturn6MonthsAgo = 1 + assetReturnsData[j].IMAB / 100;
                            assetReturnsData[i].IPCALongoReturn6MonthsAgo = 1 + assetReturnsData[j].IPCALongo / 100;
                            assetReturnsData[i].UUPReturn6MonthsAgo = 1 + assetReturnsData[j].UUP / 100;
                            assetReturnsData[i].USDBRLReturn6MonthsAgo = 1 + assetReturnsData[j].USDBRL / 100;
                        }
                        else
                        {
                            // Atualizando as rentabilidades de 6 meses
                            assetReturnsData[i].CDIReturn6MonthsAgo = assetReturnsData[i].CDIReturn6MonthsAgo * (1 + assetReturnsData[j].CDI / 100);
                            assetReturnsData[i].IBOVReturn6MonthsAgo = assetReturnsData[i].IBOVReturn6MonthsAgo * (1 + assetReturnsData[j].IBOV / 100);
                            assetReturnsData[i].SPXReturn6MonthsAgo = assetReturnsData[i].SPXReturn6MonthsAgo * (1 + assetReturnsData[j].SPX / 100);
                            assetReturnsData[i].GOLD11Return6MonthsAgo = assetReturnsData[i].GOLD11Return6MonthsAgo * (1 + assetReturnsData[j].GOLD11 / 100);
                            assetReturnsData[i].NASD11Return6MonthsAgo = assetReturnsData[i].NASD11Return6MonthsAgo * (1 + assetReturnsData[j].NASD11 / 100);
                            assetReturnsData[i].IMABReturn6MonthsAgo = assetReturnsData[i].IMABReturn6MonthsAgo * (1 + assetReturnsData[j].IMAB / 100);
                            assetReturnsData[i].IPCALongoReturn6MonthsAgo = assetReturnsData[i].IPCALongoReturn6MonthsAgo * (1 + assetReturnsData[j].IPCALongo / 100);
                            assetReturnsData[i].UUPReturn6MonthsAgo = assetReturnsData[i].UUPReturn6MonthsAgo * (1 + assetReturnsData[j].UUP / 100);
                            assetReturnsData[i].USDBRLReturn6MonthsAgo = assetReturnsData[i].USDBRLReturn6MonthsAgo * (1 + assetReturnsData[j].USDBRL / 100);
                        }

                        if (j == i - 1)
                        {
                            // Atualizando as rentabilidades de 6 meses
                            assetReturnsData[i].CDIReturn6MonthsAgo = (assetReturnsData[i].CDIReturn6MonthsAgo - 1) * 100;
                            assetReturnsData[i].IBOVReturn6MonthsAgo = (assetReturnsData[i].IBOVReturn6MonthsAgo - 1) * 100;
                            assetReturnsData[i].SPXReturn6MonthsAgo = (assetReturnsData[i].SPXReturn6MonthsAgo - 1) * 100;
                            assetReturnsData[i].GOLD11Return6MonthsAgo = (assetReturnsData[i].GOLD11Return6MonthsAgo - 1) * 100;
                            assetReturnsData[i].NASD11Return6MonthsAgo = (assetReturnsData[i].NASD11Return6MonthsAgo - 1) * 100;
                            assetReturnsData[i].IMABReturn6MonthsAgo = (assetReturnsData[i].IMABReturn6MonthsAgo - 1) * 100;
                            assetReturnsData[i].IPCALongoReturn6MonthsAgo = (assetReturnsData[i].IPCALongoReturn6MonthsAgo - 1) * 100;
                            assetReturnsData[i].UUPReturn6MonthsAgo = (assetReturnsData[i].UUPReturn6MonthsAgo - 1) * 100;
                            assetReturnsData[i].USDBRLReturn6MonthsAgo = (assetReturnsData[i].USDBRLReturn6MonthsAgo - 1) * 100;

                        }
                    }
                }

                if (i > 12)
                {
                    for (var j = i - 12; j < i; j++)
                    {
                        if (j == i - 12)
                        {
                            assetReturnsData[i].CDIReturn12MonthsAgo = 1 + assetReturnsData[j].CDI / 100;
                            assetReturnsData[i].IBOVReturn12MonthsAgo = 1 + assetReturnsData[j].IBOV / 100;
                            assetReturnsData[i].SPXReturn12MonthsAgo = 1 + assetReturnsData[j].SPX / 100;
                            assetReturnsData[i].GOLD11Return12MonthsAgo = 1 + assetReturnsData[j].GOLD11 / 100;
                            assetReturnsData[i].NASD11Return12MonthsAgo = 1 + assetReturnsData[j].NASD11 / 100;
                            assetReturnsData[i].IMABReturn12MonthsAgo = 1 + assetReturnsData[j].IMAB / 100;
                            assetReturnsData[i].IPCALongoReturn12MonthsAgo = 1 + assetReturnsData[j].IPCALongo / 100;
                            assetReturnsData[i].UUPReturn12MonthsAgo = 1 + assetReturnsData[j].UUP / 100;
                            assetReturnsData[i].USDBRLReturn12MonthsAgo = 1 + assetReturnsData[j].USDBRL / 100;
                        }
                        else
                        {
                            // Atualizando as rentabilidades de 12 meses
                            assetReturnsData[i].CDIReturn12MonthsAgo = assetReturnsData[i].CDIReturn12MonthsAgo * (1 + assetReturnsData[j].CDI / 100);
                            assetReturnsData[i].IBOVReturn12MonthsAgo = assetReturnsData[i].IBOVReturn12MonthsAgo * (1 + assetReturnsData[j].IBOV / 100);
                            assetReturnsData[i].SPXReturn12MonthsAgo = assetReturnsData[i].SPXReturn12MonthsAgo * (1 + assetReturnsData[j].SPX / 100);
                            assetReturnsData[i].GOLD11Return12MonthsAgo = assetReturnsData[i].GOLD11Return12MonthsAgo * (1 + assetReturnsData[j].GOLD11 / 100);
                            assetReturnsData[i].NASD11Return12MonthsAgo = assetReturnsData[i].NASD11Return12MonthsAgo * (1 + assetReturnsData[j].NASD11 / 100);
                            assetReturnsData[i].IMABReturn12MonthsAgo = assetReturnsData[i].IMABReturn12MonthsAgo * (1 + assetReturnsData[j].IMAB / 100);
                            assetReturnsData[i].IPCALongoReturn12MonthsAgo = assetReturnsData[i].IPCALongoReturn12MonthsAgo * (1 + assetReturnsData[j].IPCALongo / 100);
                            assetReturnsData[i].UUPReturn12MonthsAgo = assetReturnsData[i].UUPReturn12MonthsAgo * (1 + assetReturnsData[j].UUP / 100);
                            assetReturnsData[i].USDBRLReturn12MonthsAgo = assetReturnsData[i].USDBRLReturn12MonthsAgo * (1 + assetReturnsData[j].USDBRL / 100);
                        }

                        if (j == i - 1)
                        {
                            // Atualizando as rentabilidades de 12 meses
                            assetReturnsData[i].CDIReturn12MonthsAgo = (assetReturnsData[i].CDIReturn12MonthsAgo - 1) * 100;
                            assetReturnsData[i].IBOVReturn12MonthsAgo = (assetReturnsData[i].IBOVReturn12MonthsAgo - 1) * 100;
                            assetReturnsData[i].SPXReturn12MonthsAgo = (assetReturnsData[i].SPXReturn12MonthsAgo - 1) * 100;
                            assetReturnsData[i].GOLD11Return12MonthsAgo = (assetReturnsData[i].GOLD11Return12MonthsAgo - 1) * 100;
                            assetReturnsData[i].NASD11Return12MonthsAgo = (assetReturnsData[i].NASD11Return12MonthsAgo - 1) * 100;
                            assetReturnsData[i].IMABReturn12MonthsAgo = (assetReturnsData[i].IMABReturn12MonthsAgo - 1) * 100;
                            assetReturnsData[i].IPCALongoReturn12MonthsAgo = (assetReturnsData[i].IPCALongoReturn12MonthsAgo - 1) * 100;
                            assetReturnsData[i].UUPReturn12MonthsAgo = (assetReturnsData[i].UUPReturn12MonthsAgo - 1) * 100;
                            assetReturnsData[i].USDBRLReturn12MonthsAgo = (assetReturnsData[i].USDBRLReturn12MonthsAgo - 1) * 100;
                        }
                    }
                }

                if (i > 18)
                {
                    for (var j = i - 18; j < i; j++)
                    {
                        if (j == i - 18)
                        {
                            assetReturnsData[i].CDIReturn18MonthsAgo = 1 + assetReturnsData[j].CDI / 100;
                            assetReturnsData[i].IBOVReturn18MonthsAgo = 1 + assetReturnsData[j].IBOV / 100;
                            assetReturnsData[i].SPXReturn18MonthsAgo = 1 + assetReturnsData[j].SPX / 100;
                            assetReturnsData[i].GOLD11Return18MonthsAgo = 1 + assetReturnsData[j].GOLD11 / 100;
                            assetReturnsData[i].NASD11Return18MonthsAgo = 1 + assetReturnsData[j].NASD11 / 100;
                            assetReturnsData[i].IMABReturn18MonthsAgo = 1 + assetReturnsData[j].IMAB / 100;
                            assetReturnsData[i].IPCALongoReturn18MonthsAgo = 1 + assetReturnsData[j].IPCALongo / 100;
                            assetReturnsData[i].UUPReturn18MonthsAgo = 1 + assetReturnsData[j].UUP / 100;
                            assetReturnsData[i].USDBRLReturn18MonthsAgo = 1 + assetReturnsData[j].USDBRL / 100;
                        }
                        else
                        {
                            // Atualizando as rentabilidades de 18 meses
                            assetReturnsData[i].CDIReturn18MonthsAgo = assetReturnsData[i].CDIReturn18MonthsAgo * (1 + assetReturnsData[j].CDI / 100);
                            assetReturnsData[i].IBOVReturn18MonthsAgo = assetReturnsData[i].IBOVReturn18MonthsAgo * (1 + assetReturnsData[j].IBOV / 100);
                            assetReturnsData[i].SPXReturn18MonthsAgo = assetReturnsData[i].SPXReturn18MonthsAgo * (1 + assetReturnsData[j].SPX / 100);
                            assetReturnsData[i].GOLD11Return18MonthsAgo = assetReturnsData[i].GOLD11Return18MonthsAgo * (1 + assetReturnsData[j].GOLD11 / 100);
                            assetReturnsData[i].NASD11Return18MonthsAgo = assetReturnsData[i].NASD11Return18MonthsAgo * (1 + assetReturnsData[j].NASD11 / 100);
                            assetReturnsData[i].IMABReturn18MonthsAgo = assetReturnsData[i].IMABReturn18MonthsAgo * (1 + assetReturnsData[j].IMAB / 100);
                            assetReturnsData[i].IPCALongoReturn18MonthsAgo = assetReturnsData[i].IPCALongoReturn18MonthsAgo * (1 + assetReturnsData[j].IPCALongo / 100);
                            assetReturnsData[i].UUPReturn18MonthsAgo = assetReturnsData[i].UUPReturn18MonthsAgo * (1 + assetReturnsData[j].UUP / 100);
                            assetReturnsData[i].USDBRLReturn18MonthsAgo = assetReturnsData[i].USDBRLReturn18MonthsAgo * (1 + assetReturnsData[j].USDBRL / 100);
                        }

                        if (j == i - 1)
                        {
                            // Atualizando as rentabilidades de 18 meses
                            assetReturnsData[i].CDIReturn18MonthsAgo = (assetReturnsData[i].CDIReturn18MonthsAgo - 1) * 100;
                            assetReturnsData[i].IBOVReturn18MonthsAgo = (assetReturnsData[i].IBOVReturn18MonthsAgo - 1) * 100;
                            assetReturnsData[i].SPXReturn18MonthsAgo = (assetReturnsData[i].SPXReturn18MonthsAgo - 1) * 100;
                            assetReturnsData[i].GOLD11Return18MonthsAgo = (assetReturnsData[i].GOLD11Return18MonthsAgo - 1) * 100;
                            assetReturnsData[i].NASD11Return18MonthsAgo = (assetReturnsData[i].NASD11Return18MonthsAgo - 1) * 100;
                            assetReturnsData[i].IMABReturn18MonthsAgo = (assetReturnsData[i].IMABReturn18MonthsAgo - 1) * 100;
                            assetReturnsData[i].IPCALongoReturn18MonthsAgo = (assetReturnsData[i].IPCALongoReturn18MonthsAgo - 1) * 100;
                            assetReturnsData[i].UUPReturn18MonthsAgo = (assetReturnsData[i].UUPReturn18MonthsAgo - 1) * 100;
                            assetReturnsData[i].USDBRLReturn18MonthsAgo = (assetReturnsData[i].USDBRLReturn18MonthsAgo - 1) * 100;
                        }
                    }
                }

                processedData.Add(assetReturnsData[i]);
            }

            return processedData;
        }

        // Método para treinar o modelo
        private static ITransformer TrainModel(MLContext mlContext, IDataView trainData)
        {
            var pipeline = mlContext.Transforms.Concatenate("Features",
                nameof(AssetReturnData.CDIReturn12MonthsAgo),
                nameof(AssetReturnData.CDIReturn18MonthsAgo),
                nameof(AssetReturnData.CDIReturn6MonthsAgo),
                nameof(AssetReturnData.CDI),
                nameof(AssetReturnData.IBOVReturn12MonthsAgo),
                nameof(AssetReturnData.IBOVReturn18MonthsAgo),
                nameof(AssetReturnData.IBOVReturn6MonthsAgo),
                nameof(AssetReturnData.IBOV),
                nameof(AssetReturnData.IPCALongoReturn12MonthsAgo),
                nameof(AssetReturnData.IPCALongoReturn18MonthsAgo),
                nameof(AssetReturnData.IPCALongoReturn6MonthsAgo),
                nameof(AssetReturnData.IPCALongo),
                nameof(AssetReturnData.IMABReturn12MonthsAgo),
                nameof(AssetReturnData.IMABReturn18MonthsAgo),
                nameof(AssetReturnData.IMABReturn6MonthsAgo),
                nameof(AssetReturnData.IMAB),
                nameof(AssetReturnData.UUPReturn12MonthsAgo),
                nameof(AssetReturnData.UUPReturn18MonthsAgo),
                nameof(AssetReturnData.UUPReturn6MonthsAgo),
                nameof(AssetReturnData.UUP),
                nameof(AssetReturnData.SPXReturn12MonthsAgo),
                nameof(AssetReturnData.SPXReturn18MonthsAgo),
                nameof(AssetReturnData.SPXReturn6MonthsAgo),
                nameof(AssetReturnData.SPX),
                nameof(AssetReturnData.USDBRLReturn12MonthsAgo),
                nameof(AssetReturnData.USDBRLReturn18MonthsAgo),
                nameof(AssetReturnData.USDBRLReturn6MonthsAgo),
                nameof(AssetReturnData.USDBRL),
                nameof(AssetReturnData.SPXReturn12MonthsAgo),
                nameof(AssetReturnData.SPXReturn18MonthsAgo),
                nameof(AssetReturnData.SPXReturn6MonthsAgo),
                nameof(AssetReturnData.SPX),
                nameof(AssetReturnData.GOLD11Return12MonthsAgo),
                nameof(AssetReturnData.GOLD11Return18MonthsAgo),
                nameof(AssetReturnData.GOLD11Return6MonthsAgo),
                nameof(AssetReturnData.GOLD11)
                )
                .Append(mlContext.Transforms.NormalizeMinMax("Features"))
                //.Append(mlContext.Regression.Trainers.Sdca(labelColumnName: nameof(AssetReturnData.IBOV), maximumNumberOfIterations: 100));
                .Append(mlContext.Regression.Trainers.FastForest(labelColumnName: nameof(AssetReturnData.IBOV),numberOfLeaves:100,numberOfTrees:1000));

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
