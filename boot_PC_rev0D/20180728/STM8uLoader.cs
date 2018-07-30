// STM8uLoader.cs
using System;
using System.IO;
using System.IO.Ports;
using System.Threading;
using System.Collections.Generic;

public class STM8uLoader
{
	
	public static void Main(string[] arg)   { 
		Console.WriteLine(" STM8uLoader - компактный STM8 UART загрузчик/терминал v0D_27.07.2018."); 
		if (arg.Length == 0) {	Console.WriteLine(" Отсутствуют аргументы командной строки.");
								Console.WriteLine(" Первый аргумент (обязательный) - название файла прошивки.");
								Console.WriteLine(" Второй аргумент (не обязательный) - четыре шестнадцатеричные цифры слитно - адрес передачи управления.");
								Console.WriteLine(" Остальные аргументы игнорируются.");	
								Console.ReadKey(); return;}
		fileName = arg[0];
		FileOpenMemorySorting fileSorted = new FileOpenMemorySorting(fileName);
		Console.WriteLine(" " + fileName);

		if (arg.Length > 1) {
			goAdrCmdStr = int.Parse(arg[1], System.Globalization.NumberStyles.HexNumber);
			srtGoAddress = arg[1];
			Console.WriteLine(" Адрес передачи управления ${0:X4} из командной строки", goAdrCmdStr);
		}

		//Dumps	myDumps = new Dumps();
        Dumps.err_Msg = " OK";		Dumps.COM_port_Open();
		if(Dumps.err_Msg != " OK"){Console.WriteLine(Dumps.err_Msg); return;}
		Dumps.read_Key_Open();

			rvsn_byte = Dumps.Load_First_Dump(Dumps.Read_128000v0D, "Read_128000v0D");
			//rvsn_byte = Dumps.Load_First_Dump(Dumps.Reset_128000v0D, "Reset_128000v0D");

			
				Console.WriteLine(" FLASH из файла до сортировки");
				Dumps.Dump_To_Console(fileSorted.GetFLASHaddressByteSorted());
				
				Console.WriteLine(" Код загрузчика во FLASH");
				Dumps.Dump_To_Console( Dumps.Read_128k_v0D(0x8000, 8));
				
				Console.WriteLine(" FLASH из файла после сортировки");
				Dumps.Dumps_To_Console(Dumps.FLASH_AdrBlcsSorting_v0D(fileSorted.GetFLASHaddressByteSorted(), Dumps.Read_128k_v0D(0x8000, 8)));
			
				Console.WriteLine(Dumps.err_Msg);
				Console.WriteLine(" Содержимое FLASH области до записи");
				Dumps.Dump_To_Console(Dumps.Read_128k_v0D(0x8000, 64));
				Console.WriteLine(Dumps.err_Msg);
				
				//Dumps.Load_Dump_v0D(Dumps.WriteBlocks_FLASH_128000v0D88, "WriteBlocks_FLASH_128000v0D88");
				
			
				Console.WriteLine(" Содержимое FLASH области до записи");
				Dumps.Dump_To_Console(Dumps.Read_128k_v0D(0x8000, 64));
				
				Console.WriteLine(" Отправляем FLASH область на запись");
				Dumps.WriteBlocks_FLASH_128k_v0D(Dumps.FLASH_AdrBlcsSorting_v0D(fileSorted.GetFLASHaddressByteSorted(), Dumps.Read_128k_v0D(0x8000, 8)));
				
				Console.WriteLine(Dumps.err_Msg);
				
				Console.WriteLine(" Содержимое FLASH области после записи");
				Dumps.Dump_To_Console(Dumps.Read_128k_v0D(0x8000, 64));
				
				Console.WriteLine(" Перезагружаем устройство");
				Dumps.Load_Dump_v0D(Dumps.Reset_128000v0D, "Reset_128000v0D");



				
				
			
		/*	
		// проверяем целостность кода загрузчика в STM8 за исключением ячеек содержащих адреса передачи управления
		if (rvsn_byte == 0x14) {
			Dumps.Compare_Equ(0x9FFE, 0x9FFF, Dumps.rev14_srtDicAdrBts(), Dumps.Read_by_Mask128000(Dumps.rev14_srtDicAdrBts()));
			if ( Dumps.err_Msg == " OK" ) Console.WriteLine(" Код загрузчика boot_uC в STM8 соответствует версии ${0:X2} ", rvsn_byte);
			else {Console.WriteLine(Dumps.err_Msg); Exit();}// здесь есть смысл выйти из программы
			Dumps.Dumps_To_Console(Dumps.Read_by_Mask128000(Dumps.rev14_srtDicAdrBts()));
			
			if(fileSorted.GetFLASHaddressByteSorted().Count != 0){
			Dumps.Compare_NoCross(0x9FFE, 0x9FFF, Dumps.rev14_srtDicAdrBt(), fileSorted.GetFLASHaddressByteSorted());
			if ( Dumps.err_Msg == " OK" ) Console.WriteLine(" Файл прошивки не имеет адресов во FLASH памяти, пересекающихся с кодом загрузчика ");
			else {Console.WriteLine(Dumps.err_Msg); Exit();} // здесь есть смысл выйти из программы
			Dumps.err_Msg = " OK"; Dumps.Dump_To_Console(fileSorted.GetFLASHaddressByteSorted());
			} else Console.WriteLine(" Файл прошивки не содержит адресов для записи во FLASH память");
			
			if(fileSorted.GetOPTIONaddressByteSorted().Count != 0){
			Dumps.Compare_NoCross(0x9FFE, 0x9FFF, Dumps.rev14_srtDicAdrBt(), fileSorted.GetOPTIONaddressByteSorted());
			if ( Dumps.err_Msg == " OK" ) Console.WriteLine(" Файл прошивки не имеет адресов в блоке памяти OPTION Bytes, пересекающихся с кодом загрузчика ");
			else {Console.WriteLine(Dumps.err_Msg); Exit();} // здесь есть смысл выйти из программы
			Dumps.err_Msg = " OK"; Dumps.Dump_To_Console(fileSorted.GetOPTIONaddressByteSorted());
			} else Console.WriteLine(" Файл прошивки не содержит адресов для записи в блок памяти OPTION Bytes");
		}
		if (rvsn_byte == 0x25) {
			Dumps.Compare_Equ(0x4831, 0x4832, Dumps.rev25_srtDicAdrBts(), Dumps.Read_by_Mask128000(Dumps.rev25_srtDicAdrBts()));
			if ( Dumps.err_Msg == " OK" ) Console.WriteLine(" Код загрузчика boot_uC в STM8 соответствует версии ${0:X2} ", rvsn_byte);
			else {Console.WriteLine(Dumps.err_Msg); Exit();} // здесь есть смысл выйти из программы
			Dumps.Dumps_To_Console(Dumps.Read_by_Mask128000(Dumps.rev25_srtDicAdrBts()));
			
			if(fileSorted.GetFLASHaddressByteSorted().Count != 0){
			Dumps.Compare_NoCross(0x4831, 0x4832, Dumps.rev25_srtDicAdrBt(), fileSorted.GetFLASHaddressByteSorted());
			if ( Dumps.err_Msg == " OK" ) Console.WriteLine(" Файл прошивки не имеет адресов во FLASH памяти, пересекающихся с кодом загрузчика ");
			else {Console.WriteLine(Dumps.err_Msg); Exit();} // здесь есть смысл выйти из программы
			Dumps.err_Msg = " OK"; Dumps.Dump_To_Console(fileSorted.GetFLASHaddressByteSorted());
			} else Console.WriteLine(" Файл прошивки не содержит адресов для записи во FLASH память.");
			
			if(fileSorted.GetOPTIONaddressByteSorted().Count != 0){
			Dumps.Compare_NoCross(0x4831, 0x4832, Dumps.rev25_srtDicAdrBt(), fileSorted.GetOPTIONaddressByteSorted());
			if ( Dumps.err_Msg == " OK" ) Console.WriteLine(" Файл прошивки не имеет адресов в блоке памяти OPTION Bytes, пересекающихся с кодом загрузчика ");
			else {Console.WriteLine(Dumps.err_Msg); Exit();} // здесь есть смысл выйти из программы
			Dumps.err_Msg = " OK"; Dumps.Dump_To_Console(fileSorted.GetOPTIONaddressByteSorted());
			} else Console.WriteLine(" Файл прошивки не содержит адресов для записи в блок памяти OPTION Bytes.");

		}		
			//Console.WriteLine(Dumps.err_Msg);
			
			
		if(fileSorted.GetEEPROMaddressByteSorted().Count != 0){
			Console.WriteLine(" Файл прошивки содержит информацию для записи в EEPROM память.");
			Dumps.err_Msg = " OK"; Dumps.Dump_To_Console(fileSorted.GetEEPROMaddressByteSorted());
		} else Console.WriteLine(" Файл прошивки не содержит адресов для записи в EEPROM память.");
			
		if(fileSorted.GetRAMaddressByteSorted().Count != 0){
			Console.WriteLine(" Файл прошивки содержит информацию для записи в RAM память.");
			Dumps.err_Msg = " OK"; Dumps.Dump_To_Console(fileSorted.GetRAMaddressByteSorted());
		} else Console.WriteLine(" Файл прошивки не содержит адресов для записи в RAM память.");
			
			
			
			

			
			if(fileSorted.GetFLASHaddressByteSorted().Count != 0){
			srtDic = new SortedDictionary<int, byte>(fileSorted.GetFLASHaddressByteSorted());
			goAdrFrmSTM8=0;
			foreach( KeyValuePair<int, byte> kvp in srtDic ){
				if(kvp.Key == 0x9FFE) goAdrFrmSTM8 = goAdrFrmSTM8 + kvp.Value<<8;
				if(kvp.Key == 0x9FFF) goAdrFrmSTM8 = goAdrFrmSTM8 + kvp.Value;
			}

			Console.WriteLine(" Адрес передачи управления ${0:X4} в файле прошивки STM8", goAdrFrmSTM8);
				
			}

				
		//while(Dumps.key !='q'){
			


			
			if(fileSorted.GetFLASHaddressByteSorted().Count != 0){
				Console.WriteLine(" FLASH из файла до сортировки");
				Dumps.Dump_To_Console(fileSorted.GetFLASHaddressByteSorted());
				
				if (rvsn_byte == 0x14) {
				Console.WriteLine(" FLASH из файла после сортировки");
				Dumps.Dumps_To_Console(Dumps.FLASH_AdrBlcsSorting_v14(fileSorted.GetFLASHaddressByteSorted()));
				Console.WriteLine(" Отправляем FLASH область на запись");
				Dumps.WriteBlocks_FLASH_128000(Dumps.FLASH_AdrBlcsSorting_v14(fileSorted.GetFLASHaddressByteSorted()));
				Console.WriteLine(Dumps.err_Msg);
				Console.WriteLine(" Содержимое FLASH после записи.");
				Dumps.Dumps_To_Console(Dumps.ReadBlocks_128000(Dumps.FLASH_AdrBlcsSorting_v14(fileSorted.GetFLASHaddressByteSorted())));
				Console.WriteLine(Dumps.err_Msg);
				}
				if (rvsn_byte == 0x25) {
				Console.WriteLine(" FLASH из файла после сортировки");
				Dumps.Dumps_To_Console(Dumps.FLASH_AdrBlcsSorting_v25(fileSorted.GetFLASHaddressByteSorted()));
				Console.WriteLine(" Отправляем FLASH область на запись");
				Dumps.WriteBlocks_FLASH_128000(Dumps.FLASH_AdrBlcsSorting_v25(fileSorted.GetFLASHaddressByteSorted()));
				Console.WriteLine(Dumps.err_Msg);
				Console.WriteLine(" Содержимое FLASH после записи.");
				Dumps.Dumps_To_Console(Dumps.ReadBlocks_128000(Dumps.FLASH_AdrBlcsSorting_v25(fileSorted.GetFLASHaddressByteSorted())));
				Console.WriteLine(Dumps.err_Msg);
				}
			} 
			
			//Console.WriteLine();
			
			if(fileSorted.GetEEPROMaddressByteSorted().Count != 0){
				Console.WriteLine(" EEPROM из файла до сортировки");
				Dumps.Dump_To_Console(fileSorted.GetEEPROMaddressByteSorted());
				Console.WriteLine(" EEPROM из файла после сортировки");
				Dumps.Dumps_To_Console(Dumps.EEPROM_AdrBlcsSorting(fileSorted.GetEEPROMaddressByteSorted()));
				Console.WriteLine(" Отправляем EEPROM область на запись");
				Dumps.WriteBlocks_EEPROM_128000(Dumps.EEPROM_AdrBlcsSorting(fileSorted.GetEEPROMaddressByteSorted()));
				Console.WriteLine(Dumps.err_Msg);
				Console.WriteLine(" Содержимое EEPROM после записи.");
				Dumps.Dumps_To_Console(Dumps.ReadBlocks_128000(Dumps.EEPROM_AdrBlcsSorting(fileSorted.GetEEPROMaddressByteSorted())));
				Console.WriteLine(Dumps.err_Msg);
			} 
			

			if(fileSorted.GetOPTIONaddressByteSorted().Count != 0){
				Console.WriteLine(" OPTION из файла до сортировки");
				Dumps.Dump_To_Console(fileSorted.GetOPTIONaddressByteSorted());
				Console.WriteLine(" OPTION из файла после сортировки");
				Dumps.Dump_To_Console(Dumps.OPTION_AdrBlcsSorting(rvsn_byte, fileSorted.GetOPTIONaddressByteSorted()));
				Console.WriteLine(" Отправляем OPTION область на запись");
				Dumps.WriteBytes_OPTION_128000(Dumps.OPTION_AdrBlcsSorting(rvsn_byte, fileSorted.GetOPTIONaddressByteSorted()));
				Console.WriteLine(Dumps.err_Msg);
				Console.WriteLine(" Содержимое OPTION после записи.");
				Dumps.Dump_To_Console(Dumps.Read_128000(0x4800, 64));
				Console.WriteLine(Dumps.err_Msg);
			} 
			
			
			//Console.WriteLine();
			
			if(fileSorted.GetRAMaddressByteSorted().Count != 0){
				//Console.WriteLine(" RAM из файла");
				//Dumps.Dump_To_Console(fileSorted.GetRAMaddressByteSorted());
				//Console.WriteLine(" Отправляем RAM область на запись");
				Dumps.Write_RAM_128000(fileSorted.GetRAMaddressLineSorted());
				Console.WriteLine(" Содержимое RAM памяти после записи");
				Dumps.Dumps_To_Console(Dumps.Read_by_Mask128000(fileSorted.GetRAMaddressLineSorted()));
				//Console.WriteLine(Dumps.err_Msg);
			} else Console.WriteLine(" Область RAM не найдена");
			
			//Console.WriteLine();
	
			//Console.WriteLine(Dumps.err_Msg);
			
			
			if(srtGoAddress != ""){
				Console.WriteLine(" Передаем управление прикладной программе по адресу ${0:X4} в командной строке.", goAdrCmdStr);
				
		    	 go_bytes[0] = 0xF5;	//  GO cmd команда перехода
		    	 go_bytes[1] = 0x00;	//  YH не для этого метода
		    	 go_bytes[2] = 0x00;	//  YL не для этого метода
		    	 go_bytes[3] = (byte)(goAdrCmdStr>>8);	//  go_adrH 	старший байт требуемого адреса boot_OPTION
		    	 go_bytes[4] = (byte)goAdrCmdStr;	//  go_adrL 	младший байт требуемого адреса boot_OPTION
		    	 go_bytes[5] = 0x00;	//  cntr 	размер следующего к загрузке блока
				 Dumps.Go_Cmnd(go_bytes); // эту команду должен понимать каждый модуль
			} else {
					srtDic = new SortedDictionary<int, byte>(Dumps.Read_128000(0x9FFE, 2));
					goAdrFrmSTM8 = 0;
					foreach( KeyValuePair<int, byte> kvp in srtDic ){
						if(kvp.Key == 0x9FFE) goAdrFrmSTM8 = goAdrFrmSTM8 + kvp.Value<<8;
						if(kvp.Key == 0x9FFF) goAdrFrmSTM8 = goAdrFrmSTM8 + kvp.Value;	
					}
					Console.WriteLine(" Передаем управление прикладной программе по адресу ${0:X4} из STM8.", goAdrFrmSTM8);
			    	 go_bytes[0] = 0xF5;	//  GO cmd команда перехода
			    	 go_bytes[1] = 0x00;	//  YH не для этого метода
			    	 go_bytes[2] = 0x00;	//  YL не для этого метода
			    	 go_bytes[3] = (byte)(goAdrFrmSTM8>>8);	//  go_adrH 	старший байт требуемого адреса boot_OPTION
			    	 go_bytes[4] = (byte)goAdrFrmSTM8;;	//  go_adrL 	младший байт требуемого адреса boot_OPTION
			    	 go_bytes[5] = 0x00;	//  cntr 	размер следующего к загрузке блока
					 Dumps.Go_Cmnd(go_bytes); // эту команду должен понимать каждый модуль
			}
			
			Dumps.statusUART = "terminal";
			Console.Beep();
			Console.ForegroundColor = System.ConsoleColor.Green;
			Console.WriteLine(" Для передачи байта прикладной программе в шеснадцатиричном коде \n необходимо нажать последовательно две клавиши из 0...9, A...F.");
			Console.ForegroundColor = System.ConsoleColor.Gray;	
			
			while(Dumps.inCycle){

			}
			
		//}
		*/
					

		Dumps.read_Key_Close();	// останавливаем поток чтения клавиатуры		
		Dumps.COM_port_Close();
		Console.ReadKey(); return;

    } // Main();
	
	static void Exit(){
	
			Dumps.read_Key_Close();	// останавливаем поток чтения клавиатуры		
		Dumps.COM_port_Close();
		Console.ReadKey(); return;
	
	}
	


    
	public static bool unready = true;
    
	public static byte rx_byte = 0x00;
	public static byte rvsn_byte;
	public static byte[] btBytes = new byte[1];
	public static byte[] wrd4Bytes = new byte[4];
	public static byte[] blck64Bytes = new byte[64];
	public static byte[] blck192Bytes = new byte[192];
	public static byte[] txBytes = {0, 0};
	public static byte[] go_bytes = { 0xF5, 0x00, 0x00, 0x00, 0x17, 0x00}; // { GO cmd, YH, YL, go_adrH, go_adrL, cntr }
	
	public static int goAdrCmdStr;
	public static int goAdrFrmSTM8;
	public static int blckSize;
	
	public static SortedDictionary<int, byte> srtDic;

	public static string readLine = "";	
	public static string fileName = "";	
	public static string srtGoAddress = "";	
	public static string stringMemoryMap = "";
	
    public static Thread readThreadBytes;
    public static Thread readThreadKeys;

} // class STM8uLoader
// STM8uLoader.cs