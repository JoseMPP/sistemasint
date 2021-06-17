using System;
using CsvHelper;
using Microsoft.Azure.CognitiveServices.AnomalyDetector;
//using Microsoft.Azure.CognitiveServices.AnomalyDetector.Models;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.AnomalyDetector.Models;

namespace sistemasint
{
    class Program
    {
        static void Main(string[] args)
        {
            string myEndpoint = "https://anomalydetectorjmpp.cognitiveservices.azure.com/";

            string myKey = "c49336e4976c47d29cc7780fbac478aa";

            string csvFilePath = @"D:\\Sistemas Inteligentes\\datos__fabrica.csv";

            IAnomalyDetectorClient myClient = new AnomalyDetectorClient(new

            ApiKeyServiceClientCredentials(myKey))

            {

                Endpoint = myEndpoint

            };

            Request myRequest = GetPointsFromCsv(csvFilePath);

            EntireDetectSample(myClient, myRequest);

            LastDetectSample(myClient, myRequest);

            Console.WriteLine("\\nPress ENTER to exit");

            Console.ReadLine();
        }

        private static void LastDetectSample(IAnomalyDetectorClient AnomalyClient, Request AnomalyRequest)
        {
            Console.WriteLine("Detecting the anomaly status of the latest point in the series");

            LastDetectResponse result = AnomalyClient.LastDetectAsync(AnomalyRequest).Result;

            if (result.IsAnomaly)

            {

                Console.WriteLine("The latest point was detected as an anomaly");

            }

            else

            {

                Console.WriteLine("The latest point was not detected as an anomaly");

            }
        }

        private static void EntireDetectSample(IAnomalyDetectorClient AnomalyClient, Request AnomalyRequest)
        {
            Console.WriteLine("Detectando anomalías en toda la serie temporal....");

            EntireDetectResponse result =
            AnomalyClient.EntireDetectAsync(AnomalyRequest).Result;

            if (result.IsAnomaly.Contains(true))

            {
                Console.WriteLine("Una Anomalía fue detectada en el índice:");

                for (int idx = 0; idx < AnomalyRequest.Series.Count; ++idx)

{
                    if (result.IsAnomaly[idx])

{
                        Console.Write(idx);

                        Console.Write(" ");
                    }

                }

                Console.WriteLine();
            }

            else

            {
                Console.WriteLine(" No anomalies detected in the series");

            }
        }

        private static Request GetPointsFromCsv(string csvPath)
        {
            List < Point> myPointsList = new List < Point > ();

            using (System.IO.StreamReader myReader = new System.IO.StreamReader(csvPath, Encoding.UTF8))

            using (CsvReader myCsv = new CsvReader(myReader,
            CultureInfo.InvariantCulture))

            {

                myCsv.Read();

                myCsv.ReadHeader();

                while (myCsv.Read())

                {
                    Point onePoint = new Point

                    {
                        Timestamp = myCsv.GetField < DateTime> ("Fecha"),

                        Value = myCsv.GetField< Double> ("Temperatura")

                    };

                    myPointsList.Add(onePoint);

                }

            }

            // List MUST be ordered by date

            List < Point> myPointsListOrdered = myPointsList.OrderBy(o =>
             o.Timestamp).ToList();

            return new Request(myPointsListOrdered, Granularity.Daily);
        }
    }
}
