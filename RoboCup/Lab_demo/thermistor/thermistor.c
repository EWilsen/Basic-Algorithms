/******************************************************************************
*
* Freescale Semiconductor Inc.
* (c) Copyright 2011-2012 Freescale Semiconductor, Inc.
* ALL RIGHTS RESERVED.
*
***************************************************************************//*!
*
* @file thermistor.h
*
* @author b34191
*
* @version 0.0.1
*
* @date Apr. 10, 2013
*
* @brief application entry point which performs application specific tasks. 
*
*******************************************************************************
*
* provide i32ChannelValue1 demo for thermistor application on frdm-ke02z platform 
* NOTE:
	
******************************************************************************/

#include "common.h"
#include "thermistor.h"
#include "tgmath.h"
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


/******************************************************************************
* Global functions
******************************************************************************/

/*****************************************************************************//*!
+FUNCTION----------------------------------------------------------------
* @function name: thermistor_init
*
* @brief 
*		 
* @param  none 
*
* @return none
*
* @ Pass/ Fail criteria: none
*****************************************************************************/
void Thermistor_init(void)
{                    
    SIM_SCGC |= SIM_SCGC_ADC_MASK;//enable adc clock gate
    SIM_SOPT &= ~SIM_SOPT_ADHWT_MASK;

    /*select PIt as hardware trigger*/
    SIM_SOPT |= SIM_SOPT_ADHWT(0x01);
    /*inital adc*/  
    ADC_APCTL1 = 0x3000;//enable ADC external ADP12,ADP13 pin
    ADC_SC2 |= ADC_SC2_ADTRG_MASK;//enable hardware trig mode, trig source is rtc flow           
    ADC_SC3 = ADC_SC3_ADIV_MASK | ADC_SC3_ADLSMP_MASK | ADC_SC3_MODE(0x02);  
    ADC_SC4 = ADC_SC4_AFDEP(0x02);//define fifo depth is 3-level.         
    ADC_SC1 = ADC_SC1_AIEN_MASK | ADC_SC1_ADCH(0x0C);//write adc channel ADP12
    ADC_SC1 = ADC_SC1_AIEN_MASK | ADC_SC1_ADCH(0x0D);//write adc channel ADP13     
    ADC_SC1 = ADC_SC1_AIEN_MASK | ADC_SC1_ADCH(0x16);//write adc channel Temperature Sensor       
}
/*****************************************************************************//*!
+FUNCTION----------------------------------------------------------------
* @function name: thermistor_test
*
* @brief 
*		 
* @param  none 
*
* @return none
*
* @ Pass/ Fail criteria: return temperature value*10
*****************************************************************************/
static uint8_t u8SampleCycle = 0;    
static  uint16_t u16TempData = 0;
static int32 i32ChannelValue1,i32ChannelValue2,i32ChannelValue3;
int16_t Thermistor_test(void)
{            
    float fThermistorData,fTemperatureValue;  
    uint32_t u32TemperatureValue;
    uint16_t u16IntegerSection,u16DecimalSection;    
    int32 i32TempValue;    
    
    if( (ADC_SC1&ADC_SC1_COCO_MASK)==0x80 )
    {
        u16TempData = ADC_R;                 
        i32ChannelValue1 += u16TempData;        
        u16TempData = ADC_R;          
        i32ChannelValue2 += u16TempData;              
        u16TempData = ADC_R;           
        i32ChannelValue3 += u16TempData;
        
        u8SampleCycle ++;
    }

    if( u8SampleCycle < 32 )
    {
        return 0x4FFF;
    }
    else
    {
        u8SampleCycle = 0;
    }
    /*calculate average value */
    i32ChannelValue1 = i32ChannelValue1>>5;        
    i32ChannelValue2 = i32ChannelValue2>>5;           
    i32ChannelValue3 = i32ChannelValue3>>5;     
    i32TempValue = (i32ChannelValue1-i32ChannelValue2);/*delta value*/     
    printf("Delta value is %d\n",i32TempValue);       
    i32TempValue *= 4990;  
    /*float operation*/
    printf("float type data\n");     
    fThermistorData = i32TempValue/i32ChannelValue2;/*get resistor value*/             
    u16IntegerSection = (int)(fThermistorData/1000);/*resistor integer section*/              
    u16DecimalSection =(int)(((fThermistorData/1000)- u16IntegerSection)*1000);/*resistor decimal section*/                    
    printf("The thermistor resistor value is %d.%dKOhm\n",u16IntegerSection,u16DecimalSection);              
    /*get the ambinent via the R=R0 expB (1/T-1/T0)*/             
    /*B=3380K*/   
    fThermistorData= (log((double)(fThermistorData)) - log((double)(10*1000)));
    fThermistorData = (fThermistorData*(273.15+25)+3380);
    fThermistorData = 3380*(273.15+25)/fThermistorData;
    fTemperatureValue=(fThermistorData-273.15); 
    u16IntegerSection = (int)fTemperatureValue;
    u16DecimalSection =(int)((fTemperatureValue-u16IntegerSection)*100);                 
    printf("The thermistor body temperature is %d.%d Centigrade\n\n",u16IntegerSection,u16DecimalSection);      

    /*integer operation*/
    printf("integer type data\n");
    i32ChannelValue1 = i32TempValue/i32ChannelValue2;   
    u16IntegerSection = (i32ChannelValue1/1000);
    u16DecimalSection = (i32ChannelValue1-u16IntegerSection*1000);     
    printf("The thermistor resistor value is %d.%dKOhm\n",u16IntegerSection,u16DecimalSection);
    /*get the ambinent via the R=R0 expB (1/T-1/T0)*/
    /*B=3380K*/   
    fThermistorData= (log((double)(i32ChannelValue1)) - log((double)(10*1000)));
    i32ChannelValue1=(int)(fThermistorData*1000);
    i32ChannelValue1 = (i32ChannelValue1*(273+25)+3380*1000);
    i32ChannelValue1 = (3380*1000*(273+25))/i32ChannelValue1;    
    u32TemperatureValue=(i32ChannelValue1-273);
    printf("The thermistor body temperature is %d Centigrade\n\n",u32TemperatureValue );  
                   
    /*reset the variable value*/

  	return (int16_t)(fTemperatureValue*10);

}
