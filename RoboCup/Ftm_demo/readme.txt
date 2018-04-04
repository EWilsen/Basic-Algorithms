/******************** (C) COPYRIGHT 2013 Freescale ********************
* File Name          : Readme.txt
* Author             : Freescale
* Date First Issued  : 04/12/2013 : Version 1.0
* Description        : Description of the configuration of FTM module.
********************************************************************************
* History:
* 5/8/2013 : Version 1.0
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
This demo gives a detail show of ftm module configuration, the functions covers most of the module functions.

Directory contents
==================
drivers/ftm/ftm.c        ftm functions
drivers/ftm/ftm.h        functions prototype and definitions
projects/Ftm_demo.c      Main program


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
- follow the instructions displayed
- check the PWM by OSC, PTC0 ~ PT3(J10 pin 2,4,6,8)


************************END OF FILE********************************************