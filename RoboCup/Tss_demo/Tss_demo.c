
/******************************************************************************
*
* Freescale Semiconductor Inc.
* (c) Copyright 2011-2012 Freescale Semiconductor, Inc.
* ALL RIGHTS RESERVED.
*
*******************************************************************************
*
* @file KE02TSSWDemo.c
*
* @author B37811
*
* @version 1.0
*
* @date Apr-15-2013
*
* @brief providing framework of test cases for MCU. 
*
*******************************************************************************/

#include "common.h"
#include "rtc.h"
#include "TSS_API.h"
#include "app_init.h"
#include "events.h"
#include "main.h"


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
void FTM_Led_Init(void);

#ifndef OUT_LIBRARY
/********************************************************************/
int main (void)
{
    printf("\nRunning the KE02TSSWDemo project.\n");
    printf("\nSlide to change the brightness of the LEDs.\n");
    
    LED_DriveByFtm();
    /* Init HW */
    //InitPorts();
    /* Default TSS init */
    TSS_Init_ASlider();

    /* Main Loop */
    for(;;)
    {
    /* TSS Task */
    if (TSS_Task() == TSS_STATUS_OK)
    {
    }     
    /* Write your code here ... */
    }
}
#endif
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
    LED1_Toggle();
}

/********************************************************************/