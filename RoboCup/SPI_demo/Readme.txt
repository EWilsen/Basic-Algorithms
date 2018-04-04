/******************** (C) COPYRIGHT 2013 Freescale ********************
* File Name          : Readme.txt
* Author             : Freescale
* Date First Issued  : 04/16/2013 : Version 1.0
* Description        : Description of the SPI Example.
********************************************************************************
* History:
* 04/16/2013 : Version 1.0
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
This example provides a description of how  to use the SPI with hardware flow control.
Two freedom boards are needed, one acts as master, another as slave.

SPI master will send data from 0 to 254, and SPI slave should receive them correctly.

if receive failed, the uart1 will output string:
	"Error: SPI0 failed to write data"

if receive all data OK, the uart1 will output string:
	"SPI communication is success!"


Directory contents
==================
drivers/spi.h  Library Configuration file
drivers/spi.c  polling handlers
projects/SPI_demo.c      Main program

Hardware environment
====================
 - 2 Boards: freedom FRDM-KE02Z, revA
	
 - 4 cables. 
  
How to use it
=============
In order to make the program work, you must do the following :
- Connect the pin53, pin54, pin59 and pin 60 between two boards with cables.
- in spi.h, please define as "#define SPI_MASTER", 
		build the code and burn image to SPI master board.
- in spi.h, please comment the words "#define SPI_MASTER", 
		build the code and burn image to SPI slave board.
- first power on slave board, and then power on the master board.



************************END OF FILE********************************************
