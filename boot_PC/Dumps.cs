// Dumps.cs
using System;
using System.IO;
using System.IO.Ports;
using System.Threading;
using System.Collections.Generic;

public class Dumps
{
        public Dumps()
        {
	
			//Load_Dump(ReadByte_128000v14);
	
			// переходим на общение с отправленным дампом, меняем скорость COM-порта
			// sPort.BaudRate = 128000;
        	
        }// Dumps()
        
	    public static byte Load_First_Dump(byte[] dmp, string str){
	    	// метод вызывается при загрузке первого блока в верхние адреса RAM после нажатия кнопки сброса
	    	byte[] dmpToLoad = dmp;
        	byte rvsn = 0;
			Console.WriteLine(" Ждем байт 0x14 или 0x25 версии загрузчика. Нажми кнопку сброса на плате.");
			while(unready)
			{
				while(queueBytes.Count == 0){}  rx_byte = queueBytes.Dequeue();
				//while(sPort.BytesToRead == 0){}  rx_byte = (byte)sPort.ReadByte(); // ждем версию загрузчика
				
				if(rx_byte == 0x14 | rx_byte == 0x25)
				{
					Console.WriteLine(" Принят байт 0x{0:X2} от STM8", rx_byte);	
					rvsn = 	rx_byte;				
					byte[] txBytes = {(byte)dmpToLoad.Length};
					sPort.Write(txBytes, 0, 1); // отправляем размер дампа
					unready = false;
				} else Console.WriteLine(" Принят байт 0x{0:X2}. Нажми кнопку сброса на плате.", rx_byte);
	
			}//while(unready)
			sPort.Write(dmpToLoad, 0, dmpToLoad.Length);				// отправляем сам дамп
			while(sPort.BytesToWrite > 0){} //Console.WriteLine(" В STM8 отправлен модуль \"" + str + "\"");
			crntModul = str;
			// пустой буфер еще не означает, что передача последнего байта завершилась
			// до переключения скорость надо выдержать паузу
			Thread.Sleep(100); // менее 40 миллисекунд начинает сбоить
			sPort.BaudRate = 128000;
			return rvsn;
        }// Load_first_Dump()

	    public static void Go_Cmnd(byte[] gbt){
	    	// метод вызывается при замене блока в верхних адресах RAM памяти
	    	// необходимо отправить команду загруженному блоку в формате  { GO cmd, YH, YL, go_adrH, go_adrL, cntr }
	    	err_Msg = " OK";
	    	go_bytes = gbt;
        	
        	queueBytes.Clear(); // очистим очередь приема

        	sPort.Write(go_bytes, 0, 1); // передаем команду $F5
        	
				while(queueBytes.Count == 0){}  rx_byte = queueBytes.Dequeue(); 
				        	if ( rx_byte != go_bytes[0] ){ 
        						err_Msg = " Эхо не соответствует команде 0xF5"; 
        						return; }
        	sPort.Write(go_bytes, 1, go_bytes.Length-1); // передаем адреса и размер модуля

			Thread.Sleep(10); 
	    }// Go_Cmnd()
        
        
	    public static void Load_Dump(byte[] dmp, string str){
	    	// метод вызывается при замене блока в верхних адресах RAM памяти
	    	// необходимо отправить команду загруженному блоку в формате  { GO cmd, YH, YL, go_adrH, go_adrL, cntr }
	    	err_Msg = " OK";
	    	byte[] dmpToLoad = dmp;
	    	go_bytes[0] = 0xF5;	//  GO cmd команда перехода
	    	 go_bytes[1] = 0x00;	//  YH не для этого метода
	    	 go_bytes[2] = 0x00;	//  YL не для этого метода
	    	go_bytes[3] = 0x00;	//  go_adrH 	старший байт требуемого адреса boot_OPTION
	    	go_bytes[4] = 0x17;	//  go_adrL 	младший байт требуемого адреса boot_OPTION
	    	go_bytes[5] = (byte)dmpToLoad.Length;	//  cntr 	размер следующего к загрузке блока
        	
        	queueBytes.Clear(); // очистим очередь приема

        	sPort.Write(go_bytes, 0, 1); // передаем команду $F5
        	
				while(queueBytes.Count == 0){}  rx_byte = queueBytes.Dequeue(); 
				        	if ( rx_byte != go_bytes[0] ){ 
        						err_Msg = " Эхо не соответствует команде 0xF5"; 
        						return; }
        	sPort.Write(go_bytes, 1, go_bytes.Length-1); // передаем адреса и размер модуля
        	sPort.Write(dmpToLoad, 0, dmpToLoad.Length); // передаем сам модуль
			while(sPort.BytesToWrite > 0){} //Console.WriteLine(" В STM8 отправлен модуль \"" + str + "\"");
			crntModul = str;
			Thread.Sleep(10); 
	    }// Load_Dump()
        
        
        
        
        public static void Compare_Equ(int excAdr1, int excAdr2, SortedDictionary<int, byte[]> srtDic1, SortedDictionary<int, byte[]> srtDic2){
        	// srtDic1 и srtDic2 дампы памяти для сравнения
        	// excAdr1 и excAdr2 адреса исключения, содержимое этих адресов не проверяется на равенство (ячейки содержащие адреса передачи управления)
        	// для версии $14 пара $9FFE:$9FFF, для версии $25 пара $4831:$4832 
			err_Msg = " OK";
			foreach(KeyValuePair<int, byte[]> kvp in srtDic2){
				if ( srtDic1.Count == srtDic2.Count ) {
					if (srtDic1.ContainsKey(kvp.Key)){
						if (srtDic1[kvp.Key].Length == srtDic2[kvp.Key].Length){
							for(int i=0; i < kvp.Value.Length; i++){
								if(srtDic1[kvp.Key][i] != srtDic2[kvp.Key][i])
									if (kvp.Key+i != excAdr1 & kvp.Key+i != excAdr2)
									err_Msg = err_Msg + String.Format(" Содержимые ячеек с адресом ${0:X4} в блоке с начальным адресом ${1:X4} не равны \n", kvp.Key+i, kvp.Key);
							}
						} else err_Msg = err_Msg + String.Format(" Количество байт в блоке с начальным адресом ${0:X4} не равны \n", kvp.Key);
					} else err_Msg = err_Msg + String.Format(" В первом дампе памяти отсутствует блок с начальным адресом ${0:X4} \n", kvp.Key);
				} else err_Msg = err_Msg + String.Format(" Количества блоков в дампах памяти не равны \n");
			} 
			if(err_Msg != " OK")	err_Msg = err_Msg.Substring(3); // вырежем " OK"
        }
        
        public static void Compare_NoCross(int excAdr1, int excAdr2, SortedDictionary<int, byte> srtDic1, SortedDictionary<int, byte> srtDic2){
        	// srtDic1 и srtDic2 дампы памяти для сравнения
        	// excAdr1 и excAdr2 адреса исключения, содержимое этих адресов не проверяется на пересечение (ячейки содержащие адреса передачи управления)
        	// для версии $14 пара $9FFE:$9FFF, для версии $25 пара $4831:$4832 
			SortedDictionary<int, byte> AddByte1 = new SortedDictionary<int, byte>(srtDic1);
			SortedDictionary<int, byte> AddByte2 = new SortedDictionary<int, byte>(srtDic2);
			err_Msg = " OK";
			foreach(KeyValuePair<int, byte> kvp in AddByte2){
				if (AddByte1.ContainsKey(kvp.Key)){
					if (kvp.Key != excAdr1 & kvp.Key != excAdr2){
						err_Msg = err_Msg + String.Format(" Адрес ${0:X4} файла прошивки пересекается с кодом загрузчика \n", kvp.Key);
					}
				}
			}
			if(err_Msg != " OK")	err_Msg = err_Msg.Substring(3); // вырежем " OK"
        }
        
        
        
        
        public static SortedDictionary<int, byte> Read_128000(int adr, int cntr){
        	err_Msg = " OK";
        	int addrSpc = adr;	//  начальный адрес оласти памяти для выгрузки (адрес первого блока)
        	int cntrSpc = cntr;	//  размер оласти памяти для выгрузки
			int sizeBlck = 64;  // размер блока 
			SortedDictionary<int, byte> AddressByte = new SortedDictionary<int, byte>();
        	AddressByte.Clear();
        	
        	if (crntModul != "Read_128000v14_25") Load_Dump(Dumps.Read_128000v14_25, "Read_128000v14_25");
        	
        	if ( addrSpc < 0x0000 | addrSpc >= 0xF000){ 
        		err_Msg = " Адрес должен быть в диапазоне 0x0000...0xEFFF"; 
        		return AddressByte; 
        	}

			int j = sizeBlck;  // размер блока 64 байта
			queueBytes.Clear(); // очистим очередь приема
			for (; cntrSpc > 0; ){
				if(cntrSpc > 64) j = sizeBlck; else j = cntrSpc;
				//Console.WriteLine(" ${0:X4}", addrSpc);
				//queueBytes.Clear(); // очистим очередь приема
			    
				tx2Bytes[0] = (byte)(addrSpc>>8);
				tx2Bytes[1] = (byte)addrSpc;
	
				sPort.Write(tx2Bytes, 0, 1); // отправляем старший байт адрес блока / команду
	
					while(queueBytes.Count == 0){}  rx_byte = queueBytes.Dequeue(); 
					//Console.WriteLine(" Принят байт 0x{0:X2}.", rx_byte);
					        	if ( rx_byte != tx2Bytes[0] ){ 
	        						err_Msg = " Эхо не соответствует старшему байту адреса"; 
	        						return AddressByte;}
        	
					sPort.Write(tx2Bytes, 1, 1); // отправляем младший байт адрес блока
					tx2Bytes[0] = (byte)j;
					sPort.Write(tx2Bytes, 0, 1); // отправляем размер блока
					while(sPort.BytesToWrite > 0){}
	
				for (; j > 0x0000; addrSpc++){ // принимаем 64 байта
					while(queueBytes.Count == 0){}  rx_byte = queueBytes.Dequeue();
					//Console.WriteLine(" ${0:X4} ${1:X2} {2}", addrSpc, rx_byte, cntrSpc);
					AddressByte.Add(addrSpc, rx_byte);
					cntrSpc--; j--;
				}
				Thread.Sleep(100); 
			}
			//Console.WriteLine();
			return AddressByte;
		}// Read_128000()

        
        
        
        
        
        
           
        public static SortedDictionary<int, byte[]> ReadBlocks_128000(SortedDictionary<int, byte[]> adrBt){
        	err_Msg = "OK";
        	SortedDictionary<int, byte[]> AddressByte = new SortedDictionary<int, byte[]>();
        	SortedDictionary<int, byte[]> AddressByteForEach = new SortedDictionary<int, byte[]>(adrBt);
        	
        	if (crntModul != "ReadBlocks_128000v14_25") Load_Dump(Dumps.ReadBlocks_128000v14_25, "ReadBlocks_128000v14_25");
        	
        	foreach( KeyValuePair<int, byte[]> kvp in AddressByteForEach ){
        			if ( kvp.Value.Length % 64 != 0 ) err_Msg = " Имеются блоки размером не кратные 64 байта";
        		}
        	
        	if ( err_Msg != " OK") return AddressByte;
        	
		    queueBytes.Clear(); // очистим очередь приема
		    int i = 0;
		    int offset = 0;
			foreach( KeyValuePair<int, byte[]> kvp in AddressByteForEach ){
		    	/*if ( kvp.Key < 0x0000 | (kvp.Key + kvp.Value.Length - 1) > 0x427F){
		        					err_Msg = " Содержимое не должно выходить за пределы диапазона $4000...$427F."; 
		        					Console.WriteLine(" 0x{0:X4} + 0x{1:X4} - 1 = 0x{2:X4}", kvp.Key, kvp.Value.Length, kvp.Key + kvp.Value.Length );
		        					return AddressByte;}*/
		    	//if (i==0) {
		    		offset = 0;
					tx2Bytes[0] = (byte)(kvp.Key>>8);
					tx2Bytes[1] = (byte)kvp.Key;
					sPort.Write(tx2Bytes, 0, 1); // отправляем старший байт адреса блока / команду
					//Console.WriteLine(" Отправлен старший байт адреса 0x{0:X2}", tx2Bytes[0]);
						while(queueBytes.Count == 0){}  rx_byte = queueBytes.Dequeue(); 
						//Console.WriteLine(" Принят байт 0x{0:X2}.", rx_byte);
						        	if ( rx_byte != tx2Bytes[0] ){ 
		        						err_Msg = " Эхо не соответствует старшему байту адреса"; 
		        						return AddressByte;}
	        	
						sPort.Write(tx2Bytes, 1, 1); // отправляем младший байт адреса блока
						//Console.WriteLine(" Отправлен младший байт адреса 0x{0:X2}", tx2Bytes[1]);
						
						tx2Bytes[0] = (byte)( kvp.Value.Length / 64 );
						sPort.Write(tx2Bytes, 0, 1); // отправляем количество блоков
						//Console.WriteLine(" Отправлено количество блоков 0x{0:X2}", tx2Bytes[0]);
						//Thread.Sleep(100); 
		    	//}//if
		    	
				//	while(queueBytes.Count == 0){}  rx_byte = queueBytes.Dequeue(); 
		    	//Console.WriteLine(" Принят байт 0x{0:X2}.", rx_byte);
					while(queueBytes.Count != kvp.Value.Length){}
		    	AddressByte.Add(kvp.Key, queueBytes.ToArray());
		    	queueBytes.Clear();
		    	
					Thread.Sleep(100);

			}
		    Thread.Sleep(10);

			return AddressByte;
		}// ReadBlocks_128000()    
        
        
        

