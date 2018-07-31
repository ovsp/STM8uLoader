stm8/ TITLE "Read_128000v0D.asm"
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
    btjf  UART1_SR, #5, wait_byte_cntr
	
    ld     A, #$00
    ld     YH, A				;
    ld     A, UART1_DR
    ld     YL, A				; счетчик
  
; включаем светодиод	
    bset 	PB_DDR,#5 		;
    bset 	PB_CR1,#5 		; 
; 
read_block_cycle:
	ld		A, (X)
    ld     UART1_DR, A
wait_tx:
	btjf	UART1_SR, #7, wait_tx	; skip if UART1_SR.TXE = 0		TXE: Transmit data register empty
	incw	X
	decw	Y
	jrne	read_block_cycle
	
; выключаем светодиод	
    bres 	PB_DDR,#5 		;
    bres 	PB_CR1,#5 		; 

    jra     main_cycle
	
; если первый байт $F0 и более
wait_byte_cmd_test:
     cp     A, #$F5						; команда перехода
	 jreq    echo_F5cmd
wait_tx_err:
	 mov    UART1_DR, #$F1        ; неизвестная команда
	btjf	UART1_SR, #7, wait_tx_err	; skip if UART1_SR.TXE = 0		TXE: Transmit data register empty
	 jra    main_cycle
	 
echo_F5cmd:
	ld     UART1_DR, A 					; эхо команды перехода
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
	
	SKIP   59, $00
; ячейки для адреса перехода
	dc.b   $00
	dc.b   $00
	
	end
; Read_128000v0D.asm