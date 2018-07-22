using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

    public class MotorolaSRECfile_long
    {
        public MotorolaSRECfile_long(string filename)
        {
        	if (filename.ToLower().EndsWith(".s19") | filename.ToLower().EndsWith(".srec")){}
			else FileErrorMessages.Add(" Файл имеет расширение отличное от .s19(.srec)");
			try 
			{
				StreamReader sr = new StreamReader(filename);
				bool eof = false;
				int lineNumber = 0;
				while (!eof)
				{
					lineNumber++;
					SRECline line = new SRECline(sr.ReadLine(), lineNumber);
					//if (ErrorMessage == "") ErrorMessage = line.ErrorMessages;
					FileErrorMessages.AddRange(line.LineErrorMessages);
					if (line.CriticalErrors == true) CriticalError = true;
					
					switch (line.recordtype)
					{

						case SRECline.RecordType.DataRecordS19:
						case SRECline.RecordType.DataRecordSREC:
								AddressLineSorted.Add((long)(line.address), line.data);
								AddressLine.Add((long)(line.address), line.data);
								int ij=0;
								foreach( byte bt in line.data)
									{
											AddressByteSorted.Add((long)(line.address+ij), line.data[ij]);
											ij++;		
									}
								
							break;

						case SRECline.RecordType.EndOfFileS19:
						case SRECline.RecordType.EndOfFileSREC:	
							eof = true;
							break;
					}
					if (sr.EndOfStream)
					{
						eof = true;
					}
				}
				if(AddressByteSorted.Count != 0) {
					foreach( KeyValuePair<long, byte> dabs in AddressByteSorted )
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
				} else {FileErrorMessages.Add(" Файл не имеет адресов.");
				        minAddress = maxAddress = long.MinValue;}

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
        public long[] GetAddresses() { return Addresses.ToArray(); }
        public long GetMinAddress() { return minAddress; }
        public long GetMaxAddress() { return maxAddress; }
        public long GetBaseAddress() { return minAddress; }
        public byte[] GetBytes() { return Bytes.ToArray(); }
        public int GetCount() { return AddressByteSorted.Count; }
        public List<string> GetErrorMessages() {return FileErrorMessages;}
        //public string[] GetErrorMessages() { return FileErrorMessages.ToArray(); }
		public SortedDictionary<long, byte> GetAddressByteSorted() { return AddressByteSorted; }
		public SortedDictionary<long, byte[]> GetAddressLineSorted() { return AddressLineSorted; }
		public Dictionary<long, byte[]> GetAddressLine() { return AddressLine; }
		
        List<long> Addresses = new List<long>();
        List<byte> Bytes = new List<byte>();
        List<byte> data = new List<byte>();
		SortedDictionary<long, byte> AddressByteSorted = new SortedDictionary<long, byte>();
		SortedDictionary<long, byte[]> AddressLineSorted = new SortedDictionary<long, byte[]>();
		Dictionary<long, byte[]> AddressLine = new Dictionary<long, byte[]>();
		
        public class SRECline
        {
            public enum RecordType
            {
               Title = 0,
               DataRecordS19 = 1,   // s19
               DataRecordSREC = 3,   // srec
               EndOfFileSREC = 7,    // srec
               EndOfFileS19 = 9,    // s19
                //ExtendedSegmentAddress = 2,
                //StartSegmentAddress = 3,
                //ExtendedLinearAddress = 4,
                //StartLinearAddress = 5
            }

            public SRECline(string s, int ln)
            {
				
                startSymbol = s[0];
				if (startSymbol != 'S') LineErrorMessages.Add(" В начале строки " + ln + " отсутствует символ 's'");
                s = s.Substring(1);
                
                try{
					recordtypes = int.Parse(s.Substring(0, 1), System.Globalization.NumberStyles.HexNumber);
					recordtype = (RecordType)recordtypes;
					s = s.Substring(1);
				}
				catch (Exception ex){
					LineErrorMessages.Add(" В строке " + ln + " " + ex.Message);
					CriticalErrors = true;
				}  
                try{
					length = long.Parse(s.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
					s = s.Substring(2);
				}
				catch (Exception ex){
					//ErrorMessages = ex.Message;
					LineErrorMessages.Add(" В строке " + ln + " " + ex.Message);
					CriticalErrors = true;
				}
                if(recordtype == RecordType.DataRecordS19){
                
	                try{
						address = long.Parse(s.Substring(0, 4), System.Globalization.NumberStyles.HexNumber);
						s = s.Substring(4);
					}
					catch (Exception ex){
						//ErrorMessages = ex.Message;
						LineErrorMessages.Add(" В строке " + ln + " " + ex.Message);
						CriticalErrors = true;
					}
                }
                if(recordtype == RecordType.DataRecordSREC){
                
	                try{
						address = long.Parse(s.Substring(0, 8), System.Globalization.NumberStyles.HexNumber);
						s = s.Substring(8);
					}
					catch (Exception ex){
						//ErrorMessages = ex.Message;
						LineErrorMessages.Add(" В строке " + ln + " " + ex.Message);
						CriticalErrors = true;
					}
                }    
                
                if(recordtype == RecordType.DataRecordS19) {
                	data = new byte[length-3];
					for (int i = 0; i < length-3; i++)
					{
						
                		try{
							data[i] = byte.Parse(s.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
							s = s.Substring(2);
						}
						catch (Exception ex){
							LineErrorMessages.Add(" В строке " + ln + " " + ex.Message);
							CriticalErrors = true;
						}
					}
                }
                if(recordtype == RecordType.DataRecordSREC) {
                	data = new byte[length-5];
					for (int i = 0; i < length-5; i++)
					{
						
                		try{
							data[i] = byte.Parse(s.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
							s = s.Substring(2);
						}
						catch (Exception ex){
							LineErrorMessages.Add(" В строке " + ln + " " + ex.Message);
							CriticalErrors = true;
						}
					}
                }	
					
                try{
					checksum = byte.Parse(s.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
					s = s.Substring(2);
				}
				catch (Exception ex){
					//ErrorMessages = ex.Message;
					LineErrorMessages.Add(" В строке " + ln + " " + ex.Message);
					CriticalErrors = true;
				}
                if(recordtype == RecordType.DataRecordS19) {
					for (int i = 0; i < length-3; i++){
							bytes += data[i];
					}
                }
                if(recordtype == RecordType.DataRecordSREC) {
					for (int i = 0; i < length-5; i++){
							bytes += data[i];
					}
                }
				if(recordtype == RecordType.DataRecordS19) {
					if ((byte)(length + (byte)(address>>8) + (byte)address + bytes + checksum) != 0xFF) 
						LineErrorMessages.Add(" В строке " + ln + " не совпадает контрольная сумма");
				}
				if(recordtype == RecordType.DataRecordSREC) {
					if ((byte)(length + (byte)(address>>24) + (byte)(address>>16) + (byte)(address>>8) + (byte)address + bytes + checksum) != 0xFF) 
						LineErrorMessages.Add(" В строке " + ln + " не совпадает контрольная сумма");
				}
            }


            char startSymbol;
            long length;
            public long address;
            public RecordType recordtype;
			public int recordtypes;
            public  byte[] data;
			public SortedDictionary<long, byte> AddressByteSorted;
			public SortedDictionary<long, byte[]> AddressLineSorted;
			public Dictionary<long, byte[]> AddressLine;
            byte checksum;
			byte bytes = 0;
			public List<string> LineErrorMessages = new List<string>();
			public bool CriticalErrors = false;

        }
	    //public string[] ErrorMessage;
		public List<string> FileErrorMessages = new List<string>();
		public string ErrorMessage ="";
		public bool CriticalError = false;
		public long minAddress = long.MaxValue;
		public long maxAddress = long.MinValue;
    } // class MotorolaSRECfile_long
// MotorolaSRECfile_long.cs