		public static SortedDictionary<int, byte[]> Read_by_Mask128000(SortedDictionary<int, byte[]> stdctnr){
        	err_Msg = " OK";
			int sizeBlck = 64;  // размер блока 
			SortedDictionary<int, byte[]> forForEach = new SortedDictionary<int, byte[]>(stdctnr); // для цикла
			SortedDictionary<int, byte[]> AddressByte = new SortedDictionary<int, byte[]>(); // для возврата
        	AddressByte.Clear();

        	foreach(KeyValuePair<int, byte[]> kvp in forForEach){
	        	if ( kvp.Key < 0x0000 | kvp.Key >= 0xF000){ 
	        		err_Msg = " Адрес должен быть в диапазоне 0x0000...0xEFFF"; 
	        		return AddressByte; 
	        	}
        	}
        	
        	if (crntModul != "Read_128000v14_25") Load_Dump(Dumps.Read_128000v14_25, "Read_128000v14_25");	
        	
        	queueBytes.Clear(); // очистим очередь приема
        	
        	foreach(KeyValuePair<int, byte[]> kvp in forForEach){
	        	int offset = 0;	//  размер оласти памяти для выгрузки
				int j = sizeBlck;  // размер блока 64 байта
				queueBytes.Clear(); // очистим очередь приема     без этой строчки не работало

				
				for (; offset < kvp.Value.Length; ){
				//Console.WriteLine(" 0x{0:X4}", kvp.Key+offset);
					if(kvp.Value.Length - offset  > sizeBlck) j = sizeBlck; else j = kvp.Value.Length - offset;
					//Console.WriteLine(" ${0:X4}", addrSpc);
					//queueBytes.Clear(); // очистим очередь приема
				    
					tx2Bytes[0] = (byte)((kvp.Key+offset)>>8);
					tx2Bytes[1] = (byte)(kvp.Key+offset);
					
					//Console.WriteLine(" отправляем старший байт адрес блока / команду 0x{0:X2}", tx2Bytes[0]);
					sPort.Write(tx2Bytes, 0, 1); // отправляем старший байт адрес блока / команду
		
						while(queueBytes.Count == 0){}  rx_byte = queueBytes.Dequeue(); 
						//Console.WriteLine(" Принят байт 0x{0:X2}.", rx_byte);
						        	if ( rx_byte != tx2Bytes[0] ){ 
									err_Msg = String.Format(" Эхо 0x{0:X2} не соответствует старшему байту адреса 0x{1:X2}", rx_byte, tx2Bytes[0]);
		        						return AddressByte;}
	        	
						sPort.Write(tx2Bytes, 1, 1); // отправляем младший байт адреса блока
						tx2Bytes[0] = (byte)j;
						sPort.Write(tx2Bytes, 0, 1); // отправляем размер блока
						while(sPort.BytesToWrite > 0){}
		
	
						while(queueBytes.Count < offset + j){} // rx_byte = queueBytes.Dequeue();
						//Console.WriteLine(" ${0:X4} ${1:X2} {2}", addrSpc, rx_byte, crntSpc);
						offset = offset + j;
	
					}
					AddressByte.Add(kvp.Key, queueBytes.ToArray());
			}//foreach
				Thread.Sleep(100); 
			//Console.WriteLine();
			return AddressByte;
		}// Read_by_Mask128000()
	 
        
        
        
        
        
        
        
        
        
  
        public static void Write_RAM_128000(SortedDictionary<int, byte[]> adrBt){
        	err_Msg = " OK";
        	SortedDictionary<int, byte[]> AddressByte = new SortedDictionary<int, byte[]>(adrBt);
        	
        	if (crntModul != "Write_RAM_128000v14_25") Load_Dump(Dumps.Write_RAM_128000v14_25, "Write_RAM_128000v14_25");	
        	
		    queueBytes.Clear(); // очистим очередь приема
			
			foreach( KeyValuePair<int, byte[]> kvp in AddressByte ){
						if ( kvp.Key < 0x0000 | kvp.Key > 0x03FF){
		        					err_Msg = "Адрес должен быть в диапазоне $0000...#03FF."; 
		        					return;}
					tx2Bytes[0] = (byte)(kvp.Key>>8);
					tx2Bytes[1] = (byte)kvp.Key;
					
					sPort.Write(tx2Bytes, 0, 1); // отправляем старший байт адрес блока / команду
		
						while(queueBytes.Count == 0){}  rx_byte = queueBytes.Dequeue(); 
						//Console.WriteLine(" Принят байт 0x{0:X2}.", rx_byte);
						        	if ( rx_byte != tx2Bytes[0] ){ 
		        						err_Msg = " Эхо не соответствует старшему байту адреса"; 
		        						return;}
	        	
						sPort.Write(tx2Bytes, 1, 1); // отправляем младший байт адрес блока
						
						tx2Bytes[0] = (byte)kvp.Value.Length;
						sPort.Write(tx2Bytes, 0, 1); // отправляем размер блока
						sPort.Write(kvp.Value, 0, kvp.Value.Length); // отправляем сам блок
				while(sPort.BytesToWrite > 0){}		
				while(queueBytes.Count == 0){}  rx_byte = queueBytes.Dequeue(); 
				//Console.WriteLine(" Принят байт 0x{0:X2}.", rx_byte);
				if ( rx_byte != 0xFA ){ 
			        	err_Msg = " Ответ модуля не равен OK $FA"; 
			        	return;}	
			}
			return;
		}// Write_RAM_128000()

  
        
  
        public static void Copy_RAM_128000(int adrDst, int adrSrc, int cntr){
        	err_Msg = " OK";
        	int dstAdrCopy = adrDst;
        	int srcAdrCopy = adrSrc;
        	int cntrCopy  = cntr;
        					if ( dstAdrCopy < 0x0000 | (dstAdrCopy + cntrCopy) > 0x03FF){
		        					err_Msg = " Адреса для копирования должны быть в диапазоне $0000...#03FF."; 
		        					return;}
        	if (crntModul != "Copy_RAM_128000v14_25") Load_Dump(Dumps.Copy_RAM_128000v14_25, "Copy_RAM_128000v14_25");
        	
		    queueBytes.Clear(); // очистим очередь приема
		    
		    int j;
			for (; cntrCopy > 0; ){
				if(cntrCopy > 255) j = 255; else j = cntrCopy;
			    
				tx2Bytes[0] = (byte)(dstAdrCopy>>8);
				tx2Bytes[1] = (byte)dstAdrCopy;
	
				sPort.Write(tx2Bytes, 0, 1); // отправляем старший байт адреса назначения  / команду
	
					while(queueBytes.Count == 0){}  rx_byte = queueBytes.Dequeue(); 
					//Console.WriteLine(" Принят байт 0x{0:X2}.", rx_byte);
					        	if ( rx_byte != tx2Bytes[0] ){ 
	        						err_Msg = " Эхо не соответствует старшему байту адреса"; 
	        						return;} //else Console.WriteLine("Эхо соответствует старшему байту адреса");
        	
					sPort.Write(tx2Bytes, 1, 1); // отправляем младший байт байт адреса назначения
					
				tx2Bytes[0] = (byte)(srcAdrCopy>>8);
				tx2Bytes[1] = (byte)srcAdrCopy;
					sPort.Write(tx2Bytes, 0, 2); // отправляем адрес источника
					
				tx2Bytes[0] = (byte)j;
					sPort.Write(tx2Bytes, 0, 1); // отправляем размер блока для копирования
					
				while(queueBytes.Count == 0){}  rx_byte = queueBytes.Dequeue(); 
								//Console.WriteLine(" Принят байт 0x{0:X2}.", rx_byte);
								if ( rx_byte != 0xFA ){ 
							        	err_Msg = " Ответ модуля не равен OK $FA"; 
							        	return;} //else Console.WriteLine("Ответ модуля равен OK $FA");
				
				dstAdrCopy = dstAdrCopy + j;
				srcAdrCopy = srcAdrCopy + j;
				cntrCopy = cntrCopy - j;
			}
			return;
		}// Copy_RAM_128000()
       
        
        
        
        
        public static void WriteByte_EEPROM_128000(SortedDictionary<int, byte[]> adrBt){
        	err_Msg = " OK";
        	SortedDictionary<int, byte[]> AddressByte = new SortedDictionary<int, byte[]>(adrBt);
        	
        	if (crntModul != "WriteByte_EEPROM_128000v14_25") Load_Dump(Dumps.WriteByte_EEPROM_128000v14_25, "WriteByte_EEPROM_128000v14_25");

		    queueBytes.Clear(); // очистим очередь приема
			
			foreach( KeyValuePair<int, byte[]> kvp in AddressByte ){
		    	if ( kvp.Key < 0x4000 | kvp.Key > 0x427F ) {
		        					err_Msg = "Адрес должен быть в диапазоне $4000...$427F."; 
		        					return;}
					tx2Bytes[0] = (byte)(kvp.Key>>8);
					tx2Bytes[1] = (byte)kvp.Key;
					sPort.Write(tx2Bytes, 0, 1); // отправляем старший байт адрес блока / команду
		
						while(queueBytes.Count == 0){}  rx_byte = queueBytes.Dequeue(); 
						//Console.WriteLine(" Принят байт 0x{0:X2}.", rx_byte);
						        	if ( rx_byte != tx2Bytes[0] ){ 
		        						err_Msg = " Эхо не соответствует старшему байту адреса"; 
		        						return;}
	        	
						sPort.Write(tx2Bytes, 1, 1); // отправляем младший байт адрес блока
						
						//Thread.Sleep(100); 
										
						sPort.Write(kvp.Value, 0, 1); // отправляем байт для записи
				while(sPort.BytesToWrite > 0){}
				while(queueBytes.Count == 0){}  rx_byte = queueBytes.Dequeue();
				//Console.WriteLine(" Принят байт 0x{0:X2}.", rx_byte);
				if ( rx_byte != 0xFA ){ 
			        	err_Msg = " Ответ модуля не равен OK $FA"; 
			        	return;}	
				Thread.Sleep(100);
			}
		    Thread.Sleep(10);

			return;
		}// WriteByte_EEPROM_128000()
        


        public static void WriteWord_EEPROM_128000(SortedDictionary<int, byte[]> adrBt){
        	err_Msg = " OK";
        	SortedDictionary<int, byte[]> AddressByte = new SortedDictionary<int, byte[]>(adrBt);
        	
        	if (crntModul != "WriteWord_EEPROM_128000v14_25") Load_Dump(Dumps.WriteWord_EEPROM_128000v14_25, "WriteWord_EEPROM_128000v14_25");
        	
        	foreach( KeyValuePair<int, byte[]> kvp in AddressByte ){
        			if ( kvp.Value.Length != 4) err_Msg = " Имеются блоки размером не равные 4 байта";
        		}
        	
        	if ( err_Msg != " OK") return;
        	
		    queueBytes.Clear(); // очистим очередь приема
			
			foreach( KeyValuePair<int, byte[]> kvp in AddressByte ){
						if ( kvp.Key < 0x0000 | kvp.Key > 0x4240){
		        					err_Msg = " Начальный адрес должен быть в диапазоне $4000...;4240."; 
		        					return;}
					tx2Bytes[0] = (byte)(kvp.Key>>8);
					tx2Bytes[1] = (byte)kvp.Key;
					sPort.Write(tx2Bytes, 0, 1); // отправляем старший байт адрес блока / команду
		
						while(queueBytes.Count == 0){}  rx_byte = queueBytes.Dequeue(); 
						//Console.WriteLine(" Принят байт 0x{0:X2}.", rx_byte);
						        	if ( rx_byte != tx2Bytes[0] ){ 
		        						err_Msg = " Эхо не соответствует старшему байту адреса"; 
		        						return;}
	        	
						sPort.Write(tx2Bytes, 1, 1); // отправляем младший байт адрес блока
						
						tx2Bytes[0] = (byte)kvp.Value.Length;
						sPort.Write(tx2Bytes, 0, 1); // отправляем размер блока
						
						//Thread.Sleep(100); 
										
						sPort.Write(kvp.Value, 0, kvp.Value.Length); // отправляем сам блок
				while(sPort.BytesToWrite > 0){}
				while(queueBytes.Count == 0){}  rx_byte = queueBytes.Dequeue();
				//Console.WriteLine(" Принят байт 0x{0:X2}.", rx_byte);
				if ( rx_byte != 0xFA ){ 
			        	err_Msg = " Ответ модуля не равен OK $FA"; 
			        	return;}	
				Thread.Sleep(100);
			}
		    Thread.Sleep(10);

			return;
		}// WriteWord_EEPROM_128000()
         

