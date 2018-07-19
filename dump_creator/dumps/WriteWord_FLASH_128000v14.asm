stm8/

	TITLE "WriteWord_FLASH_128000v14.asm"
	MOTOROLA

	#include "STM8S103F3P.inc"

	BYTES
	segment byte at 0000 'ram0'
	
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
    cp     A, #$EF
	JRUGT  wait_byte_cmd_test              ; Relative jump if Unsigned Greater Than
    ld     XH, A				; старший байт адреса в XH
	ld     UART1_DR, A        			   ; эхо старшего байта адреса
tx_echo_adrH:
	btjf   UART1_SR, #7, tx_echo_adrH	; skip if UART1_SR.TXE = 0		TXE: Transmit data register empty
	
wait_byte_adrL:
    btjf  UART1_SR, #5, wait_byte_adrL
	
    ld     A, UART1_DR
    ld     XL, A				; младший байт адреса в XL
  
wait_byte_cntr:
    btjf   UART1_SR, #5, wait_byte_cntr	; размер блока для записи
	ldw		Y, #$0000			; размер блока
    ld     A, UART1_DR
    ld     YL, A				;
  
; включаем светодиод	
    bset 	PB_DDR,#5 		;
    bset 	PB_CR1,#5 		; 

; unlock EEPROM memory  (writing the correct MASS keys)
    mov       FLASH_PUKR, #$56           ; Write $56 then $AE in FLASH_PUKR($5062)
    mov       FLASH_PUKR, #$AE           ; If wrong keys have been entered, another key programming sequence can be issued without resetting the device.
    mov       FLASH_CR2,  #$40 
    mov       FLASH_NCR2, #$BF 
	
write_block_cycle:
wait_rx_byte:
    btjf    UART1_SR, #5, wait_rx_byte
    ld      A, UART1_DR
	ld		(X), A
	incw	X
	decw	Y
	jrne	write_block_cycle

	 mov    UART1_DR, #$FA				; OK
wait_tx_OK_FA:
	btjf   UART1_SR, #7, wait_tx_OK_FA	; skip if UART1_SR.TXE = 0		TXE: Transmit data register empty
	
; выключаем светодиод	
    bres 	PB_DDR,#5 		;
    bres 	PB_CR1,#5 		; 

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
wait_byte_adrH_toY; 
    btjf   UART1_SR, #5, wait_byte_adrH_toY
    ld     A, UART1_DR
    ld     YH, A
wait_byte_adrL_toY; 
    btjf   UART1_SR, #5, wait_byte_adrL_toY
    ld     A, UART1_DR
    ld     YL, A

; индексные регистры заняты придется использовать указатель на память
wait_byte_adrH_toM; 
    btjf   UART1_SR, #5, wait_byte_adrH_toM
    mov    $03FE, UART1_DR
wait_byte_adrL_toM; 
    btjf   UART1_SR, #5, wait_byte_adrL_toM
    mov    $03FF, UART1_DR
	
wait_byte_cntr_toA; 
    btjf   UART1_SR, #5, wait_byte_cntr_toA
    ld     A, UART1_DR
	
	ldw    X, #$03FF
	ldw    SP, X
	
	jp     [$03FE.w]
	dc.b   $00
	dc.b   $00
	end
; WriteWord_FLASH_128000v14.asm
