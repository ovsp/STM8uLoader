set STM8ASM_DIR=..\stm8_asm
set STM8ASM_INCLUDE_DIR=..\stm8_asm\include
set PATH=%STM8ASM_DIR%;%STM8ASM_INCLUDE_DIR%
@echo on


:: Assembling STM8S103F3P.asm...
asm -sym -li=list\STM8S103F3P.lsr %STM8ASM_INCLUDE_DIR%\STM8S103F3P.asm -I=%STM8ASM_INCLUDE_DIR%  -obj=obj\STM8S103F3P.obj

:: Assembling boot_FLASH.asm...
asm -sym -li=list\bootF_rev0D_5A.lsr bootF_rev0D_5A.asm -I=%STM8ASM_INCLUDE_DIR%  -obj=obj\bootF_rev0D_5A.obj


:: Running ST linker
lyn obj\STM8S103F3P.obj+obj\bootF_rev0D_5A.obj, bin\bootF_rev0D_5A.cod, ; 

:: Creating hex & s19 files
obsend bin\bootF_rev0D_5A.cod,f,bin\bootF_rev0D_5A.hex,ix
obsend bin\bootF_rev0D_5A.cod,f,bin\bootF_rev0D_5A.s19,s

pause