        public static void WriteBlock_EEPROM_128000(SortedDictionary<int, byte[]> adrBt){
        	err_Msg = " OK";
        	SortedDictionary<int, byte[]> AddressByte = new SortedDictionary<int, byte[]>(adrBt);
        	
			if (crntModul != "WriteBlock_EEPROM_128000v1425") Load_Dump(Dumps.WriteBlock_EEPROM_128000v1425, "WriteBlock_EEPROM_128000v1425");
        	
        	foreach( KeyValuePair<int, byte[]> kvp in AddressByte ){
        			if ( kvp.Value.Length != 64) err_Msg = " Имеются блоки размером не равные 64 байта";
        		}
        	
        	if ( err_Msg != " OK") return;
        	
		    queueBytes.Clear(); // очистим очередь приема
			
			foreach( KeyValuePair<int, byte[]> kvp in AddressByte ){
						if ( kvp.Key < 0x0000 | kvp.Key > 0x4240){
		        					err_Msg = " Начальный адрес должен быть в диапазоне $4000...;4240."; 
		        					return;}
					tx2Bytes[0] = (byte)(kvp.Key>>8);
					tx2Bytes[1] = (byte)kvp.Key;
					sPort.Write(tx2Bytes, 0, 1); // отправляем старший байт адрес блока / команду
		
						while(queueBytes.Count == 0){}  rx_byte = queueBytes.Dequeue(); 
						//Console.WriteLine(" Принят байт 0x{0:X2}.", rx_byte);
						        	if ( rx_byte != tx2Bytes[0] ){ 
		        						err_Msg = " Эхо не соответствует старшему байту адреса"; 
		        						return;}
	        	
						sPort.Write(tx2Bytes, 1, 1); // отправляем младший байт адрес блока
						
						tx2Bytes[0] = (byte)kvp.Value.Length;
						sPort.Write(tx2Bytes, 0, 1); // отправляем размер блока
						
						//Thread.Sleep(100); 
										
						sPort.Write(kvp.Value, 0, kvp.Value.Length); // отправляем сам блок
				while(sPort.BytesToWrite > 0){}
				while(queueBytes.Count == 0){}  rx_byte = queueBytes.Dequeue();
				//Console.WriteLine(" Принят байт 0x{0:X2}.", rx_byte);
				if ( rx_byte != 0xFA ){ 
			        	err_Msg = " Ответ модуля не равен OK $FA"; 
			        	return;}	
				Thread.Sleep(100);
			}
		    Thread.Sleep(10);

			return;
		}// WriteBlock_EEPROM_128000()
         
        
        public static void WriteBlocks_EEPROM_128000(SortedDictionary<int, byte[]> adrBt){
        	err_Msg = "OK";
        	SortedDictionary<int, byte[]> AddressByte = new SortedDictionary<int, byte[]>(adrBt);
        	
			if (crntModul != "WriteBlocks_EEPROM_128000v14_25") Load_Dump(Dumps.WriteBlocks_EEPROM_128000v14_25, "WriteBlocks_EEPROM_128000v14_25");
        	
        	foreach( KeyValuePair<int, byte[]> kvp in AddressByte ){
        			if ( kvp.Value.Length % 64 != 0 ) err_Msg = " Имеются блоки размером не кратные 64 байта";
        		}
        	
        	if ( err_Msg != " OK") return;
        	
		    queueBytes.Clear(); // очистим очередь приема
		    int i = 0;
		    int offset = 0;
			foreach( KeyValuePair<int, byte[]> kvp in AddressByte ){
		    	if ( kvp.Key < 0x4000 | (kvp.Key + kvp.Value.Length - 1) > 0x427F){
		        					err_Msg = " Содержимое не должно выходить за пределы диапазона $4000...$427F."; 
		        					Console.WriteLine(" 0x{0:X4} + 0x{1:X4} - 1 = 0x{2:X4}", kvp.Key, kvp.Value.Length, kvp.Key + kvp.Value.Length );
		        					return;}
		    	if (i==0) {
		    		offset = 0;
					tx2Bytes[0] = (byte)(kvp.Key>>8);
					tx2Bytes[1] = (byte)kvp.Key;
					sPort.Write(tx2Bytes, 0, 1); // отправляем старший байт адреса блока / команду
					//Console.WriteLine(" Отправлен старший байт адреса 0x{0:X2}", tx2Bytes[0]);
						while(queueBytes.Count == 0){}  rx_byte = queueBytes.Dequeue(); 
						//Console.WriteLine(" Принят байт 0x{0:X2}.", rx_byte);
						        	if ( rx_byte != tx2Bytes[0] ){ 
		        						err_Msg = " Эхо не соответствует старшему байту адреса"; 
		        						return;}
	        	
						sPort.Write(tx2Bytes, 1, 1); // отправляем младший байт адреса блока
						//Console.WriteLine(" Отправлен младший байт адреса 0x{0:X2}", tx2Bytes[1]);
						
						tx2Bytes[0] = (byte)( kvp.Value.Length / 64 );
						sPort.Write(tx2Bytes, 0, 1); // отправляем количество блоков
						//Console.WriteLine(" Отправлено количество блоков 0x{0:X2}", tx2Bytes[0]);
						//Thread.Sleep(100); 
		    	}//if
		    	
		    	for(i = kvp.Value.Length ; offset < kvp.Value.Length; i = i - 64, offset = offset + 64){
		    		
							sPort.Write(kvp.Value, offset, 64); // отправляем сам блок
							//Console.WriteLine(" Отправлен блок");
					while(sPort.BytesToWrite > 0){}
					while(queueBytes.Count == 0){}  rx_byte = queueBytes.Dequeue();
					//Console.WriteLine(" Принят байт 0x{0:X2}.", rx_byte);
					/*if ( rx_byte != 0xFA ){ 
				        	err_Msg = "Ответ модуля не равен OK $FA"; 
				        	return;}*/
					Thread.Sleep(100);
		    	}//for
		    	i = 0;
		   		offset = 0;
			}
		    Thread.Sleep(10);

			return;
		}// WriteBlocks_EEPROM_128000()
        
        
        

        public static void WriteBytes_OPTION_128000(SortedDictionary<int, byte> adrBt){
        	err_Msg = " OK";
        	SortedDictionary<int, byte> AddressByte = new SortedDictionary<int, byte>(adrBt);
        	
			if (crntModul != "WriteBytes_OPTION_128000v1425") Load_Dump(Dumps.WriteBytes_OPTION_128000v1425, "WriteBytes_OPTION_128000v1425");
        	
        	if ( AddressByte.Count != 64) err_Msg = " Размер блока не равен 64 байта";
        	
        	if ( err_Msg != " OK") return;
        	
		    queueBytes.Clear(); // очистим очередь приема
			
			foreach( KeyValuePair<int, byte> kvp in AddressByte ){
						if ( kvp.Key < 0x4800 | kvp.Key > 0x483F){
		        					err_Msg = " Адрес должен быть в диапазоне $4800..$483F."; 
		        					return;}
		    	}
		    
					tx2Bytes[0] = 0x48;
					tx2Bytes[1] = 0x00;
					sPort.Write(tx2Bytes, 0, 1); // отправляем старший байт адрес блока / команду
		
						while(queueBytes.Count == 0){}  rx_byte = queueBytes.Dequeue(); 
						//Console.WriteLine(" Принят байт 0x{0:X2}.", rx_byte);
						        	if ( rx_byte != tx2Bytes[0] ){ 
		        						err_Msg = " Эхо не соответствует старшему байту адреса"; 
		        						return;}
	        	
						sPort.Write(tx2Bytes, 1, 1); // отправляем младший байт адрес блока
						
						tx2Bytes[0] = 64;
						sPort.Write(tx2Bytes, 0, 1); // отправляем размер блока
						
						//Thread.Sleep(100); 
							
			foreach( KeyValuePair<int, byte> kvp in AddressByte ){  // отправляем сам блок
					tx2Bytes[0] = kvp.Value;
					while(sPort.BytesToWrite > 0){}						
					sPort.Write(tx2Bytes, 0, 1); 
						}
				while(sPort.BytesToWrite > 0){}
				while(queueBytes.Count == 0){}  rx_byte = queueBytes.Dequeue();
				//Console.WriteLine(" Принят байт 0x{0:X2}.", rx_byte);
				if ( rx_byte != 0xFA ){ 
			        	err_Msg = " Ответ модуля не равен OK $FA"; 
			        	return;}	
				Thread.Sleep(100);

			return;
		}// WriteBytes_OPTION_128000()
        
        
        
        public static void WriteByte_FLASH_128000(SortedDictionary<int, byte[]> adrBt){
        	err_Msg = " OK";
        	SortedDictionary<int, byte[]> AddressByte = new SortedDictionary<int, byte[]>(adrBt);
        	
			if (crntModul != "WriteByte_FLASH_128000v14_25") Load_Dump(Dumps.WriteByte_FLASH_128000v14_25, "WriteByte_FLASH_128000v14_25");

		    queueBytes.Clear(); // очистим очередь приема
			
			foreach( KeyValuePair<int, byte[]> kvp in AddressByte ){
		    	if ( kvp.Key < 0x8004 | (kvp.Key > 0x9FEF & kvp.Key < 0x9FFE) ) {
		        					err_Msg = " Адрес должен быть в диапазоне $8000...$9FF0, $9FFE:$9FFF."; 
		        					return;}
					tx2Bytes[0] = (byte)(kvp.Key>>8);
					tx2Bytes[1] = (byte)kvp.Key;
					sPort.Write(tx2Bytes, 0, 1); // отправляем старший байт адрес блока / команду
		
						while(queueBytes.Count == 0){}  rx_byte = queueBytes.Dequeue(); 
						//Console.WriteLine(" Принят байт 0x{0:X2}.", rx_byte);
						        	if ( rx_byte != tx2Bytes[0] ){ 
		        						err_Msg = " Эхо не соответствует старшему байту адреса"; 
		        						return;}
	        	
						sPort.Write(tx2Bytes, 1, 1); // отправляем младший байт адрес блока
						
						//Thread.Sleep(100); 
										
						sPort.Write(kvp.Value, 0, 1); // отправляем байт для записи
				while(sPort.BytesToWrite > 0){}
				while(queueBytes.Count == 0){}  rx_byte = queueBytes.Dequeue();
				//Console.WriteLine(" Принят байт 0x{0:X2}.", rx_byte);
				if ( rx_byte != 0xFA ){ 
			        	err_Msg = " Ответ модуля не равен OK $FA"; 
			        	return;}	
				Thread.Sleep(100);
			}
		    Thread.Sleep(10);

			return;
		}// WriteByte_FLASH_128000()
          

        public static void WriteWord_FLASH_128000(SortedDictionary<int, byte[]> adrBt){
        	err_Msg = " OK";
        	SortedDictionary<int, byte[]> AddressByte = new SortedDictionary<int, byte[]>(adrBt);
        	
			if (crntModul != "WriteWord_FLASH_128000v14_25") Load_Dump(Dumps.WriteWord_FLASH_128000v14_25, "WriteWord_FLASH_128000v14_25");
        	
        	foreach( KeyValuePair<int, byte[]> kvp in AddressByte ){
        			if ( kvp.Value.Length != 4) err_Msg = " Имеются блоки размером не равные 4 байта";
        		}
        	
        	if ( err_Msg != " OK") return;
        	
		    queueBytes.Clear(); // очистим очередь приема
			
			foreach( KeyValuePair<int, byte[]> kvp in AddressByte ){
						if ( kvp.Key < 0x8004 | kvp.Key > 0x9FEC){
		        					err_Msg = " Начальный адрес должен быть в диапазоне $8000...$9FEC."; 
		        					return;}
					tx2Bytes[0] = (byte)(kvp.Key>>8);
					tx2Bytes[1] = (byte)kvp.Key;
					sPort.Write(tx2Bytes, 0, 1); // отправляем старший байт адрес блока / команду
		
						while(queueBytes.Count == 0){}  rx_byte = queueBytes.Dequeue(); 
						//Console.WriteLine(" Принят байт 0x{0:X2}.", rx_byte);
						        	if ( rx_byte != tx2Bytes[0] ){ 
		        						err_Msg = " Эхо не соответствует старшему байту адреса"; 
		        						return;}
	        	
						sPort.Write(tx2Bytes, 1, 1); // отправляем младший байт адрес блока
						
						tx2Bytes[0] = (byte)kvp.Value.Length;
						sPort.Write(tx2Bytes, 0, 1); // отправляем размер блока
						
						//Thread.Sleep(100); 
										
						sPort.Write(kvp.Value, 0, kvp.Value.Length); // отправляем сам блок
				while(sPort.BytesToWrite > 0){}
				while(queueBytes.Count == 0){}  rx_byte = queueBytes.Dequeue();
				//Console.WriteLine(" Принят байт 0x{0:X2}.", rx_byte);
				if ( rx_byte != 0xFA ){ 
			        	err_Msg = " Ответ модуля не равен OK $FA"; 
			        	return;}	
				Thread.Sleep(100);
			}
		    Thread.Sleep(10);

			return;
		}// WriteWord_FLASH_128000()
         

        public static void WriteBlocks_FLASH_128000(SortedDictionary<int, byte[]> adrBt){
        	err_Msg = "OK";
        	SortedDictionary<int, byte[]> AddressByte = new SortedDictionary<int, byte[]>(adrBt);
        	
			if (crntModul != "WriteBlocks_FLASH_128000v14_25") Load_Dump(Dumps.WriteBlocks_FLASH_128000v14_25, "WriteBlocks_FLASH_128000v14_25");
        	
        	foreach( KeyValuePair<int, byte[]> kvp in AddressByte ){
        			if ( kvp.Value.Length % 64 != 0 ) err_Msg = " Имеются блоки размером не кратные 64 байта";
        		}
        	
        	if ( err_Msg != " OK") return;
        	
		    queueBytes.Clear(); // очистим очередь приема
		    int i = 0;
		    int offset = 0;
			foreach( KeyValuePair<int, byte[]> kvp in AddressByte ){
		    	if ( kvp.Key < 0x8000 | (kvp.Key + kvp.Value.Length - 1) > 0x9FFF){
		        					err_Msg = " Содержимое не должно выходить за пределы диапазона $8000...$9FFF."; 
		        					Console.WriteLine(" 0x{0:X4} + 0x{1:X4} - 1 = 0x{2:X4}", kvp.Key, kvp.Value.Length, kvp.Key + kvp.Value.Length );
		        					return;}
		    	if (i==0) {
		    		offset = 0;
					tx2Bytes[0] = (byte)(kvp.Key>>8);
					tx2Bytes[1] = (byte)kvp.Key;
					sPort.Write(tx2Bytes, 0, 1); // отправляем старший байт адреса блока / команду
					//Console.WriteLine(" Отправлен старший байт адреса 0x{0:X2}", tx2Bytes[0]);
						while(queueBytes.Count == 0){}  rx_byte = queueBytes.Dequeue(); 
						//Console.WriteLine(" Принят байт 0x{0:X2}.", rx_byte);
						        	if ( rx_byte != tx2Bytes[0] ){ 
		        						err_Msg = "Эхо не соответствует старшему байту адреса"; 
		        						return;}
	        	
						sPort.Write(tx2Bytes, 1, 1); // отправляем младший байт адреса блока
						//Console.WriteLine(" Отправлен младший байт адреса 0x{0:X2}", tx2Bytes[1]);
						
						tx2Bytes[0] = (byte)( kvp.Value.Length / 64 );
						sPort.Write(tx2Bytes, 0, 1); // отправляем количество блоков
						//Console.WriteLine(" Отправлено количество блоков 0x{0:X2}", tx2Bytes[0]);
						//Thread.Sleep(100); 
		    	}//if
		    	
		    	for(i = kvp.Value.Length ; offset < kvp.Value.Length; i = i - 64, offset = offset + 64){
		    		
							sPort.Write(kvp.Value, offset, 64); // отправляем сам блок
							//Console.WriteLine(" Отправлен блок");
					while(sPort.BytesToWrite > 0){}
					while(queueBytes.Count == 0){}  rx_byte = queueBytes.Dequeue();
					//Console.WriteLine(" Принят байт 0x{0:X2}.", rx_byte);
					/*if ( rx_byte != 0xFA ){ 
				        	err_Msg = "Ответ модуля не равен OK $FA"; 
				        	return;}*/
					Thread.Sleep(100);
		    	}//for
		    	i = 0;
		   		offset = 0;
			}
		    Thread.Sleep(10);

			return;
		}// WriteBlocks_FLASH_128000()
         
        
         
        
        
        
        
        
        
        
        
        
        
        
        
