stm8/  TITLE  "boot_uC_rev25.asm" ; boot_uC = boot_OPTION + boot_FLASH
    MOTOROLA
    .NOLIST
    #include "STM8S103F3P.inc"
    .LIST
	
	BYTES
; ********************************************************
; отсюда стартуют либо образ прикладной прораммы из EEPROM памяти
; boot_O_exit_address должен быть равен $0000 ( проверяется на <$8000)
; либо прикладная программа из файла прошивки
; адрес передается хост программе вторым аргументом в командной строке
    segment byte at 0000 'boot_O_IMG'
main_ram:
;20FE
    jra    *            ; [20 FE]
	
    WORDS
; ********************************************************
    segment byte at 4800 'boot_O_IMG'
;0000FF00FF00FF00FF00FF350D523235
;0C5235351452315A2716720B5230F8C6
;5231720B5230FB3B52314A26F5965CFC
;AE80042BFA90AE427FAE027FCC9FF600
; содержимое конфигурационных регистров
; $4800 не копируется в RAM
    dc.b    $00, $00, $FF, $00, $FF, $00, $FF, $00, $FF, $00, $FF  	
; OPTION (RAM)
; $480B ($0000)	образ начального загрузчика boot_OPTION
boot_O_start:
; настраиваем UART 96008N1 Fmaster=16/8=2МГц/9600/16
;    mov    UART1_BRR2, #0  ; [35 00 52 33]  исключаем для экономии места
    mov    UART1_BRR1, #13  ; [35 0D 52 32]
; UART1_CR2.TEN <- 1  UART1_CR2.REN <- 1  разрешаем передачу/прием
    mov    UART1_CR2, #%00001100     ; [35 0C 52 35]    
; $4813 ($0008)	
boot_E_byte1_tx:
; отправляем версию $14 загрузчика
    mov    UART1_DR, #$14 ; [35 14 52 31]
; это сигнал хост программе, что можно отправлять данные  
; ждем байт с размером дампа
; регистр X отсчитывает таймаут (примерно 200 миллисекунд)
;    clrw    X          ; [5F] регистр X уже обнулил boot_F
; $4817 ($000C)
boot_O_rx_wait_byte:
    decw  X             ; [5A]  
    jreq  boot_O_exit   ; [27 16] по истечению таймаута выходим из бутлоадера
    btjf  UART1_SR, #5, boot_O_rx_wait_byte  ; [72 OB 52 30 F8]
; первый байт принят, прекращаем отсчитывать таймаут, регистр A используем как счетчик 
; $481F ($0014)	
    ld    A, UART1_DR                  ; [C6 52 31]
; $4822 ($0017)	ждем очередной байт
boot_O_rx_wait_block:
    btjf  UART1_SR, #5, boot_O_rx_wait_block  ; [72 OB 52 30 FB]
    push  UART1_DR      ; [3B 52 31]
    dec  A              ; [4A]
; после каждой итерации в регистре A количество оставшихся для загрузки байтов		
  jrne  boot_O_rx_wait_block       ; [26 F5]
; $482D ($0022)	передаем управление принятому блоку данных
    ldw    X, SP        ; [96]
    incw    X           ; [5C]
boot_O_exit_to_FLASH:
    jp     (X)          ; [FC]
; $4830 ($0025)	передаем управление прикладной программе
boot_O_exit:
    dc.b  $AE  ; ldw X, #boot_O_exit_address ; [AE 80 04]
; $4831 ($0026)	
; адрес передачи управления прикладной программе
boot_O_exit_address:
    dc.w  main_flash    ; [80 04] 
;    dc.w  main_ram     ; [00 00]
    jrmi    boot_O_exit_to_FLASH   ; [2B FA] 
; if X < $8000 адрес пердачи управления равен $0000
; а код находится в EEPROM
boot_O_exit_to_EEPROM:
; Y <- { EEPROM_END}
    ldw     Y, #$427F   ; [90 AE 42 7F]
; X <- { EEPROM_END - EEPROM_START }
; грузим копию EEPROM в RAM
    ldw     X, #$027F   ; [AE 02 7F]
    jp      boot_F_copy ; [CC 9F F4]
; $483F	 ($0034)
    dc.b     $00 ; резервная ячейка
boot_O_end:

; ********************************************************
    segment byte at 8000-8003 'RESET_vector'
;96CC9FF2
    ldw   X, SP         ; [96]  X <- RAM_END
    jp    boot_F_start  ; [CC 9F F2]  
  
; ********************************************************
; адреса 0x8004...0x9FF1 свободны для прошивки прикладной программы
    segment byte at 8004 'main_FLASH'
main_flash:
;20FE
    jra    *            ; [20 FE]

; ********************************************************
; начальный копировщик boot_FLASH
    segment byte at 9FF2-9FFF 'boot_F'
;90AE4C0A90F6F7905A5A2AF85CFC
boot_F_start:
; Y <- { boot_O_START + RAM_END} { $480B + $03FF = $4C0A }
    ldw     Y, #$4C0A   ; [90 AE 4C 0A] 
; этот участок кода используют
; boot_FLASH, boot_OPTION и дампы из хост программы
boot_F_copy:
    ld      A, (Y)      ; [90 F6]
    ld      (X), A      ; [F7]
    decw    Y           ; [90 5A]
    decw    X           ; [5A]
    jrpl    boot_F_copy ; [2A F8] если X(Y) >= RAM_START(boot_O_START)
    incw    X           ; [5C]
    jp     (X)          ; [FC]
  end
;
