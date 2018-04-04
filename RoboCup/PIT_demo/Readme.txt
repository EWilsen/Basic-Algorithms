/******************** (C) COPYRIGHT 2013 Freescale ********************
* File Name          : Readme.txt
* Author             : Freescale
* Date First Issued  : 04/25/2013 : Version 1.0
* Description        : Description of the PITExample.
********************************************************************************
* History:
* 04/25/2013 : Version 1.0
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
This example provides a description of how  to use the PIT with automatic reload overflow interrupt.
The blue LED on the FRDM board will be toggled every 1 second by PIT timer0 overflow interrupt.
Also, the blue LED can be toggled manually by entering any key.


Directory contents
==================
drivers/PIT.h  Library Configuration file
drivers/PIT.c  PIT module intiation and PIT interrupt process
projects/PIT_demo.c      Main program

Hardware environment
====================
 - 1 Board: freedom FRDM-KE02Z, revA
	
  
How to use it
=============
In order to make the program work, you must do the following :
- Connect usb cable between FRDM board and PC usb port.


************************END OF FILE********************************************