        public static void COM_port_Open(){
			// инициализация COM порта
			if(SerialPort.GetPortNames().Length == 0)
			{	Dumps.err_Msg = " COM порты не найдены";
				//Console.WriteLine(" COM порты не найдены");
				//Console.ReadKey();
				return;}
			Console.WriteLine(" Доступны COM порты:");
	        foreach (string s in SerialPort.GetPortNames())
	        {
	            Console.WriteLine("   {0}", s);
				portName = s;
	        }
	        // создаем экземпляр COM-порта с настройками для общения с начальным загрузчиком
			sPort = new SerialPort(portName, 9600, Parity.None, 8, StopBits.One);
			 try
             {
                  sPort.Open();		//открываем COM порт                       
             }
			 catch (ThreadAbortException) {	Dumps.err_Msg = " sPort.Open() ThreadAbortException";
			 								//Console.WriteLine("sPort.Open() ThreadAbortException"); 
			 								return;}
             catch (IOException)  {	Dumps.err_Msg = " sPort.Open() IOException";
			 						//Console.WriteLine(" sPort.Open() IOException"); 
			 						return; }
             catch (TimeoutException)  {Dumps.err_Msg = " sPort.Open() TimeoutException";
			 						//Console.WriteLine(" sPort.Open() TimeoutException"); 
			 						return; }
             catch (Exception ex) {	Dumps.err_Msg = " sPort.Open() Exception" + ex.Message + "\n: read()";
			 						//Console.WriteLine(" sPort.Open() IOException"); 
			 						return; }
             
             queueBytes = new Queue<byte>();
             
             
		readThreadBytes = new Thread(Dumps.Read);
		readThreadBytes.IsBackground = true; // не позволит остаться потоку в памяти после закрытия программы ?
			try {
				readThreadBytes.Start();		// запускаем поток чтения COM порта
			} catch (Exception ex) {
									Console.WriteLine(" readThreadBytes.Start(). Exception" + ex.Message); 
									return; }
             
             
             //queueBytes.Clear();
	}
        
    public static void read_Key_Open(){
		readThreadKeys = new Thread(Dumps.readKey);
		readThreadKeys.IsBackground = true; // не позволит остаться потоку в памяти после закрытия программы ?
			try {
				readThreadKeys.Start();		// запускаем поток чтения COM порта
			} catch (Exception ex) {Console.WriteLine(" readThreadKeys.Start(). Exception" + ex.Message); } 
	}  
 	
	public static void COM_port_Close(){
        			readThreadBytes.Join();	// останавливаем поток чтения COM порта
			//readThread.Join();	// останавливаем поток чтения COM порта
			sPort.Close();		// закрываем COM порт
	}
       
	public static void read_Key_Close(){
		readThreadKeys.Join();	// останавливаем поток чтения клавиатуры	
	}  
        
	    public static void Read()
		{
			while (inCycle)
			{
				if (sPort.IsOpen)
				{
					if(statusUART == "terminal" & sPort.BaudRate == 128000) sPort.BaudRate = 9600;
					try
					{					
						if(sPort.BytesToRead > 0){
							if (statusUART == "loader") queueBytes.Enqueue((byte)sPort.ReadByte());
							else {
								if(readBound == 0) Console.WriteLine("\n Принят(ы) байт(ы):");
								else if(readBound % 16 == 0) Console.WriteLine();
								Console.Write(" ${0:X2}", (byte)sPort.ReadByte());
								readBound++;
							
							}
							//else Console.WriteLine(" Принят байт ${0:X2} ", (byte)sPort.ReadByte());
						}
					} catch (TimeoutException)   {Console.WriteLine(" Read. TimeoutException"); }
				}
				//}
			}
		}// Read()

	    public static void readKey()
		{
			while (inCycle)
			{

					try
					{
						
			while(Dumps.inCycle){
				List<char> lchar = new List<char>();
				lchar.Add(Console.ReadKey(true).KeyChar);
				lchar.Add(Console.ReadKey(true).KeyChar);
				readLine = new String(lchar.ToArray());
				try{
					btBytes[0] = byte.Parse(readLine, System.Globalization.NumberStyles.HexNumber);
					Dumps.sPort.Write(btBytes, 0, 1);
					Console.ForegroundColor = System.ConsoleColor.Green;
					Console.WriteLine("\n Отправлен байт: ${0:X2}", btBytes[0]);
					Console.ForegroundColor = System.ConsoleColor.Gray;	
					Dumps.readBound = 0;
				}
				catch{
					Console.ForegroundColor = System.ConsoleColor.Red;
					Console.WriteLine("\n Необходимо вводить только символы 0...9, A...F.");
					Console.ForegroundColor = System.ConsoleColor.Gray;		
				}


			
			}

					} catch (TimeoutException)   {Console.WriteLine(" Read. TimeoutException"); }
			}
		}// readKey()
	    
	    
	    
	    public static void Dump_To_Console(Dictionary<int, byte> rxAB){
	    	Dictionary<int, byte> AddressByte = new Dictionary<int, byte>(rxAB);
	    	if(err_Msg == " OK"){	//Console.Write(" " + err_Msg);
	    		int i = 0; int predAdr = 0;
	    		foreach( KeyValuePair<int, byte> kvp in AddressByte){
	    			if (i == 0) {	Console.Write(" ${0:X4} ", kvp.Key);
	    							predAdr = kvp.Key;}
	    			else { if( (kvp.Key % 16) == 0 | (kvp.Key - predAdr) > 1) Console.Write("\n ${0:X4} ", kvp.Key); }
					Console.Write("${0:X2} ", kvp.Value);
					i++;
					predAdr = kvp.Key;
				}// foreach
			}else Console.WriteLine("\n" + Dumps.err_Msg);
	    	Console.WriteLine("\n");
	    }
	    
	    public static void Dump_To_Console(SortedDictionary<int, byte> rxAB){
	    	SortedDictionary<int, byte> AddressByte = new SortedDictionary<int, byte>(rxAB);
	    	if(err_Msg == " OK"){	//Console.Write(" " + err_Msg);
	    		int i = 0; int predAdr = 0;
	    		foreach( KeyValuePair<int, byte> kvp in AddressByte){
	    			if (i == 0) {	Console.Write(" ${0:X4} ", kvp.Key);
	    							predAdr = kvp.Key;}
	    			else { if( (kvp.Key % 16) == 0 | (kvp.Key - predAdr) > 1) Console.Write("\n ${0:X4} ", kvp.Key); }
					Console.Write("${0:X2} ", kvp.Value);
					i++;
					predAdr = kvp.Key;
				}// foreach
			}else Console.WriteLine("\n" + Dumps.err_Msg);
	    	Console.WriteLine("\n");
	    }
	    	
	    public static void Dump_To_Console_long(Dictionary<long, byte> rxAB){
	    	Dictionary<long, byte> AddressByte = new Dictionary<long, byte>(rxAB);
	    	if(err_Msg == " OK"){	//Console.Write(" " + err_Msg);
	    		int i = 0; long predAdr = 0;
	    		foreach( KeyValuePair<long, byte> kvp in AddressByte){
	    			if (i == 0) {	Console.Write(" 0x{0:X8} ", kvp.Key);
	    							predAdr = kvp.Key;}
	    			else { if( (kvp.Key % 32) == 0 | (kvp.Key - predAdr) > 1) Console.Write("\n 0x{0:X8} ", kvp.Key); }
					Console.Write("{0:X2} ", kvp.Value);
					i++;
					predAdr = kvp.Key;
				}// foreach
			}else Console.WriteLine("\n" + Dumps.err_Msg);
	    	Console.WriteLine("\n");
	    }	

	    public static void Dump_To_Console_long(SortedDictionary<long, byte> rxAB){
	    	SortedDictionary<long, byte> AddressByte = new SortedDictionary<long, byte>(rxAB);
	    	if(err_Msg == " OK"){	//Console.Write(" " + err_Msg);
	    		int i = 0; long predAdr = 0;
	    		foreach( KeyValuePair<long, byte> kvp in AddressByte){
	    			if (i == 0) {	Console.Write(" 0x{0:X8} ", kvp.Key);
	    							predAdr = kvp.Key;}
	    			else { if( (kvp.Key % 32) == 0 | (kvp.Key - predAdr) > 1) Console.Write("\n 0x{0:X8} ", kvp.Key); }
					Console.Write("{0:X2} ", kvp.Value);
					i++;
					predAdr = kvp.Key;
				}// foreach
			}else Console.WriteLine("\n" + Dumps.err_Msg);
	    	Console.WriteLine("\n");
	    }
	    
 
	    
	    public static string Dumps_To_Console(Dictionary<int, byte[]> rxAB){
	    	Dictionary<int, byte[]> AddressByte = new Dictionary<int, byte[]>(rxAB);
	    	string strng = "";
	    	if(err_Msg == " OK"){	//Console.Write(" " + err_Msg);
	    		int ji = 0;
	    		foreach( KeyValuePair<int, byte[]> kvp in AddressByte){
	    			//int adr = kvp.Key;
	    			int j = 0;
	    			for(int i = 0; i < kvp.Value.Length ;){
	    				if(ji == 0) {Console.Write(" ${0:X4} ", kvp.Key);
	    							strng = strng + String.Format("\n ${0:X4} ", kvp.Key); }
	    				if(ji != 0 & j == 0) {Console.Write("\n ${0:X4} ", kvp.Key);
	    							strng = strng + String.Format("\n ${0:X4} ", kvp.Key); }
	    				else if(ji != 0 & (kvp.Key+i) % 16 == 0) {Console.Write("\n ${0:X4} ", kvp.Key+i);
	    												strng = strng + String.Format("\n ${0:X4} ", kvp.Key+i); }
						Console.Write("${0:X2} ", kvp.Value[i]);
						strng = strng + String.Format("${0:X2} ", kvp.Value[i]);
						i++; j++; ji++;
						
	    			}
	    		}
  		
			} else Console.WriteLine("\n" + err_Msg);
	    	Console.WriteLine("\n");
	    	return strng;
	    }
	    
	    public static string Dumps_To_Console(SortedDictionary<int, byte[]> rxAB){
	    	SortedDictionary<int, byte[]> AddressByte = new SortedDictionary<int, byte[]>(rxAB);
	    	string strng = "";
	    	if(err_Msg == " OK"){	//Console.Write(" " + err_Msg);
	    		int ji = 0;
	    		foreach( KeyValuePair<int, byte[]> kvp in AddressByte){
	    			//int adr = kvp.Key;
	    			int j = 0;
	    			for(int i = 0; i < kvp.Value.Length ;){
	    				if(ji == 0) {Console.Write(" ${0:X4} ", kvp.Key);
	    							strng = strng + String.Format("\n ${0:X4} ", kvp.Key); }
	    				if(ji != 0 & j == 0) {Console.Write("\n ${0:X4} ", kvp.Key);
	    							strng = strng + String.Format("\n ${0:X4} ", kvp.Key); }
	    				else if(ji != 0 & (kvp.Key+i) % 16 == 0) {Console.Write("\n ${0:X4} ", kvp.Key+i);
	    												strng = strng + String.Format("\n ${0:X4} ", kvp.Key+i); }
						Console.Write("${0:X2} ", kvp.Value[i]);
						strng = strng + String.Format("${0:X2} ", kvp.Value[i]);
						i++; j++; ji++;
						
	    			}
	    		}
  		
			} else Console.WriteLine("\n" + err_Msg);
	    	Console.WriteLine("\n");
	    	return strng;
	    }
	    
	    

	    public static string Dumps_To_Console_long(Dictionary<long, byte[]> rxAB){
	    	Dictionary<long, byte[]> AddressByte = new Dictionary<long, byte[]>(rxAB);
	    	string strng = "";
	    	if(err_Msg == " OK"){	//Console.Write(" " + err_Msg);
	    		int ji = 0;
	    		foreach( KeyValuePair<long, byte[]> kvp in AddressByte){
	    			//int adr = kvp.Key;
	    			int j = 0;
	    			for(int i = 0; i < kvp.Value.Length ;){
	    				if(ji == 0) {Console.Write(" 0x{0:X8} ", kvp.Key);
	    							strng = strng + String.Format("\n ${0:X8} ", kvp.Key); }
	    				if(ji != 0 & j == 0) {Console.Write("\n 0x{0:X8} ", kvp.Key);
	    							strng = strng + String.Format("\n 0x{0:X8} ", kvp.Key); }
	    				else if(ji != 0 & (kvp.Key+i) % 32 == 0) {Console.Write("\n 0x{0:X8} ", kvp.Key+i);
	    												strng = strng + String.Format("\n 0x{0:X8} ", kvp.Key+i); }
						Console.Write("{0:X2} ", kvp.Value[i]);
						strng = strng + String.Format("{0:X2} ", kvp.Value[i]);
						i++; j++; ji++;
						
	    			}
	    		}
  		
			} else Console.WriteLine("\n" + err_Msg);
	    	Console.WriteLine("\n");
	    	return strng;
	    }
	    
	    public static string Dumps_To_Console_long(SortedDictionary<long, byte[]> rxAB){
	    	SortedDictionary<long, byte[]> AddressByte = new SortedDictionary<long, byte[]>(rxAB);
	    	string strng = "";
	    	if(err_Msg == " OK"){	//Console.Write(" " + err_Msg);
	    		int ji = 0;
	    		foreach( KeyValuePair<long, byte[]> kvp in AddressByte){
	    			//int adr = kvp.Key;
	    			int j = 0;
	    			for(int i = 0; i < kvp.Value.Length ;){
	    				if(ji == 0) {Console.Write(" 0x{0:X8} ", kvp.Key);
	    							strng = strng + String.Format("\n ${0:X8} ", kvp.Key); }
	    				if(ji != 0 & j == 0) {Console.Write("\n 0x{0:X8} ", kvp.Key);
	    							strng = strng + String.Format("\n 0x{0:X8} ", kvp.Key); }
	    				else if(ji != 0 & (kvp.Key+i) % 32 == 0) {Console.Write("\n 0x{0:X8} ", kvp.Key+i);
	    												strng = strng + String.Format("\n 0x{0:X8} ", kvp.Key+i); }
						Console.Write("{0:X2} ", kvp.Value[i]);
						strng = strng + String.Format("{0:X2} ", kvp.Value[i]);
						i++; j++; ji++;
						
	    			}
	    		}
  		
			} else Console.WriteLine("\n" + err_Msg);
	    	Console.WriteLine("\n");
	    	return strng;
	    }
	    
	    
	    

