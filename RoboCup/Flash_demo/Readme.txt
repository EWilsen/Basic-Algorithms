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
This example provides a description of how demo the flash features, whcih contains of flash
and EEPROM erase,program,verify etc. operation.
This Lab provide a way to input command and parameter to implement flash or EEPROM operation.
Please follow below guide to this Lab.
	- Open P&E Terminal Utility, configure properties as below followed by clicking on ¡°Open Serial Port¡±:
		Port: USB COM; Baud: 9600; Parity: None; Bits: 8
	- Enter ¡°help¡± in the command line beginning with ¡°CMD>¡±. The command list will be printed on Terminal.
	- Enter the command ¡°ev_fb 7000¡± to erase verify (blank check) the whole flash block containing address 
	  0x7000, where ¡°7000¡± is the address in hex in the flash block. The following message will be shown 
	  as a result of the command: ¡°EraseVerify flash block failed (i.e.,flash is not blank)!¡±. 
	  This is correct because the flash block contains code that is running.
	- Enter the command ¡°ev_eep 10000000¡± to erase verify (blank check) the whole EEPROM block containing
	  address 10000000. The following message will be shown as a result of this command: ¡°EraseVerify EEPROM
	  block success (EEPROM is blank)!¡±. This is correct because the EEPROM block has not been programmed.
	- Enter the command ¡°ev_fs 7000 10¡± to erase verify (blank check) the flash section starting from 0x7000
	  with size of 10 longwords. The following message will be shown as a result of the command: ¡°EraseVerify 
	  flash section success (flash section is blank)!¡±  This is correct because this flash section has not been programmed.
	- Enter the command ¡°ev_fs 7000 100¡± to erase verify (blank check) the flash section starting from 0x7000¡± with 
	  size of 100 longwords. The following message will be shown as a result of the command: ¡°EraseVerify flash section
	  failed (i.e.,flash section is not blank)!¡±  This is correct because this flash section has been programmed with the LAB code.
	- Enter the command ¡°ev_eeps 10000000 100¡± to erase verify (blank check) the EEPROM section starting from 0x10000000 
	  with size of 100 bytes. The following message will be shown as a result of the command: ¡°EraseVerify EEPROM section success 
	  (EEPROM section is blank)!¡±  This is correct because this EEPROM section has not been programmed.
	- Enter the command ¡°ev_all¡± to erase verify (blank check) all blocks including the flash block and EEPROM block. 
	  The following message will be shown as a result of the command: ¡°EraseVerify all blocks failed (i.e.,all blocks are not blank)!¡±  
	  This is correct because the flash block has been programmed with the LAB code.
	- Enter the command ¡°pg_eep 10000000 1 2 3 4 5 6 7 8¡± to program EEPROM block starting at address 0x10000000 with 
	  data bytes 1,2,3,4,5,6,7 and 8.  The following message will be shown as a result of the command: ¡°Program EEPROM  success!¡± 
	- Enter the command ¡°pg_f 7000 1 2 3 4 5 6 7 8 255 256¡± to program flash block starting at address 0x7000 with data
	  bytes 1,2,3,4,5,6,7,8,255 and 256.  The following message will be shown as a result of the command: ¡°Program flash success!¡± 
	- Enter the command ¡°ev_eeps 10000000 8¡± to erase verify (blank check) the EEPROM section starting from 0x10000000 with 
	  size of 8 bytes. The following message will be shown as a result of the command: ¡°EraseVerify EEPROM section failed 
	  (i.e.,EEPROM section is not blank)!¡±  This is correct because this EEPROM section has been programmed in before step.
	- Enter the command ¡°ev_fs 7000 10¡± to erase verify (blank check) the flash section starting from 0x7000 with size of 
	  10 longwords. The following message will be shown as a result of the command: ¡°EraseVerify flash section failed 
	  (i.e.,flash section is not blank)!¡±  This is correct because this flash section has been programmed in above step.
	- Enter the command ¡°ers_eeps 10000000¡± to erase the EEPROM sector at address 0x10000000. The following message
	  will be shown as a result of the command: ¡°Erase EEPROM sector success!¡±  
	- Enter the command "read_memory 400 16" to read 16 bytes data from the address 0x400.


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
- Download project "Flash_demo" to flash through USB(J6) interface.
- Run the example


************************END OF FILE********************************************
