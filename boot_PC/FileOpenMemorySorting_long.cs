using System; 
using System.IO; 
using System.Collections.Generic;


	public class FileOpenMemorySorting_long  {
	
		
		public FileOpenMemorySorting_long(string filename){
			fileName = filename;
		
			if (fileName.ToLower().EndsWith(".s19") | fileName.ToLower().EndsWith(".srec")) {
				Console.WriteLine(" Файл имеет расширение .s19(.srec)");
				MotorolaSRECfile_long s19File = new MotorolaSRECfile_long(fileName);
				errorMessages = new MotorolaSRECfile_long(fileName).GetErrorMessages();
				AllMemoryAddressByte = new MotorolaSRECfile_long(fileName).GetAddressByteSorted();
				AllMemoryAddressLineSorted = new MotorolaSRECfile_long(fileName).GetAddressLineSorted();
				AllMemoryAddressLine = new MotorolaSRECfile_long(fileName).GetAddressLine();
				BaseAddress = new MotorolaSRECfile_long(fileName).GetBaseAddress();
				//AllMemoryAddressByteSorting();
				//AllMemoryAddressLineSorting();
			}
			else if (fileName.ToLower().EndsWith(".hex")/* == true*/) {
				Console.WriteLine(" Файл имеет расширение .hex");
				IntelHEXfile_long hexFile = new IntelHEXfile_long(fileName);
				errorMessages = new IntelHEXfile_long(fileName).GetErrorMessages();
				AllMemoryAddressByte = new IntelHEXfile_long(fileName).GetAddressByteSorted();
				AllMemoryAddressLineSorted = new IntelHEXfile_long(fileName).GetAddressLineSorted();
				AllMemoryAddressLine = new IntelHEXfile_long(fileName).GetAddressLine();
				BaseAddress = new IntelHEXfile_long(fileName).GetBaseAddress();
				//AllMemoryAddressByteSorting();
				//AllMemoryAddressLineSorting();
				}
			else {
				Console.WriteLine(" Файл имеет расширение отличное от .s19(.srec) или .hex");
				Console.ReadKey();
				return;
			}
		
		}// FileOpenMemorySorting(string filename)
	
        /*
			void AllMemoryAddressByteSorting() {
				RAMaddressByteSorted = new SortedDictionary<long, byte>(AllMemoryAddressByte);
				EEPROMaddressByteSorted = new SortedDictionary<long, byte>(AllMemoryAddressByte);
				OPTIONaddressByteSorted = new SortedDictionary<long, byte>(AllMemoryAddressByte);
				HWaddressByteSorted = new SortedDictionary<long, byte>(AllMemoryAddressByte);
				FLASHaddressByteSorted = new SortedDictionary<long, byte>(AllMemoryAddressByte);
				OTHERaddressByteSorted = new SortedDictionary<long, byte>(AllMemoryAddressByte);
				
				foreach(KeyValuePair<long, byte> kvp in AllMemoryAddressByte){
					if(kvp.Key < 0x0000 | kvp.Key > 0x03FF) RAMaddressByteSorted.Remove(kvp.Key); else OTHERaddressByteSorted.Remove(kvp.Key);
					if(kvp.Key < 0x4000 | kvp.Key > 0x427F) EEPROMaddressByteSorted.Remove(kvp.Key); else OTHERaddressByteSorted.Remove(kvp.Key);
					if(kvp.Key < 0x4800 | kvp.Key > 0x483F) OPTIONaddressByteSorted.Remove(kvp.Key); else OTHERaddressByteSorted.Remove(kvp.Key);
					if(kvp.Key < 0x5000 | kvp.Key > 0x57FF) HWaddressByteSorted.Remove(kvp.Key); else OTHERaddressByteSorted.Remove(kvp.Key);
					if(kvp.Key < 0x8000 | kvp.Key > 0x9FFF) FLASHaddressByteSorted.Remove(kvp.Key); else OTHERaddressByteSorted.Remove(kvp.Key);	
				}
				
			}// AllMemoryAddressByteSorting
	
			void AllMemoryAddressLineSorting() {
				RAMaddressLineSorted = new SortedDictionary<long, byte[]>(AllMemoryAddressLine);
				EEPROMaddressLineSorted = new SortedDictionary<long, byte[]>(AllMemoryAddressLine);
				OPTIONaddressLineSorted = new SortedDictionary<long, byte[]>(AllMemoryAddressLine);
				HWaddressLineSorted = new SortedDictionary<long, byte[]>(AllMemoryAddressLine);
				FLASHaddressLineSorted = new SortedDictionary<long, byte[]>(AllMemoryAddressLine);
				OTHERaddressLineSorted = new SortedDictionary<long, byte[]>(AllMemoryAddressLine);
				
				foreach(KeyValuePair<long, byte[]> kvp in AllMemoryAddressLine){
					if(kvp.Key < 0x0000 | kvp.Key > 0x03FF) RAMaddressLineSorted.Remove(kvp.Key); else OTHERaddressLineSorted.Remove(kvp.Key);
					if(kvp.Key < 0x4000 | kvp.Key > 0x427F) EEPROMaddressLineSorted.Remove(kvp.Key); else OTHERaddressLineSorted.Remove(kvp.Key);
					if(kvp.Key < 0x4800 | kvp.Key > 0x483F) OPTIONaddressLineSorted.Remove(kvp.Key); else OTHERaddressLineSorted.Remove(kvp.Key);
					if(kvp.Key < 0x5000 | kvp.Key > 0x57FF) HWaddressLineSorted.Remove(kvp.Key); else OTHERaddressLineSorted.Remove(kvp.Key);
					if(kvp.Key < 0x8000 | kvp.Key > 0x9FFF) FLASHaddressLineSorted.Remove(kvp.Key); else OTHERaddressLineSorted.Remove(kvp.Key);	
				}
			}// AllMemoryAddressLineSorting
	
	      */

        	public SortedDictionary<long, byte> GetAllMemoryaddressByteSorted() { return AllMemoryAddressByte; }
        	/*
        	public SortedDictionary<long, byte> GetRAMaddressByteSorted() { return RAMaddressByteSorted; }
        	public SortedDictionary<long, byte> GetEEPROMaddressByteSorted() { return EEPROMaddressByteSorted; }
        	public SortedDictionary<long, byte> GetOPTIONaddressByteSorted() { return OPTIONaddressByteSorted; }
        	public SortedDictionary<long, byte> GetHWaddressByteSorted() { return HWaddressByteSorted; }
        	public SortedDictionary<long, byte> GetFLASHaddressByteSorted() { return FLASHaddressByteSorted; }
        	public SortedDictionary<long, byte> GetOTHERaddressByteSorted() { return OTHERaddressByteSorted; }
        	*/
        	public Dictionary<long, byte[]> GetAllMemoryaddressLine() { return AllMemoryAddressLine; }
        	public SortedDictionary<long, byte[]> GetAllMemoryaddressLineSorted() { return AllMemoryAddressLineSorted; }
        	
        	/*
        	public SortedDictionary<long, byte[]> GetRAMaddressLineSorted() { return RAMaddressLineSorted; }
        	public SortedDictionary<long, byte[]> GetEEPROMaddressLineSorted() { return EEPROMaddressLineSorted; }
        	public SortedDictionary<long, byte[]> GetOPTIONaddressLineSorted() { return OPTIONaddressLineSorted; }
        	public SortedDictionary<long, byte[]> GetHWaddressLineSorted() { return HWaddressLineSorted; }
        	public SortedDictionary<long, byte[]> GetFLASHaddressLineSorted() { return FLASHaddressLineSorted; }
        	public SortedDictionary<long, byte[]> GetOTHERaddressLineSorted() { return OTHERaddressLineSorted; }
        	*/
        	public long GetBaseAddress() { return BaseAddress; }
        	public List<string> GetErrorMessages() {return errorMessages;}
			string fileName;
			public List<string> errorMessages;

			SortedDictionary<long, byte> AllMemoryAddressByte;
			long BaseAddress;
			/*
			SortedDictionary<long, byte> RAMaddressByteSorted;
			SortedDictionary<long, byte> EEPROMaddressByteSorted;
			SortedDictionary<long, byte> OPTIONaddressByteSorted;
			SortedDictionary<long, byte> HWaddressByteSorted;
			SortedDictionary<long, byte> FLASHaddressByteSorted;
			SortedDictionary<long, byte> OTHERaddressByteSorted;
			*/
			Dictionary<long, byte[]> AllMemoryAddressLine;
			SortedDictionary<long, byte[]> AllMemoryAddressLineSorted;
			
			/*
			SortedDictionary<long, byte[]> RAMaddressLineSorted;
			SortedDictionary<long, byte[]> EEPROMaddressLineSorted;
			SortedDictionary<long, byte[]> OPTIONaddressLineSorted;
			SortedDictionary<long, byte[]> HWaddressLineSorted;
			SortedDictionary<long, byte[]> FLASHaddressLineSorted;
			SortedDictionary<long, byte[]> OTHERaddressLineSorted;
			*/
			
	}// class FileOpenMemorySorting_long 
// FileOpenMemorySorting_long.cs

