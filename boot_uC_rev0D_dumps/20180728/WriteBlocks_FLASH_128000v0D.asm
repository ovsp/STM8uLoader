stm8/  TITLE "WriteBlocks_FLASH_128000v0D88.asm"
    MOTOROLA
    WORDS
	.NOLIST
	#include "STM8S103F3P.inc"
    .LIST
; размер дампа должен быть 243 байта
; при этом дамп будет находится в адресах $02ED...$03DF
; (затирает адеса $03DE:$03DF с адресом перехода к начальному загрузчику)
; и вызываться по адресу $02EF
; вызвавший дамп начальный загрузчик находится в адресах $03E0...$03FE
; и повторно может быть вызван по адресу $03E0 (переключит скорость на 9600, надо A<=$0D SP<=$03DF X<=$0000)
; или по адресу $03E8 (скорость не переключит, надо A<=$0D SP<=$03E7 X<=$0000)
; команда перехода к прикладной программе здесь $03FB
	segment byte at 0000-00F2 'ram0'
	dc.w   $02EF
start:
; Включаем pull-up на портах (если подтяжка не предусмотрена внешней схемой) или не включаем, все равно работает, экономим 14 байт
;  ld    A, #%01001100              ; [A6 4C]
;  cpl    A                         ; [43]
;  ld    PA_CR1, A                  ; [C7 50 03]
;  ld    PB_CR1, A                  ; [C7 50 08]
;  ld    PC_CR1, A                  ; [C7 50 0D]
;  ld    PD_CR1, A                  ; [C7 50 12] подтяжка на PD6(UART1_RX), PD2, PD1
; настраиваем UART1 на прием/передачу на скорости 9600, остальные настройки по умолчанию (8 бит, нет бита четности, 1 стоповый бит)
  mov    UART1_BRR2, #0            ; [35 00 52 33]  для Fmaster=16/8=2МГц и 128000
   mov    UART1_BRR1, #1           ; [35 0D 52 32]  для Fmaster=16/8=2МГц и 128000
   mov    UART1_CR2, #%00001100     ; [35 0C 52 35]    UART1_CR2.TEN <- 1  UART1_CR2.REN <- 1  разрешаем передачу/прием
   
main_cycle:
	
wait_byte_adrH_cmnd:
    btjf   UART1_SR, #5, wait_byte_adrH_cmnd
    ld     A, UART1_DR
    cp     A, #$F0
	JRUGE  wait_byte_cmd_test              ; Relative jump if Unsigned Greater or Equal
    ld     XH, A				; старший байт адреса в XH
	ld     UART1_DR, A        			   ; эхо старшего байта адреса
tx_echo_adrH:
	btjf   UART1_SR, #7, tx_echo_adrH	; skip if UART1_SR.TXE = 0		TXE: Transmit data register empty
	
wait_byte_adrL:
    btjf  UART1_SR, #5, wait_byte_adrL
	
    ld     A, UART1_DR
    ld     XL, A				; младший байт адреса в XL
  
wait_byte_cntr:
    btjf   UART1_SR, #5, wait_byte_cntr	; количество блоков
    mov     $03FF, UART1_DR

	
write_block_cycle:
	ldw		Y, #64			; размер блока
	
; unlock FLASH memory  (writing the correct MASS keys)
    mov       FLASH_PUKR, #$56           ; Write $56 then $AE in FLASH_PUKR($5062)
    mov       FLASH_PUKR, #$AE           ; If wrong keys have been entered, another key programming sequence can be issued without resetting the device.
; FLASH Block programming mode    после записи блока последовательность надо повторить
    mov     FLASH_CR2,  #$01     
    mov     FLASH_NCR2, #$FE     ; 
; FLASH Word programming mode    после записи слова последовательность надо повторить
;    mov     FLASH_CR2,  #$40     
;    mov     FLASH_NCR2, #$BF
; else FLASH byte programming
  
; включаем светодиод	
    bset 	PB_DDR,#5 		;
    bset 	PB_CR1,#5 		; 
; при записи блоками нет необходимости создавать буфер
; так как запись начинается только после копирования целого буфера
wait_rx_byte:
    btjf    UART1_SR, #5, wait_rx_byte
    ld      A, UART1_DR
	ld		(X), A
	incw	X
	decw	Y
	jrne	wait_rx_byte

	 mov    UART1_DR, #$FA				; OK
wait_tx_OK_FA:
	btjf   UART1_SR, #7, wait_tx_OK_FA	; skip if UART1_SR.TXE = 0		TXE: Transmit data register empty
; выключаем светодиод	
    bres 	PB_DDR,#5 		;
    bres 	PB_CR1,#5 		; 
	
	dec    $03FF
	jrne	write_block_cycle

    jra     main_cycle
	
; если первый байт $F0 и более
wait_byte_cmd_test:
     cp     A, #$F5						; команда перехода к boot_OPTION
	 jreq    echo_F5cmd
wait_tx_err:
	 mov    UART1_DR, #$F1        ; неизвестная команда
wait_tx_err_F1: 
	btjf	UART1_SR, #7, wait_tx_err_F1	; skip if UART1_SR.TXE = 0		TXE: Transmit data register empty
	 jra    main_cycle
	 
echo_F5cmd:
	ld     UART1_DR, A 					; эхо команды перехода к boot_OPTION
wait_tx_echo_F5cmd:
	btjf	UART1_SR, #7, wait_tx_echo_F5cmd	; skip if UART1_SR.TXE = 0		TXE: Transmit data register empty
	
	
; GoAdr
wait_byte_adrH_toM; 
    btjf   UART1_SR, #5, wait_byte_adrH_toM
    mov    $03E6, UART1_DR
wait_byte_adrL_toM; 
    btjf   UART1_SR, #5, wait_byte_adrL_toM
    mov    $03E7, UART1_DR
; SP
wait_byte_adrH_toSP; 
    btjf   UART1_SR, #5, wait_byte_adrH_toSP
    ld     A, UART1_DR
    ld     XH, A
wait_byte_adrL_toSP; 
    btjf   UART1_SR, #5, wait_byte_adrL_toSP
    ld     A, UART1_DR
    ld     XL, A
	ldw    SP, X
; Y
wait_byte_adrH_toY; 
;    btjf   UART1_SR, #5, wait_byte_adrH_toY
;    ld     A, UART1_DR
;    ld     YH, A
wait_byte_adrL_toY; 
;    btjf   UART1_SR, #5, wait_byte_adrL_toY
;    ld     A, UART1_DR
;    ld     YL, A
; X
wait_byte_adrH_toX; 
    btjf   UART1_SR, #5, wait_byte_adrH_toX
    ld     A, UART1_DR
    ld     XH, A
wait_byte_adrL_toX; 
    btjf   UART1_SR, #5, wait_byte_adrL_toX
    ld     A, UART1_DR
    ld     XL, A
; A
wait_byte_cntr_toA; 
    btjf   UART1_SR, #5, wait_byte_cntr_toA
    ld     A, UART1_DR

	jp     [$03E6.w]
	
	SKIP   28, $00
; ячейки для адреса перехода
	dc.b   $00
	dc.b   $00
	
	end
; WriteBlocks_FLASH_128000v0D88.asm
