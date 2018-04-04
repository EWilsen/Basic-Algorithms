
/******************************************************************************
*
* Freescale Semiconductor Inc.
* (c) Copyright 2011-2012 Freescale Semiconductor, Inc.
* ALL RIGHTS RESERVED.
*
*******************************************************************************
*
* @file FastGPIO_demo.c
*
* @author freescale
*
* @version 0.0.1
*
* @date Jul-15-2011
*
* @brief 
*   The GPIO registers are also aliased to the FastGPIO interface on the Cortx-M0+ from address 
*   0xF800_0000. Access via the FastGPIO interface occur in parallel with any instruction fetches and 
*   will therefore complete in a single cycle.
*   This example provides a demonstration of Fast GPIO access. Toggle GPIO PTB2 via Fast GPIO 
*   register, the toggle frequency can be as fast as 10MHz, while the core clock (not the bus clock) 
*   is 20MHz, indicating the Fast GPIO access in a single cycle.
*   NOTE that the optimization option must be "level high" in C compile configuration.
*******************************************************************************/

#include "common.h"
#include "rtc.h"


/******************************************************************************
* Global variables
******************************************************************************/

/******************************************************************************
* Constants and macros
******************************************************************************/

/******************************************************************************
* Local types
******************************************************************************/

/******************************************************************************
* Local function prototypes
******************************************************************************/

/******************************************************************************
* Local variables
******************************************************************************/

/******************************************************************************
* Local functions
******************************************************************************/
int main (void);
void RTC_Task(void);


/******************************************************************************
* Global functions
******************************************************************************/


/********************************************************************/
int main (void)
{  

  	printf("\nRunning the FastGPIO_demo project.\n");
    printf("\nBy default Core Clock is 20MHz");
    printf("\nBy Fast GPIO access, PTB2 output frequency is expected to be 10MHz");
    
    SIM_SOPT |= SIM_SOPT_CLKOE_MASK;    //Enable Bus Clock Out on PTH2
    //ICS_C2 |= ICS_C2_BDIV(0);
    //SIM_BUSDIV = 0;
    //SIM_SOPT &= (~SIM_SOPT_BUSREF(7));

    //config GPIO PTB2 as a general-purpose output
    FGPIOA_PDDR |= (4<<8);

    //set PTB2 output 0
    FGPIOA_PCOR = (4<<8);
    
    for(;;)
    {
#if 1
        /* toggle PTB2 by FastGPIO, the frequency is CoreClock/2 */
        FGPIOA_PTOR = 0x400;
        FGPIOA_PTOR = 0x400; 
        FGPIOA_PTOR = 0x400; 
        FGPIOA_PTOR = 0x400; 
        FGPIOA_PTOR = 0x400; 
        FGPIOA_PTOR = 0x400; 
        FGPIOA_PTOR = 0x400; 
        FGPIOA_PTOR = 0x400; 
        FGPIOA_PTOR = 0x400; 
        FGPIOA_PTOR = 0x400; 
        FGPIOA_PTOR = 0x400; 
        FGPIOA_PTOR = 0x400; 
        FGPIOA_PTOR = 0x400; 
        FGPIOA_PTOR = 0x400; 
        FGPIOA_PTOR = 0x400; 
        FGPIOA_PTOR = 0x400; 
        FGPIOA_PTOR = 0x400; 
        FGPIOA_PTOR = 0x400; 
        FGPIOA_PTOR = 0x400;
        FGPIOA_PTOR = 0x400;
        FGPIOA_PTOR = 0x400; 
        FGPIOA_PTOR = 0x400; 
        FGPIOA_PTOR = 0x400; 
        FGPIOA_PTOR = 0x400; 
        FGPIOA_PTOR = 0x400; 
        FGPIOA_PTOR = 0x400; 
        FGPIOA_PTOR = 0x400; 
        FGPIOA_PTOR = 0x400; 
        FGPIOA_PTOR = 0x400; 
        FGPIOA_PTOR = 0x400; 
        FGPIOA_PTOR = 0x400; 
        FGPIOA_PTOR = 0x400; 
        FGPIOA_PTOR = 0x400; 
        FGPIOA_PTOR = 0x400; 
        FGPIOA_PTOR = 0x400; 
        FGPIOA_PTOR = 0x400; 
        FGPIOA_PTOR = 0x400; 
        FGPIOA_PTOR = 0x400;
#else
        GPIOA_PTOR = 0x400;
        GPIOA_PTOR = 0x400;
        GPIOA_PTOR = 0x400;
        GPIOA_PTOR = 0x400;
        GPIOA_PTOR = 0x400;
        GPIOA_PTOR = 0x400;
        GPIOA_PTOR = 0x400;
        GPIOA_PTOR = 0x400;
        GPIOA_PTOR = 0x400;
        GPIOA_PTOR = 0x400;
        GPIOA_PTOR = 0x400;
        GPIOA_PTOR = 0x400;
        GPIOA_PTOR = 0x400;
        GPIOA_PTOR = 0x400;
        GPIOA_PTOR = 0x400;
        GPIOA_PTOR = 0x400;
        GPIOA_PTOR = 0x400;
        GPIOA_PTOR = 0x400;
        GPIOA_PTOR = 0x400;
        GPIOA_PTOR = 0x400;
        GPIOA_PTOR = 0x400;
        GPIOA_PTOR = 0x400;
        GPIOA_PTOR = 0x400;
        GPIOA_PTOR = 0x400;
        GPIOA_PTOR = 0x400;
        GPIOA_PTOR = 0x400;
        GPIOA_PTOR = 0x400;
        GPIOA_PTOR = 0x400;
        GPIOA_PTOR = 0x400;
        GPIOA_PTOR = 0x400;
        GPIOA_PTOR = 0x400;
        GPIOA_PTOR = 0x400;
        GPIOA_PTOR = 0x400;
        GPIOA_PTOR = 0x400;
        GPIOA_PTOR = 0x400;
        GPIOA_PTOR = 0x400;
        GPIOA_PTOR = 0x400;
        GPIOA_PTOR = 0x400;
        GPIOA_PTOR = 0x400;
        GPIOA_PTOR = 0x400;
        GPIOA_PTOR = 0x400;
        GPIOA_PTOR = 0x400;
        GPIOA_PTOR = 0x400;
        GPIOA_PTOR = 0x400;
        GPIOA_PTOR = 0x400;
        GPIOA_PTOR = 0x400;
        GPIOA_PTOR = 0x400;
        GPIOA_PTOR = 0x400;
#endif
    }
}

/*****************************************************************************//*!
+FUNCTION----------------------------------------------------------------
* @function name: RTC_Task
*
* @brief callback routine of RTC driver which does what you want to do at 
*        every RTC period.
*        
* @param  none
*
* @return none
*
* @ Pass/ Fail criteria: none
*****************************************************************************/

void RTC_Task(void)
{
    /* toggle LED1 */
    LED0_Toggle();
}
/********************************************************************/