	public static SortedDictionary<int, byte[]> EEPROM_AdrBlcsSorting(SortedDictionary<int, byte> sort) {
		SortedDictionary<int, byte> sortAdrBt = new SortedDictionary<int, byte>(sort);			// для счетчика foreach
		SortedDictionary<int, byte> fullAdrBt = new SortedDictionary<int, byte>(sortAdrBt);		// для заполнения
		SortedDictionary<int, byte[]> EEPROM_AdrBlcks = new SortedDictionary<int, byte[]>();	// для возврата методом
		//EEPROM_AdrBlcks.Clear();
		int crntADR = 0;		// текущий адрес
	    int baseADR = 0;		// базовый адрес блока или смежных блоков
	    Queue<byte> queue4000 = new Queue<byte>();
	    
		foreach(KeyValuePair<int, byte> kvp in sortAdrBt){
			// sortAdrBt содержит сортированные пары (адрес, байт) из файла прошивки,
			// адреса расположены в порядке возрастания, но не гарантируется непрерывность последовательности адреса
			// последовательность адресов может быть не выровнена по начальному и/или конечному адресу блока
			if ( (kvp.Key % 64) != 0 ){	//
				if(fullAdrBt.ContainsKey(kvp.Key - 1) == false){ // если предыдущий адрес отсутствует
					crntADR = kvp.Key - (kvp.Key % 64);
					for(;crntADR < kvp.Key; crntADR++){
						if(fullAdrBt.ContainsKey(crntADR) == false)
						fullAdrBt.Add(crntADR, 0x00); // 
						//else Console.WriteLine(" 1 Повторная запись в адрес 0x{0:X4}", crntADR);	
				   }
				}
			}	
			if ( (kvp.Key % 64) != 63){	//
				if(fullAdrBt.ContainsKey(kvp.Key + 1) == false){ // если следующий адрес отсутствует
					crntADR = kvp.Key;
					for(; (crntADR % 64) <= 62; ){
						crntADR++;
						if(fullAdrBt.ContainsKey(crntADR) == false)
						fullAdrBt.Add(crntADR, 0x00); // 
						//else Console.WriteLine(" 2 Повторная запись в адрес 0x{0:X4}", crntADR);
					}
				}	
			}
		}
		// fullAdrBt содержит сортированные пары (адрес, байт) из файла прошивки, 
	    // дополненные нулями до полного размера каждого используемого блока
	    // могут использоваться все или не все блоки
	    // используемые блоки могут граничить друг с другом
	    // или разделены неиспользуемыми блоками
	    // в следующем цикле в EEPROM_AdrBlcks должны быть сформированы пары <адрес, массив>
	    // где массив может содержать один или более смежных блоков
	    crntADR = 0;
	    baseADR = 0;
	    queue4000.Clear();
		foreach(KeyValuePair<int, byte> kvp in fullAdrBt){
			if(crntADR == 0) baseADR = crntADR = kvp.Key;
			if(fullAdrBt.ContainsKey(kvp.Key + 1)) 
				{	queue4000.Enqueue(kvp.Value);
					crntADR = kvp.Key + 1; }
			if(fullAdrBt.ContainsKey(kvp.Key + 1) == false){
				queue4000.Enqueue(kvp.Value);
				EEPROM_AdrBlcks.Add(baseADR, queue4000.ToArray());
				queue4000.Clear();
				crntADR = 0;
			}	
		}
		return EEPROM_AdrBlcks;
	}//SortedDictionary<int, byte[]> EEPROM_AdrBlcsSorting(SortedDictionary<int, byte> sort)
	    
	    
	    
	    


	public static SortedDictionary<int, byte> OPTION_AdrBlcsSorting(byte rvz, SortedDictionary<int, byte> sort) {
		SortedDictionary<int, byte> fullAdrBt = new SortedDictionary<int, byte>();		// для заполнения
		SortedDictionary<int, byte> sortAdrBt = new SortedDictionary<int, byte>(sort);		
		foreach(KeyValuePair<int, byte> kvp in sortAdrBt){
			if (kvp.Key < 0x4800 | kvp.Key > 0x483F ) err_Msg = "Имеются адреса выходящие за диапазон 0x4800...0x483F ";
		}

		if(rvz == 0x14){
			for(int i = 0 ; i < 64; i++){
				if ( sortAdrBt.ContainsKey(i + 0x4800)) fullAdrBt.Add(i + 0x4800, sortAdrBt[i + 0x4800]);
				else fullAdrBt.Add(i + 0x4800, rev14_4800_OPTION[i]);
			}
	    }
		if(rvz == 0x25){
			for(int i = 0 ; i < 64; i++){
				if ( sortAdrBt.ContainsKey(i + 0x4800)) fullAdrBt.Add(i + 0x4800, sortAdrBt[i + 0x4800]);
				else fullAdrBt.Add(i + 0x4800, rev25_4800_OPTION[i]);
			}
	    }

		return fullAdrBt;
	}//SortedDictionary<int, byte> OPTION_AdrBlcsSorting(byte rvz, SortedDictionary<int, byte> sort)
	    
	    
	    
	    
	    
		
	
	public static SortedDictionary<int, byte[]> FLASH_AdrBlcsSorting_v14(SortedDictionary<int, byte> sort) {
		SortedDictionary<int, byte> sortAdrBt = new SortedDictionary<int, byte>(sort);			// для счетчика foreach
		SortedDictionary<int, byte[]> FLASH_AdrBlcks = new SortedDictionary<int, byte[]>();	// для возврата методом
		int crntADR = 0;		// текущий адрес
	    int baseADR = 0;		// базовый адрес блока или смежных блоков
	    Queue<byte> queue8000 = new Queue<byte>();
	    
	    err_Msg = " OK";
	    if(	sortAdrBt.ContainsKey(0x8000) | sortAdrBt.ContainsKey(0x8001) | sortAdrBt.ContainsKey(0x8002) |
	      	sortAdrBt.ContainsKey(0x8003) | sortAdrBt.ContainsKey(0x9FF0) | sortAdrBt.ContainsKey(0x9FF1) |
	      	sortAdrBt.ContainsKey(0x9FF2) | sortAdrBt.ContainsKey(0x9FF3) | sortAdrBt.ContainsKey(0x9FF4) |
	      	sortAdrBt.ContainsKey(0x9FF5) | sortAdrBt.ContainsKey(0x9FF6) | sortAdrBt.ContainsKey(0x9FF7) |
	      	sortAdrBt.ContainsKey(0x9FF8) | sortAdrBt.ContainsKey(0x9FF9) | sortAdrBt.ContainsKey(0x9FFA) |
	      	sortAdrBt.ContainsKey(0x9FFB) | sortAdrBt.ContainsKey(0x9FFC) | sortAdrBt.ContainsKey(0x9FFD) )
	      	{err_Msg = " Имеются адреса кода копировщика"; return FLASH_AdrBlcks;}
	    crntADR = 0x8000;// дополняем кодом начального копировщика boot_FLASH
	    for(int i=0; i < boot_FLASH_8000_v14.Length; i++, crntADR++){
	    	sortAdrBt.Add(crntADR, boot_FLASH_8000_v14[i]);
	    }
	    crntADR = 0x9FF0;// дополняем кодом начального копировщика boot_FLASH
	    for(int i=0; i < boot_FLASH_9FF0.Length; i++, crntADR++){
	    	sortAdrBt.Add(crntADR, boot_FLASH_9FF0[i]);
	    }
	    SortedDictionary<int, byte> fullAdrBt = new SortedDictionary<int, byte>(sortAdrBt);		// для заполнения
	    
		foreach(KeyValuePair<int, byte> kvp in sortAdrBt){
			// sortAdrBt содержит сортированные пары (адрес, байт) из файла прошивки,
			// адреса расположены в порядке возрастания, но не гарантируется непрерывность последовательности адресов
			// последовательность адресов может быть не выровнена по начальному и/или конечному адресу блока
			if ( (kvp.Key % 64) != 0 ){	//
				if(fullAdrBt.ContainsKey(kvp.Key - 1) == false){ // если предыдущий адрес отсутствует
					crntADR = kvp.Key - (kvp.Key % 64);
					for(;crntADR < kvp.Key; crntADR++){
						if(fullAdrBt.ContainsKey(crntADR) == false)
						fullAdrBt.Add(crntADR, 0x00); // 
						//else Console.WriteLine(" 1 Повторная запись в адрес 0x{0:X4}", crntADR);	
				   }
				}
			}	
			if ( (kvp.Key % 64) != 63){	//
				if(fullAdrBt.ContainsKey(kvp.Key + 1) == false){ // если следующий адрес отсутствует
					crntADR = kvp.Key;
					for(; (crntADR % 64) <= 62; ){
						crntADR++;
						if(fullAdrBt.ContainsKey(crntADR) == false)
						fullAdrBt.Add(crntADR, 0x00); // 
						//else Console.WriteLine(" 2 Повторная запись в адрес 0x{0:X4}", crntADR);
					}
				}	
			}
		}
		// fullAdrBt содержит сортированные пары (адрес, байт) из файла прошивки, 
	    // дополненные нулями до полного размера каждого используемого блока
	    // могут использоваться все или не все блоки
	    // используемые блоки могут граничить друг с другом
	    // или разделены неиспользуемыми блоками
	    // в следующем цикле во FLASH_AdrBlcks должны быть сформированы пары <адрес, массив>
	    // где массив может содержать один или более смежных блоков
	    crntADR = 0;
	    baseADR = 0;
	    queue8000.Clear();
		foreach(KeyValuePair<int, byte> kvp in fullAdrBt){
			if(crntADR == 0) baseADR = crntADR = kvp.Key;
			if(fullAdrBt.ContainsKey(kvp.Key + 1)) 
				{	queue8000.Enqueue(kvp.Value);
					crntADR = kvp.Key + 1; }
			if(fullAdrBt.ContainsKey(kvp.Key + 1) == false){
				queue8000.Enqueue(kvp.Value);
				FLASH_AdrBlcks.Add(baseADR, queue8000.ToArray());
				queue8000.Clear();
				crntADR = 0;
			}	
		}
		return FLASH_AdrBlcks;
	}//SortedDictionary<int, byte[]> FLASH_AdrBlcsSorting_v14(SortedDictionary<int, byte> sort)
	
	    
	    
	    
	    
	    

	public static SortedDictionary<int, byte[]> FLASH_AdrBlcsSorting_v25(SortedDictionary<int, byte> sort) {
		SortedDictionary<int, byte> sortAdrBt = new SortedDictionary<int, byte>(sort);			// для счетчика foreach
		SortedDictionary<int, byte[]> FLASH_AdrBlcks = new SortedDictionary<int, byte[]>();	// для возврата методом
		int crntADR = 0;		// текущий адрес
	    int baseADR = 0;		// базовый адрес блока или смежных блоков
	    Queue<byte> queue8000 = new Queue<byte>();
	    
	    err_Msg = " OK";
	    if(	sortAdrBt.ContainsKey(0x8000) | sortAdrBt.ContainsKey(0x8001) | sortAdrBt.ContainsKey(0x8002) |
	      	sortAdrBt.ContainsKey(0x8003) | sortAdrBt.ContainsKey(0x9FF2) | sortAdrBt.ContainsKey(0x9FF3) |
	      	sortAdrBt.ContainsKey(0x9FF4) | sortAdrBt.ContainsKey(0x9FF5) | sortAdrBt.ContainsKey(0x9FF6) |
	      	sortAdrBt.ContainsKey(0x9FF7) | sortAdrBt.ContainsKey(0x9FF8) | sortAdrBt.ContainsKey(0x9FF9) |
	      	sortAdrBt.ContainsKey(0x9FFA) | sortAdrBt.ContainsKey(0x9FFB) | sortAdrBt.ContainsKey(0x9FFC) |
	      	sortAdrBt.ContainsKey(0x9FFD) | sortAdrBt.ContainsKey(0x9FFE) | sortAdrBt.ContainsKey(0x9FFF) )
	      	{err_Msg = " Имеются адреса кода копировщика"; return FLASH_AdrBlcks;}
	    crntADR = 0x8000;// дополняем кодом начального копировщика boot_FLASH
	    for(int i=0; i < boot_FLASH_8000_v25.Length; i++, crntADR++){
	    	sortAdrBt.Add(crntADR, boot_FLASH_8000_v25[i]);
	    }
	    crntADR = 0x9FF2;// дополняем кодом начального копировщика boot_FLASH
	    for(int i=0; i < boot_FLASH_9FF2.Length; i++, crntADR++){
	    	sortAdrBt.Add(crntADR, boot_FLASH_9FF0[i]);
	    }
	    SortedDictionary<int, byte> fullAdrBt = new SortedDictionary<int, byte>(sortAdrBt);		// для заполнения
	    
		foreach(KeyValuePair<int, byte> kvp in sortAdrBt){
			// sortAdrBt содержит сортированные пары (адрес, байт) из файла прошивки,
			// адреса расположены в порядке возрастания, но не гарантируется непрерывность последовательности адресов
			// последовательность адресов может быть не выровнена по начальному и/или конечному адресу блока
			if ( (kvp.Key % 64) != 0 ){	//
				if(fullAdrBt.ContainsKey(kvp.Key - 1) == false){ // если предыдущий адрес отсутствует
					crntADR = kvp.Key - (kvp.Key % 64);
					for(;crntADR < kvp.Key; crntADR++){
						if(fullAdrBt.ContainsKey(crntADR) == false)
						fullAdrBt.Add(crntADR, 0x00); // 
						//else Console.WriteLine(" 1 Повторная запись в адрес 0x{0:X4}", crntADR);	
				   }
				}
			}	
			if ( (kvp.Key % 64) != 63){	//
				if(fullAdrBt.ContainsKey(kvp.Key + 1) == false){ // если следующий адрес отсутствует
					crntADR = kvp.Key;
					for(; (crntADR % 64) <= 62; ){
						crntADR++;
						if(fullAdrBt.ContainsKey(crntADR) == false)
						fullAdrBt.Add(crntADR, 0x00); // 
						//else Console.WriteLine(" 2 Повторная запись в адрес 0x{0:X4}", crntADR);
					}
				}	
			}
		}
		// fullAdrBt содержит сортированные пары (адрес, байт) из файла прошивки, 
	    // дополненные нулями до полного размера каждого используемого блока
	    // могут использоваться все или не все блоки
	    // используемые блоки могут граничить друг с другом
	    // или разделены неиспользуемыми блоками
	    // в следующем цикле во FLASH_AdrBlcks должны быть сформированы пары <адрес, массив>
	    // где массив может содержать один или более смежных блоков
	    crntADR = 0;
	    baseADR = 0;
	    queue8000.Clear();
		foreach(KeyValuePair<int, byte> kvp in fullAdrBt){
			if(crntADR == 0) baseADR = crntADR = kvp.Key;
			if(fullAdrBt.ContainsKey(kvp.Key + 1)) 
				{	queue8000.Enqueue(kvp.Value);
					crntADR = kvp.Key + 1; }
			if(fullAdrBt.ContainsKey(kvp.Key + 1) == false){
				queue8000.Enqueue(kvp.Value);
				FLASH_AdrBlcks.Add(baseADR, queue8000.ToArray());
				queue8000.Clear();
				crntADR = 0;
			}	
		}
		return FLASH_AdrBlcks;
	}//SortedDictionary<int, byte[]> FLASH_AdrBlcsSorting_v25(SortedDictionary<int, byte> sort)
	
	   
	    
	    
	    
	    
	    
