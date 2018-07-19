stm8/

; STM8S103F.asm

; Copyright (c) 2003-2016 STMicroelectronics

; STM8S103F

	BYTES		; following addresses are 8-bit length

	segment byte at 0-7F 'periph'


	WORDS		; following addresses are 16-bit length

	segment byte at 5000 'periph2'


; Port A at 0x5000
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
.PA_ODR			DS.B 1		; Port A data output latch register
.PA_IDR			DS.B 1		; Port A input pin value register
.PA_DDR			DS.B 1		; Port A data direction register
.PA_CR1			DS.B 1		; Port A control register 1
.PA_CR2			DS.B 1		; Port A control register 2

; Port B at 0x5005
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
.PB_ODR			DS.B 1		; Port B data output latch register
.PB_IDR			DS.B 1		; Port B input pin value register
.PB_DDR			DS.B 1		; Port B data direction register
.PB_CR1			DS.B 1		; Port B control register 1
.PB_CR2			DS.B 1		; Port B control register 2

; Port C at 0x500a
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
.PC_ODR			DS.B 1		; Port C data output latch register
.PC_IDR			DS.B 1		; Port C input pin value register
.PC_DDR			DS.B 1		; Port C data direction register
.PC_CR1			DS.B 1		; Port C control register 1
.PC_CR2			DS.B 1		; Port C control register 2

; Port D at 0x500f
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
.PD_ODR			DS.B 1		; Port D data output latch register
.PD_IDR			DS.B 1		; Port D input pin value register
.PD_DDR			DS.B 1		; Port D data direction register
.PD_CR1			DS.B 1		; Port D control register 1
.PD_CR2			DS.B 1		; Port D control register 2
reserved1		DS.B 70		; unused

; Flash at 0x505a
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
.FLASH_CR1			DS.B 1		; Flash control register 1
.FLASH_CR2			DS.B 1		; Flash control register 2
.FLASH_NCR2			DS.B 1		; Flash complementary control register 2
.FLASH_FPR			DS.B 1		; Flash protection register
.FLASH_NFPR			DS.B 1		; Flash complementary protection register
.FLASH_IAPSR			DS.B 1		; Flash in-application programming status register
reserved2		DS.B 2		; unused
.FLASH_PUKR			DS.B 1		; Flash Program memory unprotection register
reserved3		DS.B 1		; unused
.FLASH_DUKR			DS.B 1		; Data EEPROM unprotection register
reserved4		DS.B 59		; unused

; External Interrupt Control Register (ITC) at 0x50a0
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
.EXTI_CR1			DS.B 1		; External interrupt control register 1
.EXTI_CR2			DS.B 1		; External interrupt control register 2
reserved5		DS.B 17		; unused

; Reset (RST) at 0x50b3
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
.RST_SR			DS.B 1		; Reset status register 1
reserved6		DS.B 12		; unused

; Clock Control (CLK) at 0x50c0
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
.CLK_ICKR			DS.B 1		; Internal clock control register
.CLK_ECKR			DS.B 1		; External clock control register
reserved7		DS.B 1		; unused
.CLK_CMSR			DS.B 1		; Clock master status register
.CLK_SWR			DS.B 1		; Clock master switch register
.CLK_SWCR			DS.B 1		; Clock switch control register
.CLK_CKDIVR			DS.B 1		; Clock divider register
.CLK_PCKENR1			DS.B 1		; Peripheral clock gating register 1
.CLK_CSSR			DS.B 1		; Clock security system register
.CLK_CCOR			DS.B 1		; Configurable clock control register
.CLK_PCKENR2			DS.B 1		; Peripheral clock gating register 2
.CLK_CANCCR			DS.B 1		; CAN clock control register
.CLK_HSITRIMR			DS.B 1		; HSI clock calibration trimming register
.CLK_SWIMCCR			DS.B 1		; SWIM clock control register
reserved8		DS.B 3		; unused

; Window Watchdog (WWDG) at 0x50d1
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
.WWDG_CR			DS.B 1		; WWDG Control Register
.WWDG_WR			DS.B 1		; WWDR Window Register
reserved9		DS.B 13		; unused

