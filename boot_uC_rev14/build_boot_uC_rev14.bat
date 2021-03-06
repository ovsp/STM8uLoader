set STM8ASM_DIR=..\stm8_asm
set STM8ASM_INCLUDE_DIR=..\stm8_asm\include
set PATH=%STM8ASM_DIR%;%STM8ASM_INCLUDE_DIR%
@echo on


:: Assembling STM8S103F3P.asm...
asm -sym -li=list\STM8S103F3P.lsr %STM8ASM_INCLUDE_DIR%\STM8S103F3P.asm -I=%STM8ASM_INCLUDE_DIR%  -obj=obj\STM8S103F3P.obj

:: Assembling boot_FLASH.asm...
asm -sym -li=list\boot_uC_rev14.lsr boot_uC_rev14.asm -I=%STM8ASM_INCLUDE_DIR%  -obj=obj\boot_uC_rev14.obj


:: Running ST linker
lyn obj\STM8S103F3P.obj+obj\boot_uC_rev14.obj, bin\boot_uC_rev14.cod, ; 

:: Creating hex & s19 files
obsend bin\boot_uC_rev14.cod,f,bin\boot_uC_rev14.hex,ix
obsend bin\boot_uC_rev14.cod,f,bin\boot_uC_rev14.s19,s

pause

