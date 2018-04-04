/******************** (C) COPYRIGHT 2013 Freescale ********************
* File Name          : Readme.txt
* Author             : Freescale
* Date First Issued  : 04/18/2013 : Version 1.0
* Description        : Description of the ADC Example.
********************************************************************************
* History:
* 04/18/2013 : Version 1.0
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
This example provides a description of how  to configure the ADC module to FIFO mode or no FIFO mode.
An example is given in the main.c, FIFO is used and the internal temperauture, bandgap and Vref value 
are sampled and displayed in the isr.


Directory contents
==================
drivers/adc.h            Library Configuration file
drivers/adc.c            Interrupt handlers
projects/Adc_demo.c      Main program


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
- Connect J6 USB port to the computer, open the serial terminal
- Run the example and follow the instruction displayed


************************END OF FILE********************************************
