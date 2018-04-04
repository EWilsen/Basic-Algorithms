/******************** (C) COPYRIGHT 2013 Freescale ********************
* File Name          : Readme.txt
* Author             : Freescale
* Date First Issued  : 05/02/2013 : Version 1.0
* Description        : Fast GPIO single core clock cycle example.
********************************************************************************
* History:
* 05/02/2013 : Version 1.0
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
The GPIO registers are also aliased to the FastGPIO interface on the Cortx-M0+ from address 0xF800_0000. Access via the FastGPIO interface occur in parallel with any instruction fetches and will therefore complete in a single cycle.
This example provides a demonstration of Fast GPIO access. Toggle GPIO PTB2 via Fast GPIO register, the toggle frequency can be as fast as 10MHz, while the core clock (not the bus clock) is 20MHz, indicating the Fast GPIO access in a single cycle.
NOTE that the optimization option must be "level high" in C compile configuration.

Directory contents
==================
projects/FastGPIO_demo.c      Main program

Hardware environment
====================
 - 1 Board: freedom FRDM-KE02Z, revA
	
  
How to use it
=============
In order to make the program work, you must do the following :
- Connect usb cable between FRDM board and PC usb port.


************************END OF FILE********************************************
