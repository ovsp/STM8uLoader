stm8/  TITLE “bootF_rev0D_5A.asm”
    MOTOROLA
    WORDS
    .NOLIST
    #include "STM8S103F.inc"
    .LIST

    segment byte at 404E-405C 'bootE_rev0D_5A'
;    segment byte at 414D-415C 'bootE_rev0D_5A'	
;    segment byte at 424D-425C 'bootE_rev0D_5A'
; $404E terminator не переносится в RAM
	dc.b   $00        ; [00]
; $404F ($03F4)
; {$03FF - (bootE_go_adr - bootE_start) }
    dc.w   $03F5      ; [03 F5]
bootE_start:          
; $4051 ($03F5)       
    bset   PB_DDR,#5  ; [72 1A 50 07]
    bset   PB_CR1,#5  ; [72 1A 50 08]
; $4059 ($03FD) {RAM END - 2}
    jra    *          ; [20 FE]   {RAM END - 1}
; $405B не переносится в RAM
bootE_go_adr:
    dc.w   main_flash ; [80 08]

; копировщик boot_FLASH версия $0D $88 
    segment byte at 8000-8007 'bootF_rev0D'
; $8000 RESET Reset vector
    dc.b   $AE, $40  ; [AE 40] ldw X,#$405A
;    dc.b   $AE, $41  ; [AE 41] ldw X,#$415A
;    dc.b   $AE, $42  ; [AE 42] ldw X,#$425A
cycle:
    decw   X         ; [5A]
    push   A         ; [88]
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
; bootF_rev0D_5A.asm