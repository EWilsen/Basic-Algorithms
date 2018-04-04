/******************** (C) COPYRIGHT 2013 Freescale ********************
* File Name          : Readme.txt
* Author             : Freescale
* Date First Issued  : 04/12/2013 : Version 1.0
* Description        : Description of the ACMP Example.
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
This example provides a description of how  to use the ACMP a voltage comparing detector .
Firstly, the ACMP clock and module is enabled, negative input is set to be DAC output while positive input is set to be ACMP_IN0.
Then, ACMP interrup is enabled and will be triggered while there is rising edge on the ACMP output.
So ACMP negative input is set to be VDDA/2, there will be interrupt when ACMP positive input is higher than VDDA/2. 


The ACMP is configured as follow:
  - ACMP Negative Input Select DAC output, ACMP Positive Input Select external reference 0
  - DAC refrence voltage is VDDA, DAC output is 1/2 VDDA 
  - ACMP0 input is allowed
  - CMP is enabled, Enable the ACMP Interrupt,ACMP generates interrupt on rising edge



Directory contents
==================
drivers/acmp.h  Library Configuration file
drivers/acmp.c  Interrupt handlers
projects/ACMP_demo.c      Main program


Hardware environment
====================
 - Board: freedom FRDM-KE02Z, revA
  
How to use it
=============
In order to make the program work, you must do the following :
- Connect J6 USB port to the computer, open the serial terminal
- Connect J2-2 to the external analog input
- Run the example


************************END OF FILE********************************************