   public readonly static byte[] Read_128000v14_25 = { 0x00, 
0x00, 0xFE, 0x03, 0xCC, 0x72, 0x94, 0xFF, 0x03, 0xAE, 0x31, 0x52, 0xC6, 0xFB, 0x30, 0x52, 0x0B, 
0x72, 0xFF, 0x03, 0x31, 0x52, 0x55, 0xFB, 0x30, 0x52, 0x0B, 0x72, 0xFE, 0x03, 0x31, 0x52, 0x55, 
0xFB, 0x30, 0x52, 0x0B, 0x72, 0x95, 0x90, 0x31, 0x52, 0xC6, 0xFB, 0x30, 0x52, 0x0B, 0x72, 0x95, 
0x90, 0x31, 0x52, 0xC6, 0xFB, 0x30, 0x52, 0x0B, 0x72, 0xFB, 0x30, 0x52, 0x0F, 0x72, 0x31, 0x52, 
0xC7, 0xA5, 0x20, 0xF7, 0x30, 0x52, 0x0F, 0x72, 0x31, 0x52, 0xF1, 0x35, 0x0B, 0x27, 0xF5, 0xA1, 
0xB4, 0x20, 0x08, 0x50, 0x1B, 0x72, 0x07, 0x50, 0x1B, 0x72, 0xF2, 0x26, 0x5A, 0x90, 0x5C, 0xFB, 
0x30, 0x52, 0x0F, 0x72, 0x31, 0x52, 0xC7, 0xF6, 0x08, 0x50, 0x1A, 0x72, 0x07, 0x50, 0x1A, 0x72, 
0x97, 0x90, 0x31, 0x52, 0xC6, 0x95, 0x90, 0x00, 0xA6, 0xFB, 0x30, 0x52, 0x0B, 0x72, 0x97, 0x31, 
0x52, 0xC6, 0xFB, 0x30, 0x52, 0x0B, 0x72, 0xFB, 0x30, 0x52, 0x0F, 0x72, 0x31, 0x52, 0xC7, 0x95, 
0x40, 0x22, 0xEF, 0xA1, 0x31, 0x52, 0xC6, 0xFB, 0x30, 0x52, 0x0B, 0x72, 0x35, 0x52, 0x0C, 0x35, 
0x32, 0x52, 0x01, 0x35, 0x33, 0x52, 0x00, 0x35 };
	    
   public readonly static byte[] ReadBlocks_128000v14_25 = { 0x00, 
0x00, 0xFE, 0x03, 0xCC, 0x72, 0x94, 0xFF, 0x03, 0xAE, 0x31, 0x52, 0xC6, 0xFB, 0x30, 0x52, 0x0B, 
0x72, 0xFF, 0x03, 0x31, 0x52, 0x55, 0xFB, 0x30, 0x52, 0x0B, 0x72, 0xFE, 0x03, 0x31, 0x52, 0x55, 
0xFB, 0x30, 0x52, 0x0B, 0x72, 0x97, 0x90, 0x31, 0x52, 0xC6, 0xFB, 0x30, 0x52, 0x0B, 0x72, 0x95, 
0x90, 0x31, 0x52, 0xC6, 0xFB, 0x30, 0x52, 0x0B, 0x72, 0xFB, 0x30, 0x52, 0x0F, 0x72, 0x31, 0x52, 
0xC7, 0x9F, 0x20, 0xFB, 0x30, 0x52, 0x0F, 0x72, 0x31, 0x52, 0xF1, 0x35, 0x0B, 0x27, 0xF5, 0xA1, 
0xAE, 0x20, 0xD8, 0x26, 0xFF, 0x03, 0x5A, 0x72, 0x08, 0x50, 0x1B, 0x72, 0x07, 0x50, 0x1B, 0x72, 
0xF2, 0x26, 0x5A, 0x90, 0x5C, 0xFB, 0x30, 0x52, 0x0F, 0x72, 0x31, 0x52, 0xC7, 0xF6, 0x08, 0x50, 
0x1A, 0x72, 0x07, 0x50, 0x1A, 0x72, 0x40, 0x00, 0xAE, 0x90, 0xFF, 0x03, 0x31, 0x52, 0x55, 0xFB, 
0x30, 0x52, 0x0B, 0x72, 0x97, 0x31, 0x52, 0xC6, 0xFB, 0x30, 0x52, 0x0B, 0x72, 0xFB, 0x30, 0x52, 
0x0F, 0x72, 0x31, 0x52, 0xC7, 0x95, 0x46, 0x22, 0xEF, 0xA1, 0x31, 0x52, 0xC6, 0xFB, 0x30, 0x52, 
0x0B, 0x72, 0x35, 0x52, 0x0C, 0x35, 0x32, 0x52, 0x01, 0x35, 0x33, 0x52, 0x00, 0x35 };
	    
	    
   public readonly static byte[] Write_RAM_128000v14_25 = { 0x00, 
0x00, 0xFE, 0x03, 0xCC, 0x72, 0x94, 0xFF, 0x03, 0xAE, 0x31, 0x52, 0xC6, 0xFB, 0x30, 0x52, 0x0B, 
0x72, 0xFF, 0x03, 0x31, 0x52, 0x55, 0xFB, 0x30, 0x52, 0x0B, 0x72, 0xFE, 0x03, 0x31, 0x52, 0x55, 
0xFB, 0x30, 0x52, 0x0B, 0x72, 0x97, 0x90, 0x31, 0x52, 0xC6, 0xFB, 0x30, 0x52, 0x0B, 0x72, 0x95, 
0x90, 0x31, 0x52, 0xC6, 0xFB, 0x30, 0x52, 0x0B, 0x72, 0xFB, 0x30, 0x52, 0x0F, 0x72, 0x31, 0x52, 
0xC7, 0x9C, 0x20, 0xFB, 0x30, 0x52, 0x0F, 0x72, 0x31, 0x52, 0xF1, 0x35, 0x0B, 0x27, 0xF5, 0xA1, 
0xAB, 0x20, 0x08, 0x50, 0x1B, 0x72, 0x07, 0x50, 0x1B, 0x72, 0xFB, 0x30, 0x52, 0x0F, 0x72, 0x31, 
0x52, 0xFA, 0x35, 0xF2, 0x26, 0x5A, 0x90, 0x5C, 0xF7, 0x31, 0x52, 0xC6, 0xFB, 0x30, 0x52, 0x0B, 
0x72, 0x08, 0x50, 0x1A, 0x72, 0x07, 0x50, 0x1A, 0x72, 0x97, 0x90, 0x31, 0x52, 0xC6, 0x00, 0x00, 
0xAE, 0x90, 0xFB, 0x30, 0x52, 0x0B, 0x72, 0x97, 0x31, 0x52, 0xC6, 0xFB, 0x30, 0x52, 0x0B, 0x72, 
0xFB, 0x30, 0x52, 0x0F, 0x72, 0x31, 0x52, 0xC7, 0x95, 0x49, 0x22, 0xEF, 0xA1, 0x31, 0x52, 0xC6, 
0xFB, 0x30, 0x52, 0x0B, 0x72, 0x35, 0x52, 0x0C, 0x35, 0x32, 0x52, 0x01, 0x35, 0x33, 0x52, 0x00, 
0x35 };
   public readonly static byte[] Copy_RAM_128000v14_25 = { 0x00, 
0x00, 0xFE, 0x03, 0xCC, 0x72, 0x94, 0xFF, 0x03, 0xAE, 0x31, 0x52, 0xC6, 0xFB, 0x30, 0x52, 0x0B, 
0x72, 0xFF, 0x03, 0x31, 0x52, 0x55, 0xFB, 0x30, 0x52, 0x0B, 0x72, 0xFE, 0x03, 0x31, 0x52, 0x55, 
0xFB, 0x30, 0x52, 0x0B, 0x72, 0x97, 0x90, 0x31, 0x52, 0xC6, 0xFB, 0x30, 0x52, 0x0B, 0x72, 0x95, 
0x90, 0x31, 0x52, 0xC6, 0xFB, 0x30, 0x52, 0x0B, 0x72, 0xFB, 0x30, 0x52, 0x0F, 0x72, 0x31, 0x52, 
0xC7, 0x8E, 0x20, 0xFB, 0x30, 0x52, 0x0F, 0x72, 0x31, 0x52, 0xF1, 0x35, 0x0B, 0x27, 0xF5, 0xA1, 
0x9D, 0x20, 0x08, 0x50, 0x1B, 0x72, 0x07, 0x50, 0x1B, 0x72, 0xFB, 0x30, 0x52, 0x0F, 0x72, 0x31, 
0x52, 0xFA, 0x35, 0xF4, 0x26, 0xFF, 0x03, 0x5A, 0x72, 0x5C, 0x5C, 0x90, 0xF7, 0xF6, 0x90, 0x08, 
0x50, 0x1A, 0x72, 0x07, 0x50, 0x1A, 0x72, 0xFF, 0x03, 0x31, 0x52, 0x55, 0xFB, 0x30, 0x52, 0x0B, 
0x72, 0x97, 0x90, 0x31, 0x52, 0xC6, 0xFB, 0x30, 0x52, 0x0B, 0x72, 0x95, 0x90, 0x31, 0x52, 0xC6, 
0xFB, 0x30, 0x52, 0x0B, 0x72, 0x97, 0x31, 0x52, 0xC6, 0xFB, 0x30, 0x52, 0x0B, 0x72, 0xFB, 0x30, 
0x52, 0x0F, 0x72, 0x31, 0x52, 0xC7, 0x95, 0x57, 0x22, 0xEF, 0xA1, 0x31, 0x52, 0xC6, 0xFB, 0x30, 
0x52, 0x0B, 0x72, 0x35, 0x52, 0x0C, 0x35, 0x32, 0x52, 0x01, 0x35, 0x33, 0x52, 0x00, 0x35 };
	    
