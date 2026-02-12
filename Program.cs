using System;

using TaxisDrvAPI;

namespace TestKasoChduA
{
    internal class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine($"\n------- TestKasoChduA : run on: {DateTime.Now} -------");

            TaxisDrvAPI.TaxisDrvAPI taxisDrvAPI = new TaxisDrvAPI.TaxisDrvAPI();
            TaxisDrvAPIInitResult taxisDrvAPIInitResult = new TaxisDrvAPIInitResult();

            // DajQrKodPlatby
            TaxisQrKodPlatbyParams taxisQrKodPlatbyParams = new TaxisQrKodPlatbyParams();
            TaxisQrKodPlatbyStatus taxisQrKodPlatbyStatus = new TaxisQrKodPlatbyStatus();

            // ZrusQrKodPlatby
            TaxisZrusQrKodPlatbyParams taxisZrusQrKodPlatbyParams = new TaxisZrusQrKodPlatbyParams();
            TaxisZrusQrKodPlatbyStatus taxisZrusQrKodPlatbyStatus = new TaxisZrusQrKodPlatbyStatus();

            // OverStavQrPlatby
            TaxisOverQrKodPlatbyParams taxisOverQrKodPlatbyParams = new TaxisOverQrKodPlatbyParams();
            TaxisOverQrKodPlatbyStatus taxisOverQrKodPlatbyStatus = new TaxisOverQrKodPlatbyStatus();

            // DajQrTransakcie
            TaxisQrTransakcieParams taxisQrTransakcieParams = new TaxisQrTransakcieParams();
            TaxisQrTransakcieStatus taxisQrTransakcieStatus = new TaxisQrTransakcieStatus();

            // DajInfoQrPlatby
            TaxisInfoQrKoduPlatbyParams taxisInfoQrKoduPlatbyParams = new TaxisInfoQrKoduPlatbyParams();
            TaxisInfoQrKoduPlatbyStatus taxisInfoQrKoduPlatbyStatus = new TaxisInfoQrKoduPlatbyStatus();

            //--- set kverkom for init API of chdu
            TaxisKverkomSettings taxisKverkomSettings = new TaxisKverkomSettings()
            {
                Timeout = 30000,
                TypOverenia = ETypOvereniaPlatby.Transakcia,  // 0=TransakciaDB, 1=Transakcia, 2=Dkp : which method will be used to track the payment
                Log = "log_kverkom.log",
                //Zdroj = "TaxisKverkomApi.dll"
            };

            //--- init API chdu
            Console.WriteLine($"... wait please, device now incialísed ...");
            taxisDrvAPIInitResult = taxisDrvAPI.Init(new TaxisDrvAPISettings()
            {
                AutoLoginOfflineDocuments = true,
                DIC = "1234567890",
                DKP = "88812345678900001",
                Heslo = "88812345678900001",
                MaxTimeDifference = 4200,
                KontrolovatHodnotyPlatidiel = true,
                FsServiceTimeout = 7000,
                KverkomSettings = taxisKverkomSettings,
            });
            Console.WriteLine($".:: taxisDrvAPIInitResult :\n- OK={taxisDrvAPIInitResult.OK}, ErrorCode={taxisDrvAPIInitResult.ErrorCode}, WarningCode={taxisDrvAPIInitResult.WarningCode}, Message={taxisDrvAPIInitResult.Message}");
            //--- check API initialization result
            if (!taxisDrvAPIInitResult.OK)
            {
                Console.WriteLine($".:: API initialization failed. Exiting.");

                return;  // >>>>>>> EXIT
            }

            //--- set methods parameters for kverkom

            // for DajQrKodPlatby - generating a transaction number on a financial report
            taxisQrKodPlatbyParams = new TaxisQrKodPlatbyParams()
            {
                Qr = new TaxisQrKodParams()
                {
                    Create = true,
                    PixelsPerModule = 10,
                    Level = ECCLevelQr.M
                },
                TlacVystup = TlacVystup.Nie,
                VerziaPlatby = EPayme.V2_0,
                KontextPlatby = "m",
                Suma = 3.50m,
                IBAN = "SK8711000000002916560891",  // Tatra banka
                //+IBAN = "SK2783605207004202943822",  // mBank
                NazovUctu = "Attila Jancik - TEST QR pay",
                Info = $"Platba TEST, DT:{DateTime.Now.ToShortDateString().Replace(" ", "")}T{DateTime.Now.ToString("HH:mm:ss")}",
                DatumSplatnosti = DateTime.Now.Date,
                //+Printer = "Tlačiareň QR kódov"
            };

            // *****************************************************************************
            // **  here insert string of last successed transaction for "DajQrKodPlatby"  **
            // *****************************************************************************
            var lastQrId = "QR-95044452bacf453dbda78fc62ebe7733";


            // for ZrusQrKodPlatby - canceling a transaction number on a financial report
            taxisZrusQrKodPlatbyParams = new TaxisZrusQrKodPlatbyParams()
            {
                IdTransakcie = lastQrId,
                Datum= DateTime.Now.Date,
                //+PrintParams = 
                TlacVystup = TlacVystup.Tlacit
            };

