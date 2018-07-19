using System; 
using System.IO; 
using System.Collections.Generic;


	public class FileOpenMemorySorting  {
	
		
		public FileOpenMemorySorting(string filename){
		
			fileName = filename;
		
			if (fileName.EndsWith(".s19")/* == true*/) {
				Console.WriteLine(" Файл имеет расширение .s19");
				MotorolaS19file s19File = new MotorolaS19file(fileName);
				errorMessages = new MotorolaS19file(fileName).GetErrorMessages();
				AllMemoryAddressByte = new MotorolaS19file(fileName).GetAddressByteSorted();
				AllMemoryAddressLine = new MotorolaS19file(fileName).GetAddressLineSorted();
				AllMemoryAddressByteSorting();
				AllMemoryAddressLineSorting();
			}
			else if (fileName.EndsWith(".hex")/* == true*/) {
				Console.WriteLine(" Файл имеет расширение .hex");
				IntelHEXfile hexFile = new IntelHEXfile(fileName);
				errorMessages = new IntelHEXfile(fileName).GetErrorMessages();
				AllMemoryAddressByte = new IntelHEXfile(fileName).GetAddressByteSorted();
				AllMemoryAddressLine = new IntelHEXfile(fileName).GetAddressLineSorted();
				AllMemoryAddressByteSorting();
				AllMemoryAddressLineSorting();
				}
			else {
				Console.WriteLine(" Файл имеет расширение отличное от .s19 или .hex");
				Console.ReadKey();
				return;
			}
		
		}// FileOpenMemorySorting(string filename)
	

			void AllMemoryAddressByteSorting() {
				RAMaddressByteSorted = new SortedDictionary<int, byte>(AllMemoryAddressByte);
				EEPROMaddressByteSorted = new SortedDictionary<int, byte>(AllMemoryAddressByte);
				OPTIONaddressByteSorted = new SortedDictionary<int, byte>(AllMemoryAddressByte);
				HWaddressByteSorted = new SortedDictionary<int, byte>(AllMemoryAddressByte);
				FLASHaddressByteSorted = new SortedDictionary<int, byte>(AllMemoryAddressByte);
				OTHERaddressByteSorted = new SortedDictionary<int, byte>(AllMemoryAddressByte);
				
				foreach(KeyValuePair<int, byte> kvp in AllMemoryAddressByte){
					if(kvp.Key < 0x0000 | kvp.Key > 0x03FF) RAMaddressByteSorted.Remove(kvp.Key); else OTHERaddressByteSorted.Remove(kvp.Key);
					if(kvp.Key < 0x4000 | kvp.Key > 0x427F) EEPROMaddressByteSorted.Remove(kvp.Key); else OTHERaddressByteSorted.Remove(kvp.Key);
					if(kvp.Key < 0x4800 | kvp.Key > 0x483F) OPTIONaddressByteSorted.Remove(kvp.Key); else OTHERaddressByteSorted.Remove(kvp.Key);
					if(kvp.Key < 0x5000 | kvp.Key > 0x57FF) HWaddressByteSorted.Remove(kvp.Key); else OTHERaddressByteSorted.Remove(kvp.Key);
					if(kvp.Key < 0x8000 | kvp.Key > 0x9FFF) FLASHaddressByteSorted.Remove(kvp.Key); else OTHERaddressByteSorted.Remove(kvp.Key);	
				}
				
			}// AllMemoryAddressByteSorting
	
			void AllMemoryAddressLineSorting() {
				RAMaddressLineSorted = new SortedDictionary<int, byte[]>(AllMemoryAddressLine);
				EEPROMaddressLineSorted = new SortedDictionary<int, byte[]>(AllMemoryAddressLine);
				OPTIONaddressLineSorted = new SortedDictionary<int, byte[]>(AllMemoryAddressLine);
				HWaddressLineSorted = new SortedDictionary<int, byte[]>(AllMemoryAddressLine);
				FLASHaddressLineSorted = new SortedDictionary<int, byte[]>(AllMemoryAddressLine);
				OTHERaddressLineSorted = new SortedDictionary<int, byte[]>(AllMemoryAddressLine);
				
				foreach(KeyValuePair<int, byte[]> kvp in AllMemoryAddressLine){
					if(kvp.Key < 0x0000 | kvp.Key > 0x03FF) RAMaddressLineSorted.Remove(kvp.Key); else OTHERaddressLineSorted.Remove(kvp.Key);
					if(kvp.Key < 0x4000 | kvp.Key > 0x427F) EEPROMaddressLineSorted.Remove(kvp.Key); else OTHERaddressLineSorted.Remove(kvp.Key);
					if(kvp.Key < 0x4800 | kvp.Key > 0x483F) OPTIONaddressLineSorted.Remove(kvp.Key); else OTHERaddressLineSorted.Remove(kvp.Key);
					if(kvp.Key < 0x5000 | kvp.Key > 0x57FF) HWaddressLineSorted.Remove(kvp.Key); else OTHERaddressLineSorted.Remove(kvp.Key);
					if(kvp.Key < 0x8000 | kvp.Key > 0x9FFF) FLASHaddressLineSorted.Remove(kvp.Key); else OTHERaddressLineSorted.Remove(kvp.Key);	
				}
			}// AllMemoryAddressLineSorting
	
	

        	public SortedDictionary<int, byte> GetAllMemoryaddressByteSorted() { return AllMemoryAddressByte; }
        	public SortedDictionary<int, byte> GetRAMaddressByteSorted() { return RAMaddressByteSorted; }
        	public SortedDictionary<int, byte> GetEEPROMaddressByteSorted() { return EEPROMaddressByteSorted; }
        	public SortedDictionary<int, byte> GetOPTIONaddressByteSorted() { return OPTIONaddressByteSorted; }
        	public SortedDictionary<int, byte> GetHWaddressByteSorted() { return HWaddressByteSorted; }
        	public SortedDictionary<int, byte> GetFLASHaddressByteSorted() { return FLASHaddressByteSorted; }
        	public SortedDictionary<int, byte> GetOTHERaddressByteSorted() { return OTHERaddressByteSorted; }
        	
        	
        	public SortedDictionary<int, byte[]> GetAllMemoryaddressLineSorted() { return AllMemoryAddressLine; }
        	public SortedDictionary<int, byte[]> GetRAMaddressLineSorted() { return RAMaddressLineSorted; }
        	public SortedDictionary<int, byte[]> GetEEPROMaddressLineSorted() { return EEPROMaddressLineSorted; }
        	public SortedDictionary<int, byte[]> GetOPTIONaddressLineSorted() { return OPTIONaddressLineSorted; }
        	public SortedDictionary<int, byte[]> GetHWaddressLineSorted() { return HWaddressLineSorted; }
        	public SortedDictionary<int, byte[]> GetFLASHaddressLineSorted() { return FLASHaddressLineSorted; }
        	public SortedDictionary<int, byte[]> GetOTHERaddressLineSorted() { return OTHERaddressLineSorted; }
        	
			string fileName;
			string[] errorMessages;

			SortedDictionary<int, byte> AllMemoryAddressByte;
			SortedDictionary<int, byte> RAMaddressByteSorted;
			SortedDictionary<int, byte> EEPROMaddressByteSorted;
			SortedDictionary<int, byte> OPTIONaddressByteSorted;
			SortedDictionary<int, byte> HWaddressByteSorted;
			SortedDictionary<int, byte> FLASHaddressByteSorted;
			SortedDictionary<int, byte> OTHERaddressByteSorted;
			
			SortedDictionary<int, byte[]> AllMemoryAddressLine;
			SortedDictionary<int, byte[]> RAMaddressLineSorted;
			SortedDictionary<int, byte[]> EEPROMaddressLineSorted;
			SortedDictionary<int, byte[]> OPTIONaddressLineSorted;
			SortedDictionary<int, byte[]> HWaddressLineSorted;
			SortedDictionary<int, byte[]> FLASHaddressLineSorted;
			SortedDictionary<int, byte[]> OTHERaddressLineSorted;
			
	}// class FileOpenMemorySorting 