   public readonly static byte[] WriteByte_EEPROM_128000v14_25 = { 0x00, 
0x00, 0xFE, 0x03, 0xCC, 0x72, 0x94, 0xFF, 0x03, 0xAE, 0x31, 0x52, 0xC6, 0xFB, 0x30, 0x52, 0x0B, 
0x72, 0xFF, 0x03, 0x31, 0x52, 0x55, 0xFB, 0x30, 0x52, 0x0B, 0x72, 0xFE, 0x03, 0x31, 0x52, 0x55, 
0xFB, 0x30, 0x52, 0x0B, 0x72, 0x97, 0x90, 0x31, 0x52, 0xC6, 0xFB, 0x30, 0x52, 0x0B, 0x72, 0x95, 
0x90, 0x31, 0x52, 0xC6, 0xFB, 0x30, 0x52, 0x0B, 0x72, 0xFB, 0x30, 0x52, 0x0F, 0x72, 0x31, 0x52, 
0xC7, 0xA7, 0x20, 0xFB, 0x30, 0x52, 0x0F, 0x72, 0x31, 0x52, 0xF1, 0x35, 0x0B, 0x27, 0xF5, 0xA1, 
0xB6, 0x20, 0x08, 0x50, 0x1B, 0x72, 0x07, 0x50, 0x1B, 0x72, 0xFB, 0x30, 0x52, 0x0F, 0x72, 0x31, 
0x52, 0xFA, 0x35, 0xF7, 0x31, 0x52, 0xC6, 0xFB, 0x30, 0x52, 0x0B, 0x72, 0x64, 0x50, 0x56, 0x35, 
0x64, 0x50, 0xAE, 0x35, 0x08, 0x50, 0x1A, 0x72, 0x07, 0x50, 0x1A, 0x72, 0x97, 0x31, 0x52, 0xC6, 
0xFB, 0x30, 0x52, 0x0B, 0x72, 0xFB, 0x30, 0x52, 0x0F, 0x72, 0x31, 0x52, 0xC7, 0x95, 0x3E, 0x22, 
0xEF, 0xA1, 0x31, 0x52, 0xC6, 0xFB, 0x30, 0x52, 0x0B, 0x72, 0x35, 0x52, 0x0C, 0x35, 0x32, 0x52, 
0x01, 0x35, 0x33, 0x52, 0x00, 0x35 };
	    public readonly static byte[] WriteWord_EEPROM_128000v14_25 = { 0x00,
0x00, 0xFE, 0x03, 0xCC, 0x72, 0x94, 0xFF, 0x03, 0xAE, 0x31, 0x52, 0xC6, 0xFB, 0x30, 0x52, 0x0B, 
0x72, 0xFF, 0x03, 0x31, 0x52, 0x55, 0xFB, 0x30, 0x52, 0x0B, 0x72, 0xFE, 0x03, 0x31, 0x52, 0x55, 
0xFB, 0x30, 0x52, 0x0B, 0x72, 0x97, 0x90, 0x31, 0x52, 0xC6, 0xFB, 0x30, 0x52, 0x0B, 0x72, 0x95, 
0x90, 0x31, 0x52, 0xC6, 0xFB, 0x30, 0x52, 0x0B, 0x72, 0xFB, 0x30, 0x52, 0x0F, 0x72, 0x31, 0x52, 
0xC7, 0x8C, 0x20, 0xFB, 0x30, 0x52, 0x0F, 0x72, 0x31, 0x52, 0xF1, 0x35, 0x0B, 0x27, 0xF5, 0xA1, 
0x9B, 0x20, 0x08, 0x50, 0x1B, 0x72, 0x07, 0x50, 0x1B, 0x72, 0xFB, 0x30, 0x52, 0x0F, 0x72, 0x31, 
0x52, 0xFA, 0x35, 0xF2, 0x26, 0x5A, 0x90, 0x5C, 0xF7, 0x31, 0x52, 0xC6, 0xFB, 0x30, 0x52, 0x0B, 
0x72, 0x5C, 0x50, 0xBF, 0x35, 0x5B, 0x50, 0x40, 0x35, 0x64, 0x50, 0x56, 0x35, 0x64, 0x50, 0xAE, 
0x35, 0x08, 0x50, 0x1A, 0x72, 0x07, 0x50, 0x1A, 0x72, 0x97, 0x90, 0x31, 0x52, 0xC6, 0x00, 0x00, 
0xAE, 0x90, 0xFB, 0x30, 0x52, 0x0B, 0x72, 0x97, 0x31, 0x52, 0xC6, 0xFB, 0x30, 0x52, 0x0B, 0x72, 
0xFB, 0x30, 0x52, 0x0F, 0x72, 0x31, 0x52, 0xC7, 0x95, 0x59, 0x22, 0xEF, 0xA1, 0x31, 0x52, 0xC6, 
0xFB, 0x30, 0x52, 0x0B, 0x72, 0x35, 0x52, 0x0C, 0x35, 0x32, 0x52, 0x01, 0x35, 0x33, 0x52, 0x00, 
0x35 };
   public readonly static byte[] WriteBlock_EEPROM_128000v1425 = { 0x00, //WriteBlock_EEPROM_128000v1425_20180708
0x00, 0xFE, 0x03, 0xCC, 0x72, 0x94, 0xFF, 0x03, 0xAE, 0x31, 0x52, 0xC6, 0xFB, 0x30, 0x52, 0x0B, 
0x72, 0xFF, 0x03, 0x31, 0x52, 0x55, 0xFB, 0x30, 0x52, 0x0B, 0x72, 0xFE, 0x03, 0x31, 0x52, 0x55, 
0xFB, 0x30, 0x52, 0x0B, 0x72, 0x97, 0x90, 0x31, 0x52, 0xC6, 0xFB, 0x30, 0x52, 0x0B, 0x72, 0x95, 
0x90, 0x31, 0x52, 0xC6, 0xFB, 0x30, 0x52, 0x0B, 0x72, 0xFB, 0x30, 0x52, 0x0F, 0x72, 0x31, 0x52, 
0xC7, 0x0C, 0x00, 0xCC, 0xFB, 0x30, 0x52, 0x0F, 0x72, 0x31, 0x52, 0xF1, 0x35, 0x0C, 0x27, 0xF5, 
0xA1, 0x82, 0x20, 0x08, 0x50, 0x1B, 0x72, 0x07, 0x50, 0x1B, 0x72, 0xFB, 0x30, 0x52, 0x0F, 0x72, 
0x31, 0x52, 0xFA, 0x35, 0xF4, 0x26, 0xFE, 0x03, 0x5A, 0x72, 0x5C, 0x90, 0x5C, 0xF7, 0xF6, 0x90, 
0x00, 0x01, 0xAE, 0x90, 0x5C, 0x50, 0xFE, 0x35, 0x5B, 0x50, 0x01, 0x35, 0x64, 0x50, 0x56, 0x35, 
0x64, 0x50, 0xAE, 0x35, 0x08, 0x50, 0x1A, 0x72, 0x07, 0x50, 0x1A, 0x72, 0xEE, 0x26, 0xFF, 0x03, 
0x5A, 0x72, 0x5C, 0x90, 0xF7, 0x90, 0x31, 0x52, 0xC6, 0xFB, 0x30, 0x52, 0x0B, 0x72, 0x00, 0x01, 
0xAE, 0x90, 0xFE, 0x03, 0xFF, 0x03, 0x55, 0xFF, 0x03, 0x31, 0x52, 0x55, 0xFB, 0x30, 0x52, 0x0B, 
0x72, 0x97, 0x31, 0x52, 0xC6, 0xFB, 0x30, 0x52, 0x0B, 0x72, 0xFB, 0x30, 0x52, 0x0F, 0x72, 0x31, 
0x52, 0xC7, 0x95, 0x72, 0x22, 0xEF, 0xA1, 0x31, 0x52, 0xC6, 0xFB, 0x30, 0x52, 0x0B, 0x72, 0x35, 
0x52, 0x0C, 0x35, 0x32, 0x52, 0x01, 0x35, 0x33, 0x52, 0x00, 0x35 };

   public readonly static byte[] WriteBlocks_EEPROM_128000v14_25 = { 0x00, 
0x00, 0xFE, 0x03, 0xCC, 0x72, 0x94, 0xFF, 0x03, 0xAE, 0x31, 0x52, 0xC6, 0xFB, 0x30, 0x52, 0x0B, 
0x72, 0xFF, 0x03, 0x31, 0x52, 0x55, 0xFB, 0x30, 0x52, 0x0B, 0x72, 0xFE, 0x03, 0x31, 0x52, 0x55, 
0xFB, 0x30, 0x52, 0x0B, 0x72, 0x97, 0x90, 0x31, 0x52, 0xC6, 0xFB, 0x30, 0x52, 0x0B, 0x72, 0x95, 
0x90, 0x31, 0x52, 0xC6, 0xFB, 0x30, 0x52, 0x0B, 0x72, 0xFB, 0x30, 0x52, 0x0F, 0x72, 0x31, 0x52, 
0xC7, 0x86, 0x20, 0xFB, 0x30, 0x52, 0x0F, 0x72, 0x31, 0x52, 0xF1, 0x35, 0x0B, 0x27, 0xF5, 0xA1, 
0x95, 0x20, 0xBF, 0x26, 0xFF, 0x03, 0x5A, 0x72, 0x08, 0x50, 0x1B, 0x72, 0x07, 0x50, 0x1B, 0x72, 
0xFB, 0x30, 0x52, 0x0F, 0x72, 0x31, 0x52, 0xFA, 0x35, 0xF2, 0x26, 0x5A, 0x90, 0x5C, 0xF7, 0x31, 
0x52, 0xC6, 0xFB, 0x30, 0x52, 0x0B, 0x72, 0x08, 0x50, 0x1A, 0x72, 0x07, 0x50, 0x1A, 0x72, 0x5C, 
0x50, 0xFE, 0x35, 0x5B, 0x50, 0x01, 0x35, 0x64, 0x50, 0x56, 0x35, 0x64, 0x50, 0xAE, 0x35, 0x40, 
0x00, 0xAE, 0x90, 0xFF, 0x03, 0x31, 0x52, 0x55, 0xFB, 0x30, 0x52, 0x0B, 0x72, 0x97, 0x31, 0x52, 
0xC6, 0xFB, 0x30, 0x52, 0x0B, 0x72, 0xFB, 0x30, 0x52, 0x0F, 0x72, 0x31, 0x52, 0xC7, 0x95, 0x5F, 
0x22, 0xEF, 0xA1, 0x31, 0x52, 0xC6, 0xFB, 0x30, 0x52, 0x0B, 0x72, 0x35, 0x52, 0x0C, 0x35, 0x32, 
0x52, 0x01, 0x35, 0x33, 0x52, 0x00, 0x35 };
	    
	    
   public readonly static byte[] WriteBytes_OPTION_128000v1425 = { 0x00, // WriteBytes_OPTION_128000v1425_20180708
0x00, 0xFE, 0x03, 0xCC, 0x72, 0x94, 0xFF, 0x03, 0xAE, 0x31, 0x52, 0xC6, 0xFB, 0x30, 0x52, 0x0B, 
0x72, 0xFF, 0x03, 0x31, 0x52, 0x55, 0xFB, 0x30, 0x52, 0x0B, 0x72, 0xFE, 0x03, 0x31, 0x52, 0x55, 
0xFB, 0x30, 0x52, 0x0B, 0x72, 0x97, 0x90, 0x31, 0x52, 0xC6, 0xFB, 0x30, 0x52, 0x0B, 0x72, 0x95, 
0x90, 0x31, 0x52, 0xC6, 0xFB, 0x30, 0x52, 0x0B, 0x72, 0xFB, 0x30, 0x52, 0x0F, 0x72, 0x31, 0x52, 
0xC7, 0x0C, 0x00, 0xCC, 0xFB, 0x30, 0x52, 0x0F, 0x72, 0x31, 0x52, 0xF1, 0x35, 0x0C, 0x27, 0xF5, 
0xA1, 0x82, 0x20, 0x08, 0x50, 0x1B, 0x72, 0x07, 0x50, 0x1B, 0x72, 0xFB, 0x30, 0x52, 0x0F, 0x72, 
0x31, 0x52, 0xFA, 0x35, 0xF4, 0x26, 0xFE, 0x03, 0x5A, 0x72, 0x5C, 0x90, 0x5C, 0xF7, 0xF6, 0x90, 
0x00, 0x01, 0xAE, 0x90, 0x5C, 0x50, 0x7F, 0x35, 0x5B, 0x50, 0x80, 0x35, 0x64, 0x50, 0x56, 0x35, 
0x64, 0x50, 0xAE, 0x35, 0x08, 0x50, 0x1A, 0x72, 0x07, 0x50, 0x1A, 0x72, 0xEE, 0x26, 0xFF, 0x03, 
0x5A, 0x72, 0x5C, 0x90, 0xF7, 0x90, 0x31, 0x52, 0xC6, 0xFB, 0x30, 0x52, 0x0B, 0x72, 0x00, 0x01, 
0xAE, 0x90, 0xFE, 0x03, 0xFF, 0x03, 0x55, 0xFF, 0x03, 0x31, 0x52, 0x55, 0xFB, 0x30, 0x52, 0x0B, 
0x72, 0x97, 0x31, 0x52, 0xC6, 0xFB, 0x30, 0x52, 0x0B, 0x72, 0xFB, 0x30, 0x52, 0x0F, 0x72, 0x31, 
0x52, 0xC7, 0x95, 0x72, 0x22, 0xEF, 0xA1, 0x31, 0x52, 0xC6, 0xFB, 0x30, 0x52, 0x0B, 0x72, 0x35, 
0x52, 0x0C, 0x35, 0x32, 0x52, 0x01, 0x35, 0x33, 0x52, 0x00, 0x35 };

