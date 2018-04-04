
/******************************************************************************
*
* Freescale Semiconductor Inc.
* (c) Copyright 2013-2014 Freescale Semiconductor, Inc.
* ALL RIGHTS RESERVED.
*
*******************************************************************************
*
* @file RTC_demo.c
*
* @author Freescale
*
* @version 0.0.1
*
* @date Apr-23-2013
*
* @brief providing framework of test cases for MCU. 
*
*******************************************************************************/

#include "common.h"
#include "rtc.h"

/******************************************************************************
* Global variables
******************************************************************************/
RTC_TIME_STRUCT rtc_set_time = {30,59,23,0};
RTC_TIME_STRUCT alarm_set_time = {0,0,0,1};
extern uint8 RTC_Trigger_Flag;
extern uint8 RTC_AlarmFlag, RTC_TickFlag;
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
  	printf("\nRunning the RTC_demo project.\n");
    LED1_Init();
    RTC_SetTime(&rtc_set_time);
    RTC_SetAlarm(&alarm_set_time);
    RTC_SetupTimerCallback(RTC_Task);
    RTC_Init(RTC_CLKSRC_1KHZ,0, RTC_CLK_PRESCALER_1000);
	while(1)
	{
        if (RTC_TickFlag == 1)
        {
            RTC_TickFlag = 0;
            printf("\nRTC real time is %d-day : %d-hour : %d-minute : %d-second\n",real_time.days,real_time.hours,real_time.minutes,real_time.seconds);      
        }
        if (RTC_AlarmFlag == 1)
        {
            RTC_AlarmFlag = 0;
            printf("\nRTC alarmed in time %d-day : %d-hour : %d-minute : %d-second\n",real_time.days,real_time.hours,real_time.minutes,real_time.seconds); 
            RTC_ConfigADCTriggering();  
            printf("\nADC is configured for RTC overflow triggering.\n");
        }
        if(RTC_Trigger_Flag == 1)
         {
             
             if ((ADC_SC1 & ADC_SC1_COCO_MASK) != 0)
             {
                 RTC_Trigger_Flag = 0;
                 ADC_SC1 |= ADC_SC1_COCO_MASK;	
                 printf("\nsuccess to trig ADC, conversion result is ADC_R=0x%4x\n",ADC_R);               
             }      
         }
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
    LED1_Toggle();
}
/********************************************************************/