; Independent Watchdog (IWDG) at 0x50e0
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
.IWDG_KR			DS.B 1		; IWDG Key Register
.IWDG_PR			DS.B 1		; IWDG Prescaler Register
.IWDG_RLR			DS.B 1		; IWDG Reload Register
reserved10		DS.B 13		; unused

; Auto Wake-Up (AWU) at 0x50f0
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
.AWU_CSR			DS.B 1		; AWU Control/Status Register
.AWU_APR			DS.B 1		; AWU asynchronous prescaler buffer register
.AWU_TBR			DS.B 1		; AWU Timebase selection register

; Beeper (BEEP) at 0x50f3
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
.BEEP_CSR			DS.B 1		; BEEP Control/Status Register
reserved11		DS.B 268		; unused

; Serial Peripheral Interface (SPI) at 0x5200
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
.SPI_CR1			DS.B 1		; SPI Control Register 1
.SPI_CR2			DS.B 1		; SPI Control Register 2
.SPI_ICR			DS.B 1		; SPI Interrupt Control Register
.SPI_SR			DS.B 1		; SPI Status Register
.SPI_DR			DS.B 1		; SPI Data Register
.SPI_CRCPR			DS.B 1		; SPI CRC Polynomial Register
.SPI_RXCRCR			DS.B 1		; SPI Rx CRC Register
.SPI_TXCRCR			DS.B 1		; SPI Tx CRC Register
reserved12		DS.B 8		; unused

; I2C Bus Interface (I2C) at 0x5210
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
.I2C_CR1			DS.B 1		; I2C control register 1
.I2C_CR2			DS.B 1		; I2C control register 2
.I2C_FREQR			DS.B 1		; I2C frequency register
.I2C_OARL			DS.B 1		; I2C Own address register low
.I2C_OARH			DS.B 1		; I2C Own address register high
reserved13		DS.B 1		; unused
.I2C_DR			DS.B 1		; I2C data register
.I2C_SR1			DS.B 1		; I2C status register 1
.I2C_SR2			DS.B 1		; I2C status register 2
.I2C_SR3			DS.B 1		; I2C status register 3
.I2C_ITR			DS.B 1		; I2C interrupt control register
.I2C_CCRL			DS.B 1		; I2C Clock control register low
.I2C_CCRH			DS.B 1		; I2C Clock control register high
.I2C_TRISER			DS.B 1		; I2C TRISE register
.I2C_PECR			DS.B 1		; I2C packet error checking register
reserved14		DS.B 17		; unused

; Universal synch/asynch receiver transmitter (UART1) at 0x5230
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
.UART1_SR			DS.B 1		; UART1 Status Register
.UART1_DR			DS.B 1		; UART1 Data Register
.UART1_BRR1			DS.B 1		; UART1 Baud Rate Register 1
.UART1_BRR2			DS.B 1		; UART1 Baud Rate Register 2
.UART1_CR1			DS.B 1		; UART1 Control Register 1
.UART1_CR2			DS.B 1		; UART1 Control Register 2
.UART1_CR3			DS.B 1		; UART1 Control Register 3
.UART1_CR4			DS.B 1		; UART1 Control Register 4
.UART1_CR5			DS.B 1		; UART1 Control Register 5
.UART1_GTR			DS.B 1		; UART1 Guard time Register
.UART1_PSCR			DS.B 1		; UART1 Prescaler Register
reserved15		DS.B 21		; unused

