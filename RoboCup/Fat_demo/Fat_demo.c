
/******************************************************************************
*
* Freescale Semiconductor Inc.
* (c) Copyright 2011-2012 Freescale Semiconductor, Inc.
* ALL RIGHTS RESERVED.
*
*******************************************************************************
*
* @file Lab_demo.c
*
* @author a13984
*
* @version 0.0.1
*
* @date Jul-15-2011
*
* @brief providing framework of test cases for MCU. 
*
*******************************************************************************/

#include "common.h"
#include "rtc.h"
#include "pit.h"
#include "irda.h"
#include "mma8451_test.h"
#include "TSS_DataTypes.h"
#include "app_init.h"
#include "events.h"
#include "TSS_API.h"
#include "thermistor.h"
/******************************************************************************
* Global variables
******************************************************************************/

/******************************************************************************
* Constants and macros
******************************************************************************/
enum
{
	DemoIdle = 0,
	DemoIrDa,
	DemoAccelerometer,
	DemoThermistor,
//	DemoSPICommunication,
	DemoRTC,
	DemoEnd,
}DEMO_STATE;

enum
{
	LED_Auto = 0,
	LED_Manual,
	LED_Rtc,
	LED_Idle,
}LED_ControlMode;

#define ASLIDER_DELAY_TIMER		50
/******************************************************************************
* Local types
******************************************************************************/

/******************************************************************************
* Local function prototypes
******************************************************************************/
void DemoProcessing( void );
void TSS1_fCallBack0(TSS_CONTROL_ID u8ControlId);
void TSS_fOnFault(UINT8 electrode_number);
/******************************************************************************
* Local variables
******************************************************************************/

uint8_t u8DemoState = DemoRTC;
uint8_t u8IrDaTestStatus = 0;
uint8_t u8LedControlMode = 0;
uint8_t u8CurrentDemoState;
uint8_t u8DemoModeUpdatedDelay = 0;
uint8_t u8IrDATestDelay = 0;
uint16_t u16Temperature;
uint16_t u16LedCycleCount = 0;
/******************************************************************************
* Local functions
******************************************************************************/
int main (void);
void RTC_Task(void);
/******************************************************************************
* Global functions
******************************************************************************/


/********************************************************************/

