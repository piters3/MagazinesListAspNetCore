using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CsvToDb
{
    class Program
    {
        static void Main(string[] args)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            Console.WriteLine("Rozpoczęto dodawanie danych do bazy");

            var valuesA = ProcessFile("A.csv");
            List<string[]> brokenMagazinesA = valuesA.Item2;
            List<Magazine> magazinesA = valuesA.Item1;

            var valuesB = ProcessFile("B.csv");
            List<string[]> brokenMagazinesB = valuesB.Item2;
            List<Magazine> magazinesB = valuesB.Item1;

            var valuesC = ProcessFile("C.csv");
            List<string[]> brokenMagazinesC = valuesC.Item2;
            List<Magazine> magazinesC = valuesC.Item1;

            List<Magazine> all = new List<Magazine>();

            all.AddRange(magazinesA);
            all.AddRange(RepairMagazines(brokenMagazinesA));
            all.AddRange(magazinesB);
            all.AddRange(RepairMagazines(brokenMagazinesB));
            all.AddRange(magazinesC);         
            all.AddRange(RepairMagazines(brokenMagazinesC));

            List<Magazine> magazinesWithoutNulls = RemoveNullMagazines(all);
            List<Magazine> reordered = ReorderLP(magazinesWithoutNulls);

            InsertToDatabase(reordered);

            stopwatch.Stop();
            Console.WriteLine("Czas trwania programu: {0:hh\\:mm\\:ss}", stopwatch.Elapsed);
        }

        private static List<Magazine> ReorderLP(List<Magazine> magazinesWithoutNulls)
        {
            int i = 1;
            foreach (var magazine in magazinesWithoutNulls)
            {
                if (magazine.LP != null)
                {
                    magazine.LP = i;
                    i++;
                }
            }
            return magazinesWithoutNulls;
        }


        private static List<Magazine> RemoveNullMagazines(List<Magazine> magazines)
        {
            List<Magazine> magazinesWithoutNulls = new List<Magazine>();
            foreach (var magazine in magazines)
            {
                if (magazine != null)
                {
                    magazine.List = magazine.List.Replace(";", string.Empty);   //dodane
                    magazine.Title = magazine.Title.Trim();                     //dodane
                    magazinesWithoutNulls.Add(magazine);
                }
            }
            return magazinesWithoutNulls.OrderBy(m=>m.Title).ToList();  //niepewne
        }


        private static void InsertToDatabase(List<Magazine> magazines)
        {
            using (var db = new MagazinesContext())
            {
                db.Database.EnsureCreated();
                if (!db.Magazines.Any())
                {
                    foreach (var magazine in magazines)
                    {
                        db.Magazines.Add(magazine);
                    }
                    db.SaveChanges();
                }
            }

            Console.WriteLine("Dane z pliku CSV zostały dodane do bazy danych!");
        }


        private static List<Magazine> RepairMagazines(List<string[]> broken)
        {
            List<string[]> repaired = new List<string[]>();
            List<string> repairedTitles = new List<string>();
            StringBuilder repairedTitle = new StringBuilder();
            List<Magazine> repairedMagazines = new List<Magazine>();

            for (int i = 0; i < broken.Count; i++)
            {
                for (int j = 0; j < broken[i].Length; j++)
                {
                    broken[i][j] = broken[i][j].Replace("\"", " ");

                    if (!broken[i][j].Any(char.IsDigit) && broken[i][j].Length > 2 && !broken[i][j].Contains("----"))
                    {
                        repairedTitle.Append(broken[i][j]);
                    }
                }
                if (repairedTitle.Length > 0)
                {
                    repairedTitles.Add(repairedTitle.ToString());
                    repairedTitle.Clear();
                }
            }

            for (int i = 0; i < broken.Count; i++)
            {
                for (int j = 0; j < broken[i].Length; j++)
                {
                    if (!broken[i][j].Any(char.IsDigit) && broken[i][j].Length > 2 && !broken[i][j].Contains("----"))
                    {
                        broken[i][j] = null;
                    }
                    broken[i][1] = repairedTitles[i];
                }
            }

            for (int i = 0; i < broken.Count; i++)
            {
                List<string> withoutNulls = new List<string>();

                for (int j = 0; j < broken[i].Length; j++)
                {
                    if (broken[i][j] == null)
                    {

                    }
                    else
                    {
                        withoutNulls.Add(broken[i][j]);
                    }
                }
                repaired.Add(withoutNulls.ToArray());
            }

            foreach (var item in repaired)
            {
                if (item.Length == 7)
                {
                    try
                    {
                        repairedMagazines.Add(new Magazine()
                        {
                            LP = item[0].Replace(" ", string.Empty) != string.Empty ? int.Parse(item[0]) : (int?)null,
                            Title = item[1],
                            ISSN = item[2],
                            EISSN = item[3],
                            Year = item[4],
                            Points = int.Parse(item[5]),
                            List = item[6]
                        });
                    }
                    catch
                    {
                    }
                }
            }

            return repairedMagazines;
        }

        private static (List<Magazine>, List<string[]>) ProcessFile(string path)
        {
            List<string[]> broken = new List<string[]>();
            string[] br = new string[10];

            var query = File.ReadAllLines(path)
                            .Where(l => l.Length > 1)
                            .Select(l =>
                            {
                                var columns = l.Split(',');
                                if (columns.Length == 7)
                                {
                                    try
                                    {
                                        return new Magazine
                                        {
                                            LP = columns[0] != string.Empty ? int.Parse(columns[0]) : (int?)null,
                                            Title = columns[1],
                                            ISSN = columns[2],
                                            EISSN = columns[3],
                                            Year = columns[4],
                                            Points = int.Parse(columns[5]),
                                            List = columns[6]
                                        };
                                    }
                                    catch
                                    {
                                        return null;
                                    }

                                }
                                else
                                {
                                    if (br.All(item => item == null))
                                    {
                                        br = columns;
                                    }
                                    else
                                    {
                                        br = br.Concat(columns).ToArray();
                                    }
                                    if (br.Length >= 8)
                                    {
                                        broken.Add(br);
                                        br = new string[10];
                                    }
                                    return null;
                                }
                            });

            return (query.ToList(), broken);
        }
    }
}