; 16-Bit Timer 1 (TIM1) at 0x5250
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
.TIM1_CR1			DS.B 1		; TIM1 Control register 1
.TIM1_CR2			DS.B 1		; TIM1 Control register 2
.TIM1_SMCR			DS.B 1		; TIM1 Slave Mode Control register
.TIM1_ETR			DS.B 1		; TIM1 external trigger register
.TIM1_IER			DS.B 1		; TIM1 Interrupt enable register
.TIM1_SR1			DS.B 1		; TIM1 Status register 1
.TIM1_SR2			DS.B 1		; TIM1 Status register 2
.TIM1_EGR			DS.B 1		; TIM1 Event Generation register
.TIM1_CCMR1			DS.B 1		; TIM1 Capture/Compare mode register 1
.TIM1_CCMR2			DS.B 1		; TIM1 Capture/Compare mode register 2
.TIM1_CCMR3			DS.B 1		; TIM1 Capture/Compare mode register 3
.TIM1_CCMR4			DS.B 1		; TIM1 Capture/Compare mode register 4
.TIM1_CCER1			DS.B 1		; TIM1 Capture/Compare enable register 1
.TIM1_CCER2			DS.B 1		; TIM1 Capture/Compare enable register 2
.TIM1_CNTRH			DS.B 1		; Data bits High
.TIM1_CNTRL			DS.B 1		; Data bits Low
.TIM1_PSCRH			DS.B 1		; Data bits High
.TIM1_PSCRL			DS.B 1		; Data bits Low
.TIM1_ARRH			DS.B 1		; Data bits High
.TIM1_ARRL			DS.B 1		; Data bits Low
.TIM1_RCR			DS.B 1		; TIM1 Repetition counter register
.TIM1_CCR1H			DS.B 1		; Data bits High
.TIM1_CCR1L			DS.B 1		; Data bits Low
.TIM1_CCR2H			DS.B 1		; Data bits High
.TIM1_CCR2L			DS.B 1		; Data bits Low
.TIM1_CCR3H			DS.B 1		; Data bits High
.TIM1_CCR3L			DS.B 1		; Data bits Low
.TIM1_CCR4H			DS.B 1		; Data bits High
.TIM1_CCR4L			DS.B 1		; Data bits Low
.TIM1_BKR			DS.B 1		; TIM1 Break register
.TIM1_DTR			DS.B 1		; TIM1 Dead-time register
.TIM1_OISR			DS.B 1		; TIM1 Output idle state register
reserved16		DS.B 144		; unused

; 16-Bit Timer 2 (TIM2) at 0x5300
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
.TIM2_CR1			DS.B 1		; TIM2 Control register 1
reserved17		DS.B 2		; unused
.TIM2_IER			DS.B 1		; TIM2 Interrupt enable register
.TIM2_SR1			DS.B 1		; TIM2 Status register 1
.TIM2_SR2			DS.B 1		; TIM2 Status register 2
.TIM2_EGR			DS.B 1		; TIM2 Event Generation register
.TIM2_CCMR1			DS.B 1		; TIM2 Capture/Compare mode register 1
.TIM2_CCMR2			DS.B 1		; TIM2 Capture/Compare mode register 2
.TIM2_CCMR3			DS.B 1		; TIM2 Capture/Compare mode register 3
.TIM2_CCER1			DS.B 1		; TIM2 Capture/Compare enable register 1
.TIM2_CCER2			DS.B 1		; TIM2 Capture/Compare enable register 2
.TIM2_CNTRH			DS.B 1		; Data bits High
.TIM2_CNTRL			DS.B 1		; Data bits Low
.TIM2_PSCR			DS.B 1		; TIM2 Prescaler register
.TIM2_ARRH			DS.B 1		; Data bits High
.TIM2_ARRL			DS.B 1		; Data bits Low
.TIM2_CCR1H			DS.B 1		; Data bits High
.TIM2_CCR1L			DS.B 1		; Data bits Low
.TIM2_CCR2H			DS.B 1		; Data bits High
.TIM2_CCR2L			DS.B 1		; Data bits Low
.TIM2_CCR3H			DS.B 1		; Data bits High
.TIM2_CCR3L			DS.B 1		; Data bits Low
reserved18		DS.B 41		; unused

