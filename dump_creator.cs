// dump_creator.cs
using System;
using System.IO;
using System.Collections.Generic;

public partial class Dump_creator
{
	
	public static void Main(string[] arg)   { 
		
		if (arg.Length == 0) {Console.WriteLine("Отсутствуют аргументы командной строки"); 
			Console.ReadKey(); return;}
		else fileName = arg[0];
		
		Console.WriteLine("Обнаружены {0} аргумент(ов) командной строки", arg.Length);
		
		foreach(string strArg in arg){
			Console.WriteLine(strArg);
		}//foreach
		Console.WriteLine("Загружен файл {0}", fileName);
		
		fileToWrite = fileName.Replace(".s19", "");
		fileToWrite = fileToWrite.Replace(".S19", "");
		fileToWrite = fileToWrite.Replace(".hex", "");
		fileToWrite = fileToWrite.Replace(".HEX", "");
		
		stringDump = stringDump + fileToWrite + " = { ";
						
		FileOpenMemorySorting fileSorted = new FileOpenMemorySorting(fileName);
		
		List<byte> list_Bytes = new List<byte>();
		list_Bytes.Clear();
		
		Console.Write("\nRAM memory:");
		foreach( KeyValuePair<long, byte> kvp in fileSorted.GetRAMaddressByteSorted() ){
			//if(kvp.Key % 16 == 0) Console.Write("\n${0:X4} ", kvp.Key);
			if(kvp.Key % 16 == 0) Console.Write("\n${0:X4} ", kvp.Key);
			Console.Write("${0:X2} ", kvp.Value);
		}// foreach
		Console.WriteLine("\n");
		
		
		// извлечем дамп для RAM памяти
		foreach( KeyValuePair<long, byte> kvp in fileSorted.GetAllMemoryaddressByteSorted() ){
			list_Bytes.Add(kvp.Value);
		}// foreach
		list_Bytes.Reverse();  // дамп должен быть передан в обратном порядке
		byte[] bytesDump = list_Bytes.ToArray();

		for(int i = 0; i < bytesDump.Length; i++){
			stringDump = stringDump + String.Format("0x{0:X2}", bytesDump[i]);
			if(i < bytesDump.Length - 1) stringDump = stringDump + ", ";
			if(i % 16 == 0) stringDump = stringDump + "\n";	
		}
		
		stringDump = stringDump + " };\n";
		Console.WriteLine(stringDump);
		
		StreamWriter fstr_out;

		try {
			fstr_out = new StreamWriter(fileToWrite + ".txt");
		}
		catch(IOException exc) {
			Console.WriteLine(exc.Message);
			Console.ReadLine();
			return;
		}
		
			try {
				fstr_out.WriteLine(stringDump);
			}
			catch(IOException exc) {
				Console.WriteLine(exc.Message);
				Console.ReadLine();
				return;
			}			

		fstr_out.Close();


		Console.ReadKey(); return;
    } // Main();
	public static string stringDump = "   public readonly static byte[] ";
	public readonly byte[] ReadBlock_128000 = {0x00, 0x01};
	public static string fileName;	
	public static string fileToWrite;	
}
// dump_creator.cs
