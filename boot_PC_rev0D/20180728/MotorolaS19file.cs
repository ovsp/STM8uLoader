using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

    public class MotorolaS19file
    {
        public MotorolaS19file(string filename)
        {
			if (filename.EndsWith(".s19") != true) FileErrorMessages.Add("Файл имеет расширение отличное от s19");
			
			try 
			{
				StreamReader sr = new StreamReader(filename);
				bool eof = false;
				int lineNumber = 0;
				while (!eof)
				{
					lineNumber++;
					S19line line = new S19line(sr.ReadLine(), lineNumber);
					//if (ErrorMessage == "") ErrorMessage = line.ErrorMessages;
					FileErrorMessages.AddRange(line.LineErrorMessages);
					if (line.CriticalErrors == true) CriticalError = true;
					
					switch (line.recordtype)
					{
						case S19line.RecordType.DataRecord:
								//data.AddRange(line.data);
								AddressLineSorted.Add(line.address, line.data);
								int ij=0;
								foreach( byte bt in line.data)
									{
											AddressByteSorted.Add(line.address+ij, line.data[ij]);
											ij++;		
									}
								
							break;

						case S19line.RecordType.EndOfFile:
							eof = true;
							break;
					}
					if (sr.EndOfStream)
					{
						eof = true;
					}
				}
				
				foreach( KeyValuePair<int, byte> dabs in AddressByteSorted )
				{
					Addresses.Add(dabs.Key);
					Bytes.Add(dabs.Value);
				}
				
				for (int i = 0; i < AddressByteSorted.Count; i++)
				{
					if (Addresses[i] < minAddress) minAddress = Addresses[i];
				}
				
				for (int i = 0; i < AddressByteSorted.Count; i++)
				{
					if (Addresses[i] > maxAddress) maxAddress = Addresses[i];
				}
				
				

				sr.Close();
				sr.Dispose();
			}
			catch(Exception ex)
			{
				ErrorMessage = ex.Message;
				CriticalError = true;
			}
        }

       // public byte[] GetData()
       // {
        //    return data.ToArray();
        //}
        public int[] GetAddresses() { return Addresses.ToArray(); }
        public int GetMinAddress() { return minAddress; }
        public int GetMaxAddress() { return maxAddress; }
        public byte[] GetBytes() { return Bytes.ToArray(); }
        public int GetCount() { return AddressByteSorted.Count; }
        public string[] GetErrorMessages() { return FileErrorMessages.ToArray(); }
		public SortedDictionary<int, byte> GetAddressByteSorted() { return AddressByteSorted; }
		public SortedDictionary<int, byte[]> GetAddressLineSorted() { return AddressLineSorted; }
		
        List<int> Addresses = new List<int>();
        List<byte> Bytes = new List<byte>();
        List<byte> data = new List<byte>();
		SortedDictionary<int, byte> AddressByteSorted = new SortedDictionary<int, byte>();
		SortedDictionary<int, byte[]> AddressLineSorted = new SortedDictionary<int, byte[]>();
		
        public class S19line
        {
            public enum RecordType
            {
                Title = 0,
            	DataRecord = 1,
                EndOfFile = 9,
                //ExtendedSegmentAddress = 2,
                //StartSegmentAddress = 3,
                //ExtendedLinearAddress = 4,
                //StartLinearAddress = 5
            }

            public S19line(string s, int ln)
            {
				
                startSymbol = s[0];
				//if (colon != ':') ErrorMessages = "Отсутствует двоеточие в начале одной из строк";
				if (startSymbol != 'S') LineErrorMessages.Add("В начале строки " + ln + " отсутствует символ 's'");
                s = s.Substring(1);
                
                try{
					recordtypes = int.Parse(s.Substring(0, 1), System.Globalization.NumberStyles.HexNumber);
					recordtype = (RecordType)recordtypes;
					s = s.Substring(1);
				}
				catch (Exception ex){
					LineErrorMessages.Add("В строке " + ln + " " + ex.Message);
					CriticalErrors = true;
				}  
                try{
					length = int.Parse(s.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
					s = s.Substring(2);
				}
				catch (Exception ex){
					//ErrorMessages = ex.Message;
					LineErrorMessages.Add("В строке " + ln + " " + ex.Message);
					CriticalErrors = true;
				}
                try{
					address = int.Parse(s.Substring(0, 4), System.Globalization.NumberStyles.HexNumber);
					s = s.Substring(4);
				}
				catch (Exception ex){
					//ErrorMessages = ex.Message;
					LineErrorMessages.Add("В строке " + ln + " " + ex.Message);
					CriticalErrors = true;
				}

					data = new byte[length-3];
					for (int i = 0; i < length-3; i++)
					{
						
                		try{
							data[i] = byte.Parse(s.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
							s = s.Substring(2);
						}
						catch (Exception ex){
							LineErrorMessages.Add("В строке " + ln + " " + ex.Message);
							CriticalErrors = true;
						}
					}
                try{
					checksum = byte.Parse(s.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
					s = s.Substring(2);
				}
				catch (Exception ex){
					//ErrorMessages = ex.Message;
					LineErrorMessages.Add("В строке " + ln + " " + ex.Message);
					CriticalErrors = true;
				}	
				for (int i = 0; i < length-3; i++){
						bytes += data[i];
				}
				if(recordtype == RecordType.DataRecord) {
					if ((byte)(length + (address>>8) + (byte)address + recordtypes + bytes + checksum) != 0) 
						LineErrorMessages.Add("В строке " + ln + " не совпадает контрольная сумма");
				}
            }


            char startSymbol;
            int length;
            public int address;
            public RecordType recordtype;
			public int recordtypes;
            public  byte[] data;
			public SortedDictionary<int, byte> AddressByteSorted;
			public SortedDictionary<int, byte[]> AddressLineSorted;
            byte checksum;
			byte bytes = 0;
			public List<string> LineErrorMessages = new List<string>();
			public bool CriticalErrors = false;

        }
	    //public string[] ErrorMessage;
		public List<string> FileErrorMessages = new List<string>();
		public string ErrorMessage ="";
		public bool CriticalError = false;
		public int minAddress = int.MaxValue;
		public int maxAddress = int.MinValue;
    }
