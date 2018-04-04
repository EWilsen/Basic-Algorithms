/******************** (C) COPYRIGHT 2013 Freescale ********************
* File Name          : Readme.txt
* Author             : Freescale
* Date First Issued  : 04/12/2013 : Version 1.0
* Description        : Description of the RTC Example.
********************************************************************************
* History:
* 04/15/2013 : Version 1.1
* 04/12/2013 : Version 1.0
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
This example provides a description of how  to use the RTC as calendar function with triggering ADC module.
Firstly, the RTC start up and alarm time is set and RTC is configured to print real time information every second. 
Then, RTC will increase the time each second and compare to the alarm time.
When RTC real time and alarm time is equal, the RTC triggering ADC function is set, subsequently ADC is triggered by RTC correctly.


The RTC is configured as follow:
  - Real-time clock source is 1 kHz
  - RTCSC divideris 1000
  - Real-time interrupt requests are enabled
  - RTC Modulo 0x00


Directory contents
==================
drivers/rtc.h  Library Configuration file
drivers/rtc.c  Interrupt handlers
projects/RTC_demo.c      Main program


Hardware environment
====================
 - Board: freedom FRDM-KE02Z, revA

  
How to use it
=============
In order to make the program work, you must do the following :
- Connect J6 USB port to the computer, open the serial terminal
- Run the example


************************END OF FILE********************************************
