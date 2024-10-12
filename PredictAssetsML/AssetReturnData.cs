namespace PredictAssetsML
{
    public class AssetReturnData
    {
        public DateTime Month { get; set; } // Data da observação
        public float IBOV { get; set; } // Rentabilidade do IBOV
        public float SPX { get; set; } // Rentabilidade do SPX
        public float CDI { get; set; } // Rentabilidade do CDI
        public float GOLD11 { get; set; } // Rentabilidade do GOLD11
        public float USDBRL { get; set; } // Rentabilidade do USDBRL
        public float NASD11 { get; set; } // Rentabilidade do NASD11
        public float UUP { get; set; } // Rentabilidade do UUP
        public float IMAB { get; set; } // Rentabilidade do IMAB
        public float IPCALongo { get; set; } // Rentabilidade do IPCALongo

        // Rentabilidades dos ativos para 6, 12 e 18 meses anteriores
        public float IBOVReturn6MonthsAgo { get; set; } // Rentabilidade do IBOV 6 meses atrás
        public float IBOVReturn12MonthsAgo { get; set; } // Rentabilidade do IBOV 12 meses atrás
        public float IBOVReturn18MonthsAgo { get; set; } // Rentabilidade do IBOV 18 meses atrás

        public float SPXReturn6MonthsAgo { get; set; } // Rentabilidade do SPX 6 meses atrás
        public float SPXReturn12MonthsAgo { get; set; } // Rentabilidade do SPX 12 meses atrás
        public float SPXReturn18MonthsAgo { get; set; } // Rentabilidade do SPX 18 meses atrás

        public float CDIReturn6MonthsAgo { get; set; } // Rentabilidade do CDI 6 meses atrás
        public float CDIReturn12MonthsAgo { get; set; } // Rentabilidade do CDI 12 meses atrás
        public float CDIReturn18MonthsAgo { get; set; } // Rentabilidade do CDI 18 meses atrás

        public float GOLD11Return6MonthsAgo { get; set; } // Rentabilidade do GOLD11 6 meses atrás
        public float GOLD11Return12MonthsAgo { get; set; } // Rentabilidade do GOLD11 12 meses atrás
        public float GOLD11Return18MonthsAgo { get; set; } // Rentabilidade do GOLD11 18 meses atrás

        public float USDBRLReturn6MonthsAgo { get; set; } // Rentabilidade do USDBRL 6 meses atrás
        public float USDBRLReturn12MonthsAgo { get; set; } // Rentabilidade do USDBRL 12 meses atrás
        public float USDBRLReturn18MonthsAgo { get; set; } // Rentabilidade do USDBRL 18 meses atrás

        public float NASD11Return6MonthsAgo { get; set; } // Rentabilidade do NASD11 6 meses atrás
        public float NASD11Return12MonthsAgo { get; set; } // Rentabilidade do NASD11 12 meses atrás
        public float NASD11Return18MonthsAgo { get; set; } // Rentabilidade do NASD11 18 meses atrás

        public float UUPReturn6MonthsAgo { get; set; } // Rentabilidade do UUP 6 meses atrás
        public float UUPReturn12MonthsAgo { get; set; } // Rentabilidade do UUP 12 meses atrás
        public float UUPReturn18MonthsAgo { get; set; } // Rentabilidade do UUP 18 meses atrás

        public float IMABReturn6MonthsAgo { get; set; } // Rentabilidade do IMAB 6 meses atrás
        public float IMABReturn12MonthsAgo { get; set; } // Rentabilidade do IMAB 12 meses atrás
        public float IMABReturn18MonthsAgo { get; set; } // Rentabilidade do IMAB 18 meses atrás

        public float IPCALongoReturn6MonthsAgo { get; set; } // Rentabilidade do IPCALongo 6 meses atrás
        public float IPCALongoReturn12MonthsAgo { get; set; } // Rentabilidade do IPCALongo 12 meses atrás
        public float IPCALongoReturn18MonthsAgo { get; set; } // Rentabilidade do IPCALongo 18 meses atrás
    }
}
