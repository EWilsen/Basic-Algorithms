
/******************************************************************************
*
* Freescale Semiconductor Inc.
* (c) Copyright 2011-2012 Freescale Semiconductor, Inc.
* ALL RIGHTS RESERVED.
*
*******************************************************************************
*
* @file PIT_demo.c
*
* @author b38368
*
* @version 0.0.1
*
* @date Apr-25-2013
*
* @brief blue LED on FRDM board will be toggled by PIT timer0 overflow interrupt 
*
*******************************************************************************/

#include "common.h"
#include "pit.h"

int main (void);
void Pit0CallBack(void);
/******************************************************************************
+FUNCTION----------------------------------------------------------------
* @function name: main
*
* @brief: execute main loop
*		 
* @param  none 
*
* @return none
*
* @ Pass/ Fail criteria: none
*****************************************************************************/
int main (void)
{
	char ch;     

  	printf("\nRunning the PIT_demo project.\n");
    
    /* initial PIT timer0 1 second overflow interrupt */
    /* Timer0 needs 1 second*20MHz=20,000,000 cycles, given 20MHz Bus clock */
    PIT_Init(0, 0x1312D00);    
    PIT_SetupCallBack(Pit0CallBack); 
    /* initiate blue led, which indicates PIT timer0 interrupt by toggle */
    LED2_Init();            

    while(1)
	{
        /* blue led can also be toggled manually by entering any key */
		ch = in_char();
		out_char(ch);
        LED2_Toggle();
	} 
}

void Pit0CallBack(void)
{
    /* toggle blue led, indicating PIT timer0 interrupt */
    LED2_Toggle();
}
