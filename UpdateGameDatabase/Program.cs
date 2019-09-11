using System;
using System.IO;

namespace UpdateGameDatabase
{
	class Program
	{
		static void Main(string[] args)
		{
			switch (args[0])
			{
				case "UID":
				case "U":
					Updates(args[0], args[1]);
					break;
				default:
					Inserts(args[1]);
					break;
			}
		}

		static void Updates(string arg, string s)
		{
			using (var reader = new StreamReader(@"D:\SEC.csv"))
			{
				using (var writer = new StreamWriter(@"D:\Updates.txt"))
				{
					string line;
					while ((line = reader.ReadLine()) != null)
					{
						var game = line.Split(Convert.ToChar(","));
						if (game[0] != "game")
						{
							for (int i = 0; i < game.Length; i++)
							{
								if (game[i] != "NULL" && i != 4)
								{
									game[i] = "'" + game[i] + "'";
									game[i] = game[i].Replace(";", ",");
								}
							}
							string whereClause = arg == "U"
								? string.Format(" where game = {0} and season = {1};", game[0], game[9])
								: string.Format(", game = {0} where id = {1};", game[0], game[9]);
							var lineToWrite = string.Format("Update {9} set time = {0}, week = {1}, network = {2}, networkjpg = {3}, coveragenotes = {4}, mediaindicator = {5}, " +
                                "TVType = {6}, listorder = {7}{8}"
								, game[3], game[4], game[1], game[2], game[5], game[6], game[7], game[8], whereClause, s);
							writer.WriteLine(lineToWrite);
						}
					}
					writer.Close();
				}
				reader.Close();
			}
		}

		static void Inserts(string s)
		{
            using (var reader = new StreamReader(@"D:\2016-17 MBK - Sheet1.csv"))
			{
				using (var writer = new StreamWriter(@"D:\inserts.txt"))
				{
					string line;
					writer.WriteLine(@"begin transaction;");
					int commitValue = 0;
					while ((line = reader.ReadLine()) != null)
					{
						var game = line.Split(Convert.ToChar(","));
						game[0] = game[0].Replace(";", ",");
						game[2] = game[2].Replace(";", ",");
						game[3] = game[3].Replace(";", ",");
						if (game[0] != "game")
						{
							for (int i = 0; i < game.Length; i++)
							{
								if (game[i] != "NULL" && i != 5)
									game[i] = "'" + game[i] + "'";
							}
							var lineToWrite = string.Format("Insert into {9} " + 
								"(game, network, networkjpg, coveragenotes, time, week, mediaindicator, conference, season) " +
								"VALUES " + "({0},{1},{2},{3},{4},{5},{6},{7},{8});", 
								game[0], game[1], game[2], game[3], game[4], game[5], game[6], game[7], game[8],s);
							writer.WriteLine(lineToWrite);
							commitValue++;
							if ((commitValue % 100) == 0)
							{
								writer.WriteLine(@"commit transaction;");
								writer.WriteLine(@"begin transaction;");
							}
						}
					}
					writer.WriteLine(@"commit transaction;");
					writer.Close();
				}
				reader.Close();
			}
		}
	}
}
