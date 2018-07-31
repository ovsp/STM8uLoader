## [STM8uLoader](http://nflic.ru/STM8/STM8uLoader/index.html)

Smallest size UART bootloader for STM8 models without built-in bootloader.

The bootloader code is located in FLASH memory, EEPROM memory and/or in the memory area OPTION Bytes.

STM8uLoader takes the size:

- for rev.0D only 8 bytes in FLASH memory (adr.$8000...$8007) and 36 bytes in EEPROM memory;

- for rev.14 only 20 bytes in FLASH memory (adr.$8000...$8003, $9FF0...$9FFF) and up to 52 bytes in EEPROM memory or in the OPTION Bytes memory area;

- for rev.25 only 18 bytes in FLASH memory (adr.$8000...$8003, $9FF2...$9FFF) and up to 52 bytes in EEPROM memory or in the OPTION Bytes memory area;

- for rev.36 only 18 bytes in FLASH memory (adr.$8000...$8003, $9FF2...$9FFF) and up to 53 bytes in EEPROM memory or in the OPTION Bytes memory area.

STM8uLoader does not occupy or transfer the interrupt vector table (rev.0D only takes the vector $8004 TRAP Software interrupt vector).

STM8uLoader can read the contents of the entire STM8 address space and send to the hostprogram.

STM8uLoader can transfer the code directly from the firmware file (Intel HEX or MOTOROLA S REC) to RAM memory and transfer control to the application program at any RAM memory address specified by the command line argument.

STM8uLoader can write / erase / copy the contents of non-volatile STM8 memory (FLASH, EEPROM, OPTION Byte) in bytes / words(4 bytes) / blocks(64 bytes) from the firmware file (Intel HEX or MOTOROLA S REC) and send to the host program.

STM8uLoader has a 200 millisecond timeout between the RESET event and the transfer of control to the application program.

STM8uLoader can transfer control (without the participation of the host program) of the application pogram to any FLASH memory address.

STM8uLoader can transfer code (without the participation of the host program) from EEPROM memory to RAM memory and transfer control to the application program at any memory RAM address (for rev.36) or at $0000 RAM memory address (for rev.0D, rev.14, rev.25).

STM8uLoader can transfer code (without the participation of the host program) from OPTION Bytes memory area to RAM memory and transfer control to the application program at any memory RAM address (for rev.36) or at $0000 RAM memory address (for rev.0D, rev.14, rev.25).



Code for STM8 in folder:

    boot_uC_rev0D
    
or in folder:
    
    boot_uC_rev14

 or in folder:
    
    boot_uC_rev25

or in folder:
    
    boot_uC_rev36.
    
Code for PC in folder:

    boot_PC
    
---
See also:

 [https://github.com/ovsp/STM8uLoader](https://github.com/ovsp/STM8uLoader)

 [https://sourceforge.net/projects/ovsp](https://sourceforge.net/projects/ovsp)

 [STM8uLoader - компактный STM8 UART загрузчик](http://nflic.ru/STM8/STM8uLoader/index.html)

---
Any questions please in the nflic@bk.ru
