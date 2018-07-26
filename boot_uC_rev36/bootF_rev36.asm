stm8/  TITLE “bootF_rev36.asm”
    MOTOROLA
    WORDS
    .NOLIST
    #include "STM8S103F.inc"
    .LIST
	
; ********************************************************
    segment byte at 8000-8003 'RESET_vector'

    pushw   X           ; [89]  SP-- <- RAM_START
    jp    boot_F_start  ; [CC 9F F2]  
  
; ********************************************************
; адреса 0x8004...0x9FF1 свободны для прошивки прикладной программы
    segment byte at 8004 'main_FLASH'
main_flash:
; включаем светодиод	
	bset     PB_DDR,#5 		; 
	bset     PB_CR1,#5 		; 
    jra    *            ; [20 FE]
	 
; ********************************************************
; начальный копировщик boot_FLASH
    segment byte at 9FF2-9FFF 'boot_F'
boot_F_start:
    ldw   X, SP         ; [96]  X <- {RAM_END - }
; Y <- { boot_OPTION_START + RAM_END - 2} 
    ldw     Y, #{$480B + $03FF - 2}   ; [90 AE 4C 08] 
; этот участок кода используют
; boot_FLASH, boot_OPTION и дампы из хост программы
boot_F_copy:
    ld      A, (Y)      ; [90 F6]
    ld      (X), A      ; [F7]
    decw    Y           ; [90 5A]
    decw    X           ; [5A]
; пока X(Y) >= RAM_START(boot_OPTION_START)
    jrpl    boot_F_copy ; [2A F8]
    ret                 ; [81] jump to ++SP
	
    end
; bootF_rev36.asm