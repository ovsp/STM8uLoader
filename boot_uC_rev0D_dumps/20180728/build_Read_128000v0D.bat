set STM8ASM_DIR=D:\stm\_toolchain\stm8_asm
set STM8ASM_INCLUDE_DIR=D:\stm\_toolchain\stm8_asm\include
set PATH=%STM8ASM_DIR%;%STM8ASM_INCLUDE_DIR%
@echo on


:: Assembling STM8S103F3P.asm...
asm -sym -li=list\STM8S103F3P.lsr %STM8ASM_INCLUDE_DIR%\STM8S103F3P.asm -I=%STM8ASM_INCLUDE_DIR%  -obj=obj\STM8S103F3P.obj

:: Assembling boot_FLASH.asm...
asm -sym -li=list\Read_128000v0D.lsr Read_128000v0D.asm -I=%STM8ASM_INCLUDE_DIR%  -obj=obj\Read_128000v0D.obj


:: Running ST linker
lyn obj\STM8S103F3P.obj+obj\Read_128000v0D.obj, bin\Read_128000v0D.cod, ; 

:: Creating hex & s19 files
obsend bin\Read_128000v0D.cod,f,bin\Read_128000v0D.hex,ix
obsend bin\Read_128000v0D.cod,f,bin\Read_128000v0D.s19,s

pause

