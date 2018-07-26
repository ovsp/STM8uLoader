stm8/  TITLE “bootF_rev0D_88.asm”
    MOTOROLA
    WORDS
    .NOLIST
    #include "STM8S103F.inc"
    .LIST

    segment byte at 407B-4089 'bootE_rev0D_88'
;    segment byte at 417B-4189 'bootE_rev0D_88'
; $407B terminator не переносится в RAM
	dc.b   $00        ; [00]
; $407C ($03F4)
; {$0400 - (bootE_go_adr - bootE_start) }
    dc.w   $03F6      ; [03 F6]
bootE_start:          
; $407E ($03F6)       
    bset   PB_DDR,#5  ; [72 1A 50 07]
    bset   PB_CR1,#5  ; [72 1A 50 08]
; $4086 ($03FE) {RAM END - 1}
    jra    *          ; [20 FE]   RAM END
; $4088 не переносится в RAM
bootE_go_adr:
    dc.w   main_flash ; [80 08]

; копировщик boot_FLASH версия $0D $88 
    segment byte at 8000-8007 'bootF_rev0D'
; $8000 RESET Reset vector
    dc.b   $AE, $40  ; [AE 40] ldw X,#$4088
;    dc.b   $AE, $41  ; [AE 41] ldw X,#$4188
cycle:
    push   A         ; [88]
    decw   X         ; [5A]
; $8004 TRAP Software interrupt
    ld     A, (X)    ; [F6]
    jrne   cycle     ; [26 FB] 
    ret              ; [81]
; $8008 TLI IRQ0 External top level interrupt

; прикладная программа 
    segment byte at 8008-9FFF 'main_flash'
main_flash:
    jra    *          ; [20 FE]

    end
; bootF_rev0D_88.asm