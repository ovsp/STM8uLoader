set STM8ASM_DIR=..\..\stm8_asm
set STM8ASM_INCLUDE_DIR=..\..\stm8_asm\include
set PATH=%STM8ASM_DIR%;%STM8ASM_INCLUDE_DIR%
@echo on


:: Assembling STM8S103F3P.asm...
asm -sym -li=list\STM8S103F3P.lsr %STM8ASM_INCLUDE_DIR%\STM8S103F3P.asm -I=%STM8ASM_INCLUDE_DIR%  -obj=obj\STM8S103F3P.obj

:: Assembling boot_FLASH.asm...
asm -sym -li=list\bootFE_rev0D_5A.lsr bootFE_rev0D_5A.asm -I=%STM8ASM_INCLUDE_DIR%  -obj=obj\bootFE_rev0D_5A.obj


:: Running ST linker
lyn obj\STM8S103F3P.obj+obj\bootFE_rev0D_5A.obj, bin\bootFE_rev0D_5A.cod, ; 

:: Creating hex & s19 files
obsend bin\bootFE_rev0D_5A.cod,f,bin\bootFE_rev0D_5A.hex,ix
obsend bin\bootFE_rev0D_5A.cod,f,bin\bootFE_rev0D_5A.s19,s

pause

