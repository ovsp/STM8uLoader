stm8/  TITLE “bootF_rev0D_5A.asm”
    MOTOROLA
    WORDS
    .NOLIST
;    #include "STM8S003F.inc"
    #include "STM8S103F.inc"
    .LIST

; начальный загрузчик boot_EEPROM версия $0D($5A)
    segment byte at 4039-405C 'bootE_rev0D_405A' ; STM8S003F
;    segment byte at 4139-415C 'bootE_rev0D_415A'
;    segment byte at 4239-425C 'bootE_rev0D_425A'
; $403A9 ($03DD) {RAM END - 34} terminator переносится в RAM   ?????
	dc.b   $00        ; [00]
; $403A ($03DE) {RAM END - 33}
; {$0400 - (bootE_go_adr - bootE_start - 1) }
    dc.w   $03E0      ; [03 E0]
; настраивает UART 96001N8
; отправляет BREAK и номер версии $0D
bootE_start:          
; $403C ($03E0) {RAM END - 31}       
	ld    A, #%00001101 ; [A6 0D]
	ld    UART1_BRR1, A ; [C7 52 32]
	ld    UART1_CR2, A  ; [C7 52 35]
; $4044 ($03E8) {RAM END - 23} 
	ld    UART1_DR, A   ; [C7 52 31]
; принимает дамп размером 243 байта
; и помещаеn его в стек	
; $40487 ($03EB) {RAM END - 20}   
bootE_rx_byte:
    incw  X           ; [5C]
    jreq  bootE_exit  ; [27 0C]
    btjf  UART1_SR, #5, bootE_rx_byte ; [72 0B 52 30 F8]
	push  UART1_DR    ; [3B 52 31]
	clrw  X           ; [5F]
	inc   A           ; [4C]
	jrne  bootE_rx_byte ; [26 F2]
; передает управление принятому дампу
; по адресу $02EF (последние два байта в дампе $EF $02)
; $4056 ($03FA) {RAM END - 5}
    ret              ; [81]
; $4057 ($03FB) {RAM END - 4}
bootE_exit:
	jp   [bootE_go_adr] ; [72 CC 40 5B] {RAM END - 1}
; ($03FF) RAM END  может использоваться текущим дампом
; адрес передачи управления прикладной программе
; $405B не переносится в RAM
bootE_go_adr:
    dc.w   main_flash ; [80 08]

	
; начальный копировщик boot_FLASH версия $0D($5A) 
; копирует код начиная с конечного адреса $405A($415A,$425A)
; в стек до первого нулевого байта $00
; и передает управление коду
    segment byte at 8000-8007 'bootF_rev0D'
; $8000 RESET Reset vector
    dc.b   $AE, $40  ; [AE 40 5A] ldw X,#$405A ; STM8S003F
;    dc.b   $AE, $41  ; [AE 41 5A] ldw X,#$415A
;    dc.b   $AE, $42  ; [AE 42 5A] ldw X,#$425A
cycle:
; при первом вхождении в цикл
; команда dec X затеняется командой ldw X,#$405A($415A,$425A)
    decw   X         ; [5A]
    push   A         ; [88]
; $8004 TRAP Software interrupt
    ld     A, (X)    ; [F6]
    jrne   cycle     ; [26 FB]
    ret              ; [81]		
; $8008 TLI IRQ0 External top level interrupt

; прикладная программа 
    segment byte at 8008 'main_flash'
main_flash:
    jra    *          ; [20 FE]

;    end
; bootF_rev0D_5A.asm