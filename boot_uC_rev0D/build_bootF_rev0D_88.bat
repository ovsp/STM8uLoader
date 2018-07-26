set STM8ASM_DIR=..\stm8_asm
set STM8ASM_INCLUDE_DIR=..\stm8_asm\include
set PATH=%STM8ASM_DIR%;%STM8ASM_INCLUDE_DIR%
@echo on


:: Assembling STM8S103F3P.asm...
asm -sym -li=list\STM8S103F3P.lsr %STM8ASM_INCLUDE_DIR%\STM8S103F3P.asm -I=%STM8ASM_INCLUDE_DIR%  -obj=obj\STM8S103F3P.obj

:: Assembling boot_FLASH.asm...
asm -sym -li=list\bootF_rev0D_88.lsr bootF_rev0D_88.asm -I=%STM8ASM_INCLUDE_DIR%  -obj=obj\bootF_rev0D_88.obj


:: Running ST linker
lyn obj\STM8S103F3P.obj+obj\bootF_rev0D_88.obj, bin\bootF_rev0D_88.cod, ; 

:: Creating hex & s19 files
obsend bin\bootF_rev0D_88.cod,f,bin\bootF_rev0D_88.hex,ix
obsend bin\bootF_rev0D_88.cod,f,bin\bootF_rev0D_88.s19,s

pause

