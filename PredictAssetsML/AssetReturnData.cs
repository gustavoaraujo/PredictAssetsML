namespace PredictAssetsML
{
    public class AssetReturnData
    {
        public DateTime Date { get; set; } 
        //public float Price { get; set; }
        public float Yield { get; set; }

        public float TD10 { get; set; }
        public float TD30 { get; set; }
        public float TD90 { get; set; }

        public float TD10Y { get; set; }
        public float TD30Y { get; set; }
        public float TD90Y { get; set; }

    }
}