   public readonly static byte[] WriteByte_FLASH_128000v14_25 = { 0x00, 
0x00, 0xFE, 0x03, 0xCC, 0x72, 0x94, 0xFF, 0x03, 0xAE, 0x31, 0x52, 0xC6, 0xFB, 0x30, 0x52, 0x0B, 
0x72, 0xFF, 0x03, 0x31, 0x52, 0x55, 0xFB, 0x30, 0x52, 0x0B, 0x72, 0xFE, 0x03, 0x31, 0x52, 0x55, 
0xFB, 0x30, 0x52, 0x0B, 0x72, 0x97, 0x90, 0x31, 0x52, 0xC6, 0xFB, 0x30, 0x52, 0x0B, 0x72, 0x95, 
0x90, 0x31, 0x52, 0xC6, 0xFB, 0x30, 0x52, 0x0B, 0x72, 0xFB, 0x30, 0x52, 0x0F, 0x72, 0x31, 0x52, 
0xC7, 0xA7, 0x20, 0xFB, 0x30, 0x52, 0x0F, 0x72, 0x31, 0x52, 0xF1, 0x35, 0x0B, 0x27, 0xF5, 0xA1, 
0xB6, 0x20, 0x08, 0x50, 0x1B, 0x72, 0x07, 0x50, 0x1B, 0x72, 0xFB, 0x30, 0x52, 0x0F, 0x72, 0x31, 
0x52, 0xFA, 0x35, 0xF7, 0x31, 0x52, 0xC6, 0xFB, 0x30, 0x52, 0x0B, 0x72, 0x62, 0x50, 0xAE, 0x35, 
0x62, 0x50, 0x56, 0x35, 0x08, 0x50, 0x1A, 0x72, 0x07, 0x50, 0x1A, 0x72, 0x97, 0x31, 0x52, 0xC6, 
0xFB, 0x30, 0x52, 0x0B, 0x72, 0xFB, 0x30, 0x52, 0x0F, 0x72, 0x31, 0x52, 0xC7, 0x95, 0x3E, 0x22, 
0xEF, 0xA1, 0x31, 0x52, 0xC6, 0xFB, 0x30, 0x52, 0x0B, 0x72, 0x35, 0x52, 0x0C, 0x35, 0x32, 0x52, 
0x01, 0x35, 0x33, 0x52, 0x00, 0x35 };   
   public readonly static byte[] WriteWord_FLASH_128000v14_25 = { 0x00, 
0x00, 0xFE, 0x03, 0xCC, 0x72, 0x94, 0xFF, 0x03, 0xAE, 0x31, 0x52, 0xC6, 0xFB, 0x30, 0x52, 0x0B, 
0x72, 0xFF, 0x03, 0x31, 0x52, 0x55, 0xFB, 0x30, 0x52, 0x0B, 0x72, 0xFE, 0x03, 0x31, 0x52, 0x55, 
0xFB, 0x30, 0x52, 0x0B, 0x72, 0x97, 0x90, 0x31, 0x52, 0xC6, 0xFB, 0x30, 0x52, 0x0B, 0x72, 0x95, 
0x90, 0x31, 0x52, 0xC6, 0xFB, 0x30, 0x52, 0x0B, 0x72, 0xFB, 0x30, 0x52, 0x0F, 0x72, 0x31, 0x52, 
0xC7, 0x8C, 0x20, 0xFB, 0x30, 0x52, 0x0F, 0x72, 0x31, 0x52, 0xF1, 0x35, 0x0B, 0x27, 0xF5, 0xA1, 
0x9B, 0x20, 0x08, 0x50, 0x1B, 0x72, 0x07, 0x50, 0x1B, 0x72, 0xFB, 0x30, 0x52, 0x0F, 0x72, 0x31, 
0x52, 0xFA, 0x35, 0xF2, 0x26, 0x5A, 0x90, 0x5C, 0xF7, 0x31, 0x52, 0xC6, 0xFB, 0x30, 0x52, 0x0B, 
0x72, 0x5C, 0x50, 0xBF, 0x35, 0x5B, 0x50, 0x40, 0x35, 0x62, 0x50, 0xAE, 0x35, 0x62, 0x50, 0x56, 
0x35, 0x08, 0x50, 0x1A, 0x72, 0x07, 0x50, 0x1A, 0x72, 0x97, 0x90, 0x31, 0x52, 0xC6, 0x00, 0x00, 
0xAE, 0x90, 0xFB, 0x30, 0x52, 0x0B, 0x72, 0x97, 0x31, 0x52, 0xC6, 0xFB, 0x30, 0x52, 0x0B, 0x72, 
0xFB, 0x30, 0x52, 0x0F, 0x72, 0x31, 0x52, 0xC7, 0x95, 0x59, 0x22, 0xEF, 0xA1, 0x31, 0x52, 0xC6, 
0xFB, 0x30, 0x52, 0x0B, 0x72, 0x35, 0x52, 0x0C, 0x35, 0x32, 0x52, 0x01, 0x35, 0x33, 0x52, 0x00, 
0x35 };
   public readonly static byte[] WriteBytes_EEPROM_128000v1425 = { 0x00, 
0x00, 0xFE, 0x03, 0xCC, 0x72, 0x94, 0xFF, 0x03, 0xAE, 0x31, 0x52, 0xC6, 0xFB, 0x30, 0x52, 0x0B, 
0x72, 0xFF, 0x03, 0x31, 0x52, 0x55, 0xFB, 0x30, 0x52, 0x0B, 0x72, 0xFE, 0x03, 0x31, 0x52, 0x55, 
0xFB, 0x30, 0x52, 0x0B, 0x72, 0x97, 0x90, 0x31, 0x52, 0xC6, 0xFB, 0x30, 0x52, 0x0B, 0x72, 0x95, 
0x90, 0x31, 0x52, 0xC6, 0xFB, 0x30, 0x52, 0x0B, 0x72, 0xFB, 0x30, 0x52, 0x0F, 0x72, 0x31, 0x52, 
0xC7, 0x0C, 0x00, 0xCC, 0xFB, 0x30, 0x52, 0x0F, 0x72, 0x31, 0x52, 0xF1, 0x35, 0x0C, 0x27, 0xF5, 
0xA1, 0x82, 0x20, 0x08, 0x50, 0x1B, 0x72, 0x07, 0x50, 0x1B, 0x72, 0xFB, 0x30, 0x52, 0x0F, 0x72, 
0x31, 0x52, 0xFA, 0x35, 0xF4, 0x26, 0xFE, 0x03, 0x5A, 0x72, 0x5C, 0x90, 0x5C, 0xF7, 0xF6, 0x90, 
0x00, 0x01, 0xAE, 0x90, 0x5C, 0x50, 0xFE, 0x35, 0x5B, 0x50, 0x01, 0x35, 0x64, 0x50, 0x56, 0x35, 
0x64, 0x50, 0xAE, 0x35, 0x08, 0x50, 0x1A, 0x72, 0x07, 0x50, 0x1A, 0x72, 0xEE, 0x26, 0xFF, 0x03, 
0x5A, 0x72, 0x5C, 0x90, 0xF7, 0x90, 0x31, 0x52, 0xC6, 0xFB, 0x30, 0x52, 0x0B, 0x72, 0x00, 0x01, 
0xAE, 0x90, 0xFE, 0x03, 0xFF, 0x03, 0x55, 0xFF, 0x03, 0x31, 0x52, 0x55, 0xFB, 0x30, 0x52, 0x0B, 
0x72, 0x97, 0x31, 0x52, 0xC6, 0xFB, 0x30, 0x52, 0x0B, 0x72, 0xFB, 0x30, 0x52, 0x0F, 0x72, 0x31, 
0x52, 0xC7, 0x95, 0x72, 0x22, 0xEF, 0xA1, 0x31, 0x52, 0xC6, 0xFB, 0x30, 0x52, 0x0B, 0x72, 0x35, 
0x52, 0x0C, 0x35, 0x32, 0x52, 0x01, 0x35, 0x33, 0x52, 0x00, 0x35 };
   public readonly static byte[] WriteBlocks_FLASH_128000v14_25 = { 0x00, 
0x00, 0xFE, 0x03, 0xCC, 0x72, 0x94, 0xFF, 0x03, 0xAE, 0x31, 0x52, 0xC6, 0xFB, 0x30, 0x52, 0x0B, 
0x72, 0xFF, 0x03, 0x31, 0x52, 0x55, 0xFB, 0x30, 0x52, 0x0B, 0x72, 0xFE, 0x03, 0x31, 0x52, 0x55, 
0xFB, 0x30, 0x52, 0x0B, 0x72, 0x97, 0x90, 0x31, 0x52, 0xC6, 0xFB, 0x30, 0x52, 0x0B, 0x72, 0x95, 
0x90, 0x31, 0x52, 0xC6, 0xFB, 0x30, 0x52, 0x0B, 0x72, 0xFB, 0x30, 0x52, 0x0F, 0x72, 0x31, 0x52, 
0xC7, 0x86, 0x20, 0xFB, 0x30, 0x52, 0x0F, 0x72, 0x31, 0x52, 0xF1, 0x35, 0x0B, 0x27, 0xF5, 0xA1, 
0x95, 0x20, 0xBF, 0x26, 0xFF, 0x03, 0x5A, 0x72, 0x08, 0x50, 0x1B, 0x72, 0x07, 0x50, 0x1B, 0x72, 
0xFB, 0x30, 0x52, 0x0F, 0x72, 0x31, 0x52, 0xFA, 0x35, 0xF2, 0x26, 0x5A, 0x90, 0x5C, 0xF7, 0x31, 
0x52, 0xC6, 0xFB, 0x30, 0x52, 0x0B, 0x72, 0x08, 0x50, 0x1A, 0x72, 0x07, 0x50, 0x1A, 0x72, 0x5C, 
0x50, 0xFE, 0x35, 0x5B, 0x50, 0x01, 0x35, 0x62, 0x50, 0xAE, 0x35, 0x62, 0x50, 0x56, 0x35, 0x40, 
0x00, 0xAE, 0x90, 0xFF, 0x03, 0x31, 0x52, 0x55, 0xFB, 0x30, 0x52, 0x0B, 0x72, 0x97, 0x31, 0x52, 
0xC6, 0xFB, 0x30, 0x52, 0x0B, 0x72, 0xFB, 0x30, 0x52, 0x0F, 0x72, 0x31, 0x52, 0xC7, 0x95, 0x5F, 
0x22, 0xEF, 0xA1, 0x31, 0x52, 0xC6, 0xFB, 0x30, 0x52, 0x0B, 0x72, 0x35, 0x52, 0x0C, 0x35, 0x32, 
0x52, 0x01, 0x35, 0x33, 0x52, 0x00, 0x35 };
	    
	public static readonly byte[] rev14_4800_OPTION = {
		0x00, 0x00, 0xFF, 0x00, 0xFF, 0x00, 0xFF, 0x00, 0xFF, 0x00, 0xFF, 0x35, 0x0D, 0x52, 0x32, 0x35, 
		0x0C, 0x52, 0x35, 0x35, 0x14, 0x52, 0x31, 0x5A, 0x27, 0x16, 0x72, 0x0B, 0x52, 0x30, 0xF8, 0xC6, 
		0x52, 0x31, 0x72, 0x0B, 0x52, 0x30, 0xFB, 0x3B, 0x52, 0x31, 0x4A, 0x26, 0xF5, 0x96, 0x5C, 0xFC, 
		0xCE, 0x9F, 0xFE, 0x2B, 0xFA, 0x90, 0xAE, 0x42, 0x7F, 0xAE, 0x02, 0x7F, 0xCC, 0x9F, 0xF4, 0x00 };
	public static readonly byte[] rev14_8000_FLASH = { 0x96, 0xCC, 0x9F, 0xF0 };
 	public static readonly byte[] rev14_9FF0_FLASH = { 
		0x90, 0xAE, 0x4C, 0x0A, 0x90, 0xF6, 0xF7, 0x90, 0x5A, 0x5A, 0x2A, 0xF8, 0x5C, 0xFC, 0x80, 0x04 };
	    
	public static SortedDictionary<int, byte[]> rev14_srtDicAdrBts() {
		SortedDictionary<int, byte[]> srtDicAdrBts = new SortedDictionary<int, byte[]>();
		srtDicAdrBts.Add(0x4800, rev14_4800_OPTION);
		srtDicAdrBts.Add(0x8000, rev14_8000_FLASH);
		srtDicAdrBts.Add(0x9FF0, rev14_9FF0_FLASH);
		return srtDicAdrBts;
	}
	public static SortedDictionary<int, byte> rev14_srtDicAdrBt() {
		SortedDictionary<int, byte> srtDicAdrBt = new SortedDictionary<int, byte>();
		   for (int i = 0; i < rev14_4800_OPTION.Length; i++){
		   	srtDicAdrBt.Add(i + 0x4800, rev14_4800_OPTION[i]);
		   }
		   for (int i = 0; i < rev14_8000_FLASH.Length; i++){
		   	srtDicAdrBt.Add(i + 0x8000, rev14_8000_FLASH[i]);
		   }
		   for (int i = 0; i < rev14_9FF0_FLASH.Length; i++){
		   	srtDicAdrBt.Add(i + 0x9FF0, rev14_9FF0_FLASH[i]);
		   }
		return srtDicAdrBt;
	}
	
	public static readonly byte[] rev25_4800_OPTION = {
		0x00, 0x00, 0xFF, 0x00, 0xFF, 0x00, 0xFF, 0x00, 0xFF, 0x00, 0xFF, 0x35, 0x0D, 0x52, 0x32, 0x35, 
		0x0C, 0x52, 0x35, 0x35, 0x25, 0x52, 0x31, 0x5A, 0x27, 0x16, 0x72, 0x0B, 0x52, 0x30, 0xF8, 0xC6, 
		0x52, 0x31, 0x72, 0x0B, 0x52, 0x30, 0xFB, 0x3B, 0x52, 0x31, 0x4A, 0x26, 0xF5, 0x96, 0x5C, 0xFC, 
		0xAE, 0x80, 0x04, 0x2B, 0xFA, 0x90, 0xAE, 0x42, 0x7F, 0xAE, 0x02, 0x7F, 0xCC, 0x9F, 0xF6, 0x00 };
	public static readonly byte[] rev25_8000_FLASH = { 0x96, 0xCC, 0x9F, 0xF2 };
	public static readonly byte[] rev25_9FF2_FLASH = { 
		0x90, 0xAE, 0x4C, 0x0A, 0x90, 0xF6, 0xF7, 0x90, 0x5A, 0x5A, 0x2A, 0xF8, 0x5C, 0xFC   };
	
	public static SortedDictionary<int, byte[]> rev25_srtDicAdrBts() {
		SortedDictionary<int, byte[]> srtDicAdrBts = new SortedDictionary<int, byte[]>();	
		srtDicAdrBts.Add(0x4800, rev25_4800_OPTION);
		srtDicAdrBts.Add(0x8000, rev25_8000_FLASH);
		srtDicAdrBts.Add(0x9FF2, rev25_9FF2_FLASH);
		return srtDicAdrBts;
	}
	public static SortedDictionary<int, byte> rev25_srtDicAdrBt() {
		SortedDictionary<int, byte> srtDicAdrBt = new SortedDictionary<int, byte>();
		   for (int i = 0; i < rev25_4800_OPTION.Length; i++){
		   	srtDicAdrBt.Add(i + 0x4800, rev25_4800_OPTION[i]);
		   }
		   for (int i = 0; i < rev25_8000_FLASH.Length; i++){
		   	srtDicAdrBt.Add(i + 0x8000, rev25_8000_FLASH[i]);
		   }
		   for (int i = 0; i < rev25_9FF2_FLASH.Length; i++){
		   	srtDicAdrBt.Add(i + 0x9FF2, rev25_9FF2_FLASH[i]);
		   }
		return srtDicAdrBt;
	}
	    
	    
	public static byte[] boot_FLASH_8000_v14 = {0x96, 0xCC, 0x9F, 0xF0};
	public static byte[] boot_FLASH_8000_v25 = {0x96, 0xCC, 0x9F, 0xF2};
	public static byte[] boot_FLASH_9FF0 = {0x90, 0xAE, 0x4C, 0x0A, 0x90, 0xF6, 0xF7,
											0x90, 0x5A, 0x5A, 0x2A, 0xF8, 0x5C, 0xFC};
	public static byte[] boot_FLASH_9FF2 = {0x90, 0xAE, 0x4C, 0x0A, 0x90, 0xF6, 0xF7,
											0x90, 0x5A, 0x5A, 0x2A, 0xF8, 0x5C, 0xFC};
	    
	public static bool inCycle = true;
	
	public static int readBound = 0;
    public static Queue<byte> queueBytes;
    public static Queue<char> queueChars;
    public static Thread readThread;
    public static Thread readThreadBytes;
    public static Thread readThreadKeys;
    public static SerialPort sPort;
	public static string portName;
	public static string readLine = "";	
	
	public static SortedDictionary<int, byte> rxAddressByte;
	//public static ArrayList<int> adrArrayList;
	//public static ArrayList<byte> btArrayList;
	public static bool unready = true;
	public static byte rx_byte;
	public static string statusUART = "loader";
	public static string crntModul = "";
	public static string err_Msg = " OK";	// прекращение программы
	public static string wrng_Msg = " OK";  // продолжение программы
	public static char key;
	public static byte[] btBytes = new byte[1];
	public static byte[] rx1Byte = new byte[1];
	public static byte[] tx2Bytes = new byte[2];
	public static byte[] rx4Bytes = new byte[4];
	public static byte[] rx64Bytes = new byte[64];
	public static byte[] tx64Bytes = new byte[64];
	public static byte[] go_bytes = { 0xF5, 0x00, 0x00, 0x00, 0x17, 0x00}; // { GO cmd, YH, YL, go_adrH, go_adrL, cntr }

	
}// class Dumps
// Dumps.cs