            // for OverStavQrPlatby - checking the status of a transaction number on a financial report
            taxisOverQrKodPlatbyParams = new TaxisOverQrKodPlatbyParams()
            {
                IdTransakcie = lastQrId
            };

            // for DajQrTransakcie - getting a list of transactions for a given date
            taxisQrTransakcieParams = new TaxisQrTransakcieParams()
            {
                DatumOd = DateTime.Now.Date
                //+DatumOd = DateTime.Now.AddDays(-2)
            };

            // for DajInfoQrPlatby - getting information about a transaction number on a financial report (in NOP)
            taxisInfoQrKoduPlatbyParams = new TaxisInfoQrKoduPlatbyParams()
            {
                IdTransakcie = lastQrId
            };

            //--- run methods from api chdu/kverkom

            // choices: DajQrKodPlatby , ZrusQrKodPlatby , OverStavQrPlatby , DajQrTransakcie => [] , DajInfoQrPlatby
            var runFunction = "DajQrKodPlatby";  // <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<

            Console.WriteLine($"... wait please, now communications with servers ...");

            switch (runFunction)
            {
                case "DajQrKodPlatby":  // last QrId: "QR-990cfbd3750641638f23a0fa04c99c4f"
                    // DajQrKodPlatby - generating a transaction number on a financial report
                    Console.WriteLine($".:: Calling taxisDrvAPI.DajQrKodPlatby with parameters :\n- Qr.Create={taxisQrKodPlatbyParams.Qr.Create}, Qr.PixelsPerModule={taxisQrKodPlatbyParams.Qr.PixelsPerModule}, Qr.Level={taxisQrKodPlatbyParams.Qr.Level}," +
                        $"\n -  TlacVystup={taxisQrKodPlatbyParams.TlacVystup}, VerziaPlatby={taxisQrKodPlatbyParams.VerziaPlatby}, KontextPlatby={taxisQrKodPlatbyParams.KontextPlatby}, Suma={taxisQrKodPlatbyParams.Suma}, IBAN={taxisQrKodPlatbyParams.IBAN}, NazovUctu={taxisQrKodPlatbyParams.NazovUctu}, Info={taxisQrKodPlatbyParams.Info}, DatumSplatnosti={taxisQrKodPlatbyParams.DatumSplatnosti}");

                    taxisQrKodPlatbyStatus = taxisDrvAPI.DajQrKodPlatby(taxisQrKodPlatbyParams);

                    Console.WriteLine($".:: taxisQrKodPlatbyStatus :\n- OK={taxisQrKodPlatbyStatus.OK}, ErrorCode={taxisQrKodPlatbyStatus.ErrorCode}, WarningCode={taxisQrKodPlatbyStatus.WarningCode}, Message={taxisQrKodPlatbyStatus.Message}, IdTransakcie={taxisQrKodPlatbyStatus.IdTransakcie}, Datum={taxisQrKodPlatbyStatus.Datum}, Url={taxisQrKodPlatbyStatus.Url}, HTML={taxisQrKodPlatbyStatus.HTML}");
                    break;

                case "ZrusQrKodPlatby":
                    // ZrusQrKodPlatby - print confirmation that the payment by QR code was not paid
                    Console.WriteLine($".:: Calling taxisDrvAPI.ZrusQrKodPlatby with parameters :\n- IdTransakcie={taxisZrusQrKodPlatbyParams.IdTransakcie}, Datum={taxisZrusQrKodPlatbyParams.Datum}, TlacVystup={taxisZrusQrKodPlatbyParams.TlacVystup}");

                    taxisZrusQrKodPlatbyStatus = taxisDrvAPI.ZrusQrKodPlatby(taxisZrusQrKodPlatbyParams);

                    Console.WriteLine($".:: taxisZrusQrKodPlatbyStatus :\n- OK={taxisZrusQrKodPlatbyStatus.OK}, ErrorCode={taxisZrusQrKodPlatbyStatus.ErrorCode}, WarningCode={taxisZrusQrKodPlatbyStatus.WarningCode}, Message={taxisZrusQrKodPlatbyStatus.Message}");
                    break;

                case "OverStavQrPlatby":
                    // OverStavQrPlatby - checking the status of a transaction number on a financial report
                    Console.WriteLine($".:: Calling taxisDrvAPI.OverStavQrPlatby with parameters :\n- IdTransakcie={taxisOverQrKodPlatbyParams.IdTransakcie}");

                    taxisOverQrKodPlatbyStatus = taxisDrvAPI.OverStavQrPlatby(taxisOverQrKodPlatbyParams);

                    Console.WriteLine($".:: taxisOverQrKodPlatbyStatus :\n- OK={taxisOverQrKodPlatbyStatus.OK}, ErrorCode={taxisOverQrKodPlatbyStatus.ErrorCode}, WarningCode={taxisOverQrKodPlatbyStatus.WarningCode}, Message={taxisOverQrKodPlatbyStatus.Message}");
                    if (taxisOverQrKodPlatbyStatus.Transakcia != null)
                        Console.WriteLine($"\n - IdTransakcie={taxisOverQrKodPlatbyStatus.Transakcia.IdTransakcie}, Stav={taxisOverQrKodPlatbyStatus.Transakcia.Status}, Suma={taxisOverQrKodPlatbyStatus.Transakcia.Suma}, Mena={taxisOverQrKodPlatbyStatus.Transakcia.Mena}, NazovUctu={taxisOverQrKodPlatbyStatus.Transakcia.NazovUctu}, CisloUctu={taxisOverQrKodPlatbyStatus.Transakcia.CisloUctu}, Datum={taxisOverQrKodPlatbyStatus.Transakcia.Datum}");
                    break;

                case "DajQrTransakcie":
                    // DajQrTransakcie - getting a list of transactions for a given date
                    Console.WriteLine($".:: Calling taxisDrvAPI.DajQrTransakcie with parameters :\n- Datum={taxisQrTransakcieParams.DatumOd}");

                    taxisQrTransakcieStatus = taxisDrvAPI.DajQrTransakcie(taxisQrTransakcieParams);

                    Console.WriteLine($".:: taxisQrTransakcieStatus :\n- OK={taxisQrTransakcieStatus.OK}, ErrorCode={taxisQrTransakcieStatus.ErrorCode}, WarningCode={taxisQrTransakcieStatus.WarningCode}, Message={taxisQrTransakcieStatus.Message}");
                    if (taxisQrTransakcieStatus.Transakcie.Length > 0)
                    {
                        Console.WriteLine($"\n - List of transactions:");
                        foreach (var transakcia in taxisQrTransakcieStatus.Transakcie)
                            Console.WriteLine($"\n - IdTransakcie={transakcia.IdTransakcie}, Stav={transakcia.Status}, Suma={transakcia.Suma}, Mena={transakcia.Mena}, NazovUctu={transakcia.NazovUctu}, CisloUctu={transakcia.CisloUctu}, Datum={transakcia.Datum}");
                    }
                    break;

                case "DajInfoQrPlatby":
                    // DajInfoQrPlatby - getting information about a transaction number on a financial report (in NOP)
                    Console.WriteLine($".:: Calling taxisDrvAPI.DajInfoQrPlatby with parameters :\n- IdTransakcie={taxisInfoQrKoduPlatbyParams.IdTransakcie}");

                    taxisInfoQrKoduPlatbyStatus = taxisDrvAPI.DajInfoQrPlatby(taxisInfoQrKoduPlatbyParams);

                    Console.WriteLine($".:: taxisInfoQrKoduPlatbyStatus :\n- OK={taxisInfoQrKoduPlatbyStatus.OK}, ErrorCode={taxisInfoQrKoduPlatbyStatus.ErrorCode}, WarningCode={taxisInfoQrKoduPlatbyStatus.WarningCode}, Message={taxisInfoQrKoduPlatbyStatus.Message}");
                    if (taxisInfoQrKoduPlatbyStatus.Transakcia != null)
                        Console.WriteLine($"\n - IdTransakcie={taxisInfoQrKoduPlatbyStatus.Transakcia.IdTransakcie}, Suma={taxisInfoQrKoduPlatbyStatus.Transakcia.Suma}, Mena={taxisInfoQrKoduPlatbyStatus.Transakcia.Mena}, Datum={taxisInfoQrKoduPlatbyStatus.Transakcia.Datum}, CisloUctu={taxisInfoQrKoduPlatbyStatus.Transakcia.CisloUctu}, NazovUctu={taxisInfoQrKoduPlatbyStatus.Transakcia.NazovUctu},  Hash={taxisInfoQrKoduPlatbyStatus.GetHashCode()}, DatumVytvorenia={taxisInfoQrKoduPlatbyStatus.Transakcia.DatumVytvorenia},  DKP={taxisInfoQrKoduPlatbyStatus.Transakcia.DKP},  VAT={taxisInfoQrKoduPlatbyStatus.Transakcia.VAT},  Komentar={taxisInfoQrKoduPlatbyStatus.Transakcia.Komentar},  Topic={taxisInfoQrKoduPlatbyStatus.Transakcia.Topic},  DatumZaznamenaniaNOP={taxisInfoQrKoduPlatbyStatus.Transakcia.DatumZaznamenaniaNOP},  DatumParovania={taxisInfoQrKoduPlatbyStatus.Transakcia.DatumParovania}, KodBanky={taxisInfoQrKoduPlatbyStatus.Transakcia.KodBanky},  NazovBanky={taxisInfoQrKoduPlatbyStatus.Transakcia.NazovBanky},  KodPoziadavky={taxisInfoQrKoduPlatbyStatus.Transakcia.KodPoziadavky}, DatumSpristupnenia={taxisInfoQrKoduPlatbyStatus.Transakcia.DatumSpristupnenia}");

                    break;

            }

            taxisDrvAPI.Close();
            Console.WriteLine(".:: taxisDrvAPI is Close");
            Console.WriteLine($"\n------- TestKasoChduA : end on: {DateTime.Now} -------");
        }

    }
}