; 8-Bit  Timer 4 (TIM4) at 0x5340
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
.TIM4_CR1			DS.B 1		; TIM4 Control register 1
reserved19		DS.B 2		; unused
.TIM4_IER			DS.B 1		; TIM4 Interrupt enable register
.TIM4_SR			DS.B 1		; TIM4 Status register
.TIM4_EGR			DS.B 1		; TIM4 Event Generation register
.TIM4_CNTR			DS.B 1		; TIM4 Counter
.TIM4_PSCR			DS.B 1		; TIM4 Prescaler register
.TIM4_ARR			DS.B 1		; TIM4 Auto-reload register
reserved20		DS.B 151		; unused

; 10-Bit A/D Converter (ADC1) at 0x53e0
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
.ADC_DB0RH			DS.B 1		; Data Buffer register 0 High
.ADC_DB0RL			DS.B 1		; Data Buffer register 0 Low
.ADC_DB1RH			DS.B 1		; Data Buffer register 1 High
.ADC_DB1RL			DS.B 1		; Data Buffer register 1 Low
.ADC_DB2RH			DS.B 1		; Data Buffer register 2 High
.ADC_DB2RL			DS.B 1		; Data Buffer register 2 Low
.ADC_DB3RH			DS.B 1		; Data Buffer register 3 High
.ADC_DB3RL			DS.B 1		; Data Buffer register 3 Low
.ADC_DB4RH			DS.B 1		; Data Buffer register 4 High
.ADC_DB4RL			DS.B 1		; Data Buffer register 4 Low
.ADC_DB5RH			DS.B 1		; Data Buffer register 5 High
.ADC_DB5RL			DS.B 1		; Data Buffer register 5 Low
.ADC_DB6RH			DS.B 1		; Data Buffer register 6 High
.ADC_DB6RL			DS.B 1		; Data Buffer register 6 Low
.ADC_DB7RH			DS.B 1		; Data Buffer register 7 High
.ADC_DB7RL			DS.B 1		; Data Buffer register 7 Low
.ADC_DB8RH			DS.B 1		; Data Buffer register 8 High
.ADC_DB8RL			DS.B 1		; Data Buffer register 8 Low
.ADC_DB9RH			DS.B 1		; Data Buffer register 9 High
.ADC_DB9RL			DS.B 1		; Data Buffer register 9 Low
reserved21		DS.B 12		; unused
.ADC_CSR			DS.B 1		; ADC Control/Status Register
.ADC_CR1			DS.B 1		; ADC Configuration Register 1
.ADC_CR2			DS.B 1		; ADC Configuration Register 2
.ADC_CR3			DS.B 1		; ADC Configuration Register 3
.ADC_DRH			DS.B 1		; Data bits High
.ADC_DRL			DS.B 1		; Data bits Low
.ADC_TDRH			DS.B 1		; Schmitt trigger disable High
.ADC_TDRL			DS.B 1		; Schmitt trigger disable Low
.ADC_HTRH			DS.B 1		; High Threshold Register High
.ADC_HTRL			DS.B 1		; High Threshold Register Low
.ADC_LTRH			DS.B 1		; Low Threshold Register High
.ADC_LTRL			DS.B 1		; Low Threshold Register Low
.ADC_AWSRH			DS.B 1		; Analog Watchdog Status register High
.ADC_AWSRL			DS.B 1		; Analog Watchdog Status register Low
.ADC_AWCRH			DS.B 1		; Analog Watchdog Control register High
.ADC_AWCRL			DS.B 1		; Analog Watchdog Control register Low
reserved22		DS.B 11088		; unused

;  Global configuration register (CFG) at 0x7f60
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
.CFG_GCR			DS.B 1		; CFG Global configuration register
reserved23		DS.B 15		; unused

; Interrupt Software Priority Register (ITC) at 0x7f70
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
.ITC_SPR1			DS.B 1		; Interrupt Software priority register 1
.ITC_SPR2			DS.B 1		; Interrupt Software priority register 2
.ITC_SPR3			DS.B 1		; Interrupt Software priority register 3
.ITC_SPR4			DS.B 1		; Interrupt Software priority register 4
.ITC_SPR5			DS.B 1		; Interrupt Software priority register 5
.ITC_SPR6			DS.B 1		; Interrupt Software priority register 6
.ITC_SPR7			DS.B 1		; Interrupt Software priority register 7

	end