/*****************************************************************************//*!
+FUNCTION----------------------------------------------------------------
* @function name: main
*
* @brief 
*        
* @param  none
*
* @return none
*
* @ Pass/ Fail criteria: none
*****************************************************************************/
int main(void)
{

    int16_t iTemperature;
    
    printf("FRDM-KE02Z Demo Application\n");
    RTC_SetupTimerCallback(RTC_Task);
    RTC_Init(RTC_CLKSRC_1KHZ,1, RTC_CLK_PRESCALER_100);
    
    /* initialize LED by FTM driving */
    LED_DriveByFtm();
    
    /* initialize TSS for Alisder */
    GPIOA_PIDR = 0x0; 
    TSS_Init_ASlider();
    printf("FRDM-KE02Z Demo Application\n");
    /* initialize Irda interface */
    IrDA_Init();
    
	/* initialize IIC and MMA8451 for accerometer test */
	AccelInit();
    
    /* initialize PIT for perodic timer - 10mS */
	PIT_Init(0,PIT_MODULO);
    PIT_SetupCallBack(Pit0CallBack);
    /* initialize thermistor for temperature measuring */
    Thermistor_init();

    u8LedControlMode = LED_Auto;
    
    /* LED blue and green cycle 10 time*/
    u16LedCycleCount = 0;
    LED_BlueControl(50,50);
    LED_GreenControl(0,50);
    while( u16LedCycleCount < 50 );
    LED_GreenControl(50,50);
    while( u16LedCycleCount < 3*100 );
    LED_GreenControl(0,0);
    LED_BlueControl(0,0);
    u8CurrentDemoState = u8DemoState;
    printf("FRDM-KE02Z Demo Application\n");
   	for(;;) {	
   				if( !u8IrDATestDelay )
   				{
	                 if( IrDA_Test() == 0 )
	                 {
	                 	printf("IrDA test success!\n");
	                 }
                     else
                     {
                        // printf("IrDA test fail!\n");
                     }
	                 u8IrDATestDelay = 30;
                }

				AccelDemo();

				if( u8CurrentDemoState != u8DemoState )
				{
					printf("Slip or touch the Aslider!\n");
					u8CurrentDemoState = u8DemoState;
				}

				iTemperature = Thermistor_test();

				if( ( iTemperature > 0 )&&(iTemperature < 400) )
				{
					printf("Thermistor test ok!\n");
				}
                else
                {
                    //printf("Thermistor test fail!\n");
                }
            }
    return 0;
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
static uint32_t u32CurrentTime = 0;
void RTC_Task(void)
{
	
    u32CurrentTime ++;

    if( u8LedControlMode == LED_Rtc )
    {
	    if( u32CurrentTime & 0x01 )
	    {
	    	SET_LED_BLUE(100);
	    }
	    else
	    {
	    	SET_LED_BLUE(0);
	    }

	    if( (u32CurrentTime % 10) == 0 )
	    {
	    	SET_LED_GREEN(100);
	    }
	    else if( (u32CurrentTime % 5) == 0 )
	    {
	    	SET_LED_GREEN(0);
	    }

	}
}

void Pit0CallBack( void )
{
	if( u16TiltDelay )
	{
		u16TiltDelay--;
	}

	if( u8IrDATestDelay )
	{
		u8IrDATestDelay --;
	}

	/* LED control */
	if( u8LedControlMode == LED_Auto )
	{
		LED_CallBack();
        u16LedCycleCount++;
	}

	/* determine demo updated speed */
	if( u8DemoModeUpdatedDelay )
	{
		u8DemoModeUpdatedDelay --;
	}
    
    /* run TSS task to check slider */
    TSS_Task();
    
}


/*
** ===================================================================
**     Event       :  TSS1_fCallBack4 (module Events)
**
**     Component   :  TSS1 [TSS_Library]
**     Description :
**         Callback definition for Control 1. This event is enabled
**         only if Control 3 is enabled.
**         The default CallBack Name is automatically generated with
**         automatic prefix update by current Component Name. User can
**         define own name, but then the automatic name update is not
**         functional.
**     Parameters  :
**         NAME            - DESCRIPTION
**         u8ControlId     - Valid unique Identifier of
**                           the Control which generated the CallBack
**                           function. This Id can be used for finding
**                           of Callback's source Control.
**     Returns     : Nothing
** ===================================================================
*/

void TSS1_fCallBack1(TSS_CONTROL_ID u8ControlId)
{
        /* Set LED brightness */
	if( cASlider1.DynamicStatus.Direction )
	{
		if( !u8DemoModeUpdatedDelay )
		{
			u8DemoState ++;
			if(u8DemoState > DemoEnd )
			{
				u8DemoState = DemoIrDa;
			}
			u8DemoModeUpdatedDelay = ASLIDER_DELAY_TIMER;
		}
	}
	else
	{
		if( !u8DemoModeUpdatedDelay )
		{
			if( u8DemoState == DemoIdle )
			{
				u8DemoState = DemoEnd;
			}

			u8DemoState --;
			u8DemoModeUpdatedDelay = ASLIDER_DELAY_TIMER;
		}
	}

	SET_LED_GREEN(cASlider1.Position);

  (void) u8ControlId;
  
  return;
}


/*
** ===================================================================
**     Event       :  TSS1_fOnFault (module Events)
**
**     Component   :  TSS1 [TSS_Library]
**     Description :
**         This callback function is called by TSS after Fault
**         occurence. This event is enabled always and depends on
**         selection 'generate code' if the callback is used.
**         The default CallBack Name is automatically generated with
**         automatic prefix update by current Component Name. User can
**         define own name, but then the automatic name update is not
**         functional.
**     Parameters  : UINT8
**     Returns     : Nothing
** ===================================================================
*/
void TSS_fOnFault(UINT8 electrode_number)
{
    if(tss_CSSys.Faults.ChargeTimeout || tss_CSSys.Faults.SmallCapacitor) /* If large or small capacitor fault  */
    {
      (void) TSS_SetSystemConfig(System_Faults_Register, 0x00);           /* Clear the fault flag */
    }

    if(tss_CSSys.Faults.SmallTriggerPeriod)
    {
      (void) TSS_SetSystemConfig(System_Faults_Register, 0x00);           /* Clear the fault flag */

    #if APP_TSS_USE_DCTRACKER
        /* Enables the TSS. Enables the DC Tracking feature. Default DC Tracking value is 0x64 */
        (void)TSS_SetSystemConfig(System_SystemConfig_Register,(TSS_SYSTEM_EN_MASK | TSS_DC_TRACKER_EN_MASK));
    #else
        /* Enables the TSS */
        (void)TSS_SetSystemConfig(System_SystemConfig_Register,(TSS_SYSTEM_EN_MASK));
    #endif
    }
}