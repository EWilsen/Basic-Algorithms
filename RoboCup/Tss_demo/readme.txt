/******************** (C) COPYRIGHT 2013 Freescale ********************
* File Name          : Readme.txt
* Author             : Freescale
* Date First Issued  : 04/12/2013 : Version 1.0
* Description        : Description of the GPIO method TSS sample.
********************************************************************************
* History:
* 04/15/2013 : Version 1.0
********************************************************************************
* THE PRESENT SOFTWARE WHICH IS FOR GUIDANCE ONLY AIMS AT PROVIDING CUSTOMERS
* WITH CODING INFORMATION REGARDING THEIR PRODUCTS IN ORDER FOR THEM TO SAVE TIME.
* AS A RESULT, FREESCALE SHALL NOT BE HELD LIABLE FOR ANY DIRECT,
* INDIRECT OR CONSEQUENTIAL DAMAGES WITH RESPECT TO ANY CLAIMS ARISING FROM THE
* CONTENT OF SUCH SOFTWARE AND/OR THE USE MADE BY CUSTOMERS OF THE CODING
* INFORMATION CONTAINED HEREIN IN CONNECTION WITH THEIR PRODUCTS.
*******************************************************************************/

Example description
===================
The TSSW doesn't not support KE series now, this example provides a description of how  to integrate the TSSW 3.0 into the KE02 
project, and besides show the touch sensor by changing the LEDs brightness on the FRDM.

Take care of these modifications:
1. Add #define TSS_KINETIS_MCU  1      in TSS_SystemSetup.h file. Or add define MCU_MKE02 in TSS_Sensor.h to detecte the mcu 
2. Add #define TSS_HW_TIMER     FTM0   in TSS_SystemSetup.h file.
3. Add GPIO method for PTD6 and PTD7 in TSS_SystemSetup.h file.
4. Modify DelayMS(UINT16 u16delay) function in app_init.c file.
5. Modify TSS_fOnInit(void) function in events.c file.
6. Move TSS1_fCallBack1(TSS_CONTROL_ID u8ControlId) function from events.c file to tss_demo.c file.
7. Add "derivative.h" in the prject(compatable with KL)
8. Modify the PORTA_BASE_PTR init not included in KE02 in TSS_SensorGPIO.c file.
    #if TSS_DETECT_PORT_METHOD(A,GPIO) 
    /*no PORTA in KE02*/
    const    TSS_GPIO_METHOD_ROMDATA TSS_GPIO_PORTA_METHOD_ROMDATA_CONTEXT = {GPIO_MethodControl, (UINT32*) TSS_HW_TIMER_PTR, NULL/*(UINT32*) PORTA_BASE_PTR*/, (UINT32*) PTA_BASE_PTR, TSS_HW_TIMER_VECTOR-16}; 
    #endif
9. Comment out the /* Init GPIO settings */ in GPIO_MethodControl(UINT8 u8ElNum, UINT8 u8Command) function in       TSS_SensorGPIO.c file. No need to configure them in KE02.
10. Add the TSS_HWTimerIsr() in isr.h.
11. Modify the /* LED Control Macros */ in main.h file.
12. Take care of the TSI GPIO configuration in TSS_SystemSetup.h.
13. In main(),GPIOA_PIDR must be set as 0, or the GPIO can't be set as input mode.

Directory contents
==================
projects/TSSWDemo.c      Main program


Hardware environment
====================
 - Board: freedom FRDM-KE02Z, revA

 - Hyperterminal configuration:
    - Word Length = 7 Bits
    - One Stop Bit
    - No parity
    - BaudRate = 9600 baud
    - flow control: None 
  
  
How to use it
=============
In order to make the program work, you must do the following :
- Run the example
- Slide to changing the brightness of the LEDs


************************END OF FILE********************************************