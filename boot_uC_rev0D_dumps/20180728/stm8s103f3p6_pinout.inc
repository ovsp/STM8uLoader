; stm8s103f3p6_pinout.inc
;
; STM8S103F3P6
; STM8	- Product class			STM8 microcontroller
; S		- Family type			standart
; 103	- Sub-family type		access line
; F		- Pin count				20 pins
; 3		- Program memory size	8 Kbytes
; P		- Case type				TSSOP
; 6								-40...+85 C
;                                 _ _ _ _ _ _ _ _ _ _ _ _  _ _ _ _ _ _ _ _ _ _ _ _
;                              __|                       \/                       |__ 
;            PD4  <-----------|__| PD4 (HS) TIM2_CH1            TIM2_CH2 (HS) PD3 |__|-----------> PD3
;                              1 |    UART1_CK  BEEP            AIN4   ADC_ETR    |20 
;                              __|                                                |__ 
;            PD5  <-----------|__| PD5 (HS) AIN5                AIN3     (HS) PD2 |__|-----------> PD2
;                              2 |    UART1_TX                  [TIM2_CH3]        |19 
;                              __|                                                |__  SWIM
;            PD6  <-----------|__| PD6 (HS) AIN6                SWIM     (HS) PD1 |__|---'-------> PD1
;                              3 |    UART1_RX                                    |18 
;                        NRST   _|                                                |__
;            NRST <-------'---|__| NRST                         SPI_MISO (HS) PC7 |__|-----------> PC7
;                              4 |                              [TIM1_CH2]        |17
;                              __|                                                |__
;            PA1  <-----------|__| PA1  OSCIN                   SPI_MOSI (HS) PC6 |__|-----------> PC6
;                              5 |                              [TIM1_CH1]        |16         					    3.3 V            3.3 V
;                              __|                                                |__                                 |                |
;            PA2  <-----------|__| PA2  OSCOUT                   SPI_SCK (HS) PC5 |__|-----------> PC5                |                |
;                              6 |                              [TIM2_CH1]        |15                                ---              ---
;                       GND    __|                                                |__                               |   |            |   |
;            GND  <------'----|__| Vss                     AIN2  CLK_CCO (HS) PC4 |__|-----------> PC4              |   |            |   |
;                              7 |                        [TIM1_CH2N]  TIM1_CH4   |14                               |   |            |   |
;                       5 V    __|                                                |__                                ---              ---
;            5 V  <------'    |__| Vcap                         TIM1_CH3 (HS) PC3 |__|-----------> PC3                |                |
;                              8 |                              [TIM1_CH1N] [TLI] |13 							    -----            -----
;                       3.3V   __|                                                |__ 							    \   / ->  RED    \   / ->  BLUE
;           3.3 V <------'----|__| Vdd                  [ADC_ETR] I2C_SCL (T) PB4 |__|-----------> PB4			     \ /  ->  LED     \ /  ->  LED
;                              9 |                                                |12                               -----            -----
;                              __|    [SPI_NSS]                 [TIM1_BKN]        |__   PB5                           |                |
;            PA3  <-----------|__| PA3 (HS) TIM_CH3             I2C_SDA   (T) PB5 |__|---'-------> PB5               PB5              GND
;                              10|_ _ _ _ _ _ _ _ _ _ _ __ _ _ _ _ _ _ _ _ _ _ _ _|11 
;                                                   STM8S103F3P6   
;  (HS)  high sink capability
;   (T)  true open drain (P-buffer and protection diode to VDD not implemented)
;    []  alternate function remapping option (if the same alternate function is shown twice,
;        it indicate choise not a duplication of the function)
