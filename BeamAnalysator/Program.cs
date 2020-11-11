using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace BeamAnalysator
{
    class Program
    {
        private const string DataFileName = "BeamPattern_3GHz.txt";

        static void Main(string[] args)
        {
            if (!File.Exists(DataFileName))
            {
                Console.WriteLine("Файл данных не найден");
                return;
            }

            using StreamReader reader = File.OpenText(DataFileName);

            if (!reader.EndOfStream) reader.ReadLine();
            if (!reader.EndOfStream) reader.ReadLine();

            List<PatternValue> values = new List<PatternValue>();

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();

                if (line == "")
                    continue;

                string[] components = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                if (components.Length < 3)
                    continue;

                double theta = double.Parse(components[0], CultureInfo.InvariantCulture);
                double phi = double.Parse(components[1], CultureInfo.InvariantCulture);
                double dir_db = double.Parse(components[2], CultureInfo.InvariantCulture);

                values.Add(new PatternValue
                {
                    Theta = theta,
                    Phi = phi,
                    Dir_db = dir_db
                });
            }

            const double wanted_phi = 65;

            List<PatternValue> values_65 = new List<PatternValue>();

            for (var i = 0; i < values.Count; i++)
            {
                var value = values[i];
                if (value.Phi == wanted_phi)
                    values_65.Add(value);
                if(value.Phi == wanted_phi + 180 && value.Theta != 0)
                    values_65.Add(new PatternValue
                    {
                        Theta = -value.Theta,
                        Phi = wanted_phi,
                        Dir_db = value.Dir_db
                    });
            }

            values_65.Sort((v1, v2) => Comparer<double>.Default.Compare(v1.Theta, v2.Theta));

            using var writer = File.CreateText("Pattern65.txt");

            writer.WriteLine("ДН для угла 65 градусов");
            writer.WriteLine("Theta[deg]|F[-]");
            writer.WriteLine("-----------------------");

            for (var i = 0; i < values_65.Count; i++)
            {
                var value = values_65[i];

                writer.WriteLine("{0}\t{1}", value.Theta, Math.Pow(10, value.Dir_db / 20));
            }

            //Console.ReadLine();
        }
    }

    struct PatternValue
    {
        public double Theta;
        public double Phi;
        public double Dir_db;
    }
}
