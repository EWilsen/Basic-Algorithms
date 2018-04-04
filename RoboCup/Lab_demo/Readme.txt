/******************** (C) COPYRIGHT 2013 Freescale ********************
* File Name          : Readme.txt
* Author             : Freescale
* Date First Issued  : 04/12/2013 : Version 1.0
* Description        : Description of the Lab_demo Example.
********************************************************************************
* History:
* 04/17/2013 : Version 1.0
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
This example provides a description of how demo the lad running on the FRDM_KL02E boards,from the 
start on, it will print messsage to show all Lab message and indicate how to switch lab demo by slider
on boards.
1. TSS Alisder
	Slip the slider on the boards to switch Lab demo, for example, slip down to switch to next Lab, 
	and slip up to switch to last demo,and print message to indicate which Lab demo is running.
	TSS is added to this project by the way of library, we have configured it in 
	the other project(KE02TSSWDemo).
2. Accelerometer demo
	Read the data from accelerometer sensor part(MMA8451) through I2C bus and calculate angle change,
	then drive LED to change brightness by Flextimer.
	when you rotate board, blue and green light will change.
3. IrDA
	Uart0_TX output is modulated by FTM0 channel 0( frequency:38khz),then drive infrared transmiting LED.
	UART0_RX input signal is filtered by ACMP in1, then injected to UART0.
	The UART0 is configured as follow: 
	- Word Length = 7 Bits
  - One Stop Bit
  - No parity
  - BaudRate = 4800 baud
  - Hardware flow control disabled (RTS and CTS signals)
  - Receive and transmit enabled
  Code is implemented to transmit 16 byte, then check the data received, if success, green light blink,
  otherwise,blue light blink
4. Thermistor
	Sample the Thermistor through ADC, base on the formula of temperature to get condition temperature, 
	and show it to teminal and brightness of green light indicate temperature change.
5. RTC 
	In RTC demo mode,blue light blink with 2.5Hz frequency, and green light blink with 0.5hz frequency.
6. Idle mode
	In this mode,it don't run others Lab, only Slider is active,blue light bink with cycle of 100mS on and 500mS off.
	 
Directory contents
==================
projects/Lab_demo      


Hardware environment
====================
 - Board: freedom FRDM-KE02Z, revA
	
 - Connect a null-modem male/female RS232 cable between the J6 connector 
   and PC serial port.  

 - Hyperterminal configuration:
    - Word Length = 7 Bits
    - One Stop Bit
    - No parity
    - BaudRate = 9600 baud
    - flow control: None 
  
  
How to use it
=============
In order to make the program work, you must do the following :
- Download project "Lab_demo" to flash through USB(J6) interface.
- Run the example


************************END OF FILE********************************************
